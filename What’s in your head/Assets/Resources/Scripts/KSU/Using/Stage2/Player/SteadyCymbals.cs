using JCW.AudioCtrl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.AutoAim.Player.Object
{
    [RequireComponent(typeof(AudioSource))]
    public class SteadyCymbals : AutoAimObject
    {
        //public GameObject spawner; //���׵� �� ��ġ�� �ִ� �׷���, �׷����� ������ �տ��ִ� �׷����� ������ �� ��ũ��Ʈ �޸� �׷����� �����鼭 ���ư�
        SteadyCymbalsAction playerCymbalsAction; // ���׵�
        [SerializeField] ParticleEffect particle;
        //float moveSpeed = 15f;
        //Vector3 endPosistion;
        //float departingOffset = 0.2f;
        //public bool isEndPosition = false;
        //public bool isSucceeded = false;

        //Rigidbody grappleRigidbody;
        AudioSource audioSource;

        int audioID = 0;

        protected override void Awake()
        {
            base.Awake();
            audioSource = GetComponent<AudioSource>();
            audioID = JCW.AudioCtrl.AudioSettings.SetAudio(audioSource, 1f, 60f);
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
            //1.������ٵ� �߰��ؼ� ���ν�Ƽ�ο����̱�
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
                particle.gameObject.SetActive(false);
                this.gameObject.SetActive(false);
            }

            transform.LookAt(spawner.transform.position);
            Vector3 dir = (spawner.transform.position - transform.position).normalized;
            objectRigidbody.velocity = dir * moveSpeed;
        }

        void MakeParticle()
        {
            if (!isEndPosition)
                isEndPosition = true;
            particle.particlePosition = transform.position;
            particle.gameObject.SetActive(true);
            PlayCymbalsHitSound();
        }

        void PlayCymbalsHitSound()
        {
            SoundManager.Instance.Play3D_RPC("S2_Steady_Cymbals_Hit", audioID);
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((other.gameObject.layer != LayerMask.NameToLayer("UITriggers")) && (other.gameObject.layer != LayerMask.NameToLayer("Player")) && (other.gameObject.layer != LayerMask.NameToLayer("Bullet")))
            {
                switch (other.tag)
                {
                    case "CymbalsTargetObject":
                        {
                            MakeParticle();
                            objectRigidbody.velocity = Vector3.zero;
                        }
                        break;
                    case "Rope":
                        break;
                    default:
                        {
                            MakeParticle();
                        }
                        break;
                }
            }
        }
    }
}

