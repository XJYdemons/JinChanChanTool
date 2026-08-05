# -*- coding: utf-8 -*-
"""1) lineup_m16.json → JinChanChanTool RecommendedLineUps.json（用户自己的 S16 阵容）
   2) 关闭 metatft 自动更新（ManualSettings.json 开关）——避免启动拉取海外 API 卡死
"""
import json, os, shutil
from datetime import datetime

SRC = r"D:\vibecoding\金铲铲助手\jcc-desktop\data\s16"
LINEUP = r"D:\vibecoding\金铲铲助手\jcc-desktop\data\lineup_m16.json"
DST = r"D:\vibecoding\JinChanChanTool\SourceCode\JinChanChanTool\Resources\HeroDatas\S16英雄联盟传奇"
DST_BIN = r"D:\vibecoding\JinChanChanTool\SourceCode\JinChanChanTool\bin\Release\net8.0-windows10.0.17763.0\Resources\HeroDatas\S16英雄联盟传奇"
MANSET = r"D:\vibecoding\JinChanChanTool\SourceCode\JinChanChanTool\bin\Release\net8.0-windows10.0.17763.0\Resources\ManualSettings.json"
MANSET_SRC = r"D:\vibecoding\JinChanChanTool\SourceCode\JinChanChanTool\Resources\ManualSettings.json"

def load_js(name):
    with open(os.path.join(SRC, name), encoding='utf-8') as f:
        return json.load(f)

chess = load_js('chess.js')['data']
equip = load_js('equip.js')['data']
equip_name = {str(k): v.get('name', '') for k, v in equip.items()}

# 主英雄映射（与 convert_s16.py 同规则）
by_paint = {}
for v in chess.values():
    if v.get('heroType') != '0' or str(v.get('setid')) != '16':
        continue
    paint = v.get('heroPaint', '')
    if not paint:
        continue
    if paint not in by_paint or int(v['id']) < int(by_paint[paint]['id']):
        by_paint[paint] = v
hero_by_paint = {p: v['name'] for p, v in by_paint.items()}

def norm_hero(hid):
    hid = str(hid)
    v = chess.get(hid)
    if v:
        nm = hero_by_paint.get(v.get('heroPaint', ''))
        if nm:
            return nm
    return None

# ---------- 转换阵容 ----------
ld = json.load(open(LINEUP, encoding='utf-8'))['lineup_list']
lineups, skipped = [], []
for x in ld:
    try:
        det = json.loads(x['detail'])
    except Exception:
        skipped.append(x.get('id'))
        continue
    units = []
    for hl in (det.get('hero_location', []) or []):
        if hl.get('chess_type', 'hero') != 'hero':
            continue
        nm = norm_hero(hl.get('hero_id', ''))
        if nm is None:
            continue
        eqs = [equip_name.get(e, e) for e in str(hl.get('equipment_id', '')).split(',') if e]
        eqs = (eqs + ['', '', ''])[:3]
        loc = str(hl.get('location', '0,0')).split(',')
        try:
            r, c = int(loc[0]), int(loc[1])
        except Exception:
            r, c = 0, 0
        units.append({'HeroName': nm, 'EquipmentNames': eqs,
                      'Position': {'Item1': r, 'Item2': c}})
    if not units:
        skipped.append(x.get('id'))
        continue
    q = str(x.get('quality', 'A')).upper()
    if q not in ('S', 'A', 'B', 'C', 'D'):
        q = 'A'
    lineups.append({
        'LineUpName': det.get('line_name', ''),
        'LineUpUnits': units,
        'Tier': q, 'WinRate': 0.0, 'AverageRank': 0.0,
        'PickRate': 0.0, 'TopFourRate': 0.0, 'Tags': [], 'Description': '',
    })
print(f"转换阵容: {len(lineups)} 个（跳过 {len(skipped)}）")

out = {'UpdateTime': datetime.now().astimezone().isoformat(), 'LineUps': lineups}
with open(os.path.join(DST, 'RecommendedLineUps.json'), 'w', encoding='utf-8') as f:
    json.dump(out, f, ensure_ascii=False, indent=2)
shutil.copy2(os.path.join(DST, 'RecommendedLineUps.json'), os.path.join(DST_BIN, 'RecommendedLineUps.json'))
print("RecommendedLineUps.json 已写入源码 + bin")

# ---------- 关闭自动更新 ----------
def disable_autoupdate(path):
    if not os.path.exists(path):
        print(f"跳过（不存在）: {path}")
        return
    d = json.load(open(path, encoding='utf-8'))
    d['IsAutomaticUpdateEquipment'] = False
    d['IsAutomaticUpdateLineup'] = False
    with open(path, 'w', encoding='utf-8') as f:
        json.dump(d, f, ensure_ascii=False, indent=2)
    print(f"已关闭自动更新: {path}")

disable_autoupdate(MANSET)
disable_autoupdate(MANSET_SRC)
print("DONE")
