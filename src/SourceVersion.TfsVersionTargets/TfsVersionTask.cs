using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SourceVersion.TfsVersionTargets
{
    public class TfsVersionTask : Task
    {
        [Required]
        public string SourceDirectory { get; set; }

        [Required]
        public string IntermediateOutputPath { get; set; }

        /// <summary>
        /// Specifies the version prefix to use to override the default value
        /// </summary>
        public string VersionFormat { get; set; }

        /// <summary>
        /// Specifies the suffix to use for the AssemblyInformationalVersion attribute.
        /// </summary>
        public string AssemblyInformationalFormat { get; set; }

        [Output]
        public string AssemblyInfoTempFilePath { get; set; }

        [Output]
        public string VersionJsonFilePath { get; set; }

        public override bool Execute()
        {
            this.Log.LogMessage(MessageImportance.High, "TfsVersionTask - IntermediateOutputPath {0}", IntermediateOutputPath);

            this.Log.LogMessage(MessageImportance.High, "Executing TfsVersionTask");
            var taskDirectory = GetAssemblyDirectory();

            //ResolveEventHandler handler = (sender, e) =>
            //{
            //    this.Log.LogMessage(MessageImportance.High, "Attempting to resolve {0}", e.Name);
            //    var assemblySearchPath = Path.Combine(taskDirectory, e.Name.Split(',')[0]) + ".dll";
            //    this.Log.LogMessage(MessageImportance.High, "Attempting to load {0}", assemblySearchPath);
            //    return (File.Exists(assemblySearchPath)) ? 
            //        Assembly.LoadFrom(assemblySearchPath) :
            //        null;
            //};
            //AppDomain.CurrentDomain.AssemblyResolve += handler;

            //this.Log.LogCommandLine("Hmmm, here is a command line");
            //this.Log.LogCommandLine(MessageImportance.High, "High priority message!");
            //var projectPath = Path.GetDirectoryName(BuildEngine.ProjectFileOfTaskNode);



            var watermark = Functions.GetWorkspaceWatermark(Path.GetFullPath(SourceDirectory));
            this.Log.LogMessage(MessageImportance.High, "Most recent changeset is {0}", watermark.MostRecentChangeset);
            //AppDomain.CurrentDomain.AssemblyResolve -= handler;

            //if (watermark)
            {
                this.Log.LogMessage(MessageImportance.High, "Generating AssemblyInfo content");

                string versionFormat = VersionFormat ?? "1.0.0.{changeset}";
                string assemblyInformationalFormat = 
                    AssemblyInformationalFormat ?? 
                    "ServerPath.{server-path}.Changeset.{changeset}.{clean}";

                var generatedAssemblyInfoText = GenerateAssemblyInfo(watermark, versionFormat, assemblyInformationalFormat);
                var generatedAssemblyInfoPath = Path.Combine(IntermediateOutputPath, string.Format("TfsVersionTaskAssemblyInfo.g.{0}", "cs"));
                File.WriteAllText(generatedAssemblyInfoPath, generatedAssemblyInfoText, Encoding.UTF8);
                AssemblyInfoTempFilePath = generatedAssemblyInfoPath;
                this.Log.LogMessage(MessageImportance.High, "Generated temporary AssemblyInfo file {0}", AssemblyInfoTempFilePath);
            }

            var versionJsonContent = CreateVersionJsonString(watermark);
            var versionJsonPath = Path.Combine(SourceDirectory, "version.json");
            File.WriteAllText(versionJsonPath, versionJsonContent, Encoding.UTF8);
            VersionJsonFilePath = versionJsonPath;

            return true;
        }


        private string GetAssemblyDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        private string GenerateAssemblyInfo(
            WorkspaceWatermark info,
            string versionFormat, 
            string assemblyInformationalFormat)
        {
            var changeset = info.MostRecentChangeset.ToString();
            var version = versionFormat.Replace("{changeset}", changeset);
            var assemblyInformationalVersionSuffix = assemblyInformationalFormat
                .Replace("{changeset}", changeset)
                .Replace("{server-path}", info.ServerPath)
                .Replace("{clean}", info.PendingChanges ? "Dirty" : "Clean");

            return
$@"
using System.Reflection;
[assembly: AssemblyVersion(""{version}"")]
[assembly: AssemblyInformationalVersion(""{version}+{assemblyInformationalVersionSuffix}"")]
";
        }

        private string CreateVersionJsonString(WorkspaceWatermark info)
        {
            var json = new JObject(
                new JProperty("changeset", info.MostRecentChangeset),
                new JProperty("serverPath", info.ServerPath),
                new JProperty("localPath", info.LocalPath),
                new JProperty("machine", info.Machine),
                new JProperty("user", info.User),
                new JProperty("hasPendingChanges", info.PendingChanges));

            return json.ToString();
        }
    }
}
