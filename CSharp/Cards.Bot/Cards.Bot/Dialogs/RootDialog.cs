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
            context.Wait(MessageReceivedStart);
        }

        private async Task MessageReceivedStart(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            // Creates a Card within a dialog
            Activity replyToConversation = (Activity)context.MakeMessage();
            //replyToConversation.AttachmentLayout = AttachmentLayoutTypes.List;
            replyToConversation.Attachments = new List<Attachment>();

            // creates a card image 
            var cardImages = new List<CardImage>
            {
                new CardImage
                {
                    Url = "http://adaptivecards.io/api/cat"
                }
            };

            // creates button that will be used for the hero card
            var cardButtons = new List<CardAction>
            {
                new CardAction
                {
                    Title = "Standard Cards",
                    Value = StandardCard
                },
                new CardAction
                {
                    Title = "Adaptive Cards",
                    Value = AdaptiveCard
                }
            };

            // creates the hero card and attach the cardImages and cardButtons we've just created
            var heroCard = new HeroCard
            {
                Title = "I'm a Visual Card Bot",
                Subtitle = "What's up? What do you want to do?",
                Images = cardImages,
                Buttons = cardButtons
            };

            // Converts the card into an Attachment object and add it to Attachments list
            replyToConversation.Attachments.Add(heroCard.ToAttachment());
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
                    break;
                default:
                    await context.PostAsync("Please select either Standard Card or Adaptive Card to proceed");
                    await MessageReceivedStart(context, result);
                    break;
            }
        }

        private async Task MessageReceivedDone(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceivedStart);
        }
    }
}