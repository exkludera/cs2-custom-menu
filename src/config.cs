using CounterStrikeSharp.API.Core;

public class MenuItem
{
    public string Title { get; set; } = "Menu";
    public string Type { get; set; } = "html";
    public string Command { get; set; } = "";
    public string Permission { get; set; } = "";
    public string Team { get; set; } = "";

    public List<Options> Options { get; set; } = new();
}

public class Options
{
    public string Title { get; set; } = "Command";
    public string Command { get; set; } = "";
    public string Permission { get; set; } = "";
    public string Team { get; set; } = "";

    public string Sound { get; set; } = "";
    public bool CloseMenu { get; set; } = false;
    public bool Confirm { get; set; } = false;
    public int Cooldown { get; set; } = 0;
}

public class Config : BasePluginConfig
{
    public string Prefix { get; set; } = "{green}[Menu]{default}";
    public bool Messages { get; set; } = true;
    public Dictionary<string, MenuItem> Menus { get; set; } = new();
}