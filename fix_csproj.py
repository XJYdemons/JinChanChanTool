# -*- coding: utf-8 -*-
"""修复 csproj：HeroDatas 显式条目 → 通配符
1. 删除所有 <None Remove="Resources\HeroDatas\..."/> 
2. 删除所有 <None Update="Resources\HeroDatas\...">...</None>
3. 删除所有 <Content Include="Resources\HeroDatas\...">...</Content>
4. 添加通配符 None Remove + Content Include
"""
import re, shutil, sys

CS = r"D:\vibecoding\JinChanChanTool\SourceCode\JinChanChanTool\JinChanChanTool.csproj"
bak = CS + ".bak"
shutil.copy2(CS, bak)

with open(CS, encoding="utf-8-sig") as f:
    content = f.read()

before = content
# 1. None Remove 单行条目
content = re.sub(r'<None Remove="Resources\\HeroDatas\\[^"]*"\s*/>\s*', "", content)
# 2. None Update 块（含子元素）
content = re.sub(r'<None Update="Resources\\HeroDatas\\[^"]*">.*?</None>\s*', "", content, flags=re.S)
# 3. Content Include 块
content = re.sub(r'<Content Include="Resources\\HeroDatas\\[^"]*">.*?</Content>\s*', "", content, flags=re.S)

# 4. 插入通配符 ItemGroup（</Project> 前）
wildcard = """  <ItemGroup>
    <None Remove="Resources\\HeroDatas\\**\\*" />
    <Content Include="Resources\\HeroDatas\\**\\*">
      <CopyToOutputDirectory>Always</CopyToOutputDirectory>
    </Content>
  </ItemGroup>
"""
content = content.replace("</Project>", wildcard + "</Project>", 1)

with open(CS, "w", encoding="utf-8-sig", newline="") as f:
    f.write(content)

# 统计
print("删除 None Remove:", len(re.findall(r'<None Remove="Resources\\\\HeroDatas\\\\', before)) - len(re.findall(r'<None Remove="Resources\\\\HeroDatas\\\\', content)))
print("剩余 HeroDatas 显式条目:", len(re.findall(r'(?:<None|<Content) (?:Remove|Update|Include)="Resources\\\\HeroDatas\\\\[^"]*"(?:>|/>)', content)))
print("通配符就位:", 'Resources\\\\HeroDatas\\\\**\\\\*' in content)
print("备份:", bak)
