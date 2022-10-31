using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// 
/// Current Issue       : 
/// 
/// Object Name         : 나무 출입문
/// 
/// Object Description  : 나무 버튼이 눌리면 해당 오브젝테에게 SendMessage를 보내 출입문이 열리게 된다
/// 
/// Script Description  : 
///                         
/// Player Intraction   : 
/// 
/// </summary>

namespace YC_OBJ
{
    public class DoorController : MonoBehaviour
    {
        [Header("<기획 편집 사항>")]

        [Header("[애니메이션 속도 (0 ~ 1.5 추천)]")]
        [SerializeField] float animationSpeed = 0.3f;

        [Header("[문이 열리기 전 딜레이]")]
        [SerializeField] float openDelay = 0.5f;

        [Header("[문이 닫히기 전 딜레이]")]
        [SerializeField] float closeDelay = 1.0f;

        Animator animator;

        private void Awake()
        {
            animator = this.gameObject.GetComponent<Animator>();

            animator.speed = animationSpeed;
        }

        public void SetOpen(bool _isOpen)
        {
          
            StartCoroutine(SetAnimation(_isOpen));
        }

        IEnumerator SetAnimation(bool isOpen)
        {
            if(isOpen)
            {
                yield return new WaitForSeconds(openDelay);
                animator.SetBool("isOpen", isOpen);
            }
            else
            {
                yield return new WaitForSeconds(closeDelay);
                animator.SetBool("isOpen", isOpen);
            }
        }
      
    }
}