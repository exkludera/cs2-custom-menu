using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Entities;

namespace CustomMenu;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "Custom Menu";
    public override string ModuleVersion => "1.0.6";
    public override string ModuleAuthor => "exkludera";

    public static Plugin _ { get; set; } = new();
    public static Dictionary<string, string> commandMenuId = new Dictionary<string, string>();

    public override void Load(bool hotReload)
    {
        _ = this;

        foreach (var menu in Config.Menus)
        {
            var commands = menu.Value.Command.ToLower();

            var menuId = menu.Key;

            foreach (var command in commands.Split(','))
            {
                commandMenuId[command.Trim()] = menuId;
                AddCommand(command.Trim(), menu.Value.Title, Menu.Command_OpenMenus!);
            }
        }

        Menu.Load(hotReload);
    }

    public override void Unload(bool hotReload)
    {
        commandMenuId.Clear();

        foreach (var menu in Config.Menus)
        {
            var commands = menu.Value.Command.ToLower();

            foreach (var command in commands.Split(','))
                RemoveCommand(command.Trim(), Menu.Command_OpenMenus!);
        }

        Menu.Unload();
    }

    public Config Config { get; set; } = new Config();
    public void OnConfigParsed(Config config) {
        Config = config;
        Config.Prefix = StringExtensions.ReplaceColorTags(Config.Prefix);
    }
}
