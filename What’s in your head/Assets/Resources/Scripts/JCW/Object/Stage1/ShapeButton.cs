using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.Object.Stage1
{
    public class ShapeButton : MonoBehaviour
    {
        [Header("버튼 종류")]
        [Header("0 : 별 / 1 : 태양 / 2 : 달")] [SerializeField] [Range(0,2)] int buttonType = 0;
        [Header("활성화시킬 오브젝트 목록들")] [SerializeField] List<GameObject> activeObjects;
        [Header("비활성화시킬 오브젝트 목록들")] [SerializeField] List<GameObject> inactiveObjects;

        Vector3 pressedPos;
        Vector3 unpressedPos;

        private void Awake()
        {
            transform.GetChild(0).GetChild(buttonType).gameObject.SetActive(true);
            unpressedPos = transform.localPosition;
            pressedPos = unpressedPos;
            pressedPos.y -= 0.6f;
            for (int i = 0 ; i< activeObjects.Count ; ++i)
            {
                activeObjects[i].SetActive(false);
            }
            for (int j = 0 ; j< inactiveObjects.Count ; ++j)
            {
                inactiveObjects[j].SetActive(true);
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Nella") || collision.gameObject.CompareTag("Steady"))
            {
                StartCoroutine("ButtonDown");
            }                
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Nella") || collision.gameObject.CompareTag("Steady"))
            {
                StartCoroutine("ButtonUp");
            }
        }

        IEnumerator ButtonDown()
        {            
            StopCoroutine("ButtonUp");
            while (transform.localPosition.y > pressedPos.y)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, pressedPos, Time.deltaTime * 5f);
                yield return null;
            }

            for (int i = 0 ; i< activeObjects.Count ; ++i)
            {
                activeObjects[i].SetActive(true);
            }
            for (int j = 0 ; j< inactiveObjects.Count ; ++j)
            {
                inactiveObjects[j].SetActive(false);
            }
        }

        IEnumerator ButtonUp()
        {
            float curTime = 0f;
            while (curTime<0.5f)
            {
                curTime += Time.deltaTime;
                yield return null;
            }
            StopCoroutine("ButtonDown");
            while (transform.localPosition.y < unpressedPos.y)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, unpressedPos, Time.deltaTime * 5f);
                yield return null;
            }

            for (int i = 0 ; i< activeObjects.Count ; ++i)
            {
                activeObjects[i].SetActive(false);
            }
            for (int j = 0 ; j< inactiveObjects.Count ; ++j)
            {
                inactiveObjects[j].SetActive(true);
            }
        }
    }
}

