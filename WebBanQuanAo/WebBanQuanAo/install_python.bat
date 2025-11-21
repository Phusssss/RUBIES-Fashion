@echo off
echo ========================================
echo PYTHON INSTALLATION CHECKER
echo ========================================

REM Check if Python is installed
python --version >nul 2>&1
if %errorlevel% == 0 (
    echo Python is already installed!
    python --version
    goto :install_packages
)

echo Python is not installed or not in PATH.
echo.
echo Please install Python from: https://www.python.org/downloads/
echo.
echo IMPORTANT: During installation, check "Add Python to PATH"
echo.
pause
goto :end

:install_packages
echo.
echo Installing required packages...
python -m pip install --upgrade pip
python -m pip install requests==2.31.0 beautifulsoup4==4.12.2 lxml==4.9.3

echo.
echo Installation complete!
echo You can now run: python fashion_scraper.py
echo.
pause

:end