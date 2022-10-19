using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace YC.CameraManager_
{
    public enum CharacterCamera { NELLA, STEADY};
    [RequireComponent(typeof(PhotonView))]
    public class CameraManager : MonoBehaviour
    {
        [HideInInspector] public Camera[] cameras;

        public bool isBlending { get; private set; }

        PhotonView pv;
        public static CameraManager Instance = null;

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

        }

        void Update()
        {
            if (cameras[(int)CharacterCamera.NELLA] == null || cameras[(int)CharacterCamera.STEADY] == null) return;

            if(pv.IsMine) CheckAndPlay();

            //Debug.Log(isBlending);
        }

        void CheckAndPlay()
        {
            // 초기 아무 전체화면 (1 or 2)
            // 넬라 죽음(3) -> 살아남(2)
            // 스테디 죽음(4) -> 살아남 (1)

            if (isBlending) return;

            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                //Debug.Log("넬라 카메라 전체화면");
                pv.RPC(nameof(Cor_SetFullScreen), RpcTarget.AllBuffered, 0, 2f);                 
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                //Debug.Log("스테디 카메라 전체화면");
                pv.RPC(nameof(Cor_SetFullScreen), RpcTarget.AllBuffered, 1, 2f);   
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                //Debug.Log("(넬라가 죽었을때) 스테디 카메라 전체화면을 만들어주고, 넬라 카메라 만든다");
                pv.RPC(nameof(InitCamera), RpcTarget.AllViaServer, 1);
                pv.RPC(nameof(Cor_SetSizeCamera), RpcTarget.AllViaServer, 0, 0.36f, 2f);           
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                //Debug.Log("(스테디가 죽었을때) 넬라 카메라 전체화면을 만들어주고, 스테디 카메라 만든다");
                pv.RPC(nameof(InitCamera), RpcTarget.AllBuffered, 0);
                pv.RPC(nameof(Cor_SetSizeCamera), RpcTarget.AllBuffered, 1, 0.64f, 2f);               
            }
        }
        public void NellaDeadCam()
        {
            pv.RPC(nameof(InitCamera), RpcTarget.AllViaServer, 1);
            pv.RPC(nameof(Cor_SetSizeCamera), RpcTarget.AllViaServer, 0, 0.36f, 2f);
        }

        public void SteadyDeadCam()
        {
            pv.RPC(nameof(InitCamera), RpcTarget.AllBuffered, 0);
            pv.RPC(nameof(Cor_SetSizeCamera), RpcTarget.AllBuffered, 1, 0.64f, 2f);
        }

        


        // Sizing 호출 전 초기화 함수
        [PunRPC]
        public void InitCamera(int targetCamera)
        {
            if (targetCamera == 0) // 넬라 카메라 전체화면
            {
                Rect rc1 = new Rect(0, 0, 1, 1);
                cameras[(int)CharacterCamera.NELLA].rect = rc1;
                Rect rc2 = new Rect(1, 0, 0, 1);
                cameras[(int)CharacterCamera.STEADY].rect = rc2;
            }
            else if (targetCamera == 1) // 스테디 카메라 전체화면
            {
                Rect rc1 = new Rect(0, 0, 0, 1);
                cameras[(int)CharacterCamera.NELLA].rect = rc1;
                Rect rc2 = new Rect(0, 0, 1, 1);
                cameras[(int)CharacterCamera.STEADY].rect = rc2;
            }
        }


        // 현재 카메라 상황에서 lerpTime동안 targetCamera를 전체화면으로 세팅한다
        [PunRPC]
        public void Cor_SetFullScreen(int targetCamera, float lerpTime)
        {
            StartCoroutine(SetFullScreen(targetCamera, lerpTime));
        }
        public IEnumerator SetFullScreen(int targetCamera, float LerpTime) 
        {           
            Rect camRect1 = cameras[(int)CharacterCamera.NELLA].rect;
            Rect camRect2 = cameras[(int)CharacterCamera.STEADY].rect;
            float currentTime = 0;

            isBlending = true;

            if (targetCamera == 0) // 넬라 카메라 전체화면
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
            else if (targetCamera == 1) // 스테디 카메라 전체화면
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

        // 전체화면 상황에서, targetCamera를 targetCamera까지 lerpTime동안 늘린다
        [PunRPC] 
        public void Cor_SetSizeCamera(int targetCamera, float middleValue, float lerpTime)
        {           
            StartCoroutine(SetSizeCamera(targetCamera, middleValue, lerpTime));
        }
        public IEnumerator SetSizeCamera(int targetCamera, float middleValue, float LerpTime) 
        {
            Rect camRect1 = cameras[(int)CharacterCamera.NELLA].rect;
            Rect camRect2 = cameras[(int)CharacterCamera.STEADY].rect;
            float currentTime = 0;

            isBlending = true;
       
            if (targetCamera == 0) // 스테디 전체화면 상황에서, 넬라 카메라를 만들어줌(늘려줌)
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
            else if (targetCamera == 1) // 넬라 전체화면 상황에서, 스테디 카메라를 만들어줌(늘려줌)
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
    }     
}