# Development / 开发说明

Windows + .NET 8 SDK + PowerShell. No private repository, Python or game installation is required to build or package.

```powershell
.\build.ps1 -Locked
.\package.ps1 -Locked
```

`build.ps1` restores locked dependencies, builds WPF and runs the synthetic regression suite. `package.ps1` checks the final Portable/Lite EXEs alone in empty directories, renders both languages, verifies plans, filters, selections and confirmation gates, and produces checksummed releases. Generated files stay in `.build/` and `dist/`.

| Directory | Purpose |
| --- | --- |
| `desktop/` | WPF, confirmation and instance selection; no external web server |
| `core/` | Resource planning, HiGHS optimization, exact budgets and transaction reconciliation |
| `connection/` | Client discovery, Mono injection, local adapter compilation and member bindings |
| `hook/`, `shared/` | Equipment-only observation, UI actions, native craft/refine commands and policy gates |
| `localization/` | UI strings and static display aliases |
| `native-tests/` | Synthetic differential cases, fake-game accounting and connection policy checks |
| `vendor/`, `licenses/` | Redistributable dependencies and provenance |

## Verification boundaries / 验证范围

The 283 synthetic regression cases were exported from an independent reference implementation. They contain invented inventories and transaction traces, never captured account inventories. Tests compare exact accounting and planner objectives; different equally optimal recipe mixes are accepted. Connection policy checks additionally cover account/process identity, stale UI, operation ownership and batch limits.

Smoke checks exercise packaged UI and the in-process C# engine without connecting to a game. Optional client compilation checks validate current interfaces without injecting or consuming resources:

```powershell
.\package.ps1 -Locked -ClientManaged 'C:\Game\BrownDust II_Data\Managed'
```

Compilation and synthetic tests do not prove a live server transaction. For a new client or connection adapter, validate a small, explicitly approved craft/refine batch and reconcile inventory before claiming runtime verification. Never clear an unresolved journal to obtain a passing run.

## Compatibility updates / 客户端更新

`catalog.json` records exact client assembly and database hashes. `display-catalog.json` contains names/stat descriptions; `evidence-spec.json`, `ui-policy.json` and equipment-native methods define the consumed interfaces. Update them together only after rechecking costs, native batch stopping behavior, receipt ordering and resource deltas. Do not disable fingerprint checks merely to support an update.

The adapter binds only the equipment workflow, with isolated local files. No private daily controller, account switcher, collected replay or game DLL is published. Use one automation controller per game session. Attachment does not perform gameplay actions.

## Packaging and releases / 打包发布

Increment `Directory.Build.props`; keep the XAML version and bilingual documentation aligned. Commit lockfiles. Match the Git tag to `v<Version>`. CI runs the same clean package checks without the game. Release assets are two EXEs, two ZIPs and `SHA256SUMS.txt`; internal verification manifests are not release notes. See [publication style](PUBLICATION_STYLE.md).
