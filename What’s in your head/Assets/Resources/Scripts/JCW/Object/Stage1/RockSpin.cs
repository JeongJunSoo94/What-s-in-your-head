using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JCW.Object.Stage1
{
    public class RockSpin : MonoBehaviour
    {
        [Header("회전 속도")] [SerializeField] float speed = 10f;
        Transform myTF;
        Transform seatTF;

        float targetAxis = -120f;
        bool needToConvert;

        bool isStart = false;

        private void Awake()
        {
            myTF = this.transform;
            seatTF = myTF.GetChild(1);
            StartCoroutine(nameof(WaitForPlayer));
        }

        IEnumerator RotateObj()
        {
            float curAxis = myTF.rotation.eulerAngles.x >= 180f ? myTF.rotation.eulerAngles.x - 360f :
                        myTF.rotation.eulerAngles.x <= -180f ? myTF.rotation.eulerAngles.x + 360f : myTF.rotation.eulerAngles.x;
            if (targetAxis < 0f)
            {
                while (curAxis > targetAxis)
                {
                    if (curAxis < 0.1f && curAxis > -0.1f)
                    {
                        needToConvert = false;
                        speed = 150f;
                    }
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
                    if (curAxis < 0.1f && curAxis > -0.1f)
                    {
                        speed = 150f;
                        needToConvert = false;
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

            StartCoroutine(nameof(RotateObj));
            yield break;
        }


        IEnumerator WaitForPlayer()
        {
            yield return null;
            //yield return new WaitUntil(() => PhotonNetwork.PlayerList.Length == 2);
            //isStart = true;
            StartCoroutine(nameof(RotateObj));
            yield break;
        }
    }

}
