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

# Save the SQL to temporary files
$tempSqlFile1 = [System.IO.Path]::GetTempFileName() + "_1.sql"
$tempSqlFile2 = [System.IO.Path]::GetTempFileName() + "_2.sql"
$cleanupSQL1 | Out-File -FilePath $tempSqlFile1 -Encoding UTF8
$cleanupSQL2 | Out-File -FilePath $tempSqlFile2 -Encoding UTF8

try {
    # Execute the SQL script using mysql CLI for main database
    Write-Host "Clearing $databaseName1..." -ForegroundColor Cyan
    
    if ($mysqlPassword) {
        mysql -h $mysqlHost -u $mysqlUser -p$mysqlPassword $databaseName1 < $tempSqlFile1
    }
    else {
        mysql -h $mysqlHost -u $mysqlUser $databaseName1 < $tempSqlFile1
    }
    
    # Execute the SQL script for identity database
    Write-Host "Clearing $databaseName2..." -ForegroundColor Cyan
    
    if ($mysqlPassword) {
        mysql -h $mysqlHost -u $mysqlUser -p$mysqlPassword $databaseName2 < $tempSqlFile2
    }
    else {
        mysql -h $mysqlHost -u $mysqlUser $databaseName2 < $tempSqlFile2
    }
    
    Write-Host ""
    Write-Host "✓ Databases cleared successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "All data has been deleted. Your project now looks fresh." -ForegroundColor Green
}
catch {
    Write-Host ""
    Write-Host "Error executing SQL: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Make sure:" -ForegroundColor Yellow
    Write-Host "  1. MySQL server is running" -ForegroundColor Yellow
    Write-Host "  2. mysql CLI is installed and in PATH" -ForegroundColor Yellow
    Write-Host "  3. Credentials are correct" -ForegroundColor Yellow
}
finally {
    # Clean up temporary files
    if (Test-Path $tempSqlFile1) {
        Remove-Item $tempSqlFile1
    }
    if (Test-Path $tempSqlFile2) {
        Remove-Item $tempSqlFile2
    }
}

Write-Host ""
Write-Host "===========================================" -ForegroundColor Cyan
