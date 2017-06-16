using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Cards.Bot.Helpers;

namespace Cards.Bot.Dialogs
{
    [Serializable]
    public class StandardCardDialog : IDialog<object>
    {
        private const string ReceiptCard = "Receipt card";
        private const string AnimationCard = "Animation card";
        private const string VideoCard = "Video card";
        private const string CarouselCard = "Carousel card";
        private IEnumerable<string> ChoiceOptions = new List<string> { ReceiptCard, AnimationCard, VideoCard, CarouselCard };

        public async Task StartAsync(IDialogContext context)
        {
            await PromptCardChoice(context);
        }

        public async Task PromptCardChoice(IDialogContext context)
        {
            PromptDialog.Choice(context,
                DisplaySelectedCard,
                ChoiceOptions,
                "Select any of the available standard cards",
                "The option you've selected is not valid, try again!",
                3,
                PromptStyle.Auto);
        }

        public async Task DisplaySelectedCard(IDialogContext context, IAwaitable<string> result)
        {
            var userChoice = await result;
            var message = context.MakeMessage();
            message.Speak = SpeechHelper.Speak($"You've currently selected <emphasis level=\"strong\">{userChoice}</emphasis>");

            if (userChoice.ToLower() == "carousel card")
            {
                message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                message.Attachments = GetCardsAttachments();
            }
            else
            {
                var card = CreateStandardCard(userChoice);
                message.Attachments.Add(card);
            }

            await context.PostAsync(message);

            context.Wait(ContinueStandardCard);
        }

        public async Task ContinueStandardCard(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            PromptDialog.Choice(context,
                ContinueStandardCardConfirm,
                new string[] { "Yes", "No" },
                "Do you stil want to look at the other standard cards?",
                "Select Yes or No only",
                3);
        }

        public async Task ContinueStandardCardConfirm(IDialogContext context, IAwaitable<string> result)
        {
            var message = await result;
            if(message.ToLower() == "yes")
            {
                await PromptCardChoice(context);
            }
            else
            {
                context.Done(new object());
            }
        }

        private static Attachment CreateStandardCard(string selectedCard)
        {
            switch (selectedCard)
            {
                case ReceiptCard:
                    return GetReceiptCard();
                case AnimationCard:
                    return GetAnimationCard();
                case VideoCard:
                    return GetVideoCard();
                default:
                    return GetReceiptCard();
            }
        }

        private static IList<Attachment> GetCardsAttachments()
        {
            var attachmentList = new List<Attachment>();

            for(int i = 1; i <= 5; i++)
            {
                attachmentList.Add(GetHeroCard($"Picture {i}",
                    $"Subtitle - {i}",
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                    $"https://placeholdit.imgix.net/~text?txtsize=35&txt=Picture {i}"));
            }

            return attachmentList;
        }

        private static Attachment GetHeroCard(string title, string subtitle, string text,
            string imageUrl)
        {
            var heroCard = new HeroCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage> { new CardImage(imageUrl) },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "This Button is props", value: "https://blog.botframework.com") }
            };

            return heroCard.ToAttachment();
        }

        private static Attachment GetReceiptCard()
        {
            var receiptCard = new ReceiptCard
            {
                Title = "Foo Bar",
                Facts = new List<Fact> { new Fact("Order Number", "5555"), new Fact("Payment Method", "Paypal") },
                Items = new List<ReceiptItem>
                {
                    new ReceiptItem("Hawaiian Pizza", price: "PHP 200", quantity: "3", image: new CardImage(url: "https://placeholdit.imgix.net/~text?txtsize=35&txt=Hawaiian%20Pizza")),
                    new ReceiptItem("Chicken", price: "PHP 50", quantity: "5", image: new CardImage(url: "https://placeholdit.imgix.net/~text?txtsize=35&txt=Chicken")),
                },
                Tax = "PHP 7.50",
                Total = "PHP 257.5"
            };

            return receiptCard.ToAttachment();
        }

        private static Attachment GetAnimationCard()
        {
            var animationCard = new AnimationCard
            {
                Title = "Happy Flash",
                Subtitle = "Animation Card",
                Media = new List<MediaUrl>
                {
                    new MediaUrl()
                    {
                        Url = "https://media.giphy.com/media/3NtY188QaxDdC/giphy.gif"
                    }
                }
            };

            return animationCard.ToAttachment();
        }

        private static Attachment GetVideoCard()
        {
            var videoCard = new VideoCard
            {
                Title = "Big Buck Bunny",
                Subtitle = "by the Blender Institute",
                Text = "Big Buck Bunny (code-named Peach) is a short computer-animated comedy film by the Blender Institute, part of the Blender Foundation. Like the foundation's previous film Elephants Dream, the film was made using Blender, a free software application for animation made by the same foundation. It was released as an open-source film under Creative Commons License Attribution 3.0.",
                Image = new ThumbnailUrl
                {
                    Url = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c5/Big_buck_bunny_poster_big.jpg/220px-Big_buck_bunny_poster_big.jpg"
                },
                Media = new List<MediaUrl>
                {
                    new MediaUrl()
                    {
                        Url = "http://download.blender.org/peach/bigbuckbunny_movies/BigBuckBunny_320x180.mp4"
                    }
                }
            };

            return videoCard.ToAttachment();
        }
    }
}