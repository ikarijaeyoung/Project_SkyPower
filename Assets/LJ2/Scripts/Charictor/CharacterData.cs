using JYL;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace LJ2
{
    [CreateAssetMenu(fileName = "CharictorData", menuName = "Charictor/CharictorData")]
    public class CharacterData : ScriptableObject
    {
        public int id;
        public Grade grade;
        public string characterName;
        public GameObject characterModel;

        public Elemental elemental;
        public int maxLevel;
        public int hp;
        public int hpPlus;
        public GameObject[] bulletPrefabs = new GameObject[5];
        public float attackDamage;
        public float damagePlus;
        public float attackSpeed;
        public float moveSpeed;
        public int defense;


        public int ultCoolDefault;
        public int ultCoolReduce;
        public GameObject ultVisual;

        public Parry parry;
        public int parryCool;

        public Sprite icon;
        public Sprite image;

        public int upgradeUnitDefault;
        public int upgradeUnitPlus;


    }

    public enum Grade
    {
        SSR,
        R
    }
    public enum Elemental
    {
        ��,
        ��,
        �ٶ�
    }

    public enum Parry
    {
        ��,
        ����,
        �ݻ�B
    }
}