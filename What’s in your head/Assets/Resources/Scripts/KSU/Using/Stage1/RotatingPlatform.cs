using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class RotatingPlatform : MonoBehaviour
{
    PhotonView photonView;
    Vector3 curRot;
    [SerializeField] float rotatingSpeed = 360f;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        curRot = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (photonView.IsMine)
            RotatePropel();
    }

    void RotatePropel()
    {
        curRot.y += rotatingSpeed * Time.fixedDeltaTime;
        if (curRot.y > 360f)
            curRot.y -= 360f;
        transform.rotation = Quaternion.Euler(curRot);
    }
}
