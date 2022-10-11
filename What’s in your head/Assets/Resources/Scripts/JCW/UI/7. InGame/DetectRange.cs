using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectRange : MonoBehaviour
{
    [Header("감지 범위 수치")] [SerializeField] [Range(0, 100)] float range;
    [Header("UI")] [SerializeField] GameObject detectUI;

    private void Awake()
    {
        transform.localScale = new Vector3(range, range, range);
    }


    private void OnTriggerEnter(Collider other)
    {
        detectUI.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        detectUI.SetActive(false);
    }
}
