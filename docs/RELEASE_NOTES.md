# BD2 Equipment Assistant v0.3.0

## 简体中文

### 更新内容

- 接入与钓鱼工具同源的跨版本接口适配，不再锁定发布时的游戏 DLL／数据包。
- 读取当前游戏的装备、制作和精炼费用，新增装备及中英文名称随游戏数据更新。
- 执行前重新读取费用；发生变化时要求重新计算并确认。保留原生价格、回执和库存核账。
- 明确提示无法识别的接口变化，避免新旧连接组件同时运行。

### 下载

| 版本 | 运行环境 | 建议 |
| --- | --- | --- |
| Portable | 自带 .NET | 大多数用户 |
| Lite | .NET Desktop Runtime 8 x64 | 已安装桌面运行库 |

两版均内置中英界面，单个 EXE 可独立使用；ZIP 另含说明与许可证。使用 `SHA256SUMS.txt` 校验。

### 升级

停止并关闭旧工具。如果它已连接当前游戏，请正常重启游戏一次，再打开新版读取库存、计算并确认。旧计划和核账记录保留。跨版本适配不等于保证所有未来版本，无法确认的接口会明确停止。

## English

### Changes

- Adds the fishing tool's structural cross-version binding approach, removing release-time game DLL/data-package locks.
- Reads current equipment, crafting and refinement costs from the game, including new equipment and Chinese/English names.
- Refreshes costs before execution and requires a new confirmed plan if they changed. Native quotes, receipts and inventory reconciliation remain in place.
- Reports unsupported interface changes and prevents overlapping old/new equipment components.

### Downloads

| Edition | Runtime requirement | Recommended for |
| --- | --- | --- |
| Portable | Includes .NET | Most users |
| Lite | .NET Desktop Runtime 8 x64 | Desktop runtime already installed |

Both editions include Chinese and English. Each EXE works independently; ZIPs also include documentation and licenses. Verify using `SHA256SUMS.txt`.

### Upgrade

Stop and close the old tool. If it connected to the current game session, restart the game normally once, then read inventory and calculate/confirm again in the new version. Plans and journals are preserved. Cross-version adaptation is not a guarantee for every future release; unverified interfaces stop with diagnostics.
