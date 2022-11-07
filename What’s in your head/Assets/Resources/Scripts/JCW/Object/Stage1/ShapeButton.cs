using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.Object.Stage1
{
    public class ShapeButton : MonoBehaviour
    {
        [Header("��ư ����")]
        [Header("0 : �� / 1 : �¾� / 2 : ��")] [SerializeField] [Range(0,2)] int buttonType = 0;
        [Header("Ȱ��ȭ��ų ������Ʈ ��ϵ�")] [SerializeField] List<GameObject> activeObjects;
        [Header("��Ȱ��ȭ��ų ������Ʈ ��ϵ�")] [SerializeField] List<GameObject> inactiveObjects;

        bool isPressed = false;
        Vector3 pressedPos;

        private void Awake()
        {
            transform.GetChild(0).GetChild(buttonType).gameObject.SetActive(true);
            pressedPos = transform.localPosition;
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

        private void Update()
        {
            if (!isPressed)
                return;
            if (transform.localPosition.y > pressedPos.y)
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, pressedPos, Time.deltaTime * 5f);
            else
            {
                for (int i = 0 ; i< activeObjects.Count ; ++i)
                {
                    activeObjects[i].SetActive(true);
                }
                for (int j = 0 ; j< inactiveObjects.Count ; ++j)
                {
                    inactiveObjects[j].SetActive(false);
                }
                Destroy(this);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Nella") || collision.gameObject.CompareTag("Steady"))
            {
                if(collision.transform.position.y >= this.transform.position.y)
                    isPressed = true;
            }
                
        }
    }
}

