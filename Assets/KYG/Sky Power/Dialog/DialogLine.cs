using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KYG_skyPower
{


    [Serializable]
    public class DialogLine // Dialog 규칙(단위)
    {

        public int id; 
        public string speaker; // 화자(말하는 대상)
        public string portraitKey; // Sprite는 런타임에 리소스.로드로 할당
        public string text; // 대사
        public bool isBranch; // 선택지 유무
        public List<int> nextIds = new List<int>(); // 다음 대사
        public string voiceKey; //  보이스

    }
}
