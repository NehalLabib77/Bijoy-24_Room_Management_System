-- Create the database
CREATE DATABASE IF NOT EXISTS `adminloginfo` 
CHARACTER SET utf8mb4 
COLLATE utf8mb4_general_ci;

-- Use the database
USE `adminloginfo`;

-- Drop existing table if exists (clean start)
DROP TABLE IF EXISTS `admin_login`;

-- Create simple admin login table with only username and password
CREATE TABLE `admin_login` (
    `username` VARCHAR(50) NOT NULL,
    `password` VARCHAR(50) NOT NULL,
    PRIMARY KEY (`username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Insert default admin credentials
INSERT INTO `admin_login` (`username`, `password`) VALUES
('admin', 'admin123'),
('halladmin', 'hall123');

-- Verify the data was inserted
SELECT * FROM `admin_login`;

-- Add default admin users to the admin_users table
-- Password hashing is done using PBKDF2

INSERT INTO admin_users (USERNAME, PASSWORD_HASH, ROLE, CREATED_AT, IS_ACTIVE)
VALUES 
(
    'admin',
    -- Password: admin123 (hashed with PBKDF2)
    'AgCOG7XvnxrRQVE1x6xGdKlRhSGD3rFCOPwQB1rYScNXWH4=',
    'SystemAdmin',
    NOW(),
    1
),
(
    'hallAdmin',
    -- Password: hall123 (hashed with PBKDF2)
    'AgCOG7XvnxrRQVE1x6xGdKlRhSGD3rFCOPwQB1rYScNXWH4=',
    'HallAdmin',
    NOW(),
    1
);

USE `hallmanagementdb`;

DROP TABLE IF EXISTS `admin_users`;

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

INSERT INTO `admin_users` (`USERNAME`, `PASSWORD`, `ROLE`, `IS_ACTIVE`) VALUES
('admin', 'admin123', 'SystemAdmin', 1),
('halladmin', 'hall123', 'HallAdmin', 1);
