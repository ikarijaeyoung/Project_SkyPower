using IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharictorDataTest", menuName = "Charictor/CharictorDataTest")]
public class CharictorDataTest : ScriptableObject 
{
    public string name;
    public int level;
    public int Hp;
    public int exp;
    public int attackPower;
    public int attackSpeed;
    public int moveSpeed;
    public GameObject bulletPrefab;

    public CsvTable dataTable;
}
