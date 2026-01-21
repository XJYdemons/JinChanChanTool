# JinChanChanTool

![Version](https://img.shields.io/badge/version-7.0.0-blue.svg)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey.svg)
![License](https://img.shields.io/badge/license-MIT-green.svg)
![.NET](https://img.shields.io/badge/.NET-Framework-purple.svg)

**一款专为《云顶之弈》和《金铲铲之战》设计的智能辅助工具**

[使用文档](#使用文档) • [更新记录](./Documents/更新记录.md) • [许可证](LICENSE) 

---

> 感谢你的Star，提需求、报BUG，最好能在拉Issues的同时，进QQ群954285837陈述详细内容。

## 项目简介

JinChanChanTool是一款基于图像识别技术的游戏辅助工具，通过OCR（光学字符识别）技术自动识别商店中的英雄卡牌，帮助玩家快速拿取目标英雄，让您的注意力从繁琐的D牌操作转移到阵容构筑和经济运营上。

### 核心工作原理

```
游戏画面 → 截图 → OCR识别 → 匹配目标英雄 → 模拟操作 → 完成拿牌
```

### 适用场景

- ✅ **追三星英雄**：快速搜索特定英雄，长按快捷键持续D牌
- ✅ **固定阵容上分**：保存常用阵容，一键应用，自动拿取核心卡
- ✅ **多阵容灵活转换**：保存多套备选阵容，根据场上情况快速切换

---

## 功能特性

### 自动拿牌

- 自动识别商店中的英雄卡牌
- 根据预设目标英雄自动拿取
- 支持鼠标模拟和按键模拟两种方式
- 长按快捷键持续D牌，松开自动停止

### 自动刷新商店

- 当商店没有目标英雄时自动刷新
- 配合自动拿牌实现全自动D牌

### 阵容管理

- 保存和管理多套阵容
- 每套阵容支持3个子阵容变体
- 一键切换和应用大数据网站的推荐阵容
- 支持阵容码导入导出

### 高亮显示

- 实时高亮显示商店中的目标英雄
- 可自定义高亮边框样式和颜色

### 批量选择英雄

- 按职业批量选择
- 按特质批量选择
- 快速组建阵容

### 装备推荐

- 查看每个英雄的推荐装备
- 数据来源于大数据网站
- 自动计算阵容所需散件数量

### 多场景适配

- 支持多显示器
- 支持任意DPI缩放
- 支持模拟器和云顶之弈
- 支持CPU和GPU推理（GPU速度提升10倍）

### 多赛季支持

- 内置多个赛季英雄池，可随时切换
- 支持自定义赛季英雄和装备数据
- 适配不同地区语言版本

---

## 快速开始

### 系统要求

| 项目 | 最低要求 | 推荐配置 |
|------|---------|---------|
| **操作系统** | Windows 10 (64位) | Windows 10/11 (64位) |
| **处理器** | Intel Core i3 或同等性能 | Intel Core i5 或更高 |
| **内存** | 4GB RAM | 8GB RAM 或更高 |
| **硬盘空间** | 1GB 可用空间 | 2GB 可用空间 |
| **显卡** | 集成显卡 | 英伟达独立显卡（使用GPU推理时） |

### 下载安装

#### 下载安装包

**方式1：从GitHub下载（推荐）**

1. 访问项目的 [Release 页面](https://github.com/XJYdemons/JinChanChanTool/releases)
2. 找到最新版本（如 `v7.0.0`）
3. 下载 `JinChanChanTool_vx.x.x_Windows_x64.zip` 文件

**方式2：从备用链接下载**

如果GitHub访问困难，可以使用备用下载链接：

- 百度网盘：https://pan.baidu.com/s/1fn-D8b7r7aC8Hb0Ka7wpaQ?pwd=k7mk

> **注意**：请从官方渠道下载，避免使用来源不明的安装包。

**方式3：通过QQ群文件下载**

通过添加QQ群954285837，获取群文件下载。

#### 解压与安装

JCCT是绿色软件，无需安装，解压即可使用。

**步骤**：

1. **选择安装位置**
   - 将下载的ZIP文件解压到您希望安装的目录
   - 推荐路径：`C:\Program Files\JinChanChanTool` 或 `D:\Games\JinChanChanTool`
   - 避免使用包含中文或特殊字符的路径

2. **解压文件**
   - 右键ZIP文件，选择"解压到当前文件夹"或"解压到指定文件夹"
   - 确保所有文件都被正确解压

> **提示**：如果您之前安装过旧版本，建议解压到新文件夹，避免文件冲突。

### 首次使用

首次启动软件后，会弹出配置向导窗口，引导您完成以下必要设置：

1. **设置坐标**：选择自动或手动设置游戏窗口坐标
2. **选择拿牌方式**：鼠标模拟或按键模拟
3. **选择刷新方式**：鼠标模拟或按键模拟
4. **选择OCR设备**：CPU推理或GPU推理

详细配置步骤请参阅 [第2章 开始使用](./Documents/第2章%20开始使用.md)

---

## 使用文档

完整的使用文档位于 `Documents` 目录：

- [第1章 概述](./Documents/第1章%20概述.md) - 软件介绍、硬件要求、安装方法
- [第2章 开始使用](./Documents/第2章%20开始使用.md) - 首次配置、功能使用
- [第3章 界面介绍](./Documents/第3章%20界面介绍.md) - 主窗口、各面板详解
- [第4章 自定义赛季信息](./Documents/第4章%20自定义赛季信息.md) - 自定义英雄和装备数据

---

## 界面预览

### 主窗口

![主窗口](./Documents/DocumentImages/image-20260121215709161.png)

### 配置向导

![配置向导](./Documents/DocumentImages/image-20260121215202503.png)

### 高亮显示效果

![高亮显示](./Documents/DocumentImages/image-20260121232343330.png)

---

## 常见问题

### Q: 软件能自动决策和自动对局吗？
A: 不能。本软件仅辅助拿牌操作，不会自动判断应该拿哪些英雄，也不会自动放置英雄到棋盘或合成装备。游戏的核心决策和策略仍需要您自己完成。

### Q: OCR识别准确率如何？
A: 识别准确率较高，但可能受画质、字体影响。少数情况下可能识别错误，软件提供了纠正机制来改善识别。

### Q: 支持移动端吗？
A: 不支持。仅支持Windows系统，但可以在模拟器上使用。

### Q: GPU推理如何配置？
A: 首次使用时在配置向导中选择GPU推理，程序会自动检测GPU环境并引导您完成配置。详细说明请参阅 [使用GPU推理OCR.md](./Documents/使用GPU推理OCR.md)

### Q: 如何自定义赛季英雄？

A: 请参阅 [第4章 自定义赛季信息](./Documents/第4章%20自定义赛季信息.md)，该文档详细介绍了如何编辑英雄和装备配置文件。

---

## 问题提交&联系方式

> 日志文件：`根目录/Logs`（提交问题时请附上日志）

如果您发现了Bug或有功能建议，请通过以下方式反馈：

- [提交Issue](https://github.com/XJYdemons/JinChanChanTool/issues)
- QQ交流群：954285837

---
## 技术栈

- **开发语言**：C# (.NET Framework)
- **OCR引擎**：PaddleOCR
- **图像处理**：OpenCV
- **UI框架**：Windows Forms
- **推理设备**：支持CPU和GPU（CUDA）

---

## Star History

如果这个项目对您有帮助，请给我们一个Star！

[![Star History Chart](https://api.star-history.com/svg?repos=XJYdemons/JinChanChanTool&type=Date)](https://star-history.com/#XJYdemons/JinChanChanTool&Date)

---

<div align="center">
</div>
