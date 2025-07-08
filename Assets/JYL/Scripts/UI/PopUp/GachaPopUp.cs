using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using KYG_skyPower;
using LJ2;
using JYL;

namespace JYL
{
    public class GachaPopUp : BaseUI
    {
        private Image gachaImg => GetUI<Image>("GachaImg1");
        private Image equipNframe => GetUI<Image>("EquipNImg");
        private Image equipLframe => GetUI<Image>("CharNImg");
        private Image charSSRframe => GetUI<Image>("CharSSRImg");
        private Image cover1Img => GetUI<Image>("Cover1");
        private CharacterSaveLoader chLoader;
        private EquipController equipController;
        private Animator animator;
        
        private void Start()
        {
            animator = GetComponentInChildren<Animator>();
            chLoader = gameObject.GetOrAddComponent<CharacterSaveLoader>();
            chLoader.GetCharPrefab();
            GetEvent("Cover1").Drag += dragCover;
            GetEvent("Cover1").EndDrag += endDragCover;
            foreach(CharactorController character in chLoader.charactorController)
            {
                if(character.id == StorePresenter.gachaResult[0])
                {
                    gachaImg.sprite = character.image;
                    Debug.Log($"여기 있는 캐릭의 ID {character.id}");
                }

            }
        }
        private void OnDisable()
        {
            StorePresenter.isStoreReturn = true;
            StorePresenter.isCharGacha = false;
        }
        private void dragCover(PointerEventData eventData)
        {
            cover1Img.transform.position = eventData.position;
        }
        private void endDragCover(PointerEventData eventData)
        {
            animator.Play("Disappear");
        }
    }
}

