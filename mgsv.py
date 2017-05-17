# Antti Alasalmi 2017
# Alpha 0.2
# MGSV Save Switcher command line branch
# Run script without the linux subsystem, using Windows command line tools.
# Easily switch between MGSV: TPP save files
# Please disable both The Phantom Pain and Ground Zeroes (if installed) cloud sync before trying to create empty saves
# Otherwise Steam will download the save from cloud, thus preventing an empty save creation.
# Also remember to add your own USERID and steam path to STEAM_PATH variable, check instructions above variable for formatting.

import sys, os
import steam_finder

# Local Save backup folders
DRIVE_LETTER = "C"
LOCAL_FOLDER = DRIVE_LETTER + ":\\" + "MGSV_saves\\"
# MGSV Folders, no need to change these
MGSV1 = "287700"
MGSV2 = "311340"

#STEAM_FOLDER = "/mnt/d/Steam/userdata/" + USERID
#STEAM_FOLDER = steam_finder.steam_find(LOCAL_FOLDER) + "\\userdata\\" + USERID + "\\"
# For time being, user needs to define manually the Steam folder here, format DRIVE_LETTER:\\Path\\To\\Steam e.g. D:\\Steam or C:\\Program Files\Steam
# with open(sys.path[0] + "\steam_path.txt", "r") as s:
#     STEAM_PATH = s.readlines()[0].rsplit()[0]
STEAM_PATH = steam_finder.steam_find(LOCAL_FOLDER)
if (STEAM_PATH == ""):
    print("Steam path not set, please set Steam path to steam_path.txt. Format example 'D:\\Steam'")
    input("Press enter to exit.")
    sys.exit()


# User scan
USERIDS = []
USERNAMES = {}
USERID = ""
USERNAME = ""
if (USERID == ""):
    os.chdir(STEAM_PATH + "\\userdata")
    for x in next(os.walk("."))[1]:
        USERIDS.append(x)
    for user in USERIDS:
        if (USERID != ""):
            break
        with open(STEAM_PATH + "\\userdata\\" + user + "\\config\\localconfig.vdf", "r") as reader:
            lines = reader.readlines()
        for line in lines:
            if ("PersonaName" in line):
                UNAME = line.strip().rsplit("		")[1].rsplit("\"")[1]
                USERNAMES[UNAME] = user
                break

os.system("cls")
# User selection
u  = 1
user_temp_list = []
for USER in USERNAMES:
    print(str(u) + ": Username: " + USER + ", UserID: " + USERNAMES[USER])
    u += 1
    user_temp_list.append(USERNAMES[USER])
while(True):
    try:
        selection = int(input("Select your username (number on the list): "))
    except Exception as e:
        print("Invalid selection, try again.")
        continue
    if (selection <= 0 or selection > len(USERNAMES)):
        print("Invalid selection, try again.")
    else:
        selection -= 1
        USERID = user_temp_list[selection]
        break
os.system("cls")

# Make sure the userid is not empty
if (USERID == ""):
    print("Steam userid not set. Please write the userid inside the 'userid.txt' file.")
    input("Press enter to exit.")
    sys.exit()
STEAM_FOLDER = STEAM_PATH + "\\userdata\\" + USERID + "\\"
# MGSV: TPP Save files
SAVE_FILES = [MGSV2 + "\\remote\\PERSONAL_DATA", MGSV2 + "\\remote\\TPP_CONFIG_DATA", MGSV2 + "\\remote\\TPP_GAME_DATA", MGSV1 + "\\local\\*"]


# Scan for saves
def save_scan():
    os.chdir(LOCAL_FOLDER)
    SVS = []
    for x in next(os.walk("."))[1]:
        SVS.append(x)
    return SVS


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
    with open(LOCAL_FOLDER + "current_save.txt", "r") as f_in:
        print("Currently used save:", f_in.read())
    input("Press enter to continue...")
    sys.exit()


# Remove MGSV: TPP save files in the userdata
def remove_saves():
    # Remove save files for Phantom Pain
    for x in SAVE_FILES:
        os.system("del /Q " + STEAM_FOLDER + x + " 2> nul")


# Create new save
def new_save():
    # Check what was previously used save file
    with open(LOCAL_FOLDER + "current_save.txt", "r") as f_in:
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
        os.system("mkdir " + LOCAL_FOLDER + new_save + "\\" + MGSV1)
        os.system("mkdir " + LOCAL_FOLDER + new_save + "\\" + MGSV2)
        steam_to_local(new_save)
        # Update current save used
        with open(LOCAL_FOLDER + "current_save.txt", "w") as f_out:
            f_out.write(new_save)
        print("Empty save created.")
        input("press Enter to continue...")
        sys.exit()
    else:
        print("Invalid input, cancelling operation.")
        input("Press enter to continue...")
        sys.exit()


