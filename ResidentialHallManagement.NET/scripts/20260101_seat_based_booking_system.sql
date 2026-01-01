-- =====================================================
-- SEAT-BASED ROOM BOOKING SYSTEM
-- Created: 2026-01-01
-- Purpose: Implement seat-level booking with 4 seats per room
-- Seat Types: WINDOW_LEFT, WINDOW_RIGHT, DOOR_LEFT, DOOR_RIGHT
-- =====================================================

USE hallmanagementdb;

-- -----------------------------------------------------
-- Step 1: Update seats table to support specific seat types
-- -----------------------------------------------------
ALTER TABLE seats 
MODIFY COLUMN seat_type VARCHAR(20) NOT NULL DEFAULT 'WINDOW_LEFT'
COMMENT 'WINDOW_LEFT, WINDOW_RIGHT, DOOR_LEFT, DOOR_RIGHT';

-- Update seat_type constraint comment
ALTER TABLE seats 
MODIFY COLUMN status VARCHAR(20) NOT NULL DEFAULT 'Available'
COMMENT 'Available, Pending, Booked, Maintenance';

-- Add position column for seat layout
ALTER TABLE seats 
ADD COLUMN IF NOT EXISTS position VARCHAR(10) DEFAULT NULL 
COMMENT 'LEFT or RIGHT position within the type';

-- -----------------------------------------------------
-- Step 2: Add foreign key constraints if not exist
-- -----------------------------------------------------
-- Note: Foreign keys may already exist, so we use safe approach

-- Check and add foreign key for room reference
SET @fk_exists = (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS 
                  WHERE CONSTRAINT_SCHEMA = 'hallmanagementdb' 
                  AND TABLE_NAME = 'seats' 
                  AND CONSTRAINT_NAME = 'fk_seats_rooms');

SET @sql = IF(@fk_exists = 0, 
    'ALTER TABLE seats ADD CONSTRAINT fk_seats_rooms 
     FOREIGN KEY (room_id, hall_id) REFERENCES rooms(R_ID, H_ID) ON DELETE CASCADE',
    'SELECT 1');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Check and add foreign key for booked_by student
SET @fk_exists2 = (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS 
                   WHERE CONSTRAINT_SCHEMA = 'hallmanagementdb' 
                   AND TABLE_NAME = 'seats' 
                   AND CONSTRAINT_NAME = 'fk_seats_booked_student');

SET @sql2 = IF(@fk_exists2 = 0,
    'ALTER TABLE seats ADD CONSTRAINT fk_seats_booked_student 
     FOREIGN KEY (booked_by_student_id) REFERENCES students(S_ID) ON DELETE SET NULL',
    'SELECT 1');
PREPARE stmt2 FROM @sql2;
EXECUTE stmt2;
DEALLOCATE PREPARE stmt2;

-- -----------------------------------------------------
-- Step 3: Add unique constraint for one booking per seat
-- -----------------------------------------------------
-- This ensures only one student can be booked per seat
-- Note: Using trigger-based approach since MariaDB doesn't support partial indexes
-- The application logic will handle this primarily with triggers as backup

-- -----------------------------------------------------
-- Step 4: Update seat_applications table
-- -----------------------------------------------------
-- Add constraint for seat_id foreign key
SET @fk_exists3 = (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS 
                   WHERE CONSTRAINT_SCHEMA = 'hallmanagementdb' 
                   AND TABLE_NAME = 'seat_applications' 
                   AND CONSTRAINT_NAME = 'fk_seat_applications_seat');

SET @sql3 = IF(@fk_exists3 = 0,
    'ALTER TABLE seat_applications ADD CONSTRAINT fk_seat_applications_seat 
     FOREIGN KEY (seat_id) REFERENCES seats(seat_id) ON DELETE CASCADE',
    'SELECT 1');
PREPARE stmt3 FROM @sql3;
EXECUTE stmt3;
DEALLOCATE PREPARE stmt3;

-- Add constraint for student foreign key
SET @fk_exists4 = (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS 
                   WHERE CONSTRAINT_SCHEMA = 'hallmanagementdb' 
                   AND TABLE_NAME = 'seat_applications' 
                   AND CONSTRAINT_NAME = 'fk_seat_applications_student');

SET @sql4 = IF(@fk_exists4 = 0,
    'ALTER TABLE seat_applications ADD CONSTRAINT fk_seat_applications_student 
     FOREIGN KEY (student_id) REFERENCES students(S_ID) ON DELETE CASCADE',
    'SELECT 1');
