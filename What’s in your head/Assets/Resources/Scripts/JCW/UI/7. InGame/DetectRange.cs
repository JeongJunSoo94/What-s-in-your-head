using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YC.Camera_;
using YC.Camera_Single;

public class DetectRange : MonoBehaviour
{
    [Header("감지 범위 수치")] [SerializeField] [Range(0, 100)] float range;
    [Header("UI")] [SerializeField] GameObject detectUI;
    [Header("기본 이미지")] [SerializeField] GameObject detectImg;
    [Header("월드뷰 이미지")] [SerializeField] GameObject detectImg2;
    [Header("뷰포트 이미지")] [SerializeField] GameObject detectImg3;
    [Header("오브젝트")] [SerializeField] GameObject centerObj;

    bool isDetected = false;
    Canvas camOnUI;

    private void Awake()
    {
        transform.localScale = new Vector3(range, range, range);
        camOnUI = detectUI.GetComponent<Canvas>();
    }

    private void Update()
    {
        if(isDetected)
        {
            Debug.Log(camOnUI.worldCamera.transform.position);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(centerObj.transform.position);
            Vector3 viewPortPos = Camera.main.WorldToViewportPoint(centerObj.transform.position);

            detectImg2.transform.position = screenPos;
            detectImg3.transform.position = viewPortPos;
        }
    }

    // 고민 :  왜 여기서 켜면 되고 타겟인디케이터에서 켜면 안되지?
    private void OnTriggerEnter(Collider other)
    {        
        if(other.CompareTag("Nella") || other.CompareTag("Steady"))
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                isDetected = true;
                //camOnUI.worldCamera = other.gameObject.GetComponent<CameraController>().FindCamera();
                //camOnUI.planeDistance = 1;
                detectUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isDetected = false;
        detectUI.SetActive(false);
    }
}
