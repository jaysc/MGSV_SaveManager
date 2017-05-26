using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using SteamScan;

namespace MGSV_SaveSwitcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MySteamScanner mySteamScan = new MySteamScanner();

        public MainWindow()
        {

            InitializeComponent();

            SteamScanner();

        }

        private void SteamScanner()
        {
            Console.WriteLine("Starting steam scan");
            string steamPath = this.mySteamScan.ScanSteam();
            if (steamPath == "")
            {
                MessageBox.Show("Steam not found, if you are sure it's installed and keep getting this message, please make an bug report to the GitHub project page.");
                return;
            }
            this.steamPath.Text = steamPath;

            Console.WriteLine("Getting usernames and IDs");
            Dictionary<string, string> username = this.mySteamScan.UserScan();
            Console.WriteLine(username);

            foreach (KeyValuePair<string, string> x in username)
            {
                Console.WriteLine(x.Key + " " + x.Value);
                this.userList.Items.Add(x.Key);
            }

            Console.WriteLine("Check if less than 2 users");
            Console.WriteLine(username.Count);
            if (username.Count < 2)
            {
                this.currentUser.Text = username.Keys.First();
                this.currentSave.Text = this.mySteamScan.CurrentSave(this.currentUser.Text);

                UserCheck();
            } else
            {
                this.userList.IsEnabled = true;
                this.applyUser.IsEnabled = true;
                UserCheck();
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
            } else
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
            string newSave = this.newSaveName.Text;
            if (newSave.Trim() == "")
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
                }
                else if (msgResult == MessageBoxResult.No)
                {
                    MessageBox.Show("Operation cancelled, no changes made.");
                    this.newSaveName.Text = "";
                }
            }
            SaveListing();
            Console.WriteLine("new save operation completed.");
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
                    this.currentSave.Text = this.mySteamScan.CurrentSave(this.currentUser.Text);

                }
                else if (msgResult == MessageBoxResult.No)
                {
                    MessageBox.Show("Operation cancelled, no changes made.");
                }
            }
            SaveListing();
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
                this.mySteamScan.RenameSave(this.currentSave.Text, this.currentUser.Text, this.RenameSaveName.Text);
                this.currentSave.Text = this.mySteamScan.CurrentSave(this.currentUser.Text);
            }
            else if (msgResult == MessageBoxResult.No)
            {
                MessageBox.Show("Operation cancelled, no changes made.");
            }
            this.RenameSaveName.Text = "";
            
        }

        private void LaunchGame_Click(object sender, RoutedEventArgs e)
        {
            this.mySteamScan.LaunchGame();
        }

    }
}
