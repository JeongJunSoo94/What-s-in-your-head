using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPhotonTransformView : MonoBehaviourPun, IPunObservable
{
    private bool m_firstTake = true;

    private Vector3 m_Direction;
    private Vector3 m_NetworkPosition;
    private Vector3 m_StoredPosition;
    private Vector3 m_RigidbodyVelocity;
    private Vector3 m_RigidbodyAngularVelocity;

    private Quaternion m_NetworkRotation;

    public bool m_SynchronizePosition = false;
    public bool m_SynchronizeRotation = false;
    public bool m_SynchronizeScale = false;
    public bool m_SynchronizeRigidbody = false;

    public float lerpPosition = 30f;
    public float lerpRotation = 30f;

    Rigidbody m_Rigidbody;

    // Start is called before the first frame update
    void Awake()
    {
        m_StoredPosition = transform.localPosition;
        m_NetworkPosition = Vector3.zero;
        m_NetworkRotation = Quaternion.identity;

        if(m_SynchronizeRigidbody)
            m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!this.photonView.IsMine)
        {
            if (this.m_SynchronizePosition)
            {
                transform.position = Vector3.Lerp(transform.position, m_NetworkPosition, Time.fixedDeltaTime * lerpPosition);
            }

            if (this.m_SynchronizeRotation)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, m_NetworkRotation, Time.fixedDeltaTime * lerpRotation);
            }

            if (this.m_SynchronizeRigidbody)
            {
                m_Rigidbody.velocity = m_RigidbodyVelocity;
                m_Rigidbody.velocity = m_RigidbodyAngularVelocity;
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        var tr = transform;

        // Write
        if (stream.IsWriting)
        {
            if (this.m_SynchronizePosition)
            {
                this.m_Direction = tr.position - this.m_StoredPosition;
                this.m_StoredPosition = tr.position;
                stream.SendNext(tr.position);
                stream.SendNext(this.m_Direction);
            }

            if (this.m_SynchronizeRotation)
            {
                stream.SendNext(tr.rotation);
            }

            if(this.m_SynchronizeRigidbody)
            {
                stream.SendNext(m_Rigidbody.velocity);
                stream.SendNext(m_Rigidbody.angularVelocity);
            }
        }
        // Read
        else
        {
            if (this.m_SynchronizePosition)
            {
                this.m_NetworkPosition = (Vector3)stream.ReceiveNext();
                this.m_Direction = (Vector3)stream.ReceiveNext();

                if (m_firstTake)
                {
                    tr.position = this.m_NetworkPosition;
                }
                else
                {
                    float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                    this.m_NetworkPosition += this.m_Direction * lag;
                }

            }

            if (this.m_SynchronizeRotation)
            {
                this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();
            }

            if (this.m_SynchronizeRigidbody)
            {
                m_RigidbodyVelocity = (Vector3)stream.ReceiveNext();
                m_RigidbodyAngularVelocity = (Vector3)stream.ReceiveNext();
            }

            if (m_firstTake)
            {
                m_firstTake = false;
            }
        }
    }
}
