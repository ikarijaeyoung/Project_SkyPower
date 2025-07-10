using KYG_skyPower;
using System.Collections.Generic;
using UnityEngine;

// ���â, �ΰ��� �������� ������ ����ϴ� ��ũ��Ʈ
// ���̺� ������ ��ȭ �Ǵ� ������ ��� ���� �� = Add  �� // ��� ���׷��̵� �� �� = UpdateOneEquipInfo �� 
// ����������, ��� ������ ������ ���̴� = GameData.WearingId �̰Ÿ� �ٲ�� ��
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
    public void CreateEquipInfo() // �������� ������ ��� �����͹迭�� ������. SO�� �ʱⵥ���͸� ������
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
    public void SaveFileInit() //���̺����� ���� ���� �� �۾� SaveCreatePanel���� ����.
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
    public void UpdateEquipInfoBySave() // ��� ���� ������͸� ���̺� ������ ���� �ֽ�ȭ. �������� ��� �Ҵ���. ���̺� ������ �ֽ�ȭ ���� �ؾ���.
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
                Debug.LogError($"�迭���� ��߳�: {i}  {equipData[i].id}, {Manager.Game.CurrentSave.equipSave[i].id}");
            }
            UpdateWearing();
        }
    }

    public void UpdateWearing() // ���� ���ӵ����͸� �������� ���� ���� ������ �ֽ�ȭ
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


    public void UpdateOneEquipInfo(int id, bool Upgrade = false) // �� ���� ��� ���� ������ ������Ʈ. ���׷��̵� ����
    {
        for (int i = 0; i < equipData.Length; i++)
        {
            if (equipData[i].id == id)
            {
                EquipSave tmpSave = Manager.Game.CurrentSave.equipSave[i];
                EquipInfo tmpInfo = equipData[i];
                if (Upgrade) tmpInfo.level++; // maxLevel�� ������, UI�󿡼� ��ȭ ��� ���ƾ� ��;
                tmpSave.level = tmpInfo.level;
                tmpInfo.upgradeGold = tmpInfo.level * tmpInfo.upgradeGoldPlus;
                tmpInfo.equipValue = tmpInfo.originValue + (tmpInfo.level - 1) * tmpInfo.equipValuePlus;
                Manager.Game.CurrentSave.equipSave[i] = tmpSave;
                equipData[i] = tmpInfo;
                return;
            }
        }
    }
    public void AddEquipment(int id) // ��� ����, �Ǵ� ��ȭó��
    {
        for (int i = 0; i < equipData.Length; i++)
        {
            if (equipData[i].id == id) // id�� ��ġ �� ��
            {
                Debug.Log($"���̵� ��ġ�ؼ� ���� {id}   {equipData[i].id}");
                if (equipData[i].level > 0) // �̹� ���� ��.
                {
                    Manager.Game.CurrentSave.gold += 10;
                    return;
                }

                // ���������� ����
                else if (equipData[i].level <=0)
                {
                    EquipInfo tmpInfo = equipData[i];
                    tmpInfo.level = 1;
                    EquipSave tmp = Manager.Game.CurrentSave.equipSave[i];
                    tmp.level = 1;

                    equipData[i] = tmpInfo;
                    Manager.Game.CurrentSave.equipSave[i] = tmp;
                    return;
                }
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