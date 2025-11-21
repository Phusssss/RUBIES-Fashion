@echo off
echo ========================================
echo DETAILED PRODUCT SCRAPER
echo ========================================

REM Check if Python is installed
python --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Python is not installed or not in PATH!
    echo Please run: install_python.bat first
    pause
    goto :end
)

REM Check if products_portable.json exists
if not exist "products_portable.json" (
    echo ERROR: File products_portable.json not found!
    echo Please run scraper_portable.py first to get product list
    pause
    goto :end
)

echo Running Detailed Product Scraper...
python detailed_scraper.py

pause
:end