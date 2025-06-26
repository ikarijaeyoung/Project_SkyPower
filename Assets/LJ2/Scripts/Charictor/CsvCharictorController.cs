using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IO;

public class CsvCharictorController : MonoBehaviour
{
    public CharictorHasCsv charictorHasCsv;

    // 세이브 당시 필요한 정보
    public int level = 1;
    public int exp;

    // 인게임 필요 정보
    public int Hp;
    public int attackPower;
    public int attackSpeed;
    public int moveSpeed;

    private void Start()
    {
        // ToDo 저장 상황 로드하여 level, exp가져오기
        GetParmeter(level);
    }
    private void GetParmeter(int level)
    {
        CsvReader.Read(charictorHasCsv.dataTable);

        Hp = int.Parse(charictorHasCsv.dataTable.GetData(level, 1));
        attackPower = int.Parse(charictorHasCsv.dataTable.GetData(level, 2));
        attackSpeed = int.Parse(charictorHasCsv.dataTable.GetData(level, 3));
        moveSpeed = int.Parse(charictorHasCsv.dataTable.GetData(level, 4));
    }
}
