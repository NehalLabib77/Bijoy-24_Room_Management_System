-- ========================================
-- Fix Boarder Registry Foreign Key Constraint
-- Date: 2025-12-26
-- Purpose: Make STUDENT_ID foreign key optional (nullable)
--          to allow boarder numbers without pre-assigned students
-- ========================================

USE hallmanagementdb;

-- Step 1: Drop existing foreign key constraint
ALTER TABLE `boarder_registry` 
DROP FOREIGN KEY IF EXISTS `FK_boarder_registry_students_STUDENT_ID`;

-- Step 2: Recreate the foreign key as ON DELETE SET NULL
-- This allows:
--   - STUDENT_ID to be NULL (boarder available for any student)
--   - STUDENT_ID to reference existing student (boarder pre-assigned)
--   - If student is deleted, STUDENT_ID automatically becomes NULL
ALTER TABLE `boarder_registry`
ADD CONSTRAINT `FK_boarder_registry_students_STUDENT_ID`
FOREIGN KEY (`STUDENT_ID`) 
REFERENCES `students`(`S_ID`)
ON DELETE SET NULL
ON UPDATE CASCADE;

-- Step 3: Verify the fix
SELECT 
    CONSTRAINT_NAME,
    TABLE_NAME,
    COLUMN_NAME,
    REFERENCED_TABLE_NAME,
    REFERENCED_COLUMN_NAME
FROM 
    INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE 
    TABLE_SCHEMA = 'hallmanagementdb'
    AND TABLE_NAME = 'boarder_registry'
    AND CONSTRAINT_NAME LIKE 'FK_%';

SELECT '✓ Foreign key constraint updated successfully!' AS Status;
