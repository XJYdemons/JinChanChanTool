# 金铲铲助手 (JinChanChanTool) 项目结构文档

> 最后更新: 2025-12-22

## 项目概览

金铲铲助手是一个基于 **.NET 8.0** 的 **Windows Forms** 桌面应用程序，用于辅助《金铲铲之战》(Teamfight Tactics) 游戏。项目包含 **93 个 C# 源代码文件**，采用分层架构。

---

## 完整目录树

```
JinChanChanTool/
│
├── 项目配置文件
│   ├── JinChanChanTool.csproj              # 项目文件（.NET 8.0，x64，高 DPI）
│   ├── JinChanChanTool.csproj.user         # 项目用户配置
│   ├── app.manifest                         # 应用清单
│   └── CLAUDE.md                            # Claude AI 工作流程指南
│
├── Program.cs                               # 应用入口点（依赖注入配置）
│
├── Properties/                              # 项目属性
│   ├── Resources.resx                       # 资源文件清单
│   ├── Resources.Designer.cs                # 资源设计器（自动生成）
│   └── PublishProfiles/                     # 发布配置
│
├── DataClass/                               # 数据模型层
│   ├── Hero.cs                              # 英雄数据对象
│   ├── Equipment.cs                         # 装备数据对象
│   ├── LineUp.cs                            # 阵容数据对象（含 SubLineUp、LineUpUnit）
│   ├── HeroEquipment.cs                     # 英雄装备推荐对象
│   ├── Profession.cs                        # 职业对象
│   ├── Peculiarity.cs                       # 特质对象
│   ├── ManualSettings.cs                    # 用户手动配置
│   ├── AutomaticSettings.cs                 # 自动应用配置
│   ├── RecommendedLineUp.cs                 # 推荐阵容对象
│   ├── ResultMapping.cs                     # 结果映射类
│   ├── MetatftLineupDtos.cs                 # 网络爬虫数据模型
│   └── StaticData/                          # 坐标模板（静态数据）
│       ├── JccCoordinateTemplates.cs        # 金铲铲坐标模板
│       └── TftCoordinateTemplates.cs        # TFT 坐标模板
│
├── Forms/                                   # 窗体层（16 个窗体）
│   ├── NecessaryForm/                       # 核心窗体
│   │   ├── MainForm.cs / .Designer.cs       # 主窗体
│   │   ├── SettingForm.cs / .Designer.cs    # 设置窗体
│   │   └── AboutForm.cs / .Designer.cs      # 关于窗体
│   │
│   ├── DisplayUIForm/                       # 显示交互窗体
│   │   ├── SelectForm.cs / .Designer.cs     # 英雄选择窗体
│   │   ├── OutputForm.cs / .Designer.cs     # 输出窗体
│   │   ├── EquipmentForm.cs / .Designer.cs  # 装备显示窗体
│   │   ├── LineUpForm.cs / .Designer.cs     # 阵容显示窗体
│   │   ├── LineUpSelectForm.cs / .Designer.cs    # 阵容选择窗体
│   │   └── StatusOverlayForm.cs / .Designer.cs   # 状态覆盖层
│   │
│   ├── ToolForm/                            # 工具窗体
│   │   ├── CorrectionEditorForm.cs / .Designer.cs   # OCR 纠正编辑器
│   │   ├── HeroInfoEditorForm.cs / .Designer.cs     # 英雄信息编辑器
│   │   └── ProcessSelectorForm.cs / .Designer.cs    # 进程选择器
│   │
│   └── ProgressForm.cs / .Designer.cs       # 进度显示窗体
│
├── DIYComponents/                           # 自定义组件（11 个）
│   ├── CustomTitleBar.cs                    # 自定义标题栏（支持拖拽）
│   ├── CustomPanel.cs                       # 自定义面板（DPI 适配）
│   ├── CustomFlowLayoutPanel.cs             # 自定义流布局面板
│   ├── HeroPictureBox.cs                    # 英雄头像框
│   ├── HeroAndEquipmentPictureBox.cs / .Designer.cs  # 英雄装备复合框
│   ├── HexagonBoard.cs                      # 六边形棋盘
│   ├── HexagonCell.cs                       # 六边形单元格
│   ├── BenchPanel.cs                        # 替补席面板
│   ├── EquipmentToolTip.cs                  # 装备提示框
│   └── EquipmentInformationToolTip.cs       # 装备信息提示框
│
├── Services/                                # 业务逻辑层（34 个文件）
│   │
│   ├── DataServices/                        # 数据服务
│   │   ├── Interface/                       # 接口定义
│   │   │   ├── IHeroDataService.cs
│   │   │   ├── IEquipmentService.cs
│   │   │   ├── ICorrectionService.cs
│   │   │   ├── ILineUpService.cs
│   │   │   ├── IManualSettingsService.cs
│   │   │   └── IAutomaticSettingsService.cs
│   │   │
│   │   ├── HeroDataService.cs               # 英雄数据服务
│   │   ├── EquipmentService.cs              # 装备数据服务
│   │   ├── LineUpService.cs                 # 阵容数据服务
│   │   ├── CorrectionService.cs             # OCR 纠正服务
│   │   ├── ManualSettingsService.cs         # 用户配置管理
│   │   ├── AutomaticSettingsService.cs      # 自动配置管理
│   │   ├── RecommendedLineUpService.cs      # 推荐阵容服务
│   │   └── IRecommendedLineUpService.cs     # 推荐阵容接口
│   │
│   ├── RecommendedEquipment/                # 装备推荐爬虫
│   │   ├── Interface/
│   │   │   ├── IDynamicGameDataService.cs
│   │   │   ├── ICrawlingService.cs
│   │   │   └── IHeroEquipmentDataService.cs
│   │   ├── DynamicGameDataService.cs        # 动态游戏数据
│   │   ├── CrawlingService.cs               # 装备爬虫实现
│   │   ├── HeroEquipmentDataService.cs      # 英雄装备数据
│   │   ├── TranslationModels.cs             # 翻译模型
│   │   └── UnitDetailModels.cs              # 单位详情模型
│   │
│   ├── LineupCrawling/                      # 阵容推荐爬虫
│   │   ├── Interface/
│   │   │   └── ILineupCrawlingService.cs
│   │   └── LineupCrawlingService.cs         # 阵容爬虫实现
│   │
│   ├── AutomaticSetCoordinates/             # 自动坐标设置
│   │   ├── AutomationService.cs             # 自动化服务
│   │   ├── CoordinateCalculationService.cs  # 坐标计算
│   │   ├── ProcessDiscoveryService.cs       # 进程发现
│   │   └── WindowInteractionService.cs      # 窗口交互
│   │
│   ├── ManuallySetCoordinates/              # 手动坐标设置
│   │   └── FastSettingPositionService.cs    # 快速位置设置
│   │
│   ├── Network/                             # 网络模块
│   │   └── HttpProvider.cs                  # HTTP 客户端单例
│   │
│   ├── CardService.cs                       # 卡牌服务
│   ├── QueuedOCRService.cs                  # 队列式 OCR 服务
│   └── UIBuilderService.cs                  # UI 动态构建服务
│
├── Tools/                                   # 工具类库（8 个）
│   ├── KeyBoardTools/
│   │   ├── GlobalHotkeyTool.cs              # 全局热键
│   │   └── KeyboardControlTool.cs           # 键盘控制
│   │
│   ├── MouseTools/
│   │   ├── MouseControlTool.cs              # 鼠标控制
│   │   ├── MouseHookTool.cs                 # 鼠标钩子
│   │   └── MousePositionTool.cs             # 鼠标位置
│   │
│   ├── LineUpCodeTools/
│   │   └── LineUpParser.cs                  # 阵容解析器
│   │
│   ├── ImageProcessingTool.cs               # 图像处理（OpenCvSharp）
│   └── LogTool.cs                           # 日志工具
│
├── Resources/                               # 资源文件
│   ├── CorrectionsList.json                 # OCR 纠正列表
│   ├── defaultHeroIcon.png                  # 默认英雄图标
│   │
│   ├── HeroDatas/                           # 赛季英雄数据
│   │   ├── 强音对决/                        # 赛季数据
│   │   │   ├── HeroData.json
│   │   │   └── images/                      # 英雄头像
│   │   │
│   │   └── 英雄联盟传奇/                    # 当前赛季
│   │       ├── HeroData.json                # 英雄数据
│   │       ├── Equipment.json               # 装备数据
│   │       ├── RecommendedLineUps.json      # 推荐阵容
│   │       ├── images/                      # 英雄头像
│   │       └── EquipmentImages/             # 装备图片
│   │
│   └── Models/                              # PaddleOCR 模型
│       ├── ppocr_keys_v5.txt                # OCR 字符字典
│       ├── PP-OCRv5_mobile_det_infer/       # 文本检测模型
│       └── PP-OCRv5_mobile_rec_infer/       # 文本识别模型
│
├── Documents/                               # 文档目录
│
├── bin/                                     # 编译输出
├── obj/                                     # 编译中间文件
│
└── .claude/                                 # Claude 配置
    └── settings.local.json
```

