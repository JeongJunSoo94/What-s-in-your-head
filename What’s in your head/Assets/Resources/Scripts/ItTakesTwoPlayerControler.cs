using JCW.InputBindings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Photon.Pun;
using JCW.AudioCtrl;

[RequireComponent(typeof(PhotonView))]
public class ItTakesTwoPlayerControler : MonoBehaviour
{
    [SerializeField]  private GameObject UI_BG;    
    private GameObject UI_instance;
    public float walkSpeed = 4.0f;
    public float runSpeed = 6.0f;
    public float dashSpeed = 4.0f;
    float addSpeed = 0.0f;
    public float dashTime = 0.5f;
    public float JumpPower = 10.0f;
    public float rotationSpeed = 360.0f;

    Rigidbody pRigidbody;
    Camera pCamera;
    RaycastHit[] raycastHits;

    Vector3 direction;
    Vector3 dashVec = Vector3.zero;

    public bool onPlatform = true;
    public bool isJumping = false;
    public bool isDash = false;
    public bool isAirDash = false;
    public int jumpcount = 0;
    public int dashcount = 0;

    private int life = 3;
    public int CPcount = 0;

    PhotonView photonView;
    [SerializeField]  private GameObject vCam;


    // Start is called before the first frame update
    void Start()
    {
        UI_instance = Instantiate(UI_BG, this.transform).transform.GetChild(0).gameObject;
        photonView = GetComponent<PhotonView>();
        if (!photonView.IsMine)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
            vCam.SetActive(false);
        }
        else
            pCamera = Camera.main;
        ITT_KeyManager.Instance.GetKeyDown(PlayerAction.MoveForward);
        pRigidbody = gameObject.GetComponent<Rigidbody>();
        //pCamera = Camera.main;

    }

    void Update()
    {
        if (!photonView.IsMine)
            return;
        // 임시로 해놓음
        if (ITT_KeyManager.Instance.GetKeyDown(PlayerAction.Pause))
        {
            UI_instance.SetActive(!UI_instance.activeSelf);
            if (UI_instance.activeSelf)
                Time.timeScale = 0.0f;
            else
                Time.timeScale = 1.0f;
        }
        Dash();
        Jump();

        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("현재 플레이어 목숨 : " + --life);
            Resurrect();
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            SoundManager.instance.PlayBGM("POP");
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            SoundManager.instance.PlayBGM("Tomboy");
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            SoundManager.instance.PauseResumeBGM();
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            SoundManager.instance.PlayEffect(SoundManager.instance.GetEffectClips("Explosion"));
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            SoundManager.instance.PlayEffect(SoundManager.instance.GetEffectClips("Fireball"));
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            SoundManager.instance.PlayEffect(SoundManager.instance.GetEffectClips("GetItem"));
        }
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            SoundManager.instance.PlayEffect(SoundManager.instance.GetEffectClips("WaterBall"));
        }

    }
    void Resurrect()
    {
        if (!File.Exists(Application.dataPath + "/Resources/CheckPointInfo/" +this.name + "TF" + (CPcount-1).ToString() + ".json"))
        {
            Debug.Log("체크포인트 불러오기 실패");
            return;
        }

        string jsonString = File.ReadAllText(Application.dataPath + "/Resources/CheckPointInfo/" +this.name + "TF" + (CPcount-1).ToString() + ".json");
        Debug.Log(jsonString);

        SavePosition.PlayerInfo data = JsonUtility.FromJson<SavePosition.PlayerInfo>(jsonString);
        this.transform.position = new Vector3((float)data.position[0], (float)data.position[1], (float)data.position[2]);
        this.transform.rotation = new Quaternion((float)data.rotation[0], (float)data.rotation[1], (float)data.rotation[2], (float)data.rotation[3]);        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(photonView.IsMine)
            Move();
    }
    void Move()
    {
        float moveSpeed;

        if (!isDash && ITT_KeyManager.Instance.GetKey(PlayerAction.ToggleRun))
        {
            moveSpeed = runSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }

        direction = pCamera.transform.forward * ((ITT_KeyManager.Instance.GetKey(PlayerAction.MoveForward) ? 1:0) + (ITT_KeyManager.Instance.GetKey(PlayerAction.MoveBackward) ? -1:0))
            + pCamera.transform.right * ((ITT_KeyManager.Instance.GetKey(PlayerAction.MoveRight) ? 1 : 0) + (ITT_KeyManager.Instance.GetKey(PlayerAction.MoveLeft) ? -1 : 0));
        direction.y = 0;
        direction = direction.normalized;

        Vector3 forward = Vector3.Slerp(transform.forward, direction, rotationSpeed * Time.deltaTime / Vector3.Angle(transform.forward, direction));
        transform.LookAt(transform.position + forward);

        direction = forward * moveSpeed;
        //(!onPlatform || isJumping) 으로 하면 점프후 대시를 할때 대시중 아래로 계속 떨어짐
        //((onPlatform && !isJumping)) || (isDash && isJumping)) 으로 하면 점프후 대시할때 높이유지는 되지만 대시중 점프가 막힘 대시중 점프랑 점프중 대시를 다르게 구분할수 있게해야함
        if ((onPlatform && !isJumping) || isAirDash) // 으로 하면 점프후 대시후 점프가 막힘 >>> 그래서 점프할때 isAirDash 상태면 isAirDash를 false 로 변경하니 해결
        {
            direction.y = 0;
        }
        else
        {
            direction.y = pRigidbody.velocity.y;
        }

        pRigidbody.velocity = direction + dashVec;
    }

    void Dash()
    {
        dashVec = Vector3.zero;
        if (!isDash && !(dashcount > 0) && ITT_KeyManager.Instance.GetKeyDown(PlayerAction.Dash))
        {
            StartCoroutine(nameof(CorDash));
        }
        dashVec = transform.forward.normalized * addSpeed;
    }

    IEnumerator CorDash()
    {
        if(!onPlatform)
        {
            isAirDash = true;
        }
        isDash = true;
        addSpeed = dashSpeed;
        dashcount++;
        yield return new WaitForSeconds(dashTime);
        isAirDash = false;
        isDash = false;
        addSpeed = 0.0f;
    }

    void Jump()
    {
        if (ITT_KeyManager.Instance.GetKeyDown(PlayerAction.Jump))
        {
            if(isAirDash)
            {
                isAirDash = false;
            }

            if (!isJumping || jumpcount < 2)
            {
                pRigidbody.velocity = new Vector3(pRigidbody.velocity.x, JumpPower, pRigidbody.velocity.z);
                jumpcount++;
                isJumping = true;
            }
        }
    }

    bool RaycheckGround()
    {
        raycastHits = Physics.SphereCastAll(transform.position, 0.15f, -transform.up,0.01f,LayerMask.NameToLayer("Platform")); 
        Debug.Log("raycastHits.Length: " + raycastHits.Length);
        bool rayCheck = false;
        for (int index = 0; index < raycastHits.Length; ++index)
        {
            if (raycastHits[index].collider.CompareTag("Platform"))
            {
                rayCheck = true;
            }
        }
        Debug.Log("rayCheck: " + rayCheck);
        return rayCheck;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            if(RaycheckGround())
            {
                onPlatform = true;
                isJumping = false;
                jumpcount = 0;
                dashcount = 0;
                //pRigidbody.useGravity = false
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        dashcount = 0;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            if(!RaycheckGround())
            {
                jumpcount = 1;
                onPlatform = false;
                //pRigidbody.useGravity = true;

                if (isDash && isJumping)
                {
                    {
                        //dashcount = 1; 여기는 지면대시 >> 점프 >> 대시를 막는 코드라서 지움
                        StopCoroutine(nameof(CorDash)); // 지면대시 >> 점프 >> 대시하면 처음 대시의 코루틴이 작동해서 공중대시가 빨리끝남
                        addSpeed = 0.0f;
                        isDash = false;
                    }
                }
            }
        }
    }
}