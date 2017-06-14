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
    public class StandardCardDialog : IDialog<object>
    {
        private const string SigninCard = "Sign-in card";
        private const string AnimationCard = "Animation card";
        private const string VideoCard = "Video card";
        private IEnumerable<string> choiceOptions = new List<string> { SigninCard, AnimationCard, VideoCard };

        public async Task StartAsync(IDialogContext context)
        {
            PromptDialog.Choice(context,
                DisplaySelectedCard,
                choiceOptions,
                "Select any of the available standard cards",
                "The option you've selected is not valid, try again!",
                3,
                PromptStyle.Auto);
        }

        public async Task DisplaySelectedCard(IDialogContext context, IAwaitable<string> result)
        {
            await context.PostAsync("Hello");
            context.Done(new object());
        }
    }
}