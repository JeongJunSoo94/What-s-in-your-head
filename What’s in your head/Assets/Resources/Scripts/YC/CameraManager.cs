using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YC.Camera_;

namespace YC.CameraManager_
{
    public enum CharacterCamera { NELLA, STEADY };

    [RequireComponent(typeof(PhotonView))]
    public class CameraManager : MonoBehaviour
    {
        [HideInInspector] public Camera[] cameras;

        public bool isBlending { get; private set; }

        PhotonView pv;
        public static CameraManager Instance = null;

        //float currentMiddleValue = 0.5f; // ���� ī�޶� ���� �߾Ӱ�


        [Header("<��ȹ ���� ����>")]
        [Header("���� �ð�(���潺 ��� ī�޶� �þ�� �پ��� �ӵ�)")]
        [SerializeField] float _LerpTime = 2.5f;

        CharacterCamera curFullCamera;

        Coroutine curCoroutine;

        bool wasTopView;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);

            DontDestroyOnLoad(this.gameObject);

            pv = GetComponent<PhotonView>();

            cameras = new Camera[2];
            isBlending = false;

            curFullCamera = new CharacterCamera();

        }

        void Update()
        {
            if (cameras[(int)CharacterCamera.NELLA] == null ||
                cameras[(int)CharacterCamera.STEADY] == null) return;

            if (pv.IsMine)
            {
                //Debugg();

                // << : ���� �׽�Ʈ�� ���潺 ��� ��ȯ
                //      
                //      Ÿ��Ʋ �� ���ؼ� ���ٽ�
                //      �Ʒ� �Լ� ���ӸŴ��� ���� �÷��̾�� �ҷ�������
                //      �÷��̾� ���� NormalView�Լ� ��ǲ �κ� ����(ž ������Ʈ�� �ٲٴ� �κ�)

                if (GameManager.Instance.isTopView)
                {
                    // >> : ����
                    //pv.RPC(nameof(InitCamera), RpcTarget.AllBuffered, (int)CharacterCamera.NELLA); 
                    //pv.RPC(nameof(SetDefenceModeCamera), RpcTarget.AllBuffered);

                    // >> : ����
                    if(!wasTopView)
                    {
                        pv.RPC(nameof(SetDefenceModeCamera), RpcTarget.AllBuffered);
                        wasTopView = true;
                    }                    
                }
                else if (wasTopView)
                {
                    wasTopView = false;
                }
            }
        }

        [PunRPC]
        void SetDefenceModeCamera()
        {
            // >> : ����
            //// ���� �׽�Ʈ �ӽÿ��̴�.
            //// Ÿ��Ʋ �� ���ؼ� ���ӽ�, ���ӸŴ��� ���ؼ� �÷��̾� �޾ƿ´�

            //GameObject NellaTemp = GameObject.FindGameObjectWithTag("Nella");
            //GameObject SteadyTemp = GameObject.FindGameObjectWithTag("Steady");

            //if(NellaTemp)
            //    NellaTemp.GetComponent<CameraController>().SetDefenseMode();

            //if (SteadyTemp)
            //    SteadyTemp.GetComponent<CameraController>().SetDefenseMode();


            // >> : ����
            if (GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient]) // �ڶ��
            {
                //pv.RPC(nameof(InitCamera), RpcTarget.AllBuffered, (int)CharacterCamera.NELLA);
                InitCamera((int)CharacterCamera.NELLA);
                GameObject.FindGameObjectWithTag("Nella").GetComponent<CameraController>().SetDefenseMode();
            }
            else // ���׵���
            {
                //pv.RPC(nameof(InitCamera), RpcTarget.AllBuffered, (int)CharacterCamera.STEADY);
                InitCamera((int)CharacterCamera.STEADY);
                GameObject.FindGameObjectWithTag("Steady").GetComponent<CameraController>().SetDefenseMode();
            }
        }

        void Debugg()
        {
            if (isBlending) return;

            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                //NellaDeadCam();

            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                //SteadyDeadCam();
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                //ReviveCam();
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                //Cor_SetSplitCamera((int)CharacterCamera.NELLA, 0.64f, 2f); �ٸ� �������� �̿�
            }
            else if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                //Cor_SetSplitCamera((int)CharacterCamera.STEADY, 0.36f, 2f); �ٸ� �������� �̿�

            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {

            }
        }

        public void DeadCam(bool isNella)
        {
            pv.RPC(nameof(DeadCam_RPC), RpcTarget.AllViaServer, isNella);            
        }

        [PunRPC]
        void DeadCam_RPC(bool isNella)
        {
            if(curCoroutine!=null)
                StopCoroutine(curCoroutine);
            curCoroutine = StartCoroutine(SetDeadScreen(_LerpTime, isNella));
        }

        public IEnumerator SetDeadScreen(float LerpTime, bool isNella)
        {
            Rect camRect1 = cameras[(int)CharacterCamera.NELLA].rect;
            Rect camRect2 = cameras[(int)CharacterCamera.STEADY].rect;
            float currentTime = 0;

            isBlending = true;

            if (isNella) // �ڶ� ī�޶� 0.5 -> 0.36f , ���׵� ī�޶� 0.5 -> 0.64f
            {
                while (camRect1.width > 0.36f)
                {
                    camRect1 = cameras[(int)CharacterCamera.NELLA].rect;
                    camRect2 = cameras[(int)CharacterCamera.STEADY].rect;

                    currentTime += Time.deltaTime;
                    if (currentTime >= LerpTime) currentTime = LerpTime;

                    float width = Mathf.Lerp(camRect1.width, 0.36f, currentTime * 2f / LerpTime);
                    if (width < 0.36f) { width = 0.36f; }

                    Rect rc1 = new Rect(0, camRect1.y, width, camRect1.height);
                    cameras[(int)CharacterCamera.NELLA].rect = rc1;
                    Rect rc2 = new Rect(width, camRect2.y, 1 - width, camRect2.height);
                    cameras[(int)CharacterCamera.STEADY].rect = rc2;

                    if (width == 0.36f) yield break;

                    yield return null;
                }

                isBlending = false;
            }
            else // ���׵� ī�޶� 0.5 -> 0.36f, �ڶ� ī�޶� 0.5 -> 0.64f
            {
                while (camRect2.width > 0.36f)
                {
                    camRect1 = cameras[(int)CharacterCamera.NELLA].rect;
                    camRect2 = cameras[(int)CharacterCamera.STEADY].rect;

                    currentTime += Time.deltaTime;
                    if (currentTime >= LerpTime) currentTime = LerpTime;

                    float width = Mathf.Lerp(camRect2.width, 0.36f, currentTime * 2f / LerpTime);
                    if (width < 0.36f) width = 0.36f;

                    Rect rc1 = new Rect(0, camRect1.y, 1 - width, camRect1.height);
                    cameras[(int)CharacterCamera.NELLA].rect = rc1;
                    Rect rc2 = new Rect(1 - width, camRect2.y, width, camRect2.height);
                    cameras[(int)CharacterCamera.STEADY].rect = rc2;

                    if (width == 0.36f) yield break;

                    yield return null;
                }

                isBlending = false;
            }
        }

        public void ReviveCam(bool isNella)
        {
            pv.RPC(nameof(ReviveCam_RPC), RpcTarget.AllViaServer, isNella);
        }

        // �������� 3 ���潺 ��� : �ڶ� ��Ȱ���� ����, ���׵� ��Ȱ���� �� ȣ��
        [PunRPC]
        public void ReviveCam_RPC(bool isNella)
        {
            if(curCoroutine != null)
                StopCoroutine(curCoroutine);
            curCoroutine = StartCoroutine(SetHalfScreen(_LerpTime, isNella));
        }

        

        public IEnumerator SetHalfScreen(float LerpTime, bool isNella)
        {
            Rect camRect1 = cameras[(int)CharacterCamera.NELLA].rect;
            Rect camRect2 = cameras[(int)CharacterCamera.STEADY].rect;
            float currentTime = 0;

            isBlending = true;

            if (isNella) // �ڶ� ī�޶� 0.36 -> 0.5f
            {
                while (camRect1.width < 0.5f)
                {
                    camRect1 = cameras[(int)CharacterCamera.NELLA].rect;
                    camRect2 = cameras[(int)CharacterCamera.STEADY].rect;

                    currentTime += Time.deltaTime;
                    if (currentTime >= LerpTime) currentTime = LerpTime;

                    float width = Mathf.Lerp(camRect1.width, 0.5f, currentTime / LerpTime);
                    if (width > 0.5f) width = 0.5f;

                    Rect rc1 = new Rect(0, camRect1.y, width, camRect1.height);
                    cameras[(int)CharacterCamera.NELLA].rect = rc1;
                    Rect rc2 = new Rect(width, camRect2.y, 1 - width, camRect2.height);
                    cameras[(int)CharacterCamera.STEADY].rect = rc2;

                    if (width == 0.5f) yield break;
                    yield return null;
                }
            }
            else // ���׵� ī�޶� 0.36 -> 0.5f
            {
                while (camRect2.width < 0.5f)
                {
                    camRect1 = cameras[(int)CharacterCamera.NELLA].rect;
                    camRect2 = cameras[(int)CharacterCamera.STEADY].rect;

                    currentTime += Time.deltaTime;
                    if (currentTime >= LerpTime) currentTime = LerpTime;

                    float width = Mathf.Lerp(camRect2.width, 0.5f, currentTime / LerpTime);
                    if (width > 0.5f) width = 0.5f;

                    Rect rc1 = new Rect(0, camRect1.y, 1 - width, camRect1.height);
                    cameras[(int)CharacterCamera.NELLA].rect = rc1;
                    Rect rc2 = new Rect(1 - width, camRect2.y, width, camRect2.height);
                    cameras[(int)CharacterCamera.STEADY].rect = rc2;

                    if (width == 0.5f) yield break;

                    yield return null;
                }
            }

            isBlending = false;
        }


        // Stage 3 Defence Mode - Sizing ȣ�� �� �ʱ�ȭ �Լ� (�������� ���� ī�޶� Full�� ����)
        [PunRPC]
        public void InitCamera(int targetCamera)
        {
            if (targetCamera == (int)CharacterCamera.NELLA) // �ڶ� ī�޶� ��üȭ��
            {
                Rect rc1 = new Rect(0, 0, 1, 1);
                cameras[(int)CharacterCamera.NELLA].rect = rc1;
                Rect rc2 = new Rect(1, 0, 0, 1);
                cameras[(int)CharacterCamera.STEADY].rect = rc2;

                curFullCamera = CharacterCamera.NELLA;
            }
            else if (targetCamera == (int)CharacterCamera.STEADY) // ���׵� ī�޶� ��üȭ��
            {
                Rect rc1 = new Rect(0, 0, 0, 1);
                cameras[(int)CharacterCamera.NELLA].rect = rc1;
                Rect rc2 = new Rect(0, 0, 1, 1);
                cameras[(int)CharacterCamera.STEADY].rect = rc2;

                curFullCamera = CharacterCamera.STEADY;
            }
        }


        //Stage 3 Defence Mode - Sizing ��ü ȭ�� ���� (Lerp �ð� ����)
        //[PunRPC]
        //public void Cor_SetFullScreen(float lerpTime)
        //{
        //    StartCoroutine(SetFullScreen(lerpTime));
        //}
        public IEnumerator SetFullScreen(float LerpTime)
        {
            Rect camRect1 = cameras[(int)CharacterCamera.NELLA].rect;
            Rect camRect2 = cameras[(int)CharacterCamera.STEADY].rect;
            float currentTime = 0;

            isBlending = true;

            if (curFullCamera == CharacterCamera.STEADY) // �ڶ� ī�޶� ��üȭ��
            {
                while (camRect1.width < 1)
                {
                    camRect1 = cameras[(int)CharacterCamera.NELLA].rect;
                    camRect2 = cameras[(int)CharacterCamera.STEADY].rect;

                    currentTime += Time.deltaTime;
                    if (currentTime >= LerpTime) currentTime = LerpTime;

                    float wd = Mathf.Lerp(camRect1.width, 1, currentTime / LerpTime);
                    if (wd > 1) wd = 1;

                    Rect rc1 = new Rect(camRect1.x, camRect1.y, wd, camRect1.height);
                    cameras[(int)CharacterCamera.NELLA].rect = rc1;
                    Rect rc2 = new Rect(wd, camRect2.y, 1 - wd, camRect2.height);
                    cameras[(int)CharacterCamera.STEADY].rect = rc2;

                    yield return null;
                    // >> : ���� 
                    InitCamera((int)CharacterCamera.STEADY);


                }

                isBlending = false;
            }
            else if (curFullCamera == CharacterCamera.NELLA) // ���׵� ī�޶� ��üȭ��
            {
                while (camRect2.x > 0.0001f)
                {
                    camRect1 = cameras[(int)CharacterCamera.NELLA].rect;
                    camRect2 = cameras[(int)CharacterCamera.STEADY].rect;

                    currentTime += Time.deltaTime;
                    if (currentTime >= LerpTime) currentTime = LerpTime;

                    float wd = Mathf.Lerp(camRect1.width, 0.0001f, currentTime / LerpTime);
                    if (wd < 0.0001f) wd = 0;

                    Rect rc1 = new Rect(camRect1.x, camRect1.y, wd, camRect1.height);
                    cameras[(int)CharacterCamera.NELLA].rect = rc1;
                    Rect rc2 = new Rect(wd, camRect2.y, 1 - wd, camRect2.height);
                    cameras[(int)CharacterCamera.STEADY].rect = rc2;
                    yield return null;

                    // >> : ���� 
                    InitCamera((int)CharacterCamera.NELLA);
                }

                isBlending = false;
            }
        }


        // Stage 3 Defence Mode - Sizing ��üȭ�� ��Ȳ����, ���� ��üȭ���� ���� ī�޶������� ���� ���� ī�޶� ������ش�. (Lerp �ð� ����)
        [PunRPC]
        public void Cor_SetSizeCamera(float middleValue, float lerpTime)
        {
            StartCoroutine(SetSizeCamera(middleValue, lerpTime));
        }
        public IEnumerator SetSizeCamera(float middleValue, float LerpTime)
        {
            Rect camRect1 = cameras[(int)CharacterCamera.NELLA].rect;
            Rect camRect2 = cameras[(int)CharacterCamera.STEADY].rect;
            float currentTime = 0;

            isBlending = true;

            //Debug.Log("���� ����!");
            if (curFullCamera == CharacterCamera.STEADY) // ���׵� ��üȭ���̸�, �ڶ� ī�޶� �������(�÷���)
            {
                while (camRect1.width < middleValue)
                {
                    camRect1 = cameras[(int)CharacterCamera.NELLA].rect;
                    camRect2 = cameras[(int)CharacterCamera.STEADY].rect;

                    currentTime += Time.deltaTime;
                    if (currentTime >= LerpTime) currentTime = LerpTime;

                    float wd = Mathf.Lerp(camRect1.width, middleValue, currentTime / LerpTime);

                    if (wd > middleValue) wd = middleValue;

                    Rect rc1 = new Rect(camRect1.x, camRect1.y, wd, camRect1.height);
                    cameras[(int)CharacterCamera.NELLA].rect = rc1;
                    Rect rc2 = new Rect(wd, camRect2.y, 1 - wd, camRect2.height);
                    cameras[(int)CharacterCamera.STEADY].rect = rc2;

                    //Debug.Log(wd);
                    yield return null;
                }
                //Debug.Log("���� ����!");

                isBlending = false;
            }
            else if (curFullCamera == CharacterCamera.NELLA) // �ڶ� ��üȭ���̸�, ���׵� ī�޶� �������(�÷���)
            {

                while (camRect2.x > middleValue)
                {
                    camRect1 = cameras[(int)CharacterCamera.NELLA].rect;
                    camRect2 = cameras[(int)CharacterCamera.STEADY].rect;

                    currentTime += Time.deltaTime;
                    if (currentTime >= LerpTime) currentTime = LerpTime;

                    float wd = Mathf.Lerp(camRect1.width, middleValue, currentTime / LerpTime);
                    if (wd < middleValue) wd = middleValue;

                    Rect rc1 = new Rect(camRect1.x, camRect1.y, wd, camRect1.height);
                    cameras[(int)CharacterCamera.NELLA].rect = rc1;
                    Rect rc2 = new Rect(wd, camRect2.y, 1 - wd, camRect2.height);
                    cameras[(int)CharacterCamera.STEADY].rect = rc2;
                    yield return null;
                }

                isBlending = false;
            }
        }


        // ���� ȭ�鿡�� �׾����� ex 0.5f -> 0.36f (�̿�)
        [PunRPC]
        public void Cor_SetSplitCamera(int targetCamera, float middleValue, float LerpTime)
        {
            StartCoroutine(SetSplitCamera(targetCamera, middleValue, LerpTime));
        }
        public IEnumerator SetSplitCamera(int targetCamera, float middleValue, float LerpTime)
        {
            Rect camRect1 = cameras[(int)CharacterCamera.NELLA].rect;
            Rect camRect2 = cameras[(int)CharacterCamera.STEADY].rect;
            float currentTime = 0;

            isBlending = true;

            if (targetCamera == (int)CharacterCamera.STEADY) // ���׵� �׾��� �� (���׵� ī�޶� �ٿ��ش�)
            {
                while (camRect1.width < middleValue)
                {
                    camRect1 = cameras[(int)CharacterCamera.NELLA].rect;
                    camRect2 = cameras[(int)CharacterCamera.STEADY].rect;

                    currentTime += Time.deltaTime;
                    if (currentTime >= LerpTime) currentTime = LerpTime;

                    float currentMiddleValue = Mathf.Lerp(camRect1.width, middleValue, currentTime / LerpTime);
                    if (currentMiddleValue > middleValue) currentMiddleValue = middleValue;

                    Rect rc1 = new Rect(camRect1.x, camRect1.y, currentMiddleValue, camRect1.height);
                    cameras[(int)CharacterCamera.NELLA].rect = rc1;
                    Rect rc2 = new Rect(currentMiddleValue, camRect2.y, 1 - currentMiddleValue, camRect2.height);
                    cameras[(int)CharacterCamera.STEADY].rect = rc2;

                    yield return null;
                }

                isBlending = false;
            }
            else if (targetCamera == (int)CharacterCamera.NELLA) // �ڶ� �׾��� �� (�ڶ� ī�޶� �ٿ��ش�)
            {
                while (camRect2.x > middleValue)
                {
                    camRect1 = cameras[(int)CharacterCamera.NELLA].rect;
                    camRect2 = cameras[(int)CharacterCamera.STEADY].rect;

                    currentTime += Time.deltaTime;
                    if (currentTime >= LerpTime) currentTime = LerpTime;

                    float currentMiddleValue = Mathf.Lerp(camRect1.width, middleValue, currentTime / LerpTime);
                    if (currentMiddleValue < middleValue) currentMiddleValue = middleValue;

                    Rect rc1 = new Rect(camRect1.x, camRect1.y, currentMiddleValue, camRect1.height);
                    cameras[(int)CharacterCamera.NELLA].rect = rc1;
                    Rect rc2 = new Rect(currentMiddleValue, camRect2.y, 1 - currentMiddleValue, camRect2.height);
                    cameras[(int)CharacterCamera.STEADY].rect = rc2;
                    yield return null;
                }

                isBlending = false;
            }
        }
    }
}