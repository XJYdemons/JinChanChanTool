using Emgu.CV.Structure;
using Emgu.CV;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Diagnostics;

namespace 金铲铲助手
{
    public partial class Form1 : Form
    {
        #region 变量与常量声明
        private bool switch_GetCard;//拿牌开关
        private bool switch_RefreshStore;//刷新商店开关
        private bool switch_RefreshMutation;//刷新异常突变开关
        private bool isMoneyRecognized;//是否识别到金币
        private bool isWindowExpandedHorizontally;//主窗口是否横向展开
        private bool isWindowExpandedVertically;//主窗口是否纵向展开
        private bool isTheAsynRunning; //异步任务是否运行

        private string hotKeyText1;//快捷键1的显示文本
        private string hotKeyText2;//快捷键2的显示文本
        private string hotKeyText3;//快捷键3的显示文本

        private int countOfMoney_StartRefreshStore;//开始刷新商店所需的金币数
        private int countOfMoney_StopRefreshStore;//停止刷新商店所需的金币数
        private int intervalTime_GetCard;//每次拿牌后的等待时间
        private int intervalTime_StoreRefresh;//每次商店刷新后的等待时间
        private int intervalTime_Mutation;//每次刷新异常突变后的等待时间
        private int countOfMoney;//持有的金币数
        private int dropDownBoxItem_CurrentlySelected;//现行选中下拉框项
        private int startPoint_CardScreenshotX1;//卡牌截图起始坐标X1
        private int startPoint_CardScreenshotX2;//卡牌截图起始坐标X2
        private int startPoint_CardScreenshotX3;//卡牌截图起始坐标X3
        private int startPoint_CardScreenshotX4;//卡牌截图起始坐标X4
        private int startPoint_CardScreenshotX6;//卡牌截图起始坐标X5
        private int startPoint_CardScreenshotY6;//卡牌截图起始坐标Y
        private int width_CardScreenshot;//卡牌截图宽度
        private int height_CardScreenshot;//卡牌截图高度
        private int startPoint_MoneyScreenshotX;//金币截图起点坐标X
        private int startPoint_MoneyScreenshotY;//金币截图起点坐标Y
        private int width_MoneyScreenshot;//金币截图宽度
        private int height_MoneyScreenshot;//金币截图高度
        private int startPoint_MutationScreenshotX;//异常突变截图起始坐标X
        private int startPoint_MutationScreenshotY;//异常突变截图起始坐标Y
        private int width_MutationScreenshot;//异常突变截图宽度
        private int height_MutationScreenshot;//异常突变截图高度
        private int Point_RefreshMutationX;//异常突变刷新按钮坐标X
        private int Point_RefreshMutationY;//异常突变刷新按钮坐标Y
        private int Point_EvolutionMutationX;//异常突变进化按钮坐标X
        private int Point_EvolutionMutationY;//异常突变进化按钮坐标Y
        private int Point_RefreshStoreX;//商店刷新按钮坐标X
        private int Point_RefreshStoreY;//商店刷新按钮坐标Y
        private int Index_CurrentlySelectedDisplay;//当前选择的显示器序号

        private const int HOTKEY_1 = 1;  // 自动拿牌快捷键的唯一标识符
        private const int HOTKEY_2 = 2;  // 自动刷新商店快捷键的唯一标识符
        private const int HOTKEY_3 = 3;  //自动D异变快捷键的唯一标识符
        private const int intervalTime_MouseMove = 20;//鼠标移动后等待的时间
        private const int intervalTime_MouseDown = 10;//鼠标按下后等待的时间
        private const int width_WindowExpanded = 790;  // 窗体展开时的宽度
        private const int width_WindowContracted = 435; // 窗体收缩时的宽度
        private const int height_WindowExpanded = 807;  // 窗体展开时的宽度
        private const int height_WindowContracted = 627; // 窗体收缩时的宽度

        private CancellationTokenSource cancellationTokenSource;//控制拿牌D牌多线程操作的变量
        private Screen targetScreen;//目标显示器

