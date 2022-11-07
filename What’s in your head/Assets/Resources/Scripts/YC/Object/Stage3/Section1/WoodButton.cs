using System.Collections;
using System.Collections.Generic;
using JCW.AudioCtrl;
using UnityEngine;

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
        [SerializeField] GameObject doorObj;

        DoorController doorController;
        int Count = 0; // ���� ��ư ���� ����� �ö�� �ִ��� ����.
        Animator animator;
        
        [Space] [Space]

        [Header("Case : �̷��� ��ư�� ��")]
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
