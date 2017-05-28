using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SteamScan
{
    public class MySteamScanner
    {
        private string localDir = "C:\\MGSV_saves";
        private string steamPath = "";
        private List<string> userNames = new List<string>();
        
        private string MGSV_Game = "steamapps\\common\\MGS_TPP\\mgsvtpp.exe";
        private string MGSV1 = "287700";
        private string MGSV2 = "311340";
        private Dictionary<string, string> NameID = new Dictionary<string, string>();
        private List<string> MGSV_Saves = new List<string>();
        

        /// <summary>
        /// When When steam scanner created, add mgsv save files to the list.
        /// </summary>
        public MySteamScanner()
        {
            this.MGSV_Saves.Add($"{this.MGSV1}\\local\\PERSONAL_DATA0");
            this.MGSV_Saves.Add($"{this.MGSV1}\\local\\PERSONAL_DATA1");
            this.MGSV_Saves.Add($"{this.MGSV1}\\local\\TPP_GAME_DATA0");
            this.MGSV_Saves.Add($"{this.MGSV1}\\local\\TPP_GAME_DATA1");
            this.MGSV_Saves.Add($"{this.MGSV2}\\remote\\TPP_CONFIG_DATA");
            this.MGSV_Saves.Add($"{this.MGSV2}\\remote\\TPP_GAME_DATA");
            this.MGSV_Saves.Add($"{this.MGSV2}\\remote\\PERSONAL_DATA");
        }


        /// <summary>
        /// Get users ID
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string getUserID(string username)
        {
            return this.NameID[username];
        }


        /// <summary>
        /// Find Steam install directory
        /// </summary>
        public string ScanSteam()
        {
            try
            {
                Console.WriteLine("trying to find steam_path.txt");
                this.steamPath = File.ReadAllLines($"{this.localDir}\\steam_path.txt")[0];
                Console.WriteLine("Steam path found");
                return this.steamPath;
            } catch 
            {
                Console.WriteLine("steam path not found, scanning...");
                FirstRun();

                string driveInfo = $"wmic LOGICALDISK LIST BRIEF > {this.localDir}\\drvs.txt";
                CommandPrompter(driveInfo);

                string[] drives = File.ReadAllLines($"{this.localDir}\\drvs.txt");
                string findSteam;
                string clearPathFile = $"{localDir[0]}: && del /Q {this.localDir}\\temp_path.txt 2> nul";
                CommandPrompter(clearPathFile);

                for (int i = 1; i < drives.Length; i++)
                {
                    drives[i] = drives[i][0] + ":";
                    findSteam = $"{drives[i]} && dir /s /b Steam.exe >> {this.localDir}\\temp_path.txt";
                    CommandPrompter(findSteam);
                }

                string[] temp_path = File.ReadAllLines($"{this.localDir}\\temp_path.txt");
                foreach (string x in temp_path)
                {
                    if (x.Contains("Steam.exe"))
                    {
                       
                        this.steamPath = x.Replace("Steam.exe", "").Trim();
                        this.CommandPrompter($"dir /b {this.steamPath} > {this.localDir}\\tempdir.txt");
                        foreach (string y in File.ReadAllLines($"{this.localDir}\\tempdir.txt"))
                        {
                            if (y.Contains("userdata"))
                            {
                                Console.WriteLine("Found steam!");
                                Console.WriteLine($"echo {this.steamPath.Trim()} > {this.localDir.Trim()}\\steam_path.txt");
                                this.CommandPrompter($"echo {this.steamPath.Trim()} > {this.localDir.Trim()}\\steam_path.txt");
                                return this.steamPath;
                            }
                        }
                        
                    }
                }
                return "";
            }
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


        /// <summary>
        /// Scan for users
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> UserScan()
        {
            Console.WriteLine("Scanning for users.");
            string userScan = $"{this.steamPath[0]}: && cd {this.steamPath.Trim()}userdata && dir /b /AD > {this.localDir.Trim()}\\users.txt";
            Console.WriteLine(userScan);
            CommandPrompter(userScan);
            string[] users = File.ReadAllLines($"{this.localDir.Trim()}\\users.txt");
            foreach (string userid in users)
            {
                if (userid != "anonymous")
                {
                    string[] userData = File.ReadAllLines($"{this.steamPath}\\userdata\\{userid}\\config\\localconfig.vdf");
                    foreach (string line in userData)
                    {
                        if (line.Contains("PersonaName"))
                        {
                            string username = line.Replace("PersonaName", "").Replace("		", "").Replace("\"", "").ToLower();

                            if (this.NameID.ContainsKey(username.ToLower()))
                            {
                                this.NameID.Add(username + "_", userid);
                                this.userNames.Add(username + "_");
                            } else
                            {
                                this.NameID.Add(username, userid);
                                this.userNames.Add(username);
                            }                            
                        }
                    }
                }
            }
            Console.WriteLine("User scanned, found " + this.NameID.Count + " users.");
            return this.NameID;
        }


        /// <summary>
        /// First run, create the local folder
        /// </summary>
        public void FirstRun()
        {
            Console.WriteLine("Creating local directory.");
            string command = $"mkdir {this.localDir} 2> nul";
            CommandPrompter(command);
        }


        /// <summary>
        /// Scan local folder for save files
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public List<string> SaveScan(string username)
        {
            Console.WriteLine("Save scanning starting");
            string command = $"mkdir {this.localDir}\\{username} 2> nul";
            Console.WriteLine(command);
            CommandPrompter(command);
            command = $"{this.localDir[0]}: && dir /b /AD {this.localDir}\\{username} > {this.localDir}\\{username}\\saves.txt";
            CommandPrompter(command);

            string[] saves_temp = File.ReadAllLines($"{this.localDir}\\{username}\\saves.txt");
            Console.WriteLine("saves.txt read");
            List<string> saves = new List<string>();
            foreach (string x in saves_temp)
            {
                saves.Add(x);
            }
            Console.WriteLine("saves copied to list");
            Console.WriteLine(saves.Count + " Saves found");
            
            if (saves.Count < 1 && username != "")
            {
                Console.WriteLine("No saves found, creating new one");
                this.FirstSave("original", username);
                this.ChangeCurrentSave("original", username);
            } else if (!saves.Contains(CurrentSave(username)) && username != "")
            {
                Console.WriteLine("Saves found, but the current_save not among them.");
                this.FirstSave(this.CurrentSave(username), username);
            } else if (saves.Count == 1 && username != "")
            {
                this.SteamToLocal(saves[0], username);
                this.ChangeCurrentSave(saves[0], username);
            }
            Console.WriteLine("Save Scan completed.");
            return saves;
        }


        /// <summary>
        /// copy from steam to save manager directory
        /// </summary>
        /// <param name="currentSave"></param>
        /// <param name="username"></param>
        public void SteamToLocal(string currentSave, string username)
        {
            Console.WriteLine("Steam to local starting");
            string command = $"xcopy /E /Y {this.steamPath.Trim()}userdata\\{this.NameID[username].Trim()}\\{this.MGSV1} {localDir}\\{username}\\{currentSave}\\{this.MGSV1} 2> nul";
            Console.WriteLine(command);
            this.CommandPrompter(command);
            command = $"xcopy /E /Y {this.steamPath.Trim()}userdata\\{this.NameID[username].Trim()}\\{this.MGSV2} {localDir}\\{username}\\{currentSave}\\{this.MGSV2} 2> nul";
            Console.WriteLine(command);
            this.CommandPrompter(command);
            Console.WriteLine("Steam to local completed");
        }


        /// <summary>
        /// Copy local save to steam directory
        /// </summary>
        /// <param name="currentSave"></param>
        /// <param name="username"></param>
        public void LocalToSteam(string currentSave, string username)
        {
            Console.WriteLine("local to steam starting");
            string command = $"xcopy /E /Y {this.localDir}\\{username}\\{currentSave}\\{this.MGSV1} {this.steamPath.Trim()}userdata\\{this.NameID[username].Trim()}\\{this.MGSV1} 2> nul";
            Console.WriteLine(command);
            this.CommandPrompter(command);
            command = $"xcopy /E /Y {this.localDir}\\{username}\\{currentSave}\\{this.MGSV2} {this.steamPath.Trim()}userdata\\{this.NameID[username].Trim()}\\{this.MGSV2} 2> nul";
            Console.WriteLine(command);
            this.CommandPrompter(command);
            Console.WriteLine("Local to steam complet");
        }


        /// <summary>
        /// Retrieve current save
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string CurrentSave(string username)
        {
            try
            {
                string currentsave = File.ReadAllLines($"{localDir}\\{username}\\current_save.txt")[0];
                return currentsave.Trim();
            } catch
            {
                string currentsave = "none";
                string command = $"echo {currentsave} > {this.localDir}\\{username}\\current_save.txt";
                this.CommandPrompter(command);
                return currentsave.Trim();
            }
                
        }


        /// <summary>
        /// Backup steam save for the previous user
        /// </summary>
        /// <param name="prevuser"></param>
        public void ChangeUser(string prevuser)
        {
            Console.WriteLine("Changing user.");
            SteamToLocal(CurrentSave(prevuser), prevuser);
        }

        /// <summary>
        /// Create the first save
        /// </summary>
        /// <param name="savename"></param>
        /// <param name="username"></param>
        public void FirstSave(string savename, string username)
        {
            Console.WriteLine("first save for user " + username);
            string command = $"mkdir {this.localDir}\\{username}\\{savename}";
            this.CommandPrompter(command);
            command = $"mkdir {this.localDir}\\{username}\\{savename}\\{this.MGSV1}";
            this.CommandPrompter(command);
            command = $"mkdir {this.localDir}\\{username}\\{savename}\\{this.MGSV2}";
            this.CommandPrompter(command);
            this.SteamToLocal(savename, username);
            this.ChangeCurrentSave(savename, username);
        }


        /// <summary>
        /// Switch save files
        /// </summary>
        /// <param name="nextsave"></param>
        /// <param name="username"></param>
        public void SaveSwitch(string nextsave, string username)
        {
            Console.WriteLine("Starting save switch.");
            string currentsave = CurrentSave(username);
            if (!this.SaveScan(username).Contains(currentsave))
            {
                this.FirstSave(currentsave, username);
            } else
            {
                this.SteamToLocal(currentsave, username);
            }
            this.DeleteSteamFiles(this.NameID[username]);
            this.LocalToSteam(nextsave, username);
            this.ChangeCurrentSave(nextsave, username);
            Console.WriteLine("Save switch done");
        }


        /// <summary>
        /// Delete the game save files in steam userdata
        /// </summary>
        public void DeleteSteamFiles(string userid)
        {
            Console.WriteLine("Deleting steam files.");
            foreach (string data in this.MGSV_Saves)
            {
                string command = $"del /Q {this.steamPath.Trim()}userdata\\{userid}\\{data}";
                Console.WriteLine(command + " file deleted");
                this.CommandPrompter(command);
            }
            Console.WriteLine("steam files deleted");
        }


        /// <summary>
        /// Update current save file
        /// </summary>
        /// <param name="save"></param>
        /// <param name="username"></param>
        public void ChangeCurrentSave(string save, string username)
        {
            Console.WriteLine("Updating current_save file");
            string command = $"echo {save} > {this.localDir}\\{username}\\current_save.txt";
            this.CommandPrompter(command);
            Console.WriteLine("current_save.txt updated");
        }


        /// <summary>
        /// New save file
        /// </summary>
        /// <param name="savename"></param>
        /// <param name="username"></param>
        public void NewSave(string savename, string username)
        {
            Console.WriteLine("creating new save");
            string current = CurrentSave(username);
   
            this.SteamToLocal(current, username);
            string command = $"mkdir {this.localDir}\\{username}\\{savename}";
            this.CommandPrompter(command);
            command = $"mkdir {this.localDir}\\{username}\\{savename}\\{this.MGSV1} 2> nul";
            this.CommandPrompter(command);
            command = $"mkdir {this.localDir}\\{username}\\{savename}\\{this.MGSV2} 2> nul";
            this.CommandPrompter(command);
            this.DeleteSteamFiles(this.NameID[username]);
            this.SteamToLocal(savename, username);
            
            this.ChangeCurrentSave(savename, username);
            Console.WriteLine("New save created");
        }


        /// <summary>
        /// Rename save file
        /// </summary>
        /// <param name="save"></param>
        /// <param name="username"></param>
        /// <param name="newname"></param>
        public void RenameSave(string save, string username, string newname)
        {
            Console.WriteLine("renaming save " + save + " to " + newname);
            string command = $"rename {this.localDir}\\{username}\\{save.Trim()} {newname}";
            Console.WriteLine(command);
            this.CommandPrompter(command);
            ChangeCurrentSave(newname, username);
            Console.WriteLine("rename done.");
        }


        /// <summary>
        /// Delete save file
        /// </summary>
        /// <param name="save"></param>
        /// <param name="username"></param>
        public void DeleteSave(string save, string username)
        {
            Console.WriteLine("Deleting save");
            string command = $"rmdir /S /Q {localDir}\\{username}\\{save}";
            this.CommandPrompter(command);
            
            if (save == CurrentSave(username))
            {
                Console.WriteLine("save was current save, changing save to first save found");
                this.DeleteSteamFiles(this.NameID[username]);
                string cursave = this.SaveScan(username)[0];
                this.ChangeCurrentSave(cursave, username);
                this.LocalToSteam(cursave, username);
            }
            Console.WriteLine("deletion completed.");
        }


        /// <summary>
        /// Launch the game
        /// </summary>
        public void LaunchGame()
        {
            Console.WriteLine("Launching MGSV: TPP game");
            Console.WriteLine($"{this.steamPath.Trim()}{MGSV_Game}");
            System.Diagnostics.Process.Start($"{this.steamPath.Trim()}{MGSV_Game}");
        }

    }
}

