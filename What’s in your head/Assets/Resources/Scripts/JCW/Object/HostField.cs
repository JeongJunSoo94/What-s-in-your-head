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
        [Header("���� ������ų �ʵ� ã�� �ð�")] [SerializeField] [Range(0, 13)] float infectTime = 5f;
        [Header("������ �ѷ� (�⺻ 100)")] [SerializeField] [Range(0,100)] float maxHP = 100f;
        [Header("������ ȸ�� �ӵ� (�⺻ �ʴ� 1)")] [SerializeField] [Range(0,10)] float healingSpeed = 1f;
        [Header("���ѿ� ���� �� �޴� ������ (�⺻ 10)")] [SerializeField] [Range(0,100)] float damage = 10;
        [Header("���� ���� ����")] [SerializeField] bool canInfect = true;

        public bool isPurified = false;
        int myIndex;

        float elapsedTime = 0f;

        // ������ �ʵ��� ���� ������
        List<int> nextTargetOffset;
        Transform parentObj;

        // ������ �ʵ� �����ʰ� ������ �ε��� �ִ�ġ
        int maxLimit;

        int convertIndex;

        bool isStart = false;

        // ������ �ʵ� �����ʷ� �Ͽ��� ���� ����
        MakeHostField mediator;

        float curHP;
        AudioSource audioSource;

        private void Awake()
        {
            curHP = maxHP;
            parentObj = transform.parent.gameObject.transform;
            mediator = transform.parent.gameObject.GetComponent<MakeHostField>();
            convertIndex = transform.parent.gameObject.GetComponent<ContaminationFieldSetting>().count;
            nextTargetOffset = new() { 2, -2, 2 * convertIndex, -2 * convertIndex };
            audioSource = GetComponent<AudioSource>();
            AudioCtrl.AudioSettings.SetAudio(audioSource, 1f, 60f);
            if (!canInfect)
            {
                nextTargetOffset.Clear();
                myIndex = transform.GetSiblingIndex();
            }
        }

        private void OnEnable()
        {
            if (isPurified)
            {
                Debug.Log("�̹� ��ȭ�Ǿ���");
                this.gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {
            Debug.Log("��ȭ��");
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

                    // ������ų �ε����� ���� ����ų�, �̹� �������� ��
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
                if ((myIndex - 1) % (convertIndex * 2) == 0)      nextTargetOffset.Remove(-2);
                else if ((myIndex + 1) % (convertIndex * 2) == 0) nextTargetOffset.Remove(2);

                if (myIndex < 2 * convertIndex)                  nextTargetOffset.Remove(-2 * convertIndex);
                else if (myIndex + 2 * convertIndex > maxLimit)  nextTargetOffset.Remove(2 * convertIndex);
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
                SetIndex(index);
                isStart = true;
                audioSource.Play();
                SoundManager.Instance.Play3D_RPC("ContaminationFieldCreated", audioSource);
            }
            
        }

        private void OnTriggerEnter(Collider other)
        {
            switch (other.gameObject.tag)
            {
                case "Nella":
                case "Steady":
                    other.gameObject.GetComponent<PlayerController>().Resurrect();
                    break;
            }
        }

        public void GetDamaged()
        {
            curHP -= damage;
            //Debug.Log(curHP);
            if (curHP <= 0)
            {
                SoundManager.Instance.Play3D_RPC("ContaminationFieldPurified", audioSource);
                mediator.SetPurified(myIndex);
            }
        }
    }
}
