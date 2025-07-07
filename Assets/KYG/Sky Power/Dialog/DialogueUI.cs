using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KYG.SkyPower
{
    public class DialogueUI : MonoBehaviour
    {
        public TMP_Text speakerText;
        public TMP_Text dialogText;
        public Image portrait;

        public DialogueManager manager;

        void OnEnable()
        {
            manager.OnDialogLine.AddListener(UpdateUI);
        }
        void OnDisable()
        {
            manager.OnDialogLine.RemoveListener(UpdateUI);
        }
        void UpdateUI(DialogLine line)
        {
            if (line == null)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            speakerText.text = line.speaker;
            dialogText.text = line.text;
            portrait.sprite = line.portrait;
        }
    }
}
