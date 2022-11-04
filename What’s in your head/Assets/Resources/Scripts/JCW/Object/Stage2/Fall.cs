using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JCW.Object
{
    public class Fall : MonoBehaviour
    {
        [Header("추락 초기 속도")] [SerializeField] float fallSpeed;
        [Header("추락 가속도")] [SerializeField] float fallAccelerateSpeed;
        Transform groundPlatform;

        public bool isStart = false;
        Vector3 finalPos;
        void Start()
        {
            groundPlatform = transform.GetChild(0).GetComponent<SandSackShadow>().groundPlatform;
            finalPos = transform.position;
            finalPos.y = groundPlatform.position.y;
        }

        void Update()
        {
            if (!isStart)
                return;
            transform.position = Vector3.MoveTowards(transform.position, finalPos, Time.deltaTime * fallSpeed);
            fallSpeed += Time.deltaTime * fallAccelerateSpeed;
        }


        private void OnTriggerEnter(Collider other)
        {
            switch(other.tag)
            {
                case "Nella":
                case "Steady":
                    Debug.Log("모래주머니 캐릭터 깔고 뭉갬");
                    break;
                case "Platform":
                    Debug.Log("모래주머니 땅에 추락");
                    isStart = false;
                    Destroy(this.gameObject);
                    break;
                default:
                    break;
            }
        }
    }

}
