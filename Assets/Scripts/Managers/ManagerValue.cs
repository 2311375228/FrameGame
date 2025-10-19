using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerValue
{
    public const int intInitYear = 290;

    public static EnumGameMode enumGameMode;
    public static EnumLanguageType enumLanguage = EnumLanguageType.English;
    public static Camera cameraEmployee;
    public static Camera cameraMap;
    public static float[] floMouseSpeeds = new float[] { 0.0002f, 0.0008f, 0.0015f, 0.003f, 0.005f, 0.006f, 0.008f, 0.010f, 0.012f };
    public static Vector2[] vecScreens = new Vector2[] { new Vector2(1024, 576), new Vector2(1366, 768), new Vector2(1920, 1080), new Vector2(2560, 1440) };
    public static float floTouchSpeed;//鼠标滑动速度
    public static bool booMoveCamera = true;
    public static bool booGamePlaying;
    public static int intDay = 15;
    public static int intMonth;
    public static int intYear;
    public static int intTotalDay = 0;
    public static int intTaskRank = 1;
    public static long longEndYearCoin;//主要是税收
    public static long longCoinMax = 2 * (int)Mathf.Pow(10, 5);
    public static int intNPCSellProductCount;//一年出售商品的总数
    public static int intGroundCount;//购买土地数量
    public static int intNewMailCount;//有多少新增邮件
    public static Setting setting;//设置
    public static System.Action<float> SetFloAudio;
    public static System.Action<float> SetFloBackgroundMusic;
    public static List<ViewBarTop_ItemMail.Mail> listMail = new List<ViewBarTop_ItemMail.Mail>();
    public static System.Action<ControllerCamera.EnumCameraPosition> actionCameraLocation;
    public static JsonSaveGame.GameRoot saveGame;
    static string _strSavePath = Application.persistentDataPath + "/";
    static string _strSaveStreamingAssets = Application.dataPath + "/Editor/GameFile/";
    public static string GetSavePath
    {
        get
        {
            Debug.Log(_strSavePath);
            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
            {
                return _strSaveStreamingAssets;
            }
            else
            {
                return _strSavePath;
            }
        }
    }
    //随机地图 也是创建地图 参数1=不能建造的地,2=阻碍物数量,3矿点数量
    public static System.Action<int, int, int> ActionRandomBuildMap;
    //读取保存的游戏
    public static System.Action<PropertiesGround[]> ActionReadSaveGame;

    public static System.Action<PropertiesEmployee> ActionRoleTexture;

    //ViewBuildInfo或者ViewBuildFactoryInfo执行
    //方法在ControllerGround中实现
    public static System.Action<int, MGViewToBuildBase> actionGround;
    //BuildBaseFarm中执行
    //方法在ViewBUildInfo中执行
    public static System.Action<EventBuildToViewBase> actionViewBuildFarmInfo;
    public static System.Action<EventBuildToViewBase> actionViewBuildPasture;
    public static System.Action<EventBuildToViewBase> actionViewBuildFactory;
    public static System.Action<EventBuildToViewBase> actionViewBuildShop;
    public static System.Action<EventBuildToViewBase> actionViewBuildEquipment;

    public static System.Action<EventBuildToViewBase> actionViewBuildMain;
    public static System.Action<ViewGroundToBuildMainDateBase> actionViewBuildMainDate;

    public static System.Action actionUpdateShopProduct = () => { };
    public static System.Action<EnumAudio> actionAudio;
    public static System.Action<EnumAudioCombat> actionAudioCombat;

    public static int intDungeonPointIndex;//任务点序号
    public static int intDungeonID;//任务点ID
    //战斗初始化 int[] = intEmployeeID
    public static System.Action<int[], int, int> actionInitCombat;
    //
    public static System.Action<CombatRoleShowData> actionCombatShow;

    static BackpackGrid.EnumEquipmentCombat[] enumCombats = new BackpackGrid.EnumEquipmentCombat[] {
        BackpackGrid.EnumEquipmentCombat.HP,
        BackpackGrid.EnumEquipmentCombat.ATK,
        BackpackGrid.EnumEquipmentCombat.MP,
        BackpackGrid.EnumEquipmentCombat.Speed,
    };

    /// <summary>
    /// 设置装备值
    /// </summary>
    public static void SetEquipmentItem(int intEquipmentID, BackpackGrid grid)
    {
        JsonValue.TableEquipmentItem item = ManagerCombat.Instance.GetEquipmentItem(intEquipmentID);
        grid.enumStockType = (EnumKnapsackStockType)item.intKnaspackStockType;
        grid.intPrice = item.intPrice;
        grid.intID = item.intEquipmentID;
        grid.intRank = 1;
        grid.intCount = 1;
        grid.intLimitCount = item.intKnapsackGridLimitCount;
        grid.icon = item.strICON;
        grid.strName = item.strNameChina;
        for (int i = 0; i < enumCombats.Length; i++)
        {
            if (!grid.equipment.dicEquipment.ContainsKey(enumCombats[i]))
            {
                grid.equipment.dicEquipment.Add(enumCombats[i], 0);
            }
            switch (enumCombats[i])
            {
                case BackpackGrid.EnumEquipmentCombat.HP:
                    grid.equipment.dicEquipment[enumCombats[i]] = item.intHP;
                    break;
                case BackpackGrid.EnumEquipmentCombat.ATK:
                    grid.equipment.dicEquipment[enumCombats[i]] = item.intATK;
                    break;
                case BackpackGrid.EnumEquipmentCombat.MP:
                    grid.equipment.dicEquipment[enumCombats[i]] = item.intMP;
                    break;
                case BackpackGrid.EnumEquipmentCombat.Speed:
                    grid.equipment.dicEquipment[enumCombats[i]] = item.intSpeed;
                    break;
            }
        }
        grid.equipment.strContent = item.strContentChina;
    }
    /// <summary>
    /// 设置产品值
    /// </summary>
    public static void SetProductItem(int intProductID, BackpackGrid grid)
    {
        JsonValue.DataTableBackPackItem item = ManagerProduct.Instance.GetProductTableItem(intProductID);
        grid.intPrice = ManagerCompound.Instance.GetProductPrice(item.intProductID);
        grid.intID = item.intProductID;
        grid.intRank = 1;
        grid.intCount = 0;
        grid.enumStockType = (EnumKnapsackStockType)item.intProductType;
        grid.intLimitCount = item.intKnapsackGridLimitCount;
        grid.icon = item.strIconName;
        grid.strName = ManagerProduct.Instance.GetName(intProductID, false);
        grid.product.strInfo = item.strInfoChina;
    }
    /// <summary>
    /// 玩法设定,副本攻击次数增加,则增加血量
    /// </summary>
    /// <returns></returns>
    public static int EnemyHP(int intWinCount, int intHP)
    {
        return intHP;//intHP + intWinCount * (int)(intHP * 0.1f);
    }

    /// <summary>
    /// 拆除建筑时的回收金额
    /// </summary>
    public static int DemolishBuildRecycleCoin(int intPrice)
    {
        return (int)(intPrice * 0.8f);
    }

    /// <summary>
    /// 拆除土地标记的回收金额
    /// boo=true：购买  boo=false：赎回
    /// </summary>
    public static long DemolishLandrecycleCoin(int intGroundCount, bool boo)
    {
        //每5块土地上张一个台阶
        //long intPrice_1 = (int)Mathf.Pow(10, intGroundCount / 9 + 1);
        //intPrice_1 = intPrice_1 + (intPrice_1 / 10) * (intGroundCount % 9);
        long intPrice_1 = 0;// 100 + (intGroundCount / 10 + 1) * 9 * (100 * (intGroundCount / 10 + 1));
        if (intGroundCount / 10 == 0)
        {
            intPrice_1 = 100 + intGroundCount * 30;
        }
        else if (intGroundCount / 10 >= 0 && intGroundCount / 10 < 20)
        {
            intPrice_1 = 100 + (intGroundCount / 10) * 10 * 60;
            intPrice_1 = intPrice_1 + (intGroundCount % 10 * ((intGroundCount / 10) * 60));
        }
        else if (intGroundCount / 10 >= 20 && intGroundCount / 10 < 50)
        {
            intPrice_1 = 100 + (intGroundCount / 10) * 10 * 120;
            intPrice_1 = intPrice_1 + (intGroundCount % 10 * ((intGroundCount / 10) * 120));
        }
        else if (intGroundCount / 10 >= 50 && intGroundCount / 10 < 100)
        {
            intPrice_1 = 100 + (intGroundCount / 10) * 10 * 350;
            intPrice_1 = intPrice_1 + (intGroundCount % 10 * ((intGroundCount / 10) * 350));
        }
        else if (intGroundCount / 10 >= 100 && intGroundCount / 10 < 200)
        {
            intPrice_1 = 100 + (intGroundCount / 10) * 10 * 1600;
            intPrice_1 = intPrice_1 + (intGroundCount % 10 * ((intGroundCount / 10) * 1600));
        }
        else if (intGroundCount / 10 >= 200 && intGroundCount / 10 < 300)
        {
            intPrice_1 = 100 + (intGroundCount / 10) * 10 * 2600;
            intPrice_1 = intPrice_1 + (intGroundCount % 10 * ((intGroundCount / 10) * 2600));
        }
        else if (intGroundCount / 10 >= 300)
        {
            intPrice_1 = 100 + (intGroundCount / 10) * 10 * 5200;
            intPrice_1 = intPrice_1 + (intGroundCount % 10 * ((intGroundCount / 10) * 5200));
        }



        if (boo == false)
        {
            if (intPrice_1 <= Mathf.Pow(10, 3))
            {
                intPrice_1 = (int)(intPrice_1 * 0.8f);
            }
            else if (intPrice_1 > Mathf.Pow(10, 3) && intPrice_1 <= Mathf.Pow(10, 4))
            {
                intPrice_1 -= 300;
            }
            else if (intPrice_1 > Mathf.Pow(10, 4) && intPrice_1 <= Mathf.Pow(10, 5))
            {
                intPrice_1 -= 800;
            }
            else if (Mathf.Pow(10, 5) > intPrice_1 && intPrice_1 <= Mathf.Pow(10, 7))
            {
                intPrice_1 -= 6500;
            }
            else if (Mathf.Pow(10, 7) > intPrice_1 && intPrice_1 <= Mathf.Pow(10, 9))
            {
                intPrice_1 -= 15000;
            }
            else
            {
                intPrice_1 -= 50000;
            }

        }
        return intPrice_1;
    }

    /// <summary>
    /// 返回登陆
    /// </summary>
    public static void BackLogin()
    {
        booGamePlaying = false;
        ManagerView.Instance.RemoveAll();
        ManagerView.Instance.Show(EnumView.ViewLogin);
        actionCameraLocation(ControllerCamera.EnumCameraPosition.None);
    }

    public static void SettingSave()
    {
        PlayerPrefs.SetString("setting", JsonUtility.ToJson(setting));
    }
    public static void SettingRead()
    {
        if (PlayerPrefs.HasKey("setting"))
        {
            setting = JsonUtility.FromJson<Setting>(PlayerPrefs.GetString("setting"));
        }
        //if (PlayerPrefs.HasKey("vertion"))
        //{
        //    string strVersion = PlayerPrefs.GetString("vertion");
        //    if (strVersion != Application.version)
        //    {

        //    }
        //}
        //else
        //{

        //}
        if (setting == null)
        {
            setting = new Setting();
            setting.booAudio = true;
            setting.booBackgroudMusic = true;
            setting.floAudio = 1;
            setting.floBackgroundMusic = 1;
            setting.enumLanguage = EnumLanguageType.China;
            setting.intRankSpeed = 4;
            setting.screenType = ScreenType._1080;
            PlayerPrefs.SetString("setting", JsonUtility.ToJson(setting));
        }
        enumLanguage = setting.enumLanguage;
        floTouchSpeed = floMouseSpeeds[setting.intRankSpeed];
    }

    static string[] strEnglishMonth = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
    public static string GetStrEnglishMonth(int intMonth)
    {
        string strTemp = "";
        if (enumLanguage == EnumLanguageType.English)
        {
            strTemp = strEnglishMonth[intMonth - 1];
        }
        else
        {
            strTemp = intMonth.ToString();
        }
        return strTemp;
    }

    public static string GetCoin(float floCoin)
    {
        string strTemp = "";
        if (enumLanguage == EnumLanguageType.English)
        {
            float flo = 1000;
            float flo_end = floCoin % flo;
            float flo_1 = (floCoin / flo) % flo;//Thousand
            float flo_2 = (floCoin / flo / flo) % flo;//million
            float flo_3 = floCoin / flo / flo / flo;
            if ((int)flo_3 > 0)
            {
                strTemp += " " + UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, ((int)flo_3).ToString()) + " " + ManagerLanguage.Instance.GetWord(EnumLanguageWords.Billion);
            }
            if ((int)flo_2 > 0)
            {
                strTemp += " " + UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, ((int)flo_2).ToString()) + " " + ManagerLanguage.Instance.GetWord(EnumLanguageWords.Million);
            }
            if ((int)flo_1 > 0)
            {
                strTemp += " " + UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, ((int)flo_1).ToString()) + " " + ManagerLanguage.Instance.GetWord(EnumLanguageWords.Thousand);
            }
            if (flo_end > 0)
            {
                strTemp += " " + UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, ((int)flo_end).ToString());
            }
        }
        else
        {
            float flo_end = floCoin % 10000;
            float flo_centre = (floCoin / 10000) % 10000;
            float flo_Head = (floCoin / 10000) / 10000;
            if ((int)flo_Head > 0)
            {
                strTemp += UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, ((int)flo_Head).ToString()) + ManagerLanguage.Instance.GetWord(EnumLanguageWords.Yi);
            }
            if ((int)flo_centre > 0)
            {
                strTemp += UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, ((int)flo_centre).ToString()) + ManagerLanguage.Instance.GetWord(EnumLanguageWords.Wan);
            }
            if (flo_end > 0)
            {
                strTemp += UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, ((int)flo_end).ToString());
            }
        }
        return strTemp;
    }

    [System.Serializable]
    public class Setting
    {
        public bool booAudio;
        public bool booBackgroudMusic;
        public float floAudio;
        public float floBackgroundMusic;
        public EnumLanguageType enumLanguage;
        public int intRankSpeed;
        public ScreenType screenType;
    }

    [System.Serializable]
    public enum EnumLanguageType
    {
        /// <summary>
        /// 英文
        /// </summary>
        English,
        /// <summary>
        /// 简体中文
        /// </summary>
        China,
        /// <summary>
        /// 繁体中文
        /// </summary>
        Chinese_,
        /// <summary>
        /// 日文
        /// </summary>
        Japan,
        /// <summary>
        /// 法语
        /// </summary>
        French,
        /// <summary>
        /// 葡萄牙语
        /// </summary>
        Portuguese,
        /// <summary>
        /// 俄语
        /// </summary>
        Russian,
        /// <summary>
        /// 韩语
        /// </summary>
        Korean,
        /// <summary>
        /// 西班牙语
        /// </summary>
        Spanish,
        /// <summary>
        /// 匈牙利语
        /// </summary>
        Hungarian,
        /// <summary>
        /// 意大利语
        /// </summary>
        Italian,
        /// <summary>
        /// 德语
        /// </summary>
        German,
        /// <summary>
        /// 荷兰语
        /// </summary>
        Dutch,
    }

    /// <summary>
    /// 屏幕分辨率类型
    /// </summary>
    public enum ScreenType
    {
        _576,
        _768,
        _1080,
        _1440,
        All,
    }
}
