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

        float currentMiddleValue = 0.5f; // ���� ī�޶� ���� �߾Ӱ�

        [SerializeField]
        float defenceModeFOV = 60;

        [Header("<��ȹ ���� ����>")]
        [Header("���� �ð�(���潺 ��� ī�޶� �þ�� �پ��� �ӵ�)")]
        [SerializeField] float _LerpTime = 2.5f;




        CharacterCamera curFullCamera;

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
            if (cameras[(int)CharacterCamera.NELLA] == null || cameras[(int)CharacterCamera.STEADY] == null) return;

            if (pv.IsMine)
            {
                CheckAndPlay();

                // << : ���� �׽�Ʈ�� ���潺 ��� ��ȯ
                //      
                //      Ÿ��Ʋ �� ���ؼ� ���ٽ�
                //      �Ʒ� �Լ� ���ӸŴ��� ���� �÷��̾�� �ҷ�������
                //      �÷��̾� ���� NormalView�Լ� ��ǲ �κ� ����(ž ������Ʈ�� �ٲٴ� �κ�)

                //if (Input.GetKeyDown(KeyCode.Alpha9)) // ���ӸŴ��� ���ؼ� ȣ���ϵ��� 
                if (GameManager.Instance.isTopView)
                {
                    pv.RPC(nameof(InitCamera), RpcTarget.AllBuffered, (int)CharacterCamera.NELLA);
                    pv.RPC(nameof(SetDefenceModeCamera), RpcTarget.AllBuffered);

                }
            }
        }
        [PunRPC]
        void SetDefenceModeCamera()
        {
            // ���� �׽�Ʈ �ӽÿ��̴�.
            // Ÿ��Ʋ �� ���ؼ� ���ӽ�, ���ӸŴ��� ���ؼ� �÷��̾� �޾ƿ´�
            GameObject NellaTemp = GameObject.FindGameObjectWithTag("Nella");
            GameObject SteadyTemp = GameObject.FindGameObjectWithTag("Steady");

            if (NellaTemp)
                NellaTemp.GetComponent<CameraController>().SetDefenseMode();

            if (SteadyTemp)
                SteadyTemp.GetComponent<CameraController>().SetDefenseMode();

        }

        void CheckAndPlay()
        {
            if (isBlending) return;

            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                NellaDeadCam();

            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                SteadyDeadCam();
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                ReviveCam();
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






        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> // 

        // �������� 3 ���潺 ��� : �ڶ� �׾��� �� ȣ��
        public void NellaDeadCam()
        {
            pv.RPC(nameof(InitCamera), RpcTarget.AllViaServer, (int)CharacterCamera.STEADY); // �ϴ� ���׵� ī�޶� Full�� ����
            pv.RPC(nameof(Cor_SetSizeCamera), RpcTarget.AllViaServer, 0.36f, _LerpTime);// �ڶ� ī�޶� ������� (�÷���)
        }

        // �������� 3 ���潺 ��� : ���׵� �׾��� �� ȣ��
        public void SteadyDeadCam()
        {
            pv.RPC(nameof(InitCamera), RpcTarget.AllBuffered, (int)CharacterCamera.NELLA); // �ϴ� �ڶ� ī�޶� Full�� ����
            pv.RPC(nameof(Cor_SetSizeCamera), RpcTarget.AllBuffered, 0.64f, _LerpTime); // ���׵� ī�޶� ������� (�÷���)
        }

        // �������� 3 ���潺 ��� : �ڶ� ��Ȱ���� ����, ���׵� ��Ȱ���� �� ȣ��
        public void ReviveCam()
        {
            pv.RPC(nameof(Cor_SetFullScreen), RpcTarget.AllBuffered, _LerpTime); // �ױ� �� Full�� �Ǿ��ִ� ī�޶� �ٽ� Full�� �������
        }

        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> // 




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
        [PunRPC]
        public void Cor_SetFullScreen(float lerpTime)
        {
            StartCoroutine(SetFullScreen(lerpTime));
        }
        public IEnumerator SetFullScreen(float LerpTime)
        {
            Rect camRect1 = cameras[(int)CharacterCamera.NELLA].rect;
            Rect camRect2 = cameras[(int)CharacterCamera.STEADY].rect;
            float currentTime = 0;

            isBlending = true;

            if (curFullCamera == CharacterCamera.NELLA) // �ڶ� ī�޶� ��üȭ��
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
                }

                isBlending = false;
            }
            else if (curFullCamera == CharacterCamera.STEADY) // ���׵� ī�޶� ��üȭ��
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
                    yield return null;
                }

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