# 给 JinChanChanTool 原作者提 PR 完整流程（新手版）

> 前提：本地已 clone 原作者仓库并完成修改（你的情况正是如此），需要把改动提交给原作者合并。

## 一、准备工作

1. **注册 GitHub 账号**：https://github.com/signup （如果还没有）
2. **登录 GitHub**

## 二、Fork 原仓库（网页操作，1 分钟）

1. 打开原作者仓库：https://github.com/XJYdemons/JinChanChanTool
2. 点右上角 **Fork** 按钮 → 确认
3. 完成后你账号下会出现一份副本：`https://github.com/<你的账号>/JinChanChanTool`

## 三、本地仓库关联到你的 Fork

在本地项目目录打开终端（git-bash），执行：

```bash
cd /d/vibecoding/JinChanChanTool

# 1. 把 origin 指向你自己的 fork（原来指向原作者）
git remote set-url origin https://github.com/<你的账号>/JinChanChanTool.git

# 2. 添加 upstream 指向原作者（以后同步原作者的更新用）
git remote add upstream https://github.com/XJYdemons/JinChanChanTool.git

# 3. 确认
git remote -v
# 应显示：origin=你的fork，upstream=原作者
```

## 四、创建 PR 分支并推送

```bash
# 1. 从当前 main 创建 PR 分支（名字随意，如 pr-优化改进）
git checkout -b pr-优化改进

# 2. 推送分支到你的 fork
git push origin pr-优化改进
```

> 推送时会要求登录：用户名填你的 GitHub 账号，密码处填 **Personal Access Token**（不能用账号密码）：
> GitHub 头像 → Settings → Developer settings → Personal access tokens → Generate new token（勾选 `repo` 权限）→ 复制 token 备用。不想每次都输，可以配置 credential helper。

## 五、发起 PR（网页操作）

1. 打开 `https://github.com/<你的账号>/JinChanChanTool`，会看到黄色提示条「pr-优化改进 … Compare & pull request」→ 点击
2. 确认对比方向：**base: XJYdemons/JinChanChanTool:main ← compare: <你的账号>/JinChanChanTool:pr-优化改进**
3. 粘贴 PR 描述（用 `Documents/PR描述模板.md` 里的模板）
4. 点 **Create pull request**

## 六、PR 提交后

- **等待作者 review**：作者可能会评论、要求修改
- **作者要求修改时**：
  ```bash
  cd /d/vibecoding/JinChanChanTool
  git checkout pr-优化改进
  # 修改代码...
  git add .
  git commit -m "按 review 意见修改 xxx"
  git push origin pr-优化改进
  ```
  推送后 PR 自动更新，无需重新创建
- **作者合并后**：PR 显示 Merged，你的改动进入原作者仓库

## 七、以后同步原作者的新更新

```bash
# 拉取原作者最新代码并合并到本地
git fetch upstream
git checkout main
git merge upstream/main
```

## 常见问题

| 问题 | 解决 |
|---|---|
| 推送时提示没有权限/认证失败 | 确认用的是 Token 不是密码；token 是否勾选了 repo 权限 |
| fork 后想更新 fork | 网页上 fork 仓库点 Sync fork，或本地 fetch upstream 后 push |
| 想拆多个 PR | 每个 PR 建一个分支：数据更新、bug 修复、新功能分开，分别 push 分别发起 PR |
| PR 里混入了不想提交的文件 | 检查 `git status`，用 `git rm --cached <文件>` 排除后重新 commit |

## 小贴士

- **第一次提 PR 建议用一个小改动跑通流程**（比如只提数据更新），成功后再提其他
- 提 PR 前先在 README 的 QQ 群 954285837 跟作者打声招呼，成功率更高
- 如果作者长时间没回应（项目可能不活跃），改动保留在自己 fork 即可，不影响你使用
