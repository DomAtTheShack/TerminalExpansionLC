using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using BepInEx;
using GameNetcodeStuff;
using TerminalApi.Classes;
using TerminalApi.Events;
using TimeTerminalCommand;
using UnityEngine;
using UnityEngine.Events;
using static TerminalApi.Events.Events;
using static TerminalApi.TerminalApi;

namespace TerminalExpansion
{
	[BepInPlugin("dominic.TerminalExpansion", "Dom's Terminal Expansion", "1.0.0")]
	[BepInDependency("atomic.terminalapi")]
	public class Plugin : BaseUnityPlugin
	{
		private bool isInUse;

		private void Awake()
		{
			Logger.LogInfo("TerminalExpansion Plugin is loaded!");

			// Loads the states the terminal can be in during the game
			TerminalAwake +=  TerminalIsAwake;
			TerminalWaking += TerminalIsWaking;
			TerminalStarting += TerminalIsStarting;
			TerminalStarted += TerminalIsStarted;
			TerminalParsedSentence += TextSubmitted;
			TerminalBeginUsing += OnUse;
			TerminalBeganUsing += BeganUsing;
			TerminalExited += OnTerminalExit;
			TerminalTextChanged += OnTerminalTextChanged;
			
			// Adds the time command to the game with the CommandInfo Class to show
			AddCommand("time", new CommandInfo()
			{
				DisplayTextSupplier = () => // Using Lambda Expression to run
                                // these functions in different threads
				{
					Logger.LogWarning("Time command ran!");
					return GetCurrentTime() + "\n";
				},
				Category = "Other",
				Description = "This command will display the current time."
			});

			// Adds the total command to the terminal along with a CommandInfo class
			AddCommand("total", new CommandInfo()
			{
				DisplayTextSupplier = () => // Using Lambda Expression to run
                                // these functions in different threads
				{
					Logger.LogWarning("Total Loot Calculated!");
					return GetLootFormatted() + "\n";
				},
				Category = "Other",
				Description = "This command will calculate the total loot on the ship"
			});
			
			// Adds the door control command to the
			// game along with a CommandInfo Class
			AddCommand("door", new CommandInfo()
			{
				DisplayTextSupplier = () => // Using Lambda Expression to run
                                // these functions in different threads
				{
					Logger.LogWarning("Door Command Ran");
					TerminalKeyword TempWord = new TerminalKeyword(); 
					// Makes a new terminal keyword to allow the
					// command to be executed under different names
					TempWord.isVerb = true; 
					TempWord.word = "toggle";
					AddTerminalKeyword(TempWord);
					return ToggleDoor() + "\n";
				},
				Category = "Other",
				Description = "This command will toggle the hanger door"
			},"toggle", false);
			
			AddCommand("suit", new CommandInfo()
			{
				DisplayTextSupplier = () =>
				{
					Logger.LogMessage($"Getting random suit for player {GetMyPlayerID()}");
					return getRandomSuit() + '\n';
				},
				Category = "Other",
				Description = "This will change the players suit to a new random one"
			},"random", false);
		}
		
		public string ToggleDoor()
		{
			// Check if the ship is in Orbit or not
			if (!StartOfRound.Instance.inShipPhase)
			{
				// Determine the button name based on the hangar doors state
				string buttonName = StartOfRound.Instance.hangarDoorsClosed
					? "StartButton" : "StopButton";

				// Find the corresponding button GameObject
				GameObject buttonObject = GameObject.Find(buttonName);

				// Get the InteractTrigger component from the button
				InteractTrigger interactDoorTrigger = 
					buttonObject?.GetComponentInChildren<InteractTrigger>();

				// Determine the action based on the hangar doors state
				string doorAction = StartOfRound.Instance.hangarDoorsClosed 
					? "opened" : "closed";

				// Log the door state
				Debug.Log($"Hangar doors are {doorAction}.");

				// Invoke the onInteract event if the button and event are found
				UnityEvent<PlayerControllerB> onInteractEvent 
					= interactDoorTrigger?.onInteract as UnityEvent<PlayerControllerB>;

				// Invoke the event only if all components are non-null
				onInteractEvent?.Invoke
					(GameNetworkManager.Instance?.localPlayerController);

				// Log individual messages for open and close events
				if (doorAction == "opened")
				{
					Logger.LogMessage($"Hangar doors {doorAction} successfully" + 
					             $" by interacting with button {buttonName}.");
					return $"{ConfigSettings.doorOpenString}\n";
				}
				else if (doorAction == "closed")
				{
					Logger.LogMessage($"Hangar doors {doorAction} successfully" +
					             $" by interacting with button {buttonName}.");
					return $"{ConfigSettings.doorCloseString}\n";
				}
			}
			else
			{
				return $"{ConfigSettings.doorSpaceString}\n";
			}
			return "";
		}
		
		private void BeganUsing(object sender, Events.TerminalEventArgs e)
		{
			Logger.LogMessage("Player is using terminal");
			isInUse = true;
		}
		
		public float CalculateLootValue()
		{
			// This looks like a lot but basically it grabs all GrabbaleObjects
			// in the HangerShip while ignoring two items on the ship that don't add value
			List<GrabbableObject> list = ((IEnumerable<GrabbableObject>)
					GameObject.Find("/Environment/HangarShip")
						.GetComponentsInChildren<GrabbableObject>())
				.Where<GrabbableObject>(obj => obj.name
				                               != "ClipboardManual" &&
				                               obj.name != "StickyNoteItem").ToList<GrabbableObject>(); 
	        
			return (float)list.Sum<GrabbableObject>(scrap => scrap.scrapValue); 
			// Gets the sum while formatting 
		}

