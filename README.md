# EmptyServerMapReset

The **EmptyServerMapReset** plugin automatically changes the map if the number of connected players drops below a defined threshold. It is useful for server administrators who want to reset the map when there are too few players connected.

---

## Installation

1. Install [CounterStrike Sharp](https://github.com/roflmuffin/CounterStrikeSharp) and [Metamod:Source](https://www.sourcemm.net/downloads.php/?branch=master).

2. Download [EmptyServerMapReset.zip](https://github.com/wiruwiru/EmptyServerMapReset-CS2/releases) from the releases section.

3. Unzip the archive and upload it to your CS2 game server.

4. Start the server. The plugin will be loaded and ready to use.

---

## Configuration

The `EmptyServerMapReset.json` configuration file will be automatically generated when the plugin is first loaded. Below are the available configuration options:

| Parameter                | Description                                                                                              |
|--------------------------|----------------------------------------------------------------------------------------------------------|
| `DefaultMap`              | The map to change to when the number of connected players is below the `MinimumPlayers` threshold. Default is `de_mirage`. |
| `MinimumPlayers`          | The minimum number of players required to keep the server from resetting the map. Default is `2`.        |
| `SecondsAfterEmpty`       | The delay (in seconds) before changing the map after the player count drops below the minimum. Default is `60.0` seconds. |
| `DelayBeforeMapChange`    | The delay (in seconds) before changing the map after the condition is met. Default is `3.0` seconds.      |
| `EnableDebug`             | If enabled (`true`), debug messages will be displayed in the server console for troubleshooting. Default is `false`. |

---

## Example Configuration

Below is an example of the `EmptyServerMapReset.json` file:

```json
{
  "DefaultMap": "de_mirage",
  "MinimumPlayers": 2,
  "SecondsAfterEmpty": 240.0,
  "DelayBeforeMapChange": 3.0,
  "EnableDebug": false
}
```

---