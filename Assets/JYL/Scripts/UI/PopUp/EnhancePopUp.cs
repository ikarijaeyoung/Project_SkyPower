using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    public class EnhancePopUp : BaseUI
    {
        // UIManager, GameManager 등을 퉁해 현재 켜진 강화 창이 캐릭강화창인지 장비 강화창인지 판별함
        // 캐릭 또는 장비에 재화 소모 후 Level을 올림.
        
        // 이 필드들은 선언하지말고 매니저에서 직접 가져온다.
        // private CharacterController CharacterController;
        // private Item item;

        void Start()
        {
            //GetEvent("EnhanceBtn").Click += data => item.level++; 또는 레벨증가 함수 수행
            GetEvent("EnhanceBtn").Click += data => UIManager.Instance.ClosePopUp();
        }
    }
}

