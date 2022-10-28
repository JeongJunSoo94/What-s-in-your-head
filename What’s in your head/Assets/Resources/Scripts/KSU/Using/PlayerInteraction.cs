using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using YC.Camera_;
using YC.Camera_Single;
using JCW.UI.Options.InputBindings;
namespace KSU
{
    public class PlayerInteraction : MonoBehaviour
    {
        //public Dictionary<GameObject, GameObject> detectedRopeSpawners = new Dictionary<GameObject, GameObject>();
        //public Dictionary<GameObject, GameObject> interactableRopeSpawners = new Dictionary<GameObject, GameObject>();

        //Camera mainCamera;
        //PlayerController playerController;
        //PlayerState playerState;
        PlayerInteractionState interactionState;
        PlayerState playerState;
        //HookingRope grappleAction;
        RailAction railAction;
        RopeAction ropeAction;
        Animator animator;
        // Start is called before the first frame update
        void Awake()
        {
            //playerController = GetComponent<PlayerController>();
            //playerState = GetComponent<PlayerState>();
            interactionState = GetComponent<PlayerInteractionState>();
            playerState = GetComponent<PlayerState>();
            //grappleAction = GetComponent<HookingRope>();
            railAction = GetComponent<RailAction>();
            ropeAction = GetComponent<RopeAction>();
            animator = GetComponent<Animator>();

            //ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        }

        // Update is called once per frame
        //void Update()
        //{
        //    //if (playerState.isMine)
        //    //    InputInteract();
        //}

        public void InputInteract()
        {
            if (KeyManager.Instance.GetKeyDown(PlayerAction.Interaction))
            {
                if (interactionState.isRailFounded)
                {
                    animator.SetFloat("moveToRailSpeed", 0.833f / railAction.StartRailAction());
                    animator.SetBool("isMoveToRail",true);
                    railAction.StartRailAction();
                }
                else
                {
                    if (ropeAction.interactableRope != null)
                    {
                        animator.SetBool("isMoveToRope", true);
                        ropeAction.RideRope();
                    }
                }
            }
            //if (KeyManager.Instance.GetKeyDown(PlayerAction.Jump))
            //{
            //    if (interactionState.isRidingRail)
            //    {
            //        if (!interactionState.isMovingToRail && !interactionState.isRailJumping)
            //        {
            //            railAction.StartRailJump();
            //        }
            //    }
            //    else if (interactionState.isRidingRope)
            //    {
            //        if (!interactionState.isMoveFromRope && !interactionState.isMoveToRope)
            //        {
            //            ropeAction.EscapeRope();
            //        }
            //    }
            //}
        }
    }
}
