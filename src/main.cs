using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;

namespace CustomMenu;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "Custom Menu";
    public override string ModuleVersion => "1.0.2";
    public override string ModuleAuthor => "exkludera";

    public static Plugin _ { get; set; } = new();
    public static Dictionary<string, string> commandMenuId = new Dictionary<string, string>();

    public override void Load(bool hotReload)
    {
        _ = this;

        foreach (var menu in Config.Menus)
        {
            var command = menu.Value.Command.ToLower();

            var menuId = menu.Key;
            commandMenuId[command] = menuId;

            AddCommand(command, menu.Value.Title, Menu.Command_OpenMenus!);
        }

        Menu.Load(hotReload);
    }

    public override void Unload(bool hotReload)
    {
        commandMenuId.Clear();

        foreach (var menu in Config.Menus)
        {
            var command = menu.Value.Command.ToLower();
            RemoveCommand(command, Menu.Command_OpenMenus!);
        }

        Menu.Unload();
    }

    public Config Config { get; set; } = new Config();
    public void OnConfigParsed(Config config) {
        Config = config;
        Config.Prefix = StringExtensions.ReplaceColorTags(Config.Prefix);
    }
}