        private bool[] CheckList;//勾选表
        Screen[] screens;//显示器数组
        private string[] cardName;//奕子名称数组       
        private CheckBox[] CheckBoxs;//单选框数组
        #endregion
        #region 初始化变量
        /// <summary>
        /// 初始化变量值
        /// </summary>
        private void Initialize_Variable()
        {

            switch_GetCard = false;//将拿牌开关设为false
            switch_RefreshStore = false;//将刷新商店开关设为false
            switch_RefreshMutation = false;//将D异变开关设为false
            isMoneyRecognized = false;
            isWindowExpandedHorizontally = true;//窗口默认设置为横向展开
            isWindowExpandedVertically = true;//窗口默认设置为纵向展开
            isTheAsynRunning = false;//异步任务默认关闭

            countOfMoney = 0;//将初始金币数设置为0
            dropDownBoxItem_CurrentlySelected = 0;//设置选择阵容下拉框当前选中项为0
            cancellationTokenSource = new CancellationTokenSource();

            screens = Screen.AllScreens;// 获取所有连接的显示器

            targetScreen = screens[0];//默认选择显示器0

            comboBox1.SelectedIndex = 0;//选择阵容下拉框默认选择第一个阵容
            CheckList = new bool[63];
            for (int i = 0; i < CheckList.Length; i++)
            {
                CheckList[i] = false;
            }

            cardName = new string[63]
            {
            "阿木木","艾瑞莉娅","爆爆","德莱厄斯","德莱文","婕拉","拉克丝","麦迪","莫甘娜","斯特卜","特朗德尔","薇古丝","蔚奥莱","辛吉德",
            "阿卡丽","崔丝塔娜","厄加特","范德尔","弗拉基米尔","吉格斯","卡密尔","蕾欧娜","烈娜塔","魔腾","芮尔","瑟提","泽丽",
            "布里茨","崔斯特","刀疤","卡西奥佩娅","克格莫","洛里斯","娜美","努努和威朗普","普朗克","荏妮","史密奇","斯维因","伊泽瑞尔",
            "艾克","安蓓萨","俄洛伊","盖伦","黑默丁格","库奇","蒙多医生","图奇","蔚","希尔科","伊莉丝","佐伊",
            "杰斯","金克斯","凯特琳","兰博","乐芙兰","玛尔扎哈","莫德凯撒","塞薇卡",
            "梅尔","维克托","沃里克",
             };

            CheckBoxs = new CheckBox[63]
             {
            checkBox1, checkBox2, checkBox3, checkBox4, checkBox5, checkBox6, checkBox7, checkBox8, checkBox9, checkBox10,
            checkBox11, checkBox12, checkBox13, checkBox14, checkBox15, checkBox16, checkBox17, checkBox18, checkBox19, checkBox20,
            checkBox21, checkBox22, checkBox23, checkBox24, checkBox25, checkBox26, checkBox27, checkBox28, checkBox29, checkBox30,
            checkBox31, checkBox32, checkBox33, checkBox34, checkBox35, checkBox36, checkBox37, checkBox38, checkBox39, checkBox40,
            checkBox41, checkBox42, checkBox43, checkBox44, checkBox45, checkBox46, checkBox47, checkBox48, checkBox49, checkBox50,
            checkBox51, checkBox52, checkBox53, checkBox54, checkBox55, checkBox56, checkBox57, checkBox58, checkBox59, checkBox60,
            checkBox61, checkBox62, checkBox63
             };
        }
        #endregion
        #region 组件初始化函数       
        /// <summary>
        /// 初始化组件
        /// </summary>
        private void Initialize_AllComponents()
        {
            Initialize_TextBox1();
            Initialize_TextBox2();
            Initialize_TextBox3();
            Initialize_TextBox4();
            Initialize_TextBox6();
            Initialize_TextBox7();
            Initialize_TextBox8();
            Initialize_TextBox9();
            Initialize_TextBox10();
            Initialize_TextBox11();
            Initialize_TextBox12();
            Initialize_TextBox13();
            Initialize_TextBox14();
            Initialize_TextBox15();
            Initialize_TextBox16();
            Initialize_TextBox17();
            Initialize_TextBox18();
            Initialize_TextBox19();
            Initialize_TextBox20();
            Initialize_TextBox21();
            Initialize_TextBox22();
            Initialize_TextBox23();
            Initialize_TextBox25();
            Initialize_TextBox26();
            Initialize_TextBox27();
            Initialize_TextBox28();
            Initialize_TextBox29();
            Initialize_TextBox30();
            Initialize_TextBox31();
            Initialize_TextBox32();
            Initialize_Timer1();
            Initialize_Timer2();
            Initialize_Timer3();
            Initialize_CheckBoxs();
            LoadDisplays();
            Initialize_AllMutationButton();
            #region 为小问号图标设置 ToolTip

            toolTip1.SetToolTip(pictureBox64, "每次拿牌后等待的时间。（单位：毫秒）");
            toolTip1.SetToolTip(pictureBox65, "每次商店刷新之后等待的时间。（单位：毫秒）");
            toolTip1.SetToolTip(pictureBox66, "每次刷新异常突变之前等待的时间。（单位：毫秒）");
            toolTip1.SetToolTip(pictureBox67, "勾选后开启该功能：当检测到的持有金币数小于左侧填入的数值时，将不会自动拿牌与刷新商店，直到持有金币达到左侧填入数值，此时将会取消勾选该功能，并可以正常拿牌与刷新商店。");
            toolTip1.SetToolTip(pictureBox68, "当持有的金币数小于等于左侧填入的数值时，将不会自动拿牌与刷新商店。\n默认值：2");
            toolTip1.SetToolTip(pictureBox69, "当连接多台显示器时，该选项生效。\n选择截图目标（游戏窗口）所在的显示器。\n默认值：1 - \\\\.\\DISPLAY1");

            toolTip1.SetToolTip(pictureBox70, "需要填入6个数值，分别是X1-X5与Y。" +
                "\n程序使用OCR（光学字符识别）来匹配目标是否是指定奕子，因此需要对商店奕子的名称部分进行截图操作，这批数值决定了该在哪开始截图。" +
                                             "\nX1：代表商店从左到右数第1张奕子名称的截图起始点的横坐标，截图左顶点处为起始点,水平向右为X正轴，垂直向下为Y正轴。" +
                                             "\nX2：代表商店从左到右数第2张奕子名称的截图起始点的横坐标，截图左顶点处为起始点,水平向右为X正轴，垂直向下为Y正轴。" +
                                             "\nX3：代表商店从左到右数第3张奕子名称的截图起始点的横坐标，截图左顶点处为起始点,水平向右为X正轴，垂直向下为Y正轴。" +
                                             "\nX4：代表商店从左到右数第4张奕子名称的截图起始点的横坐标，截图左顶点处为起始点,水平向右为X正轴，垂直向下为Y正轴。" +
                                             "\nX5：代表商店从左到右数第5张奕子名称的截图起始点的横坐标，截图左顶点处为起始点,水平向右为X正轴，垂直向下为Y正轴。" +
                                             "\nY：代表商店所有（5张）奕子名称的截图起始点的纵坐标，截图左顶点处为起始点,水平向右为X正轴，垂直向下为Y正轴。"
                                             );
            toolTip1.SetToolTip(pictureBox71, "宽和高的值表示从截图起点（图片左顶点）向右与向下分别截取的像素值。\n" +
                "宽：从截图起点向右截取图片的宽度。" +
                "\n高：从截图起点向下截取图片的高度。");
            toolTip1.SetToolTip(pictureBox72, "为了实时获取到当前对局您所持有的金币数，我们需要将显示金币数的文本进行截图，然后使用OCR（光学字符识别）转化为文本显示," +
                "\n因此，我们需要获取到截图的起点坐标（图片的左顶点坐标）。" +
                "\nX：代表截图起始点的横坐标，截图左顶点处为起始点,水平向右为X正轴，垂直向下为Y正轴。" +
                "\nY：代表截图起始点的纵坐标，截图左顶点处为起始点,水平向右为X正轴，垂直向下为Y正轴。");
            toolTip1.SetToolTip(pictureBox73, "宽和高的值表示从截图起点（图片左顶点）向右与向下分别截取的像素值。\n" +
                "宽：从截图起点向右截取图片的宽度。" +
                "\n高：从截图起点向下截取图片的高度。");
            toolTip1.SetToolTip(pictureBox74, "为了获取到您当前商店显示的异常突变，我们需要对显示异常突变名称的文本进行截图，然后使用OCR（光学字符识别）转化为文本显示," +
                "\n因此，我们需要获取到截图的起点坐标（图片的左顶点坐标）。" +
                "\nX：代表截图起始点的横坐标，截图左顶点处为起始点,水平向右为X正轴，垂直向下为Y正轴。" +
                "\nY：代表截图起始点的纵坐标，截图左顶点处为起始点,水平向右为X正轴，垂直向下为Y正轴。");
            toolTip1.SetToolTip(pictureBox75, "宽和高的值表示从截图起点（图片左顶点）向右与向下分别截取的像素值。\n" +
                "宽：从截图起点向右截取图片的宽度。" +
                "\n高：从截图起点向下截取图片的高度。");
            toolTip1.SetToolTip(pictureBox76, "本程序刷新异常突变使用鼠标模拟移动点击的方式，因此我们要获取到鼠标移动的目标位置，即异常\n突变刷新按钮的中心坐标。（当然也可以是按钮的其他部位，不过中心部位是最不容易点击失误的地方）");
            toolTip1.SetToolTip(pictureBox77, "本程序选择异常突变使用鼠标模拟移动点击的方式，因此我们要获取到鼠标移动的目标位置，即异常\n突变进化按钮的中心坐标。（当然也可以是按钮的其他部位，不过中心部位是最不容易点击失误的地方）");
            toolTip1.SetToolTip(pictureBox78, "本程序刷新商店使用鼠标模拟移动点击的方式，因此我们要获取到鼠标移动的目标位置，即商店刷新\n按钮的中心坐标。（当然也可以是按钮的其他部位，不过中心部位是最不容易点击失误的地方）");
            #endregion
        }
        private void Initialize_Timer1()
        {
            timer1.Tick += Timer1_Tick;
        }
        private void Initialize_Timer2()
        {
            timer2.Tick += Animation_HorizontalShrink;

        }
        private void Initialize_Timer3()
        {

            timer3.Tick += Animation_VerticalShrink;
        }
        private void Initialize_TextBox1()
        {
            textBox1.KeyDown += TextBox1_KeyDown;
            textBox1.Enter += TextBox1_Enter;
            textBox1.Leave += TextBox1_Leave;

        }
        private void Initialize_TextBox2()
        {
            textBox2.KeyDown += TextBox2_KeyDown;
            textBox2.Enter += TextBox2_Enter;
            textBox2.Leave += TextBox2_Leave;
        }
        private void Initialize_TextBox3()
        {
            textBox3.KeyDown += TextBox3_KeyDown;
            textBox3.Enter += TextBox3_Enter;
            textBox3.Leave += TextBox3_Leave;


        }
        private void Initialize_TextBox4()
        {
            textBox4.KeyDown += TextBox4_KeyDown;
            textBox4.Enter += TextBox4_Enter;
            textBox4.Leave += TextBox4_Leave;


        }
        private void Initialize_TextBox6()
        {
            textBox6.KeyDown += TextBox6_KeyDown;
            textBox6.Enter += TextBox6_Enter;
            textBox6.Leave += TextBox6_Leave;


        }
        private void Initialize_TextBox7()
        {
            textBox7.KeyDown += TextBox7_KeyDown;
            textBox7.Enter += TextBox7_Enter;
            textBox7.Leave += TextBox7_Leave;


        }
        private void Initialize_TextBox8()
        {
            textBox8.KeyDown += TextBox8_KeyDown;
            textBox8.Enter += TextBox8_Enter;
            textBox8.Leave += TextBox8_Leave;
        }
        private void Initialize_TextBox9()
        {
            textBox9.KeyDown += TextBox9_KeyDown;
            textBox9.Enter += TextBox9_Enter;
            textBox9.Leave += TextBox9_Leave;
        }
        private void Initialize_TextBox10()
        {
            textBox10.KeyDown += TextBox10_KeyDown;
            textBox10.Enter += TextBox10_Enter;
            textBox10.Leave += TextBox10_Leave;
        }
        private void Initialize_TextBox11()
        {
            textBox11.KeyDown += TextBox11_KeyDown;
            textBox11.Enter += TextBox11_Enter;
            textBox11.Leave += TextBox11_Leave;
        }
        private void Initialize_TextBox12()
        {
            textBox12.KeyDown += TextBox12_KeyDown;
            textBox12.Enter += TextBox12_Enter;
            textBox12.Leave += TextBox12_Leave;
        }
        private void Initialize_TextBox13()
        {
            textBox13.KeyDown += TextBox13_KeyDown;
            textBox13.Enter += TextBox13_Enter;
            textBox13.Leave += TextBox13_Leave;
        }
        private void Initialize_TextBox14()
        {
            textBox14.KeyDown += TextBox14_KeyDown;
            textBox14.Enter += TextBox14_Enter;
            textBox14.Leave += TextBox14_Leave;
        }
        private void Initialize_TextBox15()
        {
            textBox15.KeyDown += TextBox15_KeyDown;
            textBox15.Enter += TextBox15_Enter;
            textBox15.Leave += TextBox15_Leave;
        }
        private void Initialize_TextBox16()
        {
            textBox16.KeyDown += TextBox16_KeyDown;
            textBox16.Enter += TextBox16_Enter;
            textBox16.Leave += TextBox16_Leave;
        }
        private void Initialize_TextBox17()
        {
            textBox17.KeyDown += TextBox17_KeyDown;
            textBox17.Enter += TextBox17_Enter;
            textBox17.Leave += TextBox17_Leave;
        }
        private void Initialize_TextBox18()
        {
            textBox18.KeyDown += TextBox18_KeyDown;
            textBox18.Enter += TextBox18_Enter;
            textBox18.Leave += TextBox18_Leave;
        }
        private void Initialize_TextBox19()
        {
            textBox19.KeyDown += TextBox19_KeyDown;
            textBox19.Enter += TextBox19_Enter;
            textBox19.Leave += TextBox19_Leave;
        }
        private void Initialize_TextBox20()
        {
            textBox20.KeyDown += TextBox20_KeyDown;
            textBox20.Enter += TextBox20_Enter;
            textBox20.Leave += TextBox20_Leave;
        }
        private void Initialize_TextBox21()
        {
            textBox21.KeyDown += TextBox21_KeyDown;
            textBox21.Enter += TextBox21_Enter;
            textBox21.Leave += TextBox21_Leave;
        }
        private void Initialize_TextBox22()
        {
            textBox22.KeyDown += TextBox22_KeyDown;
            textBox22.Enter += TextBox22_Enter;
            textBox22.Leave += TextBox22_Leave;
        }
        private void Initialize_TextBox23()
        {
            textBox23.KeyDown += TextBox23_KeyDown;
            textBox23.Enter += TextBox23_Enter;
            textBox23.Leave += TextBox23_Leave;
        }
        private void Initialize_TextBox25()
        {
            textBox25.KeyDown += TextBox25_KeyDown;
            textBox25.Enter += TextBox25_Enter;
            textBox25.Leave += TextBox25_Leave;
        }
        private void Initialize_TextBox26()
        {
            textBox26.KeyDown += TextBox26_KeyDown;
            textBox26.Enter += TextBox26_Enter;
            textBox26.Leave += TextBox26_Leave;
        }
        private void Initialize_TextBox27()
        {
            textBox27.KeyDown += TextBox27_KeyDown;
            textBox27.Enter += TextBox27_Enter;
            textBox27.Leave += TextBox27_Leave;
        }
        private void Initialize_TextBox28()
        {
            textBox28.KeyDown += TextBox28_KeyDown;
            textBox28.Enter += TextBox28_Enter;
            textBox28.Leave += TextBox28_Leave;
        }
        private void Initialize_TextBox29()
        {
            textBox29.KeyDown += TextBox29_KeyDown;
            textBox29.Enter += TextBox29_Enter;
            textBox29.Leave += TextBox29_Leave;

        }
        private void Initialize_TextBox30()
        {
            textBox30.KeyDown += TextBox30_KeyDown;
            textBox30.Enter += TextBox30_Enter;
            textBox30.Leave += TextBox30_Leave;

        }
        private void Initialize_TextBox31()
        {
            textBox31.KeyDown += TextBox31_KeyDown;
            textBox31.Enter += TextBox31_Enter;
            textBox31.Leave += TextBox31_Leave;

        }
        private void Initialize_TextBox32()
        {
            textBox32.KeyDown += TextBox32_KeyDown;
            textBox32.Enter += TextBox32_Enter;
            textBox32.Leave += TextBox32_Leave;

        }
        private void Initialize_CheckBoxs()
        {
            for (int i = 0; i < CheckBoxs.Length; i++)
            {
                CheckBoxs[i].CheckedChanged += CheckBoxStateChanged;
            }

        }
        /// <summary>
        /// 加载所有显示器并填充到 ComboBox 中
        /// </summary>
        private void LoadDisplays()
        {
            // 清空显示器下拉框
            comboBox2.Items.Clear();

            // 查询每个显示器的设备名称
            for (int i = 0; i < screens.Length; i++)
            {
                // 将显示器的序号和设备名称添加到显示器下拉框
                comboBox2.Items.Add($"{i + 1} - {screens[i].DeviceName}");
            }

            // 默认选择第一个显示器（如果有）
            if (screens.Length > 0)
            {
                comboBox2.SelectedIndex = 0;
                Index_CurrentlySelectedDisplay = 0;
                targetScreen = screens[0];
            }
        }
        private void Initialize_AllMutationButton()
        {
            button90.Click += Mutation_Add;
            button89.Click += Mutation_Add;
            button87.Click += Mutation_Add;
            button86.Click += Mutation_Add;
            button85.Click += Mutation_Add;
            button84.Click += Mutation_Add;
            button83.Click += Mutation_Add;
            button82.Click += Mutation_Add;
            button81.Click += Mutation_Add;
            button80.Click += Mutation_Add;
            button79.Click += Mutation_Add;
            button78.Click += Mutation_Add;
            button77.Click += Mutation_Add;
            button76.Click += Mutation_Add;
            button75.Click += Mutation_Add;
            button74.Click += Mutation_Add;
            button73.Click += Mutation_Add;
            button72.Click += Mutation_Add;
            button71.Click += Mutation_Add;
            button70.Click += Mutation_Add;
            button69.Click += Mutation_Add;
            button68.Click += Mutation_Add;
            button67.Click += Mutation_Add;
            button66.Click += Mutation_Add;
            button65.Click += Mutation_Add;
            button64.Click += Mutation_Add;
            button63.Click += Mutation_Add;
            button62.Click += Mutation_Add;
            button61.Click += Mutation_Add;
            button60.Click += Mutation_Add;
            button59.Click += Mutation_Add;
            button58.Click += Mutation_Add;
            button57.Click += Mutation_Add;
            button56.Click += Mutation_Add;
            button55.Click += Mutation_Add;
            button54.Click += Mutation_Add;
            button53.Click += Mutation_Add;
            button52.Click += Mutation_Add;
            button51.Click += Mutation_Add;
            button50.Click += Mutation_Add;
            button49.Click += Mutation_Add;
            button48.Click += Mutation_Add;
            button47.Click += Mutation_Add;
            button46.Click += Mutation_Add;
            button45.Click += Mutation_Add;
            button35.Click += Mutation_Add;
            button34.Click += Mutation_Add;
            button33.Click += Mutation_Add;
            button32.Click += Mutation_Add;
            button31.Click += Mutation_Add;
            button30.Click += Mutation_Add;
            button29.Click += Mutation_Add;
            button28.Click += Mutation_Add;
            button27.Click += Mutation_Add;
            button26.Click += Mutation_Add;
            button25.Click += Mutation_Add;
            button24.Click += Mutation_Add;
            button23.Click += Mutation_Add;
            button22.Click += Mutation_Add;
            button21.Click += Mutation_Add;
            button20.Click += Mutation_Add;
            button19.Click += Mutation_Add;
            button18.Click += Mutation_Add;
            button17.Click += Mutation_Add;
            button16.Click += Mutation_Add;
            button15.Click += Mutation_Add;
            button14.Click += Mutation_Add;

        }

