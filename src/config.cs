using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

public class MenuItem
{
    public string Title { get; set; } = "Menu Title";
    public string Command { get; set; } = "css_example";
    public string Permission { get; set; } = "";
    public string Team { get; set; } = "";
    public List<Options> Options { get; set; } = new();
}

public class Options
{
    public string Title { get; set; } = "Command Title";
    public string Command { get; set; } = "css_example";
    public string Permission { get; set; } = "";
    public string Team { get; set; } = "";
}

public class Config : BasePluginConfig
{
    [JsonPropertyName("Prefix")]
    public string Prefix { get; set; } = "{green}[Menu]{default}";

    [JsonPropertyName("Messages")]
    public bool Messages { get; set; } = true;

    [JsonPropertyName("MenuStyle")]
    public string MenuStyle { get; set; } = "html";

    [JsonPropertyName("Menus")]
    public Dictionary<string, MenuItem> Menus { get; set; } = new()
    {
        {
            "1", new MenuItem
            {
                Title = "Public Menu",
                Command = "css_publicmenu",
                Options = new List<Options>
                {
                    new Options { Title = "Example Command", Command = "css" },
                }
            }
        },
        {
            "2", new MenuItem
            {
                Title = "VIP Menu",
                Command = "css_vipmenu",
                Permission = "@css/reservation",
                Options = new List<Options>
                {
                    new Options { Title = "VIP Command", Command = "css_vip" },
                }
            }
        },
        {
            "3", new MenuItem
            {
                Title = "Example Menu",
                Command = "css_examplemenu",
                Permission = "@css/root",
                Team = "terrorist",
                Options = new List<Options>
                {
                    new Options { Title = "Example Command", Command = "css_example", Permission = "@css/root", Team = "terrorist" }
                }
            }
        },
    };
}