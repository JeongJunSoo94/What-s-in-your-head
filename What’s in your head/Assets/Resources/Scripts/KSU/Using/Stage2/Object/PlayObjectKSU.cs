using JCW.AudioCtrl;
using KSU;
using KSU.Object.Stage2;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.Object
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(PhotonView))]
    public class PlayObjectKSU : LinkedObjectWithReciever
    {
        [Header("신호 수신 시 이동 속도")] [SerializeField] float recieveMovingSpeed;
        [Header("신호 해제 시 복귀 속도")] [SerializeField] float comebackMovingSpeed;
        [Header("접촉 시 사망 여부")] [SerializeField] bool isLethal;
        [Header("플레이어가 탈 수 있는 지 여부")] [SerializeField] bool canRide;
        [Header("이동 경로")] [SerializeField] List<Vector3> positionList;
        [Header("소리 On/Off")] [SerializeField] bool isSoundOn = false;
        [Space(20f)]
        public bool isActive = false;
        int curIndex = 0;
        int maxIndex = 0;

        bool isStart = false;

        AudioSource audioSource;
        PhotonView pv;

        private void Awake()
        {
            if (positionList.Count == 0)
                positionList.Add(transform.position);
            transform.position = positionList[0];
            maxIndex = positionList.Count - 1;
            if(isSoundOn)
            {
                audioSource = GetComponent<AudioSource>();
                pv = GetComponent<PhotonView>();
                SoundManager.Set3DAudio(pv.ViewID, audioSource, 0.7f, 25f, true);
            }            
        }

        void Update()
        {
            if (isActivated)
            {
                if (!isStart)
                {
                    StopAllCoroutines();
                    StartCoroutine(nameof(MoveToEnd));
                    isStart = true;
                }
            }
            else if (isStart)
            {
                StopAllCoroutines();
                isStart = false;
                StartCoroutine(nameof(MoveToFirst));
            }
        }

        IEnumerator MoveToEnd()
        {
            if (positionList.Count <= 1 || curIndex >= maxIndex)
                yield break;
            if (isSoundOn)
                SoundManager.Instance.PlayIndirect3D_RPC("S2_PlayObject_Move", pv.ViewID);

            while (curIndex < maxIndex)
            {
                transform.position = Vector3.MoveTowards(transform.position, positionList[curIndex + 1], Time.deltaTime * recieveMovingSpeed);
                if (Vector3.SqrMagnitude(transform.position - positionList[curIndex + 1]) <= 0.05f)
                    transform.position = positionList[++curIndex];
                yield return null;
            }

            if (curIndex > maxIndex)
                curIndex = maxIndex;

            if (isSoundOn)
                SoundManager.Instance.Stop3D_RPC(pv.ViewID);

            yield break;
        }

        IEnumerator MoveToFirst()
        {
            if (positionList.Count <= 1)
                yield break;
            if (isSoundOn)
                SoundManager.Instance.PlayIndirect3D_RPC("S2_PlayObject_Move", pv.ViewID);

            if (curIndex == maxIndex)
                --curIndex;

            while (curIndex >= 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, positionList[curIndex], Time.deltaTime * comebackMovingSpeed);
                if (Vector3.SqrMagnitude(transform.position - positionList[curIndex]) <= 0.05f)
                    transform.position = positionList[curIndex--];
                yield return null;
            }

            if (curIndex < 0)
                curIndex = 0;

            if (isSoundOn)
                SoundManager.Instance.Stop3D_RPC(pv.ViewID);
            this.enabled = false;

            yield break;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Nella") || collision.gameObject.CompareTag("Steady"))
            {
                Debug.Log("오브젝트 접근");
                Transform playerTF = collision.gameObject.transform;
                if (isLethal)
                    playerTF.gameObject.GetComponent<PlayerController>().GetDamage(12,DamageType.Dead);
                else if (canRide && playerTF.position.y >= this.transform.position.y)
                    playerTF.parent = this.transform;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Nella") || collision.gameObject.CompareTag("Steady"))
            {
                Transform playerTF = collision.gameObject.transform;
                if (playerTF.IsChildOf(transform))
                    playerTF.parent = null;
            }
        }
    }

}
