using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;
using static CounterStrikeSharp.API.Core.Listeners;

namespace CustomMenu;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "Custom Menu";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "exkludera";

    public static Plugin _ { get; set; } = new();
    public static Dictionary<string, string> commandMenuId = new Dictionary<string, string>();

    public override void Load(bool hotReload)
    {
        _ = this;

        RegisterListener<OnTick>(Menu.OnTick);

        Menu.Load(hotReload);

        foreach (var menu in Config.Menus)
        {
            var command = menu.Value.Command;
            var menuId = menu.Key;
            commandMenuId[command] = menuId;

            AddCommand(command, menu.Value.Title, Menu.Command_OpenMenus!);
        }
    }

    public override void Unload(bool hotReload)
    {
        RemoveListener<OnTick>(Menu.OnTick);

        foreach (var menu in Config.Menus)
        {
            var command = menu.Value.Command;
            var menuId = menu.Key;
            commandMenuId[command] = menuId;

            RemoveCommand(command, Menu.Command_OpenMenus!);
        }
    }

    public Config Config { get; set; } = new Config();
    public void OnConfigParsed(Config config) {
        Config = config;
        Config.Prefix = StringExtensions.ReplaceColorTags(Config.Prefix);
    }
}
