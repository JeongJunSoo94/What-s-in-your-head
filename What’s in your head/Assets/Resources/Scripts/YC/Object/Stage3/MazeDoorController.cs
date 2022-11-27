using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.AudioCtrl;
using Photon.Pun;

public class MazeDoorController : MonoBehaviour
{
    [SerializeField] float openLerpTime;
    [SerializeField] float closeLerpTime;

    [SerializeField] float openDelayTime;
    [SerializeField] float closeDelayTime;
    float originPosY;
 
    bool isCor;

    AudioSource audioSource;
    PhotonView pv;


    private void Awake()
    {
        originPosY = this.gameObject.transform.position.y;
        audioSource = this.gameObject.GetComponent<AudioSource>();
        pv = this.gameObject.GetComponent<PhotonView>();
    }

    private void Start()
    {
        SoundManager.Set3DAudio(pv.ViewID, audioSource, 1.5f, 30f, false);
    }

    public void ControlDoor(bool open)
    {

        if (isCor)
            StopAllCoroutines();

        StartCoroutine(ControlDoorCoroutine(open));
    }
 

    IEnumerator ControlDoorCoroutine(bool open)
    {
        isCor = true;

        if (open)
        {
            SoundManager.Instance.Play3D_RPC("S3S1_MazeDoor", pv.ViewID);
            yield return new WaitForSeconds(openDelayTime);
        }
        else
        {
            SoundManager.Instance.Play3D_RPC("S3S1_MazeDoor", pv.ViewID);
            yield return new WaitForSeconds(closeDelayTime);
        }


        float currentTime = 0;
        float lerpYpos;

        float height = 15;
        Vector3 thisPos = this.gameObject.transform.position;

        if (open)
        {
            float curPosY = this.gameObject.transform.position.y;
            float targetPosY = originPosY - height;

            while (curPosY > targetPosY)
            {
                curPosY = this.gameObject.transform.position.y;

                currentTime += Time.deltaTime;
                if (currentTime >= openLerpTime) currentTime = openLerpTime;
   
                lerpYpos = Mathf.Lerp(curPosY, targetPosY, currentTime / openLerpTime);
                if (lerpYpos < targetPosY) lerpYpos = targetPosY;

                this.gameObject.transform.position = new Vector3(thisPos.x, lerpYpos, thisPos.z);

                yield return null;
            }
        }
        else
        {
            float curPosY = this.gameObject.transform.position.y;

            while (curPosY < originPosY)
            {

                curPosY = this.gameObject.transform.position.y;

                currentTime += Time.deltaTime;
                if (currentTime >= closeLerpTime) currentTime = closeLerpTime;           

                lerpYpos = Mathf.Lerp(curPosY, originPosY, currentTime / closeLerpTime);
                if (lerpYpos > originPosY) lerpYpos = originPosY;

                this.gameObject.transform.position = new Vector3(thisPos.x, lerpYpos, thisPos.z);

                yield return null;
            }
        }
        isCor = false;
    }
}
