using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary> 
/// 
/// Current Issue       : 제작 에셋 포지션 수정 필요
/// 
/// Object Name         : 화분 꽃봉오리
/// 
/// Object Description  : 일정시간동안 넬라의 물총을 맞게 된다면, 꽃이 피고, 스테디의 갈고리 스킬과 상호작용이 가능하다.
/// 
/// Script Description  :
///                         
/// Player Intraction   : 넬라(물총 스킬)
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
