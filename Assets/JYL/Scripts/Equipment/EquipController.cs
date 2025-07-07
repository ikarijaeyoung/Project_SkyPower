using KYG_skyPower;
using System.Collections.Generic;
using UnityEngine;

// 장비창, 인게임 스테이지 씬에서 사용하는 스크립트
public class EquipController : MonoBehaviour
{
    private EquipmentTableSO equipTable;
    public EquipInfo[] equipData;
    private string tableSOPath = "Inventory/EquipmentTableSO";
    private void Awake()
    {
        Init();
    }
    void Start()
    {

    }

    void Update()
    {

    }
    private void Init()
    {
        equipTable = Resources.Load<EquipmentTableSO>(tableSOPath);
        equipData = new EquipInfo[equipTable.equipmentList.Count];
        CreateEquipInfo();
    }
    public void CreateEquipInfo() // 동적으로 관리할 장비 데이터배열을 생성함. SO로 초기데이터만 가져옴
    {
        int index = 0;
        foreach (EquipmentDataSO equip in equipTable.equipmentList)
        {
            EquipInfo equipInfo = new EquipInfo();
            equipInfo.id = equip.id;
            equipInfo.level = equip.level;
            equipInfo.name = equip.name;
            equipInfo.type = equip.type;
            equipInfo.grade = equip.grade;
            equipInfo.setType = equip.setType;
            equipInfo.icon = equip.icon;
            equipInfo.maxLevel = equip.maxLevel;
            equipInfo.originGold = equip.upgradeGold;
            equipInfo.upgradeGold = equip.upgradeGold;
            equipInfo.upgradeGoldPlus = equip.upgradeGoldPlus;
            equipInfo.equipValue = equip.equipValue;
            equipInfo.originValue = equip.equipValue;
            equipInfo.equipValuePlus = equip.equipValuePlus;
            equipInfo.Effect_Desc = equip.Effect_Desc;
            
            equipData[index] = equipInfo;
            
            index++;
        }
    }
    public void SaveFileInit() //세이브파일 최초 생성 시 작업 SaveCreatePanel에서 관리.
    {
        int index = 0;

        Manager.Game.CurrentSave.equipInfo = new EquipSave[equipData.Length];
        foreach (EquipInfo data in equipData)
        {
            EquipSave tmp = new();
            tmp.id = data.id;
            tmp.level = -1;

            Manager.Game.CurrentSave.equipInfo[index] = tmp;

            index++;
        }
    }
    public void UpdateEquipInfo() // 모든 동적 장비데이터를 최신화. 세이브 불러올때 사용.
    {
        for (int i = 0; i < equipData.Length; i++)
        {
            if (equipData[i].id == Manager.Game.CurrentSave.equipInfo[i].id)
            {
                EquipInfo tmpInfo = equipData[i];
                EquipSave tmpSave = Manager.Game.CurrentSave.equipInfo[i];
                tmpSave.level = equipData[i].level;
                tmpInfo.upgradeGold = equipData[i].level * equipData[i].upgradeGoldPlus;
                tmpInfo.equipValue = equipData[i].originValue + (equipData[i].level - 1) * equipData[i].equipValuePlus;
                equipData[i] = tmpInfo;
                Manager.Game.CurrentSave.equipInfo[i] = tmpSave;
            }
            else
            {
                Debug.LogError($"배열순서 어긋남: {equipData[i].id}, {Manager.Game.CurrentSave.equipInfo}");
            }
        }
    }
    public void UpdateEquipInfo(int id, bool Upgrade = false) // 장비 업그레이드 가능
    {
        for (int i = 0; i < equipData.Length; i++)
        {
            if (equipData[i].id == id)
            {
                EquipSave tmpSave = Manager.Game.CurrentSave.equipInfo[i];
                EquipInfo tmpInfo = equipData[i];
                if (Upgrade) tmpInfo.level++; // maxLevel을 찍으면, UI상에서 기능 막아야 함;
                tmpSave.level = tmpInfo.level;
                tmpInfo.upgradeGold = equipData[i].level * equipData[i].upgradeGoldPlus;
                tmpInfo.equipValue = equipData[i].originValue + (equipData[i].level - 1) * equipData[i].equipValuePlus;
                Manager.Game.CurrentSave.equipInfo[i] = tmpSave;
                equipData[i] = tmpInfo;
                return;
            }
        }
    }
    public void AddEquipment(int id)
    {
        for (int i = 0; i < equipData.Length; i++)
        {
            if (equipData[i].id == id)
            {
                if (equipData[i].level > 0)
                {
                    Manager.Game.CurrentSave.gold += 10;
                }

                else
                {
                    EquipInfo tmpInfo = equipData[i];
                    tmpInfo.level = 1;
                    EquipSave tmp = Manager.Game.CurrentSave.equipInfo[i];
                    tmp.level = 1;
                    Manager.Game.CurrentSave.equipInfo[i] = tmp;
                    equipData[i] = tmpInfo;
                }

                return;
            }
        }
    }
    public List<EquipInfo> GetEquipListByType(EquipType type)
    {
        List<EquipInfo> result = new List<EquipInfo>();
        foreach (EquipInfo info in equipData)
        {
            if (info.type == type)
            {
                EquipInfo temp = info;
                result.Add(temp);
            }
        }
        return result;
    }
}
public struct EquipInfo
{
    public int index;
    public int id;
    public string name;
    public int level;
    public EquipType type;
    public EquipGrade grade;
    public SetType setType;
    public Sprite icon;
    public int maxLevel;
    public int originGold;
    public int upgradeGold;
    public int upgradeGoldPlus;
    public int originValue;
    public int equipValue;
    public int equipValuePlus;
    public string Effect_Desc;
}