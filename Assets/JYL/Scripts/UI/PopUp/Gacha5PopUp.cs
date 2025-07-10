using KYG_skyPower;
using LJ2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JYL
{
    public class Gacha5PopUp : BaseUI
    {
        private Image[] gachaImg;
        private Image[] equipImg;
        private Image[] equipNframe;
        private Image[] equipLframe;
        private Image[] charSSRframe;
        private Image[] charNframe;
        private Image[] coverImg;

        private CharacterSaveLoader chLoader;
        private EquipController equipController;

        private void Start()
        {
            SetUI();
            if (StorePresenter.isCharGacha)
            {
                PrintCharacter();
            }
            else if (!StorePresenter.isCharGacha)
            {
               PrintEquip();
            }

        }
        private void OnDisable()
        {
            StorePresenter.isStoreReturn = true;
            StorePresenter.isCharGacha = false;
        }

        private void SetUI()
        {
            gachaImg = new Image[5];
            equipImg    = new Image[5];
            equipNframe = new Image[5];
            equipLframe  = new Image[5];
            charSSRframe = new Image[5];
            charNframe  = new Image[5];
            coverImg    = new Image[5];
            for (int i = 0; i< 5;i++)
            {
                gachaImg[i] = GetUI<Image>($"GachaImg{i + 1}");
                equipImg[i] = GetUI<Image>($"EquipImg{i + 1}");
                equipNframe[i] = GetUI<Image>($"EquipNImg{i + 1}");
                equipLframe[i] = GetUI<Image>($"EquipLImg{i + 1}");
                charSSRframe[i] = GetUI<Image>($"CharSSRImg{i + 1}");
                charNframe[i] = GetUI<Image>($"CharNImg{i + 1}");
                coverImg[i] = GetUI<Image>($"Cover{i + 1}");
                GetEvent($"Cover{i + 1}").Drag += DragCover;
                GetEvent($"Cover{i + 1}").EndDrag += EndDragCover;
            }
        }

        private void PrintCharacter()
        {
            chLoader = gameObject.GetOrAddComponent<CharacterSaveLoader>();
            chLoader.GetCharPrefab();
            for(int i = 0;i<StorePresenter.gachaResult.Count;i++)
            {
                Debug.Log($"현재 가챠결과 카운트 {StorePresenter.gachaResult.Count}  {i}번째 반복중");
                foreach (CharactorController character in chLoader.charactorController)
                {
                    if (character.id == StorePresenter.gachaResult[i])
                    {
                        Debug.Log($"현재 id {character.id}");
                        gachaImg[i].gameObject.SetActive(true);
                        gachaImg[i].sprite = character.image;
                        if (character.grade == Grade.R)
                        {
                            charNframe[i].gameObject.SetActive(true);
                        }
                        else
                        {
                            charSSRframe[i].gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
        private void PrintEquip()
        {
            equipController = gameObject.GetOrAddComponent<EquipController>();
            equipController.Init();
            for(int i = 0; i< StorePresenter.gachaResult.Count; i++)
            {
                foreach (var data in equipController.equipData)
                {
                    if (data.id == StorePresenter.gachaResult[i])
                    {
                        equipImg[i].gameObject.SetActive(true);
                        equipImg[i].sprite = data.icon;
                        if (data.grade == EquipGrade.Normal)
                        {
                            Debug.Log($"{data.id}__{data.grade}");
                            equipNframe[i].gameObject.SetActive(true);
                        }
                        else
                        {
                            Debug.Log($"{data.id}__{data.grade}");
                            equipLframe[i].gameObject.SetActive(true);
                        }
                    }
                }
            }
 
        }
        private void DragCover(PointerEventData eventData)
        {

            eventData.pointerDrag.transform.position = eventData.position;
        }
        private void EndDragCover(PointerEventData eventData)
        {
           eventData.pointerDrag.GetComponent<Animator>().Play("Disappear");
        }
    }
}


