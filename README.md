# ZombieSpeed - 僵尸加速技能插件

# ZombieSpeed - Zombie Speed Boost Plugin

---

## 📖 简介 / Introduction

**中文：**  
ZombieSpeed 是一个专为 Counter-Strike 2 开发的 CounterStrikeSharp 插件，为 T 阵营（恐怖分子）玩家提供临时加速技能。玩家可以通过按 R 键（换弹键）激活加速效果，在短时间内获得移动速度提升和视野变化。

**English：**  
ZombieSpeed is a CounterStrikeSharp plugin developed for Counter-Strike 2, providing temporary speed boost abilities for Terrorist team players. Players can activate the speed boost effect by pressing the R key (reload key), gaining increased movement speed and field of view changes for a short duration.

---

## ✨ 功能特性 / Features

### 中文
- 🚀 **加速技能**：T 阵营玩家可以激活临时加速效果
- ⏱️ **冷却机制**：技能使用后需要等待冷却时间才能再次使用
- 🎯 **视野变化**：加速时 FOV 会渐进式变化，提供更好的视觉体验
- ⌨️ **按键触发**：通过按 R 键（换弹键）快速激活技能
- 💬 **聊天提示**：使用技能时会在聊天中显示提示信息
- ⚙️ **可配置**：所有参数都可以通过配置文件自定义
- 🔄 **自动清理**：玩家断开连接或死亡时自动清理效果

### English
- 🚀 **Speed Boost**：Terrorist team players can activate temporary speed boost effects
- ⏱️ **Cooldown System**：Skills require a cooldown period after use before they can be used again
- 🎯 **FOV Changes**：FOV changes progressively during speed boost for better visual experience
- ⌨️ **Key Binding**：Quickly activate skills by pressing the R key (reload key)
- 💬 **Chat Notifications**：Display notification messages in chat when using skills
- ⚙️ **Configurable**：All parameters can be customized through configuration files
- 🔄 **Auto Cleanup**：Automatically clean up effects when players disconnect or die

---

## 📦 安装说明 / Installation

### 中文

1. **前置要求**
   - Counter-Strike 2 服务器
   - CounterStrikeSharp 框架已安装
   - .NET 8.0 运行时

2. **安装步骤**
   ```
   1. 将编译后的 ZombieSpeed.dll 文件复制到服务器的 plugins 目录
   2. 确保配置文件 ZombieSpeed.json 已生成在 configs 目录
   3. 重启服务器或使用插件热重载功能
   ```

3. **验证安装**
   - 在服务器控制台输入 `css_plugins list` 查看插件列表
   - 确认 ZombieSpeed 插件已加载

### English

1. **Requirements**
   - Counter-Strike 2 server
   - CounterStrikeSharp framework installed
   - .NET 8.0 runtime

2. **Installation Steps**
   ```
   1. Copy the compiled ZombieSpeed.dll file to the server's plugins directory
   2. Ensure the configuration file ZombieSpeed.json is generated in the configs directory
   3. Restart the server or use the plugin hot reload feature
   ```

3. **Verify Installation**
   - Type `css_plugins list` in the server console to view the plugin list
   - Confirm that the ZombieSpeed plugin is loaded

---

## ⚙️ 配置说明 / Configuration

### 中文

配置文件位置：`configs/ZombieSpeed.json`

#### 配置参数说明

| 参数名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `SpeedBoostCooldown` | float | 5.0 | 技能冷却时间（秒） |
| `SpeedBoostMultiplier` | float | 1.5 | 速度倍率（1.5 = 150% 速度） |
| `SpeedBoostDuration` | float | 3.0 | 加速持续时间（秒） |
| `SpeedBoostFov` | int | 120 | 加速时的目标 FOV 值 |
| `SpeedBoostKey` | string | "R" | 激活加速技能的按键（支持：Alt1, Alt2, Attack, Attack2, Attack3, Bullrush, Cancel, Duck, Grenade1, Grenade2, Space, Left, W, A, S, D, E, R, Shift, Right, Run, Walk, Weapon1, Weapon2, Zoom, Tab, Inspect） |

#### 配置示例

