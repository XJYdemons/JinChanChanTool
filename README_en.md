# Jin Chan Chan Tools
Automation tool for "Jin Chan Chan" and "Teamfight Tactics" S13 season to automatically pick cards, refresh shops, and refresh mutations.

## Table of Contents
1. [Introduction](#introduction)
2. [Features](#features)
3. [Installation](#installation)<br>
   3.1 [System Requirements](#system-requirements)<br>
   3.2 [Installation Steps](#installation-steps)
4. [Usage](#usage)<br>
   4.1 [First Use](#first-use)<br>
   4.2 [Feature - Auto Pick Cards](#feature-auto-pick-cards)<br>
   4.3 [Feature - Auto Refresh Shop](#feature-auto-refresh-shop)<br>
   4.4 [Feature - Auto D Mutation](#feature-auto-d-mutation)<br>
   4.5 [Save and Use Lineup](#save-and-use-lineup)<br>
5. [Developer Guide](#developer-guide)<br>
   5.1 [Getting the Project Source Code](#getting-the-project-source-code)<br>
   5.2 [Development Environment Setup](#development-environment-setup)<br>
   5.3 [Project Structure](#project-structure)<br>
   5.4 [Running the Project](#running-the-project)<br>
   5.5 [Building the Application](#building-the-application)<br>
   5.6 [FAQ](#faq)<br>
6. [Project References](#project-references)

### Introduction
* **Purpose**: This project helps users automate the card-picking process in "Jin Chan Chan" and "Teamfight Tactics" to save time during gameplay.
* **Target Audience**: "Jin Chan Chan" and "Teamfight Tactics" players.
* **Project Principle**:
  * **Auto Pick Cards**: By capturing target images, processing them, and using OCR (Optical Character Recognition), the text is converted, and similarity comparison is performed to determine if the target card is present. If it's the target card, mouse simulation is used: move the mouse, click, and release the left mouse button to pick the card.
  * **Auto Refresh Shop**: Uses mouse simulation to refresh the shop when no target card is available.
  * **Auto D Mutation**: Similar to auto pick cards, it uses OCR to recognize mutations and automatically performs the mutation action if the target mutation is present.

### Features:
* **Auto Pick Cards**: Once this feature is enabled, the tool will automatically pick the selected units once they appear in the shop.
* **Auto Refresh Shop**: When enabled, if the shop does not contain any target units, the tool will automatically refresh the shop in conjunction with auto pick cards.
* **Auto Pick/Refresh Mutations**: If the target mutation is not present in the shop, the tool will automatically refresh the shop until the mutation appears, and then pick it.
* **Multi-Monitor & Resolution Support**: The tool automatically detects connected monitors and allows users to select the game display monitor (single monitor users can ignore this step).
* **Customizable Card-Picking Conditions**: Users can set conditions such as the minimum amount of gold for auto-picking, stopping conditions, intervals between picking, refreshing, and mutation actions.
* **Hotkeys**: Hotkeys can be customized for the auto-pick cards, auto-refresh shop, and auto D mutation functions.

### Installation:
#### System Requirements
* **Operating System**: Windows 10 or higher.
* **Memory**: 4GB or more.
* **Disk Space**: 500MB or more.

#### Installation Steps
1. Visit the [Release Page](https://github.com/XJYdemons/Jin-chan-chan-Tools/releases) and download the latest installation package [S13JinChanChanTools_v2.0_Windows_x64.zip](https://github.com/XJYdemons/Jin-chan-chan-Tools/releases/download/%E5%8F%91%E8%A1%8C%E7%89%88/S13JinChanChanTools_v2.0_Windows_x64.zip).
2. After downloading the zip file, extract it to a directory.
3. Run `JinChanChanAssistant.exe`.

### Usage:
#### First Use
1. Run `JinChanChanAssistant.exe`.
2. In the right settings, select the monitor displaying the game window (single monitor users can skip this step).
   ![image](https://github.com/user-attachments/assets/d03d61fa-962a-4759-bfd0-fc9e270dc80a)
3. Open the game and enter a match as shown in the image:
   ![image](https://github.com/user-attachments/assets/0928c237-1d69-4045-93ca-65c78d8e1938)
4. Click "Quick Set Card Screenshot Coordinates & Size", and select the five card names from left to right in the shop.
   ![image](https://github.com/user-attachments/assets/b3ec1969-9f2d-4305-9c16-854c2cb6d833)
5. Click "Quick Set Gold Screenshot Coordinates & Size" and select the gold number shown in the shop.
   ![image](https://github.com/user-attachments/assets/aaefacc8-75eb-407a-ab0e-93f1e0f7a723)
6. Click "Quick Set Mutation Screenshot Coordinates & Size" and place the mouse over the shop refresh button and click to set.
7. Click "Quick Set Mutation Refresh Button Coordinates" and select the mutation icon name in the shop.
   ![image](https://github.com/user-attachments/assets/847704e0-fa62-43e5-9787-f6502e48ddd4)
8. Click "Quick Set Mutation Evolution Button Coordinates" and select the mutation evolution button in the shop.
9. Go to settings-refresh-related, enter a specific value in "Stop refreshing when gold is lower than【】", so the tool won't automatically pick cards and refresh the shop when your gold is equal to or lower than the set value.
10. Go to settings-operation interval time and customize the pick interval, shop refresh interval, and mutation refresh interval. It’s not recommended to set the shop refresh interval lower than 250ms to avoid bugs.
11. Go to settings-hotkeys to customize hotkeys for functions.

#### Feature - Auto Pick Cards
1. In the left section, select the target units under "Auto Pick Cards" and click the "Auto Pick" button to start the function. If the target card appears in the shop, it will be automatically picked.

#### Feature - Auto Refresh Shop
This function works in conjunction with Auto Pick Cards.
1. Enable Auto Pick Cards.
2. Click the "Auto Refresh Shop" button, and when no target cards are available, the shop will refresh automatically.

#### Feature - Auto D Mutation
1. In the program's left section, select the "Mutation" tab.
2. Enter the desired mutation(s) in the text box (use "|" to separate multiple mutations).
   ![image](https://github.com/user-attachments/assets/749af672-4359-4046-9b85-6b44157b30c2)
3. Add mutations using the button below the text box.
4. Once all desired mutations are added, click the "Auto D Mutation" button.
5. If the target mutation is found in the shop, the tool will click the "evolution" button and automatically disable the function. If not, it will click the "refresh" button to refresh mutations, repeating the process until the target mutation is found.

#### Save and Use Lineup
##### Save Lineup
1. Select a lineup name from the "Select Lineup" dropdown and click "Save" to store the current lineup (target units and mutations).

##### Use Lineup
1. Select the desired lineup from the dropdown to load and use it.

### Developer Guide
#### Getting the Project Source Code
1. Visit the [Release Page](https://github.com/XJYdemons/Jin-chan-chan-Tools/releases) to download the latest source code [S13JinChanChanTools_v2.0_SourceCode.zip](https://github.com/XJYdemons/Jin-chan-chan-Tools/releases/download/%E5%8F%91%E8%A1%8C%E7%89%88/S13JinChanChanTools_v2.0_SourceCode.zip).
2. Extract the zip file to a directory.

#### Development Environment Setup
* **IDE**: Visual Studio 2022
* **Required Components**:
  * .NET 8.0 SDK
  * Windows SDK

#### Project Structure


#### Running the Project
1. Open the `.sln` file in Visual Studio.
2. Select `Debug-X64` mode, as shown:
   ![image](https://github.com/user-attachments/assets/fd6e3eb3-fba4-491b-858d-31bbcb1c816d)
3. Click run, as shown:
   ![image](https://github.com/user-attachments/assets/7d8f8914-e673-4db2-a88b-be51801b06c8)

#### Building the Application
1. Open `JinChanChanAssistant.sln` in Visual Studio.
2. Select `Release-X64` mode, as shown:
   ![image](https://github.com/user-attachments/assets/8f07fb4b-3465-43d7-90ab-d01a01ac385d)
3. Click **Build** > **Build Solution**. As shown:
   ![image](https://github.com/user-attachments/assets/393a6e30-4869-4d16-9fb8-80f9c0df1314)
4. The compiled executable will be saved in the `bin\x64\Release\net8.0-windows` folder.

#### FAQ
**Problem 1:** Error when running or building the solution: Unable to process `Form1.resx` because it is from the Internet or a restricted zone, or it has a web tag. To process this file, remove the web tag. As shown:
![image](https://github.com/user-attachments/assets/1bbd0ee4-021d-4902-80da-dd1909b7e919)

**Solution**:
1. Close Visual Studio.
2. Go to the project root directory and right-click `Form1.resx`, select Properties.
3. Under "Security" at the bottom, check "Unblock" and click Apply.
   ![image](https://github.com/user-attachments/assets/3117b2c3-e027-4546-9db6-77b13ac13b7c)
4. Reopen the `JinChanChanAssistant.sln` file in Visual Studio.

### Project References:
* [Paddle OCR Sharp](https://github.com/raoyutian/PaddleOCRSharp)
* [Emgu.CV](https://github.com/emgucv/emgucv)

