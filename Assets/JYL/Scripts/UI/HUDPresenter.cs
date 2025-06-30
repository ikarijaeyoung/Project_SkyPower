using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace JYL
{
    public class HUDPresenter : BaseUI
    {
        private int maxHp { get; set; } // 플레이어 컨트롤러에서 체력 가져옴

        private int curHp;
        public int CurHp
        {
            get { return curHp; }
            set
            {
                curHp = value;
                OnHpChanged();
            }

        } // 플레이어 컨트롤러에서 현재 체력 가져옴
        private float ultGage; // 캐릭터 컨트롤러에서 정보 가져옴
        public float UltGage
        {
            get { return ultGage; }
            set
            {
                ultGage = value;
                OnGageChanged();
            }
        }
        private float parry1Cooltime = 2f;
        private float parry2Cooltime = 3f;
        private Slider hpBar => GetUI<Slider>("HPBar");
        private Image parry1Img => GetUI<Image>("ParryImgFront");
        private Image parry2Img => GetUI<Image>("Parry2ImgFront");
        private Image ultGageImg => GetUI<Image>("UltImgFront");

        private Coroutine parry1CooldownRoutine;
        private Coroutine parry2CooldownRoutine;
        void Start()
        {
            // TODO: 여기서 이미지들 채워넣음. 캐릭터 스프라이트 이미지가 연출로 쓰일듯?
            ultGage = 0;
        }
        private void OnEnable()
        {
            Init();
        }
        private void OnDisable()
        {
            UnSubscribeEvent();
        }

        void Update()
        {
            // TODO :  체력바 테스트. 플레이어 피격시 amount만큼 깎임. 플레이어 컨트롤러에서 이벤트 걸수도 있음.
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                CurHp--;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                CurHp++;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (parry1CooldownRoutine == null)
                {
                    parry1Img.fillAmount = 0;
                    parry1CooldownRoutine = StartCoroutine(Parry1Routine());
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (parry2CooldownRoutine == null)
                {
                    parry2Img.fillAmount = 0;
                    parry2CooldownRoutine = StartCoroutine(Parry2Routine());
                }
            }
            if (Input.GetKey(KeyCode.Space))
            {
                UltGage += Time.deltaTime;
                if (UltGage > 1)
                {
                    UltGage = 0;
                }
            }
        }
        private void Init()
        {
            maxHp = 100;
            CurHp = 56;
            hpBar.value = (float)curHp / maxHp;
            SubscribeEvents();
        }
        private void OnHpChanged()
        {
            hpBar.value = (float)curHp / maxHp;
        }
        private void OnGageChanged()
        {
            ultGageImg.fillAmount = UltGage;
        }
        private void OnHpBarChanged(float value)
        {
            // Handle HP change logic here
            // 피격 UI 효과도 여기서 줄 수 있음
        }
        private void SubscribeEvents()
        {
            hpBar.onValueChanged.AddListener(OnHpBarChanged);

        }
        private void UnSubscribeEvent()
        {
            hpBar.onValueChanged.RemoveListener(OnHpBarChanged);

        }

        IEnumerator Parry1Routine()
        {
            float timer = 0;
            while (true)
            {
                if (timer >= parry1Cooltime)
                {
                    timer = 0;
                    StopCoroutine(parry1CooldownRoutine);
                    parry1CooldownRoutine = null;
                    break;
                }
                else
                {
                    parry1Img.fillAmount = (float)timer / parry1Cooltime;
                }
                timer += Time.deltaTime;
                yield return null;

            }
        }
        IEnumerator Parry2Routine()
        {
            float timer = 0;
            while (true)
            {
                if (timer >= parry2Cooltime)
                {
                    timer = 0;
                    StopCoroutine(parry2CooldownRoutine);
                    parry2CooldownRoutine = null;
                    break;
                }
                else
                {
                    parry2Img.fillAmount = (float)timer / parry2Cooltime;
                }
                timer += Time.deltaTime;
                yield return null;

            }
        }
    }
}

