using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumView
{
    None,
    ViewLogin,//登陆
    ViewBuyHint,//购买提示
    ViewBarTop,//顶部显示
    ViewHint,//提示
    ViewBuilding,//建造
    ViewHouse,//房屋,包括出租屋,酒吧,城堡
    ViewBuildMain,
    ViewKnapsack,//背包
    ViewProductStockSee,//产看库存
    ViewProductionSee,//查看生产
    ViewShop,//商店
    ViewRecruit,//招聘员工
    ViewEmployeeList,//员工列表
    ViewCombatTeam,//战斗队列
    ViewGameDungeon,//副本信息
    ViewEmployeeAdd,//添加员工
    ViewProduction,//生产视图
    ViewMap,
    ViewTask,//任务界面
    ViewCombat,//战斗
    ViewFactoryEquipment,//锻造装备
    ViewEquipmentInfo,//装备信息
    ViewEmployeeInfo,//员工信息
    ViewEquipmentChange,//更换装备

    ViewSetting,//设置
    ViewESC,
    ViewHintBar,//横幅提醒

    ViewNPCBuy,
    ViewNPCSell,

    ViewBankrupt,//破产
}
