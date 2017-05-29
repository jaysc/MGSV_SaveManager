using System;
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
using System.Windows.Shapes;
using System.IO;
using System.Reflection;

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
        

        public SettingsWindow(string currentUser, string steamPath, string userid, string currentSave)
        {
            InitializeComponent();
            this.CurrentUserSettings.Text = currentUser;
            this.steampath = steamPath;
            this.userid = userid;
            this.currentSave = currentSave;
            this.CurrentSaveSettings.Text = currentSave;
            this.localPath += this.CurrentUserSettings.Text + "\\";
            this.steamConfigPath = $"{this.steampath.Trim()}userdata\\{this.userid.Trim()}\\287700\\local\\TPP_GRAPHICS_CONFIG";
            this.LoadSettings(this.graphicSettings);
        }


        /// <summary>
        /// Load the settings from the file
        /// </summary>
        private void LoadSettings(Dictionary<string, string> settings)
        {
            settings = this.LoadSettingsFromFile(settings, this.steamConfigPath);

            foreach (KeyValuePair<string, string> line in this.graphicSettings)
            {
                Console.WriteLine($"Line: {line.Key} : {line.Value}");
            }
            this.PresetBox.Text = "";
            this.depth_of_field.Text = settings["depth_of_field"];
            this.effect.Text = settings["effect"];
            this.lighting.Text = settings["lighting"];
            this.model_detail.Text = settings["model_detail"];
            this.motion_blur_amount.Text = settings["motion_blur_amount"];
            this.postprocess.Text = settings["postprocess"];
            this.shadow.Text = settings["shadow"];
            this.ssao.Text = settings["ssao"];
            this.texture.Text = settings["texture"];
            this.texture_filtering.Text = settings["texture_filtering"];
            this.volumetric_clouds.Text = settings["volumetric_clouds"];
            this.framerate_control.Text = settings["framerate_control"];
            this.vsync.Text = settings["vsync"];
            this.window_mode.Text = settings["window_mode"];
            this.ResolutionBox.Text = settings["width"] + "x" + settings["height"];
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
            this.ResolutionBox.Text = "1920" + "x" + "1080";
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
            this.ResolutionBox.Text = "1920" + "x" + "1080";
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
            this.ResolutionBox.Text = "1920" + "x" + "1080";
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
            this.ResolutionBox.Text = "1920" + "x" + "1080";

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
        public Dictionary<string, string> LoadSettingsFromFile(Dictionary<string, string> settings, string path)
        {
            settings = new Dictionary<string, string>();
            string[] settingsSteam = File.ReadAllLines(path);

            for (int i = 4; i < 15; i++)
            {
                string key = settingsSteam[i].Replace(" ", "").Replace("\"", "").Split(':')[0];
                string value = settingsSteam[i].Replace(" ", "").Replace("\"", "").Split(':')[1].Replace(",", "");
                settings.Add(key, value);
            }
            for (int i = 18; i < 24; i++)
            {
                string key = settingsSteam[i].Replace(" ", "").Replace("\"", "").Split(':')[0];
                string value = settingsSteam[i].Replace(" ", "").Replace("\"", "").Split(':')[1].Replace(",", "");
                settings.Add(key, value);
            }
            return settings;
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

            Dictionary<string, string> settingslist = new Dictionary<string, string>();

            settingslist.Add("depth_of_field", this.depth_of_field.Text);
            settingslist.Add("effect", this.effect.Text);
            settingslist.Add("lighting", this.lighting.Text);
            settingslist.Add("model_detail", this.model_detail.Text);
            settingslist.Add("motion_blur_amount", this.motion_blur_amount.Text);
            settingslist.Add("postprocess", this.postprocess.Text);
            settingslist.Add("shadow", this.shadow.Text);
            settingslist.Add("ssao", this.ssao.Text);
            settingslist.Add("texture", this.texture.Text);
            settingslist.Add("texture_filtering", this.texture_filtering.Text);
            settingslist.Add("volumetric_clouds", this.volumetric_clouds.Text);
            settingslist.Add("vsync", this.vsync.Text);
            settingslist.Add("window_mode", this.window_mode.Text);
            settingslist.Add("width", this.ResolutionBox.Text.Split('x')[0]);
            settingslist.Add("height", this.ResolutionBox.Text.Split('x')[1]);
            settingslist.Add("framerate_control", this.framerate_control.Text);

            /// Save settings and copy to the currently used save
            this.SaveSettings(settingslist);
            this.LoadSettingsFromFile(this.graphicSettings, this.steamConfigPath);

            this.Close();
        }


        /// <summary>
        /// Revert settings to last saved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Revert_Click(object sender, RoutedEventArgs e)
        {
            this.LoadSettings(this.graphicSettings);
        }


        /// <summary>
        /// Use Windows Command prompt
        /// </summary>
        /// <param name="command"></param>
        private void CommandPrompter(string command)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C {command}";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

    }
}