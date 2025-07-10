using System;
using UnityEngine;

namespace JYL
{
    public class BulletController : MonoBehaviour
    {
        [Range(0.05f, 1f)][SerializeField] float ticTime = 0.3f;
        [SerializeField] GameObject flash; // TODO: �Ѿ� ������ �ϳ��ϳ� �� ���� �޾���� ��
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
                Debug.LogWarning("��ƼŬ �ý����� ����");
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
        //if(gameObject.layer == 7) // �÷��̾�
        //{
        //    //���ǿ� ���� SetActive false
        //    // ������ ����ũ ������
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
        //    // ���� �Ѿ� ��� ó��
        //    PlayerController enemy =other.GetComponent<PlayerController>();
        //    if (enemy == null)
        //    {
        //        gameObject.SetActive(false);
        //        return;
        //    }
        //    if (!canDeactive && timer <= 0)
        //    {
        //        //enemy.TakeDamage(attackPower); // TODO:�÷��̾� �����ʿ�
        //    }
        //    else if (canDeactive)
        //    {
        //        //enemy.TakeDamage(attackPower); // TODO:�÷��̾� �����ʿ�
        //        gameObject.SetActive(false);
        //    }
        //}

        private void OnTriggerStay(Collider other)
        {
            Vector3 contactPoint = other.ClosestPoint(transform.position);
            Vector3 hitNormal = -transform.forward;
            if (gameObject.layer == 7) // �÷��̾��� �Ѿ��� ���
            {
                Enemy enemy = other.GetComponentInParent<Enemy>();
                if (enemy == null)
                {
                    Debug.Log("���ʹ� ������Ʈ�� ã�� ����");
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
            Debug.Log("Ʈ���� �ν�");
            if (gameObject.layer == 9) // ���ʹ��� �Ѿ��� ���
            {
                Debug.Log("�Ѿ��� ���� �Ѿ��� ���.");
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
            if(canDeactive)
            {
                gameObject.SetActive(false);
            }
        }
        private void OnDisable(){ }
        public void OnFire()
        {
            if(ps != null)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.Play(true);
            }
            
            if (flash != null)
            {
                GameObject flashInstance = Instantiate(flash, transform.position, Quaternion.LookRotation(transform.forward));
                if(flashInstance == null)
                {
                    Debug.Log("�÷��� ���ӿ�����Ʈ ���� ���� NUll");
                }
                ParticleSystem flashPs = flashInstance.GetComponent<ParticleSystem>();
                if (flashPs != null)
                {
                    flashPs.Play(true);
                    Destroy(flashInstance, flashPs.main.startLifetime.constant);
                }
                else if(flashPs == null)
                {
                    Debug.Log("�÷��� ��ƼŬ �ý��� ������Ʈ�� NUll");
                    ParticleSystem flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                    Destroy(flashInstance, flashPsParts.main.startLifetime.constant);
                    if(flashPsParts == null)
                    {
                        Debug.Log("�÷����� �ڽĵ� ��ƼŬ �ý��� ������Ʈ�� Null");
                    }
                    
                }
                
            }
            else if(flash == null)
            {
                Debug.Log($"�÷��� ������ ������ NUll");
            }
        }

        private void SpawnHitEffect(Vector3 hitPos, Vector3 hitNor)
        {
            if (hit != null)
            {
                GameObject hitInstance = Instantiate(hit, hitPos+0.5f*hitNor, Quaternion.LookRotation(hitNor));
                if (hitInstance == null)
                {
                    Debug.Log("��Ʈ ���� ������Ʈ ���� ����");
                }
                hitInstance.transform.SetParent(null);
                ParticleSystem hitPs = hitInstance.GetComponent<ParticleSystem>();
                if (hitPs != null)
                {
                    hitPs.Play(true);
                    Destroy(hitInstance, hitPs.main.startLifetime.constant); // ������ �κ�: MinMaxCurve���� constant ���� ���
                }
                else if (hitPs == null)
                {
                    Debug.Log("��Ʈ ��ƼŬ �ý����� Null��");
                    if (hitInstance.transform.childCount > 0)
                    {
                        var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                        if (hitPsParts == null)
                        {
                            Debug.Log("��Ʈ : �ڽĿ��Լ��� ��ƼŬ �ý����� ã�� �� ����");
                        }
                        hitPsParts.Play(true);
                        Destroy(hitInstance, hitPsParts.main.startLifetime.constant); // ������ �κ�: MinMaxCurve���� constant ���� ���
                    }
                }
            }
            else
            {
                Debug.Log("��Ʈ ���ӿ�����Ʈ ������ null");
            }
        }
    }
}
