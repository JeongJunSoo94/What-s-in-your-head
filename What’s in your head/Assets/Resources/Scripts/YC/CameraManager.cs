using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YC.CameraManager_
{
    public class CameraManager : MonoBehaviour
    {
        enum Cameras { Left, Right };
  
        public Camera[] cameras;

        [Header("Right Camera Rect : Increase")]
        [SerializeField] bool toggle1;
        [Header("Left Camera Rect : Increase")]
        [SerializeField] bool toggle2;
        [Header("Right Camera Rect : Full")]
        [SerializeField] bool toggle3;
        [Header("Left Camera Rect : Full")]
        [SerializeField] bool toggle4;
        [Header("Right Camera : X -> O")]
        [SerializeField] bool toggle5;
        [Header("Left Camera : X -> O")]
        [SerializeField] bool toggle6;

        [Space]
        [Header("보간 시간")]
        [SerializeField] float lerpTime;

        float currentMilldeValue; // 현재 카메라 분할의 중앙선 값

        ///[SerializeField] bool test = true;
        ///[SerializeField] bool testToggle = false;

        private static CameraManager sInstance;
        public static CameraManager Instance
        {
            get
            {
                if (sInstance == null)
                {
                    GameObject newGameObject = new GameObject("_CameraManager");
                    sInstance = newGameObject.AddComponent<CameraManager>();
                }
                return sInstance;
            }
        }

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            cameras = new Camera[2];

            toggle1 = false;
            toggle2 = false;
            toggle3 = false;
            toggle4 = false;
            toggle5 = false;
            toggle6 = false;

            currentMilldeValue = 0.5f;
            lerpTime = 1.0f;
        }

        void Update()
        {
            if (cameras[0] == null || cameras[1] == null) return;

            CheckAndPlay();
            ///if (test) StartCoroutine("Test");
            ///if (testToggle)
            ///{
            ///    StartCoroutine(SizingCamera(0.5f, 0, 1.0f));
            ///}
            ///if (test)
            ///{
            ///    StartCoroutine(SizingCamera(0.5f, 0, 1.0f)); // 오른쪽 full
            ///    //StartCoroutine(SizingCamera(0.5f, 1, 1.0f)); // 왼쪽 full
            ///    test = false;
            ///}

        }

        void CheckAndPlay()
        {
            if (toggle1)
            {
                StartCoroutine(SizingCamera(Cameras.Right, currentMilldeValue, 0.2f, lerpTime));
                toggle1 = false;
            }
            else if (toggle2)
            {
                StartCoroutine(SizingCamera(Cameras.Left, currentMilldeValue, 0.7f, lerpTime));
                toggle2 = false;
            }
            else if (toggle3)
            {
                StartCoroutine(SizingCamera(Cameras.Right, currentMilldeValue, 0.0f, lerpTime));
                toggle3 = false;
            }
            else if (toggle4)
            {
                StartCoroutine(SizingCamera(Cameras.Left, currentMilldeValue, 1.0f, lerpTime));
                toggle4 = false;
            }
            else if (toggle5)
            {
                StartCoroutine(SizingCamera(Cameras.Right, currentMilldeValue, 0.5f, lerpTime));
                toggle5 = false;
            }
            else if (toggle6)
            {
                StartCoroutine(SizingCamera(Cameras.Left, currentMilldeValue, 0.5f, lerpTime));
                toggle6 = false;
            }
        }


        IEnumerator SizingCamera(Cameras TargetCamera, float startPoint, float endPoint, float lerptime) // 중간지점 -> 옮길지점
        {
            Rect camRect1 = cameras[0].rect;
            Rect camRect2 = cameras[1].rect;
            float currentTime = 0;

            if (TargetCamera == Cameras.Right && startPoint > endPoint) // 오른쪽 카메라 Up
            {
                while (camRect1.width > endPoint)
                {
                    camRect1 = cameras[0].rect;
                    camRect2 = cameras[1].rect;

                    currentTime += Time.deltaTime;
                    if (currentTime >= lerptime) currentTime = lerptime;

                    float wd = Mathf.Lerp(startPoint, endPoint, currentTime / lerptime);
                    if (wd < endPoint) wd = endPoint;
                    currentMilldeValue = wd;               
                    //Debug.Log("Debug - Camera Manager : " + wd);

                    Rect rc1 = new Rect(camRect1.x, camRect1.y, wd, camRect1.height);
                    cameras[0].rect = rc1;
                    Rect rc2 = new Rect(wd, camRect2.y, camRect2.width, camRect2.height);
                    cameras[1].rect = rc2;

                    yield return null;
                }
            }
            else if (TargetCamera == Cameras.Left && startPoint < endPoint) // 왼쪽 카메라 Up
            {
                while (camRect1.width < endPoint)
                {
                    camRect1 = cameras[0].rect;
                    camRect2 = cameras[1].rect;

                    currentTime += Time.deltaTime;
                    if (currentTime >= lerptime) currentTime = lerptime;

                    float wd = Mathf.Lerp(startPoint, endPoint, currentTime / lerptime);
                    if (wd > endPoint) wd = endPoint;
                    currentMilldeValue = wd;                    
                    //Debug.Log("Debug - Camera Manager : " + wd);

                    Rect rc1 = new Rect(camRect1.x, camRect1.y, wd, camRect1.height);
                    cameras[0].rect = rc1;
                    Rect rc2 = new Rect(wd, camRect2.y, camRect2.width, camRect2.height);
                    cameras[1].rect = rc2;

                    yield return null;
                }
            }
        }



        //// 2개의 카메라를 1개의 카메라로 합친다.
        //void CombineCamera(float startPoint, float endPoint)
        //{
        //    Rect camRect1 = cameras[0].rect;
        //    Rect camRect2 = cameras[1].rect;

        //    Debug.Log("진행중!");

        //    currentTime += Time.deltaTime;

        //    if (currentTime >= lerpTime) currentTime = lerpTime;


        //    if (camRect1.width > 0.0f)
        //    {
        //        float wd = Mathf.Lerp(startPoint, endPoint, currentTime / lerpTime);
        //        if (wd < 0.0f) wd = 0.0f;

        //        Rect rc1 = new Rect(camRect1.x, camRect1.y, wd, camRect1.height);
        //        cameras[0].rect = rc1;

        //        Rect rc2 = new Rect(wd, camRect2.y, camRect2.width, camRect2.height);
        //        cameras[1].rect = rc2;
        //    }   
        //}


        // 한쪽은 줄이고 한쪽은 늘리고 or 한쪽으로 병합







        //IEnumerator Test()
        //{
        //    test = false;

        //    yield return new WaitForSeconds(3.0f);

        //    testToggle = true;
        //}


    }
}