---

## 数据模型层 (DataClass)

| 类名 | 功能 | 关键属性 |
|------|------|---------|
| **Hero** | 英雄数据 | HeroName, Cost, Profession[], Peculiarity[], Image |
| **Equipment** | 装备数据 | Name, EquipmentType, SyntheticPathway[], Image |
| **LineUp** | 阵容容器 | LineUpName, SubLineUp[3] |
| **SubLineUp** | 变阵（前/中/后期） | SubLineUpName, LineUpUnit[] |
| **LineUpUnit** | 最小单位 | HeroName, EquipmentNames[3], Position |
| **Profession** | 职业 | Title, Heros[] |
| **Peculiarity** | 特质 | Title, Heros[] |
| **ManualSettings** | 手动配置 | 用户自定义坐标设置 |
| **AutomaticSettings** | 自动配置 | 自动识别的坐标设置 |

---

## 业务逻辑层 (Services)

### 数据服务 (DataServices)
| 服务 | 接口 | 功能 |
|------|------|------|
| HeroDataService | IHeroDataService | 英雄 JSON 数据加载/管理 |
| EquipmentService | IEquipmentService | 装备数据加载/管理 |
| LineUpService | ILineUpService | 阵容数据加载/管理 |
| CorrectionService | ICorrectionService | OCR 结果纠正 |
| ManualSettingsService | IManualSettingsService | 用户配置持久化 |
| AutomaticSettingsService | IAutomaticSettingsService | 自动配置管理 |
| RecommendedLineUpService | IRecommendedLineUpService | 推荐阵容管理 |

