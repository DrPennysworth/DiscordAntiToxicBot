using IniParser;
using IniParser.Model;

namespace DiscordAntiToxicBot
{
    public static class Config
    {
        public static IniData ConfigFile;

        static Config()
        {
            // Setup the ini config
            var parser = new FileIniDataParser();
            ConfigFile = parser.ReadFile("cfg.ini");
        }       
    }
}
