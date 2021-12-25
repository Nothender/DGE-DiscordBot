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

            //Changing credentials
            if (args.Length == 1) //Connection token
                client.Credentials = new Credentials(args[0]);
            else if (args.Length == 2) //Username, and password
                client.Credentials = new Credentials(args[0], args[1]);

            return Task.CompletedTask;
        }

        public async Task<string> FetchLatestVersion(params string[] args)
        {
            if (args.Length < 2) throw new Exception("FetchLatestVersion needs at least 2 arguments (Owner, Repository)");
            
            Release release = await client.Repository.Release.GetLatest(args[0], args[1]);
            
            return $"{release.TagName}";
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
                // Gets the latest release
                Release latestRelease = await client.Repository.Release.GetLatest(owner, repository);
                  
                int assetId = latestRelease.Assets[assetIndex].Id;
                string downloadUrl = $"https://api.github.com/repos/{owner}/{repository}/releases/assets/{assetId}";

                // Download with WebClient
                using var webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.Authorization, $"token {client.Credentials.GetToken()}");
                webClient.Headers.Add(HttpRequestHeader.Accept, "application/octet-stream");

                // Download the file
                webClient.DownloadFileAsync(new Uri(downloadUrl), Paths.Get("Downloads"));
            }
            catch
            {
                AssemblyUpdater.logger.Log("Downloading failed", EnderEngine.Logger.LogLevel.ERROR);
                //idk what to do there
            }

        }

    }
}
