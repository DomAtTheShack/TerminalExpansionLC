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
using Random = UnityEngine.Random;

namespace TerminalExpansion
{
	[BepInPlugin("dominic.TerminalExpansion", "Dom's Terminal Expansion", "1.0.0")]
	[BepInDependency("atomic.terminalapi")]
	public class Plugin : BaseUnityPlugin
	{
		// Will be true while a player is currently using the terminal
		private bool isInUse;

		/*
		This method is the Main startup of the plugin 
		This holds all commands that have been added and terminal states
		*/
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
			
			//Adds the command to randomly get a new suit to apply to the player
			AddCommand("suit", new CommandInfo()
			{
				DisplayTextSupplier = () =>
				{
					Logger.LogMessage($"Getting random suit for player {GetMyPlayerID()}");
					return GetRandomSuit() + '\n';
				},
				Category = "Other",
				Description = "This will change the players suit to a new random one"
			},"random", false);
			
			AddCommand("lights", new CommandInfo()
			{
				DisplayTextSupplier = () =>
				{
					string lightStatus = 
						StartOfRound.Instance.shipRoomLights.areLightsOn ? "off" : "on";
					Logger.LogMessage($"Toggling lights {lightStatus}!");
					return ToggleShipLights() + '\n';
				},
				Category = "Other",
				Description = "This will toggle the ship lights on or off"
			},"light", false);
			
			AddCommand("bioscan", new CommandInfo()
			{
				DisplayTextSupplier = () =>
				{
					Logger.LogMessage("Scanning the base...");
					return ScanBase() + '\n';
				},
				Category = "Other",
				Description = "Will scan the base and list the current enemies"
			});
		}
		
		/*
		This toggles the ship door open or closed or
		not at all depending on ship location and current state
		*/
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

		/*
		This will toggle the ship lights and return
		a message to the terminal based on current state
		*/
		private string ToggleShipLights()
		{
			string lightStatus;
			if (StartOfRound.Instance.shipRoomLights.areLightsOn)
			{
				lightStatus = "Turing the ship lights off!";
			}
			else
			{
				lightStatus = "Turing the ship lights on!";
			}
			StartOfRound.Instance.shipRoomLights.ToggleShipLights();
			return lightStatus;
		}
		
		/*
		 * Sets the isInUse bool true to indicate that the terminal is in use
		 */
		private void BeganUsing(object sender, Events.TerminalEventArgs e)
		{
			Logger.LogMessage("Player is using terminal");
			isInUse = true;
		}
		
		/*
		 * Using a list it gets all sellable items and adds them up to show to the player
		 */
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

		/*
		 * Will check current suits and including
		 modded suits it will randomly assign a new suit
		 */
		private string GetRandomSuit()
            {
                List<UnlockableSuit> allSuits = new List<UnlockableSuit>();
                List<UnlockableItem> UnlockableSuits = new List<UnlockableItem>();

                //get allSuits
                allSuits = Resources.FindObjectsOfTypeAll<UnlockableSuit>().ToList();
                string displayText = string.Empty;

                if (allSuits.Count > 1)
                {
                    // Order the list by syncedSuitID.Value
                    allSuits = allSuits.OrderBy((UnlockableSuit suit) =>
	                    suit.suitID).ToList();

                    allSuits.RemoveAll(suit =>
	                    suit.syncedSuitID.Value < 0); //simply remove bad suit IDs

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
                            if (randomSuit != null && UnlockableSuits
	                                [randomSuit.syncedSuitID.Value] != null)
                            {
                                SuitName = UnlockableSuits
	                                [randomSuit.syncedSuitID.Value].unlockableName;
                                UnlockableSuit.SwitchSuitForPlayer(
	                                StartOfRound.Instance.allPlayerScripts[playerID],
	                                randomSuit.syncedSuitID.Value, true);
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
		
		/*
		 * Returns the current player ID to be used to assign a suit or other stuff
		 */
		private int GetMyPlayerID()
		{
			List<PlayerControllerB> allPlayers = new List<PlayerControllerB>();
			string myName = GameNetworkManager
				.Instance.localPlayerController.playerUsername;
			int returnID = -1;
			allPlayers = StartOfRound.Instance.allPlayerScripts.ToList();
			allPlayers = allPlayers.OrderBy((PlayerControllerB player) =>
				player.playerClientId).ToList();
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
		/*
		 * Gets the current time if on a planet with time
		 */
		public string GetCurrentTime()
		{
			if (!StartOfRound.Instance.currentLevel.planetHasTime)
			{
				return "The company keeps the clock from ticking here..."; 
			}
			if(StartOfRound.Instance.shipDoorsEnabled)
			{
				return "Current time: " +
				       HUDManager.Instance.clockNumber.text.Replace('\n', ' ');
			}
			return "In space time doesn't exist for some reason";
		}
		
		
		/*
		 * This will run the CalculateLootValue and return it to the terminal
		 */
        public string GetLootFormatted()
        {
	        float lootValue = CalculateLootValue();
	        return string.Format("Total Value on Ship: ${0:F0}", (object)lootValue); 
	        //Returns the total value correctly formatted
        }
        
        
		
		/*
		 * This will run every time a new character is typed into the terminal
		 */
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

		/**
		 * This will only scan the base if landed on a planet
		 * If enemies are found it will list how many there are and what type are around
		 */
		private string ScanBase()
		{
			if (StartOfRound.Instance.currentLevel.planetHasTime &&
			    
			    StartOfRound.Instance.shipDoorsEnabled)
			{
				//This gets all the spawned Enemies then after getting them it will
				//get the amount of enemies spawned that aren't players
				int totalEnemies = RoundManager.Instance.SpawnedEnemies.Count;

				string badNumber = totalEnemies.ToString();

				List<EnemyAI> listOfEnemies = RoundManager.
					Instance.SpawnedEnemies.ToList();

				var livingEnemies = 
					listOfEnemies.Where(enemy => !enemy.isEnemyDead);

				//Here is where it uses a pattern
				string livingEnemyString = string.Join
					(Environment.NewLine, livingEnemies.Select
						(enemy => enemy.ToString()));
				
				string pattern = @"\([^)]*\)";
				
				string filteredString = Regex.Replace
					(livingEnemyString, pattern, string.Empty);
				
				Logger.LogMessage(totalEnemies + " " + badNumber + " " +
				                  livingEnemies.ToString() + " " +
				                  livingEnemyString + " " + filteredString);

				return $"detected {badNumber} non-employee organic objects."
				       + '\r' +'\n' +'\r' +'\n' +
				       $"Detailed scan has defined these objects as" +
				       $" the following in the registry:" +
				       $" \r\n{filteredString}\r\n";
			}

			return "Your not on a planet or at least one with enemies...";
		}
		
		/*
		 * All methods from here and below just deal with terminal states and nothing else
		 * Terminal Events Args are the arguments that are applied to the command
		 * sender object is what sends the action to the terminal and to the game logic
		 */
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