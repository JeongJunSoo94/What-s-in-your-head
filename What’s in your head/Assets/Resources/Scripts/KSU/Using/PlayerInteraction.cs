using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public Dictionary<GameObject, GameObject> detectedRopeSpawners = new Dictionary<GameObject, GameObject>();
    public Dictionary<GameObject, GameObject> interactableRopeSpawners = new Dictionary<GameObject, GameObject>();

    Camera mainCamera;

    public float UIRadius = 100f;

    public float rangeRadius = 5f;
    public float rangeDistance = 5f;
    public LayerMask targetLayer;
    public Ray ray;
    RaycastHit _raycastHit;
    public Vector3 hVision;

    public bool isRailFounded = false;
    public bool isRailReady = false;
    bool isRidingRope = false;

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


    //public List<GameObject> detectedRopeSpawners;
    //public List<GameObject> interactableRopeSpawners;

    

    GameObject minDistObj = null;
    GameObject minDistUI = null;


    /// <summary>
    public GameObject UI;
    public GameObject detectingUI;
    public Sprite gaugeImage;
    public Sprite interactingImage;
    /// </summary>


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        //ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    }

    // Update is called once per frame
    void Update()
    {
        //SearchRail();
        DetectObjects();
        RenderInteractingUI();
        RideOnOffRope();
        //UIRender();
    }

    private void FixedUpdate()
    {
        //UITest();
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

        if(detectedRopeSpawners.Count > 0)
        {
            foreach (var interactableObj in detectedRopeSpawners)
            {
                RaycastHit hit;
                GameObject key = interactableObj.Key;
                GameObject value = interactableObj.Value;
                RopeSpawner spawner = key.GetComponentInChildren<RopeSpawner>();

                Physics.Raycast(transform.position, (key.transform.position - transform.position), out hit, spawner.detectingRange * 1.1f);

                if (key.gameObject != hit.transform.gameObject)
                {
                    value.SetActive(false);
                }
                else
                {
                    value.SetActive(true);
                    float amount = (hit.distance - spawner.interactableRange) / (spawner.detectingRange - spawner.interactableRange);
                    if (!isRailReady || isRidingRope)
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

                    if (!(amount > 0))
                    {
                        amount = 1f;
                    }

                    value.GetComponentsInChildren<Image>()[1].fillAmount = amount;

                    Vector3 screenPos = mainCamera.WorldToScreenPoint(key.transform.position);
                    screenPos.y = ConfineInRange(screenPos.y, halfUIRadius, Screen.height - halfUIRadius);

                    if (screenPos.z < 0)
                    {
                        screenPos.y = halfUIRadius;
                        screenPos.x = -screenPos.x;
                    }

                    screenPos.x = ConfineInRange(screenPos.x, halfUIRadius, Screen.width - halfUIRadius);
                    RectTransform rect = value.GetComponent<RectTransform>();

                    if (screenPos.x == halfUIRadius || screenPos.y == halfUIRadius || screenPos.x == (Screen.width - halfUIRadius) || screenPos.y == (Screen.height - halfUIRadius))
                    {
                        float resizedHalfUIRaidus = halfUIRadius * 0.8f;
                        rect.sizeDelta = new Vector2(resizedHalfUIRaidus, resizedHalfUIRaidus) * 2f;
                        screenPos.x = ConfineInRange(screenPos.x, resizedHalfUIRaidus, Screen.width - resizedHalfUIRaidus);
                        screenPos.y = ConfineInRange(screenPos.y, resizedHalfUIRaidus, Screen.height - resizedHalfUIRaidus);
                    }
                    else
                    {
                        rect.sizeDelta = new Vector2(UIRadius, UIRadius);
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
            if (JCW.Options.InputBindings.ITT_KeyManager.Instance.GetKeyDown(JCW.Options.InputBindings.PlayerAction.Interaction))
            {
                isRidingRope = false;
                float jumpPower = minDistObj.GetComponentInChildren<RopeAction>().DestroyRope();

                PlayerController3D playerController = GetComponent<PlayerController3D>();
                Vector3 inertiaVec = transform.forward;
                Debug.Log("jumpPower: " + jumpPower);
                inertiaVec.y = 0;
                transform.LookAt(transform.position + inertiaVec);
                playerController.enabled = true;
                playerController.MakeinertiaVec(playerController.walkSpeed, inertiaVec.normalized);
                playerController.moveVec = Vector3.up * playerController.jumpSpeed * jumpPower;
            }

        }
        else
        {
            if(!isRailReady)
            {
                if (JCW.Options.InputBindings.ITT_KeyManager.Instance.GetKeyDown(JCW.Options.InputBindings.PlayerAction.Interaction))
                {
                    isRidingRope = true;
                    minDistObj.GetComponentInChildren<RopeSpawner>().StartRopeAction(this.gameObject);
                }
            }
        }
        
    }

    void RenderInteractingUI()
    {
        if(minDistObj != null)
        {
            detectedRopeSpawners.GetValueOrDefault(minDistObj).GetComponentsInChildren<Image>()[1].sprite = interactingImage;
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
