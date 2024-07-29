using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using static CounterStrikeSharp.API.Core.Listeners;
using static CustomMenu.Plugin;

namespace CustomMenu;

public static class Menu
{
    public static readonly Dictionary<int, WasdMenuPlayer> Players = [];

    public static void Load(bool hotReload)
    {
        _.RegisterListener<OnTick>(OnTick);

        _.RegisterEventHandler<EventPlayerActivate>((@event, info) =>
        {
            CCSPlayerController? player = @event.Userid;

            if (player == null || !player.IsValid || player.IsBot)
                return HookResult.Continue;

            Players[player.Slot] = new WasdMenuPlayer
            {
                player = player,
                Buttons = 0
            };

            return HookResult.Continue;
        });

        _.RegisterEventHandler<EventPlayerDisconnect>((@event, info) =>
        {
            CCSPlayerController? player = @event.Userid;

            if (player == null || !player.IsValid || player.IsBot)
                return HookResult.Continue;

            Players.Remove(player.Slot);

            return HookResult.Continue;
        });

        if (hotReload)
        {
            foreach (CCSPlayerController player in Utilities.GetPlayers())
            {
                if (player.IsBot)
                    continue;

                Players[player.Slot] = new WasdMenuPlayer
                {
                    player = player,
                    Buttons = player.Buttons
                };
            }
        }
    }

    public static void Unload()
    {
        _.RemoveListener<OnTick>(OnTick);
    }

    public static void OnTick()
    {
        foreach (WasdMenuPlayer? player in Players.Values.Where(p => p.MainMenu != null))
        {
            if ((player.Buttons & PlayerButtons.Forward) == 0 && (player.player.Buttons & PlayerButtons.Forward) != 0)
            {
                player.ScrollUp();
            }
            else if ((player.Buttons & PlayerButtons.Back) == 0 && (player.player.Buttons & PlayerButtons.Back) != 0)
            {
                player.ScrollDown();
            }
            else if ((player.Buttons & PlayerButtons.Moveright) == 0 && (player.player.Buttons & PlayerButtons.Moveright) != 0)
            {
                player.Choose();
            }
            else if ((player.Buttons & PlayerButtons.Moveleft) == 0 && (player.player.Buttons & PlayerButtons.Moveleft) != 0)
            {
                player.CloseSubMenu();
            }

            if (((long)player.player.Buttons & 8589934592) == 8589934592)
            {
                player.OpenMainMenu(null);
            }

            player.Buttons = player.player.Buttons;

            if (player.CenterHtml != "")
            {
                Server.NextFrame(() =>
                    player.player.PrintToCenterHtml(player.CenterHtml)
                );
            }
        }
    }

    public static void PrintToChat(CCSPlayerController player, string message)
    {
        if (_.Config.Messages)
            player.PrintToChat(_.Config.Prefix + message);
    }

    public static void PlaySound(CCSPlayerController player, string sound)
    {
        player.ExecuteClientCommand($"play {sound}");
    }

