namespace JinChanChanTool.DataClass
{
    /// <summary>
    /// 阵容数据对象
    /// </summary>
    public class LineUp
    {
        /// <summary>
        /// 阵容名称
        /// </summary>
        public string LineUpName { get; set; } = "新阵容";


        /// <summary>
        /// 阵容状态
        /// </summary>
        public SubLineUp[] SubLineUps { get; set; } = [  new SubLineUp("前期"), new SubLineUp("中期"), new SubLineUp("后期") ];
      
        /// <summary>
        /// 对阵容命名的构造函数
        /// </summary>
        /// <param name="name"></param>
        public LineUp(string name)
        {
            LineUpName = name;            
        }

        public LineUp()
        { 

        }
    }

    /// <summary>
    /// 变阵对象，每个LineUp对象会有一个SubLineUp[]数组，容量为3，每个SubLineUp对象会有一个List<LineUpUnit>列表。
    /// </summary>
    public class SubLineUp
    {
        public string SubLineUpName { get; set; } = "新变阵";

        public List<LineUpUnit> LineUpUnits { get; set; } = [];
      
        /// <summary>
        /// 对变阵命名的构造函数
        /// </summary>
        /// <param name="name"></param>
        public SubLineUp(string name)
        {
            SubLineUpName = name;           
        }

        /// <summary>
        /// 返回最小阵容单位集合中是否包含目标英雄
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name)
        {           
            return LineUpUnits.Any(u => u.HeroName == name);
        }

        /// <summary>
        /// 从最小阵容单位集合中移除某个英雄
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Remove(string name)
        {            
            var unit = LineUpUnits.FirstOrDefault(u => u.HeroName == name);
            return unit != null && LineUpUnits.Remove(unit);
        }

        /// <summary>
        /// 根据传入的英雄名与装备数组在最小阵容单位集合中添加英雄
        /// </summary>
        /// <param name="heroName"></param>
        /// <param name="equipment"></param>
        /// <returns></returns>
        public bool Add(string heroName, string[] equipment)
        {
            if (Contains(heroName))
            {
                return false;
            }
            LineUpUnit newUnit = new LineUpUnit
            {
                HeroName = heroName,
                EquipmentNames = equipment
            };
            LineUpUnits.Add(newUnit);
            return true;
        }

        /// <summary>
        /// 根据传入的英雄名在最小阵容单位集合中添加英雄（默认无装备）
        /// </summary>
        /// <param name="heroName"></param>
        /// <param name="equipment"></param>
        /// <returns></returns>
        public bool Add(string heroName)
        {
            if (Contains(heroName))
            {
                return false;
            }
            LineUpUnit newUnit = new LineUpUnit
            {
                HeroName = heroName,
                EquipmentNames  = ["", "", ""]
            };
            LineUpUnits.Add(newUnit);
            return true;
        }
      
        /// <summary>
        /// 修改指定英雄的指定装备槽位
        /// </summary>
        /// <param name="heroName">英雄名称</param>
        /// <param name="equipmentIndex">装备槽位索引(0-2)</param>
        /// <param name="equipmentName">新装备名称</param>
        /// <returns>是否修改成功</returns>
        public bool SetEquipment(string heroName, int equipmentIndex, string equipmentName)
        {
            if (equipmentIndex is < 0 or > 2) return false;

            var unit = LineUpUnits.FirstOrDefault(u => u.HeroName == heroName);
            if (unit == null) return false;

            unit.EquipmentNames[equipmentIndex] = equipmentName;
            return true;

        }
    }

    /// <summary>
    /// 最小阵容单位 - 一个英雄及其装备
    /// </summary>
    public class LineUpUnit
    {
        public string HeroName { get; set; } = "";
        public string[] EquipmentNames { get; set; } = ["", "", ""];
        public (int, int) Position { get; set; } = (0, 0);
        public LineUpUnit(string heroName,string equipmentName1, string equipmentName2, string equipmentName3)
        {
            HeroName=heroName;
            EquipmentNames = [equipmentName1,equipmentName2, equipmentName3];
        }
        public LineUpUnit(string heroName, string equipmentName1, string equipmentName2, string equipmentName3,(int,int) position)
        {
            HeroName = heroName;
            EquipmentNames = [equipmentName1, equipmentName2, equipmentName3];
            Position = position;
        }
        public LineUpUnit()
        {
            
        }
    }
}
