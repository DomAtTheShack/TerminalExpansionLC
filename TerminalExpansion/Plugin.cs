using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BepInEx;
using System.Web;
using GameNetcodeStuff;
using TerminalApi;
using TerminalApi.Classes;
using TerminalStuff;
using UnityEngine;
using UnityEngine.Events;
using static System.Net.Mime.MediaTypeNames;
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
            
            //Add the time command
            
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
		            return getLoot() + DoorCommand() + "\n";
	            },
	            Category = "Other",
	            Description = "This command will display the current time."
            });
            
		}
		public string KillPlayerCommand(PlayerControllerB player)
		{
			if (StartOfRound.Instance == null)
				return "Game not started";

			player.KillPlayer(Vector3.up);
			return $"Killed {player.playerUsername}";
		}

		private string DoorCommand()
		{
			 if (!StartOfRound.Instance.inShipPhase)
                        {
                            //thanks chatgpt for the breakdown

                            // Determine the button name based on the hangar doors state
                            string buttonName = StartOfRound.Instance.hangarDoorsClosed ? "StartButton" : "StopButton";

                            // Find the corresponding button GameObject
                            GameObject buttonObject = GameObject.Find(buttonName);

                            // Get the InteractTrigger component from the button
                            InteractTrigger interactTrigger = buttonObject.GetComponentInChildren<InteractTrigger>();

                            // Determine the action based on the hangar doors state
                            string action = StartOfRound.Instance.hangarDoorsClosed ? "opened" : "closed";

                            // Log the door state
                            Logger.LogMessage($"Hangar doors are {action}.");

                            // Invoke the onInteract event if the button and event are found
                            if (interactTrigger != null)
                            {
                                UnityEvent<PlayerControllerB> onInteractEvent = interactTrigger.onInteract as UnityEvent<PlayerControllerB>;

                                if (onInteractEvent != null)
                                {
                                    onInteractEvent.Invoke(GameNetworkManager.Instance.localPlayerController);

                                    // Log individual messages for open and close events
                                    if (action == "opened")
                                    {
                                        Logger.LogMessage($"Hangar doors {action} successfully by interacting with button {buttonName}.");
                                        return $"{ConfigSettings.doorOpenString.Value}\n";
                                    }
                                    else if (action == "closed")
                                    {
                                        Logger.LogMessage($"Hangar doors {action} successfully by interacting with button {buttonName}.");
                                        return $"{ConfigSettings.doorCloseString.Value}\n";
                                    }
                                }
                                else
                                {
                                    // Log if onInteractEvent is null
                                    Logger.LogWarning($"Warning: onInteract event is null for button {buttonName}.");
                                }
                            }
                            else
                            {
                                // Log if interactTrigger is null
                                Logger.LogWarning($"Warning: InteractTrigger not found on button {buttonName}.");
                            }
                        }
                        else
                        {
                            return $"{ConfigSettings.doorSpaceString.Value}\n";
                        }

			return "";
		}
		
		private string Time()
		{
			if (StartOfRound.Instance.currentLevel.planetHasTime &&
			    StartOfRound.Instance.shipDoorsEnabled)
			{
				return "Current time: " +
				       HUDManager.Instance.clockNumber.text.Replace('\n', ' ');
			}
			return "Time isn't Available";
		}

        private void OnTerminalTextChanged(object sender, TerminalTextChangedEventArgs e)
        {
			string userInput = GetTerminalInput();
			Logger.LogMessage(userInput);

			if(userInput.Equals("Dominic"))
			{
				SetTerminalInput("How do you know my name?");
			}
			
        }
        

        private void OnTerminalExit(object sender, TerminalEventArgs e)
        {
            Logger.LogMessage("Terminal Exited");
            isInUse = true;
        }

        private void TerminalIsAwake(object sender, TerminalEventArgs e)
        {
	        Logger.LogMessage("Terminal is awake");

			NodeAppendLine("help", "So what are we buying today?\n" +
			                       "Or are we going to Titan?\n");
			
		
        }

		private void TerminalIsWaking(object sender, TerminalEventArgs e)
		{
			Logger.LogMessage("Terminal is waking");
		}

		private void TerminalIsStarting(object sender, TerminalEventArgs e)
		{
			Logger.LogMessage("Terminal is starting");
		}

		private void TerminalIsStarted(object sender, TerminalEventArgs e)
		{
			Logger.LogMessage("Terminal is started");
		}

        private void TextSubmitted(object sender, TerminalParseSentenceEventArgs e)
        {
            Logger.LogMessage($"Text submitted: {e.SubmittedText} Node Returned: {e.ReturnedNode}");
        }

		private void OnBeginUsing(object sender, TerminalEventArgs e)
		{
            Logger.LogMessage("Player has just started using the terminal");
        }

        private void BeganUsing(object sender, TerminalEventArgs e)
        {
            Logger.LogMessage("Player is using terminal");
            isInUse = true;
        }

        public static String getLoot()
        {
	        String totalvalue = string.Empty;
	        float lootValue = CalculateLootValue();
	        return string.Format("Total Value on Ship: ${0:F0}", (object)lootValue); 
        }
        public static float CalculateLootValue()
        {
	        List<GrabbableObject> list = ((IEnumerable<GrabbableObject>)GameObject.Find("/Environment/HangarShip").GetComponentsInChildren<GrabbableObject>())
		        .Where<GrabbableObject>(obj => obj.name != "ClipboardManual" && obj.name != "StickyNoteItem").ToList<GrabbableObject>();
	        
	        return (float)list.Sum<GrabbableObject>(scrap => scrap.scrapValue);
        }
    }
}