using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using YC.Camera_;
using YC.Camera_Single;

public class PlayerInteraction : MonoBehaviour
{
    public Dictionary<GameObject, GameObject> detectedRopeSpawners = new Dictionary<GameObject, GameObject>();
    public Dictionary<GameObject, GameObject> interactableRopeSpawners = new Dictionary<GameObject, GameObject>();

    Camera mainCamera;
    PlayerController3D playerController;
    PlayerInteractionState interactionState;

    public float UIRadius = 100f;

    public float rangeRadius = 5f;
    public float rangeDistance = 5f;

    public float escapingRopeSpeed = 6f;
    public float escapingRoDelayTime = 1f;

    public LayerMask targetLayer;
    public Ray ray;
    LayerMask layerFilter;
    RaycastHit _raycastHit;
    public Vector3 hVision;

    public bool isRailFounded = false;
    public bool isRailReady = false;
    public bool isRidingRope = false;



    Vector3 right;

    Vector3 startCenter;
    Vector3 startUp;
    Vector3 startDown;
    Vector3 startLeft;
    Vector3 startRight;

    Vector3 endCenter;
    Vector3 endUp;
    Vector3 endDown;
    Vector3 endLeft;
    Vector3 endRight;

    public GameObject minDistObj = null;


    /// <summary>
    public GameObject UI;
    public GameObject detectingUI;
    public Sprite gaugeImage;
    public Sprite interactingImage;
    /// </summary>


    // Start is called before the first frame update
    void Awake()
    {
        if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Joined)
            mainCamera = this.gameObject.GetComponent<CameraController>().FindCamera(); // 멀티용
        else
            mainCamera = this.gameObject.GetComponent<CameraController_Single>().FindCamera(); // 싱글용

        if (mainCamera == null)
            Debug.Log("카메라 NULL");
        playerController = GetComponent<PlayerController3D>();
        interactionState = GetComponent<PlayerInteractionState>();
        layerFilter = ((-1) - (1 << LayerMask.NameToLayer("Player")) - (1 << LayerMask.NameToLayer("Water")));
        //ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    }

    // Update is called once per frame
    void Update()
    {
        //SearchRail();
        DetectObjects();
        InputInteract();
        //RideOnOffRope();
        RenderInteractingUI();
        //UIRender();
    }

    private void FixedUpdate()
    {
        //RideOnOffRope();
        //DetectObjects();
        //RenderInteractingUI();
    }

    void SearchRail()
    {
        MakeGizmoVecs();
        SearchWithSphereCast();
    }

    void MakeGizmoVecs()
    {
        right = mainCamera.transform.right;
    
        hVision = mainCamera.transform.forward;
    
        startCenter = mainCamera.transform.position;
        startCenter.y = 0;
        startUp = startCenter + mainCamera.transform.up * rangeRadius;
        startDown = startCenter - mainCamera.transform.up * rangeRadius;
        startLeft = startCenter - right * rangeRadius;
        startRight = startCenter + right * rangeRadius;
    
        endCenter = startCenter + hVision * rangeDistance;
        endUp = endCenter + Vector3.up * rangeRadius;
        endDown = endCenter - Vector3.up * rangeRadius;
        endLeft = endCenter - right * rangeRadius;
        endRight = endCenter + right * rangeRadius;
    }
    
    void InputInteract()
    {
        if(JCW.UI.Options.InputBindings.ITT_KeyManager.Instance.GetKeyDown(JCW.UI.Options.InputBindings.PlayerAction.Interaction))
        {
            if(!interactionState.isRidingRope && !interactionState.isRidingRail)
            {
                if (interactionState.isRailReady)
                {

                }
                else
                {
                    if (minDistObj != null)
                    {
                        interactionState.isRidingRope = true;
                        interactionState.isMoveToRope = true;
                        GetComponent<Rigidbody>().velocity = Vector3.zero;
                        minDistObj.GetComponentInChildren<RopeSpawner>().StartRopeAction(this.gameObject);
                    }
                }
            }
            else
            {
                if (interactionState.isRidingRail)
                {
                    if (interactionState.isMoveToRail)
                    {

                    }
                }

                if (interactionState.isRidingRope)
                {
                    if (!interactionState.isMoveFromRope && !interactionState.isMoveToRope)
                    {
                        StartCoroutine("DelayEscape");
                        float jumpPower = 1f;
                        jumpPower = minDistObj.GetComponentInChildren<RopeSpawner>().EndRopeAction(this.gameObject);

                        Vector3 inertiaVec = transform.forward;
                        inertiaVec.y = 0;

                        transform.LookAt(transform.position + inertiaVec);
                        playerController.MakeinertiaVec(escapingRopeSpeed, inertiaVec.normalized);
                        playerController.moveVec = Vector3.up * playerController.jumpSpeed * jumpPower;
                        playerController.enabled = true;
                    }
                }
            }

            
        }
    }


    IEnumerator DelayEscape()
    {
        interactionState.isMoveFromRope = true;
        yield return new WaitForSeconds(1f);
        interactionState.isMoveFromRope = false;
        interactionState.isRidingRope = false;
    }
    
