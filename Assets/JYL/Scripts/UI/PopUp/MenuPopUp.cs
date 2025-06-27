using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JYL
{
    public class MenuPopUp : BaseUI
    {
        void Start()
        {
            GetUI<TMP_Text>("BackBtnText").text = "Close";
            GetEvent("BackBtn").Click += data => UIManager.Instance.ClosePopUp();
        }

        void Update()
        {

        }
    }
}


