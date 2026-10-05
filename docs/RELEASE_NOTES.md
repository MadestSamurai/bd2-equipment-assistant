# BD2 Equipment Assistant v0.3.6

## 简体中文

### 更新内容

- 修复丢失回执后观察组件一直忙碌、无法重新配置或切换工具的问题。
- 恢复连接不再依赖旧请求永久返回；结果未确认的制作、分解和精炼保留核对记录，不重复扣除资源。
- 完善统一宿主连接与多语言窗口协作。

### 下载

适用于 Windows x64。两版功能相同，均为单 EXE，内置简体中文／English。

| 版本 | 运行环境 | 建议 |
| --- | --- | --- |
| **Portable** | 内置 .NET 运行时 | 首次使用推荐，下载即用 |
| **Lite** | 需安装 .NET Desktop Runtime 8 x64 | 已安装运行时，下载更小 |

下载一种版本即可。ZIP 附中英文说明和许可证；SHA256SUMS.txt 可用于核对下载文件。

### 升级

暂停并关闭旧工具，再打开新版连接；本机设置保留。当前组件支持在游戏保持运行时更新和交接；如游戏本身仍显示断线或登录提示，请先恢复游戏连接。

[使用说明与风险声明](https://github.com/MadestSamurai/bd2-equipment-assistant/blob/main/README.md)

## English

### Changes

- Fixed the observation component remaining busy after missing responses, preventing reconfiguration or tool switching.
- Connection recovery no longer waits forever for old requests. Unconfirmed crafting, dismantling and refinement retain their records without repeating resource consumption.
- Improved shared-host connections and multilingual window integration.

### Downloads

For Windows x64. Both editions have the same features, run as a single EXE and include Simplified Chinese / English.

| Edition | Runtime requirement | Recommended for |
| --- | --- | --- |
| **Portable** | .NET included | Most users; download and run |
| **Lite** | .NET Desktop Runtime 8 x64 required | Smaller download if the runtime is installed |

Download one edition. ZIPs include both READMEs and licenses. Use SHA256SUMS.txt to verify downloaded files.

### Upgrade

Pause and close the old assistant, then connect with the new version. Local settings are retained. Current components support updates and handoff while the game stays open. If the game itself is disconnected or asking you to log in, restore its connection first.

[Usage and risk disclaimer](https://github.com/MadestSamurai/bd2-equipment-assistant/blob/main/README.en.md)

---

# BD2 Equipment Assistant v0.3.3

## 简体中文

### 更新内容

- 主窗口增加免费开源署名：GitHub MadestSamurai／B站 MadSamurai。
- 新增「来源与说明」，可查看并复制官方仓库与下载链接；随界面切换中英文。
- 统一双语 README、来源与风险说明，ZIP 附带完整说明；MIT 许可证保持不变。

### 下载

| 版本 | 运行环境 | 建议 |
| --- | --- | --- |
| **Portable** | 自带 .NET，无需另装运行库 | 大多数用户 |
| **Lite** | 需要 .NET Desktop Runtime 8 x64 | 已安装桌面运行库、希望减小下载体积 |

两版功能相同，内置简体中文／English。EXE 可独立使用；ZIP 附带双语说明与许可证。用 `SHA256SUMS.txt` 核对下载。

### 升级

停止自动操作并关闭旧工具，再打开新版。已有设置保留；本次主要更新来源与说明界面。

作者发布版免费。第三方收费不代表作者参与、背书或提供服务。[使用说明与风险提示](https://github.com/MadestSamurai/bd2-equipment-assistant/blob/main/README.md)。

## English

### Changes

- Adds free-release attribution to the main window: GitHub MadestSamurai / Bilibili MadSamurai.
- Adds About & source with selectable official repository and download links, following the selected UI language.
- Standardizes bilingual READMEs and source/risk notices, also included in ZIPs. The MIT License is unchanged.

### Downloads

| Build | Runtime | Recommended for |
| --- | --- | --- |
| **Portable** | Includes .NET; no separate runtime needed | Most users |
| **Lite** | Requires .NET Desktop Runtime 8 x64 | Smaller download when the desktop runtime is installed |

Both builds have identical features and include Simplified Chinese / English. EXEs run independently; ZIPs include bilingual documentation and licenses. Verify downloads with `SHA256SUMS.txt`.

### Upgrade

Stop automation and close the old tool, then open the new version. Existing settings are retained; this update primarily changes attribution and source information.

Official releases are free. Third-party fees do not imply the author's involvement, endorsement or support. [Usage and risk notice](https://github.com/MadestSamurai/bd2-equipment-assistant/blob/main/README.en.md).

---

# BD2 Equipment Assistant v0.3.2

## 简体中文

### 更新内容

- 修复批量制作完成首批后，被延迟出现的结果弹窗中断的问题。
- 将资源结算与结果界面收尾分开：确认结果展示已关闭后，再进入下一批。
- 修复结果弹窗叠层、装备奖励详情卡被误判为遮挡，以及页面还未就绪时误退回上一层的问题。
- 收尾中断会保留已核账进度，恢复后只处理剩余数量；不会重复制作或盲目确认未知弹窗。

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

- Fixes batch crafting stopping when its result popup appears after the first batch has settled.
- Separates resource settlement from result presentation; the next batch starts only after its result UI is handled.
- Fixes stacked result windows, embedded equipment reward cards being treated as blockers, and backing out of a page that is still becoming ready.
- Keeps verified progress if UI cleanup is interrupted. Resuming handles only the remaining quantity, without repeating crafting or blindly accepting unknown dialogs.

### Downloads

| Edition | Runtime requirement | Recommended for |
| --- | --- | --- |
| Portable | Includes .NET | Most users |
| Lite | .NET Desktop Runtime 8 x64 | Desktop runtime already installed |

Both editions include Chinese and English. Each EXE works independently; ZIPs also include documentation and licenses. Verify using `SHA256SUMS.txt`.

### Upgrade

Stop and close the old tool. If the old equipment assistant connected to the current game session, restart the game normally once, then read inventory and calculate/confirm again in the new version. Existing plans and journals are preserved.