```json
{
  "Version": 1,
  "SpeedBoostCooldown": 5.0,
  "SpeedBoostMultiplier": 1.5,
  "SpeedBoostDuration": 3.0,
  "SpeedBoostFov": 120,
  "SpeedBoostKey": "R"
}
```

#### 配置建议

- **SpeedBoostCooldown**：建议设置在 3-10 秒之间，过短可能导致游戏失衡
- **SpeedBoostMultiplier**：建议设置在 1.2-2.0 之间，过高可能影响游戏体验
- **SpeedBoostDuration**：建议设置在 2-5 秒之间
- **SpeedBoostFov**：建议设置在 100-130 之间，过大会导致画面变形
- **SpeedBoostKey**：默认为 "R"（换弹键），可配置为其他按键。如果配置无效，将自动使用默认值 "R"

### English

Configuration file location: `configs/ZombieSpeed.json`

#### Configuration Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `SpeedBoostCooldown` | float | 5.0 | Skill cooldown time (seconds) |
| `SpeedBoostMultiplier` | float | 1.5 | Speed multiplier (1.5 = 150% speed) |
| `SpeedBoostDuration` | float | 3.0 | Speed boost duration (seconds) |
| `SpeedBoostFov` | int | 120 | Target FOV value during speed boost |
| `SpeedBoostKey` | string | "R" | Key to activate speed boost skill (Supported: Alt1, Alt2, Attack, Attack2, Attack3, Bullrush, Cancel, Duck, Grenade1, Grenade2, Space, Left, W, A, S, D, E, R, Shift, Right, Run, Walk, Weapon1, Weapon2, Zoom, Tab, Inspect) |

#### Configuration Example

```json
{
  "Version": 1,
  "SpeedBoostCooldown": 5.0,
  "SpeedBoostMultiplier": 1.5,
  "SpeedBoostDuration": 3.0,
  "SpeedBoostFov": 120,
  "SpeedBoostKey": "R"
}
```

#### Configuration Recommendations

- **SpeedBoostCooldown**：Recommended between 3-10 seconds, too short may cause game imbalance
- **SpeedBoostMultiplier**：Recommended between 1.2-2.0, too high may affect gameplay
- **SpeedBoostDuration**：Recommended between 2-5 seconds
- **SpeedBoostFov**：Recommended between 100-130, too high may cause screen distortion
- **SpeedBoostKey**：Default is "R" (reload key), can be configured to other keys. If configuration is invalid, will automatically use default value "R"

---

## 🎮 使用方法 / Usage

### 中文

#### 激活技能

1. **按键方式**（推荐）
   - 加入 T 阵营（恐怖分子）
   - 确保角色存活
   - 按下配置的按键（默认为 **R 键**/换弹键）即可激活加速技能

2. **命令方式**
   - 在聊天框输入：`!speedboost` 或 `/speedboost`
   - 或在控制台输入：`css_speedboost`

#### 技能效果

- 激活后，玩家移动速度会立即提升（根据配置的倍率）
- FOV 会渐进式变化，分为三个阶段：
  - **阶段 1**：从默认 FOV（90）渐进到目标 FOV
  - **阶段 2**：保持目标 FOV
  - **阶段 3**：从目标 FOV 渐进还原到默认 FOV
- 技能持续时间结束后，速度和 FOV 会自动还原

#### 限制条件

- 只有 **T 阵营**（恐怖分子）玩家可以使用
- 玩家必须 **存活** 才能使用
- 技能有 **冷却时间**，冷却期间无法使用
- 玩家死亡或切换阵营时，效果会自动清除

### English

#### Activate Skill

1. **Key Binding Method** (Recommended)
   - Join Terrorist team
   - Ensure your character is alive
   - Press the configured key (default is **R key**/reload key) to activate the speed boost skill

2. **Command Method**
   - Type in chat: `!speedboost` or `/speedboost`
   - Or type in console: `css_speedboost`

#### Skill Effects

- After activation, player movement speed immediately increases (according to configured multiplier)
- FOV changes progressively in three phases:
  - **Phase 1**：Gradually change from default FOV (90) to target FOV
  - **Phase 2**：Maintain target FOV
  - **Phase 3**：Gradually restore from target FOV to default FOV
- After skill duration ends, speed and FOV automatically restore

#### Restrictions

