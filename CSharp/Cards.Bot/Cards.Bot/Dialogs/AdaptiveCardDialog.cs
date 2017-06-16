using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using AdaptiveCards;
using Newtonsoft.Json;
using Cards.Bot.Models;

namespace Cards.Bot.Dialogs
{
    [Serializable]
    public class AdaptiveCardDialog : IDialog<object>
    {
        private const string InputFormCard = "InputForm card";
        private const string VSTSCard = "VSTS card";
        private IEnumerable<string> ChoiceOptions = new List<string> { InputFormCard, VSTSCard };
        private static string _cardChoice = "";

        public async Task StartAsync(IDialogContext context)
        {
            await PromptCardChoice(context);
        }

        public async Task PromptCardChoice(IDialogContext context)
        {
            PromptDialog.Choice(context,
                DisplaySelectedCard,
                ChoiceOptions,
                "Select any of the available Adaptive cards",
                "The option you've selected is not valid, try again!",
                3,
                PromptStyle.Auto);
        }

        public async Task DisplaySelectedCard(IDialogContext context, IAwaitable<string> result)
        {
            _cardChoice = "";
            var userChoice = await result;
            var message = context.MakeMessage();

            var card = CreateAdaptiveCard(userChoice);
            var attachment = new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
            message.Attachments.Add(attachment);

            await context.PostAsync(message);

            context.Wait(ContinueAdaptiveCard);
        }

        public async Task ContinueAdaptiveCard(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var userMessage = await result;
            var message = context.MakeMessage();

            if (_cardChoice == InputFormCard)
            {
                // Gets value from the Adaptive Card Form
                var jsonObj = JsonConvert.DeserializeObject<UserInformation>(userMessage.Value.ToString());
                var attachment = new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = CreateFactSetCard(jsonObj.FullName, jsonObj.EmailAddress)
                };

                message.Attachments.Add(attachment);
                await context.PostAsync(message);
            }
            else
            {
                await context.PostAsync("Closing the bug now...");
            }

            PromptDialog.Choice(context,
              ContinueChoicePrompt,
              new string[] { "Yes", "No" },
              "Do you stil want to look at the other adaptive cards?",
              "Select Yes or No only",
              3);
        }

        public async Task ContinueChoicePrompt(IDialogContext context, IAwaitable<string> result)
        {
            var message = await result;
            if (message.ToLower() == "yes")
            {
                await PromptCardChoice(context);
            }
            else
            {
                context.Done(new object());
            }
        }

        private static AdaptiveCard CreateAdaptiveCard(string selectedCard)
        {
            switch (selectedCard)
            {
                case InputFormCard:
                    _cardChoice = InputFormCard;
                    return CreateInputFormCard();
                case VSTSCard:
                    _cardChoice = VSTSCard;
                    return CreateVSTSCard();
                default:
                    _cardChoice = InputFormCard;
                    return CreateInputFormCard();
            }
        }

        private static AdaptiveCard CreateVSTSCard()
        {
            var card = new AdaptiveCard();

            card.Body.Add(new TextBlock
            {
                Text = "Visual Studio Team Services Bug Card",
                Weight = TextWeight.Bolder,
                Size = TextSize.Medium
            });

            card.Body.Add(new ColumnSet
            {
                Columns = new List<Column>
                {
                    new Column
                    {
                        Size = "1",
                        Items = new List<CardElement>
                        {
                            new Image
                            {
                                Url = "https://placeholdit.imgix.net/~text?txtsize=150&txt=01",
                                Size = ImageSize.Medium
                            }
                        }
                    },
                    new Column
                    {
                        Size = "11",
                        Items = new List<CardElement>
                        {
                            new TextBlock
                            {
                                Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                                Wrap = true
                            }
                        }
                    }
                }
            });

            card.Body.Add(new FactSet
            {
                Facts = new List<AdaptiveCards.Fact>
                {
                    new AdaptiveCards.Fact
                    {
                        Title = "Work item type:",
                        Value = "Bug"
                    },
                    new AdaptiveCards.Fact
                    {
                        Title = "State:",
                        Value = "Active"
                    },
                    new AdaptiveCards.Fact
                    {
                        Title = "Area Path",
                        Value = @"t365\Tabs\Extensibility\longer to show\two\lines"
                    },
                    new AdaptiveCards.Fact
                    {
                        Title = "Iteration Path",
                        Value = @"t365\Tabs\Extensibility\longer to show\"
                    },
                    new AdaptiveCards.Fact
                    {
                        Title = "Priority",
                        Value = "1"
                    }
                }
            });

            // NOTE: There's a bug in SubmitAction when you try to use ColumnSet.
            // No value is being passed to the dialog
            card.Actions.Add(new SubmitAction
            {
                Title = "Close Bug"
            });

            card.Actions.Add(new HttpAction
            {
                Url = "https://www.google.com",
                Title = "View more work items"
            });

            return card;
        }

        private static AdaptiveCard CreateInputFormCard()
        {
            var card = new AdaptiveCard();

            card.Body.Add(new TextBlock
            {
                Text = "User Information",
                Weight = TextWeight.Bolder,
                Size = TextSize.Large
            });

            card.Body.Add(new TextBlock
            {
                Text = "Please fill up all the required details...",
                Speak = "<s>Please fill up all the required details</s>"
            });

            card.Body.Add(new TextBlock
            {
                Text = "Full Name",
                Speak = "<s>Enter your full name</s>"
            });

            card.Body.Add(new TextInput
            {
                Id = "fullName",
                Placeholder = "First name Last name",
                Style = TextInputStyle.Text
            });

            card.Body.Add(new TextBlock
            {
                Text = "Email Address",
                Speak = "<s>And your email address</s>"
            });

            card.Body.Add(new TextInput
            {
                Id = "emailAddress",
                Placeholder = "example@domain.com",
                Style = TextInputStyle.Email
            });

            card.Actions.Add(new SubmitAction
            {
                Title = "Submit"
            });

            return card;
        }

        private static AdaptiveCard CreateFactSetCard(string fullName, string emailAddress)
        {
            var card = new AdaptiveCard();

            card.Body.Add(new TextBlock
            {
                Size = TextSize.Medium,
                Weight = TextWeight.Bolder,
                Text = "Your Information"
            });

            card.Body.Add(new FactSet
            {
                Facts = new List<AdaptiveCards.Fact>
                {
                    new AdaptiveCards.Fact
                    {
                        Title = "Full Name:",
                        Value = fullName
                    },
                    new AdaptiveCards.Fact
                    {
                        Title = "Email Address:",
                        Value = emailAddress
                    }
                }
            });

            return card;
        }
    }
}