        #endregion
        #region 更新UI部分
        private void Update_TextBox1()
        {
            textBox1.Text = hotKeyText1;
        }
        private void Update_TextBox2()
        {
            textBox2.Text = hotKeyText2;

        }
        private void Update_TextBox3()
        {
            textBox3.Text = intervalTime_GetCard.ToString();
        }
        private void Update_TextBox4()
        {
            textBox4.Text = intervalTime_StoreRefresh.ToString();
        }
        private void Update_TextBox6()
        {
            textBox6.Text = countOfMoney_StopRefreshStore.ToString();
        }
        private void Update_TextBox7()
        {
            textBox7.Text = countOfMoney_StartRefreshStore.ToString();
        }
        private void Update_TextBox8()
        {
            textBox8.Text = startPoint_CardScreenshotX1.ToString();
        }
        private void Update_TextBox9()
        {
            textBox9.Text = startPoint_CardScreenshotY6.ToString();
        }
        private void Update_TextBox10()
        {
            textBox10.Text = height_CardScreenshot.ToString();
        }
        private void Update_TextBox11()
        {
            textBox11.Text = width_CardScreenshot.ToString();
        }
        private void Update_TextBox12()
        {
            textBox12.Text = height_MoneyScreenshot.ToString();
        }
        private void Update_TextBox13()
        {
            textBox13.Text = width_MoneyScreenshot.ToString();
        }
        private void Update_TextBox14()
        {
            textBox14.Text = startPoint_MoneyScreenshotY.ToString();
        }
        private void Update_TextBox15()
        {
            textBox15.Text = startPoint_MoneyScreenshotX.ToString();
        }
        private void Update_TextBox16()
        {
            textBox16.Text = height_MutationScreenshot.ToString();
        }
        private void Update_TextBox17()
        {
            textBox17.Text = width_MutationScreenshot.ToString();
        }
        private void Update_TextBox18()
        {
            textBox18.Text = startPoint_MutationScreenshotY.ToString();
        }
        private void Update_TextBox19()
        {
            textBox19.Text = startPoint_MutationScreenshotX.ToString();
        }
        private void Update_TextBox20()
        {
            textBox20.Text = startPoint_CardScreenshotX2.ToString();
        }
        private void Update_TextBox21()
        {
            textBox21.Text = startPoint_CardScreenshotX3.ToString();
        }
        private void Update_TextBox22()
        {
            textBox22.Text = startPoint_CardScreenshotX4.ToString();
        }
        private void Update_TextBox23()
        {
            textBox23.Text = startPoint_CardScreenshotX6.ToString();
        }
        private void Update_TextBox25()
        {
            textBox25.Text = Point_RefreshMutationY.ToString();
        }
        private void Update_TextBox26()

