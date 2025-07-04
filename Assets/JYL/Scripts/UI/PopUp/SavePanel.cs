using KYG_skyPower;
using TMPro;
using UnityEngine.EventSystems;

namespace JYL
{
    public class SavePanel : BaseUI
    {
        private void OnEnable()
        {
            Manager.Game.ResetSaveRef();
            for (int i = 0; i < 3; i++)
            {
                GameData data = Manager.Game.saveFiles[i];

                if (data == null || data.isEmpty)
                {
                    GetUI<TMP_Text>($"SaveText{i + 1}").text = "File Empty";
                }
                else
                {
                    GetUI<TMP_Text>($"SaveText{i + 1}").text = Manager.Game.saveFiles[i].playerName;
                }
            }
        }
        void Start()
        {
            GetEvent("SaveBtn1").Click += OnSaveSlotBtnClick;
            GetEvent("SaveBtn2").Click += OnSaveSlotBtnClick;
            GetEvent("SaveBtn3").Click += OnSaveSlotBtnClick;
        }

        private void OnSaveSlotBtnClick(PointerEventData eventData)
        {
            string name = eventData.pointerClick.name; // 클릭한 UI의 게임오브젝트 이름
            Util.ExtractTrailNumber(name, out int index); // 스트링의 끝에 붙어 있는 숫자
            Manager.Game.SelectSaveFile(index - 1);

            GameData data = Manager.Game.saveFiles[index - 1];
            if (!data.isEmpty && data != null)
            {
                UIManager.Instance.ShowPopUp<SaveFilePanel>();
            }
            else
            {
                UIManager.Instance.ShowPopUp<SaveCreatePanel>();
            }
        }
    }
}

