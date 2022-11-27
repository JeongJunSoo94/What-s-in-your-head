using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPhotonTransformView : MonoBehaviourPun, IPunObservable
{
    private Vector3 m_Direction;
    private Vector3 m_NetworkPosition;
    private Vector3 m_StoredPosition;

    private Quaternion m_NetworkRotation;

    private bool m_firstTake = true;

    public float lerpPosition = 30f;
    public float lerpRotation = 30f;

    // Start is called before the first frame update
    public void Awake()
    {
        m_StoredPosition = transform.position;
        m_NetworkPosition = Vector3.zero;
        m_NetworkRotation = Quaternion.identity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!this.photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, m_NetworkPosition, Time.fixedDeltaTime * lerpPosition * PhotonNetwork.SerializationRate);
            transform.rotation = Quaternion.Lerp(transform.rotation, m_NetworkRotation, Time.fixedDeltaTime * lerpRotation * PhotonNetwork.SerializationRate);
        }
    }

    //void OnEnable()
    //{
    //    m_firstTake = true;
    //}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            this.m_Direction = transform.position - this.m_StoredPosition;
            this.m_StoredPosition = transform.position;
            stream.SendNext(transform.position);
            stream.SendNext(this.m_Direction);
            stream.SendNext(transform.rotation);
        }
        else
        {
            this.m_NetworkPosition = (Vector3)stream.ReceiveNext();
            this.m_Direction = (Vector3)stream.ReceiveNext();
            this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            this.m_NetworkPosition += this.m_Direction * lag;
        }
    }
}
