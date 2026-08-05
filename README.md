# 生日提醒（BirthReminde）

一个用于 [ClassIsland](https://github.com/HelloWRC/ClassIsland) 的插件，可以在主界面显示当天或即将到来的生日提醒，并提供完整的生日信息管理与 CSV 批量导入能力。
只是我没想到在制作此插件的时候已经有人发布了类似插件，但是已经做都做了，还是继续做出来了。

>\[!caution]
> 此插件使用了AI辅助，即便我尽量看了大部分代码，但是一些安全方面、代码规范都会有一些问题
> 由于是我第一次写C#项目，对C#语言特性不是很熟悉，代码质量会比较差（反正写的很烂就是了）
- 项目地址：[TBHLLL/BirthReminde](https://github.com/TBHLLL/BirthReminde)
- 当前版本：1.0.0.0（见插件清单 `manifest.yml`）
- 技术栈：C# / .NET 8（`net8.0-windows`）、Avalonia 11、FluentAvalonia、CommunityToolkit.Mvvm、ClassIsland 插件 SDK

## 功能介绍

- 🎂 **生日提醒**：自动检测并显示当天或即将到来的生日，默认提醒范围为 7 天内，可自定义
- 📝 **生日管理**：支持添加、编辑、删除生日，支持全选与批量删除，双击行可快速编辑
- 📥 **CSV 批量导入**：自动识别 UTF-8 / UTF-16 / GBK 等编码，兼容多种日期写法，并支持重名处理
- 🎨 **显示定制**：可开关年龄显示、循环切换、过渡动画，并可配置循环间隔

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
| 通知推送 | 🕐 待开发 | `BirthNotified` 已占位，尚未实现 |
| Excel 文件支持 | 🕐 待开发 | 计划支持 .xlsx 格式文件导入 |

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
    │   └── Components/
    │       └── birthreminersettings.axaml(.cs)
    │       └── SettingsPage.axaml(.cs)  # 插件设置页
    ├── Properties/
    │   └── launchSettings.json      # ClassIsland 调试启动配置
    ├── bin/                         # 构建输出
    └── obj/                         # 中间文件
```

## 使用方法

1. 通过Classland导入插件.cipx文件；
2. 在主界面编辑模式中添加「生日提醒」组件；
3. 打开插件设置页，添加生日信息（姓名 + 日期），或通过 CSV 批量导入；
4. 组件会自动显示最近的生日提醒，当天生日会显示年龄信息。

CSV 格式为 `姓名,日期,备注(可选)`，例如：

```csv
张三,2026-08-05,同事
李四,1998年3月12日
```

支持 UTF-8（含 BOM）、UTF-16 与 GBK/GB18030 编码；日期支持 `2026/8/5`、`2026-08-05`、`2026年8月5日` 等多种写法。
> \[!caution]
> 这个功能是ai瞎写的，我还未对其进行验证，总之csv用excel编辑的yyyy/mm/dd格式是可以导入的。

> \[!tip]
> 未来会开发对.xlsx文件的支持
## 开发及验证

环境要求：

- Windows
- .NET 8 SDK
- ClassIsland 2.0.0 及以上（插件 SDK：`ClassIsland.PluginSdk` 1.7.106.2-dev-v2）

构建：

```powershell
dotnet restore
dotnet build
```

## 许可证

本项目基于 GNU Lesser General Public License v3.0 许可

