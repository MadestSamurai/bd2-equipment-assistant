# 0.3.5

装备目录通过分段传输读取，按请求、序号、长度和SHA核对，避免大表超过管道单条上限后超时。

Read equipment catalogs in bounded segments validated by request, sequence, length and SHA, avoiding timeouts when tables exceed the pipe frame limit.

| 版本 / Edition | 说明 / Requirements |
|---|---|
| Portable | 内置 .NET / Includes .NET |
| Lite | 需要 .NET 8 Desktop Runtime x64 / Requires .NET 8 Desktop Runtime x64 |

保持游戏运行，停止旧工具后连接新版即可交接。Keep the game running, stop the previous tool and connect the new build.
