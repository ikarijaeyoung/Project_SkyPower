using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    public class Gacha5PopUp : BaseUI
    {
        // TODO: 뽑기를 진행 시, 뽑기 완료 결과를 어디선가 가져와야 한다.
        // 가챠 뽑기 횟수 만큼, 해당 가챠 횟수에 해당하는 팝업이 필요함
        // 시간이 부족하므로, 가챠 횟수만큼 UI 요소를 늘리는 것은 인벤토리(캐릭터창,장비창) 구현 후 한다.
        private Sprite[] gachaImg = new Sprite[5];
        void Start()
        {
            // 가챠 정보를 기준으로 스프라이트들의 이미지를 채운다
            for(int i =0;i<gachaImg.Length; i++)
            {
                // 여기서 이미지 갯수만큼 UI 요소 생성해서 가운데에 UI레이아웃 컴포넌트 기능으로 정렬함
            }
        }
    }
}

