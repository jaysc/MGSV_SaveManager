# Antti Alasalmi 2017
# Easily switch between MGSV: TPP save files
# Note: This script is to be used inside Bash under Windows 10

import sys, os
import steam_finder

# Local Save backup folders
PRE_LOCAL = "/mnt/"
DRIVE_LETTER = "c"
LOCAL_FOLDER = PRE_LOCAL + DRIVE_LETTER  + "/MGSV\ saves/"
# MGSV Folders, no need to change these
MGSV1 = "287700"
MGSV2 = "311340"
# User ID, put your own here (from Steam directory -> userdata) inside the quotation marks before running the script.
USERID = ""
#STEAM_FOLDER = "/mnt/d/Steam/userdata/" + USERID
STEAM_FOLDER = steam_finder.steam_find(LOCAL_FOLDER) + "/userdata/" + USERID + "/"

SAVE_FILES = [MGSV2 + "/remote/PERSONAL_DATA", MGSV2 + "/remote/TPP_CONFIG_DATA", MGSV2 + "/remote/TPP_GAME_DATA", MGSV1 + "/local/*"]

# Scan for save files
os.system("cd " + LOCAL_FOLDER + " && find -maxdepth 1 -type d > saves.txt")
with open(LOCAL_FOLDER.replace("\ ", " ") + "saves.txt", "r") as s:
    saves = s.readlines()
os.system("rm " + LOCAL_FOLDER + "saves.txt")
SAVES = []
for x in saves:
    s = x.replace("\n", "").replace(".", "").replace("/", "")
    SAVES.append(s)
SAVES.pop(0)

# Use arguments for additional funtions
if (len(sys.argv) > 1):
    ARG = sys.argv[1]
else:
    ARG = ""

# Check current save version
if (ARG.lower() == "v" or ARG.lower() == "version"):
    with open(LOCAL_FOLDER.replace("\ ", " ") + "current_save.txt", "r") as f_in:
        print("Currently used save:", f_in.read())
    input("Press enter to continue...")
    sys.exit()

# Create a new save, backup previous save
elif (ARG.lower() == "n" or ARG.lower() == "new"):
    # Check what was previously used save file
    with open(LOCAL_FOLDER.replace("\ ", " ") + "current_save.txt", "r") as f_in:
        CURRENT_SAVE = f_in.read()
    print("Creating a fresh save and backing up previous save.")
    new_save = input("Give a name for the new save: ") or "DEFAULT"
    new_save = new_save.upper()
    print(new_save)
    confirmation = input("Confirm [y/N]  ") or "N"
    confirmation = confirmation.upper()
    print(confirmation)
    # Confirm creation of a new save file, only happens if conditions met, otherwise nothing is done
    # Backup previously used save file to corresponding save directory, do cleanup
    if (confirmation == "Y" or confirmation == "YES"):
        print("Backing up previously used save...")
        os.system("cp -r " + STEAM_FOLDER + MGSV1 + "/* " + LOCAL_FOLDER + CURRENT_SAVE + "/" + MGSV1 + " 2> /dev/null")
        os.system("cp -r " + STEAM_FOLDER + MGSV2 + "/* " + LOCAL_FOLDER + CURRENT_SAVE + "/" + MGSV2 + " 2> /dev/null")
        print("Backup complete.")
        # Remove save files for Phantom Pain
        for x in SAVE_FILES:
            os.system("rm " + STEAM_FOLDER + x)
        os.system("mkdir " + LOCAL_FOLDER + new_save)
        os.system("mkdir " + LOCAL_FOLDER + new_save + "/" + MGSV1)
        os.system("mkdir " + LOCAL_FOLDER + new_save + "/" + MGSV2)
        os.system("cp -r " + STEAM_FOLDER + MGSV1 + "/* " + LOCAL_FOLDER + new_save + "/" + MGSV1 + " 2> /dev/null")
        os.system("cp -r " + STEAM_FOLDER + MGSV2 + "/* " + LOCAL_FOLDER + new_save + "/" + MGSV2 + " 2> /dev/null")
        with open(LOCAL_FOLDER.replace("\ ", " ") + "current_save.txt", "w") as f_out:
            f_out.write(new_save)
        print("Empty save created.")
        input("press Enter to continue...")
        sys.exit()
    else:
        print("Invalid input, cancelling operation.")
        input("Press enter to continue...")
        sys.exit()