        {
            textBox26.Text = Point_RefreshMutationX.ToString();
        }
        private void Update_TextBox27()
        {
            textBox27.Text = Point_EvolutionMutationY.ToString();
        }
        private void Update_TextBox28()
        {
            textBox28.Text = Point_EvolutionMutationX.ToString();
        }
        private void Update_TextBox29()
        {
            textBox29.Text = hotKeyText3;
        }
        private void Update_TextBox30()
        {
            textBox30.Text = intervalTime_Mutation.ToString();
        }
        private void Update_TextBox31()
        {
            textBox31.Text = Point_RefreshStoreY.ToString();
        }
        private void Update_TextBox32()
        {
            textBox32.Text = Point_RefreshStoreX.ToString();
        }
        private void Update_DropDownBox()
        {
            comboBox1.SelectedIndex = dropDownBoxItem_CurrentlySelected;
        }
        private void Update_ShowSelectedTextBox()
        {

            richTextBox1.Clear(); // 清除之前的内容

            for (int i = 0; i < CheckList.Length; i++)
            {
                if (CheckList[i])
                {
                    string text_Add = cardName[i] + "|";//获取要添加的文本
                    int start_Point = richTextBox1.TextLength;  // 获取添加文本前的长度
                    richTextBox1.AppendText(text_Add);  // 添加文本

                    // 判断是否位于特定范围内
                    if (i >= 0 && i <= 13)
                    {
                        richTextBox1.Select(start_Point, text_Add.Length); // 选择刚刚添加的文本
                        richTextBox1.SelectionColor = Color.FromArgb(107, 104, 101); ; // 设置选择文本的颜色
                    }
                    if (i >= 14 && i <= 26)
                    {
                        richTextBox1.Select(start_Point, text_Add.Length); // 选择刚刚添加的文本
                        richTextBox1.SelectionColor = Color.FromArgb(5, 171, 117); // 设置选择文本的颜色
                    }
                    if (i >= 27 && i <= 39)
                    {
                        richTextBox1.Select(start_Point, text_Add.Length); // 选择刚刚添加的文本
                        richTextBox1.SelectionColor = Color.FromArgb(0, 133, 255); // 设置选择文本的颜色
                    }
                    if (i >= 40 && i <= 51)
                    {
                        richTextBox1.Select(start_Point, text_Add.Length); // 选择刚刚添加的文本
                        richTextBox1.SelectionColor = Color.FromArgb(175, 40, 195); // 设置选择文本的颜色
                    }
                    if (i >= 52 && i <= 59)
                    {
                        richTextBox1.Select(start_Point, text_Add.Length); // 选择刚刚添加的文本
                        richTextBox1.SelectionColor = Color.FromArgb(245, 158, 11); // 设置选择文本的颜色
                    }
                    if (i >= 60 && i <= 62)
                    {
                        richTextBox1.Select(start_Point, text_Add.Length); // 选择刚刚添加的文本
                        richTextBox1.SelectionColor = Color.Red; // 设置选择文本的颜色
                    }
                }
            }

            // 重置选择
            richTextBox1.Select(richTextBox1.TextLength, 0);
        }
        #endregion
        #region TextBox事件实现部分
        #region 自动拿牌快捷键
        private void TextBox1_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox1.Text = "";
        }
        private void TextBox1_Leave(object sender, EventArgs e)
        {
            // 用户离开文本框时，如果没有输入，则显示当前应用的快捷键
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox1.Text = hotKeyText1;
            }
        }
        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点
                return;
            }
            if (key != Keys.Back && key != Keys.Delete && key != Keys.Escape)
            {
                hotKeyText1 = key.ToString();
                textBox1.Text = hotKeyText1;
                e.SuppressKeyPress = true; // 阻止进一步处理按键事件，如发出声音

                //更新程序的快捷键配置
                RegisterHotKey(this.Handle, HOTKEY_1, 0, (uint)key);
            }
        }
        #endregion
        #region 自动D牌快捷键               
        private void TextBox2_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox2.Text = "";
        }
        private void TextBox2_Leave(object sender, EventArgs e)
        {
            // 用户离开文本框时，如果没有输入，则显示当前应用的快捷键
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                textBox2.Text = hotKeyText2;
            }
        }
        private void TextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点
                return;
            }
            if (key != Keys.Back && key != Keys.Delete && key != Keys.Escape)
            {
                hotKeyText2 = key.ToString();
                textBox2.Text = hotKeyText2;
                e.SuppressKeyPress = true; // 阻止进一步处理按键事件，如发出声音

                //更新程序的快捷键配置
                RegisterHotKey(this.Handle, HOTKEY_2, 0, (uint)key);
            }
        }
        #endregion
        #region 自动D异变快捷键
        private void TextBox29_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox29.Text = "";
        }
        private void TextBox29_Leave(object sender, EventArgs e)
        {
            // 用户离开文本框时，如果没有输入，则显示当前应用的快捷键
            if (string.IsNullOrWhiteSpace(textBox29.Text))
            {
                textBox29.Text = hotKeyText3;
            }
        }
        private void TextBox29_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点
                return;
            }
            if (key != Keys.Back && key != Keys.Delete && key != Keys.Escape)
            {
                hotKeyText3 = key.ToString();
                textBox29.Text = hotKeyText3;
                e.SuppressKeyPress = true; // 阻止进一步处理按键事件，如发出声音

                // 更新程序的快捷键配置
                RegisterHotKey(this.Handle, HOTKEY_3, 0, (uint)key);
            }
        }
        #endregion
        #region 商店刷新按钮坐标X
        private void TextBox32_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox32.Text = "";
        }
        private void TextBox32_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点

            }

        }
        private void TextBox32_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox32.Text))
            {

                Update_TextBox32();
            }
            else
            {
                try
                {
                    Point_RefreshStoreX = int.Parse(textBox32.Text);

                    Update_TextBox32();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox32();

                }
            }
        }
        #endregion
        #region 商店刷新按钮坐标Y
        private void TextBox31_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox31.Text = "";
        }
        private void TextBox31_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点

            }

        }
        private void TextBox31_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox31.Text))
            {

                Update_TextBox31();
            }
            else
            {
                try
                {
                    Point_RefreshStoreY = int.Parse(textBox31.Text);

                    Update_TextBox31();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox31();

                }
            }
        }
        #endregion       
        #region 异常突变进化按钮坐标X
        private void TextBox28_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox28.Text = "";
        }
        private void TextBox28_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点

            }

        }
        private void TextBox28_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox28.Text))
            {

                Update_TextBox28();
            }
            else
            {
                try
                {
                    Point_EvolutionMutationX = int.Parse(textBox28.Text);

                    Update_TextBox28();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox28();

                }
            }
        }
        #endregion
        #region 异常突变进化按钮坐标Y
        private void TextBox27_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox27.Text = "";
        }
        private void TextBox27_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点

            }

        }
        private void TextBox27_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox27.Text))
            {

                Update_TextBox27();
            }
            else
            {
                try
                {
                    Point_EvolutionMutationY = int.Parse(textBox27.Text);

                    Update_TextBox27();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox27();

                }
            }
        }
        #endregion
        #region 异常突变刷新按钮坐标X
        private void TextBox26_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox26.Text = "";
        }
        private void TextBox26_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点

            }

        }
        private void TextBox26_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox26.Text))
            {

                Update_TextBox26();
            }
            else
            {
                try
                {
                    Point_RefreshMutationX = int.Parse(textBox26.Text);

                    Update_TextBox26();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox26();

                }
            }
        }
        #endregion
        #region 异常突变刷新按钮坐标Y
        private void TextBox25_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox25.Text = "";
        }
        private void TextBox25_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点

            }

        }
        private void TextBox25_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox25.Text))
            {

                Update_TextBox25();
            }
            else
            {
                try
                {
                    Point_RefreshMutationY = int.Parse(textBox25.Text);

                    Update_TextBox25();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox25();

                }
            }
        }
        #endregion
        #region 异常突变截图起点坐标X
        private void TextBox19_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox19.Text = "";
        }
        private void TextBox19_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null; // 将活动控件设置为null，使文本框失去焦点

            }

        }
        private void TextBox19_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox19.Text))
            {

                Update_TextBox19();
            }
            else
            {
                try
                {
                    startPoint_MutationScreenshotX = int.Parse(textBox19.Text);

                    Update_TextBox19();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox19();

                }
            }
        }
        #endregion
        #region 异常突变截图起点坐标Y
        private void TextBox18_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox18.Text = "";
        }
        private void TextBox18_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null; // 将活动控件设置为null，使文本框失去焦点

            }

        }
        private void TextBox18_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox18.Text))
            {

                Update_TextBox18();
            }
            else
            {
                try
                {
                    startPoint_MutationScreenshotY = int.Parse(textBox18.Text);

                    Update_TextBox18();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox18();

                }
            }
        }
        #endregion
        #region 异常突变截图大小_宽
        private void TextBox17_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox17.Text = "";
        }
        private void TextBox17_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null; // 将活动控件设置为null，使文本框失去焦点

            }

        }
        private void TextBox17_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox17.Text))
            {

                Update_TextBox17();
            }
            else
            {
                try
                {
                    width_MutationScreenshot = int.Parse(textBox17.Text);

                    Update_TextBox17();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox17();

                }
            }
        }
        #endregion
        #region 异常突变截图大小_高
        private void TextBox16_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox16.Text = "";
        }
        private void TextBox16_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点

            }

        }
        private void TextBox16_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox16.Text))
            {

                Update_TextBox16();
            }
            else
            {
                try
                {
                    height_MutationScreenshot = int.Parse(textBox16.Text);

                    Update_TextBox16();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox16();

                }
            }
        }
        #endregion
        #region 金币截图起点坐标X
        private void TextBox15_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox15.Text = "";
        }
        private void TextBox15_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点

            }

        }
        private void TextBox15_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox15.Text))
            {

                Update_TextBox15();
            }
            else
            {
                try
                {
                    startPoint_MoneyScreenshotX = int.Parse(textBox15.Text);

                    Update_TextBox15();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox15();

                }
            }
        }
        #endregion
        #region 金币截图起点坐标Y
        private void TextBox14_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox14.Text = "";
        }
        private void TextBox14_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点

            }

        }
        private void TextBox14_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox14.Text))
            {

                Update_TextBox14();
            }
            else
            {
                try
                {
                    startPoint_MoneyScreenshotY = int.Parse(textBox14.Text);

                    Update_TextBox14();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox14();

                }
            }
        }
        #endregion
        #region 金币截图大小_宽
        private void TextBox13_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox13.Text = "";
        }
        private void TextBox13_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点

            }

        }
        private void TextBox13_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox13.Text))
            {

                Update_TextBox13();
            }
            else
            {
                try
                {
                    width_MoneyScreenshot = int.Parse(textBox13.Text);

                    Update_TextBox13();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox13();

                }
            }
        }
        #endregion
        #region 金币截图大小_高
        private void TextBox12_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox12.Text = "";
        }
        private void TextBox12_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点

            }

        }
        private void TextBox12_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox12.Text))
            {

                Update_TextBox12();
            }
            else
            {
                try
                {
                    height_MoneyScreenshot = int.Parse(textBox12.Text);

                    Update_TextBox12();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox12();

                }
            }
        }

        #endregion
        #region 奕子截图起点坐标X1
        private void TextBox8_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox8.Text = "";
        }
        private void TextBox8_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点

            }

        }
        private void TextBox8_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox8.Text))
            {

                Update_TextBox8();
            }
            else
            {
                try
                {
                    startPoint_CardScreenshotX1 = int.Parse(textBox8.Text);

                    Update_TextBox8();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox8();

                }
            }
        }
        #endregion
        #region 奕子截图起点坐标X2
        private void TextBox20_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox20.Text = "";
        }
        private void TextBox20_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点

            }

        }
        private void TextBox20_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox20.Text))
            {

                Update_TextBox20();
            }
            else
            {
                try
                {
                    startPoint_CardScreenshotX2 = int.Parse(textBox20.Text);

                    Update_TextBox20();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox20();

                }
            }
        }
        #endregion
        #region 奕子截图起点坐标X3
        private void TextBox21_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox21.Text = "";
        }
        private void TextBox21_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点

            }

        }
        private void TextBox21_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox21.Text))
            {

                Update_TextBox21();
            }
            else
            {
                try
                {
                    startPoint_CardScreenshotX3 = int.Parse(textBox21.Text);

                    Update_TextBox21();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox21();

                }
            }
        }
        #endregion
        #region 奕子截图起点坐标X4
        private void TextBox22_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox22.Text = "";
        }
        private void TextBox22_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点

            }

        }
        private void TextBox22_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox22.Text))
            {

                Update_TextBox22();
            }
            else
            {
                try
                {
                    startPoint_CardScreenshotX4 = int.Parse(textBox22.Text);

                    Update_TextBox22();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox22();

                }
            }
        }
        #endregion
        #region 奕子截图起点坐标X5
        private void TextBox23_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox23.Text = "";
        }
        private void TextBox23_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点

            }

        }
        private void TextBox23_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox23.Text))
            {

                Update_TextBox23();
            }
            else
            {
                try
                {
                    startPoint_CardScreenshotX6 = int.Parse(textBox23.Text);

                    Update_TextBox23();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox23();

                }
            }
        }
        #endregion
        #region 奕子截图起点坐标Y
        private void TextBox9_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox9.Text = "";
        }
        private void TextBox9_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点

            }

        }
        private void TextBox9_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox9.Text))
            {

                Update_TextBox9();
            }
            else
            {
                try
                {
                    startPoint_CardScreenshotY6 = int.Parse(textBox9.Text);

                    Update_TextBox9();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox9();

                }
            }
        }
        #endregion
        #region  奕子截图大小_宽
        private void TextBox11_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox11.Text = "";
        }
        private void TextBox11_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点

            }

        }
        private void TextBox11_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox11.Text))
            {

                Update_TextBox11();
            }
            else
            {
                try
                {
                    width_CardScreenshot = int.Parse(textBox11.Text);

                    Update_TextBox11();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox11();

                }
            }
        }
        #endregion
        #region  奕子截图大小_高
        private void TextBox10_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox10.Text = "";
        }
        private void TextBox10_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点

            }

        }
        private void TextBox10_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox10.Text))
            {

                Update_TextBox10();
            }
            else
            {
                try
                {
                    height_CardScreenshot = int.Parse(textBox10.Text);

                    Update_TextBox10();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox10();

                }
            }
        }

        #endregion
        #region 开始D牌金币数
        private void TextBox7_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox7.Text = "";
        }
        private void TextBox7_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点

            }

        }
        private void TextBox7_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox7.Text))
            {

                Update_TextBox7();
            }
            else
            {
                try
                {
                    countOfMoney_StartRefreshStore = int.Parse(textBox7.Text);

                    Update_TextBox7();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox7();

                }
            }
        }
        #endregion
        #region 停止D牌金币数
        private void TextBox6_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox6.Text = "";
        }
        private void TextBox6_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点

            }


        }
        private void TextBox6_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox6.Text))
            {

                Update_TextBox6();
            }
            else
            {
                try
                {
                    countOfMoney_StopRefreshStore = int.Parse(textBox6.Text);

                    Update_TextBox6();
                }
                catch
                {
                    MessageBox.Show("输入错误！");
                    Update_TextBox6();

                }
            }
        }
        #endregion        
        #region 拿牌延迟
        private void TextBox3_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox3.Text = "";
        }
        private void TextBox3_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                Update_TextBox3();
            }
            else
            {
                try
                {
                    intervalTime_GetCard = int.Parse(textBox3.Text);
                    Update_TextBox3();
                }
                catch
                {
                    MessageBox.Show("延迟输入错误！");
                    Update_TextBox3();

                }
            }
        }
        private void TextBox3_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点

            }


        }
        #endregion
        #region D牌延迟
        private void TextBox4_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox4.Text = "";
        }
        private void TextBox4_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                Update_TextBox4();
            }
            else
            {
                try
                {
                    intervalTime_StoreRefresh = int.Parse(textBox4.Text);
                    Update_TextBox4();
                }
                catch
                {
                    MessageBox.Show("延迟输入错误！");
                    Update_TextBox4();

                }
            }
        }
        private void TextBox4_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null; // 将活动控件设置为null，使文本框失去焦点

            }


        }
        #endregion
        #region D异变延迟
        private void TextBox30_Enter(object sender, EventArgs e)
        {
            // 当用户进入文本框时，清空现有内容
            textBox30.Text = "";
        }
        private void TextBox30_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox30.Text))
            {
                Update_TextBox30();
            }
            else
            {
                try
                {
                    intervalTime_Mutation = int.Parse(textBox30.Text);
                    Update_TextBox30();
                }
                catch
                {
                    MessageBox.Show("延迟输入错误！");
                    Update_TextBox30();

                }
            }
        }
        private void TextBox30_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，使文本框失去焦点

            }


        }
        #endregion
        #endregion
        #region 保存配置与读取配置
        /// <summary>
        /// 保存设置
        /// </summary>
        private void SaveSettings()
        {
            var config = new AppConfig
            {
                HotKey1 = hotKeyText1,
                HotKey2 = hotKeyText2,
                HotKey3 = hotKeyText3,
                intervalTime_GetCard = intervalTime_GetCard,
                intervalTime_StoreRefresh = intervalTime_StoreRefresh,
                intervalTime_RefreshStore = intervalTime_Mutation,
                countOfMoney_StartRefreshStore = countOfMoney_StartRefreshStore,
                countOfMoney_StopRefreshStore = countOfMoney_StopRefreshStore,

                startPoint_CardScreenshotX1 = startPoint_CardScreenshotX1,
                startPoint_CardScreenshotX2 = startPoint_CardScreenshotX2,
                startPoint_CardScreenshotX3 = startPoint_CardScreenshotX3,
                startPoint_CardScreenshotX4 = startPoint_CardScreenshotX4,
                startPoint_CardScreenshotX6 = startPoint_CardScreenshotX6,
                startPoint_CardScreenshotY6 = startPoint_CardScreenshotY6,
                width_CardScreenshot = width_CardScreenshot,
                height_CardScreenshot = height_CardScreenshot,
                startPoint_MoneyScreenshotX = startPoint_MoneyScreenshotX,
                startPoint_MoneyScreenshotY = startPoint_MoneyScreenshotY,
                width_MoneyScreenshot = width_MoneyScreenshot,
                height_MoneyScreenshot = height_MoneyScreenshot,
                startPoint_MutationScreenshotX = startPoint_MutationScreenshotX,
                startPoint_MutationScreenshotY = startPoint_MutationScreenshotY,
                width_MutationScreenshot = width_MutationScreenshot,
                height_MutationScreenshot = height_MutationScreenshot,
                Point_RefreshMutationX = Point_RefreshMutationX,
                Point_RefreshMutationY = Point_RefreshMutationY,
                Point_EvolutionMutationX = Point_EvolutionMutationX,
                Point_EvolutionMutationY = Point_EvolutionMutationY,
                Point_RefreshStoreX = Point_RefreshStoreX,
                Point_RefreshStoreY = Point_RefreshStoreY

            };
            //将config对象序列化为 JSON 字符串
            string json = JsonConvert.SerializeObject(config);
            //将"AppConfig.json"配置文件写到本地
            File.WriteAllText("AppConfig.json", json);
        }
        /// <summary>
        /// 默认设置
        /// </summary>
        private void DefaultSettings()
        {
            var config = new AppConfig
            {
                HotKey1 = "F7",
                HotKey2 = "F8",
                HotKey3 = "F11",
                intervalTime_GetCard = 50,
                intervalTime_StoreRefresh = 300,//小于等于200会导致商店未完全刷新就开始截图
                intervalTime_RefreshStore = 200,
                countOfMoney_StartRefreshStore = 0,
                countOfMoney_StopRefreshStore = 2,

                startPoint_CardScreenshotX1 = 549,
                startPoint_CardScreenshotX2 = 755,
                startPoint_CardScreenshotX3 = 961,
                startPoint_CardScreenshotX4 = 1173,
                startPoint_CardScreenshotX6 = 1380,
                startPoint_CardScreenshotY6 = 1029,
                width_CardScreenshot = 146,
                height_CardScreenshot = 31,
                startPoint_MoneyScreenshotX = 960,
                startPoint_MoneyScreenshotY = 841,
                width_MoneyScreenshot = 54,
                height_MoneyScreenshot = 36,
                startPoint_MutationScreenshotX = 570,
                startPoint_MutationScreenshotY = 900,
                width_MutationScreenshot = 250,
                height_MutationScreenshot = 38,
                Point_RefreshMutationX = 1458,
                Point_RefreshMutationY = 1015,
                Point_EvolutionMutationX = 1458,
                Point_EvolutionMutationY = 921,
                Point_RefreshStoreX = 441,
                Point_RefreshStoreY = 1027
            };
            //将config对象序列化为 JSON 字符串
            string json = JsonConvert.SerializeObject(config);
            //将"AppConfig.json"配置文件写到本地
            File.WriteAllText("AppConfig.json", json);
        }
        /// <summary>
        /// 将键名称转到keys枚举类型
        /// </summary>
        /// <param name="keyString"></param>
        /// <returns></returns>
        private Keys ConvertKeyNameToEnumerValueKeys(string keyString)
        {
            return (Keys)Enum.Parse(typeof(Keys), keyString);
        }
        /// <summary>
        /// 加载设置
        /// </summary>
        private void LoadSettings()
        {
            //如果配置文件"AppConfig.json"存在，则读取配置文件
            if (File.Exists("AppConfig.json"))
            {
                //从本地文件"AppConfig.json"读取到json
                string json = File.ReadAllText("AppConfig.json");
                //反序列化json配置到AppConfig对象config
                var config = JsonConvert.DeserializeObject<AppConfig>(json);


                hotKeyText1 = config.HotKey1;
                var key1 = ConvertKeyNameToEnumerValueKeys(config.HotKey1);
                RegisterHotKey(this.Handle, HOTKEY_1, 0, (uint)key1);

                hotKeyText2 = config.HotKey2;
                var key2 = ConvertKeyNameToEnumerValueKeys(config.HotKey2);
                RegisterHotKey(this.Handle, HOTKEY_2, 0, (uint)key2);

                hotKeyText3 = config.HotKey3;
                var key3 = ConvertKeyNameToEnumerValueKeys(config.HotKey3);
                RegisterHotKey(this.Handle, HOTKEY_3, 0, (uint)key3);

                intervalTime_GetCard = config.intervalTime_GetCard;

                intervalTime_StoreRefresh = config.intervalTime_StoreRefresh;

                intervalTime_Mutation = config.intervalTime_RefreshStore;


                countOfMoney_StopRefreshStore = config.countOfMoney_StopRefreshStore;

                countOfMoney_StartRefreshStore = config.countOfMoney_StartRefreshStore;

                startPoint_CardScreenshotX1 = config.startPoint_CardScreenshotX1;
                startPoint_CardScreenshotX2 = config.startPoint_CardScreenshotX2;
                startPoint_CardScreenshotX3 = config.startPoint_CardScreenshotX3;
                startPoint_CardScreenshotX4 = config.startPoint_CardScreenshotX4;
                startPoint_CardScreenshotX6 = config.startPoint_CardScreenshotX6;
                startPoint_CardScreenshotY6 = config.startPoint_CardScreenshotY6;
                width_CardScreenshot = config.width_CardScreenshot;
                height_CardScreenshot = config.height_CardScreenshot;
                startPoint_MoneyScreenshotX = config.startPoint_MoneyScreenshotX;
                startPoint_MoneyScreenshotY = config.startPoint_MoneyScreenshotY;
                width_MoneyScreenshot = config.width_MoneyScreenshot;
                height_MoneyScreenshot = config.height_MoneyScreenshot;
                startPoint_MutationScreenshotX = config.startPoint_MutationScreenshotX;
                startPoint_MutationScreenshotY = config.startPoint_MutationScreenshotY;
                width_MutationScreenshot = config.width_MutationScreenshot;
                height_MutationScreenshot = config.height_MutationScreenshot;
                Point_RefreshMutationX = config.Point_RefreshMutationX;
                Point_RefreshMutationY = config.Point_RefreshMutationY;
                Point_EvolutionMutationX = config.Point_EvolutionMutationX;
                Point_EvolutionMutationY = config.Point_EvolutionMutationY;
                Point_RefreshStoreX = config.Point_RefreshStoreX;
                Point_RefreshStoreY = config.Point_RefreshStoreY;


                // 更新 UI 或其他相关组件
                Update_TextBox1();
                Update_TextBox2();
                Update_TextBox3();
                Update_TextBox4();

                Update_TextBox6();
                Update_TextBox7();
                Update_TextBox8();
                Update_TextBox9();
                Update_TextBox10();
                Update_TextBox11();
                Update_TextBox12();
                Update_TextBox13();
                Update_TextBox14();
                Update_TextBox15();
                Update_TextBox16();
                Update_TextBox17();
                Update_TextBox18();
                Update_TextBox19();
                Update_TextBox20();
                Update_TextBox21();
                Update_TextBox22();
                Update_TextBox23();
                Update_TextBox25();
                Update_TextBox26();
                Update_TextBox27();
                Update_TextBox28();
                Update_TextBox29();
                Update_TextBox30();
                Update_TextBox31();
                Update_TextBox32();
            }
            else//配置文件不存在，则生成基于默认配置的配置文件，并读取
            {
                DefaultSettings();
                LoadSettings();
            }
        }
        #endregion
        #region 添加P/Invoke 快捷键声明
        [DllImport("user32.dll", SetLastError = true)]

        //RegisterHotKey是windowsAPI提供的注册全局快捷键的函数
        /*
         * IntPtr hWnd:接收热键消息的窗口的句柄。如果这个参数是零，热键消息将被发送到调用 RegisterHotKey 的线程的消息队列。
         * int id：定义热键的唯一标识符。如果多次调用 RegisterHotKey，可以使用不同的 id 来区分不同的热键。
         * uint fsModifiers：定义了一组修饰键（如 ALT、CTRL、SHIFT、WIN等），这些修饰键必须与热键一起按下以触发热键。
         * uint vk：定义热键本身的键码（即虚拟键码，如 Keys.F1、Keys.A 等）。
         */
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll", SetLastError = true)]

        //UnregisterHotKey是windowsAPI提供的注销全局快捷键的函数
        /*
         * IntPtr hWnd:接收热键消息的窗口的句柄。如果这个参数是零，热键消息将被发送到调用 RegisterHotKey 的线程的消息队列。
         * int id：定义热键的唯一标识符。如果多次调用 RegisterHotKey，可以使用不同的 id 来区分不同的热键。
         * uint fsModifiers：定义了一组修饰键（如 ALT、CTRL、SHIFT、WIN等），这些修饰键必须与热键一起按下以触发热键。
         * uint vk：定义热键本身的键码（即虚拟键码，如 Keys.F1、Keys.A 等）。
         */
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion
        #region 快捷键监听模块
        //消息处理函数，判断是否按下快捷键
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            const int WM_HOTKEY = 0x0312;  //这行定义了一个常量 WM_HOTKEY，其值为 0x0312。这个值是 Windows 消息标识符，用于表示一个热键被按下的消息。
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_1)
            {
                button1_Click(this, EventArgs.Empty);
            }
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_2)
            {
                button2_Click(this, EventArgs.Empty);
            }
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_3)
            {
                button7_Click(this, EventArgs.Empty);
            }
        }

        //程序关闭时执行注销快捷键的函数
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 注销热键
            UnregisterHotKey(this.Handle, HOTKEY_1);
            UnregisterHotKey(this.Handle, HOTKEY_2);
            UnregisterHotKey(this.Handle, HOTKEY_3);
            base.OnFormClosing(e);
        }
        #endregion
        //Form1的构造函数
        public Form1()
        {
            //自动生成，负责初始化窗体及其所有控件的布局和属性
            InitializeComponent();
        }

        //在窗体第一次显示之前被触发
        private void Form1_Load(object sender, EventArgs e)
        {
            Initialize_Variable();//初始化变量
            Initialize_AllComponents();//初始化组件
            LoadSettings();//从本地加载设置
            ReadLineUpFromLocal();//从本地读取阵容
            ReadLineUpNameFromLocal();//从本地读取阵容名称到阵容下拉框
            Update_DropDownBox();//更新阵容选择下拉框当前选择阵容
        }
        #region 组件响应事件/函数
        //快捷添加异常突变
        private void Mutation_Add(object sender, EventArgs e)
        {
            if (sender is Button clickedButton)
            {
                textBox24.Text += clickedButton.Text+"|";
            }
                
           
        }

        //有奕子被勾选或取消勾选
        private void CheckBoxStateChanged(object sender, EventArgs e)
        {
            CheckList = DetectionCheckState(CheckBoxs);
            Update_ShowSelectedTextBox();
        }

        //识别金钱计时器Timer事件
        private void Timer1_Tick(object sender, EventArgs e)
        {
            isMoneyRecognized = DetectionMoney(out countOfMoney);
            label78.Text = "当且金币：" + countOfMoney.ToString();
            label79.Text = "金币能否识别：" + (isMoneyRecognized ? "能" : "否");
        }

        //横向展开/收缩 窗口Timer事件
        private void Animation_HorizontalShrink(object sender, EventArgs e)
        {
            //如果要求主窗口展开
            if (isWindowExpandedHorizontally)
            {
                // 扩展窗体宽度
                if (this.Width < width_WindowExpanded)
                {
                    this.Width += 20;  // 窗体宽度同步增加
                }
                else
                {
                    timer2.Stop(); // 停止动画
                }
            }
            //如果要求主窗口收缩
            else
            {
                // 收缩窗体宽度
                if (this.Width > width_WindowContracted)
                {
                    this.Width -= 20;  // 窗体宽度同步减少
                }
                else
                {
                    timer2.Stop(); // 停止动画
                }
            }
        }
        //纵向展开/收缩 窗口Timer事件
        private void Animation_VerticalShrink(object sender, EventArgs e)
        {
            //如果要求主窗口展开
            if (isWindowExpandedVertically)
            {

                if (this.Height < height_WindowExpanded)
                {
                    this.Height += 20;
                }
                else
                {
                    timer3.Stop(); // 停止动画
                }
            }
            //如果要求主窗口收缩
            else
            {

                if (this.Height > height_WindowContracted)
                {
                    this.Height -= 20;
                }
                else
                {
                    timer3.Stop(); // 停止动画
                }
            }
        }
        //启动/关闭 拿牌
        private async void button1_Click(object sender, EventArgs e)
        {
            #region 改变按钮文本与开关状态
            switch_GetCard = !switch_GetCard;
            button1.Text = switch_GetCard ? "停止" : "启动";
            #endregion
            // 如果已经在运行，直接返回，避免重复启动任务
            if (isTheAsynRunning)
            {
                cancellationTokenSource.Cancel(); // 停止当前任务
                isTheAsynRunning = false; // 重置状态
                return;
            }
            // 立即重新创建一个新的 CancellationTokenSource
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            isTheAsynRunning = true;
            if (switch_GetCard)
            {
                // 启动一个新的后台任务来执行循环操作
                await Task.Run(async () =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        //判断“攒到()金币时开始D牌”功能是否勾选，若勾选且当前金币少于目标金币则等待并回到循环首部
                        if (checkBox65.CheckState == CheckState.Checked && countOfMoney < countOfMoney_StartRefreshStore)
                        {
                            await Task.Delay(1000); // 如果资源不足，等待一秒再检查
                            continue;
                        }
                        //若勾选“攒到()金币时开始D牌”功能且当前金币满足目标金币则取消勾选“攒到()金币时开始D牌”功能
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (checkBox65.CheckState == CheckState.Checked)
                            {
                                checkBox65.CheckState = CheckState.Unchecked;
                            }
                        });
                        //判断能否识别到金币或当前金币是否高于设置的“停止D牌金币数”，满足其一则等待并回到循环首部
                        if ((!isMoneyRecognized) || (countOfMoney <= countOfMoney_StopRefreshStore))
                        {
                            await Task.Delay(1000); // 如果资源不足，等待一秒再检查
                            continue;
                        }
                        // 逐个执行每个卡片的任务
                        await Screenshot_Processing_OCR_Compare_GetCard(startPoint_CardScreenshotX1, startPoint_CardScreenshotY6, 1, pictureBox79, textBox33);
                        await Screenshot_Processing_OCR_Compare_GetCard(startPoint_CardScreenshotX2, startPoint_CardScreenshotY6, 2, pictureBox80, textBox34);
                        await Screenshot_Processing_OCR_Compare_GetCard(startPoint_CardScreenshotX3, startPoint_CardScreenshotY6, 3, pictureBox81, textBox35);
                        await Screenshot_Processing_OCR_Compare_GetCard(startPoint_CardScreenshotX4, startPoint_CardScreenshotY6, 4, pictureBox82, textBox36);
                        await Screenshot_Processing_OCR_Compare_GetCard(startPoint_CardScreenshotX6, startPoint_CardScreenshotY6, 5, pictureBox83, textBox37);

                        await Task.Delay(100);//所有拿牌命令施加后等待的延迟，放置没拿到牌就刷新商店
                        if (switch_RefreshStore)
                        {

                            ControlTools.SetMousePositionAndClickLeftButton(Point_RefreshStoreX, Point_RefreshStoreY, screens, Index_CurrentlySelectedDisplay);
                            Debug.WriteLine($"刷新商店完成");

                        }
                        await Task.Delay(intervalTime_StoreRefresh);//每次刷新商店后等待的时长，方式商店未刷新就开始截图，截到与上一次商店内容一样的图
                    }
                }, token);

            }
        }

        //启动/关闭 刷新商店
        private void button2_Click(object sender, EventArgs e)
        {
            // 切换 D牌开关 状态
            switch_RefreshStore = !switch_RefreshStore;
            button2.Text = switch_RefreshStore ? "停止" : "启动";
        }

        //清空勾选
        private void button3_Click(object sender, EventArgs e)
        {
            ClearCheckBoxs();

        }

        //保存阵容
        private void button4_Click(object sender, EventArgs e)
        {
            SaveLineUpLocally();
        }

        //离开下拉框时保存现行选中项名称
        private void comboBox1_Leave(object sender, EventArgs e)
        {
            SaveLineUpNameLocally();
        }

        //当下拉框被关闭（即选择了新的或没选）时，记录当前选择的下拉框，并从中读取阵容
        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                dropDownBoxItem_CurrentlySelected = comboBox1.SelectedIndex;
            }
            ReadLineUpFromLocal();

        }


        //下拉框按下回车保存阵容名称
        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            // 捕获用户按下的键，并更新 TextBox
            var key = e.KeyCode; // 获取按键代码
            if (key == Keys.Enter)
            {
                this.ActiveControl = null;  // 将活动控件设置为null，下拉框失去焦点
                SaveLineUpNameLocally();
            }
        }

        //保存设置
        private void button5_Click_1(object sender, EventArgs e)
        {
            SaveSettings();
            MessageBox.Show("已保存设置！");
        }

        //还原默认设置
        private void button6_Click(object sender, EventArgs e)
        {
            DefaultSettings();
            LoadSettings();
            MessageBox.Show("已默认配置！");
        }

        //启动/关闭 自动D异变
        private async void button7_Click(object sender, EventArgs e)
        {
            switch_RefreshMutation = !switch_RefreshMutation;
            button7.Text = switch_RefreshMutation ? "停止" : "启动";
            // 发出一个取消信号,取消当前正在运行的任务
            cancellationTokenSource.Cancel();
            // 立即重新创建一个新的 CancellationTokenSource
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            if (switch_RefreshMutation)
            {
                // 启动一个新的后台任务来执行循环操作
                await Task.Run(async () =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        await OCR_Mutation(token);
                        await Task.Delay(intervalTime_Mutation);
                        if (!switch_RefreshMutation)
                        {
                            break;
                        }
                        ControlTools.SetMousePositionAndClickLeftButton(Point_RefreshMutationX, Point_RefreshMutationY, screens, Index_CurrentlySelectedDisplay);
                    }
                }, token);
            }
        }

        //快速设置金币截图坐标与大小
        private async void button9_Click(object sender, EventArgs e)
        {
            //启动 开始设置金币截图坐标与大小 标识
            isQuicklySetStartPointAndSize_MoneyScreenshot = true;

            Initializa_ComponentsForDrawing();
            try
            {
                // 调用异步方法并等待绘制完成
                Rectangle RE1 = await StartAndWaitingDraw();
                startPoint_MoneyScreenshotX = RE1.X;
                startPoint_MoneyScreenshotY = RE1.Y;
                width_MoneyScreenshot = RE1.Width;
                height_MoneyScreenshot = RE1.Height;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"出现错误: {ex.Message}");
            }

            Update_TextBox12();
            Update_TextBox13();
            Update_TextBox14();
            Update_TextBox15();

            //关闭 开始设置金币截图坐标与大小 标识
            isQuicklySetStartPointAndSize_MoneyScreenshot = false;
            overlayForm.Dispose();
            labelForm.Dispose();

        }

        //快速设置异常突变截图坐标与大小
        private async void button10_Click(object sender, EventArgs e)
        {
            //启动 开始设置异常突变截图坐标与大小 标识
            isQuicklySetStartPointAndSize_MutationScreenshot = true;
            Initializa_ComponentsForDrawing();
            try
            {
                // 调用异步方法并等待绘制完成
                Rectangle RE1 = await StartAndWaitingDraw();
                startPoint_MutationScreenshotX = RE1.X;
                startPoint_MutationScreenshotY = RE1.Y;
                width_MutationScreenshot = RE1.Width;
                height_MutationScreenshot = RE1.Height;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"出现错误: {ex.Message}");
            }

            Update_TextBox16();
            Update_TextBox17();
            Update_TextBox18();
            Update_TextBox19();
            //关闭 开始设置异常突变截图坐标与大小 标识
            isQuicklySetStartPointAndSize_MutationScreenshot = false;
            overlayForm.Dispose();
            labelForm.Dispose();
        }

        //快速设置异常突变刷新按钮坐标
        private async void button11_Click(object sender, EventArgs e)
        {
            //启动 开始设置异常突变刷新按钮坐标 标识
            isQuickllySetPoint_RefreshMutation = true;
            Initializa_ComponentsForDrawing();
            try
            {
                // 调用异步方法并等待绘制完成
                Point PO = await StartAndWaitingClick();
                Point_RefreshMutationX = PO.X;
                Point_RefreshMutationY = PO.Y;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"出现错误: {ex.Message}");
            }

            Update_TextBox25();
            Update_TextBox26();
            //关闭 开始设置异常突变刷新按钮坐标 标识
            isQuickllySetPoint_RefreshMutation = false;
            overlayForm.Dispose();
            labelForm.Dispose();
        }

        //快速设置异常突变进化按钮坐标
        private async void button12_Click(object sender, EventArgs e)
        {
            //启动 开始设置异常突变进化按钮坐标 标识
            isQuickllySetPoint_EvolutionMutation = true;
            Initializa_ComponentsForDrawing();
            try
            {
                // 调用异步方法并等待绘制完成
                Point PO = await StartAndWaitingClick();
                Point_EvolutionMutationX = PO.X;
                Point_EvolutionMutationY = PO.Y;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"出现错误: {ex.Message}");
            }

            Update_TextBox27();
            Update_TextBox28();
            //关闭 开始设置异常突变进化按钮坐标 标识
            isQuickllySetPoint_EvolutionMutation = false;
            overlayForm.Dispose();
            labelForm.Dispose();
        }

        //快速设置商店刷新按钮坐标
        private async void button13_Click(object sender, EventArgs e)
        {
            //启动 开始快速设置商店刷新按钮坐标 标识
            isQuickllySetPoint_RefreshStore = true;
            Initializa_ComponentsForDrawing();
            try
            {
                // 调用异步方法并等待绘制完成
                Point PO = await StartAndWaitingClick();
                Point_RefreshStoreX = PO.X;
                Point_RefreshStoreY = PO.Y;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"出现错误: {ex.Message}");
            }

            Update_TextBox31();
            Update_TextBox32();
            //关闭 开始快速设置商店刷新按钮坐标 标识
            isQuickllySetPoint_RefreshStore = false;
            overlayForm.Dispose();
            labelForm.Dispose();
        }

        //快速设置奕子截图坐标与大小
        private async void button8_Click(object sender, EventArgs e)
        {
            //启动 开始快速设置奕子截图坐标与大小 标识
            isQuicklySetStartPointAndSize_CardScreenshot = true;
            Initializa_ComponentsForDrawing();
            try
            {
                // 调用异步方法并等待绘制完成
                Rectangle RE1 = await StartAndWaitingDraw();
                startPoint_CardScreenshotX1 = RE1.X;
                startPoint_CardScreenshotY6 = RE1.Y;
                width_CardScreenshot = RE1.Width;
                height_CardScreenshot = RE1.Height;
                // 显示矩形的坐标信息


            }
            catch (Exception ex)
            {
                MessageBox.Show($"出现错误: {ex.Message}");
            }

            try
            {
                // 调用异步方法并等待绘制完成
                Rectangle RE2 = await StartAndWaitingDraw();
                startPoint_CardScreenshotX2 = RE2.X;
                startPoint_CardScreenshotY6 = RE2.Y;
                width_CardScreenshot = RE2.Width;
                height_CardScreenshot = RE2.Height;
                // 显示矩形的坐标信息


            }
            catch (Exception ex)
            {
                MessageBox.Show($"出现错误: {ex.Message}");
            }

            try
            {
                // 调用异步方法并等待绘制完成
                Rectangle RE3 = await StartAndWaitingDraw();
                startPoint_CardScreenshotX3 = RE3.X;
                startPoint_CardScreenshotY6 = RE3.Y;
                width_CardScreenshot = RE3.Width;
                height_CardScreenshot = RE3.Height;
                // 显示矩形的坐标信息


            }
            catch (Exception ex)
            {
                MessageBox.Show($"出现错误: {ex.Message}");
            }

            try
            {
                // 调用异步方法并等待绘制完成
                Rectangle RE4 = await StartAndWaitingDraw();
                startPoint_CardScreenshotX4 = RE4.X;
                startPoint_CardScreenshotY6 = RE4.Y;
                width_CardScreenshot = RE4.Width;
                height_CardScreenshot = RE4.Height;
                // 显示矩形的坐标信息


            }
            catch (Exception ex)
            {
                MessageBox.Show($"出现错误: {ex.Message}");
            }

            try
            {
                // 调用异步方法并等待绘制完成
                Rectangle RE5 = await StartAndWaitingDraw();
                startPoint_CardScreenshotX6 = RE5.X;
                startPoint_CardScreenshotY6 = RE5.Y;
                width_CardScreenshot = RE5.Width;
                height_CardScreenshot = RE5.Height;
                // 显示矩形的坐标信息


            }
            catch (Exception ex)
            {
                MessageBox.Show($"出现错误: {ex.Message}");
            }
            Update_TextBox8();
            Update_TextBox9();
            Update_TextBox10();
            Update_TextBox11();
            Update_TextBox20();
            Update_TextBox21();
            Update_TextBox22();
            Update_TextBox23();
            ////关闭 开始快速设置奕子截图坐标与大小 标识
            isQuicklySetStartPointAndSize_CardScreenshot = false;
            //重置快速设置奕子截图坐标与大小轮次为1
            times_IsQuicklySetStartPointAndSize_CardScreenshot = 1;
            overlayForm.Dispose();
            labelForm.Dispose();
        }
        //纵向收缩/展开面板
        private void label122_Click(object sender, EventArgs e)
        {
            isWindowExpandedVertically = !isWindowExpandedVertically;  // 切换 希望主窗口 变换的状态
            if (!isWindowExpandedHorizontally && isWindowExpandedVertically)
            {
                isWindowExpandedHorizontally = !isWindowExpandedHorizontally;
                timer2.Start();
                label115.Text = label115.Text == "》" ? "《" : "》";
            }
            //启动变换动画
            timer3.Start();
            //调整 展开/收缩按钮显示内容
            label122.Text = label122.Text == "▲" ? "▼" : "▲";
        }
        //横向收缩/展开面板
        private void label115_Click(object sender, EventArgs e)
        {
            isWindowExpandedHorizontally = !isWindowExpandedHorizontally;  // 切换 希望主窗口 变换的状态
                                                                           //启动变换动画
            if (isWindowExpandedVertically && !isWindowExpandedHorizontally)
            {
                isWindowExpandedVertically = !isWindowExpandedVertically;  // 切换 希望主窗口 变换的状态
                                                                           //启动变换动画
                timer3.Start();
                //调整 展开/收缩按钮显示内容
                label122.Text = label122.Text == "▲" ? "▼" : "▲";
            }
            timer2.Start();
            //调整 展开/收缩按钮显示内容
            label115.Text = label115.Text == "》" ? "《" : "》";
        }
        //鼠标进入横向展开按钮
        private void label115_MouseMove(object sender, MouseEventArgs e)
        {
            label115.ForeColor = Color.LightSeaGreen;

            this.Cursor = Cursors.Hand;
        }
        //鼠标离开横向展开按钮
        private void label115_MouseLeave(object sender, EventArgs e)
        {
            label115.ForeColor = Color.Black;

            this.Cursor = Cursors.Default;
        }
        //鼠标进入纵向展开按钮
        private void label122_MouseMove(object sender, MouseEventArgs e)
        {
            label122.ForeColor = Color.LightSeaGreen;

            this.Cursor = Cursors.Hand;
        }
        //鼠标离开纵向展开按钮
        private void label122_MouseLeave(object sender, EventArgs e)
        {
            label122.ForeColor = Color.Black;
            this.Cursor = Cursors.Default;
        }

        //当选择显示器时触发
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Index_CurrentlySelectedDisplay = comboBox2.SelectedIndex;
            targetScreen = screens[comboBox2.SelectedIndex];
        }


        #endregion
        #region 函数

        /// <summary>
        /// 异步处理每张卡片的截图、OCR识别和UI更新
        /// </summary>
        /// <param name="startPointX"></param>
        /// <param name="startPointY"></param>
        /// <param name="cardID"></param>
        /// <param name="pictureBox"></param>
        /// <param name="textBox"></param>
        /// <returns></returns>
        private async Task Screenshot_Processing_OCR_Compare_GetCard(int startPointX, int startPointY, int cardID, PictureBox pictureBox, TextBox textBox)
        {
            // 异步截图
            Bitmap card = ImageProcessingTools.ImageGRAYToBitmap(Screenshot(startPointX, startPointY, width_CardScreenshot, height_CardScreenshot, screens, Index_CurrentlySelectedDisplay));
            Debug.WriteLine($"截图完成-{cardID}");

            // 异步OCR识别
            string result = OCRTools.OCRRecognition(0, card);
            result = TextProcessingTools.ConvertResult_CardName(result);
            Debug.WriteLine($"OCR完成-{cardID}-{result}");

            // 更新UI，必须在主线程上进行
            this.Invoke((MethodInvoker)(() =>
            {
                textBox.Text = result;
                pictureBox.Image = card;
            }));

            // 调用判断并拿牌
            await CompareAndGetCard(result, startPointX);
        }
        /// <summary>
        /// 截图并处理
        /// </summary>
        /// <param name="startPointX"></param>
        /// <param name="startPointY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="screens"></param>
        /// <param name="screenID"></param>
        /// <returns></returns>
        private Image<Gray, byte> Screenshot(int startPointX, int startPointY, int width, int height, Screen[] screens, int screenID)
        {
            // 截图
            Bitmap screenShot = ImageProcessingTools.AreaScreenshots(startPointX, startPointY, width, height, screens, Index_CurrentlySelectedDisplay);
            // 将截图转换为 OpenCV 图像对象
            Image<Bgr, byte> sourceImage = ImageProcessingTools.BitmapToImageBGR(screenShot);
            Image<Bgr, byte> scallingImage = ImageProcessingTools.Scaling(sourceImage, 5);
            Image<Gray, byte> grayscaleImage = ImageProcessingTools.Grayscale(scallingImage);
            Image<Gray, byte> binarizationImage = ImageProcessingTools.Binarization(grayscaleImage);
            return ImageProcessingTools.GaussianBlur(binarizationImage, 5);

        }
        /// <summary>
        /// 比较并拿牌
        /// </summary>
        /// <param name="result"></param>
        /// <param name="startPoint_CardScreenshotX"></param>
        /// <returns></returns>
        private async Task CompareAndGetCard(string result, int startPoint_CardScreenshotX)
        {
            for (int i = 0; i < CheckList.Length; i++)
            {
                if (CheckList[i])
                {

                    if (TextProcessingTools.DetermineSimilarityLevenshtein_CardName(result, cardName[i]))
                    {
                        Debug.WriteLine($"发现{cardName[i]},开始拿取");
                        // 鼠标操作
                        ControlTools.SetMousePosition(startPoint_CardScreenshotX + width_CardScreenshot / 2, startPoint_CardScreenshotY6 - height_CardScreenshot * 2, screens, Index_CurrentlySelectedDisplay);
                        Debug.WriteLine($"移动完成");
                        await Task.Delay(intervalTime_MouseMove);
                        // 执行点击操作，逐个点击并等待
                        await Click11Times();

                        await Task.Delay(intervalTime_GetCard);
                        break;
                    }

                }
            }
        }
        /// <summary>
        /// 点击11次
        /// </summary>
        /// <returns></returns>
        private async Task Click11Times()
        {
            // 执行多次点击操作，每次点击之间都加上等待
            await Click1Time();
            await Click1Time();
            await Click1Time();
            await Click1Time();
            await Click1Time();
            await Click1Time();
            await Click1Time();
            await Click1Time();
            await Click1Time();
            await Click1Time();
            await Click1Time();
        }
        /// <summary>
        /// 点击1次
        /// </summary>
        /// <returns></returns>
        private async Task Click1Time()
        {
            ControlTools.MakeMouseLeftButtonDown();
            ControlTools.MakeMouseLeftButtonUp();

        }
        /// <summary>
        /// 检测勾选状态
        /// </summary>
        /// <param name="checkBox"></param>
        /// <returns></returns>
        public bool[] DetectionCheckState(CheckBox[] checkBox)
        {
            //申明一个长度为 单选框数组长度的 bool数组
            bool[] checkList_ = new bool[checkBox.Length];

            //为bool数组的每一项赋值为false
            for (int i = 0; i < checkList_.Length; i++)
            {
                checkList_[i] = false;
            }
            //根据单选框的勾选状态，为勾选表的每一项赋值
            for (int i = 0; i < checkList_.Length; i++)
            {
                if (checkBox[i].CheckState == CheckState.Checked)
                {
                    checkList_[i] = true;

                }
            }
            //返回检测表
            return checkList_;
        }
        /// <summary>
        /// 检测金钱数
        /// </summary>
        /// <param name="moneyCount"></param>
        /// <returns></returns>
        public bool DetectionMoney(out int moneyCount)
        {
            try
            {
                Bitmap image = ImageProcessingTools.AreaScreenshots(startPoint_MoneyScreenshotX, startPoint_MoneyScreenshotY, width_MoneyScreenshot, height_MoneyScreenshot, screens, Index_CurrentlySelectedDisplay);
                //对图片进行处理
                Image<Bgr, byte> sourceImage = ImageProcessingTools.BitmapToImageBGR(image);
                Image<Bgr, byte> scalingImage = ImageProcessingTools.Scaling(sourceImage, 10);
                Image<Gray, byte> lastImage = ImageProcessingTools.ProcessImage_Scaling2x_Grayscale_Binarization_GaussianBlur(scalingImage);
                //接受OCR结果
                string result = OCRTools.OCRRecognitionForFigure(0, ImageProcessingTools.ImageGRAYToBitmap(lastImage));
                if (int.TryParse(result, out moneyCount))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            //若出现错误，则返回无法识别金币，且当且金币设置为0
            catch
            {
                moneyCount = 0;
                return false;
            }
        }
        /// <summary>
        /// OCR识别异常突变的函数
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task OCR_Mutation(CancellationToken token)
        {
            //获取用户输入在异常突变文本框的内容并根据|分割成多份，赋值给String[]数组
            string[] mutationTextArray = TextProcessingTools.GetTextArray(textBox24);
            Bitmap screenShot = ImageProcessingTools.AreaScreenshots(startPoint_MutationScreenshotX, startPoint_MutationScreenshotY, width_MutationScreenshot, height_MutationScreenshot, screens, Index_CurrentlySelectedDisplay);
            Image<Bgr, byte> sourceImage = ImageProcessingTools.BitmapToImageBGR(screenShot);
            Image<Bgr, byte> scallingImage = ImageProcessingTools.Scaling(sourceImage, 7);
            Image<Gray, byte> LastImage = ImageProcessingTools.ProcessImage_Scaling2x_Grayscale_Binarization_GaussianBlur(scallingImage);

            string result = OCRTools.OCRRecognition(0, ImageProcessingTools.ImageGRAYToBitmap(LastImage));
            result = TextProcessingTools.ConvertResult_Mutation(result);
            this.Invoke((MethodInvoker)delegate
            {
                label102.Text = "当前识别到的异变：" + result;
            });

            for (int i = 0; i < mutationTextArray.Length; i++)
            {

                if (TextProcessingTools.DetermineSimilarityLevenshtein_MutationName(result, mutationTextArray[i]))
                {
                    ControlTools.SetMousePosition(Point_EvolutionMutationX, Point_EvolutionMutationY, screens, Index_CurrentlySelectedDisplay);
                    await Task.Delay(intervalTime_MouseMove);
                    ControlTools.MakeMouseLeftButtonDown();
                    await Task.Delay(intervalTime_MouseDown);
                    ControlTools.MakeMouseLeftButtonUp();
                    switch_RefreshMutation = false;
                    this.Invoke((MethodInvoker)delegate
                    {
                        button7.Text = "启动";
                    });
                    break;
                }
            }

        }
        /// <summary>
        /// 使奕子选择单选框全部取消勾选
        /// </summary>
        private void ClearCheckBoxs()
        {
            for (int i = 0; i < CheckBoxs.Length; i++)
            {
                CheckBoxs[i].CheckState = CheckState.Unchecked;
            }
            CheckList = DetectionCheckState(CheckBoxs);
            textBox24.Text = "";
        }
        /// <summary>
        /// 保存阵容到本地文件
        /// </summary>
        private void SaveLineUpLocally()
        {
            var lines = CheckList.Select(state => state.ToString());
            File.WriteAllLines(($"CheckList{dropDownBoxItem_CurrentlySelected}.txt"), lines);
            string mutationText = textBox24.Text;
            File.WriteAllText(($"MutationText{dropDownBoxItem_CurrentlySelected}.txt"), mutationText);
        }
        /// <summary>
        /// 从本地文件读取阵容名称
        /// </summary>
        private void ReadLineUpNameFromLocal()
        {

            if (File.Exists("zhenRongMingCheng.txt"))
            {

                string[] lineUpName = File.ReadAllLines("zhenRongMingCheng.txt");

                comboBox1.Items.Clear();
                for (int i = 0; i < lineUpName.Length; i++)
                {
                    comboBox1.Items.Add(lineUpName[i]);
                }
            }
            else
            {
                string[] lineUpName = new string[]
                {
                        "阵容1","阵容2","阵容3","阵容4","阵容5","阵容6","阵容7","阵容8","阵容9","阵容10","阵容11",
                };
                File.WriteAllLines("zhenRongMingCheng.txt", lineUpName);
                lineUpName = File.ReadAllLines("zhenRongMingCheng.txt");

                comboBox1.Items.Clear();
                for (int i = 0; i < lineUpName.Length; i++)
                {
                    comboBox1.Items.Add(lineUpName[i]);
                }
            }

        }
        /// <summary>
        /// 保存阵容名称到本地文件
        /// </summary>
        private void SaveLineUpNameLocally()
        {
            if (dropDownBoxItem_CurrentlySelected >= 0)
            {
                if (File.Exists("zhenRongMingCheng.txt"))
                {
                    string currentText = comboBox1.Text;
                    string[] lineUpName = File.ReadAllLines("zhenRongMingCheng.txt");
                    lineUpName[dropDownBoxItem_CurrentlySelected] = currentText;
                    comboBox1.Items.Clear();
                    for (int i = 0; i < lineUpName.Length; i++)
                    {
                        comboBox1.Items.Add(lineUpName[i]);
                    }
                    File.WriteAllLines("zhenRongMingCheng.txt", lineUpName);
                }
                else
                {
                    string[] lineUpName = new string[]
                    {
                        "阵容1","阵容2","阵容3","阵容4","阵容5","阵容6","阵容7","阵容8","阵容9","阵容10","阵容11",
                    };
                    File.WriteAllLines("zhenRongMingCheng.txt", lineUpName);
                    string currentText = comboBox1.Text;
                    lineUpName = File.ReadAllLines("zhenRongMingCheng.txt");
                    lineUpName[dropDownBoxItem_CurrentlySelected] = currentText;
                    comboBox1.Items.Clear();
                    for (int i = 0; i < lineUpName.Length; i++)
                    {
                        comboBox1.Items.Add(lineUpName[i]);
                    }
                    File.WriteAllLines("zhenRongMingCheng.txt", lineUpName);
                }

            }
        }

        /// <summary>
        /// 从本地文件读取阵容
        /// </summary>
        private void ReadLineUpFromLocal()
        {
            if (dropDownBoxItem_CurrentlySelected >= 0 && dropDownBoxItem_CurrentlySelected <= 10)
            {
                if (File.Exists($"CheckList{dropDownBoxItem_CurrentlySelected}.txt"))
                {
                    var lines = File.ReadAllLines($"CheckList{dropDownBoxItem_CurrentlySelected}.txt");
                    for (int i = 0; i < lines.Length && i < CheckBoxs.Length; i++)
                    {
                        CheckBoxs[i].Checked = bool.Parse(lines[i]);
                    }
                    CheckList = DetectionCheckState(CheckBoxs);
                }
                else
                {
                    bool[] checkList_ = new bool[63];
                    for (int i = 0; i < checkList_.Length; i++)
                    {
                        checkList_[i] = false;
                    }
                    var lines = checkList_.Select(state => state.ToString());
                    File.WriteAllLines(($"CheckList{dropDownBoxItem_CurrentlySelected}.txt"), lines);
                    var liness = File.ReadAllLines(($"CheckList{dropDownBoxItem_CurrentlySelected}.txt"));
                    for (int i = 0; i < liness.Length && i < CheckBoxs.Length; i++)
                    {
                        CheckBoxs[i].Checked = bool.Parse(liness[i]);
                    }
                    checkList_ = DetectionCheckState(CheckBoxs);
                }
                if (File.Exists($"MutationText{dropDownBoxItem_CurrentlySelected}.txt"))
                {
                    textBox24.Text = File.ReadAllText($"MutationText{dropDownBoxItem_CurrentlySelected}.txt");
                }
                else 
                {
                    textBox24.Text = "";
                    File.WriteAllText(($"MutationText{dropDownBoxItem_CurrentlySelected}.txt"), textBox24.Text);
                    textBox24.Text = File.ReadAllText($"MutationText{dropDownBoxItem_CurrentlySelected}.txt");
                }
            }

        }



        #endregion
        #region 矩形绘制
        public bool isDrawing = false;//是否处于绘制状态
        public bool waitClick = false;//是否处于等待点击状态
        public bool isQuicklySetStartPointAndSize_CardScreenshot = false;//是否处于快速设置奕子截图坐标与大小状态
        public int times_IsQuicklySetStartPointAndSize_CardScreenshot = 1;//当前快速设置奕子截图坐标与大小的轮次
        public bool isQuicklySetStartPointAndSize_MoneyScreenshot = false;//是否处于快速设置金币截图坐标与大小状态
        public bool isQuicklySetStartPointAndSize_MutationScreenshot = false;//是否处于快速设置异常突变截图坐标与大小状态
        public bool isQuickllySetPoint_RefreshMutation = false;//是否处于快速设置刷新异常突变按钮坐标状态
        public bool isQuickllySetPoint_EvolutionMutation = false;//是否处于快速设置进化异常突变按钮坐标状态
        public bool isQuickllySetPoint_RefreshStore = false;//是否处于快速设置商店刷新按钮坐标状态

        public Point startPoint;//开始坐标
        public Point endPoint;//结束坐标
        public Form overlayForm;  // 用于创建半透明的覆盖层的窗口
        public Form labelForm;//用于显示提示信息的窗口
        public Label showTipLabel;//用于显示提示信息的Label
        public Label showPointLabel;  // 用于显示坐标的Label                                                 
        public Rectangle currentRectangle; // 用于保存绘制中的矩形  
        public void Initializa_ComponentsForDrawing()
        {

            showTipLabel = new Label
            {
                ForeColor = Color.Red,  // 设置字体颜色为白色
                BackColor = Color.Black, // 明亮的蓝色背景
                Font = new Font("Arial", 14, FontStyle.Bold),  // 设置字体大小和加粗
                Size = new(700, 30),
                TextAlign = ContentAlignment.MiddleLeft,  // 使文本居中
            };
            overlayForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,  // 无边框
                BackColor = Color.Black,  // 设置背景颜色为黑色
                WindowState = FormWindowState.Maximized,  // 设置为最大化窗体，覆盖整个屏幕              
                ShowInTaskbar = false,  // 不显示在任务栏
                ControlBox = false,  // 禁用控制框
                TopMost = true,  // 确保在最上层

                Opacity = 0.5,  // 设置透明度为 0，窗体完全透明                                                              
            };

            // 创建一个新的窗体用于显示坐标标签
            labelForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,  // 无边框
                BackColor = Color.FromArgb(255, 50, 50, 255),   // 设置背景颜色为鲜艳的红色
                WindowState = FormWindowState.Normal,  // 正常状态，避免最大化                 
                ShowInTaskbar = false,  // 不显示在任务栏
                ControlBox = false,  // 禁用控制框
                TopMost = true,  // 确保在最上层

                Owner = overlayForm,  // 设置 overlayForm 为 labelForm 的父窗体
            };


            // 创建坐标显示标签
            showPointLabel = new Label
            {
                ForeColor = Color.White,  // 设置字体颜色为白色
                BackColor = Color.FromArgb(255, 50, 50, 255), // 明亮的蓝色背景
                Font = new Font("Arial", 14, FontStyle.Bold),  // 设置字体大小和加粗
                Location = new(0, 0),
                TextAlign = ContentAlignment.MiddleCenter,  // 使文本居中
            };


            //将coordinateLabel添加到 labelForm
            labelForm.Controls.Add(showPointLabel);
            overlayForm.Controls.Add(showTipLabel);

            // 绑定鼠标事件到覆盖层窗体
            overlayForm.MouseDown += new MouseEventHandler(BackForm_MouseDown);
            overlayForm.MouseMove += new MouseEventHandler(BackForm_MouseMove);
            overlayForm.MouseUp += new MouseEventHandler(BackForm_MouseUp);
            overlayForm.Paint += new PaintEventHandler(BackForm_Paint);

        }
        // 异步方法，启动绘制并返回矩形信息
        public async Task<Point> StartAndWaitingClick()
        {
            currentRectangle = Rectangle.Empty;
            // 设置绘制状态
            waitClick = true;

            // 设置 overlayForm 的位置和大小
            overlayForm.StartPosition = FormStartPosition.Manual;
            overlayForm.Location = new(targetScreen.Bounds.Location.X, targetScreen.Bounds.Location.Y); // 设置位置为显示器的左上角
            overlayForm.Size = targetScreen.Bounds.Size;  // 设置窗体的大小为显示器的分辨率

            // 设置 labelForm 的位置
            labelForm.StartPosition = FormStartPosition.Manual;
            labelForm.Location = new(targetScreen.Bounds.Left, targetScreen.Bounds.Top + 1400); // 可以根据需要调整位置

            labelForm.Size = new Size(360, 30); // 设置 labelForm 的大小

            // 显示窗体
            overlayForm.Show();
            labelForm.Show();
            labelForm.BringToFront();
            overlayForm.Cursor = Cursors.Cross;
            showTipLabel.Show();

            showPointLabel.Size = new(360, 30);
            if (isQuickllySetPoint_RefreshMutation)
            {
                showTipLabel.Text = "请点击异常突变刷新按钮的中心点";
            }
            if (isQuickllySetPoint_EvolutionMutation)
            {
                showTipLabel.Text = "请点击异常突变进化按钮的中心点";
            }
            if (isQuickllySetPoint_RefreshStore)
            {
                showTipLabel.Text = "请点击商店刷新按钮的中心点";
            }
            // 等待直到绘制完成
            while (waitClick)
            {
                await Task.Delay(50); // 等待绘制完成
            }

            // 绘制完成后，返回矩形信息
            return startPoint;
        }
        public async Task<Rectangle> StartAndWaitingDraw()
        {
            currentRectangle = Rectangle.Empty;
            // 设置绘制状态
            isDrawing = true;

            // 设置 overlayForm 的位置和大小
            overlayForm.StartPosition = FormStartPosition.Manual;
            overlayForm.Location = new(targetScreen.Bounds.Location.X, targetScreen.Bounds.Location.Y); // 设置位置为显示器的左上角
            overlayForm.Size = targetScreen.Bounds.Size;  // 设置窗体的大小为显示器的分辨率

            // 设置 labelForm 的位置
            labelForm.StartPosition = FormStartPosition.Manual;
            labelForm.Location = new(targetScreen.Bounds.Left, targetScreen.Bounds.Top + 1400); // 可以根据需要调整位置

            labelForm.Size = new Size(360, 30); // 设置 labelForm 的大小

            // 显示窗体
            overlayForm.Show();
            labelForm.Show();
            labelForm.BringToFront();
            overlayForm.Cursor = Cursors.Cross;
            showTipLabel.Show();

            showPointLabel.Size = new(360, 30);
            if (isQuicklySetStartPointAndSize_CardScreenshot)
            {
                showTipLabel.Text = $"请框选商店从左到右数第{times_IsQuicklySetStartPointAndSize_CardScreenshot}张奕子卡片";
                times_IsQuicklySetStartPointAndSize_CardScreenshot++;
            }
            if (isQuicklySetStartPointAndSize_MoneyScreenshot)
            {
                showTipLabel.Text = "请框选商店顶部显示的剩余金币数";
            }
            if (isQuicklySetStartPointAndSize_MutationScreenshot)
            {
                showTipLabel.Text = "请框选显示的异常突变名称，同时请横向预留更多空间";
            }

            // 等待直到绘制完成
            while (isDrawing)
            {
                await Task.Delay(50); // 等待绘制完成
            }

            // 绘制完成后，返回矩形信息
            return currentRectangle;
        }

        public void BackForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (waitClick && e.Button == MouseButtons.Left)
            {
                startPoint = e.Location; // 记录鼠标按下时的位置               
            }
            if (isDrawing && e.Button == MouseButtons.Left)
            {
                startPoint = e.Location; // 记录鼠标按下时的位置
                currentRectangle = new Rectangle(startPoint, new Size(0, 0)); // 初始化矩形               
            }

        }
        // 鼠标移动事件，实时绘制矩形
        public void BackForm_MouseMove(object sender, MouseEventArgs e)
        {

            if ((waitClick || isDrawing) && e.Button == MouseButtons.None)
            {
                labelForm.Location = new Point(e.X + targetScreen.Bounds.X, e.Y + targetScreen.Bounds.Y - showPointLabel.Height - 20); // 显示在鼠标上方
                showPointLabel.Text = $"鼠标坐标X：{e.Location.X}, 鼠标坐标Y：{e.Location.Y}";
            }

            // 如果按下鼠标并正在绘制矩形，labelForm 固定在矩形的左上角上方

            else if (isDrawing && e.Button == MouseButtons.Left)
            {
                endPoint = e.Location;

                // 更新矩形的大小
                currentRectangle.Width = Math.Abs(endPoint.X - startPoint.X);
                currentRectangle.Height = Math.Abs(endPoint.Y - startPoint.Y);

                // 设置矩形的左上角坐标
                currentRectangle.X = Math.Min(startPoint.X, endPoint.X);
                currentRectangle.Y = Math.Min(startPoint.Y, endPoint.Y);



                // 让 labelForm 固定在矩形左上角上方
                labelForm.Location = new Point(currentRectangle.Left + targetScreen.Bounds.X, currentRectangle.Top + targetScreen.Bounds.Y - showPointLabel.Height - 20);
                labelForm.Size = new(570, 30);
                showPointLabel.Size = new(570, 30);

                showPointLabel.Text = $"鼠标坐标X：{currentRectangle.Left}, 鼠标坐标Y：{currentRectangle.Top},宽度：{currentRectangle.Width}，高度：{currentRectangle.Height}";
                overlayForm.Invalidate();  // 请求重绘
            }
            showTipLabel.Location = new Point(labelForm.Location.X - targetScreen.Bounds.X, labelForm.Location.Y - 40 - targetScreen.Bounds.Y);
        }
        // 鼠标松开事件，绘制矩形并显示坐标
        public void BackForm_MouseUp(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                overlayForm.Cursor = Cursors.Default;  // 恢复鼠标指针
                overlayForm.Hide();
                labelForm.Hide();
            }
            if (waitClick && e.Button == MouseButtons.Left)
            {
                waitClick = false;
            }
            if (isDrawing && e.Button == MouseButtons.Left)
            {
                isDrawing = false;  // 结束绘制状态                                                          
            }

        }
        // 重绘事件，绘制矩形
        public void BackForm_Paint(object sender, PaintEventArgs e)
        {
            if (isDrawing || currentRectangle != Rectangle.Empty)
            {
                // 创建一个填充矩形的画刷               
                // 填充矩形
                e.Graphics.FillRectangle(Brushes.White, currentRectangle);
                // 绘制矩形的边框
                e.Graphics.DrawRectangle(Pens.Red, currentRectangle);
            }
        }

        #endregion
        #region 未启用的组件响应事件/函数
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void button14_Click_2(object sender, EventArgs e)
        {


        }
        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }
        private void button14_Click_1(object sender, EventArgs e)
        {


        }
        private void button14_Click(object sender, EventArgs e)
        {

        }
        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox8_TextChanged_1(object sender, EventArgs e)
        {

        }
        private void tabPage7_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox33_Click(object sender, EventArgs e)
        {

        }
        private void comboBox1_Enter_1(object sender, EventArgs e)
        {

        }
        private void comboBox1_TextChanged(object sender, EventArgs e)
        {



        }
        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void label64_Click(object sender, EventArgs e)
        {

        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }
        private void label79_Click(object sender, EventArgs e)
        {

        }
        #endregion











    }
}
