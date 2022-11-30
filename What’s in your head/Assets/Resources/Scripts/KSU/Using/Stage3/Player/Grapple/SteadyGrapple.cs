using System.Collections;
using System.Collections.Generic;
using JCW.AudioCtrl;
using KSU.AutoAim.Object.Monster;
using Photon.Pun;
using UnityEngine;

namespace KSU.AutoAim.Player.Object
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(PhotonView))]
    public class SteadyGrapple : AutoAimObject
    {
        //public GameObject spawner; //스테디 손 위치에 있는 그래플, 그래플을 던지면 손에있는 그래플이 꺼지고 이 스크립트 달린 그래플이 켜지면서 날아감
        SteadyGrappleAction playerGrappleAction; // 스테디
        LineRenderer grappleRope;

        bool isGrab = false;

        PhotonView pv;

        WaitForSeconds ws;


        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            grappleRope = GetComponent<LineRenderer>();
            audioSource = GetComponent<AudioSource>();
            pv = GetComponent<PhotonView>();
            SoundManager.Set3DAudio(pv.ViewID, audioSource, 1f, 50f);
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
            if (playerGrappleAction == null)
                playerGrappleAction = player.GetComponent<SteadyGrappleAction>();
            objectRigidbody.velocity = Vector3.zero;
            transform.position = startPos;
            endPosistion = endPos;
            isEndPosition = false;
            isSucceeded = false;
            moveSpeed = objectSpeed;
            departingOffset = offset;
            MakeRope();
            isGrab = false;
            this.gameObject.SetActive(true);
        }

        void MakeRope()
        {
            grappleRope.SetPosition(0, spawner.transform.position);
            grappleRope.SetPosition(1, transform.position);
        }

        protected void MoveToEndPosition()
        {
            //1.리지드바디 추가해서 벨로시티로움직이기
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

        IEnumerator DelayDeactivation(float delayTime)
        {
            if (ws == null)
                ws = new WaitForSeconds(delayTime);
            yield return ws;
            Debug.Log("그래플 꺼주기");
            this.gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((other.gameObject.layer != LayerMask.NameToLayer("UITriggers"))
                && (other.gameObject.layer != LayerMask.NameToLayer("Player"))
                && (other.gameObject.layer != LayerMask.NameToLayer("Bullet")))
            {
                switch (other.tag)
                {
                    case "GrappledObject":
                        {
                            playerGrappleAction.RecieveAutoAimObjectInfo(true, other.gameObject, AutoAimTargetType.GrappledObject);
                            isSucceeded = true;
                            objectRigidbody.velocity = Vector3.zero;
                        }
                        break;
                    case "PoisonSnake":
                        {
                            if (!isGrab)
                            {
                                PoisonSnake snake = other.gameObject.GetComponent<PoisonSnake>();
                                if (snake.GetStun())
                                {
                                    playerGrappleAction.RecieveAutoAimObjectInfo(true, other.gameObject, AutoAimTargetType.Monster);
                                    isSucceeded = true;
                                    objectRigidbody.velocity = Vector3.zero;
                                    StartCoroutine(nameof(DelayDeactivation), snake.stunTime);
                                }
                                else
                                {
                                    playerGrappleAction.RecieveAutoAimObjectInfo(false, null, AutoAimTargetType.Null);
                                    this.gameObject.SetActive(false);
                                }
                            }
                        }
                        break;
                    case "TrippleHeadSnake":
                        {
                            if (!isGrab)
                            {
                                TrippleHeadSnake snake = other.gameObject.GetComponent<TrippleHeadSnake>();
                                if (snake.GetStun())
                                {
                                    playerGrappleAction.RecieveAutoAimObjectInfo(true, other.gameObject, AutoAimTargetType.Monster);
                                    isSucceeded = true;
                                    objectRigidbody.velocity = Vector3.zero;
                                    snake.GetStun();
                                    StartCoroutine(nameof(DelayDeactivation), snake.stunTime);
                                }
                                else
                                {
                                    playerGrappleAction.RecieveAutoAimObjectInfo(false, null, AutoAimTargetType.Null);
                                    this.gameObject.SetActive(false);
                                }
                            }
                        }
                        break;
                    default:
                        {
                            if (!isGrab)
                            {
                                playerGrappleAction.RecieveAutoAimObjectInfo(false, null, AutoAimTargetType.Null);
                                this.gameObject.SetActive(false);
                            }
                        }
                        break;
                }
            }
        }
    }
}

