using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    public class ObjectPool : MonoBehaviour
    {
        [Header("Set References")]
        [SerializeField] BulletController bullets;
        
        [Header("Set Value")]
        [Range(1, 100)][SerializeField] int bulletNum; 

        private void Awake()
        {

        }
        void Start()
        {

        }

        void Update()
        {

        }
    }
}


