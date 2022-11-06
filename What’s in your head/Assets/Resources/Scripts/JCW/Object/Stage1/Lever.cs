using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    [Header("회전속도")] [SerializeField] float rotSpeed = 80f;
    [Header("상호작용 오브젝트들")] [SerializeField] List<Transform> objList;  

    Transform leverStickTF;
    int isRed = 0;
    public bool isRot = false;

    List<Vector3> towardRotList = new();

    private void Awake()
    {
        leverStickTF = transform.GetChild(1);
        towardRotList.Add(leverStickTF.localRotation.eulerAngles);
        Vector3 towardRot = towardRotList[0];
        towardRot.z = -towardRot.z;
        towardRotList.Add(towardRot);
    }

    void Update()
    {
        if (!isRot)
            return;

        Vector3 curRot = leverStickTF.localRotation.eulerAngles;
        curRot.z = curRot.z >= 180f ? curRot.z - 360f : curRot.z <= -180f ? curRot.z + 360f : curRot.z;
        leverStickTF.localRotation = Quaternion.Euler(Vector3.MoveTowards(curRot, towardRotList[1-isRed], Time.deltaTime * rotSpeed));        
        if (curRot.z - towardRotList[1-isRed].z <= 0.1f
            && curRot.z - towardRotList[1-isRed].z >= -0.1f)
        {
            isRed = 1 - isRed;
            isRot = false;
            for (int i = 0 ; i<objList.Count ; ++i)
            {
                Vector3 targetRot = objList[i].localEulerAngles;
                targetRot.z = targetRot.z == 0 ? 90f : 0f;
                objList[i].localRotation = Quaternion.Euler(targetRot);
            }
        }
    }
}
