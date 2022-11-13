using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.Object
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(PlayObject))]
    public class PlayObject_SetPos : MonoBehaviour
    {
        [Header("현 위치 저장")] [SerializeField] bool needToSetPos = false;
        [Header("리스트 초기화")] [SerializeField] bool needToClear = false;
        [Header("리스트 고정")] [SerializeField] bool isSet = false;
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
            // 현재 실행 중이면 해당 스크립트 제거
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

