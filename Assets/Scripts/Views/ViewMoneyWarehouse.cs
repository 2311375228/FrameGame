using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewMoneyWarehouse : ViewBase
{

    int intIndexGround;
    MgToBuildMoneyWarehouse mgToBuild = new MgToBuildMoneyWarehouse();
    protected override void Start()
    {
        base.Start();


    }

    public override void Show()
    {
        base.Show();


    }

    public override void SetData(Message message)
    {
        MessageMoneyWarehouse mg = message as MessageMoneyWarehouse;
        if (mg != null)
        {
            intIndexGround = mg.intIndexGround;
        }
    }

    void SendMessageGround()
    {
        ManagerValue.actionGround(intIndexGround, mgToBuild);
    }

    public class MessageMoneyWarehouse : ViewBase.Message
    {
        public int intIndexGround;
    }

    public class MgToBuildMoneyWarehouse : MGViewToBuildBase
    {

    }
}
