using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YC.Effect
{
    public class FootDustEffect : MonoBehaviour
    {
        [SerializeField] GameObject particleObj;
        PlayerState playerState;


        private void Awake()
        {
            playerState = transform.root.gameObject.GetComponent<PlayerState>();

        }

        private void FixedUpdate()
        {
            //if (Ckeck())
            //    PlayEffect();
            //else
            //    StopEffect();
        }


        public void PlayEffect()
        {
            particleObj.SetActive(true);
        }

        public void StopEffect()
        {
            particleObj.SetActive(false);
        }

        //bool Ckeck()
        //{
        //    if ((playerState == playerState.isMove || playerState == playerState.isRun && playerState == playerState.IsGrounded)
        //        && (playerState != playerState.IsJumping && playerState != playerState.IsDashing))
        //    {
        //        return true;
        //    }
        //    else
        //        return false;
        //}
       
    }
}
