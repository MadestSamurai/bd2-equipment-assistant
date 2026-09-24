# BD2 Equipment Assistant v0.3.1

## 简体中文

### 更新内容

- 修复 0.3.0 连接后读取库存失败，提示 `Native table unavailable: EquipmentTable` 的问题。
- 修正当前游戏数据表的读取入口；装备、制作配方及精炼费用继续从本机游戏读取。
- 保留中英切换、跨版本接口适配、批量制作与精炼功能。

### 下载

| 版本 | 运行环境 | 建议 |
| --- | --- | --- |
| Portable | 自带 .NET | 大多数用户 |
| Lite | .NET Desktop Runtime 8 x64 | 已安装桌面运行库 |

两版均内置中英界面，单个 EXE 可独立使用；ZIP 另含说明与许可证。使用 `SHA256SUMS.txt` 校验。

### 升级

停止并关闭旧工具。如果旧装备助手已连接当前游戏，请正常重启游戏一次，再打开新版读取库存、计算并确认。原有计划和核账记录保留。

## English

### Changes

- Fixes inventory capture failing after connection in v0.3.0 with `Native table unavailable: EquipmentTable`.
- Corrects the game's table-reading API selection. Equipment, crafting recipes and refinement costs still come from the installed game.
- Retains Chinese/English language switching, cross-version interface binding, batch crafting and batch refinement.

### Downloads

| Edition | Runtime requirement | Recommended for |
| --- | --- | --- |
| Portable | Includes .NET | Most users |
| Lite | .NET Desktop Runtime 8 x64 | Desktop runtime already installed |

Both editions include Chinese and English. Each EXE works independently; ZIPs also include documentation and licenses. Verify using `SHA256SUMS.txt`.

### Upgrade

Stop and close the old tool. If the old equipment assistant connected to the current game session, restart the game normally once, then read inventory and calculate/confirm again in the new version. Existing plans and journals are preserved.
