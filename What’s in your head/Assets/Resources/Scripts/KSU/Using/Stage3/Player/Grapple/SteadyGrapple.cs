using System.Collections;
using System.Collections.Generic;
using JCW.AudioCtrl;
using UnityEngine;

namespace KSU.AutoAim.Player.Object
{
    [RequireComponent(typeof(AudioSource))]
    public class SteadyGrapple : AutoAimObject
    {
        //public GameObject spawner; //���׵� �� ��ġ�� �ִ� �׷���, �׷����� ������ �տ��ִ� �׷����� ������ �� ��ũ��Ʈ �޸� �׷����� �����鼭 ���ư�
        SteadyGrappleAction playerGrappleAction; // ���׵�
        LineRenderer grappleRope;
        //float moveSpeed = 15f;
        //Vector3 endPosistion;
        //float departingOffset = 0.2f;
        //public bool isEndPosition = false;
        //public bool isSucceeded = false;

        //Rigidbody grappleRigidbody;
        //AudioSource audioSource;

        // Start is called before the first frame update
        void Awake()
        {
            objectRigidbody = GetComponent<Rigidbody>();
            grappleRope = GetComponent<LineRenderer>();
            audioSource = GetComponent<AudioSource>();
            JCW.AudioCtrl.AudioSettings.SetAudio(audioSource, 1f, 50f);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            MakeRope();
            if (!isSucceeded)
            {
                MoveToEndPosition();
            }
        }

        public override void InitObject(Vector3 startPos, Vector3 endPos, float objectSpeed, float offset)
        {
            objectRigidbody.velocity = Vector3.zero;
            transform.position = startPos;
            endPosistion = endPos;
            isEndPosition = false;
            isSucceeded = false;
            moveSpeed = objectSpeed;
            departingOffset = offset;
            MakeRope();
            this.gameObject.SetActive(true);
        }

        //public void InitGrapple(Vector3 startPos, Vector3 endPos, float grappleSpeed, float Offset)
        //{
        //    grappleRigidbody.velocity = Vector3.zero;
        //    transform.position = startPos;
        //    endPosistion = endPos;
        //    isEndPosition = false;
        //    isSucceeded = false;
        //    moveSpeed = grappleSpeed;
        //    departingOffset = Offset;
        //    MakeRope();
        //    this.gameObject.SetActive(true);
        //}

        //void MoveToEndPosition()
        //{
        //    //1.������ٵ� �߰��ؼ� ���ν�Ƽ�ο����̱�
        //    if (Vector3.Distance(endPosistion, transform.position) < departingOffset)
        //    {
        //        isEndPosition = true;
        //        if (this.gameObject.activeSelf)
        //        {
        //            player.RecieveGrappleInfo(false, null, GrappleTargetType.Null);
        //        }
        //        this.gameObject.SetActive(false);
        //    }
        //    else
        //    {
        //        isEndPosition = false;
        //    }
        //    transform.LookAt(endPosistion);
        //    Vector3 dir = (endPosistion - transform.position).normalized;
        //    objectRigidbody.velocity = dir * moveSpeed;
        //}

        void MakeRope()
        {
            grappleRope.SetPosition(0, spawner.transform.position);
            grappleRope.SetPosition(1, transform.position);
        }

        protected void MoveToEndPosition()
        {
            //1.������ٵ� �߰��ؼ� ���ν�Ƽ�ο����̱�
            if (Vector3.Distance(endPosistion, transform.position) < departingOffset)
            {
                isEndPosition = true;
                if (this.gameObject.activeSelf)
                {
                    playerGrappleAction.RecieveAutoAimObjectInfo(false, null, AutoAimTargetType.Null);
                }
                this.gameObject.SetActive(false);
            }
            else
            {
                isEndPosition = false;
            }
            transform.LookAt(endPosistion);
            Vector3 dir = (endPosistion - transform.position).normalized;
            objectRigidbody.velocity = dir * moveSpeed;
        }

        private void OnTriggerEnter(Collider other)
        {
            if((other.gameObject.layer != LayerMask.NameToLayer("UITriggers")) && (other.gameObject.layer != LayerMask.NameToLayer("Player")) && (other.gameObject.layer != LayerMask.NameToLayer("Bullet")))
            {
                if (playerGrappleAction == null)
                    playerGrappleAction = player.GetComponent<SteadyGrappleAction>();

                    switch (other.tag)
                {
                    case "GrappledObject":
                        {
                            SoundManager.Instance.Play3D_RPC("GrappleSound", audioSource);
                            playerGrappleAction.RecieveAutoAimObjectInfo(true, other.gameObject, AutoAimTargetType.GrappledObject);
                            isSucceeded = true;
                            objectRigidbody.velocity = Vector3.zero;
                        }
                        break;
                    case "PoisonSnake":
                        {
                            playerGrappleAction.RecieveAutoAimObjectInfo(true, other.gameObject, AutoAimTargetType.Monster);
                            isSucceeded = true;
                            objectRigidbody.velocity = Vector3.zero;
                        }
                        break;
                    case "TrippleHeadSnake":
                        {
                            playerGrappleAction.RecieveAutoAimObjectInfo(true, other.gameObject, AutoAimTargetType.Monster);
                            isSucceeded = true;
                            objectRigidbody.velocity = Vector3.zero;
                        }
                        break;
                    default:
                        {
                            playerGrappleAction.RecieveAutoAimObjectInfo(false, null, AutoAimTargetType.Null);
                            this.gameObject.SetActive(false);
                        }
                        break;
                }
            }
        }
    }
}

