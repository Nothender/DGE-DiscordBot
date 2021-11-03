using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DGE
{
    public interface IFetcher
    {

        public Task<string> FetchLatestVersion(params string[] args);

        /// <summary>
        /// Changes options for connection / website
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Task ChangeOptions(params string[] args);

        /// <summary>
        /// Downloads the latest release on the disk in the download folder
        /// </summary>
        /// <param name="args">Where/How you download the latest release</param>
        public Task DownloadLatestRelease(params string[] args);

    }
}
