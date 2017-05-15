# Antti Alasalmi 2017
# Locate Steam directory

import os

# Locate Steam directory
def steam_find():
    # Check if locations directory already found and saved
    try:
        with open("locations.txt", "r") as i:
            DIR_TEMP = i.readlines()
        return DIR_TEMP[0].rstrip()
    # Search for Steam directory
    except Exception as e:
        print("First time running, locating steam directory. This might take a few moments.")
        os.system("find /mnt -type d -name Steam 2> /dev/null > locations.txt")

        with open("locations.txt", "r") as x:
            LOCATIONS = x.readlines()

        for x in LOCATIONS:
            DIR = x.replace(" ", "\ ").replace("(", "\(").replace(")", "\)")
            SEARCH = "ls " + DIR
            
            os.system(SEARCH.rstrip() + " > test.txt")
            
            with open("test.txt", "r") as k:
                DATA  = k.readlines()
            for i in DATA:
                if(i.rstrip() == "userdata"):
                    print("Steam install directory:", DIR)
                    with open("locations.txt", "w") as w:
                        w.write(DIR)
                    os.system("rm test.txt")
                    return DIR