PREPARE stmt4 FROM @sql4;
EXECUTE stmt4;
DEALLOCATE PREPARE stmt4;

-- -----------------------------------------------------
-- Step 5: Create trigger to prevent double booking
-- -----------------------------------------------------
DROP TRIGGER IF EXISTS trg_prevent_double_booking;

DELIMITER //
CREATE TRIGGER trg_prevent_double_booking
BEFORE UPDATE ON seats
FOR EACH ROW
BEGIN
    -- If trying to book a seat that's already booked
    IF NEW.status = 'Booked' AND OLD.status = 'Booked' 
       AND NEW.booked_by_student_id != OLD.booked_by_student_id THEN
        SIGNAL SQLSTATE '45000' 
        SET MESSAGE_TEXT = 'Seat is already booked by another student';
    END IF;
END//
DELIMITER ;

-- -----------------------------------------------------
-- Step 6: Create trigger to auto-reject other applications on approval
-- -----------------------------------------------------
DROP TRIGGER IF EXISTS trg_auto_reject_other_applications;

DELIMITER //
CREATE TRIGGER trg_auto_reject_other_applications
AFTER UPDATE ON seat_applications
FOR EACH ROW
BEGIN
    -- When an application is approved, reject all other pending applications for the same seat
    IF NEW.status = 'Approved' AND OLD.status = 'Pending' THEN
        UPDATE seat_applications 
        SET status = 'Rejected', 
            admin_remarks = 'Auto-rejected: Seat was assigned to another student',
            processed_date = NOW(),
            updated_at = NOW()
        WHERE seat_id = NEW.seat_id 
          AND application_id != NEW.application_id 
          AND status = 'Pending';
    END IF;
END//
DELIMITER ;

-- -----------------------------------------------------
-- Step 7: Create stored procedure to create seats for a room
-- -----------------------------------------------------
DROP PROCEDURE IF EXISTS sp_create_room_seats;

DELIMITER //
CREATE PROCEDURE sp_create_room_seats(
    IN p_room_id VARCHAR(4),
    IN p_hall_id INT
)
BEGIN
    DECLARE seat_count INT;
    
    -- Check if seats already exist for this room
    SELECT COUNT(*) INTO seat_count FROM seats WHERE room_id = p_room_id AND hall_id = p_hall_id;
    
    IF seat_count = 0 THEN
        -- Create 4 seats for the room
        INSERT INTO seats (room_id, hall_id, seat_number, seat_type, seat_label, status, position, created_at)
        VALUES 
            (p_room_id, p_hall_id, 1, 'WINDOW_LEFT', 'Window Side - Left', 'Available', 'LEFT', NOW()),
            (p_room_id, p_hall_id, 2, 'WINDOW_RIGHT', 'Window Side - Right', 'Available', 'RIGHT', NOW()),
            (p_room_id, p_hall_id, 3, 'DOOR_LEFT', 'Door Side - Left', 'Available', 'LEFT', NOW()),
            (p_room_id, p_hall_id, 4, 'DOOR_RIGHT', 'Door Side - Right', 'Available', 'RIGHT', NOW());
    END IF;
END//
DELIMITER ;

-- -----------------------------------------------------
-- Step 8: Create stored procedure for seat booking approval
-- -----------------------------------------------------
DROP PROCEDURE IF EXISTS sp_approve_seat_application;

