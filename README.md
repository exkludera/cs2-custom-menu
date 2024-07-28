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
```json
{
  "Prefix": "{green}[Menu]{default}",
  "Messages": true, //no permission & selecting message
  "MenuStyle": "html", //chat - html - wasd
  "Menus": {
    "1": {
      "Title": "Public Menu",
      "Command": "css_publicmenu",
      "Options": [
        {
          "Title": "Example Command",
          "Command": "css"
        }
      ]
    },
    "2": {
      "Title": "VIP Menu",
      "Command": "css_vipmenu",
      "Permission": "@css/reservation",
      "Options": [
        {
          "Title": "VIP Command",
          "Command": "css_vip"
        }
      ]
    },
    "3": {
      "Title": "Example Menu",
      "Command": "css_examplemenu",
      "Permission": "@css/root",
      "Team": "terrorist",
      "Options": [
        {
          "Title": "Example Command",
          "Command": "css_example",
          "Permission": "@css/root",
          "Team": "terrorist",
          "Sound": "sounds/buttons/blip1.vsnd",
          "CloseMenu": "false"
        }
      ]
    }
  }
}
```
