using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewHouse_Rental : MonoBehaviour
{
    public Text textContent;
    public Text textContentInfo;
    public Button btnRentAdd;
    public Button btnRentReduce;
    public Button btnRentReset;
    public Button btnConfirm;
    public Button btnClose;
    public InputField inputRent;

    public int intPriceChange;
    public int intHouseRank;
    public int intPrice;
    public int intIncome;
    public System.Action<bool,int> SendToBuildRental;
    ViewHintBar.MessageHintBar barMessage = new ViewHintBar.MessageHintBar();

    private void Start()
    {
        btnRentAdd.onClick.AddListener(() =>
        {
            if (intPriceChange + 100 > intHouseRank * 1500)
            {
                intPriceChange = intHouseRank * 1500;
                barMessage.strHintBar = "已达到该出租屋的租金上限制,若要提高上线,请升级出租屋";
                ManagerView.Instance.Show(EnumView.ViewHintBar);
                ManagerView.Instance.SetData(EnumView.ViewHintBar, barMessage);
            }
            else
            {
                intPriceChange += 100;
            }
             inputRent.text = intPriceChange.ToString();
        });
        btnRentReduce.onClick.AddListener(() =>
        {
            if (intPriceChange - 100 < 10)
            {
                intPriceChange = 10;
                barMessage.strHintBar = "不能再减了,最低金额是10";
                ManagerView.Instance.Show(EnumView.ViewHintBar);
                ManagerView.Instance.SetData(EnumView.ViewHintBar, barMessage);
            }
            else
            {
                intPriceChange -= 100;
            }
             inputRent.text = intPriceChange.ToString();
        });
        //输入金额
        inputRent.onValueChanged.AddListener((value) =>
        {
            if (value.Length > 0)
            {
                intPriceChange = int.Parse(value.ToString());
                if (intPriceChange > intHouseRank * 1500)
                {
                    intPriceChange = intHouseRank * 1500;
                     inputRent.text = intPriceChange.ToString();
                    barMessage.strHintBar = "已达到该出租屋的租金上限制,若要提高上线,请升级出租屋";
                    ManagerView.Instance.Show(EnumView.ViewHintBar);
                    ManagerView.Instance.SetData(EnumView.ViewHintBar, barMessage);
                }
            }
        });
        //重设租金
        btnRentReset.onClick.AddListener(() =>
        {
            inputRent.text = intPrice.ToString();
            intPriceChange = intPrice;
        });
        //确认租金
         btnConfirm.onClick.AddListener(() =>
        {
            intPrice = intPriceChange;
            SendToBuildRental(true, intPrice);
        });
        //关闭
         btnClose.onClick.AddListener(() => {  gameObject.SetActive(false); });
    }

    public void Show()
    {

    }
}
