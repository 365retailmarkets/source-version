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
            var versionJsonContent = File.ReadAllText(context.Server.MapPath("~/version.json"));

            context.Response.ContentType = "application/json";
            context.Response.Write(versionJsonContent);
        }
    }
}
