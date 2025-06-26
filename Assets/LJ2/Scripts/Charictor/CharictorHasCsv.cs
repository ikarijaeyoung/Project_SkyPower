using IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharictorHasCsv", menuName = "Charictor/CharictorHasCsv")]
public class CharictorHasCsv : ScriptableObject
{
    public string name;
    public int step;
    public CsvTable dataTable;
    public GameObject bullet;
    public Sprite image;
    public string element;
}
