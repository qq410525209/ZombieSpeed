using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace ZombieSpeed
{
    public class Buttons
    {
        public static readonly Dictionary<string, PlayerButtons> ButtonMapping = new()
        {
            { "Alt1", PlayerButtons.Alt1 },
            { "Alt2", PlayerButtons.Alt2 },
            { "Attack", PlayerButtons.Attack },
            { "Attack2", PlayerButtons.Attack2 },
            { "Attack3", PlayerButtons.Attack3 },
            { "Bullrush", PlayerButtons.Bullrush },
            { "Cancel", PlayerButtons.Cancel },
            { "Duck", PlayerButtons.Duck },
            { "Grenade1", PlayerButtons.Grenade1 },
            { "Grenade2", PlayerButtons.Grenade2 },
            { "Space", PlayerButtons.Jump },
            { "Left", PlayerButtons.Left },
            { "W", PlayerButtons.Forward },
            { "A", PlayerButtons.Moveleft },
            { "S", PlayerButtons.Back },
            { "D", PlayerButtons.Moveright },
            { "E", PlayerButtons.Use },
            { "R", PlayerButtons.Reload },
            { "Shift", PlayerButtons.Speed },
            { "Right", PlayerButtons.Right },
            { "Run", PlayerButtons.Run },
            { "Walk", PlayerButtons.Walk },
            { "Weapon1", PlayerButtons.Weapon1 },
            { "Weapon2", PlayerButtons.Weapon2 },
            { "Zoom", PlayerButtons.Zoom },
            { "Tab", PlayerButtons.Scoreboard },
            { "Inspect", PlayerButtons.Inspect }
        };
    }

    public class ZombieSpeed : BasePlugin, IPluginConfig<ZombieSpeedConfig>
    {
        public override string ModuleName => "ZombieSpeed";
        public override string ModuleDescription => "T阵营玩家加速技能";
        public override string ModuleAuthor => "僵尸加速技能";
        public override string ModuleVersion => "0.0.3";

        public ZombieSpeedConfig Config { get; set; } = new();

        // Store cooldown time for each player
        // 存储每个玩家的CD时间
        private Dictionary<ulong, float> _playerCooldowns = new Dictionary<ulong, float>();
        
        // Store the end time of speed boost effect for each player
        // 存储每个玩家当前加速效果的结束时间
        private Dictionary<ulong, float> _playerSpeedBoostEndTime = new Dictionary<ulong, float>();
        
        // Store the start time of speed boost effect for each player (used for calculating FOV change progress)
        // 存储每个玩家加速效果的开始时间（用于计算FOV变化进度）
        private Dictionary<ulong, float> _playerSpeedBoostStartTime = new Dictionary<ulong, float>();
        
        // Store player information (used for detecting key presses)
        // 存储每个玩家的信息（用于检测按键按下）
        private Dictionary<ulong, PlayerInfo> _playerInfo = new Dictionary<ulong, PlayerInfo>();
        
        // Store original FOV for each player (used to restore after speed boost ends)
        // 存储每个玩家的原始FOV（用于加速结束后还原）
        private Dictionary<ulong, uint> _playerOriginalFov = new Dictionary<ulong, uint>();

        // Store configured key
        // 存储配置的按键
        private PlayerButtons _speedBoostKey = PlayerButtons.Reload;

        // Store player information
        // 存储玩家信息
        private class PlayerInfo
        {
            public PlayerButtons PrevButtons { get; set; } = 0;
        }

        public void OnConfigParsed(ZombieSpeedConfig config)
        {
            Config = config;
            
            // Parse configured key name
            // 解析配置的按键名称
            if (!string.IsNullOrEmpty(config.SpeedBoostKey) && 
                Buttons.ButtonMapping.TryGetValue(config.SpeedBoostKey, out var button))
            {
                _speedBoostKey = button;
            }
            else
            {
                // If configuration is invalid, use default value R (Reload)
                // 如果配置无效，使用默认值 R (Reload)
                _speedBoostKey = PlayerButtons.Reload;
                // Localizer may not be initialized in OnConfigParsed, use English message
                // 在 OnConfigParsed 中 Localizer 可能还未初始化，使用英文提示
                Server.PrintToConsole($"[ZombieSpeed] Invalid key configuration '{config.SpeedBoostKey ?? "null"}', using default value 'R' (Reload)");
            }
        }

        public override void Load(bool hotReload)
        {
            RegisterListener<Listeners.OnTick>(OnTick);
            RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        }

        public override void Unload(bool hotReload)
        {
            // Restore FOV for all players
            // 还原所有玩家的FOV
            foreach (var player in Utilities.GetPlayers())
            {
                if (player != null && player.IsValid && _playerOriginalFov.ContainsKey(player.SteamID))
                {
                    SetFov(player, 90);
                }
            }
            
            _playerCooldowns.Clear();
            _playerSpeedBoostEndTime.Clear();
            _playerSpeedBoostStartTime.Clear();
            _playerInfo.Clear();
            _playerOriginalFov.Clear();
        }

        [GameEventHandler]
        public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
        {
            var player = @event.Userid;
            if (player != null && player.IsValid)
            {
                var steamId = player.SteamID;
                _playerCooldowns.Remove(steamId);
                _playerSpeedBoostEndTime.Remove(steamId);
                _playerSpeedBoostStartTime.Remove(steamId);
                _playerInfo.Remove(steamId);
                
                // Restore FOV
                // 还原FOV
                if (_playerOriginalFov.ContainsKey(steamId))
                {
                    SetFov(player, 90);
                    _playerOriginalFov.Remove(steamId);
                }
            }
            return HookResult.Continue;
        }

        private void OnTick()
        {
            var currentTime = Server.CurrentTime;

            foreach (var player in Utilities.GetPlayers())
            {
                if (!player.IsValid || player.IsBot || !player.PlayerPawn.IsValid)
                    continue;

                var pawn = player.PlayerPawn.Value;
                if (pawn == null || !pawn.IsValid)
                    continue;

                // Check if player is on Terrorist team and alive
                // 检查玩家是否为T阵营且存活
                if (player.Team != CsTeam.Terrorist || pawn.Health <= 0)
                {
                    // If player is not on Terrorist team or is dead, clear speed boost effect and restore FOV
                    // 如果玩家不是T阵营或已死亡，清除加速效果并还原FOV
                    if (_playerSpeedBoostEndTime.ContainsKey(player.SteamID))
                    {
                        _playerSpeedBoostEndTime.Remove(player.SteamID);
                        _playerSpeedBoostStartTime.Remove(player.SteamID);
                        if (pawn.VelocityModifier != 1.0f)
                        {
                            pawn.VelocityModifier = 1.0f;
                        }
                        // Restore FOV
                        // 还原FOV
                        if (_playerOriginalFov.ContainsKey(player.SteamID))
                        {
                            SetFov(player, (int)_playerOriginalFov[player.SteamID]);
                            _playerOriginalFov.Remove(player.SteamID);
                        }
                    }
                    continue;
                }

                // Check if speed boost effect has ended
                // 检查加速效果是否结束
                if (_playerSpeedBoostEndTime.ContainsKey(player.SteamID))
                {
                    if (currentTime >= _playerSpeedBoostEndTime[player.SteamID])
                    {
                        // Speed boost effect ended, restore FOV
                        // 加速效果结束，还原FOV
                        _playerSpeedBoostEndTime.Remove(player.SteamID);
                        _playerSpeedBoostStartTime.Remove(player.SteamID);
                        pawn.VelocityModifier = 1.0f;
                        if (_playerOriginalFov.ContainsKey(player.SteamID))
                        {
                            SetFov(player, (int)_playerOriginalFov[player.SteamID]);
                            _playerOriginalFov.Remove(player.SteamID);
                        }
                    }
                    else
                    {
                        // Maintain speed boost effect
                        // 保持加速效果
                        pawn.VelocityModifier = Config.SpeedBoostMultiplier;
                        
                        // Progressively update FOV
                        // 渐进更新FOV
                        UpdateFovProgressively(player, currentTime);
                    }
                }

                // Detect R key press (using the same method as DoubleJump)
                // 检测R键按下（使用与DoubleJump相同的方法）
                CheckPlayerButtonInput(player, pawn, currentTime);
            }
        }

        // Use speed boost skill
        // 使用加速技能
        [ConsoleCommand("css_speedboost", "使用加速技能")]
        public void OnSpeedBoostCommand(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid || player.IsBot)
                return;

            var pawn = player.PlayerPawn.Value;
            if (pawn == null || !pawn.IsValid || pawn.Health <= 0)
            {
                player.PrintToChat($" {ChatColors.Green}{Localizer["prefix"]} {ChatColors.White}{Localizer["must_be_alive"]}");
                return;
            }

            // Check if player is on Terrorist team
            // 检查是否为T阵营
            if (player.Team != CsTeam.Terrorist)
            {
                player.PrintToChat($" {ChatColors.Green}{Localizer["prefix"]} {ChatColors.White}{Localizer["only_terrorist"]}");
                return;
            }

            var steamId = player.SteamID;
            var currentTime = Server.CurrentTime;

            // Check cooldown
            // 检查CD
            if (_playerCooldowns.ContainsKey(steamId))
            {
                var cooldownEndTime = _playerCooldowns[steamId];
                if (currentTime < cooldownEndTime)
                {
                    var remainingTime = cooldownEndTime - currentTime;
                    player.PrintToChat($" {ChatColors.Green}{Localizer["prefix"]} {ChatColors.White}{Localizer["cooldown_remaining", $"{remainingTime:F1}"]}");
                    return;
                }
            }

            // Use skill
            // 使用技能
            if (pawn != null && pawn.IsValid)
            {
                // Save original FOV (if not saved yet), default FOV is 90
                // 保存原始FOV（如果还没有保存），默认FOV是90
                if (!_playerOriginalFov.ContainsKey(steamId))
                {
                    var currentFov = player.DesiredFOV;
                    // If FOV is invalid (0 or less than 90), use default value 90
                    // 如果FOV无效（为0或小于90），使用默认值90
                    _playerOriginalFov[steamId] = (currentFov <= 0 || currentFov < 90) ? 90 : currentFov;
                }
                
                pawn.VelocityModifier = Config.SpeedBoostMultiplier;
                _playerSpeedBoostStartTime[steamId] = currentTime;
                _playerSpeedBoostEndTime[steamId] = currentTime + Config.SpeedBoostDuration;
                _playerCooldowns[steamId] = currentTime + Config.SpeedBoostCooldown;

                player.PrintToChat($" {ChatColors.Green}{Localizer["prefix"]} {ChatColors.White}{Localizer["skill_activated", $"{Config.SpeedBoostDuration}"]}");
            }
        }

        // Detect R key press (using the same method as DoubleJump)
        // 检测R键按下（使用与DoubleJump相同的方法）
        private void CheckPlayerButtonInput(CCSPlayerController player, CCSPlayerPawn pawn, float currentTime)
        {
            if (pawn == null || !pawn.IsValid || pawn.Health <= 0)
                return;

            var steamId = player.SteamID;
            
            // Get or create player information
            // 获取或创建玩家信息
            if (!_playerInfo.TryGetValue(steamId, out var playerInfo))
            {
                playerInfo = new PlayerInfo();
                _playerInfo.Add(steamId, playerInfo);
            }

            // Get current button state
            // 获取当前按钮状态
            var currentButtons = player.Buttons;
            
            // Detect if configured key is pressed
            // 检测配置的按键是否按下
            var keyWasPressed = (playerInfo.PrevButtons & _speedBoostKey) != 0;
            var keyIsPressed = (currentButtons & _speedBoostKey) != 0;

            // Detect the moment when configured key transitions from released to pressed (trigger once)
            // 检测配置的按键从释放到按下的瞬间（触发一次）
            if (!keyWasPressed && keyIsPressed)
            {
                // Check if on cooldown
                // 检查是否在冷却中
                if (_playerCooldowns.ContainsKey(steamId))
                {
                    var cooldownEndTime = _playerCooldowns[steamId];
                    if (currentTime < cooldownEndTime)
                    {
                        var remainingTime = cooldownEndTime - currentTime;
                        player.PrintToChat($" {ChatColors.Green}{Localizer["prefix"]} {ChatColors.White}{Localizer["cooldown_remaining", $"{remainingTime:F1}"]}");
                    }
                    else
                    {
                        // Cooldown ended, trigger speed boost skill
                        // 冷却结束，触发加速技能
                        ActivateSpeedBoost(player, pawn, currentTime);
                    }
                }
                else
                {
                    // No cooldown, directly trigger speed boost skill
                    // 没有冷却，直接触发加速技能
                    ActivateSpeedBoost(player, pawn, currentTime);
                }
            }

            // Save current button state for next detection
            // 保存当前按钮状态，用于下次检测
            playerInfo.PrevButtons = currentButtons;
        }

        private void ActivateSpeedBoost(CCSPlayerController player, CCSPlayerPawn pawn, float currentTime)
        {
            if (pawn == null || !pawn.IsValid || pawn.Health <= 0)
                return;

            var steamId = player.SteamID;

            // Save original FOV (if not saved yet), default FOV is 90
            // 保存原始FOV（如果还没有保存），默认FOV是90
            if (!_playerOriginalFov.ContainsKey(steamId))
            {
                var currentFov = player.DesiredFOV;
                // If FOV is invalid (0 or less than 90), use default value 90
                // 如果FOV无效（为0或小于90），使用默认值90
                _playerOriginalFov[steamId] = (currentFov <= 0 || currentFov < 90) ? 90 : currentFov;
            }

            // Use skill
            // 使用技能
            pawn.VelocityModifier = Config.SpeedBoostMultiplier;
            _playerSpeedBoostStartTime[steamId] = currentTime;
            _playerSpeedBoostEndTime[steamId] = currentTime + Config.SpeedBoostDuration;
            _playerCooldowns[steamId] = currentTime + Config.SpeedBoostCooldown;

            player.PrintToChat($" {ChatColors.Green}{Localizer["prefix"]} {ChatColors.White}{Localizer["skill_activated", $"{Config.SpeedBoostDuration}"]}");
        }

        // Progressively update FOV
        // 渐进更新FOV
        private void UpdateFovProgressively(CCSPlayerController player, float currentTime)
        {
            if (!_playerSpeedBoostStartTime.ContainsKey(player.SteamID) || 
                !_playerOriginalFov.ContainsKey(player.SteamID))
                return;

            var steamId = player.SteamID;
            var startTime = _playerSpeedBoostStartTime[steamId];
            var endTime = _playerSpeedBoostEndTime[steamId];
            var elapsed = currentTime - startTime;
            var duration = endTime - startTime;
            // Each phase takes 1/3 of the time
            // 每个阶段占1/3时间
            var phaseDuration = duration / 3.0f;

            // Default FOV is 90, ensure progressive change starts from 90
            // 默认FOV是90，确保从90开始渐进
            const int defaultFov = 90;
            var originalFov = (int)_playerOriginalFov[steamId];
            // If original FOV is invalid (0 or less than 90), use default value 90
            // 如果原始FOV无效（为0或小于90），使用默认值90
            if (originalFov <= 0 || originalFov < defaultFov)
            {
                originalFov = defaultFov;
            }
            
            var targetFov = Config.SpeedBoostFov;
            int currentFov;

            if (elapsed < phaseDuration)
            {
                // Phase 1: Progressively change from 90 (default FOV) to target FOV
                // 阶段1：从90（默认FOV）渐进到目标FOV
                var progress = elapsed / phaseDuration;
                currentFov = Lerp(defaultFov, targetFov, progress);
            }
            else if (elapsed < phaseDuration * 2)
            {
                // Phase 2: Maintain target FOV
                // 阶段2：保持目标FOV
                currentFov = targetFov;
            }
            else
            {
                // Phase 3: Progressively restore from target FOV to 90 (default FOV)
                // 阶段3：从目标FOV渐进还原到90（默认FOV）
                var progress = (elapsed - phaseDuration * 2) / phaseDuration;
                currentFov = Lerp(targetFov, defaultFov, progress);
            }

            SetFov(player, currentFov);
        }

        // Linear interpolation function
        // 线性插值函数
        private int Lerp(int start, int end, float t)
        {
            // Limit t between 0 and 1
            // 限制t在0-1之间
            t = Math.Max(0, Math.Min(1, t));
            return (int)(start + (end - start) * t);
        }

        // Set player FOV
        // 设置玩家FOV
        private void SetFov(CCSPlayerController? player, int desiredFov)
        {
            if (player == null || !player.IsValid)
                return;

            player.DesiredFOV = (uint)desiredFov;
            Utilities.SetStateChanged(player, "CBasePlayerController", "m_iDesiredFOV");
        }
    }
}
