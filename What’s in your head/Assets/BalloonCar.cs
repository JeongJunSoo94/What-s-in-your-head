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
            objRigidbody.velocity = Vector3.up * floatingSpeed;
            yield return new WaitForSeconds(delayTime);
            objRigidbody.velocity = Vector3.zero;
            objRigidbody.useGravity = true;
            DestroyBalloon();
        }

        void DestroyBalloon()
        {
            objRigidbody.velocity = Vector3.zero;
            Destroy(balloon);
        }
    }
}

