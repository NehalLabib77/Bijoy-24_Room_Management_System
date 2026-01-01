-- ============================================================
-- Migration: Create Seats and Seat Applications Tables
-- Date: December 28, 2025
-- Description: Creates tables for seat booking system
-- ============================================================

-- Create seats table
CREATE TABLE IF NOT EXISTS seats (
    seat_id INT AUTO_INCREMENT PRIMARY KEY,
    room_id VARCHAR(4) NOT NULL,
    hall_id INT NOT NULL,
    seat_number INT NOT NULL COMMENT '1-4 for each room',
    seat_type VARCHAR(20) NOT NULL DEFAULT 'Door' COMMENT 'Window or Door',
    status VARCHAR(20) NOT NULL DEFAULT 'Available' COMMENT 'Available, Booked, Reserved',
    booked_by_student_id VARCHAR(10) NULL,
    booking_date DATETIME NULL,
    seat_label VARCHAR(50) NULL,
    is_temporarily_held TINYINT(1) NOT NULL DEFAULT 0,
    held_by_student_id VARCHAR(10) NULL,
    held_until DATETIME NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,
    
    -- Indexes
    INDEX idx_seats_room (room_id),
    INDEX idx_seats_hall (hall_id),
    INDEX idx_seats_status (status),
    INDEX idx_seats_booked_by (booked_by_student_id),
    INDEX idx_seats_held_by (held_by_student_id),
    
    -- Unique constraint: each room can only have seats 1-4
    UNIQUE KEY uk_room_seat (room_id, hall_id, seat_number),
    
    -- Foreign keys
    CONSTRAINT fk_seats_room FOREIGN KEY (room_id, hall_id) REFERENCES rooms(room_id, hall_id) ON DELETE CASCADE,
    CONSTRAINT fk_seats_hall FOREIGN KEY (hall_id) REFERENCES hall(id) ON DELETE CASCADE,
    CONSTRAINT fk_seats_booked_by FOREIGN KEY (booked_by_student_id) REFERENCES students(s_id) ON DELETE SET NULL,
    CONSTRAINT fk_seats_held_by FOREIGN KEY (held_by_student_id) REFERENCES students(s_id) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create seat_applications table
CREATE TABLE IF NOT EXISTS seat_applications (
    application_id INT AUTO_INCREMENT PRIMARY KEY,
    student_id VARCHAR(10) NOT NULL,
    seat_id INT NOT NULL,
    hall_id INT NOT NULL,
    room_id VARCHAR(4) NOT NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'Pending' COMMENT 'Pending, Approved, Rejected, Cancelled',
    application_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    processed_date DATETIME NULL,
    student_remarks VARCHAR(500) NULL,
    admin_remarks VARCHAR(500) NULL,
    processed_by VARCHAR(255) NULL,
    academic_year VARCHAR(50) NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,
    
    -- Indexes
    INDEX idx_seat_apps_student (student_id),
    INDEX idx_seat_apps_seat (seat_id),
    INDEX idx_seat_apps_hall (hall_id),
    INDEX idx_seat_apps_room (room_id),
    INDEX idx_seat_apps_status (status),
    INDEX idx_seat_apps_date (application_date),
    
    -- Foreign keys
    CONSTRAINT fk_seat_apps_student FOREIGN KEY (student_id) REFERENCES students(s_id) ON DELETE CASCADE,
    CONSTRAINT fk_seat_apps_seat FOREIGN KEY (seat_id) REFERENCES seats(seat_id) ON DELETE CASCADE,
    CONSTRAINT fk_seat_apps_hall FOREIGN KEY (hall_id) REFERENCES hall(id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- Optional: Insert sample seats for existing rooms
-- This will create 4 seats per room (if rooms exist)
-- ============================================================
-- Uncomment the following to auto-generate seats for existing rooms:

-- INSERT INTO seats (room_id, hall_id, seat_number, seat_type, status)
-- SELECT 
--     r.id AS room_id,
--     r.hall_id,
--     s.seat_number,
--     CASE WHEN s.seat_number IN (1, 2) THEN 'Window' ELSE 'Door' END AS seat_type,
--     'Available' AS status
-- FROM rooms r
-- CROSS JOIN (
--     SELECT 1 AS seat_number UNION ALL
--     SELECT 2 UNION ALL
--     SELECT 3 UNION ALL
--     SELECT 4
-- ) s
-- ON DUPLICATE KEY UPDATE updated_at = CURRENT_TIMESTAMP;

-- ============================================================
-- Verification Queries
-- ============================================================
-- SELECT COUNT(*) as total_seats FROM seats;
-- SELECT COUNT(*) as total_applications FROM seat_applications;
-- SELECT seat_type, status, COUNT(*) as count FROM seats GROUP BY seat_type, status;
