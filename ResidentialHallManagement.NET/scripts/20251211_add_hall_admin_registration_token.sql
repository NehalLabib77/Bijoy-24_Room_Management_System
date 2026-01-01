-- Add registration token columns to hall_admins table
USE hallmanagementdb;

-- Add REGISTRATION_TOKEN column
ALTER TABLE hall_admins 
ADD COLUMN REGISTRATION_TOKEN VARCHAR(100) NULL,
ADD COLUMN IS_REGISTERED BOOLEAN DEFAULT FALSE;

-- Add index for faster token lookup
CREATE INDEX idx_registration_token ON hall_admins(REGISTRATION_TOKEN);

SELECT 'Migration completed: Added REGISTRATION_TOKEN and IS_REGISTERED columns to hall_admins table' AS status;
