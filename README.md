# cs2-custom-menu
**a plugin to create custom menus**
> supports chat, centerhtml & wasd menu
>
> has options for permission based or team based menus and commands

<br>

<details>
	<summary>showcase</summary>
	<img src="" width="800"> <br>
</details>

<br>

## information:

### requirements
- [MetaMod](https://cs2.poggu.me/metamod/installation)
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)

<br>

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
          "Team": "terrorist"
        }
      ]
    }
  }
}
```
