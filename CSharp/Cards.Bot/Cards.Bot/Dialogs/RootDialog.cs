using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace Cards.Bot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string AdaptiveCard = "Adaptive Card";
        private const string StandardCard = "Standard Card";

        public async Task StartAsync(IDialogContext context)
        {
            // See https://github.com/Microsoft/BotBuilder-Samples for more examples

            context.Wait(MessageReceivedStart);
        }

        private async Task MessageReceivedStart(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            // Creates a Card within a dialog
            var replyToConversation = context.MakeMessage();

            // Converts the card into an Attachment object and add it to Attachments list
            replyToConversation.Attachments.Add(CreateGreetingsCard());
            await context.PostAsync(replyToConversation);

            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            switch (message.Text)
            {
                case StandardCard:
                    context.Call(new StandardCardDialog(), MessageReceivedDone);
                    break;
                case AdaptiveCard:
                    context.Call(new AdaptiveCardDialog(), MessageReceivedDone);
                    break;
                default:
                    await context.PostAsync("Please select either Standard Card or Adaptive Card to proceed");
                    await MessageReceivedStart(context, result);
                    break;
            }
        }

        private async Task MessageReceivedDone(IDialogContext context, IAwaitable<object> result)
        {
            await MessageReceivedStart(context, null);
        }

        private static Attachment CreateGreetingsCard()
        {
            return new HeroCard
            {
                Title = "I'm a Visual Card Bot",
                Subtitle = "What's up? What do you want to do?",
                Images = new List<CardImage>
                {
                    new CardImage { Url = "http://adaptivecards.io/api/cat" }
                },
                Buttons = new List<CardAction>
                {
                    new CardAction { Title = "Standard Cards", Value = StandardCard },
                    new CardAction { Title = "Adaptive Cards", Value = AdaptiveCard }
                }
            }.ToAttachment();
        }
    }
}