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
	public override string ModuleVersion => "1.0.0";
	public override string ModuleAuthor => "luca.uy";
	public override string ModuleDescription => "Automatically changes map if player count is too low.";

	public required BaseConfigs Config { get; set; }
	public void OnConfigParsed(BaseConfigs config)
	{
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
						Utils.DebugMessage($"Player count still < {Config.MinimumPlayers}. Notifying players and changing map to '{Config.DefaultMap}' in {Config.DelayBeforeMapChange} seconds...");

						Server.PrintToChatAll($"{Localizer["prefix"]} {string.Format(Localizer["low.player.mapchange"], Config.DelayBeforeMapChange)}");

						AddTimer(Config.DelayBeforeMapChange, () =>
						{
							Utils.DebugMessage($"Executing changelevel to '{Config.DefaultMap}'...");
							Server.ExecuteCommand($"changelevel {Config.DefaultMap}");
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