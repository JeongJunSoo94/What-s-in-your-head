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

    [Tooltip("Indicates if localPosition and localRotation should be used. Scale ignores this setting, and always uses localScale to avoid issues with lossyScale.")]
    public bool m_UseLocal = false;

    bool m_firstTake = false;
    bool m_ownerWorldFirstTake = false;
    bool m_ownerLocalFirstTake = false;
    public bool ownerHasParent = false;
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
        m_ownerWorldFirstTake = true;
        m_ownerLocalFirstTake = true;
    }

    public void Update()
    {
        if(photonView.IsMine)
        {
            if (ownerHasParent)
            {
                if (transform.parent == null)
                {
                    this.photonView.RPC(nameof(SendHasParent), RpcTarget.AllViaServer, false, true);
                }
            }
            else
            {
                if (transform.parent != null)
                {
                    this.photonView.RPC(nameof(SendHasParent), RpcTarget.AllViaServer, true, true);
                }
            }
        }
        else
        {
            if (remoteHasParent)
            {
                if (transform.parent == null)
                {
                    this.photonView.RPC(nameof(SendHasParent), RpcTarget.AllViaServer, false, false);
                }
            }
            else
            {
                if (transform.parent != null)
                {
                    this.photonView.RPC(nameof(SendHasParent), RpcTarget.AllViaServer, true, false);
                }
            }
        }
    }

    public void FixedUpdate()
    {
        if (!this.photonView.IsMine)
        {
            if (m_UseLocal)
            {
                //if (m_Direction.magnitude < Mathf.Epsilon)
                //{
                //    transform.localPosition = this.m_NetworkPosition;
                //}
                //else
                {
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, this.m_NetworkPosition, this.m_Distance * Time.fixedDeltaTime * PhotonNetwork.SerializationRate);
                }
                //transform.localRotation = Quaternion.RotateTowards(transform.localRotation, this.m_NetworkRotation, this.m_Angle * Time.fixedDeltaTime * PhotonNetwork.SerializationRate);
            }
            else
            {
                //if (m_Direction.magnitude < Mathf.Epsilon)
                //{
                //    transform.position = this.m_NetworkPosition;
                //}
                //else
                {
                    transform.position = Vector3.MoveTowards(transform.position, this.m_NetworkPosition, this.m_Distance * Time.fixedDeltaTime * PhotonNetwork.SerializationRate);
                }
                
                //transform.rotation = Quaternion.RotateTowards(transform.rotation, this.m_NetworkRotation, this.m_Angle * Time.fixedDeltaTime * PhotonNetwork.SerializationRate);
            }
            transform.rotation = Quaternion.RotateTowards(transform.rotation, this.m_NetworkRotation, this.m_Angle * Time.fixedDeltaTime * PhotonNetwork.SerializationRate);
        }
    }

    [PunRPC]
    void SendHasParent(bool hasP, bool isMine)
    {
        if (isMine)
            ownerHasParent = hasP;
        else
            remoteHasParent = hasP;
    }

    [PunRPC]
    void SendUseLocal(bool useL)
    {
        m_UseLocal = useL;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (!photonView.IsMine)
                return;
            if (m_UseLocal)
            {
                if (!ownerHasParent || !remoteHasParent)
                    photonView.RPC(nameof(SendUseLocal), RpcTarget.AllViaServer, false);
            }
            else
            {
                if (ownerHasParent && remoteHasParent)
                    photonView.RPC(nameof(SendUseLocal), RpcTarget.AllViaServer, true);
            }


            if (m_UseLocal)
            {
                if(m_ownerLocalFirstTake)
                {
                    m_ownerWorldFirstTake = true;
                    m_ownerLocalFirstTake = false;
                    this.m_Direction = Vector3.zero;
                    this.m_StoredPosition = transform.localPosition;
                    stream.SendNext(transform.localPosition);
                    stream.SendNext(this.m_Direction);
                }
                else
                {
                    this.m_Direction = transform.localPosition - this.m_StoredPosition;
                    this.m_StoredPosition = transform.localPosition;
                    stream.SendNext(transform.localPosition);
                    stream.SendNext(this.m_Direction);
                }
            }
            else
            {
                if (m_ownerWorldFirstTake)
                {
                    m_ownerLocalFirstTake = true;
                    m_ownerWorldFirstTake = false;
                    this.m_Direction = Vector3.zero;
                    this.m_StoredPosition = transform.position;
                    stream.SendNext(transform.position);
                    stream.SendNext(this.m_Direction);
                }
                else
                {
                    this.m_Direction = transform.position - this.m_StoredPosition;
                    this.m_StoredPosition = transform.position;
                    stream.SendNext(transform.position);
                    stream.SendNext(this.m_Direction);
                }
            }

            //if (m_UseLocal)
            //{
            //    stream.SendNext(transform.localRotation);
            //}
            //else
            //{
            //    stream.SendNext(transform.rotation);
            //}
            stream.SendNext(transform.rotation);
        }
        else
        {
            this.m_NetworkPosition = (Vector3)stream.ReceiveNext();
            this.m_Direction = (Vector3)stream.ReceiveNext();

            if (m_firstTake)
            {
                if (m_UseLocal)
                    transform.localPosition = this.m_NetworkPosition;
                else
                    transform.position = this.m_NetworkPosition;

                this.m_Distance = 0f;
            }
            else
            {
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                this.m_NetworkPosition += this.m_Direction * lag;
                if (m_UseLocal)
                {
                    this.m_Distance = Vector3.Distance(transform.localPosition, this.m_NetworkPosition);
                }
                else
                {
                    this.m_Distance = Vector3.Distance(transform.position, this.m_NetworkPosition);
                }
            }

            this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();

            if (m_firstTake)
            {
                this.m_Angle = 0f;

                transform.rotation = this.m_NetworkRotation;
            }
            else
            {
                this.m_Angle = Quaternion.Angle(transform.rotation, this.m_NetworkRotation);
            }

            if (m_firstTake)
            {
                m_firstTake = false;
            }
        }
    }
}
