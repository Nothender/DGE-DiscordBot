using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using DGE.Core;
using Discord.WebSocket;

namespace DGE.Misc.BetaTesting
{
    public static class BetaTestingPointsCounter
    {
        private static string filename = "BetaTestingUserPoints.txt";
        private static string filepath = Paths.Get("SaveData") + filename;

        public static int GetUserPoints(ulong userId)
        { //When a user doens't exist we assume it has 0 points
            string id = userId.ToString();
            if (!File.Exists(filepath))
                return 0;
            foreach(string line in File.ReadAllLines(filepath))
            {
                if (line.StartsWith(id))
                    return int.Parse(line.Split('|')[1]);
            }
            return 0; //If user was not found
        }

        public static List<UserPointsPair> GetUsersPoints(DiscordSocketClient client) //TODO: Not very clean having the client like that
        {
            List<UserPointsPair> users = new List<UserPointsPair>(10);

            if (!File.Exists(filepath))
                return users;

            foreach (string line in File.ReadAllLines(filepath))
            {
                string[] lineSplit = line.Split('|');
                users.Add(new UserPointsPair(ulong.Parse(lineSplit[0]), int.Parse(lineSplit[1]), client));
            }
            return users;
        }

        public static void GivePointsToUser(ulong userId, int points = 1)
        {

            string id = userId.ToString();

            if (File.Exists(filepath))
            {
                string[] fileLines = File.ReadAllLines(filepath);

                for (int i = 0; i < fileLines.Length; i++)
                {
                    string line = fileLines[i];
                    if (line.StartsWith(id))
                    {
                        string[] userPoints = line.Split('|');
                        userPoints[1] = (int.Parse(userPoints[1]) + points).ToString();
                        fileLines[i] = string.Join('|', userPoints);
                        File.WriteAllLines(filepath, fileLines);
                        return;
                    }
                }
            }

            File.AppendAllLines(filepath, new string[1] { $"{id}|{points}" });

        }

        /// <summary>
        /// Resets points for the specified user of userId
        /// </summary>
        public static void ResetPoints(ulong userId)
        {
            if (!File.Exists(filepath))
                return;
            string id = userId.ToString();
            File.WriteAllLines(filepath, File.ReadAllLines(filepath).Where(l => !l.StartsWith(id)).ToArray());
        }

        /// <summary>
        /// Resets points for every user
        /// </summary>
        public static void ResetPoints()
        {
            File.Delete(filepath);
        }

    }
}
