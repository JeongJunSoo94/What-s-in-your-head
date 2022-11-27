using JCW.AudioCtrl;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.Object.Interaction.Stage1
{
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(AudioSource))]
    public class BalloonCar : InteractingTargetObject
    {
        Rigidbody objRigidbody;
        [SerializeField] GameObject balloon;
        [SerializeField] float delayTime = 3f;
        [SerializeField] float floatingSpeed = 3f;

        AudioSource audioSource;
        PhotonView pv;

        WaitForSeconds ws;

        private void Awake()
        {
            pv = GetComponent<PhotonView>();
            audioSource = GetComponent<AudioSource>();
            ws = new(delayTime);

            SoundManager.Set3DAudio(pv.ViewID, audioSource, 1f, 50f);
        }

        // Start is called before the first frame update
        void Start()
        {
            objRigidbody = GetComponent<Rigidbody>();
            objRigidbody.useGravity = false;
            objRigidbody.isKinematic = true;
        }

        public override void RecieveActivation(bool isActive)
        {
            if(!isActivated && isActive)
            {
                isActivated = isActive;
                StartCoroutine(nameof(StartActivation));
            }
        }

        IEnumerator StartActivation()
        {
            objRigidbody.isKinematic = false;
            objRigidbody.velocity = Vector3.up * floatingSpeed;
            yield return ws;
            objRigidbody.useGravity = true;
            StartCoroutine(nameof(DestroyBalloon));
        }

        IEnumerator DestroyBalloon()
        {
            objRigidbody.velocity = Vector3.zero;        
            Destroy(balloon);
            SoundManager.Instance.Play3D_RPC("S1S1_BalloonPop", pv.ViewID);
            //WaitForEndOfFrame temp = new WaitForEndOfFrame();

            yield return new WaitUntil(() => transform.position.y < 13.2f);
            objRigidbody.isKinematic = true;
            yield break;

            //while (true)
            //{
            //    if (transform.position.y < 13.2f)
            //    {
            //        objRigidbody.isKinematic = true;
            //        yield break;
            //    }
            //    yield return temp;
            //}
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.CompareTag("Platform"))
            {
                SoundManager.Set3DAudio(pv.ViewID, audioSource, 0.2f, 50f);
                SoundManager.Instance.Play3D_RPC("S1S1_CarDrop", pv.ViewID);
            }
        }
    }
}

