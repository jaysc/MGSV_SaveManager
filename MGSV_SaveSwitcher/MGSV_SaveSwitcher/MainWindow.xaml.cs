using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Diagnostics;
using SteamScan;
using ManagerLogger;

namespace MGSV_SaveSwitcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Logger myLogger;
        MySteamScanner mySteamScan;
        string steampath = "";
        Dictionary<string, string> username;

        public MainWindow(MySteamScanner mySteamScan, string steam)
        {
            this.myLogger = new Logger(mySteamScan.configPath);
            InitializeComponent();
            this.mySteamScan = mySteamScan;

            this.steampath = steam;
            Console.WriteLine(this.steampath);

            if (this.steampath == "")
            {
                MessageBox.Show("Steam not found, if you are sure it's installed and keep getting this message, please make an bug report to the GitHub project page.");
                this.Close();
            } else
            {
                SaveManager();
            }
        }


        /// <summary>
        /// program logic starts here
        /// </summary>
        private void SaveManager()
        {
            this.myLogger.LogToFile("Save manager started.");
            
            this.steamPath.Text = this.steampath;


            Console.WriteLine("Getting usernames and IDs");
            this.myLogger.LogToFile("Getting users.");
            this.username = this.mySteamScan.UserScan();
            Console.WriteLine(username);

            this.myLogger.LogToFile("Adding usernames to the userlist.");
            foreach (KeyValuePair<string, string> x in this.username)
            {
                Console.WriteLine(x.Key + " " + x.Value);
                this.userList.Items.Add(x.Key);
            }

            try
            {
                List<string> filedata = File.ReadAllLines(Path.Combine(this.mySteamScan.configPath, "currentuser.txt")).ToList();
                this.currentUser.Text = filedata[0].Trim();
                this.currentSave.Text = this.mySteamScan.CurrentSave(this.currentSave.Text);
                if (this.username.Count > 1)
                {
                    this.myLogger.LogToFile($"{this.username.Count} users found, enabling user list.");
                    this.userList.IsEnabled = true;
                    this.applyUser.IsEnabled = true;
                }
                Console.WriteLine("Read current user from a file");
                UserCheck();
            } catch (IOException e)
            {
                this.myLogger.LogToFile($"Error: {e}");
                Console.WriteLine($"Error: {e}");
                Console.WriteLine("Check if less than 2 users");
                Console.WriteLine(username.Count);
                if (this.username.Count < 2)
                {
                    this.currentUser.Text = username.Keys.First();
                    UserCheck();
                    this.mySteamScan.CurrentUser(this.currentUser.Text, this.username[this.currentUser.Text]);
                }
                else
                {
                    this.userList.IsEnabled = true;
                    this.applyUser.IsEnabled = true;
                    UserCheck();
                }
            }
        }


        /// <summary>
        /// When user is selected, enable save options
        /// </summary>
        private Boolean UserCheck()
        {
            Console.WriteLine("User check");
            if (this.currentUser.Text != "")
            {
                SaveListing();
                this.currentSave.Text = this.mySteamScan.CurrentSave(this.currentUser.Text);
                this.RenameSaveName.IsReadOnly = false;
                this.RenameSaveName.Background = Brushes.White;
                this.ApplyRename.IsEnabled = true;
                this.newSaveName.IsReadOnly = false;
                this.newSaveName.Background = Brushes.White;
                this.applyNewSaveName.IsEnabled = true;
                this.StartSaveScan.IsEnabled = true;

                return true;
            }
            return false;
        }


        /// <summary>
        /// Add saves to the dropdown menus
        /// </summary>
        private void SaveListing()
        {
            List<string> saves = this.mySteamScan.SaveScan(this.currentUser.Text);

            if (saves.Count > 1)
            {
                this.saveDelList.IsEnabled = true;
                this.applyDelSave.IsEnabled = true;
                this.saveChangeList.IsEnabled = true;
                this.applySaveChange.IsEnabled = true;
            }  else
            {
                this.saveDelList.IsEnabled = false;
                this.applyDelSave.IsEnabled = false;
                this.saveChangeList.IsEnabled = false;
                this.applySaveChange.IsEnabled = false;
            }

            List<string> delList = new List<string>();
            foreach (string y in this.saveDelList.Items)
            {
                if (!saves.Contains(y)) {
                    delList.Add(y);
                }
            }
            foreach (string x in delList)
            {
                this.saveDelList.Items.Remove(x);
            }

            delList = new List<string>();
            foreach (string y in this.saveChangeList.Items)
            {
                if (!saves.Contains(y))
                {
                    delList.Add(y);
                }
            }
            foreach (string x in delList)
            {
                this.saveChangeList.Items.Remove(x);
            }

            foreach (string x in saves)
            {
                if (!this.saveDelList.Items.Contains(x))
                {
                    this.saveDelList.Items.Add(x);
                }
                if (!this.saveChangeList.Items.Contains(x))
                {
                    this.saveChangeList.Items.Add(x);
                }
            }

            foreach (string x in this.saveChangeList.Items)
            {
                Console.WriteLine(x);
            }
        }


        /// <summary>
        /// Apply selected user from dropdown menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyUser_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Applying user selection from the button");
            string userSelection = this.userList.Text;
            string previousUser = this.currentUser.Text;
            Console.WriteLine("Previous " + previousUser);

            this.userList.Text = "";
            this.currentUser.Text = userSelection;
            this.mySteamScan.CurrentUser(this.currentUser.Text, this.username[this.currentUser.Text]);

            if (previousUser != "")
            {
                this.mySteamScan.ChangeUser(previousUser);
                SaveListing();
            } 
            UserCheck();
        }


        /// <summary>
        /// Apply selected save file (save switching)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplySaveChange_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Applying save change.");
            string saveSelection = this.saveChangeList.Text;
            if (saveSelection.Trim() != "")
            {
                this.saveChangeList.Text = "";
                this.currentSave.Text = saveSelection;
                this.mySteamScan.SaveSwitch(this.currentSave.Text, this.currentUser.Text);
            }
            SaveListing();
            Console.WriteLine("Save changed.");
        }


        /// <summary>
        /// Apply the name for new save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyNewSaveName_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Applying new save");
            string newSave = this.newSaveName.Text.ToLower();
            if (this.saveChangeList.Items.Contains(newSave))
            {
                MessageBox.Show($"Save by the name '{newSave}' already exists, please try another name.");
                this.newSaveName.Text = "";
            } else if (newSave.Trim() == "")
            {
                MessageBox.Show("Please enter a valid name.");
            } else
            {
                MessageBoxResult msgResult = MessageBox.Show($"Create a new save with name '{newSave}'?", "New Save", MessageBoxButton.YesNo);
                if (msgResult == MessageBoxResult.Yes)
                {
                    this.currentSave.Text = newSave;
                    this.mySteamScan.NewSave(newSave, this.currentUser.Text);
                    this.newSaveName.Text = "";
                    SaveListing();
                    Console.WriteLine("new save operation completed.");
                    this.myLogger.LogToFile($"Created a save named {newSave}");
                }
                else if (msgResult == MessageBoxResult.No)
                {
                    MessageBox.Show("Operation cancelled, no changes made.");
                    this.newSaveName.Text = "";
                }
            }
        }


        /// <summary>
        /// Apply selected save to be deleted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyDelSave_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Deleting save");
            string saveSelection = this.saveDelList.Text;
            if (this.saveDelList.Text.Trim() == "")
            {
                MessageBox.Show("No save selected.");
            } else
            {
                this.saveDelList.Text = "";
                string message = $"Delete save {saveSelection}?";

                MessageBoxResult msgResult = MessageBox.Show($"{message}", "Delete Save", MessageBoxButton.YesNo);
                if (msgResult == MessageBoxResult.Yes)
                {
                    this.mySteamScan.DeleteSave(saveSelection, this.currentUser.Text);
                    this.myLogger.LogToFile($"Save {saveSelection} deleted.");
                }
                else if (msgResult == MessageBoxResult.No)
                {
                    MessageBox.Show("Operation cancelled, no changes made.");
                }
            }
            SaveListing();
            this.currentSave.Text = this.mySteamScan.CurrentSave(this.currentUser.Text);
            Console.WriteLine("delete save operation done.");
        }


        /// <summary>
        /// Scan for save files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveScan_Click(object sender, RoutedEventArgs e)
        {
            SaveListing();
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
        /// Apply rename
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyRename_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Renaming save.");
            MessageBoxResult msgResult = MessageBox.Show($"Rename '{this.currentSave.Text}' to '{this.RenameSaveName}' ?", "Rename Save", MessageBoxButton.YesNo);
            if (msgResult == MessageBoxResult.Yes)
            {
                this.myLogger.LogToFile($"Renaming save {this.currentSave.Text} to {this.RenameSave.Text}");
                this.mySteamScan.RenameSave(this.currentSave.Text, this.currentUser.Text, this.RenameSaveName.Text);
                this.currentSave.Text = this.mySteamScan.CurrentSave(this.currentUser.Text);
            }
            else if (msgResult == MessageBoxResult.No)
            {
                MessageBox.Show("Operation cancelled, no changes made.");
            }
            this.RenameSaveName.Text = "";
        }


        private void BackWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}