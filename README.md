# 🎮 WulinTrainer (大侠立志传修改器)

基于 **BepInEx 6 (IL2CPP)** 的《大侠立志传》游戏内修改器，提供丰富的游戏辅助功能，支持本体与 DLC 内容。

---

## ✨ 功能一览

### 📋 角色面板 (Role)
- 修改角色属性（力量、体质、敏捷等）
- 添加 / 管理特质 (Trait)
- 添加 / 管理 Buff

### 🎒 物品面板 (Item)
- 搜索并添加游戏内物品
- 按类别批量清除背包物品（装备 / 秘籍 / 消耗品 / 材料 / 地图 / 其他）

### ⚔️ 武学面板 (Martial)
- 搜索并添加武学

### 👥 NPC 面板 (NPC)
- 管理 NPC 相关数据

### 🛠 杂项面板 (Misc)
- **时间冻结** — 暂停游戏内时间流逝
- **自动恢复** — 自动恢复队伍体力 / 心情 / 生命
- **免战** — 跳过战斗
- **友好关系** — 关系值修改
- **成就解锁** — 一键解锁全部成就
- **武学大成** — 武学直接满级
- **修改银两 / 帮贡 / 江湖历练 / 冲穴点数**
- **移动速度 / 战斗速度** 调节滑块
- **解锁全部地图**
- **解锁全部配方**
- **自定义快捷键绑定**

### 🗺 地图快捷切换
通过快捷键快速切换不同大地图区域（无名小村、襄樊地区、中州地区、五岳地区、西南地区、西域地区）

### 🎮 DLC 支持
- DLC 角色面板 / Buff 面板 / 物品面板 / 杂项面板

---

## ⌨️ 默认快捷键

| 按键 | 功能 |
|---|---|
| `Tab` | 打开 / 关闭修改器面板 |
| `=` | 加速 |
| `-` | 减速 |
| `F1` | 一键恢复全队 |
| `F2` | 刷新悬赏任务 |
| `F3` ~ `F8` | 切换各大地图区域 |
| `Ctrl + F8` | 打开游戏控制台 |

> 所有快捷键均可在面板内自定义修改。

---

## 📦 安装方法

### 前置要求
- [BepInEx 6 (IL2CPP Bleeding Edge)](https://builds.bepinex.dev/projects/bepinex_be)

### 步骤
1. 将 BepInEx 6 BE 安装到游戏根目录
2. 首次启动游戏，让 BepInEx 生成 `interop` 文件夹
3. 将编译好的 `HaxxToyBox.dll` 放入 `BepInEx/plugins/` 目录
4. 启动游戏，进入存档后按 `Tab` 键呼出修改器界面

---

## 🔨 从源码构建

### 环境要求
- .NET 6.0 SDK
- Visual Studio 2022+ 或任何支持 .NET 6 的 IDE

### 构建步骤
1. 克隆本仓库
2. 在项目根目录创建 `lib/` 文件夹
3. 从游戏目录 `BepInEx/interop/` 中复制所有 DLL 到 `lib/`
4. 打开 `HaxxToyBox.sln`，修改 `.csproj` 中的 PreBuild/PostBuild 路径为你的游戏安装路径
5. 编译项目

> ⚠️ `lib/` 目录包含游戏特定的引用 DLL，体积较大且受版权保护，不包含在仓库中。你需要从自己的游戏安装目录中获取这些文件。

---

## 🏗 项目结构

```
├── ToyBox.cs           # BepInEx 插件入口
├── ToyBoxBehaviour.cs  # 核心行为组件，UI 初始化与热键处理
├── GUI/                # UI 面板
│   ├── RolePanel.cs        # 角色属性编辑
│   ├── ItemPanel.cs        # 物品管理
│   ├── MartialPanel.cs     # 武学管理
│   ├── NpcPanel.cs         # NPC 管理
│   ├── MiscPanel.cs        # 杂项功能
│   ├── TraitPanel.cs       # 特质添加
│   ├── BuffPanel.cs        # Buff 添加
│   ├── DlcPanel.cs         # DLC 面板
│   └── ...
├── Patches/            # Harmony 补丁
│   └── MiscPatch.cs        # 游戏逻辑 Hook
├── Config/             # 配置管理
│   ├── ConfigManager.cs    # 快捷键配置
│   └── ConfigHandler.cs    # 配置读写
├── Utilities/          # 工具类
└── Assets/             # UI AssetBundle 资源
```

---

## 📜 技术栈

- **C# / .NET 6.0**
- **BepInEx 6 IL2CPP** — 模组加载框架
- **HarmonyX** — 运行时方法 Hook
- **UniverseLib** — IL2CPP UI 工具库
- **Unity UI (uGUI)** — 游戏内界面

---

## ⚠️ 免责声明

本项目仅供学习和研究用途。使用修改器可能违反游戏的用户协议，请自行承担相关风险。

---

## 📄 License

MIT License
