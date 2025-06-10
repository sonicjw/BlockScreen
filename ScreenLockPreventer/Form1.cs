using System;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using Microsoft.Win32;

namespace ScreenLockPreventer
{
    public partial class Form1 : Form
    {
        // Define EXECUTION_STATE flags
        private const uint ES_CONTINUOUS = 0x80000000;
        private const uint ES_SYSTEM_REQUIRED = 0x00000001;
        private const uint ES_DISPLAY_REQUIRED = 0x00000002;

        // Import SetThreadExecutionState
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern uint SetThreadExecutionState(uint esFlags);

        private ToolStripMenuItem enableMenuItem = null!;
        private ToolStripMenuItem disableMenuItem = null!;
        private ToolStripMenuItem setTimerMenuItem = null!;
        private ToolStripMenuItem exitMenuItem = null!;
        private ToolStripMenuItem runOnStartupMenuItem = null!;

        // Registry constants
        private const string StartupRegistryKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string ApplicationName = "ScreenLockPreventer";

        public Form1()
        {
            InitializeComponent();
            // Initialize the timer
            this.disableTimer = new System.Windows.Forms.Timer(this.components!); // Use null-forgiving operator
            this.disableTimer.Tick += new System.EventHandler(this.DisableTimer_Tick);

            InitializeContextMenu();
            InitializeNotifyIcon();
            SetInitialState(); // Sets initial startup menu item state

            this.Load += Form1_Load; // For hiding window
            this.FormClosing += Form1_FormClosing;
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            // Hide the main window on startup
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Hide();
            // Initial check state for runOnStartupMenuItem is set in SetInitialState,
            // which is called before Form1_Load.
        }

        private void InitializeNotifyIcon()
        {
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            // Text is set in SetInitialState and updated dynamically
            notifyIcon1.Icon = SystemIcons.Application; // Load a default system icon
            notifyIcon1.Visible = true;
        }

        private void InitializeContextMenu()
        {
            enableMenuItem = new ToolStripMenuItem("Enable");
            enableMenuItem.Click += EnableMenuItem_Click;

            disableMenuItem = new ToolStripMenuItem("Disable");
            disableMenuItem.Click += DisableMenuItem_Click;

            setTimerMenuItem = new ToolStripMenuItem("Set Timer...");
            setTimerMenuItem.Click += SetTimerMenuItem_Click;

            exitMenuItem = new ToolStripMenuItem("Exit");
            exitMenuItem.Click += ExitMenuItem_Click;

            runOnStartupMenuItem = new ToolStripMenuItem("Run on Startup");
            runOnStartupMenuItem.CheckOnClick = true;
            runOnStartupMenuItem.Click += RunOnStartupMenuItem_Click;

            contextMenuStrip1.Items.AddRange(new ToolStripItem[] {
                enableMenuItem,
                disableMenuItem,
                setTimerMenuItem,
                new ToolStripSeparator(),
                runOnStartupMenuItem, // Added here
                new ToolStripSeparator(), // Separator before Exit
                exitMenuItem
            });
        }

        private void SetInitialState()
        {
            // Default to disabled (AllowScreenLock)
            AllowScreenLock();
            disableMenuItem.Checked = true;
            enableMenuItem.Checked = false;
            notifyIcon1.Text = "Screen Lock Preventer (Inactive)";
            if (runOnStartupMenuItem != null) // Ensure it's initialized
            {
                runOnStartupMenuItem.Checked = IsStartupEnabled();
            }
        }

        private void RunOnStartupMenuItem_Click(object? sender, EventArgs e)
        {
            if (runOnStartupMenuItem == null) return;
            SetStartup(runOnStartupMenuItem.Checked);
        }

