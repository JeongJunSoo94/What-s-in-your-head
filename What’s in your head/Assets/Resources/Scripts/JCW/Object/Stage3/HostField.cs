using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KSU;
using JCW.AudioCtrl;

namespace JCW.Object
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(PhotonView))]
    public class HostField : MonoBehaviour
    {
        [Header("다음 감염시킬 필드 찾는 시간")] [SerializeField] [Range(0, 13)] float infectTime = 5f;
        [Header("게이지 총량 (기본 100)")] [SerializeField] [Range(0,100)] float maxHP = 100f;
        [Header("게이지 회복 속도 (기본 초당 1)")] [SerializeField] [Range(0,10)] float healingSpeed = 1f;
        [Header("물총에 닿을 시 받는 데미지 (기본 10)")] [SerializeField] [Range(0,100)] float damage = 10;
        [Header("감염 가능 여부")] [SerializeField] bool canInfect = true;

        public bool isPurified = false;
        int myIndex;

        float elapsedTime = 0f;

        // 감염된 필드의 인접 오프셋
        List<int> nextTargetOffset;
        Transform parentObj;

        // 오염된 필드 스포너가 가지는 인덱스 최대치
        int maxLimit;

        int convertWidthIndex;
        //int convertIndex;
        //int convertHeightIndex;

        bool isStart = false;
        bool isDead = false;

        // 오염된 필드 스포너로 하여금 간접 실행
        MakeHostField mediator;

        float curHP;
        AudioSource audioSource;
        Animator animator;
        PhotonView pv;

        WaitUntil wu;

        bool isFirst = true;
        BoxCollider boxCollider;

        private void Awake()
        {
            wu = new WaitUntil(() => GameManager.Instance.GetCharOnScene());
            curHP = maxHP;            
            audioSource = GetComponent<AudioSource>();
            pv = GetComponent<PhotonView>();
            SoundManager.Set3DAudio(pv.ViewID, audioSource, 0.6f, 40f);
            boxCollider = GetComponent<BoxCollider>();

            animator = GetComponent<Animator>();

            if (!canInfect)
            {
                myIndex = transform.GetSiblingIndex();
            }
            else
            {
                parentObj = transform.parent.gameObject.transform;
                mediator = transform.parent.gameObject.GetComponent<MakeHostField>();
                convertWidthIndex = transform.parent.gameObject.GetComponent<ContaminationFieldSetting>().widthCount;
                nextTargetOffset = new() { 2, -2, 2 * convertWidthIndex, -2 * convertWidthIndex };
            }
        }

        private void OnEnable()
        {
            if ((isFirst && canInfect) || isPurified)
                this.gameObject.SetActive(false);
            else if(canInfect)
                SoundManager.Instance.Play3D("S3_ContaminationFieldCreated", pv.ViewID);
        }

        private void OnDisable()
        {
            if(isFirst && canInfect)
                isFirst = false;
            else
                isPurified = true;
        }


        void Update()
        {
            if (!isStart || isFirst)
                return;
            if (isPurified)
            {
                this.gameObject.SetActive(false);
                return;
            }
            else if (nextTargetOffset.Count != 0 && PhotonNetwork.IsMasterClient)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime > infectTime)
                {
                    elapsedTime = 0f;
                    var random = new System.Random(Guid.NewGuid().GetHashCode());
                    int i = random.Next(0, nextTargetOffset.Count);

                    // 전염시킬 인덱스가 수를 벗어나거나, 이미 켜져있을 때
                    while (myIndex + nextTargetOffset[i] < 0 || myIndex + nextTargetOffset[i] > maxLimit
                        || parentObj.GetChild(myIndex + nextTargetOffset[i]).gameObject.activeSelf)
                    {
                        nextTargetOffset.Remove(nextTargetOffset[i]);
                        if (nextTargetOffset.Count == 0)
                            return;
                        i = random.Next(0, nextTargetOffset.Count);
                    }

                    mediator.Infect(myIndex, nextTargetOffset[i]);
                    nextTargetOffset.RemoveAt(i);
                }
            }
            curHP = curHP >= maxHP ? maxHP : curHP+Time.deltaTime*healingSpeed;
        }

        public void SetIndex(int index)
        {
            if (!isPurified)
            {
                maxLimit = transform.parent.gameObject.transform.childCount - 1;
                myIndex = index;
                //Debug.Log(convertIndex);
                if ((myIndex - 1) % (convertWidthIndex * 2) == 0)      nextTargetOffset.Remove(-2);
                else if ((myIndex + 1) % (convertWidthIndex * 2) == 0) nextTargetOffset.Remove(2);

                if (myIndex < 2 * convertWidthIndex)                  nextTargetOffset.Remove(-2 * convertWidthIndex);
                else if (myIndex + 2 * convertWidthIndex > maxLimit)  nextTargetOffset.Remove(2 * convertWidthIndex);
            }
        }

        public void DeleteHost(int hostIndex)
        {
            if (!isPurified)
            {
                nextTargetOffset.Remove(hostIndex - myIndex);
                isStart = true;
            }            

        }

        public void SetStart(int index)
        {
            if (!isPurified)
            {
                gameObject.SetActive(true);
                SetIndex(index);
                isStart = true;     
            }
            
        }

        private void OnTriggerEnter(Collider other)
        {
            switch (other.gameObject.tag)
            {
                case "Nella":
                case "Steady":
                    other.GetComponent<PlayerController>().GetDamage(12, DamageType.Dead);
                    break;
                case "NellaWater":
                    GetDamaged();
                    break;

            }
        }

        public void GetDamaged()
        {
            curHP -= damage;
            //Debug.Log(curHP);
            if (curHP <= 0 && !isDead)
            {
                isDead = true;
                isPurified = true;
                animator.Play("Destroy");
                boxCollider.enabled = false;
                SoundManager.Instance.Play3D("S3_ContaminationFieldPurified", pv.ViewID);
            }
        }
        // 애니메이션 클립에서 실행.
        public void DestroyField()
        {
            this.gameObject.SetActive(false);
            this.enabled = false;
        }

    }
}
