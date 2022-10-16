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

        //public float UIRadius = 100f;

        //public float rangeRadius = 5f;
        //public float rangeDistance = 5f;

        

        //public LayerMask targetLayer;
        //public Ray ray;
        //LayerMask layerFilterForRope;
        //LayerMask layerFilterForRail;
        //RaycastHit _raycastHit;
        //public Vector3 hVision;

        //public bool isRailFounded = false;
        //public bool isRailReady = false;
        //public bool isRidingRope = false;




        

        //[SerializeField] GameObject rayOrigin;
        //public GameObject minDistRope = null;
        //public GameObject hookableTarget;

        /// <summary>
        public GameObject UI;
        public GameObject detectingUI;
        public Sprite gaugeImage;
        public Sprite interactingImage;
        /// </summary>


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
            if (ITT_KeyManager.Instance.GetKeyDown(PlayerAction.Interaction))
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
            if(ITT_KeyManager.Instance.GetKeyDown(PlayerAction.Jump))
            {
                if (interactionState.isRidingRail)
                {
                    if (!interactionState.isMoveToRail)
                    {
                        railAction.EscapeRailAction();
                    }
                }
                else if (interactionState.isRidingRope)
                {
                    if (!interactionState.isMoveFromRope && !interactionState.isMoveToRope)
                    {
                        //ropeAction.EscapeRope();
                    }
                }
            }
        }
    }
}
