-- =====================================================
-- SYSTEM ADMIN CREATION SCRIPT
-- Database: hallmanagementdb
-- =====================================================
-- Run this script in XAMPP phpMyAdmin to create the initial System Admin
-- =====================================================

USE `hallmanagementdb`;

-- 1. Ensure admin_users table exists (it should from previous steps)
CREATE TABLE IF NOT EXISTS `admin_users` (
    `ADMIN_USER_ID` INT NOT NULL AUTO_INCREMENT,
    `USERNAME` VARCHAR(100) NOT NULL,
    `PASSWORD` VARCHAR(100) NOT NULL,
    `ROLE` VARCHAR(50) DEFAULT 'SystemAdmin',
    `CREATED_AT` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `IS_ACTIVE` TINYINT(1) DEFAULT 1,
    PRIMARY KEY (`ADMIN_USER_ID`),
    UNIQUE KEY `UK_USERNAME` (`USERNAME`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 2. Clear existing admins to start fresh (OPTIONAL - remove if you want to keep data)
-- TRUNCATE TABLE `admin_users`;

-- 3. Insert System Admin
-- Username: admin
-- Password: admin123
INSERT INTO `admin_users` (`USERNAME`, `PASSWORD`, `ROLE`, `IS_ACTIVE`) 
VALUES ('admin', 'admin123', 'SystemAdmin', 1);

-- 4. Verify
SELECT * FROM `admin_users`;
