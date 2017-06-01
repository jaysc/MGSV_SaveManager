using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using SteamScan;

namespace MGSV_SaveSwitcher
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        string steampath;
        string userid;
        string currentSave;
        string localPath = "C:\\MGSV_saves\\";
        string steamConfigPath;
        private bool handle = true;
        Dictionary<string, string> graphicSettings = new Dictionary<string, string>();
        MySteamScanner SteamScanner = new MySteamScanner();

        public SettingsWindow()
        {
            InitializeComponent();
            
            try
            {
                this.steampath = this.SteamScanner.ScanSteam();
                this.CurrentUserSettings.Text = this.SteamScanner.GetCurrentUser();
                this.userid = this.SteamScanner.GetCurrentID();
                this.currentSave = this.SteamScanner.CurrentSave(this.CurrentUserSettings.Text);
                this.localPath += this.CurrentUserSettings.Text + "\\";
                this.steamConfigPath = $"{this.steampath.Trim()}userdata\\{this.userid.Trim()}\\287700\\local\\TPP_GRAPHICS_CONFIG";
                this.LoadSettings();
            } catch
            {
                MessageBox.Show("Please select user first from the 'Saves' menu.");
                this.Close();
            }
        }
        

        /// <summary>
        /// Load the settings from the file
        /// </summary>
        private void LoadSettings()
        {
            this.LoadSettingsFromFile();
            this.PresetBox.Text = "";
            this.depth_of_field.Text = this.graphicSettings["depth_of_field"];
            this.effect.Text = this.graphicSettings["effect"];
            this.lighting.Text = this.graphicSettings["lighting"];
            this.model_detail.Text = this.graphicSettings["model_detail"];
            this.motion_blur_amount.Text = this.graphicSettings["motion_blur_amount"];
            this.postprocess.Text = this.graphicSettings["postprocess"];
            this.shadow.Text = this.graphicSettings["shadow"];
            this.ssao.Text = this.graphicSettings["ssao"];
            this.texture.Text = this.graphicSettings["texture"];
            this.texture_filtering.Text = this.graphicSettings["texture_filtering"];
            this.volumetric_clouds.Text = this.graphicSettings["volumetric_clouds"];
            this.framerate_control.Text = this.graphicSettings["framerate_control"];
            this.vsync.Text = this.graphicSettings["vsync"];
            this.window_mode.Text = this.graphicSettings["window_mode"];
            this.ResolutionBox.Text = this.graphicSettings["width"] + "x" + this.graphicSettings["height"];
        }


        /// <summary>
        /// Low preset
        /// </summary>
        public void LowPreset()
        {
            this.depth_of_field.Text = "Disable";
            this.effect.Text = "Low";
            this.lighting.Text = "Low";
            this.model_detail.Text = "Low";
            this.motion_blur_amount.Text = "Off";
            this.postprocess.Text = "Off";
            this.shadow.Text = "Low";
            this.ssao.Text = "Off";
            this.texture.Text = "Low";
            this.texture_filtering.Text = "Medium";
            this.volumetric_clouds.Text = "Off";
            this.framerate_control.Text = "Auto";
            this.vsync.Text = "Enable";
            this.window_mode.Text = "FullScreenWindowed";
            ///this.ResolutionBox.Text = "1920" + "x" + "1080";
        }


        /// <summary>
        /// Medium preset
        /// </summary>
        public void MediumPreset()
        {
            this.depth_of_field.Text = "Disable";
            this.effect.Text = "High";
            this.lighting.Text = "Low";
            this.model_detail.Text = "Medium";
            this.motion_blur_amount.Text = "Small";
            this.postprocess.Text = "Low";
            this.shadow.Text = "Medium";
            this.ssao.Text = "Off";
            this.texture.Text = "Medium";
            this.texture_filtering.Text = "Medium";
            this.volumetric_clouds.Text = "Off";
            this.framerate_control.Text = "Auto";
            this.vsync.Text = "Enable";
            this.window_mode.Text = "FullScreenWindowed";
            ///this.ResolutionBox.Text = "1920" + "x" + "1080";
        }


        /// <summary>
        /// High preset
        /// </summary>
        public void HighPreset()
        {
            this.depth_of_field.Text = "Enable";
            this.effect.Text = "High";
            this.lighting.Text = "High";
            this.model_detail.Text = "High";
            this.motion_blur_amount.Text = "Medium";
            this.postprocess.Text = "High";
            this.shadow.Text = "High";
            this.ssao.Text = "High";
            this.texture.Text = "High";
            this.texture_filtering.Text = "High";
            this.volumetric_clouds.Text = "On";
            this.framerate_control.Text = "Auto";
            this.vsync.Text = "Enable";
            this.window_mode.Text = "FullScreenWindowed";
            ///this.ResolutionBox.Text = "1920" + "x" + "1080";
        }


        /// <summary>
        /// Very high preset
        /// </summary>
        public void VeryHighPreset()
        {
            this.depth_of_field.Text = "Enable";
            this.effect.Text = "ExtraHigh";
            this.lighting.Text = "ExtraHigh";
            this.model_detail.Text = "ExtraHigh";
            this.motion_blur_amount.Text = "Large";
            this.postprocess.Text = "ExtraHigh";
            this.shadow.Text = "ExtraHigh";
            this.ssao.Text = "ExtraHigh";
            this.texture.Text = "ExtraHigh";
            this.texture_filtering.Text = "ExtraHigh";
            this.volumetric_clouds.Text = "On";
            this.framerate_control.Text = "Auto";
            this.vsync.Text = "Enable";
            this.window_mode.Text = "FullScreenWindowed";
            ///this.ResolutionBox.Text = "1920" + "x" + "1080";

        }


        /// <summary>
        /// drobdown list event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_DropDownClosed (object sender, EventArgs e)
        {
            Console.WriteLine("ComboBox closed");
            if (handle) Handle();
            this.handle = true;
        }


        /// <summary>
        /// Dropbown if changed activator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyPreset (object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("Preset selected");
            ComboBox cmb = sender as ComboBox;
            this.handle = !cmb.IsDropDownOpen;
            Handle();
        }


        /// <summary>
        /// Apply preset to the settings options
        /// </summary>
        private void Handle()
        {
            if (this.PresetBox.SelectedItem != null)
            {
                switch (this.PresetBox.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last())
                {
                    case "Very High":
                        this.VeryHighPreset();
                        break;
                    case "High":
                        this.HighPreset();
                        break;
                    case "Medium":
                        this.MediumPreset();
                        break;
                    case "Low":
                        this.LowPreset();
                        break;
                    case "":
                        break;
                }
            }    
        }


        /// <summary>
        /// Load settings to the graphics settings dictionary from file
        /// </summary>
        /// <param name="settings"></param>
        public void LoadSettingsFromFile()
        {
            this.graphicSettings = new Dictionary<string, string>();
            string[] settingsSteam = File.ReadAllLines(this.steamConfigPath);

            for (int i = 4; i < 15; i++)
            {
                string key = settingsSteam[i].Replace(" ", "").Replace("\"", "").Trim().Split(':')[0];
                string value = settingsSteam[i].Replace(" ", "").Replace("\"", "").Trim().Split(':')[1].Replace(",", "");
                this.graphicSettings.Add(key, value);
            }
            for (int i = 18; i < 24; i++)
            {
                string key = settingsSteam[i].Replace(" ", "").Replace("\"", "").Trim().Split(':')[0];
                string value = settingsSteam[i].Replace(" ", "").Replace("\"", "").Trim().Split(':')[1].Replace(",", "");
                this.graphicSettings.Add(key, value);
            }
        }

        /// <summary>
        /// Save settings to a file
        /// </summary>
        /// <param name="settings"></param>
        private void SaveSettings(Dictionary<string, string> finalsettings)
        {
            string[] settings = File.ReadAllLines($"{this.steampath.Trim()}userdata\\{this.userid.Trim()}\\287700\\local\\TPP_GRAPHICS_CONFIG");
            List<string> temp_settings = new List<string>();

            foreach (string line in settings)
            {
                temp_settings.Add(line);
            }

            foreach (KeyValuePair<string, string> keyvalue in finalsettings)
            {
                foreach (string x in temp_settings)
                {
                    if (x.Contains($"\"{keyvalue.Key}\"") && keyvalue.Key != "display_index")
                    {
                        if (keyvalue.Key == "width" || keyvalue.Key == "height" || keyvalue.Key == "display_index")
                        {
                            Console.WriteLine(keyvalue.Key);
                            string k = $"{x.Split(new string[] { " : " }, StringSplitOptions.None)[0]} : {finalsettings[keyvalue.Key]},";
                            Console.WriteLine(k);
                            temp_settings[temp_settings.IndexOf(x)] = k;
                            break;
                        } else if (keyvalue.Key == "volumetric_clouds" || keyvalue.Key == "window_mode")
                        {
                            Console.WriteLine(keyvalue.Key);
                            string k = $"{x.Split(new string[] { " : " }, StringSplitOptions.None)[0]} : \"{finalsettings[keyvalue.Key]}\"";
                            Console.WriteLine(k);
                            temp_settings[temp_settings.IndexOf(x)] = k;
                            break;
                        } else
                        {
                            Console.WriteLine(keyvalue.Key);
                            string k = $"{x.Split(new string[] { " : " }, StringSplitOptions.None)[0]} : \"{finalsettings[keyvalue.Key]}\",";
                            Console.WriteLine(k);
                            temp_settings[temp_settings.IndexOf(x)] = k;
                            break;
                        }
                    }
                }
            }

            foreach (string x in temp_settings)
            {
                Console.WriteLine(x);
            }

            File.WriteAllLines($"C:\\MGSV_saves\\{this.CurrentUserSettings.Text}\\{this.currentSave}\\287700\\local\\TPP_GRAPHICS_CONFIG", temp_settings);
            string command = $"copy /Y C:\\MGSV_saves\\{this.CurrentUserSettings.Text}\\{this.currentSave}\\287700\\local\\TPP_GRAPHICS_CONFIG {this.steampath.Trim()}userdata\\{this.userid}\\287700\\local\\";
            Console.WriteLine(command);
            this.CommandPrompter(command);
        }


        /// <summary>
        /// Cancel settings, nothing saved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// Apply graphics settings and close settings window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplySettings_Click(object sender, RoutedEventArgs e)
        {
            /// Get settings from the dropdown menus
            Dictionary<string, string> settingslist = new Dictionary<string, string>
            {
                { "depth_of_field", this.depth_of_field.Text },
                { "effect", this.effect.Text },
                { "lighting", this.lighting.Text },
                { "model_detail", this.model_detail.Text },
                { "motion_blur_amount", this.motion_blur_amount.Text },
                { "postprocess", this.postprocess.Text },
                { "shadow", this.shadow.Text },
                { "ssao", this.ssao.Text },
                { "texture", this.texture.Text },
                { "texture_filtering", this.texture_filtering.Text },
                { "volumetric_clouds", this.volumetric_clouds.Text },
                { "vsync", this.vsync.Text },
                { "window_mode", this.window_mode.Text },
                { "width", this.ResolutionBox.Text.Split('x')[0] },
                { "height", this.ResolutionBox.Text.Split('x')[1]},
                { "framerate_control", this.framerate_control.Text }
            };
            /// Save settings and copy to the currently used save, close window
            this.SaveSettings(settingslist);
            this.LoadSettingsFromFile();
            this.Close();
        }


        /// <summary>
        /// Revert settings to last saved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Revert_Click(object sender, RoutedEventArgs e)
        {
            this.LoadSettings();
        }


        /// <summary>
        /// Use Windows Command prompt
        /// </summary>
        /// <param name="command"></param>
        private void CommandPrompter(string command)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo()
            {
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = $"/C {command}"
            };
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
    }
}