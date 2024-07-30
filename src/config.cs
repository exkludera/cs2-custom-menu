using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

public class MenuItem
{
    public string Title { get; set; } = "Menu";
    public string Type { get; set; } = "html";
    public string Command { get; set; } = "css_example";
    public string Permission { get; set; } = "";
    public string Team { get; set; } = "";

    public List<Options> Options { get; set; } = new();
}

public class Options
{
    public string Title { get; set; } = "Command";
    public string Command { get; set; } = "css_example";
    public string Permission { get; set; } = "";
    public string Team { get; set; } = "";

    public string Sound { get; set; } = "";
    public bool CloseMenu { get; set; } = false;
    public bool Confirm { get; set; } = false;
    public int Cooldown { get; set; } = 0;
}

public class Config : BasePluginConfig
{
    [JsonPropertyName("Prefix")]
    public string Prefix { get; set; } = "{green}[Menu]{default}";

    [JsonPropertyName("Messages")]
    public bool Messages { get; set; } = true;

    [JsonPropertyName("Menus")]
    public Dictionary<string, MenuItem> Menus { get; set; } = new()
    {
        {
            "1", new MenuItem
            {
                Title = "Public Menu",
                Type = "html",
                Command = "css_publicmenu",
                Options = new List<Options>
                {
                    new Options {
                        Title = "Example Command",
                        Command = "css"
                    },
                }
            }
        },
        {   
            "2", new MenuItem
            {
                Title = "VIP Menu",
                Type = "chat",
                Command = "css_vipmenu",
                Permission = "@css/reservation",
                Options = new List<Options>
                {
                    new Options {
                        Title = "VIP Command",
                        Command = "css_vip"
                    },
                }
            }
        },
        {
            "3", new MenuItem
            {
                Title = "Example Menu",
                Type = "wasd",
                Command = "css_examplemenu",
                Permission = "@css/root",
                Team = "terrorist",
                Options = new List<Options>
                {
                    new Options {
                        Title = "Example Command",
                        Command = "css_example",
                        Permission = "@css/root",
                        Team = "terrorist",
                        Sound = "sounds/buttons/blip1.vsnd",
                        CloseMenu = false,
                        Confirm = true,
                        Cooldown = 5
                    }
                }
            }
        },
    };
}