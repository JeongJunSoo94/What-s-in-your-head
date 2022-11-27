using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using JCW.AudioCtrl;

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
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(PhotonView))]
    public class DoorController : MonoBehaviour
    {
        [Header("<기획 편집 사항>")]

        [Header("[애니메이션 속도 (0 ~ 1.5 추천)]")]
        [SerializeField] float animationSpeed = 0.3f;

        Animator animator;
        AudioSource audioSource;
        PhotonView pv;
        
        private void Awake()
        {
            animator = this.gameObject.GetComponent<Animator>();

            animator.speed = animationSpeed;
            pv = GetComponent<PhotonView>();

            audioSource = GetComponent<AudioSource>();
            SoundManager.Set3DAudio(pv.ViewID, audioSource, 3f, 50f);
        }

        public void SetOpen(bool _isOpen)
        {
            animator.SetBool("isOpen", _isOpen);
            StartCoroutine(nameof(SoundEffect));
        }     

        IEnumerator SoundEffect()
        {
            yield return new WaitForSeconds(1.1f);
            SoundManager.Instance.Play3D_RPC("S3S1_DoorOpen2", pv.ViewID);
        }
    }
}