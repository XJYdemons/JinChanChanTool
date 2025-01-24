# S13金铲铲助手
适用于游戏“金铲铲之战”与“云顶之奕”S13赛季的自动化拿牌、刷新商店、刷新异常突变的工具。
## 目录
1. [简介](#简介)
2. [功能特点](#功能特点)
3. [安装](#安装)<br>
   3.1 [系统要求](#系统要求)<br>
   3.2 [安装步骤](#安装步骤)
4. [使用](#使用)<br>
   4.1 [首次使用](#首次使用)<br>
   4.2 [功能-自动拿牌](#功能-自动拿牌)<br>
   4.3 [功能-自动刷新商店](#功能-自动刷新商店)<br>
   4.4 [功能-自动D异变](#功能-自动D异变)<br>
   4.5 [保存阵容与使用阵容](#保存阵容与使用阵容)<br>
5. [开发者指南](#开发者指南)<br>
   5.1 [获取项目源码文件](#获取项目源码文件)<br>
   5.2 [开发环境配置](#开发环境配置)<br>
   5.3 [项目结构](#项目结构)<br>
   5.4 [运行项目](#运行项目)<br>
   5.5 [构建应用](#构建应用)<br>
   5.6 [常见问题](#常见问题)<br>
6. [项目引用](#项目引用)
### 简介
* 项目的用途：本项目可帮助用户在进行金铲铲/云顶之奕对局时解放双手，实现拿牌自动化。
* 目标用户：金铲铲之战、云顶之战玩家
* 项目实现原理：
  * 自动拿牌：通过截取目标图片，处理后再进行OCR（光学字符识别）的方式转化为文本，通过文本相似度对比判断是否为目标卡牌。如果是目标卡牌，则模拟鼠标操作：改变鼠标位置-按下鼠标左键-抬起鼠标左键 拿取卡牌。
  * 自动刷新商店：通过模拟鼠标操作：改变鼠标位置-按下鼠标左键-抬起鼠标左键 刷新商店。
  * 自动D异变：通过截取目标图片，处理后再进行OCR（光学字符识别）的方式转化为文本，通过文本相似度对比判断是否为目标异变。如果是目标异变，则模拟鼠标操作：改变鼠标位置-按下鼠标左键-抬起鼠标左键 拿取异变。
### 功能特点：
* 自动拿牌：开启该功能后，用户勾选好需要的奕子后，将自动拿取。
* 自动刷新商店：开启该功能后，当前商店没有目标奕子时，会自动刷新商店，配合自动拿牌使用。
* 自动拿/刷新异常突变：开启该功能后，会自动拿取用户希望拿取的异变，若当且商店没有目标异变，则会自动刷新商店。
* 适配多显示器，任意分辨率游戏窗口：对于多显示器用户，本项目在启动时将会检测用户已连接显示器，并加载到下拉框供用户选择。
* 可自定义的拿牌条件：用户可自定义金币高于多少时开始拿牌、低于多少时停止拿牌、拿牌的间隔、商店刷新的间隔、异常突变刷新的间隔。
* 对于自动拿牌、自动刷新商店、自动D异变功能，提供快捷键功能，并可以手动修改快捷键。
### 安装：
#### 系统要求
* 操作系统：Windows10或更高版本
* 内存：4GB或以上
* 硬盘空间：500MB或以上
#### 安装步骤
1. 前往[Release 页面](https://github.com/XJYdemons/Jin-chan-chan-Tools/releases) 下载最新的安装包[S13JinChanChanTools_v2.0_Windows_x64.zip](https://github.com/XJYdemons/Jin-chan-chan-Tools/releases/download/%E5%8F%91%E8%A1%8C%E7%89%88/S13JinChanChanTools_v2.0_Windows_x64.zip)。
2. 下载压缩包文件后，解压到一个目录中。
3. 运行`金铲铲助手.exe`。
### 使用：
#### 首次使用
1. 运行`金铲铲助手.exe`。
2. 在右侧 设置-截图-选择显示器，选择显示游戏窗口的显示器（单显示器用户忽略该步骤）
![image](https://github.com/user-attachments/assets/d03d61fa-962a-4759-bfd0-fc9e270dc80a)
3. 打开游戏，进入对局，如图：
![image](https://github.com/user-attachments/assets/0928c237-1d69-4045-93ca-65c78d8e1938)
4. 点击“快速设置奕子截图坐标与大小”，分别框选商店从左到右的5个奕子名称，如图（需要将奕子名称行用矩形包裹，但不要包裹奕子价格）：
![image](https://github.com/user-attachments/assets/b3ec1969-9f2d-4305-9c16-854c2cb6d833)
5. 点击“快速设置金币截图坐标与大小”，框选商店显示的金币数字（注意不要框选到左侧的金币图标），如图：
![image](https://github.com/user-attachments/assets/aaefacc8-75eb-407a-ab0e-93f1e0f7a723)
6. 点击“快速设置异常突变截图坐标与大小”，将鼠标放置在商店的刷新按钮上，单击鼠标左键即可成功设置。
7. 点击“快速设置异常突变截图坐标与大小”，框选商店用于显示异常图标名称的那行，如图：
![image](https://github.com/user-attachments/assets/847704e0-fa62-43e5-9787-f6502e48ddd4)
8. 点击“快速设置异常突变刷新按钮坐标”，将鼠标放置在异常突变的刷新按钮上，单击鼠标左键即可成功设置。
9. 点击“快速设置异常突变进化按钮坐标”，将鼠标放置在异常突变的进化按钮上，单击鼠标左键即可成功设置。
10. 来到设置-刷新相关，在“低于【】金币时自动停止刷新商店”处填入具体数值，当您持有的金币数小于等于左侧填入的数值时，将不会自动拿牌与刷新商店。
11. 来到设置-操作间隔时间，可自定义修改拿牌间隔、商店刷新间隔和异常突变刷新间隔，其中商店刷新间隔不推荐低于250毫秒，否则会出现无法正常识别拿牌的bug。
12. 来到设置-快捷键，在这里可以自定义功能快捷键。
#### 功能-自动拿牌
1. 在应用左侧，勾选需要自动拿取奕子头像下的单选框，当所有希望自动拿取的奕子完成勾选后，可按“自动拿牌”按钮开启该功能，此时，若商店出现目标奕子，则会自动拿取。
#### 功能-自动刷新商店
此功能需要在开启“自动拿牌”功能的同时，开启该功能才有效。
1. 开启自动拿牌功能。
2. 单击“自动刷新商店”按钮开启该功能，开始后，当商店不含有目标奕子时，将会自动刷新商店。
#### 功能-自动D异变
1. 在程序左侧功能区选择“异常突变”选项卡。
2. 在该选项卡的文本框内输入希望自动拿取的异常突变（若存在多个则用“|”分隔），如图：
![image](https://github.com/user-attachments/assets/749af672-4359-4046-9b85-6b44157b30c2)
3. 也可以通过文本框下方的按钮来快速添加异常突变。
4. 将所有希望自动拿取的异常突变添加完成后，单击“自动D异常突变”按钮。
5. 若商店此时存在目标“异常图标”，程序会单击“进化”按钮并自动关闭该功能；若此时商店不存在目标“异常突变”，程序会单击“刷新”按钮来刷新异常突变，该步骤会循环，直到出现目标异常突变为止。
#### 保存阵容与使用阵容
##### 保存阵容
1. 通过“选择奕子”功能区右上角的“选择阵容”下拉框选择想要保存到的阵容，单击阵容名称可以为阵容改名。
2. 点击“保存”按钮即可将当前阵容（包括目标奕子与异常突变）保存到当前所选阵容内。
##### 使用阵容
1.通过“选择奕子”功能区右上角的“选择阵容”下拉框选择想要使用的阵容即可。
### 开发者指南
#### 获取项目源码文件
1. 前往[Release 页面](https://github.com/XJYdemons/Jin-chan-chan-Tools/releases) 下载最新的源码[S13JinChanChanTools_v2.0_SourceCode.zip](https://github.com/XJYdemons/Jin-chan-chan-Tools/releases/download/%E5%8F%91%E8%A1%8C%E7%89%88/S13JinChanChanTools_v2.0_SourceCode.zip))。
2. 下载压缩包文件后，解压到一个目录中。
#### 开发环境配置
* IDE：Visual Studio 2022
* 必需组件：<br>
  * .NET 8.0 SDK
  * Windows SDK
#### 项目结构
 ```
├── /bin                         # 编译后的输出目录
├── /obj                         # 中间文件目录，包含编译时生成的临时文件
├── /Properties                  # 项目属性文件夹
├── /runtimes                    # Emgu.CV运行时库目录
├── AppConfig.cs                 # 应用程序配置相关的 C# 文件
├── ControlTools.cs              # 负责控制鼠标模拟操作的工具类
├── Emgu.CV.Bitmap.dll           # Emgu CV 的 Bitmap 支持库
├── Emgu.CV.dll                  # Emgu CV 核心功能库
├── Emgu.CV.Platform.NetCore.dll # Emgu CV 的 .NET Core 平台支持库
├── Form1.cs                     # 主窗体代码文件
├── Form1.Designer.cs            # 自动生成的窗体设计文件
├── Form1.resx                   # 主窗体的资源文件
├── ImageProcessingTools.cs      # 图像处理工具类
├── libiomp5md.dll               # Intel OpenMP 支持库
├── mkldnn.dll                   # MKL-DNN 动态链接库，用于深度学习加速
├── mkml.dll                     # MKL 相关库文件
├── OCRTools.cs                  # OCR 功能工具类
├── opencv_world470.dll          # OpenCV 4.7.0 主动态链接库
├── paddle_inference.dll         # PaddlePaddle 推理库
├── Program.cs                   # 应用程序入口点
├── tbb12.dll                    # Intel TBB 动态链接库，用于多线程支持
├── TextProcessingTools.cs       # 文本处理工具类
├── 金铲铲助手.csproj             # 项目文件，用于描述 C# 项目
├── 金铲铲助手.csproj.user        # 用户特定的项目配置文件
└── 金铲铲助手.sln                # 解决方案文件
```
#### 运行项目
1. 使用 Visual Studio 打开 .sln 文件。
2. 选择`Debug-X64`模式，如图：
![image](https://github.com/user-attachments/assets/fd6e3eb3-fba4-491b-858d-31bbcb1c816d)
3. 点击运行，如图：
![image](https://github.com/user-attachments/assets/7d8f8914-e673-4db2-a88b-be51801b06c8)
#### 构建应用
1. 使用 Visual Studio 打开金铲铲助手.sln 文件。
2. 选择`Release-X64`模式，如图：
 ![image](https://github.com/user-attachments/assets/8f07fb4b-3465-43d7-90ab-d01a01ac385d)
3. 点击 **生成** > **生成解决方案**。如图：
![image](https://github.com/user-attachments/assets/393a6e30-4869-4d16-9fb8-80f9c0df1314)
4. 构建的可执行文件将保存在 bin\x64\Release\net8.0-windows 文件夹下。
#### 常见问题
**问题1：** 运行或生成解决方案时报错：无法处理文件 Form1.resx，因为它位于 Internet 或受限区域中，或者文件上具有 Web 标记。要想处理这些文件，请删除 Web 标记。如图：
![image](https://github.com/user-attachments/assets/1bbd0ee4-021d-4902-80da-dd1909b7e919)
**解决：**
1. 先关闭VisualStudio。
2. 来到项目根目录，找到文件“Form1.resx”，右键菜单-属性。
3. 在下方“安全”-“此文件来自其他计算机，可能被阻止以帮助保护该计算机”处，勾选“解除锁定”，并点击应用按钮。如图：
![image](https://github.com/user-attachments/assets/3117b2c3-e027-4546-9db6-77b13ac13b7c)
4. 再次用VisualStudio打开金铲铲助手.sln 文件。
### 项目引用：
* [Paddle OCR Sharp](https://github.com/raoyutian/PaddleOCRSharp)
* [Emgu.CV](https://github.com/emgucv/emgucv)

