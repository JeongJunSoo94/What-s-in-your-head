using KSU.Object.Interaction;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.Object.Stage1
{
    public class Lever : InteractableObject
    {
        [Header("레버 회전속도")] [SerializeField] float rotSpeed = 80f;
        [Header("아이스크림 회전속도")] [SerializeField] float iceRotSpeed = 160f;
        [Header("상호작용 오브젝트들")] [SerializeField] List<Transform> objList;

        Transform leverStickTF;
        int isRed = 0;
        bool canStart = true;

        List<Vector3> towardRotList = new();
        Coroutine coroutine = null;

        override protected void Awake()
        {
            base.Awake();
            leverStickTF = transform.GetChild(2);
            towardRotList.Add(leverStickTF.localRotation.eulerAngles);
            Vector3 towardRot = towardRotList[0];
            towardRot.z = -towardRot.z;
            towardRotList.Add(towardRot);
        }

        void FixedUpdate()
        {
            if (isInteractable || !canStart )
                return;

            canStart = false;
            RotateRPC();
        }

        void RotateRPC()
        {
            photonView.RPC(nameof(RotateObject_RPC), RpcTarget.AllViaServer);
        }

        [PunRPC]
        void RotateObject_RPC()
        {
            if (coroutine == null)
                coroutine = StartCoroutine(nameof(RotateObject));
        }

        IEnumerator RotateObject()
        {
            canStart = false;
            Vector3 curRot = leverStickTF.localRotation.eulerAngles;
            while (curRot.z - towardRotList[1 - isRed].z > 0.1f
                || curRot.z - towardRotList[1 - isRed].z < -0.1f)
            {
                curRot = leverStickTF.localRotation.eulerAngles;
                curRot.z = curRot.z >= 180f ? curRot.z - 360f : curRot.z <= -180f ? curRot.z + 360f : curRot.z;
                leverStickTF.localRotation = Quaternion.Euler(Vector3.MoveTowards(curRot, towardRotList[1 - isRed], Time.deltaTime * rotSpeed));
                yield return null;
            }

            isInteractable = true;
            isRed = 1 - isRed;
            for (int i = 0 ; i < objList.Count ; ++i)
            {
                StartCoroutine(RotateIceCream(objList[i]));
                //Vector3 targetRot = objList[i].localEulerAngles;
                //targetRot.x = targetRot.x == 0 ? 90f : 0f;
                //objList[i].localRotation = Quaternion.Euler(targetRot);
            }
            coroutine = null;
            canStart = true;
            yield break;
        }

        IEnumerator RotateIceCream(Transform target)
        {
            Vector3 targetRot = target.localEulerAngles;
            Vector3 curRot = targetRot;
            targetRot.x = targetRot.x == 0 ? 90f : 0f;

            while (curRot.x - targetRot.x > 0.1f
                || curRot.x - targetRot.x < -0.1f)
            {
                curRot = target.localRotation.eulerAngles;
                curRot.x = curRot.x >= 180f ? curRot.x - 360f : curRot.x <= -180f ? curRot.x + 360f : curRot.x;
                target.localRotation = Quaternion.Euler(Vector3.MoveTowards(curRot, targetRot, Time.deltaTime * iceRotSpeed));
                yield return null;
            }
            yield break;
        }
    }
}
