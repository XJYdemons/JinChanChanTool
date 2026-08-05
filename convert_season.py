# -*- coding: utf-8 -*-
"""通用赛季转换：用户金铲铲助手数据 → JinChanChanTool 赛季目录
用法: python convert_season.py <mode> <src_dir> <lineup_file|-> <dst_dir> [season_display]
例:   python convert_season.py 17 s17 lineup_m17.json S17
      python convert_season.py 8  s18 lineup_total_m18.json "怪兽入侵(金铲铲)"
      python convert_season.py 1  s1  lineup_m1.json "时空裂痕"
"""
import json, os, re, ssl, sys, shutil, urllib.request
from collections import Counter, defaultdict
from datetime import datetime

MODE, SRC_DIR, LINEUP, DST_NAME = sys.argv[1], sys.argv[2], sys.argv[3], sys.argv[4]
SRC = os.path.join(r"D:\vibecoding\金铲铲助手\jcc-desktop\data", SRC_DIR)
LINEUP_PATH = os.path.join(r"D:\vibecoding\金铲铲助手\jcc-desktop\data", LINEUP) if LINEUP != '-' else None
DST = os.path.join(r"D:\vibecoding\JinChanChanTool\SourceCode\JinChanChanTool\Resources\HeroDatas", DST_NAME)
DST_BIN = os.path.join(r"D:\vibecoding\JinChanChanTool\SourceCode\JinChanChanTool\bin\Release\net8.0-windows10.0.17763.0\Resources\HeroDatas", DST_NAME)
PRICE_FIX = {'16': {'海克斯霸龙': 7}}.get(MODE, {})
TYPE_MAP = {'基础装备': '散件', '成型装备': '普通装备', '光明武器': '光明装备',
            '神器装备': '奥恩神器', '转职纹章': '转职纹章', '特殊装备': '特殊装备'}

def load_js(name):
    with open(os.path.join(SRC, name), encoding='utf-8') as f:
        return json.load(f)

chess = load_js('chess.js')['data']
job = load_js('job.js')['data']
race = load_js('race.js')['data']
equip = load_js('equip.js')['data']
job_name = {str(k): v.get('name', '') for k, v in job.items()}
race_name = {str(k): v.get('name', '') for k, v in race.items()}
equip_name = {str(k): v.get('name', '') for k, v in equip.items()}

def clean_tags(s):
    out = []
    for x in str(s).split('|'):
        x = x.strip()
        if x and x != '-1':
            out.append(x)
    return out

# ---------- 英雄 ----------
by_paint = {}
for v in chess.values():
    if v.get('heroType') != '0' or str(v.get('setid')) != MODE:
        continue
    paint = v.get('heroPaint', '')
    if not paint:
        continue
    if paint not in by_paint:
        by_paint[paint] = {'v': v, 'stars': {str(v['id']): v}}
    else:
        by_paint[paint]['stars'][str(v['id'])] = v
by_name = {}
for paint, grp in sorted(by_paint.items()):
    v = grp['v']
    nm = v['name']
    if nm not in by_name:
        by_name[nm] = (paint, grp)
    else:
        if int(v['id']) < int(by_name[nm][1]['v']['id']):
            by_name[nm] = (paint, grp)

heroes, hero_by_id, hero_by_paint = [], {}, {}
for paint, grp in by_name.values():
    v = grp['v']
    sp = int(v.get('sellPrice', -1) or -1)
    price = sp if sp > 0 else int(v.get('price', 0) or 0)
    if price > 7:
        price = int(v.get('price', 5) or 5)
    if v['name'] in PRICE_FIX:
        price = PRICE_FIX[v['name']]
    if price == 0 and v.get('name') != '提伯斯':
        continue
    profs = list(dict.fromkeys(job_name.get(x, x) for x in clean_tags(v.get('class'))))
    pecs = list(dict.fromkeys(race_name.get(x, x) for x in clean_tags(v.get('species'))))
    h = {'HeroName': v['name'], 'Cost': price, 'Profession': profs, 'Peculiarity': pecs,
         '_id': str(v['id']), '_paint': paint, '_picture': v.get('picture', '')}
    heroes.append(h)
    hero_by_id[h['_id']] = h
    hero_by_paint[h['_paint']] = h
heroes.sort(key=lambda h: (h['Cost'], h['HeroName']))
print(f"[{DST_NAME}] 英雄: {len(heroes)}")

# ---------- 装备 ----------
equips = []
for v in equip.values():
    if str(v.get('type', '')) == '-1':
        continue
    syn = []
    for s in (v.get('synthesis1'), v.get('synthesis2')):
        s = str(s or '0')
        if s != '0' and s in equip_name:
            syn.append(equip_name[s])
    e = {'Name': v['name'], 'EquipmentType': TYPE_MAP.get(v.get('type', ''), v.get('type', '')),
         '_id': str(v['id']), '_picture': v.get('picture', '')}
    if syn:
        e['SyntheticPathway'] = syn
    equips.append(e)
print(f"[{DST_NAME}] 装备: {len(equips)}")

