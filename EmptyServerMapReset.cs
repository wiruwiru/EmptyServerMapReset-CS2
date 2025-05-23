using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Timers;

namespace EmptyServerMapReset;

[MinimumApiVersion(300)]
public class EmptyServerMapReset : BasePlugin, IPluginConfig<BaseConfigs>
{
	public override string ModuleName => "EmptyServerMapReset";
	public override string ModuleVersion => "1.0.1";
	public override string ModuleAuthor => "luca.uy";
	public override string ModuleDescription => "Automatically changes map if player count is too low.";

	public required BaseConfigs Config { get; set; }
	public void OnConfigParsed(BaseConfigs config)
	{
		if (string.IsNullOrWhiteSpace(config.DefaultMap))
		{
			Utils.DebugMessage("DefaultMap configuration is empty. Setting to default 'de_mirage'.");
			config.DefaultMap = "de_mirage";
		}

		if (config.DefaultMap.StartsWith("ws:"))
		{
			string workshopId = config.DefaultMap.Replace("ws:", "").Trim();

			if (string.IsNullOrWhiteSpace(workshopId) || !long.TryParse(workshopId, out _))
			{
				Utils.DebugMessage("The provided Workshop map ID is invalid. It should follow the format 'ws:12345678', where 12345678 is a numeric ID. Setting to default 'de_mirage'.");
				config.DefaultMap = "de_mirage";
			}

			Utils.DebugMessage($"Workshop map configured with ID: {workshopId}");
		}
		else if (!Server.IsMapValid(config.DefaultMap))
		{
			Utils.DebugMessage($"Warning: Default map '{config.DefaultMap}' doesn't appear to be valid! Setting to default 'de_mirage'.");
			config.DefaultMap = "de_mirage";
		}

		Config = config;
		Utils.Config = config;
	}

	public override void Load(bool hotReload)
	{
		if (hotReload)
		{
			Utils.DebugMessage("Reloading plugin...");
		}

		RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnectPre, HookMode.Pre);
	}

	[GameEventHandler]
	private HookResult OnPlayerDisconnectPre(EventPlayerDisconnect @event, GameEventInfo info)
	{
		if (@event.Userid is not CCSPlayerController player || player == null || !player.IsValid || player.IsBot)
			return HookResult.Continue;

		AddTimer(0.3f, () =>
		{
			var connectedPlayers = Utilities.GetPlayers().Where(x => !x.IsBot && x.Connected == PlayerConnectedState.PlayerConnected).ToList();
			Utils.DebugMessage($"Player disconnected. Connected players (excluding bots): {connectedPlayers.Count}");

			if (connectedPlayers.Count < Config.MinimumPlayers)
			{
				Utils.DebugMessage($"Connected players ({connectedPlayers.Count}) < MinimumPlayers ({Config.MinimumPlayers}). Waiting {Config.SecondsAfterEmpty} seconds before verifying again...");

				AddTimer(Config.SecondsAfterEmpty, () =>
				{
					var checkAgain = Utilities.GetPlayers().Where(x => !x.IsBot && x.Connected == PlayerConnectedState.PlayerConnected).ToList();
					Utils.DebugMessage($"Rechecking player count after delay: {checkAgain.Count} players connected.");

					if (checkAgain.Count < Config.MinimumPlayers)
					{
						string mapDisplayName = Config.DefaultMap.StartsWith("ws:") ? $"Workshop Map (ID: {Config.DefaultMap.Replace("ws:", "")})" : Config.DefaultMap;

						Utils.DebugMessage($"Player count still < {Config.MinimumPlayers}. Notifying players and changing map to '{mapDisplayName}' in {Config.DelayBeforeMapChange} seconds...");
						Server.PrintToChatAll($"{Localizer["prefix"]} {string.Format(Localizer["low.player.mapchange"], Config.DelayBeforeMapChange)}");

						AddTimer(Config.DelayBeforeMapChange, () =>
						{
							string command = Config.DefaultMap.StartsWith("ws:") ? $"host_workshop_map {Config.DefaultMap.Replace("ws:", "")}" : $"changelevel {Config.DefaultMap}";

							Utils.DebugMessage($"Executing command: {command}");
							Server.ExecuteCommand(command);
						});
					}
					else
					{
						Utils.DebugMessage($"Player count increased to {checkAgain.Count}. Map change canceled.");
					}
				});
			}
			else
			{
				Utils.DebugMessage($"Connected players ({connectedPlayers.Count}) > MinimumPlayers ({Config.MinimumPlayers}). No action taken.");
			}
		});

		return HookResult.Continue;
	}
}