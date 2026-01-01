-- Migration: Add Faculty/Semester if they don't exist and fix BoarderRegistry relationships
-- Run this SQL script manually or through EF Core migration

-- Check and add S_FACULTY column if it doesn't exist
SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = DATABASE() 
  AND TABLE_NAME = 'students' 
  AND COLUMN_NAME = 'S_FACULTY';

SET @sql = IF(@col_exists = 0,
    'ALTER TABLE students ADD COLUMN S_FACULTY varchar(50) NOT NULL DEFAULT ''Engineering''',
    'SELECT ''S_FACULTY column already exists'' AS Info');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Check and add S_SEMESTER column if it doesn't exist  
SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = DATABASE() 
  AND TABLE_NAME = 'students' 
  AND COLUMN_NAME = 'S_SEMESTER';

SET @sql = IF(@col_exists = 0,
    'ALTER TABLE students ADD COLUMN S_SEMESTER int NOT NULL DEFAULT 1',
    'SELECT ''S_SEMESTER column already exists'' AS Info');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Update empty or NULL BoarderNo values with temporary unique placeholders
UPDATE students 
SET S_BOARDER_NO = CONCAT('TEMP_', S_ID) 
WHERE S_BOARDER_NO IS NULL OR S_BOARDER_NO = '';

-- Create temporary boarder registry entries for existing students
INSERT IGNORE INTO boarder_registry (BOARDER_NO, NAME, STATUS, STUDENT_ID)
SELECT S_BOARDER_NO, S_NAME, 'Assigned', S_ID
FROM students
WHERE S_BOARDER_NO LIKE 'TEMP_%' OR S_BOARDER_NO NOT IN (SELECT BOARDER_NO FROM boarder_registry);

-- Check and create unique index on S_BOARDER_NO if it doesn't exist
SET @index_exists = 0;
SELECT COUNT(*) INTO @index_exists 
FROM INFORMATION_SCHEMA.STATISTICS 
WHERE TABLE_SCHEMA = DATABASE() 
  AND TABLE_NAME = 'students' 
  AND INDEX_NAME = 'IX_students_S_BOARDER_NO';

SET @sql = IF(@index_exists = 0,
    'CREATE UNIQUE INDEX IX_students_S_BOARDER_NO ON students(S_BOARDER_NO)',
    'SELECT ''Index IX_students_S_BOARDER_NO already exists'' AS Info');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Check and create index on boarder_registry.STUDENT_ID if it doesn't exist
SET @index_exists = 0;
SELECT COUNT(*) INTO @index_exists 
FROM INFORMATION_SCHEMA.STATISTICS 
WHERE TABLE_SCHEMA = DATABASE() 
  AND TABLE_NAME = 'boarder_registry' 
  AND INDEX_NAME = 'IX_boarder_registry_STUDENT_ID';

SET @sql = IF(@index_exists = 0,
    'CREATE INDEX IX_boarder_registry_STUDENT_ID ON boarder_registry(STUDENT_ID)',
    'SELECT ''Index IX_boarder_registry_STUDENT_ID already exists'' AS Info');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Check and add foreign key from boarder_registry to students if it doesn't exist
SET @fk_exists = 0;
SELECT COUNT(*) INTO @fk_exists 
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
WHERE TABLE_SCHEMA = DATABASE() 
  AND TABLE_NAME = 'boarder_registry' 
  AND CONSTRAINT_NAME = 'FK_boarder_registry_students_STUDENT_ID';

SET @sql = IF(@fk_exists = 0,
    'ALTER TABLE boarder_registry ADD CONSTRAINT FK_boarder_registry_students_STUDENT_ID FOREIGN KEY (STUDENT_ID) REFERENCES students(S_ID)',
    'SELECT ''FK_boarder_registry_students_STUDENT_ID already exists'' AS Info');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Check and add foreign key from students to boarder_registry if it doesn't exist
SET @fk_exists = 0;
SELECT COUNT(*) INTO @fk_exists 
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
WHERE TABLE_SCHEMA = DATABASE() 
  AND TABLE_NAME = 'students' 
  AND CONSTRAINT_NAME = 'FK_students_boarder_registry_S_BOARDER_NO';

SET @sql = IF(@fk_exists = 0,
    'ALTER TABLE students ADD CONSTRAINT FK_students_boarder_registry_S_BOARDER_NO FOREIGN KEY (S_BOARDER_NO) REFERENCES boarder_registry(BOARDER_NO) ON DELETE CASCADE',
    'SELECT ''FK_students_boarder_registry_S_BOARDER_NO already exists'' AS Info');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SELECT 'Migration completed successfully!' AS Result;
