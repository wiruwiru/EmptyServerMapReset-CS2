using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace EmptyServerMapReset;

public class BaseConfigs : BasePluginConfig
{
    [JsonPropertyName("DefaultMap")]
    public string DefaultMap { get; set; } = "de_mirage";

    [JsonPropertyName("MinimumPlayers")]
    public int MinimumPlayers { get; set; } = 2;

    [JsonPropertyName("SecondsAfterEmpty")]
    public float SecondsAfterEmpty { get; set; } = 120.0f;

    [JsonPropertyName("DelayBeforeMapChange")]
    public float DelayBeforeMapChange { get; set; } = 3.0f;

    [JsonPropertyName("EnableDebug")]
    public bool EnableDebug { get; set; } = false;

}