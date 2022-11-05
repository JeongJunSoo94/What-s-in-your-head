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
        PlayerInteractionState interactionState;
        PlayerState playerState;
        public RailAction railAction;
        public RopeAction ropeAction;
        Animator animator;
        // Start is called before the first frame update
        void Awake()
        {
            interactionState = GetComponent<PlayerInteractionState>();
            playerState = GetComponent<PlayerState>();
            railAction = GetComponent<RailAction>();
            ropeAction = GetComponent<RopeAction>();
            animator = GetComponent<Animator>();
        }

        public bool InputInteract()
        {
            if (KeyManager.Instance.GetKeyDown(PlayerAction.Interaction))
            {
                if (interactionState.isRailFounded && !animator.GetBool("isMoveToRail") && !interactionState.GetWhetherHasParent())
                {
                    animator.SetFloat("moveToRailSpeed", 0.833f / railAction.StartRailAction());
                    animator.SetBool("isMoveToRail", true);
                    return true;
                }
                else
                {
                    if (ropeAction.interactableRope != null && !interactionState.isMoveFromRope && !interactionState.isRopeEscapeDelayOn)
                    {
                        animator.SetBool("isMoveToRope", ropeAction.RideRope());
                        return true;
                    }
                }
            }
            return false;
        }

        public void InitInteraction()
        {
            if (interactionState.isRidingRail)
            {
                railAction.EscapeRailAction();
            }
            else if (interactionState.isRidingRope)
            {
                ropeAction.EscapeRope();
            }
            railAction.InitDictionary();
            ropeAction.InitDictionary();
            interactionState.InitInteractionState();
        }
    }
}
