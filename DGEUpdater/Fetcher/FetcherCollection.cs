using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DGE
{
    public static class FetcherCollection
    {

        private static Dictionary<string, IFetcher> fetchers = new Dictionary<string, IFetcher>();

        /// <summary>
        /// Inits and create a fetcher with the specified arguments, doesn't if a fetcher of that type already exists
        /// </summary>
        /// <param name="arguments">FetcherType, args0, args1, ...</param>
        public static void InitFetcher(params string[] arguments)
        {
            string[] versionFetcherArgs = arguments.Skip(1).ToArray();

            switch (arguments[0])
            {
                case "github":
                    if (!fetchers.ContainsKey("github")) fetchers.Add("github", new GithubFetcher(versionFetcherArgs));
                    fetchers["github"].ChangeOptions(versionFetcherArgs);
                    break;
                default:
                    throw new Exception($"No fetcher of type {arguments[0]} exists.");
            }
        }

        public static string FetchLatestVersion(params string[] arguments)
        {
            if (!fetchers.ContainsKey(arguments[0])) throw new Exception($"No fetcher of type {arguments[0]} was instantiated");

            string[] versionFetcherArgs = arguments.Skip(1).ToArray();

            return fetchers[arguments[0]].FetchLatestVersion(versionFetcherArgs).Result;
        }

        public static void DownloadLatestVersion(params string[] arguments)
        {
            if (!fetchers.ContainsKey(arguments[0])) throw new Exception($"No fetcher of type {arguments[0]} was instantiated");

            string[] deleteFetcherArgs = arguments.Skip(1).ToArray();

            fetchers[arguments[0]].DownloadLatestRelease(deleteFetcherArgs).Wait();

        }

    }
}
