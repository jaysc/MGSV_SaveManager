# Antti Alasalmi 2017
# Locate Steam directory

import os

# Locate Steam directory
def steam_find(LOCAL_DIR):
    # Check if locations directory already found and saved
    try:
        with open(LOCAL_DIR + "locations.txt", "r") as i:
            DIR_TEMP = i.readlines()
        return DIR_TEMP[0].rstrip()
    # Search for Steam directory
    except Exception as e:
        print("First time running, locating steam directory. This might take a few moments.")
        os.system("mkdir " + LOCAL_DIR)
        os.system("find /mnt -type d -name Steam 2> /dev/null > " + LOCAL_DIR + "locations.txt")

        with open(LOCAL_DIR + "locations.txt", "r") as x:
            LOCATIONS = x.readlines()
        # Search for right Steam directory, which contains the userdata folder
        for x in LOCATIONS:
            DIR = x.replace(" ", "\ ").replace("(", "\(").replace(")", "\)")
            SEARCH = "ls " + DIR
            os.system(SEARCH.rstrip() + " > " + LOCAL_DIR + "dir_temp.txt")
            with open(LOCAL_DIR + "dir_temp.txt", "r") as k:
                DATA  = k.readlines()
            for i in DATA:
                if(i.rstrip() == "userdata"):
                    print("Steam install directory:", DIR)
                    with open(LOCAL_DIR + "locations.txt", "w") as w:
                        w.write(DIR)
                    print("Creating .bat shortcut file...")
                    os.system("pwd > " + LOCAL_DIR + "pwd_tmp.txt")
                    with open(LOCAL_DIR + "pwd_tmp.txt", "r") as pwd:
                        SCRIPT_DIR = pwd.readlines()[0].rstrip()
                    print(SCRIPT_DIR)
                    # Creating startup scripts.
                    with open(LOCAL_DIR + "MGSV_SaveSwitcher.bat", "w") as f:
                        f.write("bash -c \"cd " + SCRIPT_DIR + " && python3 mgsv.py\"")
                    with open(LOCAL_DIR + "MGSV_SaveSwitcher_NewSave.bat", "w") as f:
                        f.write("bash -c \"cd " + SCRIPT_DIR + " && python3 mgsv.py n\"")
                    with open(LOCAL_DIR + "MGSV_SaveSwitcher_CurrentSave.bat", "w") as f:
                        f.write("bash -c \"cd " + SCRIPT_DIR + " && python3 mgsv.py v\"")
                    os.system("rm " + LOCAL_DIR + "pwd_tmp.txt")
                    os.system("rm " + LOCAL_DIR + "dir_temp.txt")
                    return DIR.rsplit("\n")[0]