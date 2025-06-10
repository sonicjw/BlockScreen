# Screen Lock Preventer

## Overview
Screen Lock Preventer is a lightweight Windows utility that prevents your computer from automatically locking the screen or entering sleep mode while running. It runs in the system tray, allowing you to easily enable or disable the feature as needed.

## Features
- **Prevent Screen Lock**: Keep your computer awake without affecting screen savers
- **System Tray Operation**: Runs quietly in the background with minimal resource usage
- **Timer Function**: Set a specific duration to keep the screen unlocked
- **Auto-start Option**: Configure the application to launch at Windows startup
- **Easy Toggle**: Quickly enable or disable the feature through the system tray icon

## How to Use
1. **Launch the Application**: Run ScreenLockPreventer.exe
2. **Access Controls**: Right-click the system tray icon to access all functions
3. **Enable/Disable**: Select "Enable" to prevent screen locking or "Disable" to restore normal behavior
4. **Set Timer**: Choose "Set Timer..." to keep the screen unlocked for a specific duration
5. **Auto-start**: Toggle "Run on Startup" to control whether the application launches when Windows starts

## System Requirements
- Windows operating system
- .NET 8.0 Runtime
- Administrator privileges (for startup configuration)

## Technical Details
Screen Lock Preventer uses the Windows API function `SetThreadExecutionState()` to prevent the system from automatically entering sleep mode or turning off the display. It does not interfere with manually triggered screen locks (Win+L) or screen savers.

## License
This project is licensed under the MIT License - see the LICENSE file for details.
