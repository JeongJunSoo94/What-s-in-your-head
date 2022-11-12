using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JCW.Object.Stage1
{
    public class RockSpin : MonoBehaviour
    {
        [Header("회전 속도")] [SerializeField] float speed = 10f;
        //float curAxis;
        Transform myTF;
        Transform seatTF;

        //float targetAxis = -120f;
        //bool needToConvert;

        bool isStart = false;
        //float curAxis;

        private void Awake()
        {
            myTF = this.transform;
            seatTF = myTF.GetChild(1);
            StartCoroutine(nameof(WaitForPlayer));

            //curAxis = myTF.rotation.eulerAngles.x;
            //curAxis += (curAxis > 180 ? -360 : 0);
        }

        private void FixedUpdate()
        {
            if (!isStart)
                return;

            myTF.Rotate(Vector3.right, -speed * Time.fixedDeltaTime, Space.World);
            seatTF.Rotate(Vector3.right, speed * 2f * Time.fixedDeltaTime, Space.World);
            /*
            if(targetAxis < 0f)
            {
                Debug.Log("현재 각 : " + curAxis + " / 목표 각 : " + targetAxis);
                if (curAxis < 0.1f && curAxis > -0.1f)
                {
                    needToConvert = false;
                    speed = 150f;
                }
                speed = needToConvert ? speed - Time.fixedDeltaTime * 3f : speed + Time.fixedDeltaTime * 3f;

                //Debug.Log("리버스 전 : " + myTF.rotation.eulerAngles.x);
                myTF.Rotate(Vector3.right, -speed * Time.fixedDeltaTime, Space.World);
                seatTF.Rotate(Vector3.right, speed * 2f * Time.fixedDeltaTime, Space.World);

                curAxis = myTF.rotation.eulerAngles.x;
                curAxis += (curAxis > 180 ? -360 : 0);
                //myTF.rotation = Quaternion.Euler(curAxis, 0f, 0f);
                //curAxis = curAxis <= -90f && curAxis > -180f ? -180 - curAxis : 
                //    curAxis >= 90f && curAxis < 180f ? 180 - curAxis : curAxis;

                if (curAxis <= targetAxis)
                   targetAxis *= -1f;
            }
            else
            {
                if (curAxis < 0.1f && curAxis > -0.1f)
                {
                    needToConvert = false;
                    speed = 150f;
                }

               
                speed = needToConvert ? speed - Time.fixedDeltaTime * 3f : speed + Time.fixedDeltaTime * 3f;
                myTF.Rotate(-Vector3.right, -speed * Time.fixedDeltaTime, Space.World);
                seatTF.Rotate(-Vector3.right, speed * 2f * Time.fixedDeltaTime, Space.World);

                curAxis = myTF.rotation.eulerAngles.x;
                curAxis += (curAxis > 180 ? -360 : 0);
                myTF.rotation = Quaternion.Euler(curAxis, 0f, 0f);
                //curAxis = curAxis <= -90f && curAxis > -180f ? -180 - curAxis : 
                //    curAxis >= 90f && curAxis < 180f ? 180 - curAxis : curAxis;
                if (curAxis >= targetAxis)
                    targetAxis *= -1f;
            }
            needToConvert = true;
            */

        }
        /*
        IEnumerator RotateObj()
        {
            float curAxis = myTF.rotation.eulerAngles.x >= 180f ? myTF.rotation.eulerAngles.x - 360f :
                        myTF.rotation.eulerAngles.x <= -180f ? myTF.rotation.eulerAngles.x + 360f : myTF.rotation.eulerAngles.x;
            if (targetAxis < 0f)
            {
                while (curAxis > targetAxis)
                {
                    Debug.Log("현재 x각 : " + curAxis);
                    if (curAxis < 0.1f && curAxis > -0.1f)
                        speed = 150f;
                    speed = needToConvert ? speed - Time.deltaTime*3f : speed + Time.deltaTime*3f;

                    myTF.Rotate(Vector3.right, -speed * Time.deltaTime, Space.World);
                    seatTF.Rotate(Vector3.right, speed*2f * Time.deltaTime, Space.World);
                    curAxis = myTF.rotation.eulerAngles.x >= 180f ? myTF.rotation.eulerAngles.x - 360f : 
                        myTF.rotation.eulerAngles.x <= -180f ? myTF.rotation.eulerAngles.x + 360f : myTF.rotation.eulerAngles.x;
                    yield return null;
                }
                needToConvert = true;
                targetAxis *= -1f;
            }
            else
            {
                while (curAxis < targetAxis)
                {
                    Debug.Log("현재 x각 : " + curAxis);
                    if (curAxis < 0.1f && curAxis > -0.1f)
                    {
                        speed = 150f;
                        needToConvert = false;
                        break;
                    }
                    speed = needToConvert ? speed - Time.deltaTime*3f : speed + Time.deltaTime*3f;

                    myTF.Rotate(-Vector3.right, -speed * Time.deltaTime, Space.World);
                    seatTF.Rotate(-Vector3.right, speed*2f * Time.deltaTime, Space.World);
                    curAxis = myTF.rotation.eulerAngles.x >= 180f ? myTF.rotation.eulerAngles.x - 360f :
                        myTF.rotation.eulerAngles.x <= -180f ? myTF.rotation.eulerAngles.x + 360f : myTF.rotation.eulerAngles.x;
                    yield return null;
                }
                needToConvert = true;
                targetAxis *= -1f;
            }

            //StartCoroutine(nameof(RotateObj));
            yield break;
        }
        */

        IEnumerator WaitForPlayer()
        {
            yield return null;
            //yield return new WaitUntil(() => PhotonNetwork.PlayerList.Length == 2);
            isStart = true;
            //StartCoroutine(nameof(RotateObj));
            yield break;
        }
    }

}
