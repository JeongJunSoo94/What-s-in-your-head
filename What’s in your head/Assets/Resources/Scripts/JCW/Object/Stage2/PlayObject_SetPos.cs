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

        private void Awake()
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
                needToSetPos = false;
                needToClear = false;
                Destroy(this);
                return;
            }
            if(needToSetPos)
            {
                playObject.SetPositionToList(transform.position);
                needToSetPos = false;
            }
            if(needToClear)
            {
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