# list available saves
def saves_list(s):
    i = 1
    for x in s:
        print(str(i) + ": " + x)
        i += 1


# Copy save from steam to local
def steam_to_local(CRNT):
    os.system("xcopy /E /Y " + STEAM_FOLDER + MGSV1 + " " + LOCAL_FOLDER + CRNT + "\\" + MGSV1 + " 2> nul")
    os.system("xcopy /E /Y " + STEAM_FOLDER + MGSV2 + " " + LOCAL_FOLDER + CRNT + "\\" + MGSV2 + " 2> nul")


# Copy save from local to steam
def local_to_steam(CRNT_SV):
    os.system("xcopy /E /Y " + LOCAL_FOLDER + CRNT_SV + "\\" + MGSV1 + " " + STEAM_FOLDER + MGSV1 + " 2> nul")
    os.system("xcopy /E /Y " + LOCAL_FOLDER + CRNT_SV + "\\" + MGSV2 + " " + STEAM_FOLDER + MGSV2 + " 2> nul")


# save switch function
def save_switch():
    SAVES = []
    SAVES = save_scan()
    with open(LOCAL_FOLDER + "current_save.txt", "r") as f_in:
        CURRENT_SAVE = f_in.read()
    # Check if a save file still exists, if not, use the first save in the save list
    # Also create directories for backup possible previous save files from Steam directories
    if (CURRENT_SAVE not in SAVES):
        print("error, current save no more. Switching to first save.")
        CURRENT_SAVE = SAVES[0]  
        os.system("mkdir " + LOCAL_FOLDER + "OLD" + " 2> nul")
        os.system("mkdir " + LOCAL_FOLDER + "OLD\\" + MGSV1 + " 2> nul")
        os.system("mkdir " + LOCAL_FOLDER + "OLD\\" + MGSV2 + " 2> nul")   
        print("Backing up previous save to 'OLD' folder.")
        steam_to_local("old")
        remove_saves()
        local_to_steam(CURRENT_SAVE)
    # List save files available
    saves_list(SAVES)
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
        with open(LOCAL_FOLDER + "current_save.txt", "w") as f_out:
            f_out.write(SAVES[choice])
        input("press enter to continue")
    except Exception as e:
        print("Error:", e)
        input("Press enter to continue")

# First run, create a local save of your currently used MGSV: TPP save
def first_run(e):
    print("Error:", e)
    print("Probably first time running. Adding your current save file to saves list.")
    os.system("mkdir " + LOCAL_FOLDER + " 2> nul")
    SAVE_CURRENT = input("Name your save file:  ").upper() or "DEFAULT"
    with open(LOCAL_FOLDER + "current_save.txt", "w") as x:
        x.write(SAVE_CURRENT)
    os.system("mkdir " + LOCAL_FOLDER + SAVE_CURRENT)
    os.system("mkdir " + LOCAL_FOLDER + SAVE_CURRENT + "\\" + MGSV1)
    os.system("mkdir " + LOCAL_FOLDER + SAVE_CURRENT + "\\" + MGSV2)
    steam_to_local(SAVE_CURRENT)
    os.system("cd > " + LOCAL_FOLDER + "script_path.txt")
    with open(LOCAL_FOLDER + "script_path.txt", "r") as r:
        SCRIPT_DIR = r.readlines()[0].rstrip()
    # Creating startup scripts.
    with open(LOCAL_FOLDER + "MGSV_SaveSwitcher.bat", "w") as f:
        f.write(SCRIPT_DIR[0:2] + " && cd " + SCRIPT_DIR + " && python mgsv.py")
    with open(LOCAL_FOLDER + "MGSV_SaveSwitcher_NewSave.bat", "w") as f:
        f.write(SCRIPT_DIR[0:2] + " && cd " + SCRIPT_DIR + " && python mgsv.py n")
    with open(LOCAL_FOLDER + "MGSV_SaveSwitcher_CurrentSave.bat", "w") as f:
        f.write(SCRIPT_DIR[0:2] + " && cd " + SCRIPT_DIR + " && python mgsv.py v")
    print("Shortcut scripts created to " + LOCAL_FOLDER + ". These scripts require Python to be installed.")
    sys.exit()


# Start script
main()