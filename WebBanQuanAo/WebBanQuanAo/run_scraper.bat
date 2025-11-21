@echo off
echo ========================================
echo FASHION SCRAPER TOOL
echo ========================================

REM Check if Python is installed
python --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Python is not installed or not in PATH!
    echo.
    echo Please run: install_python.bat first
    echo.
    pause
    goto :end
)

echo Python found! Installing packages...
python -m pip install requests beautifulsoup4 lxml

echo.
echo Running Fashion Scraper...
python fashion_scraper.py

pause
:end