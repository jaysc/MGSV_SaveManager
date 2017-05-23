# MGSV\_Save\_Switcher
**This script is still under development and bugs might appear. Most of fatal bugs should be dealth with, so the saves should be pretty safe and I try to catch as many bugs as I can before pushing new features out.**  
A Python 3 script to easily switch between multiple **Metal Gear Solid V: The Phantom Pain** save files and create new, empty saves.  
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

## How to use
There are two version of script available. If you have Python 3 installed, you can use the MGSV\_Python and the Python scripts inside it. If you don't have Python installed or otherwise want to use standalone executables, you can use MGSV_Standalone. Just make sure you download the entire folder and keep all the files inside it. Where you put the folder doesn't matter.  
* For the Python version, Python 3 is required to be installed. Install it from [Python website](https://www.python.org/)
* Before running this script, please mind the following prerequisites
  * In order for this script to create new, empty files, disable Cloud Sync in Steam game properties for MGSV: The Phantom Pain and (if installed, MGSV: Ground Zeroes).
  * If you want to backup your save before running this script, head to Steam directory > userdata > your-user-id > **287700** and **311340** are the folders including MGSV files. Copy these somewhere safe.  
* With the **standalone** version
  * In the **MGSV_Standalone** folder, just run the **MGSV\_SaveSwitcher.exe** file.
* With the **Python 3** version
  * In **MGSV_Python** folder, run **MGSV\_SaveSwitcher.py** file.
  * If this doesn't work, you can try to launch it from command prompt
    * Press **WinKey+R**
    * Type **cmd** and press **Enter**
    * navigate to the MGSV Save Switcher folder with **cd** command. Use **help cd** if you need help with the syntax.
    * In the script folder, type **python MGSV\_SaveSwitcher.py** to run the script.
  * Since you have python installed, you can also run .bat scripts inside the local folder
    * More about these in the **How to use** section below.

# .bat script usage (Requires Python 3 installed)
* To create a new, empty save, use .bat shortcut **MGSV\_SaveSwitcher\_MGSV\_SaveSwitcher\_NewSave.bat** in the **MGSV_saves** folder (default location **C:\\MGSV\_saves**)
* To check currently used save, use .bat shortcut **MGSV\_SaveSwitcher\_MGSV\_SaveSwitcher\_CurrentSave.bat** in the **MGSV_saves** folder (default location **C:\\MGSV\_saves**)
* There is also a script for normal script use, **MGSV\_MGSV\_SaveSwitcher\_SaveSwitcher.bat** in the **MGSV_saves** folder (default location **C:\\MGSV\_saves**)
* These .bat shortcuts can be used from outside the save folder as well, if for example you want to have them in your Desktop or some other, easily accessible directory.