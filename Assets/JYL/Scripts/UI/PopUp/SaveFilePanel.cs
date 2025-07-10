using KYG_skyPower;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace JYL
{
    public class SaveFilePanel : BaseUI
    {
        // �ҷ��Դ� ���̺� ������ �������� �����͸� ä���
        GameData data;
        
        void Start()
        {
            data = Manager.Game.CurrentSave;
            GetUI<TMP_Text>("SaveFileData").text = $"{Manager.Game.CurrentSave.playerName}";
            GetEvent("SaveDelBtn").Click += OnDelClick;
            GetEvent("SaveStartBtn").Click += OnStartClick;
        }

        //���̺������� ����
        private void OnDelClick(PointerEventData eventData)
        {
            Manager.Save.GameDelete(data, Manager.Game.currentSaveIndex + 1);
            UIManager.Instance.ClosePopUp();
            Manager.Game.ResetSaveRef();
        }

        //���̺� ���Ϸ� ���� ����
        private void OnStartClick(PointerEventData eventData)
        {
            // �� �Ѿ -> mainScene
            // ���� UI��� ���ؼ� ���̺������� ���õǾ� ����.
            UIManager.Instance.CleanPopUp();
            Manager.SDM.SyncRuntimeDataWithStageInfo();
            SceneManager.LoadSceneAsync("bMainScene_JYL");
        }

    }
}
