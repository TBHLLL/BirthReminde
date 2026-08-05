# 生日提醒（BirthReminde）

一个用于 [ClassIsland](https://github.com/HelloWRC/ClassIsland) 的插件，可以在主界面显示当天或即将到来的生日提醒，并提供完整的生日信息管理与 CSV 批量导入能力。

- 项目地址：[TBHLLL/BirthReminde](https://github.com/TBHLLL/BirthReminde)
- 当前版本：1.0.0.0（见插件清单 `manifest.yml`）
- 技术栈：C# / .NET 8（`net8.0-windows`）、Avalonia 11、FluentAvalonia、CommunityToolkit.Mvvm、ClassIsland 插件 SDK

## 功能介绍

- 🎂 **生日提醒**：自动检测并显示当天或即将到来的生日，默认提醒范围为 7 天内，可自定义
- ⚡ **实时更新**：每 60 秒自动刷新一次，并在数据或设置变化时立即更新显示
- 📝 **生日管理**：支持添加、编辑、删除生日，支持全选与批量删除，双击行可快速编辑
- 📥 **CSV 批量导入**：自动识别 UTF-8 / UTF-16 / GBK 等编码，兼容多种日期写法，并支持重名处理
- 🎨 **显示定制**：可开关年龄显示、循环切换、过渡动画，并可配置循环间隔
- 💾 **设置持久化**：数据与设置修改后自动保存到插件配置目录的 `Settings.json`

## 显示说明

- **当天生日**：显示 `今天生日！🎂🎂🎂` 和 `X岁`
- **明天生日**：显示 `明天生日` 和 `X+1岁`
- **其他情况**：显示 `还有X天` 和 `X+1岁`
- **无生日或不在提醒范围内**：显示 `近期无生日`

## 功能实现状况

| 功能 | 状态 | 说明 |
| --- | --- | --- |
| 生日提醒组件 | ✅ 已完成 | 显示姓名、倒计时文案与年龄 |
| 提醒范围设置 | ✅ 已完成 | 可自定义提醒天数（1 天起） |
| 年龄显示开关 | ✅ 已完成 | 可在组件设置中关闭 |
| 循环切换 | ✅ 已完成 | 多个生日时按设置间隔循环显示 |
| 过渡动画 | ✅ 已完成 | 切换内容时淡出/淡入 |
| 生日增删改、批量删除 | ✅ 已完成 | 设置页 DataGrid 管理，双击行编辑 |
| CSV 批量导入 | ✅ 已完成 | 编码识别、日期解析、重名处理 |
| 紧凑模式 | 🚧 部分实现 | 设置项已提供，组件布局暂未根据该选项调整 |
| 字体大小与颜色 | 🚧 依赖宿主 | 由 ClassIsland 组件高级设置调整，插件内不再维护 |
| 通知推送 | 🕐 待开发 | `BirthNotified` 已占位，尚未实现 |

## 项目结构

```text
BirthReminde/
├── BirthReminde.sln
├── .gitignore
├── README.md                       # 项目 README
└── BirthReminde/                   # 插件工程
    ├── BirthReminde.csproj
    ├── manifest.yml                 # 插件清单
    ├── Plugin.cs                    # 插件入口（注册组件、设置页与编码支持）
    ├── icon.png
    ├── Models/
    │   ├── BirthdayInfo.cs          # 生日数据模型（倒计时、年龄计算）
    │   ├── BirthdayRowViewModel.cs  # 列表行包装（勾选状态，不持久化）
    │   └── BirthRemindeComponentSettings.cs
    ├── Notifications/
    │   └── BirthNotified.cs         # 通知提供者（待实现）
    ├── Settings/
    │   ├── BirthRemindeSettings.cs  # 全局设置与自动保存
    │   └── ImortCSV.cs              # CSV 解析与导入分析
    ├── Views/
    │   ├── birthreminder.axaml(.cs) # 主界面提醒组件
    │   ├── SettingsPage.axaml(.cs)  # 插件设置页
    │   └── Components/
    │       └── birthreminersettings.axaml(.cs)
    ├── Properties/
    │   └── launchSettings.json      # ClassIsland 调试启动配置
    ├── bin/                         # 构建输出
    └── obj/                         # 中间文件
```

## 使用方法

1. 将构建产物放入 ClassIsland 的插件目录并启用该插件；
2. 在主界面编辑模式中添加「生日提醒」组件；
3. 打开插件设置页，添加生日信息（姓名 + 日期），或通过 CSV 批量导入；
4. 组件会自动显示最近的生日提醒，当天生日会显示年龄信息。

CSV 格式为 `姓名,日期,备注(可选)`，例如：

```csv
张三,2026-08-05,同事
李四,1998年3月12日
```

支持 UTF-8（含 BOM）、UTF-16 与 GBK/GB18030 编码；日期支持 `2026/8/5`、`2026-08-05`、`2026年8月5日` 等多种写法。

## 开发及验证

环境要求：

- Windows
- .NET 8 SDK
- ClassIsland 1.7.x 及以上（插件 SDK：`ClassIsland.PluginSdk` 1.7.106.2-dev-v2）

构建：

```powershell
dotnet restore
dotnet build
```

调试：项目已提供 `Properties/launchSettings.json` 中的「ClassIsland 插件」启动配置。在 Rider/Visual Studio 中设置 `ClassIsland_DebugBinaryFile` 与 `ClassIsland_DebugBinaryDirectory` 指向本机 ClassIsland 可执行文件后即可启动调试，ClassIsland 会通过 `-epp` 参数加载插件输出目录。

验证建议：

1. 添加一个今天过生日的条目，确认组件显示「今天生日！」及年龄；
2. 添加多个生日并开启循环切换，确认按间隔轮换；
3. 修改提醒范围，确认倒计时显示随之更新；
4. 分别导入 UTF-8 与 GBK 编码的 CSV，确认解析与重名处理符合预期；
5. 重启 ClassIsland，确认设置与生日数据已持久化。

## 许可证

当前仓库未包含 `LICENSE` 文件，在补充许可证之前默认保留所有权利。若计划公开发布，请添加合适的开源许可证（例如 MIT）并在本 README 中注明。
