using System;
using System.Windows;
using SteamScan;
using System.Net;
using System.Diagnostics;
using System.Windows.Navigation;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ManagerLogger;

namespace MGSV_SaveSwitcher
{
    /// <summary>
    /// Interaction logic for Launcher.xaml
    /// </summary>
    public partial class Launcher : Window
    {
        MySteamScanner SteamScanner;
        Logger myLogger;
        WebClient webReader = new WebClient();
        string branch = "master";
        bool alertOnOff = false;
        string currentVersion = "v2.5.1";
        int curVersion = 251;
        string configFiles = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "MGSV_SaveManager");
        string localPath;
        string steamPath;

        public Launcher()
        {
            this.myLogger = new Logger(this.configFiles);
            this.myLogger.LogToFile("Application starting");
            InitializeComponent();
            Closing += LauncherClosing;
            this.Version.Text = this.currentVersion;
            this.UpdateCheck();
            
            try
            {
                this.localPath = File.ReadAllLines(Path.Combine(this.configFiles, "localpath.txt"))[0].Trim().Replace("\"", "");
                if (Directory.Exists(this.localPath))
                {
                    this.SteamScanner = new MySteamScanner(this.localPath, this.configFiles);
                    this.steamPath = this.SteamScanner.ScanSteam();
                    this.CheckSteamPath();
                } else
                {
                    this.myLogger.LogToFile("Local path doesn't exist.");
                    if (this.SaveLocation())
                    {
                        this.SteamScanner = new MySteamScanner(this.localPath, this.configFiles);
                        this.steamPath = this.SteamScanner.ScanSteam();
                        this.CheckSteamPath();
                    }
                    else
                    {
                        App.Current.Shutdown();
                    }
                }
            } catch (Exception e)
            {
                this.myLogger.LogToFile($"Local path not yet defined, {e}");
                if (this.SaveLocation())
                {
                    if (!Directory.Exists(this.localPath) || (Directory.Exists(this.localPath) && Directory.GetDirectories(this.localPath).Length < 1))
                    {
                        this.myLogger.LogToFile("Local directory doesn't exist, or exist but doesn't contain saves.");
                        this.CopyOld();
                    }
                    this.SteamScanner = new MySteamScanner(this.localPath, this.configFiles);
                    this.steamPath = this.SteamScanner.ScanSteam();
                    this.CheckSteamPath();
                }
                else
                {
                    App.Current.Shutdown();
                }
            }
        }


