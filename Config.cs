using System.Text.Json.Serialization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Config;

namespace ZombieSpeed
{
    public class ZombieSpeedConfig : BasePluginConfig
    {
        public override int Version { get; set; } = 1;

        [JsonPropertyName("SpeedBoostCooldown")]
        public float SpeedBoostCooldown { get; set; } = 5.0f;

        [JsonPropertyName("SpeedBoostMultiplier")]
        public float SpeedBoostMultiplier { get; set; } = 1.5f;

        [JsonPropertyName("SpeedBoostDuration")]
        public float SpeedBoostDuration { get; set; } = 3.0f;

        [JsonPropertyName("SpeedBoostFov")]
        public int SpeedBoostFov { get; set; } = 120;
    }
}
