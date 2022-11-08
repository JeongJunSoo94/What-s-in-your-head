using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    [Header("开林青老 锭 true"), SerializeField] bool isReverseRun = false;
    [SerializeField] float trainSpeed = 12f;
    [SerializeField] float departingOffset = 0.2f;
    [SerializeField] float delayTime = 3f;
    [SerializeField] GameObject train;
    float destPos = -1f;
    bool isEnd = false;
    Cinemachine.CinemachineSmoothPath track;
    Cinemachine.CinemachineDollyCart trainCart;
    PhotonView photonView;
    // Start is called before the first frame update
    void Awake()
    {
        track = GetComponentInParent<Cinemachine.CinemachineSmoothPath>();
        trainCart = GetComponent<Cinemachine.CinemachineDollyCart>();
        trainCart.m_Path = track;
        photonView = PhotonView.Get(this);
        InitTrain();
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine)
        {
            CheckEnd();
        }
    }

    void InitTrain()
    {
        isEnd = false;
        if(isReverseRun)
        {
            trainCart.m_Speed = -trainSpeed;
            trainCart.m_Position = track.m_Waypoints.Length - 1.005f;
            if (destPos <= 0)
                destPos = 0.005f;
        }
        else
        {
            trainCart.m_Speed = trainSpeed;
            trainCart.m_Position = 0.005f;
            if (destPos <= 0)
                destPos = track.m_Waypoints.Length - 1.005f;
        }
        train.SetActive(true); 
    }

    void CheckEnd()
    {
        if (isReverseRun)
        {
            if ((trainCart.m_Position < destPos) && !isEnd)
            {
                StartCoroutine(nameof(DelayReset));
            }
        }
        else
        {
            if ((trainCart.m_Position > destPos) && !isEnd)
            {
                StartCoroutine(nameof(DelayReset));
            }
        }
    }

    IEnumerator DelayReset()
    {
        isEnd = true;
        train.SetActive(false);
        yield return new WaitForSeconds(delayTime);
        InitTrain();
    }
}