    public GameObject SearchWithSphereCast()
    {
        ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    
        isRailFounded = Physics.SphereCast(ray.origin, rangeRadius, ray.direction, out _raycastHit, rangeDistance, targetLayer, QueryTriggerInteraction.Ignore);
    
        //Debug.Log(_raycastHit.point);
        //Debug.Log(m_Camera.transform.position - hVision * Physics.defaultContactOffset);
        if (isRailFounded)
        {
            Debug.Log("origin: " + ray.origin + ", distance: " + Vector3.Distance(ray.origin, _raycastHit.point));
        }
        //if(isFound)
        //{
        //    if(tag == _raycastHit.collider.tag)
        //    {
        //        return _raycastHit.collider.gameObject;
        //    }
        //}
    
        return null;
    }

    void DetectObjects()
    {
        float halfUIRadius = UIRadius / 2f;
        float minDist = 500f;
        //minDistObj = null;

        if(detectedRopeSpawners.Count > 0)
        {
            foreach (var interactableObj in detectedRopeSpawners)
            {
                RaycastHit hit;
                GameObject key = interactableObj.Key;
                GameObject value = interactableObj.Value;
                RopeSpawner spawner = key.GetComponentInChildren<RopeSpawner>();

                Physics.Raycast(transform.position, (key.transform.position - transform.position), out hit, spawner.detectingRange * 1.5f, layerFilter, QueryTriggerInteraction.Ignore);

                if (key.gameObject != hit.transform.gameObject)
                {
                    value.SetActive(false);
                }
                else
                {
                    value.SetActive(true);
                    float amount = 1f - (hit.distance - spawner.interactableRange) / (spawner.detectingRange - spawner.interactableRange);
                    if (!isRailReady && !interactionState.isRidingRope)
                    {
                        if (hit.distance < spawner.interactableRange)
                        {
                            if (minDist > hit.distance)
                            {
                                minDist = hit.distance;
                                if(minDistObj != null)
                                {
                                    detectedRopeSpawners.GetValueOrDefault(minDistObj).GetComponentsInChildren<Image>()[1].sprite = gaugeImage;
                                }
                                minDistObj = key;
                            }
                        }
                    }

                    if (amount > 1f)
                    {
                        amount = 1f;
                    }

                    value.GetComponentsInChildren<Image>()[1].fillAmount = amount;

                    Vector3 screenPos = mainCamera.WorldToScreenPoint(key.transform.position);

                    float yScreenMin = halfUIRadius;
                    float yScreenMax = Screen.height - halfUIRadius;
                    float xScreenMin = halfUIRadius;
                    float xScreenMax = Screen.width - halfUIRadius;

                    switch(this.gameObject.tag)
                    {
                        case "Nella":
                            {
                                xScreenMax = Screen.width * mainCamera.rect.width - halfUIRadius;
                            }
                            break;
                        case "Steady":
                            {
                                xScreenMin = halfUIRadius + Screen.width* mainCamera.rect.width;
                            }
                            break;
                    }

                    screenPos.y = ConfineInRange(screenPos.y, yScreenMin, yScreenMax);

                    if (screenPos.z < 0)
                    {
                        screenPos.y = halfUIRadius;
                        screenPos.x = -screenPos.x;
                    }

                    screenPos.x = ConfineInRange(screenPos.x, xScreenMin, xScreenMax);
                    RectTransform[] rects = value.GetComponentsInChildren<RectTransform>();

                    if (screenPos.x == xScreenMin || screenPos.y == yScreenMin || screenPos.x == xScreenMax || screenPos.y == yScreenMax)
                    {
                        float resizedHalfUIRaidus = halfUIRadius * 0.8f;
                        for(int i =0; i<rects.Length;++i)
                        {
                            rects[i].sizeDelta = new Vector2(resizedHalfUIRaidus, resizedHalfUIRaidus) * 2f;
                            if (i != 0)
                            {
                                rects[i].localPosition = Vector3.zero;
                            }
                        }

                        yScreenMin = resizedHalfUIRaidus;
                        yScreenMax = Screen.height - resizedHalfUIRaidus;

                        switch (this.gameObject.tag)
                        {
                            case "Nella":
                                {
                                    xScreenMax = Screen.width * mainCamera.rect.width - resizedHalfUIRaidus;
                                }
                                break;
                            case "Steady":
                                {
                                    xScreenMin = resizedHalfUIRaidus + Screen.width * mainCamera.rect.width;
                                }
                                break;
                        }

                        screenPos.x = ConfineInRange(screenPos.x, xScreenMin, xScreenMax);
                        screenPos.y = ConfineInRange(screenPos.y, yScreenMin, yScreenMax);
                    }
                    else
                    {
                        for (int i = 0; i < rects.Length; ++i)
                        {
                            rects[i].sizeDelta = new Vector2(UIRadius, UIRadius);
                            if(i != 0)
                            {
                                rects[i].localPosition = Vector3.zero;
                            }
                        }
                        
                    }
                    value.transform.position = screenPos;
                }
            }
        }
    }
    
