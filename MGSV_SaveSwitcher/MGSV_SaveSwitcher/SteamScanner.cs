using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ManagerLogger;

namespace SteamScan
{
    public class MySteamScanner
    {
        Logger myLogger;
        public string localDir;
        public string configPath;
        private string steamPath = "";
        private string gamePath = "";
        private string MGSV_Game = "mgsvtpp.exe";
        private static string MGSV1 = "287700";
        private static string MGSV2 = "311340";

        /// Files affecting save data
        private List<string> MGSV_Saves = new List<string>(new string[] {
            $"{MGSV1}\\local\\PERSONAL_DATA0",
            $"{MGSV1}\\local\\PERSONAL_DATA1",
            $"{MGSV1}\\local\\TPP_GAME_DATA0",
            $"{MGSV1}\\local\\TPP_GAME_DATA1",
            $"{MGSV2}\\remote\\TPP_CONFIG_DATA",
            $"{MGSV2}\\remote\\TPP_GAME_DATA",
            $"{MGSV2}\\remote\\PERSONAL_DATA"
        });

        private List<string> userNames = new List<string>();
        private Dictionary<string, string> NameID = new Dictionary<string, string>();
        

        /// <summary>
        /// 
        /// </summary>
        public MySteamScanner(string localpath, string configpath)
        {
            this.localDir = localpath;
            this.configPath = configpath;
            this.myLogger = new Logger(this.configPath);
        }


