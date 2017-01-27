using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Text;

namespace TheGuessingGame
{
    [Serializable]
    public class NumberGuesserDialog : IDialog<object>
    {
        protected int intNumberToGuess;
        protected int intAttempts;

        public async Task StartAsync(IDialogContext context)
        {
            // Generate a random number
            Random random = new Random();
            this.intNumberToGuess = random.Next(1, 10);

            // Set Attempts
            this.intAttempts = 1;

            // Start the Game
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context,
            IAwaitable<IMessageActivity> argument)
        {
            int intGuessedNumber;

            // Get the text passed
            var message = await argument;

            // See if a number was passed
            if (!int.TryParse(message.Text, out intGuessedNumber))
            {
                // A number was not passed                
                await context.PostAsync("Hi Welcome! - Guess a number between 1 and 10");
                context.Wait(MessageReceivedAsync);
            }

            // This code will run when the user has entered a number
            if (int.TryParse(message.Text, out intGuessedNumber))
            {
                // A number was passed
                // See if it was the correct number
                if (intGuessedNumber != this.intNumberToGuess)
                {
                    // The number was not correct
                    this.intAttempts++;
                    await context.PostAsync("Not correct. Guess again.");
                    context.Wait(MessageReceivedAsync);
                }
                else
                {
                    // Game completed
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Congratulations! ");
                    sb.Append("The number to guess was {0}. ");
                    sb.Append("You needed {1} attempts. ");
                    sb.Append("Would you like to play again?");

                    string CongratulationsStringPrompt =
                        string.Format(sb.ToString(),
                        this.intNumberToGuess,
                        this.intAttempts);

                    // Put PromptDialog here
                    PromptDialog.Confirm(
                        context,
                        PlayAgainAsync,
                        CongratulationsStringPrompt,
                        "Didn't get that!");
                }
            }
        }

        private async Task PlayAgainAsync(IDialogContext context, IAwaitable<bool> result)
        {
            // Generate new random number
            Random random = new Random();
            this.intNumberToGuess = random.Next(1, 10);

            // Reset attempts
            this.intAttempts = 1;

            // Get the response from the user
            var confirm = await result;

            if (confirm) // They said yes
            {
                // Start a new Game
                await context.PostAsync("Hi Welcome! - Guess a number between 1 and 10");
                context.Wait(MessageReceivedAsync);
            }
            else // They said no
            {
                await context.PostAsync("Goodbye!");
                context.Wait(MessageReceivedAsync);
            }
        }
    }
}