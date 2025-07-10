using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JYL;


public class CharacterIconPrefab : BaseUI
{

    public Image ssrFrame;
    public Image rFrame;
    public Image charImg;
    public void Init()
    {
        base.Awake();
        ssrFrame = GetUI<Image>("RFrameImg");
        rFrame = GetUI<Image>("SSRFrameImg");
        charImg = GetUI<Image>("CharIconImg");
    }
}
