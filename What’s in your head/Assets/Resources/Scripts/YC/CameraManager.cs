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

        float currentMiddleValue = 0.5f; // 현재 카메라 분할 중앙값

        [SerializeField]
        float defenceModeFOV = 60;

        [Header("<기획 편집 사항>")]
        [Header("보간 시간(디펜스 모드 카메라가 늘어나고 줄어드는 속도)")]
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

                // << : 유닛 테스트용 디펜스 모드 전환
                //      
                //      타이틀 씬 통해서 접근시
                //      아래 함수 게임매니저 통해 플레이어들 불러오도록
                //      플레이어 각각 NormalView함수 인풋 부분 수정(탑 스테이트로 바꾸는 부분)

                //if (Input.GetKeyDown(KeyCode.Alpha9)) // 게임매니저 통해서 호출하도록 
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
            // 유닛 테스트 임시용이다.
            // 타이틀 씬 통해서 접속시, 게임매니저 통해서 플레이어 받아온다
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
                //Cor_SetSplitCamera((int)CharacterCamera.NELLA, 0.64f, 2f); 다른 스테이지 미완
            }
            else if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                //Cor_SetSplitCamera((int)CharacterCamera.STEADY, 0.36f, 2f); 다른 스테이지 미완

            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {

            }
        }






        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> // 

        // 스테이지 3 디펜스 모드 : 넬라 죽었을 때 호출
        public void NellaDeadCam()
        {
            pv.RPC(nameof(InitCamera), RpcTarget.AllViaServer, (int)CharacterCamera.STEADY); // 일단 스테디 카메라를 Full로 세팅
            pv.RPC(nameof(Cor_SetSizeCamera), RpcTarget.AllViaServer, 0.36f, _LerpTime);// 넬라 카메라를 만들어줌 (늘려줌)
        }

        // 스테이지 3 디펜스 모드 : 스테디 죽었을 때 호출
        public void SteadyDeadCam()
        {
            pv.RPC(nameof(InitCamera), RpcTarget.AllBuffered, (int)CharacterCamera.NELLA); // 일단 넬라 카메라를 Full로 세팅
            pv.RPC(nameof(Cor_SetSizeCamera), RpcTarget.AllBuffered, 0.64f, _LerpTime); // 스테디 카메라를 만들어줌 (늘려줌)
        }

        // 스테이지 3 디펜스 모드 : 넬라가 부활했을 때나, 스테디가 부활했을 때 호출
        public void ReviveCam()
        {
            pv.RPC(nameof(Cor_SetFullScreen), RpcTarget.AllBuffered, _LerpTime); // 죽기 전 Full로 되어있던 카메라를 다시 Full로 만들어줌
        }

        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> // 




        // Stage 3 Defence Mode - Sizing 호출 전 초기화 함수 (한프레임 동안 카메라를 Full로 세팅)
        [PunRPC]
        public void InitCamera(int targetCamera)
        {
            if (targetCamera == (int)CharacterCamera.NELLA) // 넬라 카메라 전체화면
            {
                Rect rc1 = new Rect(0, 0, 1, 1);
                cameras[(int)CharacterCamera.NELLA].rect = rc1;
                Rect rc2 = new Rect(1, 0, 0, 1);
                cameras[(int)CharacterCamera.STEADY].rect = rc2;

                curFullCamera = CharacterCamera.NELLA;
            }
            else if (targetCamera == (int)CharacterCamera.STEADY) // 스테디 카메라 전체화면
            {
                Rect rc1 = new Rect(0, 0, 0, 1);
                cameras[(int)CharacterCamera.NELLA].rect = rc1;
                Rect rc2 = new Rect(0, 0, 1, 1);
                cameras[(int)CharacterCamera.STEADY].rect = rc2;

                curFullCamera = CharacterCamera.STEADY;
            }
        }


        //Stage 3 Defence Mode - Sizing 전체 화면 세팅 (Lerp 시간 동안)
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

            if (curFullCamera == CharacterCamera.NELLA) // 넬라 카메라 전체화면
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
            else if (curFullCamera == CharacterCamera.STEADY) // 스테디 카메라 전체화면
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


        // Stage 3 Defence Mode - Sizing 전체화면 상황에서, 현재 전체화면이 누구 카메라인지에 따라 새로 카메라를 만들어준다. (Lerp 시간 동안)
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

            if (curFullCamera == CharacterCamera.STEADY) // 스테디 전체화면이면, 넬라 카메라를 만들어줌(늘려줌)
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
            else if (curFullCamera == CharacterCamera.NELLA) // 넬라 전체화면이면, 스테디 카메라를 만들어줌(늘려줌)
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


        // 분할 화면에서 죽었을때 ex 0.5f -> 0.36f (미완)
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

            if (targetCamera == (int)CharacterCamera.STEADY) // 스테디 죽었을 때 (스테디 카메라를 줄여준다)
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
            else if (targetCamera == (int)CharacterCamera.NELLA) // 넬라 죽었을 때 (넬라 카메라를 줄여준다)
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