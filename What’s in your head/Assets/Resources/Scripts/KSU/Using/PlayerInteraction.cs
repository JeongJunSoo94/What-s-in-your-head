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
        //PlayerController3D playerController;
        //CharacterState3D playerState;
        PlayerInteractionState interactionState;

        //HookingRope grappleAction;
        RailAction railAction;
        RopeAction ropeAction;

        // Start is called before the first frame update
        void Awake()
        {
            //playerController = GetComponent<PlayerController3D>();
            //playerState = GetComponent<CharacterState3D>();
            interactionState = GetComponent<PlayerInteractionState>();
            
            //grappleAction = GetComponent<HookingRope>();
            railAction = GetComponent<RailAction>();
            ropeAction = GetComponent<RopeAction>();

            //ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        }

        // Update is called once per frame
        void Update()
        {
            InputInteract();
        }

        void InputInteract()
        {
            if (KeyManager.Instance.GetKeyDown(PlayerAction.Interaction))
            {
                if (!interactionState.isRidingRope && !interactionState.isRidingRail)
                {
                    if (interactionState.isRailFounded)
                    {
                        railAction.StartRailAction();
                    }
                    else
                    {
                        if (ropeAction.interactableRope != null)
                        {
                            ropeAction.RideRope();
                        }
                    }
                }
                else
                {
                    if (interactionState.isRidingRail)
                    {
                        if (!interactionState.isMoveToRail)
                        {
                            //railAction.EscapeRailAction();
                        }
                    }

                    if (interactionState.isRidingRope)
                    {
                        if (!interactionState.isMoveFromRope && !interactionState.isMoveToRope)
                        {
                            ropeAction.EscapeRope();
                        }
                    }
                }
            }
            if(KeyManager.Instance.GetKeyDown(PlayerAction.Jump))
            {
                if (interactionState.isRidingRail)
                {
                    if (!interactionState.isMoveToRail)
                    {
                        //railAction.EscapeRailAction();
                    }
                }
                else if (interactionState.isRidingRope)
                {
                    if (!interactionState.isMoveFromRope && !interactionState.isMoveToRope)
                    {
                        ropeAction.EscapeRope();
                    }
                }
            }
        }
    }
}
