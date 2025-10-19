using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBarTop_TopBar : MonoBehaviour
{
    public Text textUserNickname;
    public Text textGameMode;
    public Text textNowTime;//当前时间
    public Text textDate;//游戏日期
    public Text textTotalTime;//游戏总时长
    public Text textCoin;
    public Text textMapTitle;
    public Button btnCoin;
    public Button btnSetting;//设置
    public Button btnHint;//提醒
    public Button btnBackpack;//背包
    public Button btnMap;//地图
    public Button btnMail;//邮件
    public Button btnGoto;//前往目的地
    public Button btnClose;
    public Slider sliderCoin;
    public RawImage rawImageMap;
    public GameObject goMap;
    public RectTransform rectTargetLand;
}
