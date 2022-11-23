using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPhotonTransformView : MonoBehaviourPun, IPunObservable
{
    private float m_Distance;
    private float m_Angle;

    private Vector3 m_Direction;
    private Vector3 m_NetworkPosition;
    private Vector3 m_StoredPosition;

    private Quaternion m_NetworkRotation;

    public bool m_SynchronizePosition = true;
    public bool m_SynchronizeRotation = true;
    public bool m_SynchronizeScale = false;

    [Tooltip("Indicates if localPosition and localRotation should be used. Scale ignores this setting, and always uses localScale to avoid issues with lossyScale.")]
    public bool m_UseLocal;

    bool m_firstTake = false;
    bool ownerHasParent = false;
    bool remoteHasParent = false;

    public void Awake()
    {
        m_StoredPosition = transform.localPosition;
        m_NetworkPosition = Vector3.zero;
        m_NetworkRotation = Quaternion.identity;
    }

    void OnEnable()
    {
        m_firstTake = true;
    }

    public void FixedUpdate()
    {
        var tr = transform;

        if (!this.photonView.IsMine)
        {
            if (m_UseLocal)

            {
                tr.localPosition = Vector3.MoveTowards(tr.localPosition, this.m_NetworkPosition, this.m_Distance * Time.fixedDeltaTime * PhotonNetwork.SerializationRate);
                tr.localRotation = Quaternion.RotateTowards(tr.localRotation, this.m_NetworkRotation, this.m_Angle * Time.fixedDeltaTime * PhotonNetwork.SerializationRate);
            }
            else
            {
                tr.position = Vector3.MoveTowards(tr.position, this.m_NetworkPosition, this.m_Distance * Time.fixedDeltaTime * PhotonNetwork.SerializationRate);
                tr.rotation = Quaternion.RotateTowards(tr.rotation, this.m_NetworkRotation, this.m_Angle * Time.fixedDeltaTime * PhotonNetwork.SerializationRate);
            }

            if (remoteHasParent)
            {
                if (transform.parent == null)
                {
                    remoteHasParent = false;
                    this.photonView.RPC(nameof(SendHasParent), RpcTarget.AllViaServer, remoteHasParent, photonView.IsMine);
                }
            }
            else
            {
                if (transform.parent != null)
                {
                    remoteHasParent = true;
                    this.photonView.RPC(nameof(SendHasParent), RpcTarget.AllViaServer, remoteHasParent, photonView.IsMine);
                }
            }
        }
        else
        {
            if (ownerHasParent)
            {
                if (transform.parent == null)
                {
                    ownerHasParent = false;
                    this.photonView.RPC(nameof(SendHasParent), RpcTarget.AllViaServer, ownerHasParent, photonView.IsMine);
                }
            }
            else
            {
                if (transform.parent != null)
                {
                    ownerHasParent = true;
                    this.photonView.RPC(nameof(SendHasParent), RpcTarget.AllViaServer, ownerHasParent, photonView.IsMine);
                }
            }
        }
    }

    [PunRPC]
    void SendHasParent(bool hasP, bool isMine)
    {
        if (isMine)
            ownerHasParent = hasP;
        else
            remoteHasParent = hasP;

        if (ownerHasParent && remoteHasParent)
            m_UseLocal = true;
        else
            m_UseLocal = false;
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        var tr = transform;

        // Write
        if (stream.IsWriting)
        {
            if (this.m_SynchronizePosition)
            {
                if (m_UseLocal)
                {
                    this.m_Direction = tr.localPosition - this.m_StoredPosition;
                    this.m_StoredPosition = tr.localPosition;
                    stream.SendNext(tr.localPosition);
                    stream.SendNext(this.m_Direction);
                }
                else
                {
                    this.m_Direction = tr.position - this.m_StoredPosition;
                    this.m_StoredPosition = tr.position;
                    stream.SendNext(tr.position);
                    stream.SendNext(this.m_Direction);
                }
            }

            if (this.m_SynchronizeRotation)
            {
                if (m_UseLocal)
                {
                    stream.SendNext(tr.localRotation);
                }
                else
                {
                    stream.SendNext(tr.rotation);
                }
            }

            if (this.m_SynchronizeScale)
            {
                stream.SendNext(tr.localScale);
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
                    if (m_UseLocal)
                        tr.localPosition = this.m_NetworkPosition;
                    else
                        tr.position = this.m_NetworkPosition;

                    this.m_Distance = 0f;
                }
                else
                {
                    float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                    this.m_NetworkPosition += this.m_Direction * lag;
                    if (m_UseLocal)
                    {
                        this.m_Distance = Vector3.Distance(tr.localPosition, this.m_NetworkPosition);
                    }
                    else
                    {
                        this.m_Distance = Vector3.Distance(tr.position, this.m_NetworkPosition);
                    }
                }

            }

            if (this.m_SynchronizeRotation)
            {
                this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();

                if (m_firstTake)
                {
                    this.m_Angle = 0f;

                    if (m_UseLocal)
                    {
                        tr.localRotation = this.m_NetworkRotation;
                    }
                    else
                    {
                        tr.rotation = this.m_NetworkRotation;
                    }
                }
                else
                {
                    if (m_UseLocal)
                    {
                        this.m_Angle = Quaternion.Angle(tr.localRotation, this.m_NetworkRotation);
                    }
                    else
                    {
                        this.m_Angle = Quaternion.Angle(tr.rotation, this.m_NetworkRotation);
                    }
                }
            }

            if (this.m_SynchronizeScale)
            {
                tr.localScale = (Vector3)stream.ReceiveNext();
            }

            if (m_firstTake)
            {
                m_firstTake = false;
            }
        }
    }
}
