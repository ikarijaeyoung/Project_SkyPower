using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using LJ2;

namespace JYL
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Set Scriptable Object")]
        [SerializeField] PlayerModel playerModel; // -> 캐릭터 컨트롤러로 대체 예정
        // CharacterController[] character -> 캐릭터 3명을 배열로 받음. 게임매니저의 파티조합을 가져옴

        [Header("Set References")]
        [SerializeField] List<ObjectPool> bulletPools;
        [SerializeField] Transform muzzlePoint;
        [SerializeField] RectTransform leftUI;
        [SerializeField] RectTransform rightUI;
        // TODO : UI 시스템 구축 후, 시스템에서 불러오는 식으로 참조 시킨다


        [Header("Set Value")]
        [Range(0.1f, 5)][SerializeField] float bulletReturnTimer = 2f;
        [Range(0.1f, 3)][SerializeField] float fireDelay = 1f;

        private PlayerInput playerInput;
        private Rigidbody rig;
        private InputAction attackAction;
        private CharactorController mainCharController;
        private CharacterSaveLoader charDataLoader;
        //private InputAction parryAction1;
        //private InputAction parryAction2;
        //private InputAction ultAction;
        //private InputAction menuAction;

        private int hp { get; set; }
        private int attackPower { get; set; }
        private float attackSpeed { get; set; }
        private float moveSpeed { get; set; }
        private int defence { get; set; }


        // 좌, 우 UI 사이즈
        private float leftMargin;
        private float rightMargin;

        //private int level;
        //private int hp;
        private int poolIndex = 0;
        private Vector2 inputDir;
        //private bool isAttack;

        private ObjectPool curBulletPool => bulletPools[poolIndex];
        private void Awake()
        {
            Init();
        }
        private void OnEnable()
        {
            //CreatePlayer();
            rig = GetComponent<Rigidbody>();
            SubscribeEvents();
        }
        private void Update()
        {
            PlayerHandler();
        }

        private void FixedUpdate()
        {

        }

        private void LateUpdate()
        {
            // 애니메이션 - 궁극기 등
        }

        private void OnTriggerEnter(Collider other)
        {
            // 적 총알에 맞으면 피격
        }
        private void OnDisable()
        {
            UnSubscribeEvents();
        }

        //private void CreatePlayer()
        //{
        //   플레이어 생성 - 게임매니저의 파티조합을 가져옴. 
        //   Instantiate(character[0].prefab, transform); -> 캐릭터 컨트롤러에 있는 프리팹을 통해 캐릭터 생성
        //   
        //}
        private void Init()
        {
            playerInput = GetComponent<PlayerInput>();
            charDataLoader = GetComponent<CharacterSaveLoader>();
            charDataLoader.GetCharPrefab();
            foreach(var charData in charDataLoader.charactorController)
            {
                switch(charData.partySet)
                {
                    case PartySet.Main:
                    mainCharController = charData;
                        break;
                    case PartySet.Sub1:

                        break;
                    case PartySet.Sub2:
                        break;
                }
            }
            attackAction = playerInput.actions["Attack"];

            // parryAction1 = playerInput.actions["parry1"];
            // parryAction2 = playerInput.actions["parry2"];
            // ultAction = playerInput.actions["Ult"];
            // menuAction = playerInput.actions["menu"];

            // TODO: UI 시스템 구축 후, UI Manager에서 참조하는 식으로 변경
            if(leftUI!= null)
            {
                leftMargin = leftUI.rect.width/Camera.main.pixelWidth;
            }
            else
            {
                leftMargin = 0.1f;
                Debug.LogWarning("왼쪽 UI 참조 안됐음");
            }
            if (rightUI != null)
            {
                rightMargin = 1- rightUI.rect.width/Camera.main.pixelWidth;
            }
            else
            {
                rightMargin = 0.9f;
                Debug.LogWarning("오른쪽 UI 참조 안됐음");
            }
        }
            // 캐릭터 필드 세팅
        private void CharacterParameterSetting()
        {
            //mainCharController.
        }
        private void SubscribeEvents()
        {
            attackAction.started += Fire;
        }
        private void UnSubscribeEvents()
        {
            attackAction.started -= Fire;
        }

        private void PlayerHandler()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                poolIndex = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                poolIndex = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                poolIndex = 2;
            }
            SetMove();
            //UseUlt
            //Parry
        }

        private void SetMove()
        {
            Vector2 clampInput = ClampMoveInput(inputDir);
            if (clampInput == Vector2.zero)
            {
                rig.velocity = Vector3.zero;
                return;
            }
            Vector3 moveDir = new Vector3(clampInput.x, 0f, clampInput.y);
            rig.velocity = moveDir * playerModel.playerSpeed;
        }

        private Vector2 ClampMoveInput(Vector2 inputDirection)
        {
            if (inputDirection == Vector2.zero)
            {
                return Vector2.zero;
            }

            // 카메라 기준 스크린 좌표로 판단
            //Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

            // 플레이어가 카메라 뒤에 있다는 뜻
            //if (screenPos.z <= 0) return Vector2.zero;

            //if (screenPos.x <= 0 && inputDirection.x < 0) inputDirection.x = 0;
            //if (screenPos.x >= Camera.main.pixelWidth && inputDirection.x > 0) inputDirection.x = 0;
            //if (screenPos.y <= 0 && inputDirection.y < 0) inputDirection.y = 0;
            //if (screenPos.y >= Camera.main.pixelHeight && inputDirection.y > 0) inputDirection.y = 0;

            // 뷰포트 기준 좌표로 좀 더 간단화 가능
            // 뷰포트는 0~1사이의 값으로만 정해져 있다. 매우 정확하게 떨어지진 않겠지만, 어느정도 커버가 된다

            Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
            if(viewportPos.z<=0) return Vector2.zero;

            if (viewportPos.x <= leftMargin && inputDirection.x < 0) inputDirection.x = 0;
            if (viewportPos.x >= rightMargin && inputDirection.x > 0) inputDirection.x = 0;

            if (viewportPos.y <= 0 && inputDirection.y < 0) inputDirection.y = 0;
            if (viewportPos.y >= 1 && inputDirection.y > 0) inputDirection.y = 0;

            return inputDirection;
        }

        public void OnMove(InputValue value)
        {
            inputDir = value.Get<Vector2>();
        }

        private void Fire(InputAction.CallbackContext ctx)
        {
            BulletPrefabController bulletPrefab = curBulletPool.ObjectOut() as BulletPrefabController;
            bulletPrefab.transform.position = muzzlePoint.position;
            bulletPrefab.ReturnToPool(bulletReturnTimer);
            foreach (BulletInfo info in bulletPrefab.bulletInfo)
            {
                if (info.rig == null)
                {
                    continue;
                }
                info.trans.gameObject.SetActive(true);
                info.trans.localPosition = info.originPos;
                info.rig.velocity = Vector3.zero;
                info.bulletController.attackPower = this.attackPower;
                info.rig.AddForce(playerModel.fireSpeed * info.trans.forward, ForceMode.Impulse); // 이 부분을 커스텀하면 됨
            }
        }
        //private void UseUlt()
        //{
        //  궁극기 -  입력을 통해 들어옴
        //  if(ultGage>=100)
        //  character[0].Ult();
        //}

        //private void Parry(int index)
        //{
        //  들어온 캐릭터에 따른 패링스킬 사용
        //  character[index].Parry();
        //}
    }
}

// 버튼 순서에 맞게 알아서 배치되게

// Dictionary<string,Scene> sceneList;
// sceneList  시작할 때, 빌드에 포함 된 씬 전부 저장
// public int curScene  = sceneList[0]; // sceneList[scene.Title];

// 
// void SceneChange(string sceneName)
// {
//      씬 전환 적업이 일어남
//      들어오는게 스테이지 = 
//      -> 데이터 여기서 들어옴
//      스테이지의 스크립터블 오브젝트가 필요함
//      스테이지 저장
//      stage_1,1
//      string.split('_',',') -> string[] s = "stage","1","1" 
//      s[0] => curScene
//      int 
//      s[1],[2] => int.parse
//      curscene = 
//      1-1 => 월드 변수, 스테이지 변수
//      string 숫자 받아ㅏ오면됨
//      curScene = sceneName
// }
// 
// 스테이지마다 달라져야 하는것, 가져와야 하는 것
// 라이트, 에너미(스포너), 에너미의  설정값, 보스, 엘리트몬스터
// 맵데이터, 플레이어 데이터(자동), <= SceneChange
// 