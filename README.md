> 感谢你的Star，提需求、报BUG，最好能在拉Issues的同时，进QQ群954285837陈述详细内容。
# Jin Chan Chan Tools
适用于游戏“金铲铲之战”与“云顶之奕”S14赛季的自动化拿牌、刷新商店的工具。
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
   4.4 [保存阵容与使用阵容](#保存阵容与使用阵容)<br>
   4.5 [用户自定义英雄卡池](#用户自定义英雄卡池)<br>
5. [其他链接](#其他链接)<br>
6. [问题](#问题)<br>
### 简介
* 项目的用途：本项目可帮助用户在进行金铲铲/云顶之奕对局时解放双手，实现拿牌自动化。
* 目标用户：金铲铲之战、云顶之战玩家
* 项目实现原理：
  * 自动拿牌：通过截取目标图片，处理后再进行OCR（光学字符识别）的方式转化为文本，通过文本相似度对比判断是否为目标卡牌。如果是目标卡牌，则模拟鼠标操作：改变鼠标位置-按下鼠标左键-抬起鼠标左键 拿取卡牌。
  * 自动刷新商店：通过模拟鼠标操作：改变鼠标位置-按下鼠标左键-抬起鼠标左键 刷新商店。<br>
### 功能特点：
* 快捷设置游戏内的截图位置：无需用户手动测量输入游戏内牌库位置、商店刷新按钮位置，提供快速设置功能。
* 自动拿牌：开启该功能后，用户勾选好需要的奕子后，将自动拿取。
* 自动刷新商店：开启该功能后，当前商店没有目标奕子时，会自动刷新商店，配合自动拿牌使用。
* 适配多显示器、任意DPI（缩放方式）、任意分辨率：对于多显示器用户，本项目在启动时将会检测用户已连接显示器，并加载到下拉框供用户选择。
* 对于自动拿牌、自动刷新商店、召出/隐藏窗口，提供系统级全局快捷键快捷键功能，保证在游戏内不会被拦截，并可以手动修改快捷键。
* 程序默认窗口置顶并可以通过快捷键隐藏/召出，因此在游戏中无需切屏就可使用各项功能。
* 支持根据职业或特质一键批量选择英雄。
* 每个阵容都配有三套子阵容槽位，方便灵活变阵。
* 支持配置文件内用户自定义英雄选择器的英雄头像、名字、所属阵容：
   * 该功能用途之一是能支持用户自行更新赛季英雄。
   * 该功能用途之二是能支持用户根据所处地区客户端语言进行本地化支持。
   * 该功能用途之三是能支持用户自定义英雄头像。<br>
### 安装：
#### 系统要求
* 操作系统：Windows10或更高版本
* 内存：4GB或以上
* 硬盘空间：500MB或以上<br>
#### 安装步骤
1. 前往[Release 页面](https://github.com/XJYdemons/Jin-chan-chan-Tools/releases) 下载最新的安装包"S14JinChanChanTools_vx.x.x_Windows_x64.zip"。
2. 下载压缩包文件后，解压到一个目录中。
3. 以**管理员身份**运行`金铲铲助手.exe`。<br>
### 使用：
#### 首次使用
1. 以**管理员身份**运行`金铲铲助手.exe`。
2. 在顶部菜单栏-设置-截图-选择显示器，选择用于显示游戏窗口的那台显示器（单显示器用户忽略该步骤）

![示例图片](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage1.png)

3. 打开游戏，进入对局
   * 点击-“快速设置奕子截图坐标与大小”，分别框选商店从左到右的5个奕子名称（需要将奕子名称行用矩形包裹，但不要包裹奕子价格），
   * 点击-“快速设置商店刷新按钮坐标”，将鼠标放置到用于刷新商店的按钮上，单击鼠标左键即可设置成功。

![示例图片](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage2.gif)

![示例图片](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage3.png)

![示例图片](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage5.png)

4. （可选）设置-快捷键，可以查看当前功能的快捷键，支持修改所有快捷键。<br>
#### 功能-自动拿牌
1. 点击英雄头像框或勾选其下方的单选框可以选择要自动拿取的奕子，当所有希望自动拿取的奕子完成勾选后，可按“自动拿牌”按钮开启该功能，此时，若商店出现目标奕子，则会自动拿取。

![示例图片](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage7.png)<br>

#### 功能-自动刷新商店
此功能需要在开启“自动拿牌”功能的同时，开启该功能才有效。
1. 开启自动拿牌功能。
2. 单击“自动刷新商店”按钮开启该功能，开始后，当商店不含有目标奕子时，将会自动刷新商店。

![示例图片](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage8.png)<br>

#### 保存阵容与使用阵容
##### 保存阵容
1. 通过“选择奕子”功能区右上角的“选择阵容”下拉框选择想要保存到的阵容，单击阵容名称可以为阵容改名。
2. 点击“保存”按钮即可将当前阵容保存到当前所选阵容内。<br>
##### 使用阵容
1. 通过“选择奕子”功能区右上角的“选择阵容”下拉框选择想要使用的阵容即可。

![示例图片](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage9.gif)

![示例图片](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage10.png)<br>

#### 用户自定义英雄卡池
> 此功能可供用户自行更新、其他模式英雄池（例如福星）以及解决非中文使用地区的英雄名称差异问题。
##### 第一步：配置英雄信息
1. 在软件根目录（软件的安装目录）下，找到Resources文件夹，找到其中的HeroInfo.json文件，使用记事本或其他代码编辑器（例如Vs Code）打开。

![示例图片](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage11.png)

2. 打开该文件后，格式如下：该文件结构是以一对`[]`包裹数个英雄数据结构，每个英雄数据的格式都严格遵循`{{"HeroName": "英雄名字","Cost": 英雄费用},"Profession": "职业1|职业2","Peculiarity": "特质1|特质2"
}`，在前一个英雄数据结构后增加一个需要先加一个英文逗号`,`.(若已是文件中最后一个英雄数据结构则无需加逗号)
```
[
{"HeroName": "阿利斯塔","Cost": 1,"Profession": "斗士", "Peculiarity": "福牛守护者"},
{"HeroName": "波比","Cost": 1,"Profession": "堡垒卫士", "Peculiarity": "赛博老大"},
{"HeroName": "千珏","Cost": 1,"Profession": "迅捷射手|强袭射手", "Peculiarity": "魔装机神"},
{"HeroName": "崔斯特","Cost": 2, "Profession": "迅捷射手","Peculiarity": "辛迪加"},
{"HeroName": "莫德凯撒","Cost": 3,"Profession": "高级工程师|斗士","Peculiarity": "源计划"},
{"HeroName": "赛娜","Cost": 3, "Profession": "杀手","Peculiarity": "圣灵使者"},
{"HeroName": "厄运小姐","Cost": 4, "Profession": "人造人","Peculiarity": "辛迪加"},
{"HeroName": "吉格斯","Cost": 4, "Profession": "战略分析师","Peculiarity": "赛博老大"},
{"HeroName": "可酷伯","Cost": 5, "Profession": "斗士","Peculiarity": "赛博老大"},
{"HeroName": "雷克顿","Cost": 5,  "Profession": "堡垒卫士", "Peculiarity": "鳄霸|圣灵使者"}
]
```
3. 所有英雄数据结构配置完成后，保存文件即可。<br>

##### 第二步：配置英雄头像
1. 在软件根目录（软件的安装目录）下，找到Resources文件夹，找到其中images文件夹，打开该文件夹。

![示例图片](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage12.png)<br>
![示例图片](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage13.png)<br>
2. 该文件夹用于存放英雄头像图片，确保HeroInfo.json文件中的每一个英雄项都有对应的同名图片（格式要求是.png）<br>
举例：<br>
假设HeroInfo.json中有三个英雄结构:<br>
分别是 `{"HeroName": "阿利斯塔","Cost": 1,"Profession": "斗士", "Peculiarity": "福牛守护者"}` `{"HeroName": "吉格斯","Cost": 4, "Profession": "战略分析师","Peculiarity": "赛博老大"}` `{"HeroName": "可酷伯","Cost": 5, "Profession": "斗士","Peculiarity": "赛博老大"}`<br>
那么在images中就应该存在三个英雄头像图片文件`阿利斯塔.png`、`吉格斯.png`、`可酷伯.png`.<br>

### 其他链接
* 备用下载链接：https://pan.baidu.com/s/15h6dra2pCfw7HWEWE6h1Sg?pwd=sr3h<br>

### 问题
**问题1：** 无法拿牌、快捷键失效等。<br>
**解决：** 以管理员身份运行程序。
