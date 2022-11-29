using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.Object.Interaction
{
    public class PushablePlate : InteractableObject
    {
        public Vector3 standardVector;
        public float standardAngle = 90f;

        public int pushingNum = 0;

        bool NellaPushing = false;
        bool SteadyPushing = false;

        PlayerState NellaState;
        PlayerState SteadyState;

        bool isEnterCollision = false;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Nella"))
            {
                if (NellaState == null)
                    NellaState = collision.gameObject.GetComponent<PlayerState>();
                if (NellaState.isMine)
                {
                    isEnterCollision = true;
                }
            }

            if (collision.gameObject.CompareTag("Steady"))
            {
                if (SteadyState == null)
                    SteadyState = collision.gameObject.GetComponent<PlayerState>();
                if (SteadyState.isMine)
                {
                    isEnterCollision = true;
                }
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if(isEnterCollision)
            {
                if (collision.gameObject.CompareTag("Nella"))
                {
                    if (NellaState == null)
                        NellaState = collision.gameObject.GetComponent<PlayerState>();
                    if (NellaState.isMine)
                    {
                        //if (NellaRigidbody == null)
                        //    NellaRigidbody = collision.gameObject.GetComponent<Rigidbody>();

                        if (Vector3.Angle(NellaState.transform.forward, standardVector) < standardAngle)
                        {
                            if (!isActivated)
                            {
                                isActivated = true;
                                photonView.RPC(nameof(SetPush), RpcTarget.AllViaServer, true, true);
                                photonView.RPC(nameof(SendActivation), RpcTarget.AllViaServer);
                            }
                        }
                        else
                        {
                            if (isActivated)
                            {
                                isActivated = false;
                                photonView.RPC(nameof(SetPush), RpcTarget.AllViaServer, true, false);
                                photonView.RPC(nameof(SendActivation), RpcTarget.AllViaServer);
                            }
                        }
                    }
                }

                if (collision.gameObject.CompareTag("Steady"))
                {
                    if (SteadyState == null)
                        SteadyState = collision.gameObject.GetComponent<PlayerState>();
                    if (SteadyState.isMine)
                    {
                        //if (SteadyRigidbody == null)
                        //    SteadyRigidbody = collision.gameObject.GetComponent<Rigidbody>();

                        if (Vector3.Angle(SteadyState.transform.forward, standardVector) < standardAngle)
                        {
                            if (!isActivated)
                            {
                                isActivated = true;
                                photonView.RPC(nameof(SetPush), RpcTarget.AllViaServer, false, true);
                                photonView.RPC(nameof(SendActivation), RpcTarget.AllViaServer);
                            }
                        }
                        else
                        {
                            if (isActivated)
                            {
                                isActivated = false;
                                photonView.RPC(nameof(SetPush), RpcTarget.AllViaServer, false, false);
                                photonView.RPC(nameof(SendActivation), RpcTarget.AllViaServer);
                            }
                        }
                    }
                }
            }
        }

        [PunRPC]
        void SetPush(bool isNella, bool isPush)
        {
            if (isNella)
                NellaPushing = isPush;
            else
                SteadyPushing = isPush;

            pushingNum = 0;
            if (NellaPushing)
                pushingNum++;
            if (SteadyPushing)
                pushingNum++;
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Nella"))
            {
                if (NellaState == null)
                    NellaState = collision.gameObject.GetComponent<PlayerState>();
                if (NellaState.isMine)
                {
                    isActivated = false;
                    photonView.RPC(nameof(SetPush), RpcTarget.AllViaServer, true, false);
                    isEnterCollision = false;
                }
            }

            if (collision.gameObject.CompareTag("Steady"))
            {
                if (SteadyState == null)
                    SteadyState = collision.gameObject.GetComponent<PlayerState>();
                if (SteadyState.isMine)
                {
                    isActivated = false;
                    photonView.RPC(nameof(SetPush), RpcTarget.AllViaServer, false, false);
                    isEnterCollision = false;
                }
            }
        }

    }
}
