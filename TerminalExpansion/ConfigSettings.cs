using BepInEx.Configuration;
using JetBrains.Annotations;

namespace TerminalStuff
{
    public static class ConfigSettings
    {
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
        
    }
}