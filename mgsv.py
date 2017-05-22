# Antti Alasalmi 2017
# MGSV Save Switcher command line branch
# Easily switch between MGSV: TPP save files
# Please disable both The Phantom Pain and Ground Zeroes (if installed) cloud sync before trying to create empty saves
# Otherwise Steam will download the save from cloud, thus preventing an empty save creation.

import sys, os
import steam_finder
import user_scanner

# Check if arguments have been used
def check_args():
    if (len(sys.argv) > 1):
        ARG = sys.argv[1]
    else:
        ARG = ""
    return ARG

# About page
def about():
    os.system("cls")
    print("\nMetal Gear Solid V: The Phantom Pain Save Switcher\nAntti Alasalmi 2017\nGithub page https://github.com/thatsafy/MGSV_Save_Switcher")
    input("\nPress enter to continue")

# Check arguments, if about page wanted, run it right away and exit.
ARGUMENT = check_args()
if (ARGUMENT.lower() == "a" or ARGUMENT.lower() == "about"):
    about()
    sys.exit()

# Script directory for creating the .bat shortcuts
SCRIPT_DIR = os.path.dirname(os.path.realpath("mgsv.py"))
# Local Save backup folders
DRIVE_LETTER = "C"
LOCAL_FOLDER = DRIVE_LETTER + ":\\" + "MGSV_saves\\"
try:
    os.system("mkdir " + LOCAL_FOLDER + " 2> nul")
except Exception as e:
    pass
# MGSV Folders, no need to change these
MGSV1 = "287700"
MGSV2 = "311340"
# Steam path
STEAM_PATH = steam_finder.steam_find(LOCAL_FOLDER)
if (STEAM_PATH == ""):
    print("Error, Steam path not set.\nTry running again, or make an issue in the GitHub page https://github.com/thatsafy/MGSV_Save_Switcher")
    input("Press enter to exit.")
    sys.exit()

# User scan
USER_NAMES = user_scanner.user_scan(STEAM_PATH)
USERNAME = user_scanner.user_name(USER_NAMES)
USERID = USER_NAMES[USERNAME]
os.system("cls")

# Make sure the userid is not empty
if (USERID == ""):
    print("UserID not set.\nTry running again, or make an issue in the GitHub page https://github.com/thatsafy/MGSV_Save_Switcher")
    input("Press enter to exit.")
    sys.exit()
STEAM_FOLDER = STEAM_PATH + "\\userdata\\" + USERID + "\\"
# MGSV: TPP Save files
SAVE_FILES = [MGSV2 + "\\remote\\PERSONAL_DATA", MGSV2 + "\\remote\\TPP_CONFIG_DATA", MGSV2 + "\\remote\\TPP_GAME_DATA", MGSV1 + "\\local\\*"]


# Scan for saves
def save_scan():
    os.chdir(LOCAL_FOLDER + USERNAME)
    SVS = []
    for x in next(os.walk("."))[1]:
        if (x != "OLD"):
            SVS.append(x)
    return SVS

# Delete save
def save_delete():
    SAVES = save_scan()
    saves_list(SAVES)
    print("0 : Cancel")
    while(True):
        try:
            save_del = int(input("Save to delete: "))
            if (save_del == 0):
                print("Cancelling. No changes made.")
                input("Press enter to exit")
                sys.exit()
            elif (save_del - 1 < 0 or save_del - 1 > len(SAVES)):
                print("Invalid selection, please try again.")
            else:
                while (True):
                    print("Selected save " + SAVES[save_del - 1])
                    sure = input("Are you sure?[y/N] : ").upper() or "N"
                    if (sure == ("Y" or "YES")):
                        os.system("cd " + LOCAL_FOLDER + USERNAME)
                        os.system("rmdir /S /Q " + SAVES[save_del - 1])
                        print("Save " + SAVES[save_del - 1] + " deleted.")
                        input("Press enter to exit")
                        sys.exit()
                    elif (sure == ("N" or "NO")):
                        print("Not deleting, cancelling operation without making changes.")
                        input("Press enter to exit")
                        sys.exit()
                    else:
                        print("Uh, didn't quite catch that, try again.")
                        SAVES = save_scan()
                        saves_list(SAVES)
                        print("0 : Cancel")
        except Exception as e:
            print(e)
            print("Invalid selection, please try again.")

# Main program
def main():
    try:
        # Check if arguments used
        ARG = ARGUMENT
        # If version
        if (ARG.lower() == "v" or ARG.lower() == "version"):
            print("Current save for user " + USERNAME + ": " + get_version())
        # If new save
        elif (ARG.lower() == "n" or ARG.lower() == "new"):
            new_save()
        elif (ARG.lower() == "d" or ARG.lower() == "delete"):
            save_delete()
        # If no valid arguments, go to main menu
        else:
            menu_items = ["Save Switch", "New Save", "Delete Save", "About", "Exit"]
            while (True):
                os.system("cls")
                print("Main Menu")
                print("Currently used save: " + get_version())
                i = 1
                for item in menu_items:
                    print(str(i) + ": " + item)
                    i += 1
                try:
                    selection = int(input("Selection: "))
                    if (selection == 1):
                        save_switch()
                    elif (selection == 2):
                        new_save()
                    elif (selection == 3):
                        save_delete()
                    elif (selection == 4):
                        about()
                    elif (selection == 5):
                        print("Exiting...")
                        sys.exit()
                except Exception as e:
                    print("Invalid selection, try again.")
    except Exception as e:
        first_run(e)
    
    input("Press enter to continue...")
    sys.exit()

