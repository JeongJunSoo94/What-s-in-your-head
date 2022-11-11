using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeDoorController : MonoBehaviour
{
    [SerializeField] float openLerpTime;
    [SerializeField] float closeLerpTime;
    [SerializeField] float delayTime;

    float originPosY;
 
    bool isCor;

    private void Awake()
    {
        originPosY = this.gameObject.transform.position.y;
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

        yield return new WaitForSeconds(delayTime);

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
