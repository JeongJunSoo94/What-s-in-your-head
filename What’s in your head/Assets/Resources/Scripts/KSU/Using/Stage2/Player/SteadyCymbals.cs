using JCW.AudioCtrl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.AutoAim.Player.Object
{
    [RequireComponent(typeof(AudioSource))]
    public class SteadyCymbals : AutoAimObject
    {
        //public GameObject spawner; //스테디 손 위치에 있는 그래플, 그래플을 던지면 손에있는 그래플이 꺼지고 이 스크립트 달린 그래플이 켜지면서 날아감
        SteadyCymbalsAction playerCymbalsAction; // 스테디
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
            audioSource = GetComponent<AudioSource>();
            JCW.AudioCtrl.AudioSettings.SetAudio(audioSource, 1f, 50f);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if(spawner != null)
            {
                if (!isEndPosition)
                {
                    MoveToEndPosition();
                }
                else
                {
                    MoveToPlayer();
                }
            }
        }

        public override void InitObject(Vector3 startPos, Vector3 endPos, float objectSpeed, float offset)
        {
            if (playerCymbalsAction == null)
                playerCymbalsAction = player.GetComponent<SteadyCymbalsAction>();
            objectRigidbody.velocity = Vector3.zero;
            transform.position = startPos;
            endPosistion = endPos;
            isEndPosition = false;
            isSucceeded = false;
            moveSpeed = objectSpeed;
            departingOffset = offset;
            transform.LookAt(endPosistion);
            this.gameObject.SetActive(true);
        }

        protected void MoveToEndPosition()
        {
            //1.리지드바디 추가해서 벨로시티로움직이기
            if (Vector3.Distance(endPosistion, transform.position) < departingOffset)
            {
                isEndPosition = true;
            }
            transform.LookAt(endPosistion);
            Vector3 dir = (endPosistion - transform.position).normalized;
            objectRigidbody.velocity = dir * moveSpeed;
        }

        void MoveToPlayer()
        {
            if (Vector3.Distance(spawner.transform.position, transform.position) < departingOffset)
            {
                isEndPosition = false;
                playerCymbalsAction.RecieveAutoAimObjectInfo(false, null, AutoAimTargetType.Null);
                this.gameObject.SetActive(false);
            }

            transform.LookAt(spawner.transform.position);
            Vector3 dir = (spawner.transform.position - transform.position).normalized;
            objectRigidbody.velocity = dir * moveSpeed;
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((other.gameObject.layer != LayerMask.NameToLayer("UITriggers")) && (other.gameObject.layer != LayerMask.NameToLayer("Player")) && (other.gameObject.layer != LayerMask.NameToLayer("Bullet")))
            {
                switch (other.tag)
                {
                    case "CymbalsTargetObject":
                        {
                            if (!isEndPosition)
                                isEndPosition = true;
                            objectRigidbody.velocity = Vector3.zero;
                        }
                        break;
                    case "Rope":
                        break;
                    default:
                        {
                            if (!isEndPosition)
                                isEndPosition = true;
                        }
                        break;
                }
            }
        }
    }
}

