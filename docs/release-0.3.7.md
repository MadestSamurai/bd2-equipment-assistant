# BD2 Equipment Assistant v0.3.7

## 简体中文

### 更新内容

- 修复新启动游戏尚无通信组件时，过早读取库存快照导致连接超时的问题。
- 短暂的只读通信超时／断管有限重试；连接被其他工具接管或身份变化时仍立即停止。
- 保留原始错误和阶段诊断，区分连接失败与执行后待核对；清理连接时的错误不再遮盖真正原因。
- 制作、分解和精炼不会因超时自动重复提交，避免重复消耗。
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

- Fixed premature inventory snapshot reads before a newly launched game has a communication component.
- Added bounded retries for transient read-only timeouts and broken pipes. Ownership or identity changes still stop immediately.
- Preserve the original error and operation phase, distinguish connection failures from unconfirmed execution, and prevent cleanup errors from masking the cause.
- Crafting, dismantling and refinement commands are not resubmitted after timeouts, avoiding duplicate resource consumption.
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
