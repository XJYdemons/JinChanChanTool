# -*- coding: utf-8 -*-
"""通用阵容转换：用户 lineup JSON → JinChanChanTool RecommendedLineUps.json
用法: python convert_lineups_generic.py <mode> <src_dir> <lineup_file> <dst_season>
例:   python convert_lineups_generic.py 17 s17 lineup_m17.json "S17"
      python convert_lineups_generic.py 8  s18 lineup_total_m18.json "怪兽入侵(金铲铲)"
"""
import json, os, shutil, sys
from datetime import datetime

MODE, SRC_DIR, LINEUP, DST_NAME = sys.argv[1], sys.argv[2], sys.argv[3], sys.argv[4]
SRC = os.path.join(r"D:\vibecoding\金铲铲助手\jcc-desktop\data", SRC_DIR)
LINEUP_PATH = os.path.join(r"D:\vibecoding\金铲铲助手\jcc-desktop\data", LINEUP)
DST = os.path.join(r"D:\vibecoding\JinChanChanTool\SourceCode\JinChanChanTool\Resources\HeroDatas", DST_NAME)
DST_BIN = os.path.join(r"D:\vibecoding\JinChanChanTool\SourceCode\JinChanChanTool\bin\Release\net8.0-windows10.0.17763.0\Resources\HeroDatas", DST_NAME)

def load_js(name):
    with open(os.path.join(SRC, name), encoding='utf-8') as f:
        return json.load(f)

chess = load_js('chess.js')['data']
equip = load_js('equip.js')['data']
equip_name = {str(k): v.get('name', '') for k, v in equip.items()}

# 主英雄映射（与 convert_season.py 同规则）
by_paint = {}
for v in chess.values():
    if v.get('heroType') != '0' or str(v.get('setid')) != MODE:
        continue
    paint = v.get('heroPaint', '')
    if not paint:
        continue
    if paint not in by_paint or int(v['id']) < int(by_paint[paint]['id']):
        by_paint[paint] = v
hero_by_paint = {p: v['name'] for p, v in by_paint.items()}

def norm_hero(hid):
    v = chess.get(str(hid))
    if v:
        nm = hero_by_paint.get(v.get('heroPaint', ''))
        if nm:
            return nm
    return None

ld = json.load(open(LINEUP_PATH, encoding='utf-8'))['lineup_list']
lineups, skipped = [], []

def convert_units(hl_list):
    """hero_location 列表 → LineUpUnit 列表"""
    units = []
    for hl in hl_list or []:
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
    return units

for x in ld:
    try:
        det = json.loads(x['detail'])
    except Exception:
        skipped.append(x.get('id'))
        continue
    full_units = convert_units(det.get('hero_location'))
    if not full_units:
        skipped.append(x.get('id'))
        continue
    # 变阵：前期/中期/后期（后期=完整阵容 hero_location，推荐卡片只显示后期）
    sub_units = [
        convert_units(det.get('y21_early_heros')),
        convert_units(det.get('y21_metaphase_heros')),
        full_units,
    ]
    q = str(x.get('quality', 'A')).upper()
    if q not in ('S', 'A', 'B', 'C', 'D'):
        q = 'A'
    lineups.append({
        'LineUpName': det.get('line_name', ''),
        'LineUpUnits': full_units,
        'SubLineUps': [{'LineUpUnits': u} for u in sub_units],
        'Tier': q, 'WinRate': 0.0, 'AverageRank': 0.0,
        'PickRate': 0.0, 'TopFourRate': 0.0, 'Tags': [], 'Description': '',
    })
print(f"[{DST_NAME}] 转换阵容: {len(lineups)} 个（跳过 {len(skipped)}）")

out = {'UpdateTime': datetime.now().astimezone().isoformat(), 'LineUps': lineups}
os.makedirs(DST, exist_ok=True)
os.makedirs(DST_BIN, exist_ok=True)
with open(os.path.join(DST, 'RecommendedLineUps.json'), 'w', encoding='utf-8') as f:
    json.dump(out, f, ensure_ascii=False, indent=2)
shutil.copy2(os.path.join(DST, 'RecommendedLineUps.json'), os.path.join(DST_BIN, 'RecommendedLineUps.json'))
print(f"[{DST_NAME}] 已写入源码 + bin")
