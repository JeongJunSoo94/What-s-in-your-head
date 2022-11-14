using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU
{
    public class MovingPlatform : MonoBehaviour
    {

        public Vector3 startPosition = new Vector3(-3, 0.5f, -3);
        public Vector3 endPosition = new Vector3(-3, 5.5f, -3);

        public float moveSpeed = 2f;

        bool isMovingToEnd = true;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            MoveRepeatly();
        }

        void MoveRepeatly()
        {
            if(isMovingToEnd)
            {
                if(Vector3.Distance(endPosition,transform.position) < 0.1f)
                {
                    isMovingToEnd = false;
                    return;
                }

                transform.position = Vector3.MoveTowards(transform.position, endPosition, moveSpeed * Time.fixedDeltaTime);
            }
            else
            {
                if (Vector3.Distance(startPosition, transform.position) < 0.1f)
                {
                    isMovingToEnd = true;
                    return;
                }
                transform.position = Vector3.MoveTowards(transform.position, startPosition, moveSpeed * Time.fixedDeltaTime);
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            if(collision.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                collision.gameObject.transform.parent = this.gameObject.transform;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                collision.gameObject.transform.parent = null;
            }
        }
    }
}
