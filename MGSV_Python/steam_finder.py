# Antti Alasalmi 2017
# Locate Steam directory

import os

# Locate Steam directory
def steam_find(LOCAL_DIR):
    # See if the path has been found already
    try:
        with open(LOCAL_DIR + "steam_path.txt", "r") as i:
            DIR_TEMP = i.read().replace("\\Steam.exe", "").rstrip()
        return DIR_TEMP
    # Path missing, scan for Steam path
    except Exception as e:
        # List all usable drives, remove first line containing header
        os.system("wmic LOGICALDISK LIST BRIEF > " + LOCAL_DIR + "drvs.txt")
        with open(LOCAL_DIR + "drvs.txt", "r") as reader:
            DRIVES = reader.readlines()
        DRIVES.pop(0)
        # Create file where to save drive letters
        with open(LOCAL_DIR + "drives.txt", "w") as writer:
            writer.write("")

        for line in DRIVES:
            with open(LOCAL_DIR + "drives.txt", "a") as writer:
                writer.write(line.strip().replace(" ", "").replace("\x00", "")[0:2])
        # Remove : and last item which is empty.   
        with open(LOCAL_DIR + "drives.txt", "r") as reader:
            d = reader.read().rsplit(":")
        d.pop()
        STEAM_PATH = ""
        # Start searching the Steam directory. Iterate through the drives searching for Steam.exe
        for i in d:
            os.system(i + ": && dir /s /b Steam.exe > " + LOCAL_DIR + "steam_path.txt")
            with open(LOCAL_DIR + "steam_path.txt", "r") as r:
                k = r.read()
                if ("Steam.exe" in k):
                    STEAM_PATH = k.replace("\\Steam.exe", "").rstrip()
                    print("Steam found in " + STEAM_PATH)
                    break
        # Remove files used in the previous steps since no longer needed
        os.system("del /Q " + LOCAL_DIR + "drvs.txt 2> nul")
        os.system("del /Q " + LOCAL_DIR + "drives.txt 2> nul")
        return STEAM_PATH