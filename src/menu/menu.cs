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
    public static readonly Dictionary<int, WasdMenuPlayer> WasdPlayers = new();

    private const int oneSecond = 64;
    private static readonly Dictionary<int, PlayerCooldown> Cooldowns = new();
    public class PlayerCooldown
    {
        public Dictionary<string, int> OptionCooldowns { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> OptionCooldownTicks { get; set; } = new Dictionary<string, int>();
    }

    public static void Load(bool hotReload)
    {
        _.RegisterListener<OnTick>(OnTick);

        _.RegisterEventHandler<EventPlayerActivate>((@event, info) =>
        {
            CCSPlayerController? player = @event.Userid;

            if (player == null || !player.IsValid || player.IsBot)
                return HookResult.Continue;

            WasdPlayers[player.Slot] = new WasdMenuPlayer
            {
                player = player,
                Buttons = 0
            };

            Cooldowns[player.Slot] = new PlayerCooldown();

            return HookResult.Continue;
        });

        _.RegisterEventHandler<EventPlayerDisconnect>((@event, info) =>
        {
            CCSPlayerController? player = @event.Userid;

            if (player == null || !player.IsValid || player.IsBot)
                return HookResult.Continue;

            WasdPlayers.Remove(player.Slot);
            Cooldowns.Remove(player.Slot);

            return HookResult.Continue;
        });

        if (hotReload)
        {
            foreach (CCSPlayerController player in Utilities.GetPlayers())
            {
                if (player.IsBot)
                    continue;

                WasdPlayers[player.Slot] = new WasdMenuPlayer
                {
                    player = player,
                    Buttons = player.Buttons
                };

                //Cooldowns[player.Slot] = new PlayerCooldown();
            }
        }
    }

    public static void Unload()
    {
        _.RemoveListener<OnTick>(OnTick);

        WasdPlayers.Clear();
        Cooldowns.Clear();
    }

    public static void OnTick()
    {
        foreach (var playerCooldown in Cooldowns.Values)
        {
            var optionsToReset = new List<string>();

            foreach (var optionCooldown in playerCooldown.OptionCooldownTicks)
            {
                var optionCommand = optionCooldown.Key;
                var currentTicks = optionCooldown.Value;
                var cooldownDuration = playerCooldown.OptionCooldowns[optionCommand];

                if (currentTicks < cooldownDuration * oneSecond)
                    playerCooldown.OptionCooldownTicks[optionCommand]++;

                else optionsToReset.Add(optionCommand);
            }

            foreach (var option in optionsToReset)
            {
                playerCooldown.OptionCooldownTicks.Remove(option);
                playerCooldown.OptionCooldowns.Remove(option);
            }
        }

        foreach (WasdMenuPlayer? player in WasdPlayers.Values.Where(p => p.MainMenu != null))
        {
            if ((player.Buttons & PlayerButtons.Forward) == 0 && (player.player.Buttons & PlayerButtons.Forward) != 0)
                player.ScrollUp();

            else if ((player.Buttons & PlayerButtons.Back) == 0 && (player.player.Buttons & PlayerButtons.Back) != 0)
                player.ScrollDown();

            else if ((player.Buttons & PlayerButtons.Moveright) == 0 && (player.player.Buttons & PlayerButtons.Moveright) != 0)
                player.Choose();

            else if ((player.Buttons & PlayerButtons.Moveleft) == 0 && (player.player.Buttons & PlayerButtons.Moveleft) != 0)
                player.CloseSubMenu();

            if (((long)player.player.Buttons & 8589934592) == 8589934592)
                player.OpenMainMenu(null);

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

            string permission = menuConfig.Permission.ToLower();

            string team = menuConfig.Team.ToLower();

            bool isTeamValid = (team == "t" || team == "terrorist") && player.Team == CsTeam.Terrorist ||
                               (team == "ct" || team == "counterterrorist") && player.Team == CsTeam.CounterTerrorist ||
                               (team == "" || team == "both" || team == "all");

            if ((!string.IsNullOrEmpty(permission) && !AdminManager.PlayerHasPermissions(player, permission)) || !isTeamValid)
            {
                PrintToChat(player, _.Localizer["NoPermission"]);
                return;
            }

            switch (menuConfig.Type.ToLower())
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
        if (CommandCooldown(player, option.Command, option.Cooldown))
        {
            PrintToChat(player, _.Localizer["Cooldown"]);
            return;
        }

        PrintToChat(player, _.Localizer["Selected", option.Title]);

        var commands = option.Command.Split(',');
        foreach (var command in commands)
            player.ExecuteClientCommandFromServer(command.Trim());

        if (option.Sound.Contains("vsnd"))
            PlaySound(player, option.Sound);

        if (option.CloseMenu)
        {
            MenuManager.CloseActiveMenu(player);
            WasdManager.CloseMenu(player);
        }

        if (!Cooldowns.ContainsKey(player.Slot))
        {
            Cooldowns[player.Slot] = new PlayerCooldown();
        }

        if (option.Cooldown > 0)
        {
            Cooldowns[player.Slot].OptionCooldowns[option.Command] = option.Cooldown;
            Cooldowns[player.Slot].OptionCooldownTicks[option.Command] = 0;
        }
    }

    public static bool CommandCooldown(CCSPlayerController? player, string command, int cooldown)
    {
        if (player == null || !Cooldowns.ContainsKey(player.Slot))
            return false;

        if (Cooldowns[player.Slot].OptionCooldownTicks.TryGetValue(command, out var cooldownTicks))
        {
            if (cooldownTicks < cooldown * oneSecond)
                return true;
        }

        return false;
    }
}