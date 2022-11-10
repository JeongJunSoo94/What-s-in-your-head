using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JCW.Object
{
    public class Fall : MonoBehaviour
    {
        [Header("추락 초기 속도")] [SerializeField] float fallSpeed;
        [Header("추락 가속도")] [SerializeField] float fallAccelerateSpeed;
        [Header("바로 아래의 플랫폼")] [SerializeField] Transform groundPlatform;
        //Transform groundPlatform;

        public bool isStart = false;
        Vector3 finalPos;
        void Start()
        {
            if (!groundPlatform)
            {
                Debug.Log("추락할 플랫폼이 지정되지 않았습니다.");
                Destroy(this.gameObject);
                return;
            }
            transform.GetChild(0).GetComponent<SandSackShadow>().groundPlatform = this.groundPlatform;
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

                    //현재는 추락 시 오브젝트 자체를 없애지만, 이펙트 실행하는 함수를 실행 후에 이펙트가 끝난 이후에 없애거나 SetActive(false)해야 할듯.
                    Destroy(this.gameObject);
                    break;
                default:
                    break;
            }
        }
    }

}
