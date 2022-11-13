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
        [Header("����Ʈ ����")] [SerializeField] bool isSet = false;
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
            if (isSet)
            {
                needToClear = false;
                needToSetPos = false;
            }
            if(needToSetPos && !isSet)
            {
                playObject.SetPositionToList(transform.position);
                needToSetPos = false;
            }
            if(needToClear && !isSet)
            {
                playObject.ClearList();
                needToClear = false;
            }
        }
    }
}