### 爬虫服务 (Crawling)
| 服务 | 功能 |
|------|------|
| CrawlingService | 从网络爬取装备推荐数据 |
| LineupCrawlingService | 从网络爬取阵容推荐数据 |
| DynamicGameDataService | 获取动态游戏数据 |
| HeroEquipmentDataService | 管理英雄装备推荐数据 |

### 坐标设置服务
| 服务 | 功能 |
|------|------|
| AutomationService | 自动化操作服务 |
| CoordinateCalculationService | 坐标计算 |
| ProcessDiscoveryService | 游戏进程发现 |
| WindowInteractionService | 窗口交互 |
| FastSettingPositionService | 快速手动设置位置 |

### 核心服务
| 服务 | 功能 |
|------|------|
| QueuedOCRService | 队列式 OCR 识别（CPU/GPU） |
| UIBuilderService | 动态 UI 构建（高 DPI 适配） |
| CardService | 卡牌服务 |
| HttpProvider | HTTP 客户端单例 |

---

## 窗体层 (Forms)

### 核心窗体 (NecessaryForm)
| 窗体 | 功能 |
|------|------|
| MainForm | 主窗体，应用核心 UI |
| SettingForm | 设置窗体 |
| AboutForm | 关于窗体 |

### 显示窗体 (DisplayUIForm)
| 窗体 | 功能 |
|------|------|
| SelectForm | 英雄选择界面 |
| OutputForm | 输出显示界面 |
| EquipmentForm | 装备展示界面 |
| LineUpForm | 阵容展示界面 |
| LineUpSelectForm | 阵容选择界面 |
| StatusOverlayForm | 状态覆盖层（透明） |

### 工具窗体 (ToolForm)
| 窗体 | 功能 |
|------|------|
| CorrectionEditorForm | OCR 纠正规则编辑器 |
| HeroInfoEditorForm | 英雄信息编辑器 |
| ProcessSelectorForm | 游戏进程选择器 |

---