- Only **Terrorist team** players can use it
- Player must be **alive** to use
- Skill has **cooldown period**, cannot be used during cooldown
- Effects automatically clear when player dies or switches teams

---

## 💻 命令说明 / Commands

### 中文

| 命令 | 权限 | 说明 |
|------|------|------|
| `css_speedboost` | 所有玩家 | 手动激活加速技能 |
| `!speedboost` | 所有玩家 | 聊天命令，激活加速技能 |
| `/speedboost` | 所有玩家 | 聊天命令，激活加速技能 |

### English

| Command | Permission | Description |
|---------|------------|-------------|
| `css_speedboost` | All players | Manually activate speed boost skill |
| `!speedboost` | All players | Chat command to activate speed boost skill |
| `/speedboost` | All players | Chat command to activate speed boost skill |

---

## 🔧 技术细节 / Technical Details

### 中文

#### 实现原理

- **速度修改**：通过修改 `CCSPlayerPawn.VelocityModifier` 属性来改变玩家移动速度
- **FOV 管理**：使用 `CCSPlayerController.DesiredFOV` 和 `SetStateChanged` 来更新玩家视野
- **按键检测**：通过监听玩家按钮状态变化来检测 R 键按下事件
- **冷却管理**：使用字典存储每个玩家的冷却结束时间，在每 tick 检查冷却状态

#### 性能优化

- 使用字典（Dictionary）存储玩家数据，O(1) 查找效率
- 在 OnTick 事件中只处理有效且存活的玩家
- 自动清理断开连接玩家的数据，防止内存泄漏

#### 兼容性

- **CounterStrikeSharp API**：兼容最新版本
- **游戏版本**：Counter-Strike 2
- **.NET 版本**：.NET 8.0

### English

#### Implementation Details

- **Speed Modification**：Change player movement speed by modifying `CCSPlayerPawn.VelocityModifier` property
- **FOV Management**：Update player field of view using `CCSPlayerController.DesiredFOV` and `SetStateChanged`
- **Key Detection**：Detect R key press events by monitoring player button state changes
- **Cooldown Management**：Use dictionary to store each player's cooldown end time, check cooldown status every tick

#### Performance Optimization

- Use Dictionary to store player data with O(1) lookup efficiency
- Only process valid and alive players in OnTick event
- Automatically clean up disconnected player data to prevent memory leaks

#### Compatibility

- **CounterStrikeSharp API**：Compatible with latest version
- **Game Version**：Counter-Strike 2
- **.NET Version**：.NET 8.0

---

## 🐛 已知问题 / Known Issues

### 中文

- 无已知问题

如有发现 bug，请在 GitHub Issues 中报告。

### English

- No known issues

If you find any bugs, please report them in GitHub Issues.

---

## 📝 更新日志 / Changelog

### 中文

#### v0.0.1
- 初始版本发布
- 实现基础加速技能功能
- 支持 R 键触发
- 支持 FOV 渐进变化
- 支持冷却时间机制
- 支持配置文件自定义

### English

#### v0.0.1
- Initial release
- Implemented basic speed boost skill functionality
- Support R key trigger
- Support progressive FOV changes
- Support cooldown mechanism
- Support configuration file customization

---

## 📄 许可证 / License

### 中文

本项目采用 MIT 许可证。详见 LICENSE 文件。

### English

This project is licensed under the MIT License. See the LICENSE file for details.

---

## 👨‍💻 作者信息 / Author

### 中文

- **插件名称**：ZombieSpeed
- **作者**：僵尸加速技能
- **版本**：0.0.1
- **描述**：T 阵营玩家加速技能

### English

- **Plugin Name**：ZombieSpeed
- **Author**：僵尸加速技能
- **Version**：0.0.1
- **Description**：Terrorist team player speed boost skill

---

## 🤝 贡献 / Contributing

### 中文

欢迎提交 Pull Request 或报告问题！

### English

Pull requests and issue reports are welcome!

---

## 📞 支持 / Support

### 中文

如有问题或建议，请通过以下方式联系：
- GitHub Issues
- 服务器管理员

### English

For questions or suggestions, please contact through:
- GitHub Issues
- Server Administrator

---

**感谢使用 ZombieSpeed 插件！**  
**Thank you for using ZombieSpeed plugin!**

