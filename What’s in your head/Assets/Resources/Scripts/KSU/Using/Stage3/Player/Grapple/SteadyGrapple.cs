using System.Collections;
using System.Collections.Generic;
using JCW.AudioCtrl;
using KSU.AutoAim.Object.Monster;
using UnityEngine;

namespace KSU.AutoAim.Player.Object
{
    [RequireComponent(typeof(AudioSource))]
    public class SteadyGrapple : AutoAimObject
    {
        //public GameObject spawner; //스테디 손 위치에 있는 그래플, 그래플을 던지면 손에있는 그래플이 꺼지고 이 스크립트 달린 그래플이 켜지면서 날아감
        SteadyGrappleAction playerGrappleAction; // 스테디
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
            yield return new WaitForSeconds(delayTime);
            this.gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if((other.gameObject.layer != LayerMask.NameToLayer("UITriggers")) && (other.gameObject.layer != LayerMask.NameToLayer("Player")) && (other.gameObject.layer != LayerMask.NameToLayer("Bullet")))
            {
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
                            PoisonSnake snake =  other.gameObject.GetComponent<PoisonSnake>();
                            if (snake.GetStun())
                            {
                                StartCoroutine(nameof(DelayDeactivation), snake.stunTime);
                            }
                        }
                        break;
                    case "TrippleHeadSnake":
                        {
                            playerGrappleAction.RecieveAutoAimObjectInfo(true, other.gameObject, AutoAimTargetType.Monster);
                            isSucceeded = true;
                            objectRigidbody.velocity = Vector3.zero;
                            TrippleHeadSnake snake = other.gameObject.GetComponent<TrippleHeadSnake>();
                            if (snake.GetStun())
                            {
                                StartCoroutine(nameof(DelayDeactivation), snake.stunTime);
                            }
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

