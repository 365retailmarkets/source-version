using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SourceVersion.WebForms
{
    public class Version : IHttpHandler
    {
        public bool IsReusable => true;

        public void ProcessRequest(HttpContext context)
        {
            var versionText = File.ReadAllText(context.Server.MapPath("~/version.txt"));

            if (String.Equals(context.Request.ContentType, "text/plain"))
            {
                context.Response.ContentType = context.Request.ContentType;
                context.Response.Write(versionText);
                return;
            }

            var regexPattern =
@"Local information:
  Local path : (?<localInfo_localPath>.*)
  Server path: (?<localInfo_serverPath>.*)
  Changeset  : (?<localInfo_changeset>.*)
  Change     : (?<localInfo_change>.*)
  Type       : (?<localInfo_type>.*)
Server information:
  Server path  : (?<serverInfo_serverPath>.*)
  Changeset    : (?<serverInfo_changeset>.*)
  Deletion ID  : (?<serverInfo_deletionId>.*)
  Lock         : (?<serverInfo_lock>.*)
  Lock owner   : (?<serverInfo_lockOwner>.*)
  Last modified: (?<serverInfo_lastModified>.*)
  Type         : (?<serverInfo_type>.*)";

            var infoRegex = new Regex(regexPattern, RegexOptions.Multiline);
            var match = infoRegex.Match(versionText);

            var json = new JObject();

            var names = infoRegex.GetGroupNames();

            names.
                Skip(1). // HACK: Skip whole pattern capture
                Select(name => name.Split('_')).
                GroupBy(nameParts => nameParts[0], nameParts => nameParts[1]).
                ToList().
                ForEach(g =>
                {
                    var info = new JObject();

                    foreach (var fieldName in g)
                    {
                        info.Add(fieldName, match.Groups[$"{g.Key}_{fieldName}"].Value);
                    }

                    json.Add(g.Key, info);
                });

            context.Response.ContentType = "application/json";
            context.Response.Write(json.ToString());
        }
    }
}
