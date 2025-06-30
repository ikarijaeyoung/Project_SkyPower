using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JYL
{
    public class StageSelectPopUp : BaseUI
    {

        // UIManager에 현재 선택한 월드의 인덱스를 참조할 필요가 있음.
        // 만약 해당 월드가 lock된 경우, 클릭을 막음. 해당 정보는 게임 매니저 또는 스테이지매니저, 씬매니저에 있음
        void Start()
        {
            GetEvent("Stage1").Click += OnStageClick;
        }

        void Update()
        {

        }

        private void OnStageClick(PointerEventData eventData)
        {
            Util.ExtractTrailNumber(eventData.pointerClick.gameObject.name, out int index);
            Debug.Log($"해당 스테이지 선택됨 : {index}");
            // 여기서 index를 UIManager에 저장
            UIManager.Instance.ShowPopUp<StageInerSelectPopUp>();

        }
    }
}

