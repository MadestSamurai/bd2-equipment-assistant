# Localization / 翻译维护

One binary supports `zh-CN` and `en-US`; the top-right switch saves the preference locally. The system language is used only before a preference is saved.

- `localization/en-US.json`: Chinese UI source keys mapped to English. Preserve numbered format arguments such as `{0:N0}`.
- `localization/labels.json`: XAML resource IDs mapped to source keys.
- `localization/game-names.json`: Chinese/English display aliases for equipment and characters.
- `desktop/Language.cs`: display-only translation and preferences.

Never translate identifiers, raw inventory, plan hashes, transaction fields or receipts. Both languages must describe the same costs, targets and stopping rules. Dynamic stat labels are translated for display; original and translated search terms are retained. Raw diagnostics and some low-level exceptions remain unchanged.

`package.ps1` checks both languages, plans, equipment selection, round-trip language switching and Chinese/English search on synthetic data. Review its rendered `powder.png`, `refine.png`, `refine-small.png` and `language-switched.png` before release. Long English labels must wrap instead of clipping.

README files and release notes are maintained separately. The disclaimer appears once below each README's H1. Do not insert it by replacing a generic word or heading globally.