DELIMITER //
CREATE PROCEDURE sp_approve_seat_application(
    IN p_application_id INT,
    IN p_processed_by VARCHAR(255),
    IN p_remarks VARCHAR(500),
    OUT p_success BOOLEAN,
    OUT p_message VARCHAR(255)
)
BEGIN
    DECLARE v_seat_id INT;
    DECLARE v_student_id VARCHAR(10);
    DECLARE v_seat_status VARCHAR(20);
    DECLARE v_room_id VARCHAR(4);
    DECLARE v_hall_id INT;
    
    -- Initialize output
    SET p_success = FALSE;
    SET p_message = '';
    
    -- Start transaction
    START TRANSACTION;
    
    -- Get application details
    SELECT seat_id, student_id, room_id, hall_id 
    INTO v_seat_id, v_student_id, v_room_id, v_hall_id
    FROM seat_applications 
    WHERE application_id = p_application_id AND status = 'Pending'
    FOR UPDATE;
    
    IF v_seat_id IS NULL THEN
        SET p_message = 'Application not found or already processed';
        ROLLBACK;
    ELSE
        -- Check seat status
        SELECT status INTO v_seat_status FROM seats WHERE seat_id = v_seat_id FOR UPDATE;
        
        IF v_seat_status = 'Booked' THEN
            SET p_message = 'Seat is already booked by another student';
            ROLLBACK;
        ELSE
            -- Update application status
            UPDATE seat_applications 
            SET status = 'Approved',
                processed_date = NOW(),
                processed_by = p_processed_by,
                admin_remarks = p_remarks,
                updated_at = NOW()
            WHERE application_id = p_application_id;
            
            -- Update seat status
            UPDATE seats 
            SET status = 'Booked',
                booked_by_student_id = v_student_id,
                booked_on = NOW(),
                is_temporarily_held = FALSE,
                held_by_student_id = NULL,
                held_until = NULL,
                updated_at = NOW()
            WHERE seat_id = v_seat_id;
            
            -- Update room available slots
            UPDATE rooms 
            SET R_AVAILABLE = GREATEST(0, R_AVAILABLE - 1)
            WHERE R_ID = v_room_id AND H_ID = v_hall_id;
            
            SET p_success = TRUE;
            SET p_message = 'Seat booking approved successfully';
            COMMIT;
        END IF;
    END IF;
END//
DELIMITER ;

-- -----------------------------------------------------
-- Step 9: Create view for seat applications with details
-- -----------------------------------------------------
DROP VIEW IF EXISTS v_seat_applications_details;

CREATE VIEW v_seat_applications_details AS
SELECT 
    sa.application_id,
    sa.student_id,
    s.S_NAME as student_name,
    s.S_DEPARTMENT as department,
    s.S_FACULTY as faculty,
    s.S_SEMESTER as student_semester,
    sa.seat_id,
    st.seat_number,
    st.seat_type,
    st.seat_label,
    st.status as seat_status,
    sa.room_id,
    r.R_NUMBER as room_number,
    r.R_FLOOR as floor,
    r.R_BLOCK as block,
    sa.hall_id,
    h.H_NAME as hall_name,
    sa.status as application_status,
    sa.application_date,
    sa.processed_date,
    sa.processed_by,
    sa.student_remarks,
    sa.admin_remarks,
    sa.academic_year,
    sa.semester as application_semester
FROM seat_applications sa
INNER JOIN students s ON sa.student_id = s.S_ID
INNER JOIN seats st ON sa.seat_id = st.seat_id
INNER JOIN rooms r ON st.room_id = r.R_ID AND st.hall_id = r.H_ID
INNER JOIN hall h ON sa.hall_id = h.H_ID;

-- -----------------------------------------------------
-- Step 10: Create seats for all existing rooms
-- -----------------------------------------------------
-- This will create 4 seats for each room that doesn't have seats yet

INSERT INTO seats (room_id, hall_id, seat_number, seat_type, seat_label, status, position, created_at)
SELECT 
    r.R_ID, 
    r.H_ID, 
    n.seat_num,
    CASE n.seat_num 
        WHEN 1 THEN 'WINDOW_LEFT'
        WHEN 2 THEN 'WINDOW_RIGHT'
        WHEN 3 THEN 'DOOR_LEFT'
        WHEN 4 THEN 'DOOR_RIGHT'
    END,
    CASE n.seat_num 
        WHEN 1 THEN 'Window Side - Left'
        WHEN 2 THEN 'Window Side - Right'
        WHEN 3 THEN 'Door Side - Left'
        WHEN 4 THEN 'Door Side - Right'
    END,
    'Available',
    CASE WHEN n.seat_num IN (1, 3) THEN 'LEFT' ELSE 'RIGHT' END,
    NOW()
FROM rooms r
CROSS JOIN (SELECT 1 as seat_num UNION SELECT 2 UNION SELECT 3 UNION SELECT 4) n
WHERE NOT EXISTS (
    SELECT 1 FROM seats s WHERE s.room_id = r.R_ID AND s.hall_id = r.H_ID
);

-- -----------------------------------------------------
-- Step 11: Show results
-- -----------------------------------------------------
SELECT 'Database migration completed successfully!' as Result;

SELECT 
    COUNT(DISTINCT CONCAT(room_id, '-', hall_id)) as total_rooms_with_seats,
    COUNT(*) as total_seats
FROM seats;

SELECT seat_type, COUNT(*) as count FROM seats GROUP BY seat_type;
