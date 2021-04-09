using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DiscordGameEngine.UI.Feedback
{
    public struct FeedbackInfo
    {
        public string infoSource;
        public string summary;
        public string description;
        public string[] additionalInfo;
        public readonly DateTime date;
        public readonly string reportId; // Report Ids are generated to be able to have duplicates,

        public FeedbackInfo(string infoSource, string summary, string description, string[] additionalInfo, params string[] identifyingInfo)
        {
            this.infoSource = infoSource;
            this.summary = summary;
            this.description = description;
            this.additionalInfo = additionalInfo;
            date = DateTime.Now;

            //Getting an id //TODO: Reduce generated Id size, make it better more specific and all, but too lazy right now and anyway im stpuid
            StringBuilder tempId = new StringBuilder();
            int value = -42;
            string[] words = identifyingInfo is null ? infoSource.Split(new char[2] { ' ', ',' }).Concat(summary.Split(new char[2] { ' ', ',' })).ToArray() : string.Join(' ', identifyingInfo).Split(' ');
            foreach (string word in words)
            {
                tempId.Append($"{(0 + (word.Length > 0 ? word[0] : '_'))}{word.Sum(c => c + value)}");
                value = value + 1;
            }
            reportId = tempId.ToString();
            tempId.Clear();

            DiscordGameEngineBot.DGELogger.Log($"FeedbackReportId : {reportId}", EnderEngine.Logger.LogLevel.DEBUG);
        }

        public override string ToString()
        {
            return $"{infoSource} (id = {reportId}) :" +
                $"\n\n{summary}" +
                $"\n\n    {description}" +
                $"\n\n {(additionalInfo.Length > 0 ? $"AdditionalInfo :\n- {string.Join("\n- ", additionalInfo)}" : "")}" +
                $"\n\n|  filled in at : {date}";
        }

    }
}