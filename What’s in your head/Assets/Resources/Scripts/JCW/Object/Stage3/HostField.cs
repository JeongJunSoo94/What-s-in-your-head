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

        int audioID = 0;

        private void Awake()
        {
            curHP = maxHP;            
            audioSource = GetComponent<AudioSource>();
            audioID = AudioCtrl.AudioSettings.SetAudio(audioSource, 0.05f, 30f);
            if (!canInfect)
            {                
                myIndex = transform.GetSiblingIndex();
            }
            else
            {
                parentObj = transform.parent.gameObject.transform;
                mediator = transform.parent.gameObject.GetComponent<MakeHostField>();
                //convertIndex = transform.parent.gameObject.GetComponent<ContaminationFieldSetting>().count;
                convertWidthIndex = transform.parent.gameObject.GetComponent<ContaminationFieldSetting>().widthCount;
                //convertHeightIndex = transform.parent.gameObject.GetComponent<ContaminationFieldSetting>().heightCount;
                nextTargetOffset = new() { 2, -2, 2 * convertWidthIndex, -2 * convertWidthIndex };
            }
            animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            if (isPurified)
                this.gameObject.SetActive(false);
            else
                SoundManager.Instance.Play3D_RPC("S3_ContaminationFieldCreated", audioID);
        }

        private void OnDisable()
        {
            isPurified = true;
        }


        void Update()
        {
            if (!isStart)
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
                //audioSource.Play();                
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
                animator.Play("Destroy");
                SoundManager.Instance.Play3D_RPC("S3_ContaminationFieldPurified", audioID);
            }
        }
        public void DestroyField()
        {
            this.enabled = false;
            this.gameObject.SetActive(false);
        }
    }
}
