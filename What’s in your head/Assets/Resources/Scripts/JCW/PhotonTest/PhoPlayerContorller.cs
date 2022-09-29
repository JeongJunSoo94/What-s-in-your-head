using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhoPlayerContorller : MonoBehaviour
{
    [SerializeField]
    [Range(0,10)]
    private float Mouse_Sensitivity = 4.5f;

    [SerializeField]
    private GameObject GunSound = null;

    public float runSpeed = 6.0f;
    public float rotationSpeed = 360.0f;

    CharacterController pcController;
    Vector3 direction;

    Animator animator;

    public Animator idleAnimator;
    public Animator swordAnimator;
    public Animator bowAnimator;
    public Animator gunAnimator;
    public GameObject Sword;
    public GameObject Bow;
    public GameObject Arrow;
    public GameObject Gun;

    private PhotonView photonView;


    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (!photonView.IsMine)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
        }
        pcController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;
        animator.SetFloat("Speed", pcController.velocity.magnitude);
        CharacterControl_Slerp();
        Sit();
        Attack();
        WeaponChange();
        RotateView();
    }

    void Sit()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if(animator.GetBool("Sit"))
                animator.SetBool("Sit", false);
            else
                animator.SetBool("Sit", true);
        }
    }

    void Attack()
    {
        if(Sword.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                animator.SetTrigger("Attack");
            }
        }
        else if(Bow.activeSelf)
        {            
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (animator.GetBool("Attack_Ready")
                    || animator.GetCurrentAnimatorStateInfo(0).IsName("Run_Ready"))
                { 
                    animator.SetTrigger("Attack_Fire");
                    animator.SetBool("Attack_Ready", false);
                }
                else
                    animator.SetBool("Attack_Ready",true);
            }
        }
        else if(Gun.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                animator.SetTrigger("Attack_Gun");
            }
        }
        
    }

    void WeaponChange()
    {
        // 검
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (animator.runtimeAnimatorController.name != swordAnimator.runtimeAnimatorController.name)
            {
                animator.runtimeAnimatorController = swordAnimator.runtimeAnimatorController;
                Gun.SetActive(false);
                Bow.SetActive(false);
                Sword.SetActive(true);
            }
            else
            {
                animator.runtimeAnimatorController = idleAnimator.runtimeAnimatorController;
                Sword.SetActive(false);
            }
        }

        // 활
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (animator.runtimeAnimatorController.name != bowAnimator.runtimeAnimatorController.name)
            {
                animator.runtimeAnimatorController = bowAnimator.runtimeAnimatorController;
                Gun.SetActive(false);
                Bow.SetActive(true);
                Sword.SetActive(false);
            }
                
            else
            {
                animator.runtimeAnimatorController = idleAnimator.runtimeAnimatorController;
                Bow.SetActive(false);
            }
                
        }

        // 총
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (animator.runtimeAnimatorController.name != gunAnimator.runtimeAnimatorController.name)
            {
                animator.runtimeAnimatorController = gunAnimator.runtimeAnimatorController;
                Gun.SetActive(true);
                Bow.SetActive(false);
                Sword.SetActive(false);
            }
            else
            {
                animator.runtimeAnimatorController = idleAnimator.runtimeAnimatorController;
                Gun.SetActive(false);
            }
        }
    }

    void CharacterControl_Slerp()
    {
        direction = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            direction = transform.forward;
        }

        if (Input.GetKey(KeyCode.A))
        {            
            transform.Rotate(0.0f, -1.0f, 0.0f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0.0f, 1.0f, 0.0f);
        }   
        pcController.Move(runSpeed * Time.deltaTime * direction);
    }

    void RotateView()
    {
        if (Input.GetMouseButton(1))
        {
            transform.Rotate(0.0f,
                                Input.GetAxisRaw("Mouse X") * Mouse_Sensitivity,
                                0.0f);
        }
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0.0f, transform.rotation.eulerAngles.y, 0.0f));
    }

    void setOnCollider()
    {
        Sword.transform.GetChild(0).transform.gameObject.GetComponent<BoxCollider>().enabled = true;
        print("검 콜리전 ON");
    }

    void setOffCollider()
    {
        Sword.transform.GetChild(0).transform.gameObject.GetComponent<BoxCollider>().enabled = false;
        print(Sword.transform.GetChild(0).transform.gameObject.GetComponent<BoxCollider>().name);
        print("검 콜리전 OFF");
    }

    void FireArrow()
    {
        Vector3 ArrowDirection = Bow.transform.rotation * Vector3.left;
        Instantiate(Arrow, Bow.transform.position, Quaternion.LookRotation(ArrowDirection));
    }

    void GunFire_Sound()
    {
        GunSound.GetComponent<AudioSource>().Play();
    }
    
}
