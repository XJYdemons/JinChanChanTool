# -*- coding: utf-8 -*-
"""把用户金铲铲助手 s16 数据转换为 JinChanChanTool S16 数据格式
输入: D:\vibecoding\金铲铲助手\jcc-desktop\data\s16\{chess,job,race,equip}.js + lineup_m16.json
输出: JinChanChanTool Resources/HeroDatas/S16英雄联盟传奇/
"""
import json, os, re, ssl, urllib.request
from collections import Counter, defaultdict
from datetime import datetime

SRC = r"D:\vibecoding\金铲铲助手\jcc-desktop\data\s16"
LINEUP = r"D:\vibecoding\金铲铲助手\jcc-desktop\data\lineup_m16.json"
DST = r"D:\vibecoding\JinChanChanTool\SourceCode\JinChanChanTool\Resources\HeroDatas\S16英雄联盟传奇"
DST_BIN = r"D:\vibecoding\JinChanChanTool\SourceCode\JinChanChanTool\bin\Release\net8.0-windows10.0.17763.0\Resources\HeroDatas\S16英雄联盟传奇"
PRICE_FIX = {'海克斯霸龙': 7}
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
    """拆 '293|300' -> ['斗士','护卫']，-1/空 -> []，去重保序"""
    out = []
    for x in str(s).split('|'):
        x = x.strip()
        if x and x != '-1':
            out.append(x)
    return out

# ---------- 英雄提取（对齐 build_data.py 规则） ----------
by_paint = {}
for v in chess.values():
    if v.get('heroType') != '0':
        continue
    if str(v.get('setid')) != '16':
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

heroes = []          # 主英雄（heroType=0）
hero_by_id = {}      # id -> hero
hero_by_paint = {}   # paint -> hero
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
    profs = [job_name.get(x, x) for x in clean_tags(v.get('class'))]
    pecs = [race_name.get(x, x) for x in clean_tags(v.get('species'))]
    profs = list(dict.fromkeys(profs))
    pecs = list(dict.fromkeys(pecs))
    h = {'HeroName': v['name'], 'Cost': price, 'Profession': profs,
         'Peculiarity': pecs, '_id': str(v['id']), '_paint': paint,
         '_picture': v.get('picture', '')}
    heroes.append(h)
    hero_by_id[h['_id']] = h
    hero_by_paint[h['_paint']] = h
heroes.sort(key=lambda h: (h['Cost'], h['HeroName']))
print(f"英雄: {len(heroes)}")

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
print(f"装备: {len(equips)}")

# ---------- 推荐装备（lineup_m16.json 统计，复刻 build_data.py heroEquip） ----------
def norm_hero(hid):
    hid = str(hid)
    if hid in hero_by_id:
        return hero_by_id[hid]
    v = chess.get(hid)
    if v:
        t = hero_by_paint.get(v.get('heroPaint', ''))
        if t:
            return t
    return None

equip_reco = defaultdict(Counter)
ld = json.load(open(LINEUP, encoding='utf-8'))['lineup_list']
for x in ld:
    try:
        det = json.loads(x['detail'])
    except Exception:
        continue
    for hl in (det.get('hero_location', []) or []):
        target = norm_hero(hl.get('hero_id', ''))
        if target is None:
            continue
        eqs = [e for e in str(hl.get('equipment_id', '')).split(',') if e]
        for e in eqs:
            equip_reco[target['_id']][e] += 1
hero_equip = {hid: [equip_name.get(e, e) for e, _c in cnt.most_common(3)]
              for hid, cnt in equip_reco.items()}
print(f"推荐装备英雄数: {len(hero_equip)}")

# ---------- 写 JSON ----------
hero_json = [{'HeroName': h['HeroName'], 'Cost': h['Cost'],
              'Profession': h['Profession'], 'Peculiarity': h['Peculiarity']} for h in heroes]
eq_json = [{k: v[k] for k in ('Name', 'EquipmentType') + (('SyntheticPathway',) if 'SyntheticPathway' in v else ())}
           for v in equips]
eqdata_json = {'UpdateTime': datetime.now().astimezone().isoformat(),
               'Data': {h['HeroName']: hero_equip.get(h['_id'], []) for h in heroes}}

os.makedirs(DST, exist_ok=True)
with open(os.path.join(DST, 'HeroData.json'), 'w', encoding='utf-8') as f:
    json.dump(hero_json, f, ensure_ascii=False, indent=2)
with open(os.path.join(DST, 'Equipment.json'), 'w', encoding='utf-8') as f:
    json.dump(eq_json, f, ensure_ascii=False, indent=2)
with open(os.path.join(DST, 'EquipmentData.json'), 'w', encoding='utf-8') as f:
    json.dump(eqdata_json, f, ensure_ascii=False, indent=2)
print("JSON 已写入", DST)

# ---------- 图片同步 ----------
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

new_hero_imgs, reused_hero, dl_hero = 0, 0, 0
for h in heroes:
    if h['HeroName'] in existing_imgs:
        reused_hero += 1
        continue
    if download(h['_picture'], os.path.join(img_dir, h['HeroName'] + '.png')):
        new_hero_imgs += 1
    else:
        dl_hero += 1
new_eq_imgs, reused_eq, dl_eq = 0, 0, 0
for e in equips:
    if e['Name'] in existing_eqimgs:
        reused_eq += 1
        continue
    if download(e['_picture'], os.path.join(eqimg_dir, e['Name'] + '.png')):
        new_eq_imgs += 1
    else:
        dl_eq += 1
# 删除多余图片
old_names = {h['HeroName'] for h in heroes}
for f in os.listdir(img_dir):
    if f.rsplit('.', 1)[0] not in old_names:
        os.remove(os.path.join(img_dir, f))
        print(f"  删除多余英雄图: {f}")
old_eqnames = {e['Name'] for e in equips}
for f in os.listdir(eqimg_dir):
    if f.rsplit('.', 1)[0] not in old_eqnames:
        os.remove(os.path.join(eqimg_dir, f))
        print(f"  删除多余装备图: {f}")
print(f"英雄图: 复用{reused_hero} 新下载{new_hero_imgs} 失败{dl_hero} | 装备图: 复用{reused_eq} 新下载{new_eq_imgs} 失败{dl_eq}")

# ---------- 同步到 bin 输出目录 ----------
import shutil
for sub in ('HeroData.json', 'Equipment.json', 'EquipmentData.json'):
    shutil.copy2(os.path.join(DST, sub), os.path.join(DST_BIN, sub))
    print(f"同步 bin: {sub}")
if os.path.isdir(os.path.join(DST_BIN, 'images')):
    shutil.rmtree(os.path.join(DST_BIN, 'images'))
if os.path.isdir(os.path.join(DST_BIN, 'EquipmentImages')):
    shutil.rmtree(os.path.join(DST_BIN, 'EquipmentImages'))
shutil.copytree(os.path.join(DST, 'images'), os.path.join(DST_BIN, 'images'))
shutil.copytree(os.path.join(DST, 'EquipmentImages'), os.path.join(DST_BIN, 'EquipmentImages'))
print("bin 图片目录已同步")
