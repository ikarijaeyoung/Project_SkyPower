using System;
using UnityEngine;

namespace JYL
{
    public class BulletController : MonoBehaviour
    {
        [Range(0.05f, 1f)][SerializeField] float ticTime = 0.5f;
        [SerializeField] GameObject flash; // TODO: 총알 프리팹 하나하나 다 직접 달아줘야 함
        [SerializeField] GameObject hit;
        public Rigidbody rig;
        private Collider col;
        private ParticleSystem ps;
        public int attackPower = 0;
        public bool canDeactive = true;
        private float timer;
        void Awake()
        {
            ps = GetComponent<ParticleSystem>();
            if(ps == null)
            {
                Debug.LogError("파티클 시스템이 없음");
            }
            rig = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
            rig.constraints = RigidbodyConstraints.FreezeRotation;
            rig.useGravity = false;
            col.isTrigger = true;
            timer = ticTime;
        }
        private void OnEnable()
        {
            timer = ticTime;
        }
        private void Update()
        {
            if (!canDeactive)
            {
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                }
                else if (timer < 0)
                {
                    timer = ticTime;
                }
            }
        }
        //private void OnTriggerStay(Collider other)
        //{
        //if(gameObject.layer == 7) // 플레이어
        //{
        //    //조건에 따라서 SetActive false
        //    // 적한테 테이크 데미지
        //    Enemy enemy = other.GetComponent<Enemy>();
        //    if (enemy == null)
        //    {
        //        gameObject.SetActive(false);
        //        return;
        //    }
        //    if (!canDeactive && timer <= 0)
        //    {
        //        enemy.TakeDamage(attackPower);
        //    }
        //    else if (canDeactive)
        //    {
        //        enemy.TakeDamage(attackPower);
        //        gameObject.SetActive(false);
        //    }
        //}
        //else if(gameObject.layer == 9)
        //{
        //    // 적이 총알 쏜거 처리
        //    PlayerController enemy =other.GetComponent<PlayerController>();
        //    if (enemy == null)
        //    {
        //        gameObject.SetActive(false);
        //        return;
        //    }
        //    if (!canDeactive && timer <= 0)
        //    {
        //        //enemy.TakeDamage(attackPower); // TODO:플레이어 구현필요
        //    }
        //    else if (canDeactive)
        //    {
        //        //enemy.TakeDamage(attackPower); // TODO:플레이어 구현필요
        //        gameObject.SetActive(false);
        //    }
        //}

        private void OnTriggerStay(Collider other)
        {
            Vector3 contactPoint = other.ClosestPoint(transform.position);
            Vector3 hitNormal = -transform.forward;
            if (gameObject.layer == 7) // 플레이어의 총알일 경우
            {
                Enemy enemy = other.GetComponentInParent<Enemy>();
                if (enemy == null)
                {
                    Debug.Log("에너미 컴포넌트를 찾지 못함");
                    SpawnHitEffect(contactPoint,hitNormal);
                    gameObject.SetActive(false);
                    return;
                }
                if (!canDeactive && timer <= 0)
                {
                    enemy.TakeDamage(attackPower);
                    SpawnHitEffect(contactPoint,hitNormal);
                }
                else if (canDeactive)
                {
                    enemy.TakeDamage(attackPower);
                    SpawnHitEffect(contactPoint,hitNormal);
                    gameObject.SetActive(false);
                }
            }
            Debug.Log("트리거 인식");
            if (gameObject.layer == 9) // 에너미의 총알일 경우
            {
                Debug.Log("총알이 적의 총알일 경우.");
                PlayerController player = other.GetComponent<PlayerController>();
                if (player == null)
                {
                    SpawnHitEffect(contactPoint,hitNormal);
                    gameObject.SetActive(false);
                    return;
                }
                if (!canDeactive && timer <= 0)
                {
                    player.TakeDamage(attackPower); 
                    SpawnHitEffect(contactPoint,hitNormal);
                }
                else if (canDeactive)
                {
                    player.TakeDamage(attackPower); 
                    SpawnHitEffect(contactPoint,hitNormal);
                    gameObject.SetActive(false);
                }
            }
        }
        private void OnDisable(){ }
        public void OnFire()
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play(true);
            if (flash != null)
            {
                GameObject flashInstance = Instantiate(flash, transform.position, Quaternion.LookRotation(transform.forward));
                if(flashInstance == null)
                {
                    Debug.Log("플래시 게임오브젝트 생성 실패 NUll");
                }
                ParticleSystem flashPs = flashInstance.GetComponent<ParticleSystem>();
                if (flashPs != null)
                {
                    flashPs.Play(true);
                    Destroy(flashInstance, flashPs.main.startLifetime.constant);
                }
                else if(flashPs == null)
                {
                    Debug.Log("플래시 파티클 시스템 컴포넌트가 NUll");
                    ParticleSystem flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                    Destroy(flashInstance, flashPsParts.main.startLifetime.constant);
                    if(flashPsParts == null)
                    {
                        Debug.Log("플래시의 자식도 파티클 시스템 컴포넌트가 Null");
                    }
                    
                }
                
            }
            else if(flash == null)
            {
                Debug.Log($"플래시 프리팹 참조가 NUll");
            }
        }

        private void SpawnHitEffect(Vector3 hitPos, Vector3 hitNor)
        {
            if (hit != null)
            {
                GameObject hitInstance = Instantiate(hit, hitPos+0.5f*hitNor, Quaternion.LookRotation(hitNor));
                if (hitInstance == null)
                {
                    Debug.Log("히트 게임 오브젝트 생성 실패");
                }
                hitInstance.transform.SetParent(null);
                ParticleSystem hitPs = hitInstance.GetComponent<ParticleSystem>();
                if (hitPs != null)
                {
                    hitPs.Play(true);
                    Destroy(hitInstance, hitPs.main.startLifetime.constant); // 수정된 부분: MinMaxCurve에서 constant 값을 사용
                }
                else if (hitPs == null)
                {
                    Debug.Log("히트 파티클 시스템이 Null임");
                    if (hitInstance.transform.childCount > 0)
                    {
                        var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                        if (hitPsParts == null)
                        {
                            Debug.Log("히트 : 자식에게서도 파티클 시스템을 찾을 수 없음");
                        }
                        hitPsParts.Play(true);
                        Destroy(hitInstance, hitPsParts.main.startLifetime.constant); // 수정된 부분: MinMaxCurve에서 constant 값을 사용
                    }
                }
            }
            else
            {
                Debug.Log("히트 게임오브젝트 참조가 null");
            }
        }
    }
}
