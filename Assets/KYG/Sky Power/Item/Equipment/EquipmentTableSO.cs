using KYG_skyPower;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{
// ��� ��� �� ���� �����ϴ� �����̳�
[CreateAssetMenu(fileName = "EquipmentTableSO", menuName = "Equipment/EquipmentTableSO")]
public class EquipmentTableSO : ScriptableObject
{
    public List<EquipmentDataSO> equipmentList = new List<EquipmentDataSO>();
}
}