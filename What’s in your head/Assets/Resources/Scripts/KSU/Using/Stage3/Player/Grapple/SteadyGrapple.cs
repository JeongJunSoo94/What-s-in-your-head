using System.Collections;
using System.Collections.Generic;
using JCW.AudioCtrl;
using UnityEngine;

namespace KSU
{
    [RequireComponent(typeof(AudioSource))]
    public class SteadyGrapple : MonoBehaviour
    {
        public SteadyGrappleAction player; // 스테디
        public GameObject spawner; //스테디 손 위치에 있는 그래플, 그래플을 던지면 손에있는 그래플이 꺼지고 이 스크립트 달린 그래플이 켜지면서 날아감
        LineRenderer grappleRope;
        Vector3 endPosistion;

        float departingOffset = 0.2f;
        public bool isEndPosition = false;
        public bool isSucceeded = false;

        float moveSpeed = 15f;
        float airDrag = 10f;
        Rigidbody grappleRigidbody;
        public float horizonInertiaSpeed;
        public float gravity = 20f;
        public Vector3 horizonInertiaVec;
        public Vector3 fallingVelocity;

        public float delayDeactivationTime = 1f;

        AudioSource audioSource;

        // Start is called before the first frame update
        void Awake()
        {
            grappleRigidbody = GetComponent<Rigidbody>();
            grappleRope = GetComponent<LineRenderer>();
            audioSource = GetComponent<AudioSource>();
            JCW.AudioCtrl.AudioSettings.SetAudio(audioSource, 1f, 50f);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if(!isSucceeded)
            {
                if (isEndPosition)
                {
                    FallToGround();

                }
                else
                {
                    MoveToEndPosition();
                }
            }
            MakeRope();
        }

        public void InitGrapple(Vector3 startPos, Vector3 endPos, float grappleSpeed, float Offset)
        {
            grappleRigidbody.velocity = Vector3.zero;
            transform.position = startPos;
            endPosistion = endPos;
            isEndPosition = false;
            isSucceeded = false;
            moveSpeed = grappleSpeed;
            departingOffset = Offset;
            this.gameObject.SetActive(true);
        }

        void MoveToEndPosition()
        {
            //1.리지드바디 추가해서 벨로시티로움직이기
            if (Vector3.Distance(endPosistion, transform.position) < departingOffset)
            {
                isEndPosition = true;
                horizonInertiaVec = grappleRigidbody.velocity;
                horizonInertiaVec.y = 0;
                horizonInertiaSpeed = horizonInertiaVec.magnitude;
                StartCoroutine(nameof(DelayDeactivation));
            }
            else
            {
                isEndPosition = false;
            }
            transform.LookAt(endPosistion);
            Vector3 dir = (endPosistion - transform.position).normalized;
            grappleRigidbody.velocity = dir * moveSpeed;
        }

        void FallToGround()
        {
            horizonInertiaSpeed -= moveSpeed * airDrag * Time.fixedDeltaTime;
            if (horizonInertiaSpeed < 0)
                horizonInertiaSpeed = 0f;
            fallingVelocity = horizonInertiaVec * horizonInertiaSpeed;
            fallingVelocity.y = grappleRigidbody.velocity.y + gravity * Time.fixedDeltaTime; 
        }

        void MakeRope()
        {
            grappleRope.SetPosition(0, spawner.transform.position);
            grappleRope.SetPosition(1, transform.position);
        }

        IEnumerator DelayDeactivation()
        {
            if (GameManager.Instance.isTopView)
            {
                yield return new WaitForSeconds(0f);
            }
            else
            {
                yield return new WaitForSeconds(delayDeactivationTime);

            }
            if (this.gameObject.activeSelf)
            {
                player.RecieveGrappleInfo(false, null, GrappleTargetType.Null);
            }
            this.gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if((other.gameObject.layer != LayerMask.NameToLayer("UITriggers")) && (other.gameObject.layer != LayerMask.NameToLayer("Player")) && (other.gameObject.layer != LayerMask.NameToLayer("Bullet")))
            {
                switch(other.tag)
                {
                    case "GrappledObject":
                        {
                            SoundManager.Instance.Play3D_RPC("GrappleSound", audioSource);
                            player.RecieveGrappleInfo(true, other.gameObject, GrappleTargetType.GrappledObject);
                            isSucceeded = true;
                            grappleRigidbody.velocity = Vector3.zero;
                        }
                        break;
                    case "PoisonSnake":
                        {
                            player.RecieveGrappleInfo(true, other.gameObject, GrappleTargetType.Monster);
                            isSucceeded = true;
                            grappleRigidbody.velocity = Vector3.zero;
                        }
                        break;
                    case "TrippleHeadSnake":
                        {
                            player.RecieveGrappleInfo(true, other.gameObject, GrappleTargetType.Monster);
                            isSucceeded = true;
                            grappleRigidbody.velocity = Vector3.zero;
                        }
                        break;
                    default:
                        {
                            player.RecieveGrappleInfo(false, null, GrappleTargetType.Null);
                            this.gameObject.SetActive(false);
                        }
                        break;
                }
            }
        }
    }
}

