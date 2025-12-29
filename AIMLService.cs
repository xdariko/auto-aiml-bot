using AIMLbot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AIMLTGBot
{
    public class AIMLService
    {
        private readonly Bot bot;
        private readonly Dictionary<long, User> users = new Dictionary<long, User>();

        private static readonly CultureInfo RuCulture = new CultureInfo("ru-RU");

        public AIMLService()
        {
            bot = new Bot();
            bot.loadSettings();
            bot.isAcceptingUserInput = false;
            bot.loadAIMLFromFiles();
            bot.isAcceptingUserInput = true;
        }

        private static string NormalizeForAiml(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var s = input.Trim();
            s = s.Normalize(NormalizationForm.FormD);
            s = Regex.Replace(s, @"\p{Mn}+", "");
            s = s.Normalize(NormalizationForm.FormC);
            s = s.Replace('ё', 'е').Replace('Ё', 'Е');
            s = Regex.Replace(s, @"[\p{P}\p{S}]+", " ");
            s = Regex.Replace(s, @"\s+", " ").Trim();
            s = s.ToUpper(RuCulture);

            return s;
        }

        public string Talk(long userId, string userName, string phrase)
        {
            var result = "";
            User user;

            var normalizedPhrase = NormalizeForAiml(phrase);

            if (!users.TryGetValue(userId, out user))
            {
                user = new User(userId.ToString(), bot);
                users.Add(userId, user);

                var normalizedName = NormalizeForAiml(userName);
                var r = new Request($"МЕНЯ ЗОВУТ {normalizedName}", user, bot);
                result += bot.Chat(r).Output + Environment.NewLine;
            }

            result += bot.Chat(new Request(normalizedPhrase, user, bot)).Output;
            return result;
        }
    }
}
