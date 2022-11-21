using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    [Header("开林青老 锭 true"), SerializeField] bool isReverseRun = false;
    [SerializeField] float trainSpeed = 12f;
    [SerializeField] float delayTime = 3f;
    [SerializeField] GameObject train;
    float destPos = -1f;
    bool isEnd = false;
    Cinemachine.CinemachineSmoothPath track;
    Cinemachine.CinemachineDollyCart trainCart;
    PhotonView photonView;

    bool isFuncStart = false;

    void Awake()
    {
        track = GetComponentInParent<Cinemachine.CinemachineSmoothPath>();
        trainCart = GetComponent<Cinemachine.CinemachineDollyCart>();
        trainCart.m_Path = track;
        photonView = PhotonView.Get(this);
        StartCoroutine(nameof(WaitForPlayer));
    }

    void Update()
    {
        if (!isFuncStart)
            return;

        if(photonView.IsMine)
        {
            CheckEnd();
        }
    }

    void InitTrain()
    {
        isEnd = false;
        trainCart.m_Speed = 0f;
        if(isReverseRun)
        {
            trainCart.m_Position = track.m_Waypoints.Length - 1.005f;
            if (destPos <= 0)
                destPos = 0.005f;
        }
        else
        {
            trainCart.m_Position = 0.005f;
            train.transform.localRotation = Quaternion.Euler(Vector3.up * 180f);
            if (destPos <= 0)
                destPos = track.m_Waypoints.Length - 1.005f;
        }
    }

    void ActivateTrain()
    {
        train.SetActive(true);
        trainCart.m_Speed = isReverseRun ? -trainSpeed : trainSpeed;
    }

    void CheckEnd()
    {
        if (isReverseRun)
        {
            if ((trainCart.m_Position < destPos) && !isEnd)
            {
                photonView.RPC(nameof(StartDelay),RpcTarget.AllViaServer);
                //StartCoroutine(nameof(DelayReset));
            }
        }
        else
        {
            if ((trainCart.m_Position > destPos) && !isEnd)
            {
                photonView.RPC(nameof(StartDelay), RpcTarget.AllViaServer);
                //StartCoroutine(nameof(DelayReset));
            }
        }
    }

    [PunRPC]
    void StartDelay()
    {
        StartCoroutine(nameof(DelayReset));
    }

    IEnumerator DelayReset()
    {
        isEnd = true;
        train.SetActive(false);
        InitTrain();
        yield return new WaitForSeconds(delayTime);
        ActivateTrain();
    }

    IEnumerator WaitForPlayer()
    {
        yield return new WaitUntil(() => GameManager.Instance.GetCharOnScene(true) && GameManager.Instance.GetCharOnScene(false));

        InitTrain();
        ActivateTrain();

        isFuncStart = true;

    }
}