    void RideOnOffRope()
    {
        if (isRidingRope)
        {
            if (JCW.UI.Options.InputBindings.ITT_KeyManager.Instance.GetKeyDown(JCW.UI.Options.InputBindings.PlayerAction.Interaction))
            {
                isRidingRope = false;
                float jumpPower = 1f;

                jumpPower = minDistObj.GetComponentInChildren<RopeSpawner>().EndRopeAction(this.gameObject);

                PlayerController3D playerController = GetComponent<PlayerController3D>();
                Vector3 inertiaVec = transform.forward;
                Debug.Log("jumpPower: " + jumpPower);
                inertiaVec.y = 0;
                transform.LookAt(transform.position + inertiaVec);
                playerController.MakeinertiaVec(playerController.walkSpeed, inertiaVec.normalized);
                playerController.moveVec = Vector3.up * playerController.jumpSpeed * jumpPower;
                playerController.enabled = true;
            }

        }
        else
        {
            if(!isRailReady)
            {
                if(minDistObj != null)
                {
                    if (JCW.UI.Options.InputBindings.ITT_KeyManager.Instance.GetKeyDown(JCW.UI.Options.InputBindings.PlayerAction.Interaction))
                    {
                        minDistObj.GetComponentInChildren<RopeSpawner>().StartRopeAction(this.gameObject);
                    }
                }
            }
        }
        
    }

    void RenderInteractingUI()
    {
        if(minDistObj != null && !interactionState.isRidingRope)
        {
            RaycastHit hit;
            RopeSpawner spawner = minDistObj.GetComponentInChildren<RopeSpawner>();

            Physics.Raycast(transform.position, (minDistObj.transform.position - transform.position), out hit, spawner.detectingRange * 1.5f, layerFilter, QueryTriggerInteraction.Ignore);

            if (!(hit.distance < spawner.interactableRange))
            {
                detectedRopeSpawners.GetValueOrDefault(minDistObj).GetComponentsInChildren<Image>()[1].sprite = gaugeImage;
                minDistObj = null;
            }
            else
            {
                GameObject minDistUI = detectedRopeSpawners.GetValueOrDefault(minDistObj);
                if (minDistUI != null)
                {
                    detectedRopeSpawners.GetValueOrDefault(minDistObj).GetComponentsInChildren<Image>()[1].sprite = interactingImage;
                }
            }
        }
    }

    void UIRender()
    {

        Camera mainCamera = Camera.main;

        foreach(var ropeSpawner in detectedRopeSpawners)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(ropeSpawner.Key.transform.position);

            screenPos.y = ConfineInRange(screenPos.y, 50, Screen.height - 50);

            if (screenPos.z < 0)
            {
                screenPos.y = 50;
                screenPos.x = -screenPos.x;
            }

            screenPos.x = ConfineInRange(screenPos.x, 50, Screen.width - 50);

            ropeSpawner.Value.transform.position = screenPos;
        }

        foreach (var ropeSpawner in interactableRopeSpawners)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(ropeSpawner.Key.transform.position);

            screenPos.y = ConfineInRange(screenPos.y, 50, Screen.height - 50);

            if (screenPos.z < 0)
            {
                screenPos.y = 50;
                screenPos.x = -screenPos.x;
            }

            screenPos.x = ConfineInRange(screenPos.x, 50, Screen.width - 50);

            ropeSpawner.Value.transform.position = screenPos;
        }


    }

    float ConfineInRange(float num, float min, float max)
    {
        if (num < min)
        {
            num = min;
            return num;
        }

        if (num > max)
        {
            num = max;
            return num;
        }

        return num;
    }


    private void OnDrawGizmos()
    {
        if (isRailFounded)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_raycastHit.point, 1f);
        }
    
        //{
        //    Gizmos.DrawLine(startUp, endUp);
        //    Gizmos.DrawLine(startDown, endDown);
        //    Gizmos.DrawLine(startRight, endRight);
        //    Gizmos.DrawLine(startLeft, endLeft);
    
        //    Gizmos.DrawWireSphere(startCenter, rangeRadius);
        //    Gizmos.DrawWireSphere(endCenter, rangeRadius);
        //}
    }
    private void OnTriggerEnter(Collider other)
    {
        switch(other.gameObject.tag)
        {
            case "RopeSpawner":
                {
                    detectedRopeSpawners.Add(other.gameObject.transform.parent.gameObject, Instantiate(detectingUI, -Vector3.zero, Quaternion.Euler(Vector3.zero), UI.transform));
                }
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "RopeSpawner":
                {
                    GameObject detectingUIObj = detectedRopeSpawners.GetValueOrDefault(other.gameObject.transform.parent.gameObject);
                    detectedRopeSpawners.Remove(other.gameObject.transform.parent.gameObject);
                    Destroy(detectingUIObj);
                }
                break;
        }
    }
}
