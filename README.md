# MGSV_Save_Switcher
**This script is still under development and bugs might appear. Most of fatal bugs should be dealth with, so the saves should be pretty safe and I try to catch as many bugs as I can before pushing new features out.**
A Python 3 script to easily switch between multiple Metal Gear Solid V: The Phantom Pain save files and create new, empty saves.  
This script at this development stage requires [Python 3](https://www.python.org/) to be installed on your computer.  
If you experience bugs and errors, please create an issue with as accurate description of the issue as possible so I can try replicate and fix it.  
![Metal Gear Solid V: The Phantom Pain](http://static.gosunoob.com/img/1/2015/08/mgsv-the-phantom-pain-tips.jpg)
## Current functionality
* Support for multiple users. A save folder for each user to which their saves are, well, saved.
* Switching between multiple save files.
* Create a new save file
* Current save file used
* Delete a save file
* Automatically detects Steam users in the Steam directory
* Automatically locates Steam directory
  * The functionality of this might vary depending if you have some exotic setup. Should work in most cases, where Steam have been installed normally (either to default location or to an other drive/directory).
## Planned features
* Graphical user interface
* Executable without requiring user installed Python 3 (currently testing and trying to implement it by release version 1.0)
* Save deletion
## How to use
* For time being, Python 3 is required to be installed. Install it from [Python website](https://www.python.org/)
* Before running this script, please mind the following prerequisites
  * In order for this script to create new, empty files, disable Cloud Sync in Steam game properties for MGSV: The Phantom Pain and (if installed, MGSV: Ground Zeroes).
  * If you want to backup your save before running this script, head to Steam directory > userdata > your-user-id > **287700** and **311340** are the folders including MGSV files. Copy these somewhere safe.
* Run the mgsv.py file
  * If Python is correctly installed, you should be able to just double click the mgsv.py file to run it
  * If this doesn't work, you can try to launch it from command prompt
    * Press **WinKey+R**
    * Type **cmd** and press **Enter**
    * navigate to the MGSV Save Switcher folder with **cd** command. Use **help cd** if you need help with the syntax.
    * In the script folder, type **python mgsv.py** to run the script.
  * Select your username from the list by typing the **number** left to the username and press **Enter**
  * During first run:
    * Script will ask you to name the **currently** used MGSV: TPP save.
  * Afterwards the similarly to the user selection, use the number next to the Save name to switch between saves the script has found.
  * To create a new, empty save, use .bat shortcut **MGSV\_SaveSwitcher\_NewSave.bat** in the **MGSV_saves** folder (default location **C:\\MGSV\_saves**)
  * To check currently used save, use .bat shortcut **MGSV\_SaveSwitcher\_CurrentSave.bat** in the **MGSV_saves** folder (default location **C:\\MGSV\_saves**)
  * There is also a script for normal script use, **MGSV\_SaveSwitcher.bat** in the **MGSV_saves** folder (default location **C:\\MGSV\_saves**)
  * These .bat shortcuts can be used from outside the save folder as well, if for example you want to have them in your Desktop or some other, easily accessible directory.