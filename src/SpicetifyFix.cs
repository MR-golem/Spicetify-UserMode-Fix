using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Principal;
using System.Windows.Forms;

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}

sealed class MainForm : Form
{
    private readonly Button installButton;
    private readonly ProgressBar progressBar;
    private readonly Label statusLabel;

    public MainForm()
    {
        Text = "Spicetify Fix - User Mode";
        Size = new Size(520, 295);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;
        ForeColor = Color.Black;
        Font = new Font("Segoe UI", 9);

        var title = new Label
        {
            Text = "Spicetify Fix",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.Black,
            Location = new Point(20, 18),
            Size = new Size(480, 26)
        };
        Controls.Add(title);

        var desc = new Label
        {
            Text = "If you use a modified Windows and Spicetify refuses to install\r\nbecause the command runs as Administrator — this fix is for you.",
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(60, 60, 60),
            Location = new Point(20, 46),
            Size = new Size(480, 32)
        };
        Controls.Add(desc);

        var divider = new Panel
        {
            BackColor = Color.Black,
            Location = new Point(0, 85),
            Size = new Size(520, 1)
        };
        Controls.Add(divider);

        bool isAdmin = IsAdministrator();
        var mode = new Label
        {
            Text = isAdmin ? "Current mode: Administrator (will install as User)" : "Current mode: User \u2713",
            Font = new Font("Segoe UI", 8, FontStyle.Bold),
            ForeColor = isAdmin ? Color.FromArgb(180, 0, 0) : Color.FromArgb(0, 120, 0),
            Location = new Point(20, 98),
            Size = new Size(480, 16)
        };
        Controls.Add(mode);

        var cmdBox = new Label
        {
            Text = "iwr -useb https://raw.githubusercontent.com/spicetify/cli/main/install.ps1 | iex",
            Font = new Font("Consolas", 7),
            ForeColor = Color.FromArgb(100, 100, 100),
            BackColor = Color.FromArgb(245, 245, 245),
            BorderStyle = BorderStyle.FixedSingle,
            Location = new Point(20, 122),
            Size = new Size(475, 22),
            TextAlign = ContentAlignment.MiddleCenter
        };
        Controls.Add(cmdBox);

        installButton = new Button
        {
            Text = "Install Spicetify",
            Font = new Font("Segoe UI", 9),
            Location = new Point(185, 160),
            Size = new Size(150, 32),
            BackColor = Color.White,
            ForeColor = Color.Black,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        installButton.FlatAppearance.BorderColor = Color.Black;
        installButton.FlatAppearance.BorderSize = 1;
        installButton.Click += OnInstallClicked;
        Controls.Add(installButton);

        progressBar = new ProgressBar
        {
            Location = new Point(20, 210),
            Size = new Size(475, 14),
            Style = ProgressBarStyle.Continuous,
            Value = 0
        };
        Controls.Add(progressBar);

        statusLabel = new Label
        {
            Text = "Ready",
            Font = new Font("Segoe UI", 7),
            ForeColor = Color.FromArgb(80, 80, 80),
            Location = new Point(20, 228),
            Size = new Size(475, 14),
            TextAlign = ContentAlignment.MiddleCenter
        };
        Controls.Add(statusLabel);

        var footer = new Label
        {
            Text = "Spicetify CLI \u2022 User Mode Only",
            Font = new Font("Segoe UI", 7),
            ForeColor = Color.FromArgb(160, 160, 160),
            Location = new Point(20, 250),
            Size = new Size(475, 14),
            TextAlign = ContentAlignment.MiddleCenter
        };
        Controls.Add(footer);
    }

    private static bool IsAdministrator()
    {
        try { return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator); }
        catch { return false; }
    }

    private void OnInstallClicked(object sender, EventArgs e)
    {
        installButton.Enabled = false;
        progressBar.Style = ProgressBarStyle.Marquee;
        statusLabel.Text = "Opening PowerShell...";

        try
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string psFile = Path.Combine(baseDir, "run_spicetify.ps1");

            if (!File.Exists(psFile))
                File.WriteAllText(psFile, "iwr -useb https://raw.githubusercontent.com/spicetify/cli/main/install.ps1 | iex\r\npause");

            if (IsAdministrator())
            {
                var psi = new ProcessStartInfo("cmd.exe", "/c runas /trustlevel:0x20000 \"powershell -NoExit -ExecutionPolicy Bypass -File " + psFile + "\"")
                {
                    UseShellExecute = false,
                    CreateNoWindow = false
                };
                Process.Start(psi);
                statusLabel.Text = "PowerShell opened as User";
            }
            else
            {
                var psi = new ProcessStartInfo("powershell.exe", "-NoExit -ExecutionPolicy Bypass -File \"" + psFile + "\"")
                {
                    UseShellExecute = true
                };
                Process.Start(psi);
                statusLabel.Text = "PowerShell opened";
            }

            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 100;
        }
        catch (Exception ex)
        {
            statusLabel.Text = "Error: " + ex.Message;
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 0;
        }

        installButton.Enabled = true;
    }
}