# ---------- 推荐装备（阵容统计） ----------
equip_reco = defaultdict(Counter)
if LINEUP_PATH and os.path.exists(LINEUP_PATH):
    def norm_hero(hid):
        hid = str(hid)
        if hid in hero_by_id:
            return hero_by_id[hid]
        v = chess.get(hid)
        if v:
            return hero_by_paint.get(v.get('heroPaint', ''))
        return None
    try:
        ld = json.load(open(LINEUP_PATH, encoding='utf-8')).get('lineup_list', [])
    except Exception as ex:
        ld = []
        print(f"  阵容读取失败: {ex}")
    for x in ld:
        try:
            det = json.loads(x['detail'])
        except Exception:
            continue
        for hl in (det.get('hero_location', []) or []):
            target = norm_hero(hl.get('hero_id', ''))
            if target is None:
                continue
            for e in str(hl.get('equipment_id', '')).split(','):
                if e:
                    equip_reco[target['_id']][e] += 1
    hero_equip = {hid: [equip_name.get(e, e) for e, _c in cnt.most_common(3)]
                  for hid, cnt in equip_reco.items()}
    print(f"[{DST_NAME}] 推荐装备英雄数: {len(hero_equip)}")
else:
    hero_equip = {}
    print(f"[{DST_NAME}] 无阵容数据，推荐装备为空")

# ---------- 写 JSON ----------
os.makedirs(DST, exist_ok=True)
hero_json = [{'HeroName': h['HeroName'], 'Cost': h['Cost'],
              'Profession': h['Profession'], 'Peculiarity': h['Peculiarity']} for h in heroes]
eq_json = [{k: v[k] for k in ('Name', 'EquipmentType') + (('SyntheticPathway',) if 'SyntheticPathway' in v else ())}
           for v in equips]
eqdata_json = {'UpdateTime': datetime.now().astimezone().isoformat(),
               'Data': {h['HeroName']: hero_equip.get(h['_id'], []) for h in heroes}}
with open(os.path.join(DST, 'HeroData.json'), 'w', encoding='utf-8') as f:
    json.dump(hero_json, f, ensure_ascii=False, indent=2)
with open(os.path.join(DST, 'Equipment.json'), 'w', encoding='utf-8') as f:
    json.dump(eq_json, f, ensure_ascii=False, indent=2)
with open(os.path.join(DST, 'EquipmentData.json'), 'w', encoding='utf-8') as f:
    json.dump(eqdata_json, f, ensure_ascii=False, indent=2)

# ---------- 图片 ----------
ctx = ssl.create_default_context()
ctx.check_hostname = False
ctx.verify_mode = ssl.CERT_NONE

def download(url, dest):
    if not url or os.path.exists(dest):
        return False
    try:
        req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0'})
        with urllib.request.urlopen(req, timeout=20, context=ctx) as r, open(dest, 'wb') as f:
            f.write(r.read())
        return True
    except Exception as ex:
        print(f"  下载失败 {url}: {ex}")
        return False

img_dir = os.path.join(DST, 'images')
eqimg_dir = os.path.join(DST, 'EquipmentImages')
os.makedirs(img_dir, exist_ok=True)
os.makedirs(eqimg_dir, exist_ok=True)
existing_imgs = {i.rsplit('.', 1)[0] for i in os.listdir(img_dir)}
existing_eqimgs = {i.rsplit('.', 1)[0] for i in os.listdir(eqimg_dir)}
reused_h = sum(1 for h in heroes if h['HeroName'] in existing_imgs)
new_h = sum(1 for h in heroes if h['HeroName'] not in existing_imgs and download(h['_picture'], os.path.join(img_dir, h['HeroName'] + '.png')))
reused_e = sum(1 for e in equips if e['Name'] in existing_eqimgs)
new_e = sum(1 for e in equips if e['Name'] not in existing_eqimgs and download(e['_picture'], os.path.join(eqimg_dir, e['Name'] + '.png')))
for f in os.listdir(img_dir):
    if f.rsplit('.', 1)[0] not in {h['HeroName'] for h in heroes}:
        os.remove(os.path.join(img_dir, f))
for f in os.listdir(eqimg_dir):
    if f.rsplit('.', 1)[0] not in {e['Name'] for e in equips}:
        os.remove(os.path.join(eqimg_dir, f))
print(f"[{DST_NAME}] 英雄图: 复用{reused_h} 新下{new_h} | 装备图: 复用{reused_e} 新下{new_e}")

# ---------- bin 同步 ----------
os.makedirs(DST_BIN, exist_ok=True)
for sub in ('HeroData.json', 'Equipment.json', 'EquipmentData.json'):
    shutil.copy2(os.path.join(DST, sub), os.path.join(DST_BIN, sub))
for d in ('images', 'EquipmentImages'):
    dd = os.path.join(DST_BIN, d)
    if os.path.isdir(dd):
        shutil.rmtree(dd)
    shutil.copytree(os.path.join(DST, d), dd)
print(f"[{DST_NAME}] bin 已同步")
print(f"[{DST_NAME}] DONE")
