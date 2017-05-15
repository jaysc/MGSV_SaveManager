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
# User ID, put your own here (from Steam directory -> userdata)
USERID = "61333905"
#STEAM_FOLDER = "/mnt/d/Steam/userdata/" + USERID
STEAM_FOLDER = steam_finder.steam_find(LOCAL_FOLDER) + "/userdata/" + USERID + "/"

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
    with open(LOCAL_FOLDER.replace("\ ", " ") + "current_save.txt", "r") as f_in:
        CURRENT_SAVE = f_in.read()
    print("Creating a fresh save and backing up previous save.")
    new_save = input("Give a name for the new save: ") or "DEFAULT"
    new_save = new_save.upper()
    print(new_save)
    confirmation = input("Confirm [y/N]  ") or "N"
    confirmation = confirmation.upper()
    print(confirmation)
    if (confirmation == "Y" or confirmation == "YES"):
        os.system("cp -r " + STEAM_FOLDER + "/" + MGSV1 + " " + LOCAL_FOLDER + CURRENT_SAVE)
        os.system("cp -r " + STEAM_FOLDER + "/" + MGSV2 + " " + LOCAL_FOLDER + CURRENT_SAVE)
        os.system("mkdir " + LOCAL_FOLDER + new_save)
        os.system("rm -r " + STEAM_FOLDER + MGSV1)
        os.system("rm -r " + STEAM_FOLDER + MGSV2)
        with open(LOCAL_FOLDER.replace("\ ", " ") + "current_save.txt", "w") as f_out:
            f_out.write(new_save)
        input("Empty save created, press Enter to continue...")
    else:
        print("Invalid input, cancelling operation.")
        input("Press enter to continue...")
        sys.exit()

# Main save switching functionality
try:
    with open(LOCAL_FOLDER.replace("\ ", " ") + "current_save.txt", "r") as f_in:
        CURRENT_SAVE = f_in.read()
    print("Current save:", CURRENT_SAVE)
    i = 1
    for x in SAVES:
        print(str(i) + ": " + x)
        i += 1
    while(True):
        try:
            choice = int(input("Select save to use: ")) - 1
            break
        except Exception as e:
            print("Invalid selection, try again")
    os.system("cp -r " + STEAM_FOLDER + MGSV1 + " " + LOCAL_FOLDER + CURRENT_SAVE)
    os.system("cp -r " + STEAM_FOLDER + MGSV2 + " " + LOCAL_FOLDER + CURRENT_SAVE)
    os.system("cp -r " + LOCAL_FOLDER + SAVES[choice] + "/*" + " " + STEAM_FOLDER)
    try:
        with open(LOCAL_FOLDER.replace("\ ", " ") + "current_save.txt", "w") as f_out:
            f_out.write(SAVES[choice])
    except Exception as e:
        print("Error:", e)
# First time running, copy current save to local directory
except Exception as e:
    print("Error:", e)
    print("Probably first time running. Creating first save file.")
    os.system("mkdir " + LOCAL_FOLDER)
    SAVE_CURRENT = input("Save file name:  ").upper() or "DEFAULT"
    with open(LOCAL_FOLDER.replace("\ ", " ") + "current_save.txt", "w") as x:
        x.write(SAVE_CURRENT)
    os.system("mkdir " + LOCAL_FOLDER + "/" + SAVE_CURRENT)
    os.system("cp -r " + STEAM_FOLDER + MGSV1 + " " + LOCAL_FOLDER + SAVE_CURRENT)
    os.system("cp -r " + STEAM_FOLDER + MGSV2 + " " + LOCAL_FOLDER + SAVE_CURRENT)