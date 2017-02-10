using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.IO;

namespace SourceVersion.TfsVersionTargets
{
    static class Functions
    {
        public static Func<string, int?> GetMostRecentChangeset = (directory) =>
        {
            // The workspace info for the provided path
            WorkspaceInfo wsInfo = Workstation.Current.GetLocalWorkspaceInfo(directory);

            // Get the TeamProjectCollection and VersionControl server associated with the
            // WorkspaceInfo
            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(wsInfo.ServerUri);
            VersionControlServer vcServer = tpc.GetService<VersionControlServer>();

            // Now get the actual Workspace OM object
            Workspace ws = vcServer.GetWorkspace(wsInfo);

            // We are interested in the current version of the workspace
            VersionSpec versionSpec = new WorkspaceVersionSpec(ws);

            var mostRecentChangesetNumber = ws
                .GetLocalVersions(new[] { new ItemSpec(directory, RecursionType.Full) }, false)
                .FirstOrDefault() // Get most recent history item
                ?.Max(i => (int?)i.Version);

            //var changeset = ws.VersionControlServer.GetChangeset(mostRecentChangesetNumber, false, false);

            //return changeset;

            return mostRecentChangesetNumber;
        };

        public static Func<string, WorkspaceWatermark> GetWorkspaceWatermark = (directory) =>
        {
            // The workspace info for the provided path
            WorkspaceInfo wsInfo = Workstation.Current.GetLocalWorkspaceInfo(directory);

            // Get the TeamProjectCollection and VersionControl server associated with the
            // WorkspaceInfo
            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(wsInfo.ServerUri);
            VersionControlServer vcServer = tpc.GetService<VersionControlServer>();

            // Now get the actual Workspace OM object
            Workspace ws = vcServer.GetWorkspace(wsInfo);

            // We are interested in the current version of the workspace
            VersionSpec versionSpec = new WorkspaceVersionSpec(ws);

            var mostRecentChangesetNumber = ws
                .GetLocalVersions(new[] { new ItemSpec(directory, RecursionType.Full) }, false)
                .FirstOrDefault() // Get most recent history item
                ?.Max(i => (int?)i.Version);

            //var changeset = ws.VersionControlServer.GetChangeset(mostRecentChangesetNumber, false, false);

            //return changeset;

            //return mostRecentChangesetNumber;

            var serverPath = ws.GetServerItemForLocalItem(directory);
            var pending = ws.GetPendingChanges();
            var owner = ws.OwnerName;

            return new WorkspaceWatermark()
            {
                MostRecentChangeset = mostRecentChangesetNumber,
                ServerPath = serverPath,
                LocalPath = directory,
                User = owner,
                Machine = Environment.MachineName,
                PendingChanges = pending.Any(),
            };
        };
    }

    public class WorkspaceWatermark
    {
        public int? MostRecentChangeset { get; set; }
        public string ServerPath { get; set; }
        public string LocalPath { get; set; }
        public bool PendingChanges { get; set; }
        public string User { get; set; }
        public string Machine { get; set; }

        //public string[] PendingChanges { get; set; }
    }

    //public class PendingChange
    //{
    //    public string Path { get; set; }
       
    //}
}
