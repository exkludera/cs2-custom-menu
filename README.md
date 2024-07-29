# cs2-custom-menu
**a plugin to create custom menus**
> supports chat, centerhtml & wasd menu
>
> has options for permission based or team based menus and commands

<br>

<details>
	<summary>showcase</summary>
	<video src="https://github.com/user-attachments/assets/07574910-1b56-48e4-90de-39342743bdaa">
</details>

<br>

## information:

### requirements
- [MetaMod](https://cs2.poggu.me/metamod/installation)
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)

<br>

<a href='https://ko-fi.com/G2G2Y3Z9R' target='_blank'><img style='border:0px; height:75px;' src='https://storage.ko-fi.com/cdn/brandasset/kofi_s_tag_dark.png?_gl=1*6vhavf*_gcl_au*MTIwNjcwMzM4OC4xNzE1NzA0NjM5*_ga*NjE5MjYyMjkzLjE3MTU3MDQ2MTM.*_ga_M13FZ7VQ2C*MTcyMjIwMDA2NS4xNy4xLjE3MjIyMDA0MDUuNjAuMC4w' border='0' alt='Buy Me a Coffee at ko-fi.com' /></a> <br>

## example config
**Messages** - Default: `true` (sends no permission & selecting message) <br>
**MenuStyle** - Default: `"html"` (options: chat/html/wasd) <br>
**Permission** - Default: `""` (empty for no check, @css/reservation for vip) <br>
**Team** - Default: `""` (T for Terrorist, CT for CounterTerrorist or empty for both) <br>

**Sound** - Default: `""` (use vsnd like sounds/buttons/blip1.vsnd) <br>
**CloseMenu** - Default: `false` (close the menu on select) <br>
**Confirm** - Default: `false` (opens a confirmation menu on select) <br>
**Cooldown** - Default: `0` (how many seconds until the command can be used again) <br>

```json
{
  "Prefix": "{green}[Menu]{default}",
  "Messages": true,
  "MenuStyle": "html",
  "Menus": {
    "1": {
      "Title": "Example Menu",
      "Command": "css_examplemenu",
      "Permission": "",
      "Team": "",
      "Options": [
        {
          "Title": "Example Command",
          "Command": "css_example",
          "Permission": "",
          "Team": "",
          "Sound": "",
          "CloseMenu": false,
          "Confirm": false,
          "Cooldown": 0
        },
        {
          "Title": "Example Command 2",
          "Command": "css_example2",
          "Permission": "@css/root",
          "Team": "T",
          "Sound": "sounds/buttons/blip1.vsnd",
          "CloseMenu": true,
          "Confirm": true,
          "Cooldown": 3
        }
      ]
    }
  }
}
```