# Main save switching functionality
try:
    with open(LOCAL_FOLDER.replace("\ ", " ") + "current_save.txt", "r") as f_in:
        CURRENT_SAVE = f_in.read()
    # Check if a save file still exists, if not, use the first save in the save list
    # Also create directories for backup possible previous save files from Steam directories
    if (CURRENT_SAVE not in SAVES):
        print("error, current save no more. Switching to first save.")
        CURRENT_SAVE = SAVES[0]
        
        os.system("mkdir " + LOCAL_FOLDER + "OLD" + " 2> /dev/null")
        os.system("mkdir " + LOCAL_FOLDER + "OLD/" + MGSV1 + " 2> /dev/null")
        os.system("mkdir " + LOCAL_FOLDER + "OLD/" + MGSV2 + " 2> /dev/null")   
        
        print("Backing up previous save to 'OLD' folder.")
        os.system("cp -r " + STEAM_FOLDER + MGSV1 + "/*" + LOCAL_FOLDER + "OLD/" + MGSV1 + " 2> /dev/null")
        os.system("cp -r " + STEAM_FOLDER + MGSV2 + "/*" + LOCAL_FOLDER + "OLD/" + MGSV2 + " 2> /dev/null")
        os.system("cp -r " + LOCAL_FOLDER + CURRENT_SAVE + "/" + MGSV1 + "/* " + STEAM_FOLDER + MGSV1 + " 2> /dev/null")
        os.system("cp -r " + LOCAL_FOLDER + CURRENT_SAVE + "/" + MGSV2 + "/* " + STEAM_FOLDER + MGSV2 + " 2> /dev/null")
    
    # List available saves
    i = 1
    for x in SAVES:
        print(str(i) + ": " + x)
        i += 1
    
    # Save selection
    print("Current save:", CURRENT_SAVE)
    while(True):
        try:
            choice = int(input("Select save to use: ")) - 1
            break
        except Exception as e:
            print("Invalid selection, try again")
    # Clean up previously used save, switch to the selected save file
    os.system("cp -r " + STEAM_FOLDER + MGSV1 + "/* " + LOCAL_FOLDER + CURRENT_SAVE + "/" + MGSV1 + " 2> /dev/null")
    os.system("cp -r " + STEAM_FOLDER + MGSV2 + "/* " + LOCAL_FOLDER + CURRENT_SAVE + "/" + MGSV2 + " 2> /dev/null")
    # Remove  old save files for Phantom Pain
    for x in SAVE_FILES:
        os.system("rm " + STEAM_FOLDER + x)
    # Load save files
    os.system("cp -r " + LOCAL_FOLDER + SAVES[choice] + "/" + MGSV1 + "/* " + STEAM_FOLDER + MGSV1 + " 2> /dev/null")
    os.system("cp -r " + LOCAL_FOLDER + SAVES[choice] + "/" + MGSV2 + "/* " + STEAM_FOLDER + MGSV2 + " 2> /dev/null")
    # Save currently used save file into a file
    try:
        with open(LOCAL_FOLDER.replace("\ ", " ") + "current_save.txt", "w") as f_out:
            f_out.write(SAVES[choice])
    except Exception as e:
        print("Error:", e)

# First time running, copy current save to local directory
except Exception as e:
    print("Error:", e)
    print("Probably first time running. Adding your current save file to saves list.")
    os.system("mkdir " + LOCAL_FOLDER)
    SAVE_CURRENT = input("Name your save file:  ").upper() or "DEFAULT"
    with open(LOCAL_FOLDER.replace("\ ", " ") + "current_save.txt", "w") as x:
        x.write(SAVE_CURRENT)
    os.system("mkdir " + LOCAL_FOLDER + SAVE_CURRENT)
    os.system("mkdir " + LOCAL_FOLDER + SAVE_CURRENT + "/" + MGSV1)
    os.system("mkdir " + LOCAL_FOLDER + SAVE_CURRENT + "/" + MGSV2)
    os.system("cp -r " + STEAM_FOLDER + MGSV1 + "/* " + LOCAL_FOLDER + SAVE_CURRENT + "/" + MGSV1 + " 2> /dev/null")
    os.system("cp -r " + STEAM_FOLDER + MGSV2 + "/* " + LOCAL_FOLDER + SAVE_CURRENT + "/" + MGSV2 + " 2> /dev/null")