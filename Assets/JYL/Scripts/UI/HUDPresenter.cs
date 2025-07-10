using KYG_skyPower;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace JYL
{
    public class HUDPresenter : BaseUI
    {
        private PlayerController player;
        [SerializeField] private Sprite ultSprite;
        [SerializeField] private Sprite parry1Sprite;
        [SerializeField] private Sprite parry2Sprite;
        public UnityEvent<int> onSeqChanged;

        private int maxHp { get; set; } // �÷��̾� ��Ʈ�ѷ����� ü�� ������

        private int curHp;
        public int CurHp
        {
            get { return curHp; }
            set
            {
                curHp = value;
                OnHpChanged();
            }

        } // �÷��̾� ��Ʈ�ѷ����� ���� ü�� ������
        private float ultGage = 0f; // ĳ���� ��Ʈ�ѷ����� ���� ������
        public float UltGage
        {
            get { return ultGage; }
            set
            {
                ultGage = value;
                OnGageChanged();
            }
        }
        private float parryCooltime = 2f;

        // ���� ���������� ������ ������ ������
        private int maxSeq { get; set; }
        public int curSeq { get; set; }


        private Slider hpBar => GetUI<Slider>("HPBar");
        private Slider pgBar => GetUI<Slider>("ProgressBar");
        private Image parry1BackImg => GetUI<Image>("ParryImgBack");
        private Image parry1Img => GetUI<Image>("ParryImgFront");
        private Image parry2BackImg => GetUI<Image>("Parry2ImgBack");
        private Image parry2Img => GetUI<Image>("Parry2ImgFront");
        private Image ultGageImg => GetUI<Image>("UltImgFront");
        private Image ultGageBackImg => GetUI<Image>("UltImgBack");
        private Image ultIllust => GetUI<Image>("UltImg");
        private Image parryIllust => GetUI<Image>("ParryImg");

        public RectTransform leftUI => GetUI<RectTransform>("LeftUIImg");
        public RectTransform rightUI => GetUI<RectTransform>("RightUIImg");

        // private Animator ultAnimator => GetUI<Animator>("UltImg");
        private Animator ultAnimator => GetUI<Animator>("UltImg");
        private Animator parryAnimator => GetUI<Animator>("ParryImg");


        private Coroutine parry1CooldownRoutine;
        private Coroutine parry2CooldownRoutine;

        private new void Awake()
        {
        }
        private void OnEnable()
        {
        }
        void Start() { }
        private void OnDisable() => onSeqChanged.RemoveListener(SetProgressBar);

        void Update()
        {
            // ESC ���� ��, ���� ���� �˾�
            if (Input.GetKeyDown(KeyCode.Escape) && !PopUpUI.IsPopUpActive && !Util.escPressed)
            {
                UIManager.Instance.ShowPopUp<StagePopUp>();
                Util.ConsumeESC();
                Time.timeScale = 0;
            }

            if (!PopUpUI.IsPopUpActive && !Manager.Game.isPaused)
            {
                Time.timeScale = 1;
                Debug.Log($"�ٽ� Ǯ��.{Time.timeScale}");
            }
        }
        private void LateUpdate() { }

        public void SetPlayer(PlayerController playerController)
        {
            player = playerController;
        }
        public void Init()
        {
            base.Awake();
            maxHp = player.Hp;
            CurHp = player.Hp;
            ultGage = 0;

            hpBar.value = (float)CurHp / maxHp;
            Debug.Log($"{maxHp}_{CurHp}_{hpBar.value}");

            ultIllust.sprite = player.mainCharController.image;
            ultGageImg.sprite = player.mainCharController.icon;
            ultGageBackImg.sprite = player.mainCharController.icon;

            SetParry();

            parryIllust.gameObject.SetActive(false);
            ultIllust.gameObject.SetActive(false);

            StageEnemyData currentStage = Manager.SDM.runtimeData[Manager.Game.selectWorldIndex - 1].subStages[Manager.Game.selectStageIndex - 1].stageEnemyData;
            maxSeq = currentStage.sequence.Count - 1;
            onSeqChanged.AddListener(SetProgressBar);
        }
        public void HpSet() 
        {

        }
        private void SetParry()
        {
            if (player.sub1CharController != null)
            {
                parry1Img.sprite = player.sub1CharController.icon;
                parry1BackImg.sprite = player.sub1CharController.icon;
            }
            else
            {
                parry1Img.sprite = null;
                Color color = parry1Img.color;
                color.a = 0f;
                parry1Img.color = color;

                parry1BackImg.sprite = null;
                parry1BackImg.color = color;
            }
            if (player.sub2CharController != null)
            {
                parry2Img.sprite = player.sub2CharController.icon;
                parry2BackImg.sprite = player.sub2CharController.icon;
            }
            else
            {
                parry2Img.sprite = null;
                Color color = parry2Img.color;
                color.a = 0f;
                parry2Img.color = color;

                parry2BackImg.sprite = null;
                parry2BackImg.color = color;
            }
        }
        public void OnHpChanged() { hpBar.value = (float)CurHp / maxHp; }
        private void OnGageChanged()=> ultGageImg.fillAmount = UltGage;

        //private void OnHpBarChanged(float value)
        //{
        //    // Handle HP change logic here
        //    // �ǰ� UI ȿ���� ���⼭ �� �� ����
        //}
        //private void SubscribeEvents()
        //{
        //    hpBar.onValueChanged.AddListener(OnHpBarChanged);
        //}
        //private void UnSubscribeEvent()
        //{
        //    hpBar.onValueChanged.RemoveListener(OnHpBarChanged);
        //}

        private void SetProgressBar(int seq) { pgBar.value = (float)seq / maxSeq; }

        //TODO : �ñر� ��� �� ����
        public void UseUltimate()
        {
            UltGage = 0;
            ultIllust.gameObject.SetActive(true);
            ultAnimator.Play("ActivateSkill");
        }

        public void UseParry1()
        {
            if (parry1CooldownRoutine == null)
            {
                parry1CooldownRoutine = StartCoroutine(Parry1Routine());
            }
        }
        IEnumerator Parry1Routine()
        {
            parryIllust.sprite = player.sub1CharController.image;
            parryIllust.gameObject.SetActive(true);
            parry1Img.fillAmount = 0;
            float timer = 0;
            parryAnimator.Play("ActiveParry");
            while (true)
            {
                if (timer > parryCooltime)
                {
                    timer = 0;
                    StopCoroutine(parry1CooldownRoutine);
                    parry1CooldownRoutine = null;
                    parryIllust.gameObject.SetActive(false);
                    break;
                }
                else
                {
                    parry1Img.fillAmount = (float)timer / parryCooltime;
                }
                timer += Time.deltaTime;
                yield return null;

            }
        }
        public void UseParry2()
        {
            if (parry2CooldownRoutine == null)
            {
                parry2CooldownRoutine = StartCoroutine(Parry2Routine());
            }
        }
        IEnumerator Parry2Routine()
        {
            parryIllust.sprite = player.sub2CharController.image;
            parryIllust.gameObject.SetActive(true);
            parry2Img.fillAmount = 0;
            float timer = 0;
            parryAnimator.Play("ActiveParry");
            while (true)
            {
                if (timer > parryCooltime)
                {
                    timer = 0;
                    StopCoroutine(parry2CooldownRoutine);
                    parry2CooldownRoutine = null;
                    parryIllust.gameObject.SetActive(false);
                    break;
                }
                else
                {
                    parry2Img.fillAmount = (float)timer / parryCooltime;
                }
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }
}

