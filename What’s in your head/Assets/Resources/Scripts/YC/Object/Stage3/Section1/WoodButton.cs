using System.Collections;
using System.Collections.Generic;
using JCW.AudioCtrl;
using UnityEngine;

/// <summary> 
/// 
/// Current Issue       : 에셋 제작중 (애니메이션까지) / 플레이어 컨트롤러 관련 이슈
/// 
/// Object Name         : 나무 버튼
/// 
/// Object Description  : 해당 오브젝트는 바닥에 있는 버튼이다. 플레이어가 버튼 위로 올라갈시 버튼이 눌러지며(애니메이션), 버튼이 눌러지면 나무 출입문이 열린다.
///                         버튼에서 내려오게 되면 다시 버튼은 올라가고, 출입문은 닫히게 된다
/// 
/// Script Description  : 두 오브젝트간 정보 교환은 Trigger 기반 SendMessage 사용
///                         버튼 특성상 옆에서 누르는 게 아니라 위에서 눌러야 작동하는 방식이다. 따라서 버튼 위에 트리거 콜라이더를 만들어 체크한다
///                         
/// Player Intraction   : 플레이어
/// 
/// </summary>


namespace YC_OBJ
{
    public class WoodButton : MonoBehaviour
    {
        [Header("Case : 일반 버튼과 문")]
        [SerializeField] GameObject doorObj;

        DoorController doorController;
        int Count = 0; // 현재 버튼 위에 몇명이 올라와 있는지 센다.
        Animator animator;
        
        [Space] [Space]

        [Header("Case : 미로의 버튼과 문")]
        [SerializeField] bool isInMaze = false;
        [SerializeField] GameObject MazeDoor;

        void Start()
        {
            doorController = doorObj.GetComponent<DoorController>();
            animator = this.gameObject.GetComponent<Animator>();
        }

        public void SetAnimation(int _count)
        {
            Count = _count;

            if (isInMaze)
            {
                
            }
            else
            {
                if (Count > 0)
                {
                    doorController.SendMessage(nameof(doorController.SetOpen), true);

                    animator.SetBool("isUp", false);
                    animator.SetBool("isDown", true);
                }
                else
                if (Count == 0)
                {
                    doorController.SendMessage(nameof(doorController.SetOpen), false);

                    animator.SetBool("isUp", true);
                    animator.SetBool("isDown", false);
                }
            }          
        }



    }
}
