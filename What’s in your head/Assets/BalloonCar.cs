using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.Object.Interaction.Stage1
{
    public class BalloonCar : InteractingTargetObject
    {
        Rigidbody objRigidbody;
        [SerializeField] GameObject balloon;
        [SerializeField] float delayTime = 3f;
        [SerializeField] float floatingSpeed = 3f;
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
            yield return new WaitForSeconds(delayTime);
            objRigidbody.velocity = Vector3.zero;
            objRigidbody.useGravity = true;
            StartCoroutine(nameof(DestroyBalloon));
        }

        IEnumerator DestroyBalloon()
        {
            objRigidbody.velocity = Vector3.zero;
            Destroy(balloon);
            WaitForEndOfFrame temp = new WaitForEndOfFrame();
            while (true)
            {
                if (transform.position.y < 13.2f)
                {
                    objRigidbody.isKinematic = true;
                    yield break;
                }
                yield return temp;
            }
        }
    }
}

