-- =====================================================
-- ADMIN USERS TABLE SETUP
-- Database: hallmanagementdb (main database)
-- Table: admin_users (username, password, role, etc.)
-- =====================================================
-- Run this script in XAMPP phpMyAdmin
-- =====================================================

-- Use the main database
USE `hallmanagementdb`;

-- Step 1: Drop existing table if exists (clean start)
DROP TABLE IF EXISTS `admin_users`;

-- Step 2: Create admin_users table
CREATE TABLE `admin_users` (
    `ADMIN_USER_ID` INT NOT NULL AUTO_INCREMENT,
    `USERNAME` VARCHAR(100) NOT NULL,
    `PASSWORD` VARCHAR(100) NOT NULL,
    `ROLE` VARCHAR(50) DEFAULT 'SystemAdmin',
    `CREATED_AT` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `IS_ACTIVE` TINYINT(1) DEFAULT 1,
    PRIMARY KEY (`ADMIN_USER_ID`),
    UNIQUE KEY `UK_USERNAME` (`USERNAME`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Step 3: Insert default admin credentials
INSERT INTO `admin_users` (`USERNAME`, `PASSWORD`, `ROLE`, `IS_ACTIVE`) VALUES
('admin', 'admin123', 'SystemAdmin', 1),
('halladmin', 'hall123', 'HallAdmin', 1);

-- Step 4: Verify the data was inserted
SELECT * FROM `admin_users`;

-- =====================================================
-- EXPECTED OUTPUT:
-- +---------------+------------+----------+-------------+---------------------+-----------+
-- | ADMIN_USER_ID | USERNAME   | PASSWORD | ROLE        | CREATED_AT          | IS_ACTIVE |
-- +---------------+------------+----------+-------------+---------------------+-----------+
-- | 1             | admin      | admin123 | SystemAdmin | 2025-12-11 12:00:00 | 1         |
-- | 2             | halladmin  | hall123  | HallAdmin   | 2025-12-11 12:00:00 | 1         |
-- +---------------+------------+----------+-------------+---------------------+-----------+
-- =====================================================

-- LOGIN CREDENTIALS:
-- System Admin: username = 'admin', password = 'admin123'
-- Hall Admin:   username = 'halladmin', password = 'hall123'
-- =====================================================
