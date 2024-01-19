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
using TerminalApi.Events;
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
			                       "Or are we going to Titan?\n");
			
		
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

		public void LogMessage(string str)
		{
			Logger.LogMessage(str);
		}

		public void LogWarning(string str)
		{
			Logger.LogWarning(str);
		}
    }
}