        private void SetStartup(bool enable)
        {
            try
            {
                RegistryKey? rk = Registry.CurrentUser.OpenSubKey(StartupRegistryKeyPath, true);
                if (rk == null)
                {
                    // This should ideally not happen for CurrentUser path unless major OS issue.
                    MessageBox.Show("Could not open registry key. Startup setting not changed.", "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (enable)
                {
                    rk.SetValue(ApplicationName, Application.ExecutablePath);
                }
                else
                {
                    rk.DeleteValue(ApplicationName, false); // Do not throw if not found
                }
                rk.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error configuring startup: {ex.Message}", "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Optionally revert check state if setting failed
                if (runOnStartupMenuItem != null) runOnStartupMenuItem.Checked = !enable;
            }
        }

        private bool IsStartupEnabled()
        {
            try
            {
                RegistryKey? rk = Registry.CurrentUser.OpenSubKey(StartupRegistryKeyPath, false);
                if (rk == null) return false;

                string? value = (string?)rk.GetValue(ApplicationName);
                rk.Close();
                // Ensure the path stored is actually this executable's path
                return !string.IsNullOrEmpty(value) && value.Equals(Application.ExecutablePath, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false; // Report as not enabled on error
            }
        }

        private void EnableMenuItem_Click(object? sender, EventArgs e)
        {
            if (disableTimer != null) disableTimer.Stop();
            PreventScreenLock();
            enableMenuItem.Checked = true;
            disableMenuItem.Checked = false;
            notifyIcon1.Text = "Screen Lock Preventer (Active)";
        }

        private void DisableMenuItem_Click(object? sender, EventArgs e)
        {
            if (disableTimer != null) disableTimer.Stop();
            AllowScreenLock();
            disableMenuItem.Checked = true;
            enableMenuItem.Checked = false;
            notifyIcon1.Text = "Screen Lock Preventer (Inactive)";
        }

        private void SetTimerMenuItem_Click(object? sender, EventArgs e)
        {
            string input = Interaction.InputBox("Enter duration in hours to keep screen awake:", "Set Timer", "1");
            if (double.TryParse(input, out double hours) && hours > 0)
            {
                if (disableTimer == null) // Should not happen if constructor ran correctly
                {
                    // This redundant check might be unnecessary if components! is reliable.
                    // However, defensive coding doesn't hurt.
                    this.disableTimer = new System.Windows.Forms.Timer(this.components!);
                    this.disableTimer.Tick += new System.EventHandler(this.DisableTimer_Tick);
                }
                disableTimer.Stop();
                disableTimer.Interval = (int)(hours * 60.0 * 60.0 * 1000.0);
                disableTimer.Start();
                PreventScreenLock();
                enableMenuItem.Checked = true;
                disableMenuItem.Checked = false;
                DateTime endTime = DateTime.Now.AddMilliseconds(disableTimer.Interval);
                notifyIcon1.Text = $"SLP Active until {endTime:HH:mm}";
            }
            else if (!string.IsNullOrEmpty(input)) // Only show error if user entered something invalid
            {
                MessageBox.Show("Invalid input. Please enter a positive number for hours.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisableTimer_Tick(object? sender, EventArgs e)
        {
            AllowScreenLock();
            if (disableTimer != null) disableTimer.Stop();
            disableMenuItem.Checked = true;
            enableMenuItem.Checked = false;
            notifyIcon1.Text = "Screen Lock Preventer (Inactive)";
            notifyIcon1.ShowBalloonTip(2000, "Screen Lock Preventer", "Timer elapsed. Screen lock is now allowed.", ToolTipIcon.Info);
        }

        private void ExitMenuItem_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // Ensure the icon is removed from the tray.
            if (notifyIcon1 != null)
            {
                notifyIcon1.Visible = false;
            }
            // Ensure normal screen lock behavior is restored on exit.
            AllowScreenLock();
            if (disableTimer != null)
            {
                disableTimer.Stop();
                disableTimer.Dispose(); // Release timer resources
            }
        }

        /// <summary>
        /// Prevents the screen from locking and the system from sleeping.
        /// </summary>
        public void PreventScreenLock()
        {
            SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED);
        }

        /// <summary>
        /// Allows the screen to lock and the system to sleep normally.
        /// </summary>
        public void AllowScreenLock()
        {
            SetThreadExecutionState(ES_CONTINUOUS);
        }
    }
}
