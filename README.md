# MGSV_Save_Switcher
**The functionality with this version is still quite in the early stages. There is some more bugs to be fixed compared to the bash version, but this should still work if you don't try anything too fancy. Please backup your saves before using this.**  
A python 3 script to easily switch between multiple Metal Gear Solid V: The Phantom Pain save files.  
This version will allow running the script without the Linux subsystem, using only Python 3 and Windows command prompt.  
## Current functionality
* Switching between multiple save files.
* Create a new save file.
* Current save file used.
## How to use
* For time being, Python 3 is required to be installed. Install it from [Python website](https://www.python.org/)
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