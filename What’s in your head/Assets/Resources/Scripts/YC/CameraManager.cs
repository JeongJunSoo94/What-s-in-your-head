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

        //float currentMiddleValue = 0.5f; // 현재 카메라 분할 중앙값


        [Header("<기획 편집 사항>")]
        [Header("보간 시간(디펜스 모드 카메라가 늘어나고 줄어드는 속도)")]
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

                // << : 유닛 테스트용 디펜스 모드 전환
                //      
                //      타이틀 씬 통해서 접근시
                //      아래 함수 게임매니저 통해 플레이어들 불러오도록
                //      플레이어 각각 NormalView함수 인풋 부분 수정(탑 스테이트로 바꾸는 부분)

                if (GameManager.Instance.isTopView)
                {
                    // >> : 기존
                    //pv.RPC(nameof(InitCamera), RpcTarget.AllBuffered, (int)CharacterCamera.NELLA); 
                    //pv.RPC(nameof(SetDefenceModeCamera), RpcTarget.AllBuffered);

                    // >> : 수정
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
            // >> : 기존
            //// 유닛 테스트 임시용이다.
            //// 타이틀 씬 통해서 접속시, 게임매니저 통해서 플레이어 받아온다

            //GameObject NellaTemp = GameObject.FindGameObjectWithTag("Nella");
            //GameObject SteadyTemp = GameObject.FindGameObjectWithTag("Steady");

            //if(NellaTemp)
            //    NellaTemp.GetComponent<CameraController>().SetDefenseMode();

            //if (SteadyTemp)
            //    SteadyTemp.GetComponent<CameraController>().SetDefenseMode();


            // >> : 수정
            if (GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient]) // 넬라면
            {
                //pv.RPC(nameof(InitCamera), RpcTarget.AllBuffered, (int)CharacterCamera.NELLA);
                InitCamera((int)CharacterCamera.NELLA);
                GameObject.FindGameObjectWithTag("Nella").GetComponent<CameraController>().SetDefenseMode();
            }
            else // 스테디라면
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

            if (isNella) // 넬라 카메라를 0.5 -> 0.36f , 스테디 카메라를 0.5 -> 0.64f
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
            else // 스테디 카메라를 0.5 -> 0.36f, 넬라 카메라를 0.5 -> 0.64f
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

        // 스테이지 3 디펜스 모드 : 넬라가 부활했을 때나, 스테디가 부활했을 때 호출
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

            if (isNella) // 넬라 카메라를 0.36 -> 0.5f
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
            else // 스테디 카메라를 0.36 -> 0.5f
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

            if (curFullCamera == CharacterCamera.STEADY) // 넬라 카메라 전체화면
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
                    // >> : 수정 
                    InitCamera((int)CharacterCamera.STEADY);


                }

                isBlending = false;
            }
            else if (curFullCamera == CharacterCamera.NELLA) // 스테디 카메라 전체화면
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

                    // >> : 수정 
                    InitCamera((int)CharacterCamera.NELLA);
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

            //Debug.Log("보간 시작!");
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

                    //Debug.Log(wd);
                    yield return null;
                }
                //Debug.Log("보간 종료!");

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