		private string getRandomSuit()
            {
                List<UnlockableSuit> allSuits = new List<UnlockableSuit>();
                List<UnlockableItem> UnlockableSuits = new List<UnlockableItem>();

                //get allSuits
                allSuits = Resources.FindObjectsOfTypeAll<UnlockableSuit>().ToList();
                string displayText = string.Empty;

                if (allSuits.Count > 1)
                {
                    // Order the list by syncedSuitID.Value
                    allSuits = allSuits.OrderBy((UnlockableSuit suit) => suit.suitID).ToList();

                    allSuits.RemoveAll(suit => suit.syncedSuitID.Value < 0); //simply remove bad suit IDs

                    UnlockableSuits = StartOfRound.Instance.unlockablesList.unlockables;

                    
                    int playerID = GetMyPlayerID();


                    if (UnlockableSuits != null)
                    {
                        for (int i = 0; i < UnlockableSuits.Count; i++)
                        {
                            // Get a random index
                            int randomIndex = UnityEngine.Random.Range(0, allSuits.Count);
                            string SuitName;

                            // Get the UnlockableSuit at the random index
                            UnlockableSuit randomSuit = allSuits[randomIndex];
                            if (randomSuit != null && UnlockableSuits[randomSuit.syncedSuitID.Value] != null)
                            {
                                SuitName = UnlockableSuits[randomSuit.syncedSuitID.Value].unlockableName;
                                UnlockableSuit.SwitchSuitForPlayer(StartOfRound.Instance.allPlayerScripts[playerID], randomSuit.syncedSuitID.Value, true);
                                randomSuit.SwitchSuitServerRpc(playerID);
                                randomSuit.SwitchSuitClientRpc(playerID);
                                displayText = $"Changing suit to {SuitName}!\r\n";
                                return displayText;
                            }
                            else
                            {
                                displayText = "A suit could not be found.\r\n";
                                Logger.LogInfo($"Random suit ID was invalid or null");
                                return displayText;
                            }
                        }
                    }

                    displayText = "A suit could not be found.\r\n";
                    Logger.LogMessage($"Unlockables are null");
                    return displayText;
                }
                else
                {
                    displayText = "Not enough suits detected.\r\n";
                    Logger.LogMessage($"allsuits count too low");
                    return displayText;
                }
            }
		
		private int GetMyPlayerID()
		{
			List<PlayerControllerB> allPlayers = new List<PlayerControllerB>();
			string myName = GameNetworkManager.Instance.localPlayerController.playerUsername;
			int returnID = -1;
			allPlayers = StartOfRound.Instance.allPlayerScripts.ToList();
			allPlayers = allPlayers.OrderBy((PlayerControllerB player) => player.playerClientId).ToList();
			for (int i = 0; i < allPlayers.Count; i++)
			{
				if (StartOfRound.Instance.allPlayerScripts[i].playerUsername == myName)
				{
					Logger.LogInfo("Found my playerID");
					returnID = i;
					break;
				}
			}
			if (returnID == -1)
				Logger.LogInfo("Failed to find ID");
			return returnID;
		}
		
		public string GetCurrentTime()
		{
			if (StartOfRound.Instance.currentLevel.planetHasTime &&
			    StartOfRound.Instance.shipDoorsEnabled)
			{
				return "Current time: " +
				       HUDManager.Instance.clockNumber.text.Replace('\n', ' ');
			}
			return "In space time doesn't exist for some reason";
		}
		
		
		// This will run the CalculateLootValue and return it to the terminal
        public string GetLootFormatted()
        {
	        float lootValue = CalculateLootValue();
	        return string.Format("Total Value on Ship: ${0:F0}", (object)lootValue); 
	        //Returns the total value correctly formatted
        }
        
        
		
		// This will run every time a new character is typed into the terminal
		private void OnTerminalTextChanged(object sender,
			Events.TerminalTextChangedEventArgs e)
		{
			string terminalUserInput = GetTerminalInput();
			Logger.LogMessage(terminalUserInput);

			switch (terminalUserInput.ToLower())
			{
				case "dominic":
					SetTerminalInput("How do you know my name?");
					break;
			}
			
		}
		private void OnUse(object sender, Events.TerminalEventArgs e)
		{
			Logger.LogMessage("Player has just started using the terminal");
		}

		private void OnTerminalExit(object sender, Events.TerminalEventArgs e)
		{
			Logger.LogMessage("Terminal Exited");
			isInUse = false;
		}

		private void TerminalIsAwake(object sender, Events.TerminalEventArgs e)
		{
			Logger.LogMessage("Terminal is awake");

			NodeAppendLine("help", "So what are we buying today?\n" +
			                       "Or are we going to Rend?\n");
		}

		private void TerminalIsWaking(object sender, Events.TerminalEventArgs e)
		{
			Logger.LogMessage("Terminal is waking");
		}

		private void TerminalIsStarting(object sender, Events.TerminalEventArgs e)
		{
			Logger.LogMessage("Terminal is starting");
		}

		private void TerminalIsStarted(object sender, Events.TerminalEventArgs e)
		{
			Logger.LogMessage("Terminal is started");
		}

		private void TextSubmitted(object sender,
			Events.TerminalParseSentenceEventArgs e)
		{
			Logger.LogMessage($"Text submitted:" +
			                  $" {e.SubmittedText} Node Returned: {e.ReturnedNode}");
		}
    }
}