# Cross-version adaptation / 跨版本适配

## 简体中文

装备助手与钓鱼工具使用同源的本机接口结构匹配器，独立于魔兽／公会模拟器及其数据快照。

1. 读取本机游戏元数据，以类型结构、方法体特征和调用位置解析需要的接口。只在结果唯一时绑定。
2. 用本机 Managed DLL 编译装备专用组件，检查运行游戏的 MVID 与此次编译一致。
3. 从已加载的游戏表读取装备、配方、强化、分解、精炼、词条、角色和名称；每次库存刷新及执行前重新读取。
4. 按当前数据重新生成目录。目录改变后，旧的确认计划失效，需要重新计算并确认。
5. 每批仍校验原生预览、服务器回执及实际库存差额。不重复提交结果未知的消费。

游戏 DLL 或数据包的整文件哈希变化本身不再阻止连接。程序不使用发布时的费用表处理真实消费。静态内置目录仅供离线自检；不作为连接失败的替代数据。

接口歧义、原生批量入口变化、随机品质导致不同产粉等规则变化，仍需要人工适配。不会自动生成不受支持的调用。新旧连接组件不同时运行，升级后如旧组件仍在游戏内，请正常重启游戏一次。

## English

The assistant uses the same local structural binding approach as the fishing tool, independently of the monster-hunt/guild simulators and their snapshots.

- Resolve required interfaces using type shapes, normalized body fingerprints and call sites; ambiguous matches are rejected.
- Compile against the installed client and verify its MVID before attaching.
- Read currently loaded equipment, recipes, enhancement/dismantling/refinement costs, options and bilingual names on refresh and before execution.
- Invalidate confirmed plans when their catalog changes; require recalculation and confirmation.
- Keep native quotes, server receipts and inventory reconciliation. Never resend an operation whose result is unknown.

A changed game DLL or data package hash alone does not block connection. Embedded catalogs are offline test fixtures, never a fallback for real spending. Unsupported batch semantics or cost structures still require maintenance. Restart the game normally once when upgrading from an already attached older component.
