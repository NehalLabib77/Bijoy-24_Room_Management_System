-- Add BoarderRegistry table
CREATE TABLE IF NOT EXISTS `boarder_registry` (
    `BOARDER_NO` VARCHAR(20) NOT NULL,
    `NAME` VARCHAR(50) DEFAULT NULL,
    `STATUS` VARCHAR(10) DEFAULT 'Available',
    `STUDENT_ID` VARCHAR(10) DEFAULT NULL,
    PRIMARY KEY (`BOARDER_NO`)
);

-- Add BoarderNo column to students table
ALTER TABLE `students` ADD COLUMN `S_BOARDER_NO` VARCHAR(20) NOT NULL DEFAULT '';

-- Add AvailableSlots column to rooms table
ALTER TABLE `rooms` ADD COLUMN `R_AVAILABLE` INT DEFAULT 0;

-- Initialize AvailableSlots to RoomCapacity
UPDATE `rooms` SET `R_AVAILABLE` = `R_CAPACITY` WHERE `R_AVAILABLE` = 0;

INSERT INTO admin_users (USERNAME, PASSWORD_HASH, ROLE, CREATED_AT, IS_ACTIVE)
VALUES 
(
    'admin',
    'AgCOG7XvnxrRQVE1x6xGdKlRhSGD3rFCOPwQB1rYScNXWH4=',
    'SystemAdmin',
    NOW(),
    1
),
(
    'hallAdmin',
    'AgCOG7XvnxrRQVE1x6xGdKlRhSGD3rFCOPwQB1rYScNXWH4=',
    'HallAdmin',
    NOW(),
    1
);