## 自定义组件 (DIYComponents)

| 组件 | 功能 |
|------|------|
| CustomTitleBar | 自定义标题栏（支持拖拽移动） |
| CustomPanel | 自定义面板（DPI 适配） |
| CustomFlowLayoutPanel | 自定义流布局面板 |
| HeroPictureBox | 英雄头像框（边框、选中状态） |
| HeroAndEquipmentPictureBox | 英雄装备复合显示框 |
| HexagonBoard | 六边形棋盘 |
| HexagonCell | 六边形单元格 |
| BenchPanel | 替补席面板 |
| EquipmentToolTip | 装备提示框 |
| EquipmentInformationToolTip | 装备详细信息提示框 |

---

## 工具类 (Tools)

### 键盘工具 (KeyBoardTools)
| 工具 | 功能 |
|------|------|
| GlobalHotkeyTool | 全局热键注册与监听 |
| KeyboardControlTool | 键盘按键模拟 |

### 鼠标工具 (MouseTools)
| 工具 | 功能 |
|------|------|
| MouseControlTool | 鼠标移动与点击 |
| MouseHookTool | 鼠标钩子监听 |
| MousePositionTool | 获取鼠标位置 |

### 其他工具
| 工具 | 功能 |
|------|------|
| ImageProcessingTool | 图像处理（OpenCvSharp） |
| LineUpParser | 阵容代码解析 |
| LogTool | 日志输出 |

---

## 资源文件结构

### 英雄数据 (HeroDatas)
```
HeroDatas/
├── 强音对决/           # 历史赛季
│   ├── HeroData.json   # 68 个英雄
│   └── images/         # 英雄头像
│
└── 英雄联盟传奇/       # 当前赛季
    ├── HeroData.json   # ~150+ 英雄
    ├── Equipment.json  # 装备数据
    ├── RecommendedLineUps.json
    ├── images/         # 100+ 英雄头像
    └── EquipmentImages/ # 200+ 装备图片
```

### OCR 模型 (Models)
```
Models/
├── ppocr_keys_v5.txt              # 字符字典 (91K)
├── PP-OCRv5_mobile_det_infer/     # 文本检测 (4.8M)
│   ├── inference.json
│   ├── inference.pdiparams
│   └── inference.yml
└── PP-OCRv5_mobile_rec_infer/     # 文本识别 (17M)
    ├── inference.json
    ├── inference.pdiparams
    └── inference.yml
```

---

## 技术栈

| 类别 | 技术 |
|------|------|
| 框架 | .NET 8.0 (Windows Forms) |
| 语言 | C# 12.0 (Nullable, ImplicitUsings) |
| 图像处理 | OpenCvSharp4 |
| OCR | Sdcb.PaddleOCR (CPU/GPU) |
| 网络 | HttpClient (连接复用) |
| 序列化 | Newtonsoft.Json, YamlDotNet |
| 高 DPI | PerMonitorV2 |
| 自动化 | 全局热键 + 鼠标钩子 |

---

## 架构特点

### 1. 依赖注入
```csharp
// Program.cs
new MainForm(
    _iManualSettingsService,
    _iAutomaticSettingsService,
    _iHeroDataService,
    ...
);
```

### 2. 高 DPI 适配
```csharp
// Designer.cs
AutoScaleDimensions = new SizeF(96F, 96F);
AutoScaleMode = AutoScaleMode.Dpi;

// UIBuilderService
private int Dpi_S(int i) => SelectForm.Instance.LogicalToDeviceUnits(i);
private int Dpi_M(int i) => _mainForm.LogicalToDeviceUnits(i);
```

### 3. 网络单例模式
```csharp
// HttpProvider.cs
public static class HttpProvider
{
    private static readonly HttpClient _instance;
    public static HttpClient Client => _instance;
}
```

### 4. 队列式 OCR
```csharp
// QueuedOCRService
// 基于队列处理 OCR 请求
// 支持 CPU/GPU 自动选择
// 线程数智能优化
```

---

## 文件统计

| 类别 | 数量 |
|------|------|
| C# 源文件 | 93 |
| Designer.cs | 13 |
| 资源文件总数 | 326 |
| 英雄头像 | ~165 |
| 装备图片 | 200+ |
| 阵容 JSON | ~4 |
