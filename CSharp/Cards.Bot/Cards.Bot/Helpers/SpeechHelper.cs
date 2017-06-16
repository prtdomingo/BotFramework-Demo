using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Cards.Bot.Helpers
{
    public static class SpeechHelper
    {
        public static string Speak(string text)
        {
            return $"<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\">{text}</speak>";
        }
    }
}