# Check currently used save
def get_version():
    with open(LOCAL_FOLDER + USERNAME + "\\current_save.txt", "r") as f_in:
        #print("Currently used save:", f_in.read())
        CUR_SAVE = f_in.read()
    return CUR_SAVE


# Remove MGSV: TPP save files in the userdata
def remove_saves():
    # Remove save files for Phantom Pain
    for x in SAVE_FILES:
        os.system("del /Q " + STEAM_FOLDER + x + " 2> nul")


# Create new save
def new_save():
    # Check what was previously used save file
    CURRENT_SAVE = get_version()
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
        os.system("mkdir " + LOCAL_FOLDER + USERNAME + "\\" + new_save)
        os.system("mkdir " + LOCAL_FOLDER + USERNAME + "\\" + new_save + "\\" + MGSV1)
        os.system("mkdir " + LOCAL_FOLDER + USERNAME + "\\" + new_save + "\\" + MGSV2)
        steam_to_local(new_save)
        # Update current save used
        with open(LOCAL_FOLDER + USERNAME + "\\current_save.txt", "w") as f_out:
            f_out.write(new_save)
        print("Empty save created.")
    else:
        print("Invalid input, cancelling operation.")


# list available saves
def saves_list(s):
    i = 1
    print("Available saves for user " + USERNAME)
    for x in s:
        print(str(i) + ": " + x)
        i += 1


# Copy save from steam to local
def steam_to_local(CRNT):
    os.system("xcopy /E /Y " + STEAM_FOLDER + MGSV1 + " " + LOCAL_FOLDER + USERNAME + "\\" + CRNT + "\\" + MGSV1 + " 2> nul")
    os.system("xcopy /E /Y " + STEAM_FOLDER + MGSV2 + " " + LOCAL_FOLDER + USERNAME + "\\" + CRNT + "\\" + MGSV2 + " 2> nul")


# Copy save from local to steam
def local_to_steam(CRNT_SV):
    os.system("xcopy /E /Y " + LOCAL_FOLDER + USERNAME + "\\" + CRNT_SV + "\\" + MGSV1 + " " + STEAM_FOLDER + MGSV1 + " 2> nul")
    os.system("xcopy /E /Y " + LOCAL_FOLDER + USERNAME + "\\" + CRNT_SV + "\\" + MGSV2 + " " + STEAM_FOLDER + MGSV2 + " 2> nul")


# save switch function
def save_switch():
    os.system("cls")
    SAVES = []
    SAVES = save_scan()
    CURRENT_SAVE = get_version()
    # Check if a save file still exists, if not, use the first save in the save list
    # Also create directories for backup possible previous save files from Steam directories
    if (CURRENT_SAVE not in SAVES):
        print("error, current save no more. Switching to first save.")
        CURRENT_SAVE = SAVES[0]  
        os.system("mkdir " + LOCAL_FOLDER + USERNAME + "\\OLD" + " 2> nul")
        os.system("mkdir " + LOCAL_FOLDER + USERNAME + "\\OLD\\" + MGSV1 + " 2> nul")
        os.system("mkdir " + LOCAL_FOLDER + USERNAME + "\\OLD\\" + MGSV2 + " 2> nul")   
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
        with open(LOCAL_FOLDER + USERNAME + "\\current_save.txt", "w") as f_out:
            f_out.write(SAVES[choice])
    except Exception as e:
        print("Error:", e)

# First run, create a local save of your currently used MGSV: TPP save
def first_run(e):
    print("Probably first time running. Adding your current save file to saves list.")
    SAVE_CURRENT = input("Name your save file:  ").upper() or "DEFAULT"
    os.system("mkdir " + LOCAL_FOLDER + USERNAME + "\\" + SAVE_CURRENT)
    os.system("mkdir " + LOCAL_FOLDER + USERNAME + "\\" + SAVE_CURRENT + "\\" + MGSV1)
    os.system("mkdir " + LOCAL_FOLDER + USERNAME + "\\" + SAVE_CURRENT + "\\" + MGSV2)
    with open(LOCAL_FOLDER + USERNAME + "\\current_save.txt", "w") as x:
        x.write(SAVE_CURRENT)
    steam_to_local(SAVE_CURRENT)
    # Creating startup scripts.
    with open(LOCAL_FOLDER + "MGSV_SaveSwitcher.bat", "w") as f:
        f.write(SCRIPT_DIR[0:2] + " && cd " + SCRIPT_DIR + " && python mgsv.py")
    with open(LOCAL_FOLDER + "MGSV_SaveSwitcher_NewSave.bat", "w") as f:
        f.write(SCRIPT_DIR[0:2] + " && cd " + SCRIPT_DIR + " && python mgsv.py n")
    with open(LOCAL_FOLDER + "MGSV_SaveSwitcher_CurrentSave.bat", "w") as f:
        f.write(SCRIPT_DIR[0:2] + " && cd " + SCRIPT_DIR + " && python mgsv.py v")
    with open(LOCAL_FOLDER + "MGSV_SaveSwitcher_DeleteSave.bat", "w") as f:
        f.write(SCRIPT_DIR[0:2] + " && cd " + SCRIPT_DIR + " && python mgsv.py d")
    with open(LOCAL_FOLDER + "MGSV_SaveSwitcher_About.bat", "w") as f:
        f.write(SCRIPT_DIR[0:2] + " && cd " + SCRIPT_DIR + " && python mgsv.py a")
    print("Shortcut scripts created to " + LOCAL_FOLDER + ". These scripts require Python to be installed.")


# Start script
main()