        /// <summary>
        /// Choose where to store save files
        /// </summary>
        private bool SaveLocation()
        {
            MessageBox.Show("Select a location where to store the save files.\nOld files will be copied to the new location.");
            this.myLogger.LogToFile("Selecting save location...");
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    Console.WriteLine(dialog.SelectedPath);
                    this.localPath = Path.Combine(dialog.SelectedPath, "MGSV_SaveManager");
                    File.WriteAllText(Path.Combine(this.configFiles, "localpath.txt"), this.localPath);
                    return true;
                }
                else
                {
                    Console.WriteLine("Cancelled or error");
                    this.myLogger.LogToFile("Save location selection cancelled.");
                    return false;
                }
            }
        }


        /// <summary>
        /// Copy old configs and saves
        /// </summary>
        private void CopyOld()
        {
            this.CommandPrompter($"dir /b C:\\MGSV_saves > {Path.Combine(this.configFiles, "old_files.txt")}");
            this.CommandPrompter($"dir /b /ad C:\\MGSV_saves > {Path.Combine(this.configFiles, "save_folders.txt")}");
            List<string> oldfiles = new List<string>(){"currentuser.txt", "current_save.txt", "steam_path.txt", "users.txt"};
            List<string> old_configs = File.ReadAllLines(Path.Combine(this.configFiles, "old_files.txt")).ToList<string>();
            List<string> old_saves = File.ReadAllLines(Path.Combine(this.configFiles, "save_folders.txt")).ToList<string>();

            Directory.CreateDirectory(this.localPath);

            foreach (string x in old_configs)
            {
                if (oldfiles.Contains(x))
                {
                    File.Copy(Path.Combine("C:\\MGSV_saves", x.Trim()), Path.Combine(this.configFiles, x.Trim()), true);
                }
            }
            foreach (string x in old_saves)
            {
                this.CommandPrompter($"xcopy /E /Y {Path.Combine("C:\\MGSV_saves", x)} {Path.Combine(this.localPath, x)}\\");
            }
            File.Delete(Path.Combine(this.configFiles, "old_files.txt"));
            File.Delete(Path.Combine(this.configFiles, "save_folders.txt"));
            this.myLogger.LogToFile("Old files copied to the new location.");
        }


        /// <summary>
        /// Check steam path
        /// </summary>
        private void CheckSteamPath()
        {
            if (this.steamPath == "")
            {
                MessageBox.Show("Error with locating Steam, try running the application again.");
                this.myLogger.LogToFile("Error, Steam path empty.");
            }
            else if (steamPath.Contains("Error:"))
            {
                MessageBox.Show(this.steamPath.Replace("Error:", ""));
                this.myLogger.LogToFile($"{this.steamPath}");
            }
            else
            {
                this.LaunchGameButton.IsEnabled = true;
                this.SavesButton.IsEnabled = true;
                try
                {
                    Console.WriteLine($"{this.SteamScanner.GetCurrentUser()}");
                    Console.WriteLine("current user found");
                    Console.WriteLine($"{this.SteamScanner.CurrentSave(this.SteamScanner.GetCurrentUser())}");
                    this.SteamScanner.SteamToLocal(this.SteamScanner.CurrentSave(this.SteamScanner.GetCurrentUser()), this.SteamScanner.GetCurrentUser());
                    this.SettingsButton.IsEnabled = true;
                    this.SettingsButton.Content = "Settings";
                }
                catch (Exception e)
                {
                    Console.WriteLine("No current user or save found, probably first run.");
                    this.myLogger.LogToFile($"No current user, settings menu disabled. {e}");
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
            this.myLogger.LogToFile("Checking for updates...");
            this.myLogger.LogToFile($"Current version: {this.curVersion}");
            try
            {
                string webRelease = this.webReader.DownloadString($"https://raw.githubusercontent.com/thatsafy/MGSV_SaveManager/{this.branch}/latest.txt");
                List<string> temp = webRelease.Split('.').ToList<string>();
                string latest = "";
                foreach (string x in temp)
                {
                    latest += x;
                }
                int version = Int32.Parse(latest);

                if (this.curVersion < version)
                {
                    this.myLogger.LogToFile($"New release available: {webRelease}");
                    this.UpdateMessage(webRelease);
                    this.ToggleAlert();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while checking for updates.\nError: {e}");
                this.myLogger.LogToFile($"Error while checking for updates, {e}");
            }
        }


        /// <summary>
        /// Toggle alert image and link
        /// </summary>
        private void ToggleAlert()
        {
            this.myLogger.LogToFile("Toggling update notifications on/off.");
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
            string releaseNotes = this.webReader.DownloadString($"https://raw.githubusercontent.com/thatsafy/MGSV_SaveManager/{this.branch}/releasenotes.txt");
            MessageBox.Show($"New release available.\nCurrent release: {this.currentVersion}\nLatest release: {release}\n{releaseNotes}");
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
            SettingsWindow SettingsWindow = new SettingsWindow(this.SteamScanner, this.localPath, this.steamPath, this.configFiles);
            try {
                SettingsWindow.Show();
                this.IsEnabled = false;
                this.myLogger.LogToFile("Settings menu opened.");
            } catch (Exception x)
            {
                Console.WriteLine("Settings not opened.");
                this.myLogger.LogToFile($"Error: Settings menu not opened. {x}");
            }
            SettingsWindow.Closing += this.EnableLauncher;
        }


        /// <summary>
        /// Enable launcher elements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnableLauncher(object sender, EventArgs e)
        {
            this.myLogger.LogToFile("Launcher enabled.");
            this.IsEnabled = true;
            if (!this.SettingsButton.IsEnabled && this.SteamScanner.GetCurrentUser() != "")
            {
                this.SettingsButton.Content = "Settings";
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
            this.myLogger.LogToFile("Application quit.");
            App.Current.Shutdown();
        }


        /// <summary>
        /// Closing main window closes entire application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LauncherClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.myLogger.LogToFile("Application quit.");
            App.Current.Shutdown();
        }


        /// <summary>
        /// Launch saves window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Saves_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            this.myLogger.LogToFile("Opening saves window.");
            Console.WriteLine("open saves window");
            MainWindow SavesWindow = new MainWindow(this.SteamScanner, this.steamPath);
            SavesWindow.Closing += this.EnableLauncher;
            try
            {
                SavesWindow.Show();
                this.IsEnabled = false;
                this.myLogger.LogToFile("Saves menu opened.");
                
            }
            catch (Exception x)
            {
                Console.WriteLine("Saves not opened.");
                this.myLogger.LogToFile($"Error: Saves menu not opened. {x}");
                this.IsEnabled = true;
            }
        }


        /// <summary>
        /// Use Windows Command prompt
        /// </summary>
        /// <param name="command"></param>
        private void CommandPrompter(string command)
        {
            this.myLogger.LogToFile($"CLI command: {command}");
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = $"/C {command}"
            };
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }


        /// <summary>
        /// Open saves directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenSaves_Click(object sender, RoutedEventArgs e)
        {
            this.myLogger.LogToFile("Opening saves directory.");
            Process.Start(this.localPath);
        }


        /// <summary>
        /// Open configs directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenConfigs_Click(object sender, RoutedEventArgs e)
        {
            this.myLogger.LogToFile("Opening config directory.");
            Process.Start(this.configFiles);
        }
    }
}