        /// <summary>
        /// Get users ID
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string GetUserID(string username)
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
                this.myLogger.LogToFile("Attempting reading steam path from the file.");
                Console.WriteLine("trying to find steam_path.txt");
                this.steamPath = File.ReadAllLines(Path.Combine(this.configPath, "steam_path.txt"))[0].Replace("\"", "");
                Console.WriteLine("Steam path found");
                this.gamePath = File.ReadAllLines(Path.Combine(this.configPath, "MGSV.txt"))[0].Replace("\"", "");
                this.myLogger.LogToFile($"Steam installed in {this.steamPath}");
                this.myLogger.LogToFile($"MGSV installed in {this.gamePath}");
                return this.steamPath;
            } catch (IOException e)
            {
                this.myLogger.LogToFile($"Steam or game path not yet set, {e}");
                Console.WriteLine("steam/mgsv path not found, scanning...");
                FirstRun();

                this.myLogger.LogToFile("Listing available drives.");
                string driveInfo = $"wmic LOGICALDISK LIST BRIEF > \"{Path.Combine(this.configPath, "drvs.txt")}\"";
                Console.WriteLine(driveInfo);
                CommandPrompter(driveInfo);
                Console.WriteLine(Path.Combine(this.configPath, "drvs.txt"));
                List<string> drives = File.ReadAllLines(Path.Combine(this.configPath, "drvs.txt")).ToList<string>();
                string findSteam;
                File.Delete(Path.Combine(this.configPath, "temp_path.txt"));

                this.myLogger.LogToFile("Scannning through the drives.");
                for (int i = 1; i < drives.Count; i++)
                {
                    drives[i] = drives[i][0] + ":";
                    findSteam = $"{drives[i]} && cd {drives[i]}\\ && dir /s /b Steam.exe >> \"{Path.Combine(this.configPath, "temp_path.txt")}\"";
                    CommandPrompter(findSteam);
                }
                Console.WriteLine(Path.Combine(this.configPath, "temp_path.txt"));
                List<string> temp_path = File.ReadAllLines(Path.Combine(this.configPath, "temp_path.txt")).ToList<string>();
                this.myLogger.LogToFile("Searching for Steam.exe.");
                foreach (string x in temp_path)
                {
                    this.myLogger.LogToFile($"Possible steam path: {x}");
                    /// Check if directory contains Steam.exe
                    if (x.Contains("Steam.exe"))
                    {
                        this.steamPath = x.Replace("Steam.exe", "").Replace("\"", "");
                        this.myLogger.LogToFile($"Searching steam from {this.steamPath}");
                        /// Check if the directory contains userdata folder
                        if (Directory.GetDirectories(this.steamPath, "userdata").Length > 0)
                        {
                            this.myLogger.LogToFile($"Steam found in {this.steamPath}");
                            /// Check if MGSV in Steam install directory
                            if (Directory.GetDirectories(Path.Combine(this.steamPath, "steamapps\\common"), "MGS_TPP").Length < 0)
                            {
                                File.WriteAllText(Path.Combine(this.configPath, "steam_path.txt"), this.steamPath.Trim());
                                File.Delete(Path.Combine(this.configPath, "temp_path.txt"));
                                File.Delete(Path.Combine(this.configPath, "drvs.txt"));
                                this.myLogger.LogToFile($"MGSV installation found in {this.steamPath}.");
                                this.gamePath = $"{Path.Combine(this.steamPath, "steamapps\\common\\MGS_TPP")}";
                                return this.steamPath;
                            }
                            /// If MGSV not found under Steam install directory, search elsewhere
                            else
                            {
                                this.myLogger.LogToFile($"Error: MGSV not found in the Steam install path, searching elsewhere...");
                                for (int i = 1; i < drives.Count; i++)
                                {
                                    drives[i] = drives[i][0] + ":";
                                    string findMGSV = $"{drives[i]} && cd {drives[i]}\\ && dir /s /b MGS_TPP >> \"{Path.Combine(this.configPath, "mgsv_path.txt")}\"";
                                    CommandPrompter(findMGSV);
                                }

                                List<string> mgsv_path = File.ReadAllLines(Path.Combine(this.configPath, "mgsv_path.txt")).ToList<string>();
                                foreach (string path in mgsv_path)
                                {
                                    this.myLogger.LogToFile($"Checking path {path}");
                                    if (Directory.GetFiles(Path.Combine(path.Replace("\"", "")), "mgsvtpp.exe").Length > 0)
                                    {
                                        this.gamePath = $"{path}";
                                        this.myLogger.LogToFile($"Found MGSV installation in {this.gamePath}");
                                        File.Delete(Path.Combine(this.configPath, "temp_path.txt"));
                                        File.Delete(Path.Combine(this.configPath, "mgsv_path.txt"));
                                        File.Delete(Path.Combine(this.configPath, "drvs.txt"));
                                        File.WriteAllText(Path.Combine(this.configPath, "steam_path.txt"), this.steamPath.Trim());
                                        File.WriteAllText($"{Path.Combine(this.configPath, "MGSV.txt")}", this.gamePath);
                                        return this.steamPath;
                                    }
                                }
                                this.myLogger.LogToFile("Error, MGSV installation not found.");
                                return "Error:No MGSV installation found.";
                            }
                        }
                    }
                }
                this.myLogger.LogToFile("Error: Steam not found.");
                return "Error: Steam installation not found.";
            }
        }


        /// <summary>
        /// Use Windows Command prompt
        /// </summary>
        /// <param name="command"></param>
        private void CommandPrompter(string command)
        {
            this.myLogger.LogToFile($"CLI command: {command}");
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


        /// <summary>
        /// Scan userdata folder for steam users
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> UserScan()
        {
            this.userNames = new List<string>();
            this.NameID = new Dictionary<string, string>();
            Console.WriteLine("Scanning for users.");
            string command = $"dir /b /AD \"{Path.Combine(this.steamPath.Trim(), "userdata")}\"> \"{Path.Combine(this.configPath, "users.txt")}\"";
            Console.WriteLine(command);
            CommandPrompter(command);
            try { 
                List<string> users = File.ReadAllLines(Path.Combine(this.configPath.Trim(), "users.txt")).ToList<string>();
                foreach (string userid in users)
                {
                    if (userid != "anonymous")
                    {
                        try
                        {
                            List<string> userData = File.ReadAllLines($"{this.steamPath}\\userdata\\{userid}\\config\\localconfig.vdf").ToList();
                            foreach (string line in userData)
                            {
                                if (line.Contains("PersonaName"))
                                {
                                    string username = line.Replace("PersonaName", "").Replace("		", "").Replace("\"", "").ToLower();
                                    this.myLogger.LogToFile($"Found user {username} with userid {userid}.");
                                    Console.WriteLine(username);
                                    if (this.NameID.ContainsKey(username))
                                    {
                                        this.NameID.Add(username + "_", userid);
                                        this.userNames.Add(username + "_");
                                    }
                                    else
                                    {
                                        this.NameID.Add(username, userid);
                                        this.userNames.Add(username);
                                    }
                                }
                            }
                            Console.WriteLine("User scanned, found " + this.NameID.Count + " users.");
                        }
                        catch (IOException)
                        {
                            Console.WriteLine("localconfig.vdf not found.");
                        }
                    }
                }
                return this.NameID;
            } catch (IOException e)
            {
                this.myLogger.LogToFile($"Error: {e}");
                Console.WriteLine($"Error: {e}");
                return this.NameID;
            }
        }


        /// <summary>
        /// First run, create the local folder
        /// </summary>
        public void FirstRun()
        {
            Directory.CreateDirectory(this.localDir);
            this.myLogger.LogToFile($"Created save directory in {this.localDir}");
        }


        /// <summary>
        /// Scan local folder for save files
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public List<string> SaveScan(string username)
        {
            this.myLogger.LogToFile("Scanning for saves.");
            Directory.CreateDirectory(Path.Combine(this.localDir, username));
            string command = $"{this.localDir[0]}: && dir /b /AD \"{Path.Combine(this.localDir, username)}\" > \"{Path.Combine(this.localDir, username, "saves.txt")}\"";
            CommandPrompter(command);
            List<string> saves_temp = File.ReadAllLines(Path.Combine(this.localDir, username, "saves.txt")).ToList<string>();

            Console.WriteLine(saves_temp.Count + " Saves found");
            this.myLogger.LogToFile($"{saves_temp.Count} saves found.");
            
            if (saves_temp.Count < 1 && username != "")
            {
                Console.WriteLine("No saves found, creating new one");
                this.FirstSave("original", username);
                this.ChangeCurrentSave("original", username);
            } else if (!saves_temp.Contains(CurrentSave(username)) && username != "")
            {
                Console.WriteLine("Saves found, but the current_save not among them.");
                this.FirstSave(this.CurrentSave(username), username);
            } else if (saves_temp.Count == 1 && username != "")
            {
                this.SteamToLocal(saves_temp[0], username);
                this.ChangeCurrentSave(saves_temp[0], username);
            }
            Console.WriteLine("Save Scan completed.");
            return saves_temp;
        }


        /// <summary>
        /// copy from steam to save manager directory
        /// </summary>
        /// <param name="currentSave"></param>
        /// <param name="username"></param>
        public void SteamToLocal(string currentSave, string username)
        {
            Console.WriteLine("Steam to local starting");
            Console.WriteLine(this.steamPath);
            string command;

            try
            {
                command = $"xcopy /E /Y \"{Path.Combine(this.steamPath, "userdata", this.NameID[username], MGSV1)}\" \"{Path.Combine(this.localDir, username, currentSave, MGSV1)}\" 2> nul";
                
            } catch 
            {
                command = $"xcopy /E /Y \"{Path.Combine(this.steamPath, "userdata", this.GetCurrentID(), MGSV1)}\" \"{Path.Combine(this.localDir, username, currentSave, MGSV1)}\" 2> nul";
            }
            Console.WriteLine(command);
            this.CommandPrompter(command);

            try
            {
                command = $"xcopy /E /Y \"{Path.Combine(this.steamPath, "userdata", this.NameID[username], MGSV2)}\" \"{Path.Combine(this.localDir, username, currentSave, MGSV2)}\" 2> nul";
            } catch
            {
                command = $"xcopy /E /Y \"{Path.Combine(this.steamPath, "userdata", this.GetCurrentID(), MGSV2)}\" \"{Path.Combine(this.localDir, username, currentSave, MGSV2)}\" 2> nul";
            }
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

            string command = $"xcopy /E /Y \"{Path.Combine(this.localDir, username, currentSave, MGSV1)}\" \"{Path.Combine(this.steamPath, "userdata", this.NameID[username], MGSV1)}\" 2> nul";
            Console.WriteLine(command);
            this.CommandPrompter(command);

            command = $"xcopy /E /Y \"{Path.Combine(this.localDir, username, currentSave, MGSV2)}\" \"{Path.Combine(this.steamPath, "userdata", this.NameID[username], MGSV2)}\" 2> nul";
            Console.WriteLine(command);
            this.CommandPrompter(command);

            Console.WriteLine("Local to steam complete");
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
                string currentsave = File.ReadAllLines(Path.Combine(this.localDir, username, "current_save.txt"))[0];
                return currentsave.Trim();
            } catch (IOException e)
            {
                this.myLogger.LogToFile($"Error: {e}");


                string currentsave = "none";
                File.WriteAllText(Path.Combine(this.localDir, username, "current_save.txt"), currentsave);
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
            Directory.CreateDirectory(Path.Combine(this.localDir, username, savename));
            Directory.CreateDirectory(Path.Combine(this.localDir, username, savename, MGSV1));
            Directory.CreateDirectory(Path.Combine(this.localDir, username, savename, MGSV2));

            this.SteamToLocal(savename, username);
            this.ChangeCurrentSave(savename, username);
        }


        /// <summary>
        /// Save current user to a file
        /// </summary>
        /// <param name="username"></param>
        public void CurrentUser(string username, string userid)
        {
            try
            {
                File.WriteAllText(Path.Combine(this.configPath, "currentuser.txt"), $"{username}\n{userid}");
            } catch (IOException e)
            {
                this.myLogger.LogToFile($"Error: {e}");
            }
            
        }


        /// <summary>
        /// Get current user from the file.
        /// </summary>
        /// <returns></returns>
        public string GetCurrentUser()
        {
            return File.ReadAllLines(Path.Combine(this.configPath, "currentuser.txt"))[0].Trim();
        }


        /// <summary>
        /// Get userid of the currently used user
        /// </summary>
        /// <returns></returns>
        public string GetCurrentID()
        {
            return File.ReadAllLines(Path.Combine(this.configPath, "currentuser.txt"))[1].Trim();
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
                File.Delete(Path.Combine(this.steamPath, "userdata", userid, data));
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
            File.WriteAllText(Path.Combine(this.localDir, username, "current_save.txt"), save);
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

            Directory.CreateDirectory(Path.Combine(this.localDir, username, savename));
            Directory.CreateDirectory(Path.Combine(this.localDir, username, savename, MGSV1));
            Directory.CreateDirectory(Path.Combine(this.localDir, username, savename, MGSV2));

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
            Directory.Move(Path.Combine(this.localDir, username, save), Path.Combine(this.localDir, username, newname));
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
            Directory.Delete(Path.Combine(this.localDir, username, save), true);
            
            if (save == CurrentSave(username))
            {
                Console.WriteLine("save was current save, changing save to first save found");
                this.myLogger.LogToFile("Deleted currently used save, changing to the first save found.");
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
            try
            {
                Console.WriteLine("Launching MGSV: TPP game");
                Console.WriteLine($"{Path.Combine(this.gamePath, this.MGSV_Game)}");
                System.Diagnostics.Process.Start($"{Path.Combine(this.gamePath, this.MGSV_Game)}");
            } catch (Exception e)
            {
                this.myLogger.LogToFile($"Error while launching MGSV, {e}");
            }
            
        }

    }
}