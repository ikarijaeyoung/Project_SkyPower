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
    private int Hp;
    public int HP { get { return Hp; } set { Hp = value; } }
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

    public void GetEXP()
    {
        // ToDo : Csv 형식에 따라 성장 적용
    }

    public void UseUlt()
    {
        // Todo : 캐릭터 별 궁극기 효과 함수 실행
    }

    public void Parrying()
    {
        // Todo : 캐릭터 별 패링 효과 함수 실행
    }
}
