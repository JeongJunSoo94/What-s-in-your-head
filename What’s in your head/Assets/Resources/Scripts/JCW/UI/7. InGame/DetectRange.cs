using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YC.Camera_;
using YC.Camera_Single;

public class DetectRange : MonoBehaviour
{
    [Header("���� ���� ��ġ")] [SerializeField] [Range(0, 100)] float range;
    [Header("UI")] [SerializeField] GameObject detectUI;
    [Header("�⺻ �̹���")] [SerializeField] GameObject detectImg;
    [Header("����� �̹���")] [SerializeField] GameObject detectImg2;
    [Header("����Ʈ �̹���")] [SerializeField] GameObject detectImg3;
    [Header("������Ʈ")] [SerializeField] GameObject centerObj;

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

    // ��� :  �� ���⼭ �Ѹ� �ǰ� Ÿ���ε������Ϳ��� �Ѹ� �ȵ���?
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
