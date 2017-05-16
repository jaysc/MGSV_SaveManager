# Antti Alasalmi 2017
# Easily switch between MGSV: TPP save files
# Note: This script is to be used inside Bash under Windows 10
# Please disable both The Phantom Pain and Ground Zeroes (if installed) cloud sync before trying to create empty saves
# Otherwise Steam will download the save from cloud, thus preventing an empty save creation.
# Also remember to add your own USERID

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
if (USERID == ""):
    print("Steam userid not set, please put your own userid to the USERID varbiable inside the quotation marks.")
    sys.exit()
#STEAM_FOLDER = "/mnt/d/Steam/userdata/" + USERID
STEAM_FOLDER = steam_finder.steam_find(LOCAL_FOLDER) + "/userdata/" + USERID + "/"
# MGSV: TPP Save files
SAVE_FILES = [MGSV2 + "/remote/PERSONAL_DATA", MGSV2 + "/remote/TPP_CONFIG_DATA", MGSV2 + "/remote/TPP_GAME_DATA", MGSV1 + "/local/*"]
SAVES = []


# Scan for saves
def save_scan():
    os.system("cd " + LOCAL_FOLDER + " && find -maxdepth 1 -type d > saves.txt")
    with open(LOCAL_FOLDER.replace("\ ", " ") + "saves.txt", "r") as s:
        saves = s.readlines()
    os.system("rm " + LOCAL_FOLDER + "saves.txt")
    SVS = []
    for x in saves:
        s = x.replace("\n", "").replace(".", "").replace("/", "")
        SVS.append(s)
    SVS.pop(0)
    return SVS


# Scan for save files
SAVES = save_scan()


# Main program
def main():
    # Check if arguments used
    ARG = check_args()
    # If version
    if (ARG.lower() == "v" or ARG.lower() == "version"):
        check_version()
    # If new save
    elif (ARG.lower() == "n" or ARG.lower() == "new"):
        new_save()
    
    try:
        save_switch()
    except Exception as e:
        first_run(e)


# Check if arguments have been used
def check_args():
    if (len(sys.argv) > 1):
        ARG = sys.argv[1]
    else:
        ARG = ""
    return ARG


# Check currently used save
def check_version():
    with open(LOCAL_FOLDER.replace("\ ", " ") + "current_save.txt", "r") as f_in:
        print("Currently used save:", f_in.read())
    input("Press enter to continue...")
    sys.exit()


# Remove MGSV: TPP save files in the userdata
def remove_saves():
    # Remove save files for Phantom Pain
    for x in SAVE_FILES:
        os.system("rm " + STEAM_FOLDER + x + " 2> /dev/null")


# Create new save
def new_save():
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
        steam_to_local(CURRENT_SAVE)
        print("Backup complete.")
        # Remove save files for Phantom Pain
        remove_saves()
        # Create new save directory, copy empty MGSV save there
        os.system("mkdir " + LOCAL_FOLDER + new_save)
        os.system("mkdir " + LOCAL_FOLDER + new_save + "/" + MGSV1)
        os.system("mkdir " + LOCAL_FOLDER + new_save + "/" + MGSV2)
        steam_to_local(new_save)
        # Update current save used
        with open(LOCAL_FOLDER.replace("\ ", " ") + "current_save.txt", "w") as f_out:
            f_out.write(new_save)
        print("Empty save created.")
        input("press Enter to continue...")
        sys.exit()
    else:
        print("Invalid input, cancelling operation.")
        input("Press enter to continue...")
        sys.exit()


# list available saves
def saves_list():
    i = 1
    for x in SAVES:
        print(str(i) + ": " + x)
        i += 1


# Copy save from steam to local
def steam_to_local(CRNT):
    os.system("cp -r " + STEAM_FOLDER + MGSV1 + "/* " + LOCAL_FOLDER + CRNT + "/" + MGSV1 + " 2> /dev/null")
    os.system("cp -r " + STEAM_FOLDER + MGSV2 + "/* " + LOCAL_FOLDER + CRNT + "/" + MGSV2 + " 2> /dev/null")


# Copy save from local to steam
def local_to_steam(CRNT_SV):
    os.system("cp -r " + LOCAL_FOLDER + CRNT_SV + "/" + MGSV1 + "/* " + STEAM_FOLDER + MGSV1 + " 2> /dev/null")
    os.system("cp -r " + LOCAL_FOLDER + CRNT_SV + "/" + MGSV2 + "/* " + STEAM_FOLDER + MGSV2 + " 2> /dev/null")


# save switch function
def save_switch():
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
        steam_to_local("old")
        remove_saves()
        local_to_steam(CURRENT_SAVE)
    
    saves_list()
  
    print("Current save:", CURRENT_SAVE)
    while(True):
        try:
            choice = int(input("Select save to use: ")) - 1
            break
        except Exception as e:
            print("Invalid selection, try again")
    # Clean up previously used save, switch to the selected save file
    steam_to_local(CURRENT_SAVE)
    # Remove  old save files for Phantom Pain
    remove_saves()
    # Load save files
    local_to_steam(SAVES[choice])
    # Save currently used save file into a file
    try:
        with open(LOCAL_FOLDER.replace("\ ", " ") + "current_save.txt", "w") as f_out:
            f_out.write(SAVES[choice])
        input("press enter to continue")
    except Exception as e:
        print("Error:", e)
        input("Press enter to continue")


# First run, create a local save of your currently used MGSV: TPP save
def first_run(e):
    print("Error:", e)
    print("Probably first time running. Adding your current save file to saves list.")
    os.system("mkdir " + LOCAL_FOLDER)
    SAVE_CURRENT = input("Name your save file:  ").upper() or "DEFAULT"
    with open(LOCAL_FOLDER.replace("\ ", " ") + "current_save.txt", "w") as x:
        x.write(SAVE_CURRENT)
    os.system("mkdir " + LOCAL_FOLDER + SAVE_CURRENT)
    os.system("mkdir " + LOCAL_FOLDER + SAVE_CURRENT + "/" + MGSV1)
    os.system("mkdir " + LOCAL_FOLDER + SAVE_CURRENT + "/" + MGSV2)
    steam_to_local(SAVE_CURRENT)


# Start script
main()