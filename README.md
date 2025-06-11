# ScreenLockPreventer

ScreenLockPreventer is a Windows application that prevents your computer from locking the screen or going to sleep. It runs quietly in the system tray and provides a simple context menu for control.

## Features

- **Prevent Screen Lock:** Keeps your screen active and prevents the system from sleeping.
- **System Tray Icon:** Runs in the system tray for easy access.
- **Context Menu:**
    - **Enable/Disable:** Manually toggle the screen lock prevention.
    - **Set Timer:** Set a duration (in hours) after which the screen lock prevention will automatically disable.
    - **Run on Startup:** Configure the application to start automatically when Windows boots.
    - **Exit:** Close the application.
- **Customizable Icon:** Uses an embedded icon but can be customized.

## How to Use

1.  **Download:** Get the latest release from the [Releases](../../releases) page (assuming this will be the location).
2.  **Run:** Execute the `ScreenLockPreventer.exe` file.
3.  **Control:** Right-click the ScreenLockPreventer icon in the system tray to access the context menu options.

## Building from Source

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/your-username/ScreenLockPreventer.git
    cd ScreenLockPreventer
    ```
    *(Replace `your-username` with the actual GitHub username/organization)*
2.  **Open in Visual Studio:** Open the `ScreenLockPreventer.sln` file (or the `.csproj` file if a solution file isn't explicitly listed, though it's common for C# projects) with Visual Studio.
3.  **Build:** Build the solution (usually `Build > Build Solution` or `Ctrl+Shift+B`). The executable will typically be found in a subfolder like `bin/Debug` or `bin/Release`.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
