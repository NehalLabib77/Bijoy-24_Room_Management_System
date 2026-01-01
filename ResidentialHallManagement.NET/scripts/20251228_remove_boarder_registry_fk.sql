-- ========================================
-- Remove BoarderRegistry StudentId Foreign Key Constraint
-- Date: 2025-12-28
-- Purpose: Allow system admin to create boarder registry entries
--          with student IDs that don't exist yet in the system
--          (pre-registration/allocation before student registers)
-- ========================================

USE hallmanagementdb;

-- Drop the foreign key constraint that prevents saving non-existent student IDs
ALTER TABLE `boarder_registry` 
DROP FOREIGN KEY IF EXISTS `FK_boarder_registry_students_STUDENT_ID`;

-- Verify the constraint is removed
SELECT 
    CONSTRAINT_NAME,
    TABLE_NAME,
    COLUMN_NAME,
    REFERENCED_TABLE_NAME,
    REFERENCED_COLUMN_NAME
FROM information_schema.KEY_COLUMN_USAGE
WHERE TABLE_SCHEMA = 'hallmanagementdb'
  AND TABLE_NAME = 'boarder_registry'
  AND CONSTRAINT_NAME LIKE 'FK_%';

SELECT 'Foreign key constraint removed successfully. BoarderRegistry.STUDENT_ID can now store any value.' AS Result;
