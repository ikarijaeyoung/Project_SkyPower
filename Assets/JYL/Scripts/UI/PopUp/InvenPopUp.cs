using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    public class InvenPopUp : BaseUI
    {
        // 게임매니저에서 캐릭터 정보를 불러와야 함
        private GameObject invenScroll => GetUI("InvenScroll");
        void Start()
        {
            // 장비 클릭시 활성화
            invenScroll.SetActive(false);
        }

        void Update()
        {

        }
    }
}


