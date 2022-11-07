using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary> 
/// 
/// Current Issue       : ���� ���� ������ ���� �ʿ�
/// 
/// Object Name         : ȭ�� �ɺ�����
/// 
/// Object Description  : �����ð����� �ڶ��� ������ �°� �ȴٸ�, ���� �ǰ�, ���׵��� ���� ��ų�� ��ȣ�ۿ��� �����ϴ�.
/// 
/// Script Description  :
///                         
/// Player Intraction   : �ڶ�(���� ��ų)
/// 
/// </summary>
/// 

namespace YC_OBJ
{
    public class PotFlower : MonoBehaviour
    {
        Animator animator;

        [SerializeField] GameObject GrappleObj;

        private void Awake()
        {      
            animator = this.gameObject.GetComponent<Animator>();
        }

        public void SetBloom()
        {
            animator.SetBool("isBoom", true);
            GrappleObj.SetActive(true);
        }    
    }
}
