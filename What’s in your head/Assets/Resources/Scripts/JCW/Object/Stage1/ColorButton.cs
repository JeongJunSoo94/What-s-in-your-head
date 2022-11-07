using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.Object.Stage1
{
    public class ColorButton : MonoBehaviour
    {
        [Header("현재 컬러")]
        [Header("0~6 : 빨강~보라")] [SerializeField] [Range(0,6)] int colorNum;
        [Header("활성화시킬 오브젝트 목록들")] [SerializeField] List<GameObject> activeObjects;
        [Header("재생성 쿨타임")] [SerializeField] [Range(0,10)] float resetTime = 7f;

        bool isPressed = false;
        bool isEnd = false;
        int bothPress = 0;
        Vector3 pressedPos;
        Transform buttonTF;

        Coroutine coroutine;

        private void Awake()
        {
            for (int i = 0 ; i<7 ; ++i)
            {
                if (i == colorNum)
                    buttonTF = transform.GetChild(i).GetChild(0).gameObject.transform;
                transform.GetChild(i).gameObject.SetActive(i == colorNum);
            }
            pressedPos = buttonTF.localPosition;
            pressedPos.y -= 0.6f;
        }

        private void Update()
        {
            if (!isPressed || isEnd)
                return;
            if (buttonTF.localPosition.y > pressedPos.y)
                buttonTF.localPosition = Vector3.MoveTowards(buttonTF.localPosition, pressedPos, Time.deltaTime * 5f);
            else
            {
                var random = new System.Random(Guid.NewGuid().GetHashCode());
                for (int i = 0 ; i < activeObjects.Count ; ++i)
                {   
                    activeObjects[i].SetActive(false);
                }
                isEnd = true;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Nella") || collision.gameObject.CompareTag("Steady"))
            {
                Debug.Log("버튼 접촉 확인");
                if (collision.transform.position.y >= buttonTF.position.y)
                {
                    StopCoroutine(coroutine);
                    isPressed = true;
                    ++bothPress;
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Nella") || collision.gameObject.CompareTag("Steady"))
            {
                --bothPress;
                if(bothPress <= 0)
                {
                    isPressed = false;
                    coroutine = StartCoroutine(nameof(Reset));
                }
            }
        }

        IEnumerator Reset()
        {
            float curTime = 0f;
            while (curTime < resetTime)
            {
                curTime += Time.deltaTime;
                yield return null;
            }
            for (int i = 0 ; i < activeObjects.Count ; ++i)
            {
                activeObjects[i].SetActive(true);
            }
            isEnd = false;
            yield break;

        }
    }

}
