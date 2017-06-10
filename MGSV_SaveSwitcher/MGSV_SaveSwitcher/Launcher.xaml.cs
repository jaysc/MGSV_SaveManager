using System;
using System.Windows;
using SteamScan;
using System.Net;
using System.Diagnostics;
using System.Windows.Navigation;


namespace MGSV_SaveSwitcher
{
    /// <summary>
    /// Interaction logic for Launcher.xaml
    /// </summary>
    public partial class Launcher : Window
    {
        MySteamScanner SteamScanner = new MySteamScanner();
        WebClient webReader = new WebClient();
        string branch = "master";
        bool alertOnOff = false;
        string currentVersion = "v2.4.1";
        int curVersion = 241;

        string steamPath = "";
        
        public Launcher()
        {
            InitializeComponent();
            Closing += LauncherClosing;
            this.Version.Text = this.currentVersion;
            this.UpdateCheck();

            this.steamPath = this.SteamScanner.ScanSteam();
            if (steamPath == "")
            {
                MessageBox.Show("Steam not found, if you are sure it's installed and keep getting this message, please make an bug report to the GitHub project page.");
            } else
            {
                this.LaunchGameButton.IsEnabled = true;
                this.SavesButton.IsEnabled = true;
                try
                {
                    Console.WriteLine($"{this.SteamScanner.GetCurrentUser()}");
                    Console.WriteLine($"{this.SteamScanner.CurrentSave(this.SteamScanner.GetCurrentUser())}");
                    this.SteamScanner.SteamToLocal(this.SteamScanner.CurrentSave(this.SteamScanner.GetCurrentUser()), this.SteamScanner.GetCurrentUser());
                    this.SettingsButton.IsEnabled = true;
                    Console.WriteLine("Steam to local done.");
                }
                catch
                {
                    Console.WriteLine("No current user or save found, probably first run.");
                }
            }
        }


        /// <summary>
        /// Launch The Phantom Pain
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LaunchGame_Click(object sender, RoutedEventArgs e)
        {
            this.SteamScanner.LaunchGame();
        }


        /// <summary>
        /// Check if new version available
        /// </summary>
        private void UpdateCheck()
        {
            try
            {
                string webRelease = this.webReader.DownloadString($"https://raw.githubusercontent.com/thatsafy/MGSV_SaveManager/{this.branch}/latest.txt");
                string[] temp = webRelease.Split('.');
                string latest = "";
                foreach (string x in temp)
                {
                    latest += x;
                }
                int version = Int32.Parse(latest);

                if (this.curVersion < version)
                {
                    this.UpdateMessage(webRelease);
                    this.ToggleAlert();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while checking for updates.\nError: {e}");
            }
        }


        /// <summary>
        /// Toggle alert image and link
        /// </summary>
        private void ToggleAlert()
        {
            if (this.alertOnOff)
            {
                this.alertOnOff = false;
                this.alertlink.IsEnabled = false;
                this.alertimage.Visibility = Visibility.Hidden;
                this.UpdateText.Visibility = Visibility.Hidden;
            }
            else
            {
                this.alertOnOff = true;
                this.alertlink.IsEnabled = true;
                this.alertimage.Visibility = Visibility.Visible;
                this.UpdateText.Visibility = Visibility.Visible;
            }
        }


        /// <summary>
        /// Popup message if new release
        /// </summary>
        /// <param name="release"></param>
        private void UpdateMessage(string release)
        {
            MessageBox.Show($"New release available.\nCurrent release: {this.currentVersion}\nLatest release: {release}");
        }


        /// <summary>
        /// Open hyperlink
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }


        /// <summary>
        /// Launch saves window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LaunchSettings(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("open saves window");
            SettingsWindow SettingsWindow = new SettingsWindow();
            try {
                SettingsWindow.Show();
                this.IsEnabled = false;
            } catch
            {
                Console.WriteLine("Settings not opened.");
            }
            SettingsWindow.Closing += this.EnableLauncher;
        }


        /// <summary>
        /// Enable launcher elements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnableLauncher(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.IsEnabled = true;
            if (!this.SettingsButton.IsEnabled && this.SteamScanner.GetCurrentUser() != "")
            {
                this.SettingsButton.IsEnabled = true;
            }
        }


        /// <summary>
        /// Exit launcher, close all windows associated with launcher
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }


        /// <summary>
        /// Closing main window closes entire application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LauncherClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void Saves_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("open saves window");
            MainWindow SavesWindow = new MainWindow(this.SteamScanner);
            SavesWindow.Show();
            this.IsEnabled = false;
            SavesWindow.Closing += this.EnableLauncher;
        }
    }
}
