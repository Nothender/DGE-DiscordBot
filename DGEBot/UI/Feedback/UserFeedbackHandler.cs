using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Discord.WebSocket;
using EnderEngine;
using System.Linq;
using Discord;
using DGE.Core;
using DGE.Bot;

namespace DGE.UI.Feedback
{
    public static class UserFeedbackHandler
    {
        private static string filename = "FeedbackReports.txt";
        private static string reportIdsFilename = "FeedbackReports-LinesIds.txt"; //Where the line and corresponding report id's are found
        private static string filepath = Paths.PathToStorage + filename;
        private static string reportIdsFilepath = Paths.PathToStorage + reportIdsFilename;

        private static int linesCounter;

        static UserFeedbackHandler()
        {
            if (File.Exists(filepath))
                linesCounter = File.ReadAllLines(filepath).Length + 1;
            else
                linesCounter = 1;
        }

        /// <summary>
        /// Sends the feedback info as a report (into the channels and saves it), returns true if it was already sent
        /// </summary>
        /// <param name="feedbackInfo"></param>
        /// <returns></returns>
        public static bool SendFeedback(FeedbackInfo feedbackInfo, IMessageChannel feedbackChannel)
        {
            if (File.Exists(reportIdsFilepath) && File.ReadAllLines(reportIdsFilepath).Any(s => s.StartsWith(feedbackInfo.reportId)))
            {
                AssemblyBot.logger.Log($"Report {feedbackInfo.reportId} was skipped (already sent)", Logger.LogLevel.INFO);
                return true;
            }

            string feedbackMessage = feedbackInfo.ToString();

            IMessage feedbackDiscordMessage = feedbackChannel.SendMessageAsync("\n################\n\n" + feedbackMessage + "\n\n################\n").Result;
            File.AppendAllText(filepath, feedbackMessage += '\n');

            int linesEnd = linesCounter + feedbackMessage.Split('\n').Length - 1;
            
            using (FileStream stream = File.Open(reportIdsFilepath, FileMode.Append)) 
            {
                stream.Write(new UTF8Encoding().GetBytes($"{feedbackInfo.reportId}|{linesCounter}|{linesEnd}|{feedbackDiscordMessage.Id}\n")); //Id|LineBegin|LineEnd|MessageId
            }

            linesCounter = linesEnd;

            AssemblyBot.logger.Log($"new FeedbackReport was created (id: {feedbackInfo.reportId}, BeginLineIndex: {linesCounter} EndLineIndex: {linesEnd})", Logger.LogLevel.INFO);
            return false;
        }

        public static void ClearReports(IMessageChannel feedbackChannel)
        {
            //Resetting everything
            int reportsCount = 42; //if we don't know how many reports were created
            if (File.Exists(reportIdsFilepath))
            {
                reportsCount = File.ReadAllLines(reportIdsFilepath).Length;
                File.Delete(reportIdsFilepath);
            }
            File.Delete(filepath);
            (feedbackChannel as SocketTextChannel).DeleteMessagesAsync(feedbackChannel.GetMessagesAsync(reportsCount).FlattenAsync().Result);
            linesCounter = 1;
        }

        public static void DeleteReport(string reportId, IMessageChannel feedbackChannel)
        {
            //command is slow, and stupidly made... BUT IT WORKS

            //Checking for errors or problems that could happen when trying to clear the report
            bool reportsFileExists = File.Exists(filepath); //TODO: Finish
            bool reportsIdsInfoFileExists = File.Exists(reportIdsFilepath);
            bool messagesInReportsChannel = feedbackChannel.GetMessagesAsync(1).FlattenAsync().Result.Count() > 0;

            if (!reportsIdsInfoFileExists) //If we don't have info on the reports
            {
                if (reportsFileExists || messagesInReportsChannel)
                {
                    throw new Exception("Manual report deleting/clearing may be needed (The reports file or reports channel is not empty/cleared)");
                }
                else if (!reportsFileExists && !messagesInReportsChannel)
                    return; //Everything was deleted
            }

            //Getting report info
            bool reportInfoFound = false;
            int reportInfoLinesCount = 0;
            string[] reportInfo = null;
            string[] reportsIdsInfos = File.ReadAllLines(reportIdsFilepath);
            for (int i = 0; i < reportsIdsInfos.Length; i++)
            {
                string reportIdInfo = reportsIdsInfos[i];
                if (reportInfoFound)
                {
                    string[] reportInfoToOffset = reportIdInfo.Split('|');
                    reportInfoToOffset[1] = $"{ int.Parse(reportInfoToOffset[1]) - reportInfoLinesCount }";
                    reportInfoToOffset[2] = $"{ int.Parse(reportInfoToOffset[2]) - reportInfoLinesCount }";
                    reportsIdsInfos[i] = string.Join('|', reportInfoToOffset);
                }
                else if (reportIdInfo.StartsWith(reportId))
                {
                    reportInfoFound = true;
                    reportInfo = reportIdInfo.Split('|');
                    reportInfoLinesCount = int.Parse(reportInfo[2]) - int.Parse(reportInfo[1]);
                }

            }
            if (reportInfo is null) //Return if we didn't find any info on the feedback report
                throw new KeyNotFoundException($"Couldn't delete report of id {reportId} because it does not exist or was not found"); //Manual deleting as well may be needed

            feedbackChannel.DeleteMessageAsync(ulong.Parse(reportInfo[3])); //Delete the associated discord message

            File.WriteAllLines(reportIdsFilepath, reportsIdsInfos.Where(l => !l.StartsWith(reportId)).ToArray()); //Removing reportInfo from the reportsIdsInfosFile
            linesCounter -= reportInfoLinesCount; //Updating the counter

            if (reportsFileExists)
            {
                File.WriteAllLines(filepath, File.ReadLines(filepath).Select(
                    (l, i) => {
                        i += 1;
                        if (i >= int.Parse(reportInfo[1]) && i < int.Parse(reportInfo[2]))
                            return "[DGE-FRL]REMOVE_LINE";
                        return l;
                    }).Where(l => l != "[DGE-FRL]REMOVE_LINE").ToList());

            }

        }

    }
}
