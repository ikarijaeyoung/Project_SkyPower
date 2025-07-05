using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{
[CreateAssetMenu(fileName = "EquipmentDataSO", menuName = "Equipment/EquipmentDataSO")]
public class EquipmentDataSO : ScriptableObject
{
    public int Equip_Id; 
    public string Equip_Grade; 
    public string Equip_Name;
    public string Equip_Type;
    public string Equip_Set_Type;
    public string Equip_Img;
    public int Equip_Level;
    public int Equip_Maxlevel;
    public int Equip_Upgrade_Default;
    public int Equip_Upgrade_Plus;
    public string Stat_Type;
    public int Base_Value;
    public int Per_Level;
    public string Effect_Trigger;
    public string Effect_Timing;
    public string Effect_Type;
    public int Effect_Value;
    public int Effect_Time;
    public int Effect_Chance;
    public string Effect_Desc;
}
}