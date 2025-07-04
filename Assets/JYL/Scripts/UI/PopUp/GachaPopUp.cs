using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    public class GachaPopUp : BaseUI
    {
        // TODO: 뽑기를 진행 시, 뽑기 완료 결과를 어디선가 가져와야 한다.
        // 가챠 뽑기 횟수 만큼, 해당 가챠 횟수에 해당하는 팝업이 필요함
        // 시간이 부족하므로, 가챠 횟수만큼 UI 요소를 늘리는 것은 인벤토리(캐릭터창,장비창) 구현 후 한다.
        private Sprite gachaImg;
        void Start()
        {
            // 가챠 정보를 기준으로 스프라이트들의 이미지를 채운다
        }
    }
}

