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
            // 현재 실행 중이면 해당 스크립트 제거
            if (Application.isPlaying)
            {
                Debug.Log("실행 중이므로 삭제");

                Debug.Log("needToSetPos : " + needToSetPos);
                Debug.Log("needToClear : " + needToClear);
                needToSetPos = false;
                needToClear = false;
                Destroy(this);
                return;
            }
            if(needToSetPos)
            {
                Debug.Log("추가");
                playObject.SetPositionToList(transform.position);
                needToSetPos = false;
            }
            if(needToClear)
            {
                Debug.Log("클리어");
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

