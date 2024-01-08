using System.Threading.Tasks;
using BepInEx;
using System.Web;
using TerminalApi;
using TerminalApi.Classes;
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

            //Dom's First Test

            AddCommand("domstime", new CommandInfo()
            {
                DisplayTextSupplier = () =>
                {
                    Logger.LogWarning("Wowow, this ran.");
					return domsTime() + "\n";
                },
                Category = "Other"
            });

			
        }

		private string domsTime()
		{
			if (StartOfRound.Instance.currentLevel.planetHasTime && StartOfRound.Instance.shipDoorsEnabled)
				return "YesToTime";
			return "NoToTime";
		}

        private void OnTerminalTextChanged(object sender, TerminalTextChangedEventArgs e)
        {
			string userInput = GetTerminalInput();
			Logger.LogMessage(userInput);
			// Or
			Logger.LogMessage(e.CurrentInputText);

			if(userInput.Equals("Jew"))
			{
				SetTerminalInput("Jew??\n Eww don't type that!");
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

			NodeAppendLine("help", "\nSo what are we buying today?\n");
			
		
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

    }
}