using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using GameNetcodeStuff;
using TerminalExpansion;
using TerminalStuff;
using UnityEngine;
using UnityEngine.Events;

namespace TimeTerminalCommand;

public class Commands
{
	private Plugin _logger = new Plugin();
	public string DoorCommand()
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
			_logger.LogMessage($"Hangar doors are {action}.");

			// Invoke the onInteract event if the button and event are found
			if (interactTrigger != null)
			{
				UnityEvent<PlayerControllerB> onInteractEvent =
					interactTrigger.onInteract as UnityEvent<PlayerControllerB>;

				if (onInteractEvent != null)
				{
					onInteractEvent.Invoke(GameNetworkManager.Instance.localPlayerController);

					// Log individual messages for open and close events
					if (action == "opened")
					{
						_logger.LogMessage(
							$"Hangar doors {action} successfully by interacting with button {buttonName}.");
						return $"{ConfigSettings.doorOpenString.Value}\n";
					}
					else if (action == "closed")
					{ _logger.LogMessage(
							$"Hangar doors {action} successfully by interacting with button {buttonName}.");
						return $"{ConfigSettings.doorCloseString.Value}\n";
					}
				}
				else
				{
					// Log if onInteractEvent is null
					_logger.LogWarning($"Warning: onInteract event is null for button {buttonName}.");
				}
			}
			else
			{
				// Log if interactTrigger is null
				_logger.LogWarning($"Warning: InteractTrigger not found on button {buttonName}.");
			}
		}
		else
		{
			return $"{ConfigSettings.doorSpaceString.Value}\n";
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
        public String getLoot()
        {
	        String totalvalue = string.Empty;
	        float lootValue = CalculateLootValue();
	        return string.Format("Total Value on Ship: ${0:F0}", (object)lootValue); 
        }
        public float CalculateLootValue()
        {
	        List<GrabbableObject> list = ((IEnumerable<GrabbableObject>)GameObject.Find("/Environment/HangarShip").GetComponentsInChildren<GrabbableObject>())
		        .Where<GrabbableObject>(obj => obj.name != "ClipboardManual" && obj.name != "StickyNoteItem").ToList<GrabbableObject>();
	        
	        return (float)list.Sum<GrabbableObject>(scrap => scrap.scrapValue);
        }
}