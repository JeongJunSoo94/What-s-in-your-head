using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.Object
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(PlayObject))]
    public class PlayObject_SetPos : MonoBehaviour
    {
        [Header("�� ��ġ ����")] [SerializeField] bool needToSetPos = false;
        [Header("����Ʈ �ʱ�ȭ")] [SerializeField] bool needToClear = false;
        PlayObject playObject;

        private void Start()
        {
            if (Application.isEditor)
            {
                if (playObject == null)
                    playObject = GetComponent<PlayObject>();
            }
            else
            {
                needToSetPos = false;
                needToClear = false;
                Destroy(this);
                return;
            }
        }

        void Update()
        {
            // ���� ���� ���̸� �ش� ��ũ��Ʈ ����
            if (Application.isPlaying)
            {
                Debug.Log("���� ���̹Ƿ� ����");

                Debug.Log("needToSetPos : " + needToSetPos);
                Debug.Log("needToClear : " + needToClear);
                needToSetPos = false;
                needToClear = false;
                Destroy(this);
                return;
            }
            if(needToSetPos)
            {
                Debug.Log("�߰�");
                playObject.SetPositionToList(transform.position);
                needToSetPos = false;
            }
            if(needToClear)
            {
                Debug.Log("Ŭ����");
                playObject.ClearList();
                needToClear = false;
            }
        }

        private void OnApplicationQuit()
        {
            needToSetPos = false;
            needToClear = false;
        }
    }
}

