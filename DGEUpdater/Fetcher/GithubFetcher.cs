using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using System.Linq;

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
            
            Release release = (await client.Repository.Release.GetAll(args[0], args[1])).ToArray()[0];
            return $"{release.TagName}";
        }

        public async Task DownloadLatestRelease(params string[] args)
        {

            if (args.Length < 3) throw new Exception("FetchLatestVersion needs at least 3 arguments (Owner, Repository, Asset index)");



        }

    }
}
