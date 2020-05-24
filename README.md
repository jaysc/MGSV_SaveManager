# MGSV Save Manager

This has been modified slightly since it wasn't working as I wanted it to. These are just minor tweaks and I have tried to keep the core logic the same.

- Asks for steam and game installation location
  - Previously it would try and scan your hard drives, now it will asks for the game location.
- Config files and logger is stored in the same folder where the application was ran. (Creates a new folder called `MGSV_SaveManager`)
- Minor UI tweaks

In general, I wouldn't call this program the most stable but seems to do the job.

---

If you experience bugs and errors, please create an issue with as accurate description of the issue as possible so I can try replicate and fix 
Download the latest release from the [Releases](https://github.com/thatsafy/MGSV_Save_Switcher/releases).  
Project wiki page on [Metal Gear Modding Wikia](http://metalgearmodding.wikia.com/wiki/Metal_Gear_Solid_V_Save_Manager)  
  
MGSV Save Manager is a handy, all in one management software to manage save files and configure in game graphical settings. The program is a single executable, which does not require installation and can be run from anywhere in the system, so just place it somewhere you can easily access it!  

![Metal Gear Solid V: The Phantom Pain](http://static.gosunoob.com/img/1/2015/08/mgsv-the-phantom-pain-tips.jpg)
  
## Current functionality
* Multiple users
* Multiple saves to switch between
* Save renaming
* Save deletion
* Creation of new saves
* Configure in-game graphics settings
  * Currently Resolution and Display not configurable via the manager.
* Logging
  
## How to use
**If you want to create new, empty saves, turn off Steam Cloud sync for both MGSV: The Phantom Pain and MGSV: Ground Zeroes. Otherwise Steam will download old cloud synced save.**
When first running, the program will create a local save directory in **C:\\MGSV\_saves**. All the save files are stored under **C:\\MGSV\_saves\\USERNAME**.  
* If you only have a single user, the program will select that user automatically.
* If multiple users are found, select your username from the list.
* On the first launch, the currently in use save will be backed up and named **"Original"**. You can use the **Rename** field to name it something else if you wish.
* **Create new saves** by typing a save name to the save name field and clicking **Apply**. The program will then switch to that save automatically and backup your previous save.
* **Switch between saves** by selecting a save from the list (only if more than one save found) and clicking **Apply**.
* **Delete saves** by selecting the save from the list and click **Apply**.
* If you add a save file manually while the program is running, you can click **Save Scan** to update the save lists.
If you encounter bugs, please create **an issue** with the **log.txt** attached and if possible a short description what you were trying to do. The log file is located in **%programdata%\\MGSV_SaveManager\\logs\\log.txt**.  