using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KSU.AutoAim.Player
{
    public class AutoAimObject : MonoBehaviour
    {
        public GameObject player;
        public GameObject spawner;
        protected Rigidbody objectRigidbody;
        protected AudioSource audioSource;

        protected float moveSpeed = 15f;
        protected Vector3 endPosistion;
        protected float departingOffset = 0.2f;
        public bool isEndPosition = false;
        public bool isSucceeded = false;

        public virtual void InitObject(Vector3 startPos, Vector3 endPos, float objectSpeed, float offset)
        {
            objectRigidbody.velocity = Vector3.zero;
            transform.position = startPos;
            endPosistion = endPos;
            isEndPosition = false;
            isSucceeded = false;
            moveSpeed = objectSpeed;
            departingOffset = offset;
            this.gameObject.SetActive(true);
        }
    }
}
