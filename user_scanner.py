# Antti Alasalmi 2017
# Save Scanner

import os, sys

# Scan for users
def user_scan(STEAM_PATH):
    # Variables to be used
    USERIDS = []
    USERNAMES = {}
 
    os.chdir(STEAM_PATH + "\\userdata")
    for x in next(os.walk("."))[1]:
        USERIDS.append(x)
    for user in USERIDS:
        with open(STEAM_PATH + "\\userdata\\" + user + "\\config\\localconfig.vdf", "r") as reader:
            lines = reader.readlines()
        for line in lines:
            if ("PersonaName" in line):
                UNAME = line.strip().rsplit("		")[1].rsplit("\"")[1]
                USERNAMES[UNAME] = user
                break
    return USERNAMES

# Username selection
def user_name(USERNAMES):
    os.system("cls")
    u  = 1
    user_temp_list = []
    for USER in USERNAMES:
        print(str(u) + ": Username: " + USER + ", UserID: " + USERNAMES[USER])
        u += 1
        user_temp_list.append(USER)
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
            USERNAME = user_temp_list[selection]
            break
    return USERNAME
