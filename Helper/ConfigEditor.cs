using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIRBot.Helper
{
    public static class ConfigEditor
    {
        private static readonly string configName = "appsettings.json";

        public static void UpdateReloadDate(IConfiguration config)
        {
            var json = File.ReadAllText(configName);

            // Parse content into JObject
            var jObject = JObject.Parse(json);

            // Update the value of the LastReloadDate field in the file and at runtime
            jObject["LastReloadDate"] = DateTime.Now.ToString("yyyy-MM-dd");
            config["LastReloadDate"] = DateTime.Now.ToString("yyyy-MM-dd");

            // Write back to file
            File.WriteAllText(configName, jObject.ToString());
        }
    }
}
