using BepInEx.Configuration;
using JetBrains.Annotations;

namespace TerminalStuff
{
    public static class ConfigSettings
    {
        //establish commands that can be turned on or off here
        public static ConfigEntry<bool> ModNetworking;
        public static ConfigEntry<bool> terminalLobby; //lobby name command
        public static ConfigEntry<bool> terminalCams; //cams command
        public static ConfigEntry<bool> terminalQuit; //quit command
        public static ConfigEntry<bool> terminalClear; //clear command
        public static ConfigEntry<bool> terminalLoot; //loot command
        public static ConfigEntry<bool> terminalLol; //lol command
        public static ConfigEntry<bool> terminalHeal; //heal command
        public static ConfigEntry<bool> terminalFov; //Fov command
        public static ConfigEntry<bool> terminalGamble; //Gamble command
        public static ConfigEntry<bool> terminalLever; //Lever command
        public static ConfigEntry<bool> terminalDanger; //Danger command
        public static ConfigEntry<bool> terminalVitals; //Vitals command
        public static ConfigEntry<bool> terminalBioScan; //BioScan command
        public static ConfigEntry<bool> terminalVitalsUpgrade; //Vitals Upgrade command
        public static ConfigEntry<bool> terminalTP; //Teleporter command
        public static ConfigEntry<bool> terminalITP; //Inverse Teleporter command
        public static ConfigEntry<bool> terminalMods; //Modlist command
        public static ConfigEntry<bool> terminalKick; //Kick command (host only)
        public static ConfigEntry<bool> terminalFcolor; //Flashlight color command
        public static ConfigEntry<bool> terminalMap; //Map shortcut
        public static ConfigEntry<bool> terminalMinimap; //Minimap command
        public static ConfigEntry<bool> terminalMinicams; //Minicams command
        public static ConfigEntry<bool> terminalOverlay; //Overlay cams command
        public static ConfigEntry<bool> terminalDoor; //Door Toggle command
        public static ConfigEntry<bool> terminalLights; //Light Toggle command
        public static ConfigEntry<bool> terminalScolor; //Light colors command
        public static ConfigEntry<bool> terminalAlwaysOn; //AlwaysOn command
        public static ConfigEntry<bool> terminalLink; //Link command
        public static ConfigEntry<bool> terminalLink2; //Link2 command
        public static ConfigEntry<bool> terminalRandomSuit; //RandomSuit command


        //Strings for display messages
        public static ConfigEntry<string> doorOpenString; //Door String
        public static ConfigEntry<string> doorCloseString; //Door String
        public static ConfigEntry<string> doorSpaceString; //Door String
        public static ConfigEntry<string> quitString; //Quit String
        public static ConfigEntry<string> leverString; //Lever String
        public static ConfigEntry<string> kickString; //Kick String
        public static ConfigEntry<string> kickNoString; //Kick NO string
        public static ConfigEntry<string> kickNotHostString; //Kick not host string
        public static ConfigEntry<string> lolStartString; //lol, start video string
        public static ConfigEntry<string> lolStopString; //lol, stop video string
        public static ConfigEntry<string> tpMessageString; //TP Message String
        public static ConfigEntry<string> itpMessageString; //TP Message String
        public static ConfigEntry<string> vitalsPoorString; //Vitals can't afford string
        public static ConfigEntry<string> vitalsUpgradePoor; //Vitals Upgrade can't afford string
        public static ConfigEntry<string> healIsFullString; //full health string
        public static ConfigEntry<string> healString; //healing player string
        public static ConfigEntry<string> camString; //Cameras on string
        public static ConfigEntry<string> camString2; //Cameras off string
        public static ConfigEntry<string> mapString; //map on string
        public static ConfigEntry<string> mapString2; //map off string
        public static ConfigEntry<string> ovString; //overlay on string
        public static ConfigEntry<string> ovString2; //overlay off string
        public static ConfigEntry<string> mmString; //minimap on string
        public static ConfigEntry<string> mmString2; //minimap off string
        public static ConfigEntry<string> mcString; //minicam on string
        public static ConfigEntry<string> mcString2; //minicam off string


        //Cost configs
        public static ConfigEntry<int> vitalsCost; //Cost of Vitals Command
        public static ConfigEntry<int> vitalsUpgradeCost; //Cost of Vitals Upgrade Command
        public static ConfigEntry<int> bioScanUpgradeCost; //Cost of Enemy Scan Upgrade Command
        public static ConfigEntry<int> enemyScanCost; //Cost of Enemy Scan Command

        //Other config items
        public static ConfigEntry<int> gambleMinimum; //Minimum amount of credits needed to gamble
        public static ConfigEntry<bool> gamblePityMode; //enable or disable pity for gamblers
        public static ConfigEntry<int> gamblePityCredits; //Pity Credits for losers
        public static ConfigEntry<string> gamblePoorString; //gamble credits too low string
        public static ConfigEntry<string> videoFolderPath; //Specify a different folder with videos
        public static ConfigEntry<bool> leverConfirmOverride; //disable confirmation check for lever
        public static ConfigEntry<bool> camsNeverHide;
        public static ConfigEntry<bool> networkedNodes; //enable or disable networked terminal nodes (beta)
        public static ConfigEntry<string> defaultCamsView;
        public static ConfigEntry<int> ovOpacity; //Opacity Percentage for Overlay Cams View
        public static ConfigEntry<string> customLink;
        public static ConfigEntry<string> customLink2;
        public static ConfigEntry<string> customLinkHint;
        public static ConfigEntry<string> customLink2Hint;
        public static ConfigEntry<string> homeLine1;
        public static ConfigEntry<string> homeLine2;
        public static ConfigEntry<string> homeLine3;

        //keyword strings (terminalapi)
        public static ConfigEntry<string> alwaysOnKeyword; //string to match keyword
        public static ConfigEntry<string> minimapKeyword;
        public static ConfigEntry<string> minicamsKeyword;
        public static ConfigEntry<string> overlayKeyword;
        public static ConfigEntry<string> doorKeyword;
        public static ConfigEntry<string> lightsKeyword;
        public static ConfigEntry<string> modsKeyword2;
        public static ConfigEntry<string> tpKeyword2;
        public static ConfigEntry<string> itpKeyword2;
        public static ConfigEntry<string> quitKeyword2;
        public static ConfigEntry<string> lolKeyword;
        public static ConfigEntry<string> clearKeyword2;
        public static ConfigEntry<string> dangerKeyword;
        public static ConfigEntry<string> healKeyword2;
        public static ConfigEntry<string> lootKeyword2;
        public static ConfigEntry<string> camsKeyword2;
        public static ConfigEntry<string> mapKeyword2;
        public static ConfigEntry<string> randomSuitKeyword;

        //terminal patcher keywords
        public static ConfigEntry<string> fcolorKeyword;
        public static ConfigEntry<string> gambleKeyword;
        public static ConfigEntry<string> leverKeyword;
        public static ConfigEntry<string> scolorKeyword;
        public static ConfigEntry<string> linkKeyword;
        public static ConfigEntry<string> link2Keyword;
        
    }
}