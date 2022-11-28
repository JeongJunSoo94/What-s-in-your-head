using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace YC_OBJ
{
    public class BuildingDoor : MonoBehaviour
    {
        float moveSpeed = 10f;

        bool isOpen = false;

        Vector3 targetPos;



        void Awake()
        {
            targetPos = new Vector3(transform.position.x, transform.position.y - 26f, transform.position.z);
        }

        private void FixedUpdate()
        {
            if (isOpen)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.fixedDeltaTime);
            }
        }

        public void SetOpen()
        {
            isOpen = true;
        }

        
    }
}
