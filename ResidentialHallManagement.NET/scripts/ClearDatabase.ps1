# Database Cleanup Script
# This script clears all data from the residential hall management system

Write-Host "===========================================" -ForegroundColor Cyan
Write-Host "Database Cleanup Script" -ForegroundColor Cyan
Write-Host "===========================================" -ForegroundColor Cyan
Write-Host ""

$databaseName1 = "hallmanagementdb"
$databaseName2 = "hallmanagementidentitydb"
$mysqlUser = "root"
$mysqlPassword = ""
$mysqlHost = "localhost"

Write-Host "Target Databases:" -ForegroundColor Green
Write-Host "  1. $databaseName1" -ForegroundColor Yellow
Write-Host "  2. $databaseName2" -ForegroundColor Yellow
Write-Host ""

Write-Host "WARNING: This will delete ALL data from both databases" -ForegroundColor Red
Write-Host ""

$confirmation = Read-Host "Type 'YES' to confirm deletion"

if ($confirmation -ne "YES") {
    Write-Host "Aborted." -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "Clearing databases..." -ForegroundColor Cyan

# Create the SQL cleanup commands for main database
$cleanupSQL1 = @"
-- Disable foreign key checks temporarily
SET FOREIGN_KEY_CHECKS = 0;

-- Delete all data from tables
TRUNCATE TABLE IF EXISTS maintenance_requests;
TRUNCATE TABLE IF EXISTS room_change_requests;
TRUNCATE TABLE IF EXISTS room_application_requests;
TRUNCATE TABLE IF EXISTS room_assignments;
TRUNCATE TABLE IF EXISTS assigned_room;
TRUNCATE TABLE IF EXISTS assigned_hall;
TRUNCATE TABLE IF EXISTS rooms;
TRUNCATE TABLE IF EXISTS boarder_registry;
TRUNCATE TABLE IF EXISTS students;
TRUNCATE TABLE IF EXISTS hall_admins;
TRUNCATE TABLE IF EXISTS admins;
TRUNCATE TABLE IF EXISTS hall;

-- Re-enable foreign key checks
SET FOREIGN_KEY_CHECKS = 1;

SELECT 'Main database cleared successfully!' AS result;
"@

# Create the SQL cleanup commands for identity database
$cleanupSQL2 = @"
-- Disable foreign key checks temporarily
SET FOREIGN_KEY_CHECKS = 0;

-- Delete all data from identity tables (except roles)
DELETE FROM AspNetUserRoles;
DELETE FROM AspNetUserClaims;
DELETE FROM AspNetUserLogins;
DELETE FROM AspNetUserTokens;
DELETE FROM AspNetUsers;

-- Re-enable foreign key checks
SET FOREIGN_KEY_CHECKS = 1;

SELECT 'Identity database cleared successfully!' AS result;
"@
