using KYG_skyPower;
using LJ2;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JYL
{
    public class GachaPopUp : BaseUI
    {
        private Image gachaImg => GetUI<Image>("GachaImg1");
        private Image equipImg => GetUI<Image>("EquipImg");
        private Image equipNframe => GetUI<Image>("EquipNImg");
        private Image equipLframe => GetUI<Image>("EquipLImg");
        private Image charSSRframe => GetUI<Image>("CharSSRImg");
        private Image charNframe => GetUI<Image>("CharNImg");
        private Image cover1Img => GetUI<Image>("Cover1");
        private CharacterSaveLoader chLoader;
        private EquipController equipController;
        private Animator animator;

        private void Start()
        {
            animator = GetComponentInChildren<Animator>();
            if (StorePresenter.isCharGacha)
            {
                chLoader = gameObject.GetOrAddComponent<CharacterSaveLoader>();
                chLoader.GetCharPrefab();

                foreach (CharactorController character in chLoader.charactorController)
                {
                    if (character.id == StorePresenter.gachaResult[0])
                    {
                        gachaImg.gameObject.SetActive(true);
                        gachaImg.sprite = character.image;
                        if(character.grade == Grade.R)
                        {
                            charNframe.gameObject.SetActive(true);
                        }
                        else
                        {
                            charSSRframe.gameObject.SetActive(true);
                        }
                    }
                }
            }
            else if(!StorePresenter.isCharGacha)
            {
                equipController = gameObject.GetOrAddComponent<EquipController>();
                equipController.Init();
                foreach(var data in equipController.equipData)
                {
                    if(data.id ==  StorePresenter.gachaResult[0])
                    {
                        equipImg.gameObject.SetActive(true);
                        equipImg.sprite = data.icon;
                        if(data.grade ==EquipGrade.Normal)
                        {
                            equipNframe.gameObject.SetActive(true);
                        }
                        else
                        {
                            equipLframe.gameObject.SetActive(true);
                        }
                    }
                }
            }

            GetEvent("Cover1").Drag += dragCover;
            GetEvent("Cover1").EndDrag += endDragCover;
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

