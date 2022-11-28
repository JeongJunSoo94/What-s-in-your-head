using System.Collections;
using System.Collections.Generic;
using JCW.AudioCtrl;
using UnityEngine;
using Photon.Pun;
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
        [SerializeField] GameObject doorObj1;
        [SerializeField] GameObject doorObj2;

        DoorController doorController1;
        DoorController doorController2;
        int Count = 0; // 현재 버튼 위에 몇명이 올라와 있는지 센다.
        Animator animator;
        
        [Space] [Space]

        [Header("Case : 미로의 버튼과 문")]
        [SerializeField] bool isInMaze = false;
        [SerializeField] GameObject MazeDoor;
        MazeDoorController mazeDoorController;

        PhotonView pv;
        AudioSource audioSource;


        private void Awake()
        {
            pv = this.gameObject.GetComponent<PhotonView>();
            audioSource = this.gameObject.GetComponent<AudioSource>();

        }
        void Start()
        {
            if (!isInMaze)
            {
                doorController1 = doorObj1.GetComponent<DoorController>();
                doorController2 = doorObj2.GetComponent<DoorController>();
            }
            if(isInMaze)
                mazeDoorController = MazeDoor.GetComponent<MazeDoorController>();

            animator = this.gameObject.GetComponent<Animator>();

            SoundManager.Set3DAudio(pv.ViewID, audioSource, 2f, 10f, false);
        }

        public void SetAnimation(int _count)
        {
            Count = _count;

            if (isInMaze) // << : 2섹션 (미로 안에 있는 문)
            {
                if (Count > 0)
                {
                    StartCoroutine(nameof(PlayEffectSound), 0.3f);

                    mazeDoorController.SendMessage(nameof(mazeDoorController.ControlDoor), true);
                    animator.SetBool("isUp", false);
                    animator.SetBool("isDown", true);

                }
                else if (Count == 0)
                {
                    mazeDoorController.SendMessage(nameof(mazeDoorController.ControlDoor), false);
                    animator.SetBool("isUp", false);
                    animator.SetBool("isUp", true);
                    animator.SetBool("isDown", false);
                }
            }
            else // << : 1섹션 (2섹션으로 들어가는 출입문)
            {

                if (Count > 0)
                {
                    StartCoroutine(nameof(PlayEffectSound), 0.3f);

                    doorController1.SendMessage(nameof(doorController1.SetOpen), true);
                    doorController2.SendMessage(nameof(doorController2.SetOpen), true);

                    animator.SetBool("isUp", false);
                    animator.SetBool("isDown", true);
                  
                }
                else if (Count == 0)
                {
                    doorController1.SendMessage(nameof(doorController1.SetOpen), false);
                    doorController2.SendMessage(nameof(doorController2.SetOpen), false);


                    animator.SetBool("isUp", true);
                    animator.SetBool("isDown", false);
                }
            }          
        }

        IEnumerator PlayEffectSound(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            SoundManager.Instance.Play3D_RPC("All_ButtonTurnOn", pv.ViewID);
        }


    }
}
