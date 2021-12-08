using System;
using System.Collections.Generic;
using System.Text;

namespace DGE
{
    public class ProjectInfo
    {
        public string[] DownloadLatestGet; //The url / website options from / with which we can download the latest update
        public string[] VersionLatestGet; //The url / website options from / with which we can fetch the latest version tag
        public string[] FetcherOptions; //The option to create the fetcher (ex : fetcher type, login info)
        public DGEVersion Version; //The Global/Project version

        public override string ToString() => Version.ToString();

    }
}
