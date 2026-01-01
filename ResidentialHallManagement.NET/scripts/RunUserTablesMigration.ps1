# Run User Tables Migration Script
# Date: December 26, 2025

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "User Information Tables Migration" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Database connection details
$server = "localhost"
$database = "hallmanagementdb"
$username = "root"
$password = ""

Write-Host "Connecting to database: $database on $server" -ForegroundColor Yellow
Write-Host ""

# Prompt for password if not set
if ([string]::IsNullOrEmpty($password)) {
    $securePassword = Read-Host "Enter MySQL root password" -AsSecureString
    $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $password = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
}

# Path to the migration script
$scriptPath = Join-Path $PSScriptRoot "20251226_separate_user_tables.sql"

if (-not (Test-Path $scriptPath)) {
    Write-Host "Error: Migration script not found at $scriptPath" -ForegroundColor Red
    exit 1
}

Write-Host "Running migration script..." -ForegroundColor Green
Write-Host ""

# Run the migration using mysql command
try {
    if ([string]::IsNullOrEmpty($password)) {
        mysql -h $server -u $username $database -e "source $scriptPath"
    }
    else {
        $env:MYSQL_PWD = $password
        mysql -h $server -u $username $database -e "source $scriptPath"
        Remove-Item Env:\MYSQL_PWD
    }
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "Migration completed successfully!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Created tables:" -ForegroundColor Cyan
    Write-Host "  - system_admins" -ForegroundColor White
    Write-Host "  - hall_admins_info" -ForegroundColor White
    Write-Host "  - students_info" -ForegroundColor White
    Write-Host "  - user_activity_log" -ForegroundColor White
    Write-Host ""
    Write-Host "Created views:" -ForegroundColor Cyan
    Write-Host "  - v_all_active_users" -ForegroundColor White
    Write-Host "  - v_student_details" -ForegroundColor White
    Write-Host "  - v_hall_admin_details" -ForegroundColor White
    Write-Host ""
}
catch {
    Write-Host ""
    Write-Host "Error running migration:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}

Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
