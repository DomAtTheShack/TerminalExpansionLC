using System;
using System.Collections.Generic;
using System.Linq;
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
			Logger.LogInfo("Plugin Test Plugin is loaded!");

			// Loads the states the terminal can be in during the game
			TerminalAwake +=  TerminalIsAwake;
			TerminalWaking += TerminalIsWaking;
			TerminalStarting += TerminalIsStarting;
			TerminalStarted += TerminalIsStarted;
			TerminalParsedSentence += TextSubmitted;
			TerminalBeginUsing += OnBeginUsing;
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
					return Time() + "\n";
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
					return getLoot() + "\n";
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
					return DoorCommand() + "\n";
				},
				Category = "Other",
				Description = "This command will toggle the hanger door"
			},"toggle", false);
		}
		
		public string DoorCommand()
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
				InteractTrigger interactTrigger = 
					buttonObject?.GetComponentInChildren<InteractTrigger>();

				// Determine the action based on the hangar doors state
				string action = StartOfRound.Instance.hangarDoorsClosed 
					? "opened" : "closed";

				// Log the door state
				Debug.Log($"Hangar doors are {action}.");

				// Invoke the onInteract event if the button and event are found
				UnityEvent<PlayerControllerB> onInteractEvent 
					= interactTrigger?.onInteract as UnityEvent<PlayerControllerB>;

				// Invoke the event only if all components are non-null
				onInteractEvent?.Invoke
					(GameNetworkManager.Instance?.localPlayerController);

				// Log individual messages for open and close events
				if (action == "opened")
				{
					Logger.LogMessage($"Hangar doors {action} successfully" + 
					             $" by interacting with button {buttonName}.");
					return $"{ConfigSettings.doorOpenString}\n";
				}
				else if (action == "closed")
				{
					Logger.LogMessage($"Hangar doors {action} successfully" +
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

		public string Time()
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
        public string getLoot()
        {
	        string totalvalue = string.Empty;
	        float lootValue = CalculateLootValue();
	        return string.Format("Total Value on Ship: ${0:F0}", (object)lootValue); 
	        //Returns the total value correctly formatted
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
		
		// This will run every time a new character is typed into the terminal
		private void OnTerminalTextChanged(object sender,
			Events.TerminalTextChangedEventArgs e)
		{
			string userInput = GetTerminalInput();
			Logger.LogMessage(userInput);

			switch (userInput.ToLower())
			{
				case "dominic":
					SetTerminalInput("How do you know my name?");
					break;
			}

			
		}

		private void OnTerminalExit(object sender, Events.TerminalEventArgs e)
		{
			Logger.LogMessage("Terminal Exited");
			isInUse = true;
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

		private void OnBeginUsing(object sender, Events.TerminalEventArgs e)
		{
			Logger.LogMessage("Player has just started using the terminal");
		}

		private void BeganUsing(object sender, Events.TerminalEventArgs e)
		{
			Logger.LogMessage("Player is using terminal");
			isInUse = true;
		}
    }
}