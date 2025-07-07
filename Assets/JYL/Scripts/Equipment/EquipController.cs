using KYG_skyPower;
using System.Collections.Generic;
using UnityEngine;

// 장비창, 인게임 스테이지 씬에서 사용하는 스크립트
// 세이브 파일이 변화 되는 구간은 장비를 얻을 때 = Add  ㅇ // 장비를 업그레이드 할 때 = UpdateOneEquipInfo ㅇ 
// 마지막으로, 장비 장착을 변경할 때이다 = GameData.WearingId 이거만 바뀌면 됨
public class EquipController : MonoBehaviour
{
    private EquipmentTableSO equipTable;
    private string tableSOPath = "Inventory/EquipmentTableSO";

    public EquipInfo[] equipData;

    public EquipInfo weapon;
    public EquipInfo armor;
    public EquipInfo accessory;
    private void Awake() { }
    void Start() { }

    void Update() { }
    public void Init()
    {
        equipTable = Resources.Load<EquipmentTableSO>(tableSOPath);
        equipData = new EquipInfo[equipTable.equipmentList.Count];
        weapon = new EquipInfo();
        armor = new EquipInfo();
        accessory = new EquipInfo();
        CreateEquipInfo();
    }
    public void CreateEquipInfo() // 동적으로 관리할 장비 데이터배열을 생성함. SO로 초기데이터만 가져옴
    {
        int index = 0;
        foreach (EquipmentDataSO equip in equipTable.equipmentList)
        {
            EquipInfo equipInfo = new EquipInfo();
            equipInfo.id = equip.id;
            equipInfo.level = -1;
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

        Manager.Game.CurrentSave.equipSave = new EquipSave[equipData.Length];
        foreach (EquipInfo data in equipData)
        {
            EquipSave tmp = new();
            tmp.id = data.id;
            tmp.level = -1;

            Manager.Game.CurrentSave.equipSave[index] = tmp;

            index++;
        }
    }
    public void UpdateEquipInfoBySave() // 모든 동적 장비데이터를 세이브 파일을 통해 최신화. 장착중인 장비도 할당함. 세이브 파일은 최신화 따로 해야함.
    {

        for (int i = 0; i < equipData.Length; i++)
        {
            if (equipData[i].id == Manager.Game.CurrentSave.equipSave[i].id)
            {
                EquipInfo tmpInfo = equipData[i];
                EquipSave tmpSave = Manager.Game.CurrentSave.equipSave[i];
                tmpInfo.level = tmpSave.level;
                tmpInfo.upgradeGold = tmpSave.level * equipData[i].upgradeGoldPlus;
                tmpInfo.equipValue = equipData[i].originValue + (tmpSave.level - 1) * equipData[i].equipValuePlus;
                equipData[i] = tmpInfo;
            }
            else
            {
                Debug.LogError($"배열순서 어긋남: {i}  {equipData[i].id}, {Manager.Game.CurrentSave.equipSave[i].id}");
            }
            UpdateWearing();
        }
    }

    public void UpdateWearing() // 동적 게임데이터를 기준으로 현재 장착 장비들을 최신화
    {
        for (int i = 0; i < equipData.Length; i++)
        {
            if (Manager.Game.CurrentSave.wearingId[0] == equipData[i].id && equipData[i].id != 0)
            {
                weapon = equipData[i];
            }
            if (Manager.Game.CurrentSave.wearingId[1] == equipData[i].id && equipData[i].id != 0)
            {
                armor = equipData[i];
            }

            if (Manager.Game.CurrentSave.wearingId[2] == equipData[i].id && equipData[i].id != 0)
            {
                accessory = equipData[i];
            }
        }
    }


    public void UpdateOneEquipInfo(int id, bool Upgrade = false) // 한 개의 장비 동적 데이터 업데이트. 업그레이드 가능
    {
        for (int i = 0; i < equipData.Length; i++)
        {
            if (equipData[i].id == id)
            {
                EquipSave tmpSave = Manager.Game.CurrentSave.equipSave[i];
                EquipInfo tmpInfo = equipData[i];
                if (Upgrade) tmpInfo.level++; // maxLevel을 찍으면, UI상에서 강화 기능 막아야 함;
                tmpSave.level = tmpInfo.level;
                tmpInfo.upgradeGold = equipData[i].level * equipData[i].upgradeGoldPlus;
                tmpInfo.equipValue = equipData[i].originValue + (equipData[i].level - 1) * equipData[i].equipValuePlus;
                Manager.Game.CurrentSave.equipSave[i] = tmpSave;
                equipData[i] = tmpInfo;
                return;
            }
        }
    }
    public void AddEquipment(int id) // 장비 습득, 또는 재화처리
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
                    EquipSave tmp = Manager.Game.CurrentSave.equipSave[i];
                    tmp.level = 1;

                    equipData[i] = tmpInfo;
                    Manager.Game.CurrentSave.equipSave[i] = tmp;
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
    public EquipInfo FindEquip(int id)
    {
        foreach(EquipInfo info in equipData)
        {
            if(info.id == id) { return info; }
        }
        return new EquipInfo();
    }
}
public struct EquipInfo
{
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