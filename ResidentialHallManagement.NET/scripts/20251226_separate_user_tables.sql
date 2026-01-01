-- Migration Script: Separate User Information Tables
-- Date: December 26, 2025
-- Purpose: Create dedicated tables for System Admins, Hall Admins, and Students

-- ==================================================
-- 1. System Admins Table (Enhanced)
-- ==================================================
CREATE TABLE IF NOT EXISTS `system_admins` (
    `admin_id` INT AUTO_INCREMENT PRIMARY KEY,
    `user_id` VARCHAR(255) NOT NULL,
    `username` VARCHAR(100) NOT NULL UNIQUE,
    `email` VARCHAR(255) NOT NULL,
    `full_name` VARCHAR(200) NOT NULL,
    `phone` VARCHAR(20),
    `role` VARCHAR(50) DEFAULT 'SystemAdmin',
    `department` VARCHAR(100),
    `employee_id` VARCHAR(50),
    `is_active` BOOLEAN DEFAULT TRUE,
    `is_super_admin` BOOLEAN DEFAULT FALSE,
    `last_login` DATETIME,
    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `created_by` VARCHAR(100),
    `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `updated_by` VARCHAR(100),
    `profile_image_url` VARCHAR(500),
    `notes` TEXT,
    INDEX idx_user_id (user_id),
    INDEX idx_username (username),
    INDEX idx_email (email),
    INDEX idx_is_active (is_active)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- ==================================================
-- 2. Hall Admins Table (Enhanced)
-- ==================================================
CREATE TABLE IF NOT EXISTS `hall_admins_info` (
    `hall_admin_id` INT AUTO_INCREMENT PRIMARY KEY,
    `user_id` VARCHAR(255) NOT NULL,
    `hall_id` INT NOT NULL,
    `username` VARCHAR(100) NOT NULL UNIQUE,
    `email` VARCHAR(255) NOT NULL,
    `full_name` VARCHAR(200) NOT NULL,
    `phone` VARCHAR(20),
    `admin_role` VARCHAR(50) DEFAULT 'HallAdmin',
    `employee_id` VARCHAR(50),
    `designation` VARCHAR(100),
    `start_date` DATE,
    `end_date` DATE,
    `is_active` BOOLEAN DEFAULT TRUE,
    `registration_token` VARCHAR(100) UNIQUE,
    `is_registered` BOOLEAN DEFAULT FALSE,
    `last_login` DATETIME,
    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `created_by` VARCHAR(100),
    `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `updated_by` VARCHAR(100),
    `profile_image_url` VARCHAR(500),
    `address` TEXT,
    `emergency_contact` VARCHAR(100),
    `notes` TEXT,
    INDEX idx_user_id (user_id),
    FOREIGN KEY (`hall_id`) REFERENCES `hall`(`H_ID`) ON DELETE CASCADE,
    INDEX idx_username (username),
    INDEX idx_email (email),
    INDEX idx_hall_id (hall_id),
    INDEX idx_is_active (is_active),
    INDEX idx_registration_token (registration_token)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- ==================================================
-- 3. Students Table (Enhanced)
-- ==================================================
CREATE TABLE IF NOT EXISTS `students_info` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `user_id` VARCHAR(255) NOT NULL,
    `student_id` VARCHAR(50) NOT NULL UNIQUE,
    `username` VARCHAR(100) NOT NULL UNIQUE,
    `email` VARCHAR(255) NOT NULL,
    `full_name` VARCHAR(200) NOT NULL,
    `father_name` VARCHAR(200),
    `mother_name` VARCHAR(200),
    `date_of_birth` DATE,
    `gender` VARCHAR(20),
    `blood_group` VARCHAR(10),
    `religion` VARCHAR(50),
    `nationality` VARCHAR(50) DEFAULT 'Bangladeshi',
    `phone` VARCHAR(20),
    `emergency_contact` VARCHAR(100),
    `emergency_contact_name` VARCHAR(200),
    `emergency_contact_relation` VARCHAR(50),
    `permanent_address` TEXT,
    `present_address` TEXT,
    `boarder_no` VARCHAR(50),
    `faculty` VARCHAR(100) NOT NULL,
    `department` VARCHAR(100),
    `semester` INT NOT NULL,
    `session` VARCHAR(20),
    `cgpa` DECIMAL(4,2),
    `admission_date` DATE,
    `hall_id` INT,
    `room_id` INT,
    `status` VARCHAR(20) DEFAULT 'RUNNING',
    `is_active` BOOLEAN DEFAULT TRUE,
    `last_login` DATETIME,
    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `profile_image_url` VARCHAR(500),
    `guardian_info` TEXT,
    `medical_info` TEXT,
    `notes` TEXT,
    INDEX idx_user_id (user_id),
    FOREIGN KEY (`hall_id`) REFERENCES `hall`(`H_ID`) ON DELETE SET NULL,
    INDEX idx_student_id (student_id),
    INDEX idx_username (username),
    INDEX idx_email (email),
    INDEX idx_boarder_no (boarder_no),
    INDEX idx_faculty (faculty),
    INDEX idx_semester (semester),
    INDEX idx_status (status),
    INDEX idx_hall_id (hall_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- ==================================================
-- 4. User Activity Log (Tracking all user activities)
-- ==================================================
CREATE TABLE IF NOT EXISTS `user_activity_log` (
    `log_id` BIGINT AUTO_INCREMENT PRIMARY KEY,
    `user_id` VARCHAR(255) NOT NULL,
    `user_type` VARCHAR(50) NOT NULL, -- 'SystemAdmin', 'HallAdmin', 'Student'
    `action` VARCHAR(100) NOT NULL,
    `description` TEXT,
    `ip_address` VARCHAR(50),
    `user_agent` VARCHAR(500),
    `timestamp` DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_user_id (user_id),
    INDEX idx_user_type (user_type),
    INDEX idx_timestamp (timestamp),
    INDEX idx_action (action)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- ==================================================
-- 5. Migrate existing data (if any)
-- ==================================================

-- Migrate existing admin users to system_admins table
INSERT IGNORE INTO `system_admins` 
    (`user_id`, `username`, `email`, `full_name`, `role`, `is_active`, `created_at`)
SELECT 
    u.`Id`,
    u.`UserName`,
    u.`Email`,
    COALESCE(u.`FullName`, u.`UserName`),
    'SystemAdmin',
    TRUE,
    NOW()
FROM `hallmanagementidentitydb`.`aspnetusers` u
INNER JOIN `hallmanagementidentitydb`.`aspnetuserroles` ur ON u.`Id` = ur.`UserId`
INNER JOIN `hallmanagementidentitydb`.`aspnetroles` r ON ur.`RoleId` = r.`Id`
WHERE r.`Name` = 'Admin' 
AND u.`UserType` = 'Admin'
AND NOT EXISTS (SELECT 1 FROM `system_admins` sa WHERE sa.`user_id` = u.`Id`);

-- Migrate existing hall admins to hall_admins_info table
INSERT IGNORE INTO `hall_admins_info` 
    (`user_id`, `hall_id`, `username`, `email`, `full_name`, `admin_role`, `is_active`, `is_registered`, `created_at`)
SELECT 
    u.`Id`,
    COALESCE(u.`HallId`, 1),
    u.`UserName`,
    u.`Email`,
    COALESCE(u.`FullName`, u.`UserName`),
    'HallAdmin',
    TRUE,
    TRUE,
    NOW()
FROM `hallmanagementidentitydb`.`aspnetusers` u
INNER JOIN `hallmanagementidentitydb`.`aspnetuserroles` ur ON u.`Id` = ur.`UserId`
INNER JOIN `hallmanagementidentitydb`.`aspnetroles` r ON ur.`RoleId` = r.`Id`
WHERE r.`Name` = 'HallAdmin' 
AND u.`UserType` = 'HallAdmin'
AND NOT EXISTS (SELECT 1 FROM `hall_admins_info` ha WHERE ha.`user_id` = u.`Id`);

-- Migrate existing students to students_info table
INSERT IGNORE INTO `students_info` 
    (`user_id`, `student_id`, `username`, `email`, `full_name`, `boarder_no`, 
     `faculty`, `semester`, `status`, `is_active`, `created_at`)
SELECT 
    u.`Id`,
    COALESCE(u.`StudentId`, CONCAT('STU', LPAD(u.`Id`, 6, '0'))),
    u.`UserName`,
    u.`Email`,
    COALESCE(u.`FullName`, u.`UserName`),
    s.`S_BOARDER_NO`,
    COALESCE(s.`S_FACULTY`, 'Unknown'),
    COALESCE(s.`S_SEMESTER`, 1),
    COALESCE(s.`S_STATUS`, 'RUNNING'),
    TRUE,
    NOW()
FROM `hallmanagementidentitydb`.`aspnetusers` u
INNER JOIN `hallmanagementidentitydb`.`aspnetuserroles` ur ON u.`Id` = ur.`UserId`
INNER JOIN `hallmanagementidentitydb`.`aspnetroles` r ON ur.`RoleId` = r.`Id`
LEFT JOIN `students` s ON s.`S_ID` = u.`StudentId`
WHERE r.`Name` = 'Student' 
AND u.`UserType` = 'Student'
AND NOT EXISTS (SELECT 1 FROM `students_info` si WHERE si.`user_id` = u.`Id`);

-- ==================================================
-- 6. Create views for easy access
-- ==================================================

-- View: All Active Users
CREATE OR REPLACE VIEW `v_all_active_users` AS
SELECT 
    'SystemAdmin' AS user_type,
    sa.admin_id AS id,
    sa.user_id,
    sa.username,
    sa.email,
    sa.full_name,
    sa.phone,
    NULL AS hall_id,
    sa.is_active,
    sa.last_login,
    sa.created_at
FROM system_admins sa
WHERE sa.is_active = TRUE
UNION ALL
SELECT 
    'HallAdmin' AS user_type,
    ha.hall_admin_id AS id,
    ha.user_id,
    ha.username,
    ha.email,
    ha.full_name,
    ha.phone,
    ha.hall_id,
    ha.is_active,
    ha.last_login,
    ha.created_at
FROM hall_admins_info ha
WHERE ha.is_active = TRUE
UNION ALL
SELECT 
    'Student' AS user_type,
    si.id,
    si.user_id,
    si.username,
    si.email,
    si.full_name,
    si.phone,
    si.hall_id,
    si.is_active,
    si.last_login,
    si.created_at
FROM students_info si
WHERE si.is_active = TRUE;

-- View: Student Details with Hall Info
CREATE OR REPLACE VIEW `v_student_details` AS
SELECT 
    si.*,
    h.H_NAME AS hall_name,
    h.H_LOCATION AS hall_location,
    r.R_NUMBER AS room_number,
    r.R_NAME AS room_name,
    r.R_CAPACITY AS room_capacity
FROM students_info si
LEFT JOIN hall h ON si.hall_id = h.H_ID
LEFT JOIN rooms r ON si.room_id = r.R_ID AND si.hall_id = r.H_ID;

-- View: Hall Admin Details with Hall Info
CREATE OR REPLACE VIEW `v_hall_admin_details` AS
SELECT 
    ha.*,
    h.H_NAME AS hall_name,
    h.H_LOCATION AS hall_location,
    h.H_TYPE AS hall_type,
    h.H_CAPACITY AS hall_capacity
FROM hall_admins_info ha
LEFT JOIN hall h ON ha.hall_id = h.H_ID;

-- ==================================================
-- 7. Grant necessary permissions (adjust as needed)
-- ==================================================
-- GRANT SELECT, INSERT, UPDATE, DELETE ON system_admins TO 'your_app_user'@'localhost';
-- GRANT SELECT, INSERT, UPDATE, DELETE ON hall_admins_info TO 'your_app_user'@'localhost';
-- GRANT SELECT, INSERT, UPDATE, DELETE ON students_info TO 'your_app_user'@'localhost';
-- GRANT SELECT, INSERT, UPDATE, DELETE ON user_activity_log TO 'your_app_user'@'localhost';

SELECT 'Migration completed successfully!' AS status;
