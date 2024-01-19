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

			TerminalAwake +=  TerminalIsAwake;
			TerminalWaking += TerminalIsWaking;
			TerminalStarting += TerminalIsStarting;
			TerminalStarted += TerminalIsStarted;
			TerminalParsedSentence += TextSubmitted;
			TerminalBeginUsing += OnBeginUsing;
			TerminalBeganUsing += BeganUsing;
			TerminalExited += OnTerminalExit;
			TerminalTextChanged += OnTerminalTextChanged;
			
			// Now that _commandMgr is initialized, add your commands
			AddCommand("time", new CommandInfo()
			{
				DisplayTextSupplier = () =>
				{
					Logger.LogWarning("Time command ran!");
					return Time() + "\n";
				},
				Category = "Other",
				Description = "This command will display the current time."
			});

			AddCommand("total", new CommandInfo()
			{
				DisplayTextSupplier = () =>
				{
					Logger.LogWarning("Total Loot Calculated!");
					return getLoot() + "\n";
				},
				Category = "Other",
				Description = "This command will calculate the total loot on the ship"
			});
			AddCommand("door", new CommandInfo()
			{
				DisplayTextSupplier = () =>
				{
					Logger.LogWarning("Door Command Ran");
					TerminalKeyword TempWord = new TerminalKeyword();
					TempWord.isVerb = true;
					TempWord.word = "toggle";
					AddTerminalKeyword(TempWord);
					return DoorCommand() + "\n";
				},
				Category = "Other",
				Description = "This command will toggle the hanger door"
			},"toggle", false);
		}

		public string KillPlayerCommand(PlayerControllerB player)
		{
			if (StartOfRound.Instance == null)
				return "Game not started";

			player.KillPlayer(Vector3.up);
			return $"Killed {player.playerUsername}";
		}
		
		public string DoorCommand()
		{
			if (!StartOfRound.Instance.inShipPhase)
			{
				// Determine the button name based on the hangar doors state
				string buttonName = StartOfRound.Instance.hangarDoorsClosed ? "StartButton" : "StopButton";

				// Find the corresponding button GameObject
				GameObject buttonObject = GameObject.Find(buttonName);

				// Get the InteractTrigger component from the button
				InteractTrigger interactTrigger = buttonObject?.GetComponentInChildren<InteractTrigger>();

				// Determine the action based on the hangar doors state
				string action = StartOfRound.Instance.hangarDoorsClosed ? "opened" : "closed";

				// Log the door state
				Debug.Log($"Hangar doors are {action}.");

				// Invoke the onInteract event if the button and event are found
				UnityEvent<PlayerControllerB> onInteractEvent = interactTrigger?.onInteract as UnityEvent<PlayerControllerB>;

				// Invoke the event only if all components are non-null
				onInteractEvent?.Invoke(GameNetworkManager.Instance?.localPlayerController);

				// Log individual messages for open and close events
				if (action == "opened")
				{
					Logger.LogMessage($"Hangar doors {action} successfully by interacting with button {buttonName}.");
					return $"{ConfigSettings.doorOpenString}\n";
				}
				else if (action == "closed")
				{
					Logger.LogMessage($"Hangar doors {action} successfully by interacting with button {buttonName}.");
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
			return "Time isn't Available";
		}
        public string getLoot()
        {
	        string totalvalue = string.Empty;
	        float lootValue = CalculateLootValue();
	        return string.Format("Total Value on Ship: ${0:F0}", (object)lootValue); 
        }
        public float CalculateLootValue()
        {
	        List<GrabbableObject> list = ((IEnumerable<GrabbableObject>)GameObject.Find("/Environment/HangarShip").GetComponentsInChildren<GrabbableObject>())
		        .Where<GrabbableObject>(obj => obj.name != "ClipboardManual" && obj.name != "StickyNoteItem").ToList<GrabbableObject>();
	        
	        return (float)list.Sum<GrabbableObject>(scrap => scrap.scrapValue);
        }
		
		
		private void OnTerminalTextChanged(object sender, Events.TerminalTextChangedEventArgs e)
		{
			string userInput = GetTerminalInput();
			Logger.LogMessage(userInput);

			if(userInput.Equals("Dominic"))
			{
				SetTerminalInput("How do you know my name?");
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

		private void TextSubmitted(object sender, Events.TerminalParseSentenceEventArgs e)
		{
			Logger.LogMessage($"Text submitted: {e.SubmittedText} Node Returned: {e.ReturnedNode}");
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