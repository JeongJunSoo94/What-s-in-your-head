using KSU.Object.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.Object.Stage1
{
    public class Lever : InteractableObject
    {
        [Header("회전속도")] [SerializeField] float rotSpeed = 80f;
        [Header("상호작용 오브젝트들")] [SerializeField] List<Transform> objList;

        Transform leverStickTF;
        int isRed = 0;
        //public bool isRot = false;

        List<Vector3> towardRotList = new();

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
            if (isInteractable)
                return;

            Vector3 curRot = leverStickTF.localRotation.eulerAngles;
            curRot.z = curRot.z >= 180f ? curRot.z - 360f : curRot.z <= -180f ? curRot.z + 360f : curRot.z;
            leverStickTF.localRotation = Quaternion.Euler(Vector3.MoveTowards(curRot, towardRotList[1 - isRed], Time.fixedDeltaTime * rotSpeed));
            if (curRot.z - towardRotList[1 - isRed].z <= 0.1f
                && curRot.z - towardRotList[1 - isRed].z >= -0.1f)
            {
                isRed = 1 - isRed;
                isInteractable = true;
                for (int i = 0; i < objList.Count; ++i)
                {
                    Vector3 targetRot = objList[i].localEulerAngles;
                    targetRot.z = targetRot.z == 0 ? 90f : 0f;
                    objList[i].localRotation = Quaternion.Euler(targetRot);
                }
            }
        }
    }
}
