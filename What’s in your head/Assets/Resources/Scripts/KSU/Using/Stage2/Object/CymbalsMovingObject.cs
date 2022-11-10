using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.Object.Stage2
{
    public class CymbalsMovingObject : LinkedObjectWithReciever
    {
        [SerializeField] Vector3 startPosition;
        [SerializeField] Vector3 endPosition;
        public float departingOffset = 0.5f;
        public float moveSpeed = 6f;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            MovePosition();
        }

        void MovePosition()
        {
            if (isActivated)
            {
                if (Vector3.Distance(transform.position, endPosition) < departingOffset)
                    return;
                transform.position = Vector3.MoveTowards(transform.position, endPosition, moveSpeed * Time.fixedDeltaTime);
            }
            else
            {
                if (Vector3.Distance(transform.position, startPosition) < departingOffset)
                    return;
                transform.position = Vector3.MoveTowards(transform.position, startPosition, moveSpeed * Time.fixedDeltaTime);
            }
        }
    }
}
