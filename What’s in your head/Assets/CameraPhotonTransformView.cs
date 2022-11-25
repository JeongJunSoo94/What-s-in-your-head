using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPhotonTransformView : MonoBehaviourPun, IPunObservable
{
    private Vector3 m_NetworkPosition;

    private Quaternion m_NetworkRotation;

    public float lerpPosition = 30f;
    public float lerpRotation = 30f;

    // Start is called before the first frame update
    void Awake()
    {
        m_NetworkPosition = Vector3.zero;
        m_NetworkRotation = Quaternion.identity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!this.photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, m_NetworkPosition, Time.fixedDeltaTime * lerpPosition);
            transform.rotation = Quaternion.Lerp(transform.rotation, m_NetworkRotation, Time.fixedDeltaTime * lerpRotation);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            this.m_NetworkPosition = (Vector3)stream.ReceiveNext();
            this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
