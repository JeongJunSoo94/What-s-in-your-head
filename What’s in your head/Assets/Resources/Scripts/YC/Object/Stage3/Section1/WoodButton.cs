using System.Collections;
using System.Collections.Generic;
using JCW.AudioCtrl;
using UnityEngine;
using Photon.Pun;
/// <summary> 
/// 
/// Current Issue       : ���� ������ (�ִϸ��̼Ǳ���) / �÷��̾� ��Ʈ�ѷ� ���� �̽�
/// 
/// Object Name         : ���� ��ư
/// 
/// Object Description  : �ش� ������Ʈ�� �ٴڿ� �ִ� ��ư�̴�. �÷��̾ ��ư ���� �ö󰥽� ��ư�� ��������(�ִϸ��̼�), ��ư�� �������� ���� ���Թ��� ������.
///                         ��ư���� �������� �Ǹ� �ٽ� ��ư�� �ö󰡰�, ���Թ��� ������ �ȴ�
/// 
/// Script Description  : �� ������Ʈ�� ���� ��ȯ�� Trigger ��� SendMessage ���
///                         ��ư Ư���� ������ ������ �� �ƴ϶� ������ ������ �۵��ϴ� ����̴�. ���� ��ư ���� Ʈ���� �ݶ��̴��� ����� üũ�Ѵ�
///                         
/// Player Intraction   : �÷��̾�
/// 
/// </summary>


namespace YC_OBJ
{
    public class WoodButton : MonoBehaviour
    {
        [Header("Case : �Ϲ� ��ư�� ��")]
        [SerializeField] GameObject doorObj1;
        [SerializeField] GameObject doorObj2;

        DoorController doorController1;
        DoorController doorController2;
        int Count = 0; // ���� ��ư ���� ����� �ö�� �ִ��� ����.
        Animator animator;
        
        [Space] [Space]

        [Header("Case : �̷��� ��ư�� ��")]
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

            if (isInMaze) // << : 2���� (�̷� �ȿ� �ִ� ��)
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
            else // << : 1���� (2�������� ���� ���Թ�)
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
