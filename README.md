# MGSV_Save_Switcher
A python 3 script to easily switch between multiple Metal Gear Solid V: The Phantom Pain save files.
Current version supports only running through Windows 10 Bash with Python 3.
To enable Windows 10 Linux Bash, follow this installation guide: [Linux Subsystem on Windows 10](https://msdn.microsoft.com/en-us/commandline/wsl/install_guide)

## Current functionality
* Switching between multiple save files.
* Create a new save file.
  * Currently only backs up the current save file, and then deletes it.
* Current save file used.
* Temporary and configure files saved in the same directory tree as the save files.

## Script Usage
* This script doesn't really care where you run it from, as long as steam_finder.py and mgsv.py are in the same directory.
* Save files saved on default to root of C: drive into "MGSV Saves" folder, if you want you can change the drive by editing mgsv.py file.
* During first time running (or missing locations.txt file inside the local directory), the script will create .bat shortcuts, one for each of functionality. You can run these .bat shortcuts anywhere for easy use of this script. 
* For save file switcing:
  * in **bash**: **python3 mgsv.py** (will be renaming this later)
    * When running first time, the script tries to scan for Steam directory, this might take a couple of moments so don't panic!
    * Select save file to use by writing the **number** of save and press **Enter**
    * You are now running the selected save file! Test it by launching the game!
* To check current save file in use
  * in **bash**: **python3 mgsv.py v** or **python3 mgsv.py version**
* To create a new save
  * in **bash**: **python3 mgsv.py n** or **python3 mgsv.py new**
    * Choose a save name and press enter
    * Confirm by typing either Y or N (default)
    * Currently used save is backed up and then deleted.
      * This method currently also removes all the settings, so you probably want to configure those after getting to the main menu.