    [CommandHelper(minArgs: 0, whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public static void Command_OpenMenus(CCSPlayerController player, CommandInfo info)
    {
        if (commandMenuId.TryGetValue(info.GetCommandString, out var menuId))
        {
            var menuConfig = _.Config.Menus[menuId];

            string permissionFlag = menuConfig.Permission.ToLower();

            string team = menuConfig.Team.ToLower();

            bool isTeamValid = (team == "t" || team == "terrorist") && player.Team == CsTeam.Terrorist ||
                               (team == "ct" || team == "counterterrorist") && player.Team == CsTeam.CounterTerrorist ||
                               (team == "" || team == "both" || team == "all");

            if ((!string.IsNullOrEmpty(permissionFlag) && !AdminManager.PlayerHasPermissions(player, permissionFlag)) || !isTeamValid)
            {
                PrintToChat(player, _.Localizer["NoPermission"]);
                return;
            }

            switch (_.Config.MenuStyle.ToLower())
            {
                case "chat":
                case "text":
                    Open_Chat(player, menuId);
                    break;
                case "html":
                case "center":
                case "centerhtml":
                case "hud":
                    Open_HTML(player, menuId);
                    break;
                case "wasd":
                case "wasdmenu":
                    Open_WASD(player, menuId);
                    break;
                default:
                    Open_HTML(player, menuId);
                    break;
            }
        }
    }

    public static void Open_Chat(CCSPlayerController player, string menuId)
    {
        var menuConfig = _.Config.Menus[menuId];

        ChatMenu menu = new(menuConfig.Title);

        foreach (var option in menuConfig.Options)
        {
            if (string.IsNullOrEmpty(option.Permission) || AdminManager.PlayerHasPermissions(player, option.Permission))
            {
                menu.AddMenuOption(option.Title, (player, menuOption) =>
                {
                    if (option.Confirm)
                    {
                        ChatMenu confirmMenu = new(_.Localizer["ConfirmTitle"]);

                        confirmMenu.AddMenuOption(_.Localizer["ConfirmAccept"], (player, confirmMenuOption) => {
                            ExecuteOption(player, option);
                        });

                        confirmMenu.AddMenuOption(_.Localizer["ConfirmDecline"], (player, confirmMenuOption) => {
                            MenuManager.OpenChatMenu(player, menu);
                        });

                        MenuManager.OpenChatMenu(player, confirmMenu);
                    }
                    else ExecuteOption(player, option);
                });
            }
        }

        MenuManager.OpenChatMenu(player, menu);
    }

    public static void Open_HTML(CCSPlayerController player, string menuId)
    {
        var menuConfig = _.Config.Menus[menuId];

        CenterHtmlMenu menu = new(menuConfig.Title, _);

        foreach (var option in menuConfig.Options)
        {
            if (string.IsNullOrEmpty(option.Permission) || AdminManager.PlayerHasPermissions(player, option.Permission))
            {
                menu.AddMenuOption(option.Title, (player, menuOption) =>
                {
                    if (option.Confirm)
                    {
                        CenterHtmlMenu confirmMenu = new(_.Localizer["ConfirmTitle"], _);

                        confirmMenu.AddMenuOption(_.Localizer["ConfirmAccept"], (player, confirmMenuOption) => {
                            ExecuteOption(player, option);
                        });

                        confirmMenu.AddMenuOption(_.Localizer["ConfirmDecline"], (player, confirmMenuOption) => {
                            MenuManager.OpenCenterHtmlMenu(_, player, menu);
                        });

                        MenuManager.OpenCenterHtmlMenu(_, player, confirmMenu);
                    }
                    else ExecuteOption(player, option);
                });
            }
        }

        MenuManager.OpenCenterHtmlMenu(_, player, menu);
    }

    public static void Open_WASD(CCSPlayerController player, string menuId)
    {
        var menuConfig = _.Config.Menus[menuId];

        IWasdMenu menu = WasdManager.CreateMenu(menuConfig.Title);

        foreach (var option in menuConfig.Options)
        {
            if (string.IsNullOrEmpty(option.Permission) || AdminManager.PlayerHasPermissions(player, menuConfig.Permission))
            {
                menu.Add(option.Title, (player, menuOption) =>
                {
                    if (option.Confirm)
                    {
                        IWasdMenu confirmMenu = WasdManager.CreateMenu(_.Localizer["ConfirmTitle"]);

                        confirmMenu.Add(_.Localizer["ConfirmAccept"], (player, confirmMenuOption) =>
                        {
                            ExecuteOption(player, option);
                        });

                        confirmMenu.Add(_.Localizer["ConfirmDecline"], (player, confirmMenuOption) =>
                        {
                            WasdManager.OpenMainMenu(player, menu);
                        });

                        WasdManager.OpenMainMenu(player, confirmMenu);
                    }
                    else ExecuteOption(player, option);
                });
            }
        }

        WasdManager.OpenMainMenu(player, menu);
    }

    private static void ExecuteOption(CCSPlayerController player, Options option)
    {
        PrintToChat(player, _.Localizer["Selected", option.Title]);

        player.ExecuteClientCommandFromServer(option.Command);

        if (option.Sound.Contains("vsnd"))
            PlaySound(player, option.Sound);

        if (option.CloseMenu)
            MenuManager.CloseActiveMenu(player);
    }
}