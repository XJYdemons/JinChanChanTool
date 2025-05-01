!!!For submitting requirements or reporting bugs, it's recommended to join QQ group 954285837 for detailed communication while creating Issues.
# Jin Chan Chan Tools
An automated card-picking and shop-refreshing tool for "TFT: Teamfight Tactics" and "Jin Chan Chan Battle" Season 14.
## Table of Contents
1. [Introduction](#introduction)
2. [Features](#features)
3. [Installation](#installation)<br>
   3.1 [System Requirements](#system-requirements)<br>
   3.2 [Installation Steps](#installation-steps)
4. [Usage](#usage)<br>
   4.1 [First-time Use](#first-time-use)<br>
   4.2 [Auto-pick Cards](#auto-pick-cards)<br>
   4.3 [Auto-refresh Shop](#auto-refresh-shop)<br>
   4.4 [Save & Load Comps](#save--load-comps)<br>
   4.5 [Custom Hero Pool Configuration](#CustomHeroPoolConfiguration)<br>
5. [Additional Links](#AdditionalLinks)<br>
6. [FAQ](#FAQs)<br>

### Introduction
* Purpose: Automates card-picking operations during TFT/Jin Chan Chan matches.
* Target Users: TFT and Jin Chan Chan players
* Implementation:
  * Auto-pick: Uses screen capture, OCR processing, and text similarity matching to identify target cards, then simulates mouse clicks.
  * Auto-refresh: Simulates mouse clicks on shop refresh button.<br>

### Features
* Quick setup for in-game coordinates
* Auto-pick cards
* Auto-refresh shop
* Multi-monitor/DPI/resolution support
* Customizable conditions (gold thresholds, intervals)
* Global hotkeys (configurable)
* Always-on-top window with hide/show shortcut
* Customizable hero pool (names/avatars)<br>

### Installation
#### System Requirements
* OS: Windows 10+
* RAM: 4GB+
* Storage: 500MB+<br>

#### Installation Steps
1. Download latest release from [Release Page](https://github.com/XJYdemons/Jin-chan-chan-Tools/releases)
2. Extract "S14JinChanChanTools_vx.x.x_Windows_x64.zip"
3. Run `JinChanChan Assistant.exe` ​**as Administrator**​<br>

### Usage
#### First-time Use
1. Run as Administrator
2. Set monitor in Settings > Screenshot (skip for single monitor)

![Demo](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage1.png)

3. In-game setup:
   * "Quick Set Card Area" - Select name areas of 5 shop units (exclude prices)
   * "Quick Set Gold Area" - Select gold numbers (exclude icon)
   * "Quick Set Refresh Button" - Click refresh button location

![Demo](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage2.gif)

![Demo](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage3.png)

![Demo](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage4.png)

![Demo](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage5.png)

4. Set gold thresholds in Settings > Picking
5. (Optional) Adjust intervals (recommended ≥250ms)
6. (Optional) Configure hotkeys
7. Test functionality via Test > Monitor panel

![Demo](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage6.png)

8. Enjoy!<br>

#### Auto-pick Cards
1. Check target units
2. Click "Auto-pick" or use hotkey

![Demo](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage7.png)<br>

#### Auto-refresh Shop
1. Requires Auto-pick enabled
2. Click "Auto-refresh" or use hotkey

![Demo](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage8.png)<br>

#### Save & Load Comps
##### Save
1. Select comp from dropdown
2. Click "Save"<br>

##### Load
1. Select comp from dropdown

![Demo](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage9.gif)

![Demo](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage10.png)<br>

#### CustomHeroPoolConfiguration
> This feature allows users to manually update hero pools for other modes (e.g., Fortune Mode) and resolve hero name discrepancies in non-Chinese regions.
##### Step 1: Configure Hero Information
1. Navigate to the `Resources` folder in the software root directory (installation directory). Locate the `HeroInfo.json` file and open it with Notepad or a code editor (e.g., VS Code).

![Example Image](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage11.png)

2. The file structure consists of a pair of `[]` enclosing multiple hero data entries. Each hero entry strictly follows the format `{"HeroName": "Hero Name", "Cost": Hero Cost}`. Add a comma `,` after each entry (except the last one).
```
[
{"HeroName": "阿利斯塔","Cost": 1},{"HeroName": "波比","Cost": 1},{"HeroName": "艾克","Cost": 2},
{"HeroName": "崔斯特","Cost": 2},{"HeroName": "莫德凯撒","Cost": 3},{"HeroName": "赛娜","Cost": 3},
{"HeroName": "厄运小姐","Cost": 4},{"HeroName": "吉格斯","Cost": 4},{"HeroName": "可酷伯","Cost": 5},
{"HeroName": "雷克顿","Cost": 5}
]
```

3. Save the file after configuring all hero entries.<br>

##### Step 2: Configure Hero Avatars
1. In the software root directory, navigate to the `Resources` folder and open the `images` subfolder.

![Example Image](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage12.png)<br>
![Example Image](https://github.com/XJYdemons/Jin-chan-chan-Tools/blob/main/DemoImage/DemoImage13.png)<br>
2. Ensure every hero entry in `HeroInfo.json` has a corresponding `.png` avatar file with the ​**exact same name**​ as `HeroName`.<br>
Example:<br>
If `HeroInfo.json` contains:<br>
`{"HeroName": "阿利斯塔","Cost": 1}`, `{"HeroName": "吉格斯","Cost": 4}`, `{"HeroName": "可酷伯","Cost": 5}`,<br>
then `images` must include `阿利斯塔.png`, `吉格斯.png`, and `可酷伯.png`.<br>

### AdditionalLinks
* Alternative Download: https://pan.baidu.com/s/15h6dra2pCfw7HWEWE6h1Sg?pwd=sr3h<br>

### FAQs
​**Q1:​**​ Unable to pick heroes or use hotkeys.<br>
​**Solution:​**​ Run the program as an administrator.
