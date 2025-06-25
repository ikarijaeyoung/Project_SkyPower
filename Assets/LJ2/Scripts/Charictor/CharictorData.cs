using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CharictorData", menuName = "Charictor/CharictorData")] 
public class CharictorData : ScriptableObject
{
    public int level;
    public int Hp;
    public int exp;
    public int attackPower;
    public float attackSpeed;
    public float moveSpeed;
    public GameObject bulletPrefab;
    public GameObject model;
    public Sprite image;
    public int[][] table;
}
