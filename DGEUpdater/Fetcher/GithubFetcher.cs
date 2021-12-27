using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using System.Linq;
using DGE.Core;

namespace DGE
{
    public class GithubFetcher : IFetcher
    {

        private GitHubClient client;

        public GithubFetcher(params string[] args)
        {
            ChangeOptions(args);
        }

        public Task ChangeOptions(params string[] args)
        {
            if(client is null) client = new GitHubClient(new ProductHeaderValue("DGE-AutoUpdater"));

            // Changing credentials
            if (args.Length == 1) // Connection token
                client.Credentials = new Credentials(args[0]);
            else if (args.Length == 2) // Username, and password
                client.Credentials = new Credentials(args[0], args[1]);

            return Task.CompletedTask;
        }

        private async Task<Release> GetLatestRelease(string owner, string repository)
        {
            return (await client.Repository.Release.GetAll(owner, repository)).ToArray()[0];
        }

        public async Task<string> FetchLatestVersion(params string[] args)
        {
            if (args.Length != 2) throw new Exception("FetchLatestVersion needs 2 arguments (Owner, Repository)");

            return $"{(await GetLatestRelease(args[0], args[1])).TagName}";
        }

        public async Task DownloadLatestRelease(params string[] args)
        {
            if (args.Length < 2) throw new Exception("DownloadLatestRelease needs at least 2 arguments (and a 3rd optional one) (Owner, Repository, AssetIndex = 0)");

            string owner = args[0], repository = args[1];

            int assetIndex = 0;
            if (args.Length > 2)
            {
                if (int.TryParse(args[2], out int res))
                    assetIndex = res;
                
            }

            try
            {
                Release latestRelease = await GetLatestRelease(args[0], args[1]);
                  
                if (assetIndex < 0)
                {
                    assetIndex = latestRelease.Assets.Count - assetIndex; //Taking the n - assetIndex element
                    if (assetIndex < 0) assetIndex = 0; //If the value is out of bounds we clamp it to 0
                }
                ReleaseAsset asset = latestRelease.Assets[assetIndex];
                string downloadURL = asset.BrowserDownloadUrl;

                // Download with WebClient
                using var webClient = new WebClient();

                string token = client.Credentials.GetToken();
                if (!string.IsNullOrEmpty(token))
                    webClient.Headers.Add(HttpRequestHeader.Authorization, $"token {token}"); // If the client has a token we use it to download the asset

                webClient.Headers.Add(HttpRequestHeader.Accept, "application/octet-stream");

                // Download the file
                webClient.DownloadFile(downloadURL, $"{Paths.Get("Downloads")}{asset.Name}");
            }
            catch
            {
                AssemblyUpdater.logger.Log("Downloading failed", EnderEngine.Logger.LogLevel.ERROR);
                //idk what to do there
            }

        }

    }
}
