using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using System.Linq;

using Cinemachine;

using JCW.UI.Options.InputBindings;
using YC.CameraManager_;
using UnityEngine.Rendering.Universal;

using KSU;

namespace YC.Camera_
{
    public class CameraController : MonoBehaviour, IPunObservable
    {

        // Photon 
        PhotonView pv;

        // Player Cam
        //public Camera mainCam;
        public Camera mainCam { get; private set; }

        // Cinemachine
        public CinemachineBrain cinemachineBrain;
        List<CinemachineVirtualCameraBase> camList;

        CinemachineVirtualCameraBase backCam;
        CinemachineVirtualCameraBase sholderCam;
        CinemachineVirtualCameraBase topCam;

        enum CamState { back, sholder, top, };
        CamState curCam;
        CamState preCam;

        CamState curCam_Clone;

        [Header("[Back View ī�޶� ���콺 ����]")]
        [SerializeField] [Range(0, 100)] float backView_MouseSensitivity;
        [Header("[Back View ī�޶� ���콺 ����]")]
        [SerializeField] [Range(0, 100)] float sholderView_MouseSensitivity;


        PlayerController player;

        float sholderViewMaxY;
        [Header("[Sholder View Y�˵� Up ���� ��]")]
        [SerializeField] [Range(0, 1)] float sholderAxisY_MaxUp = 0.3f;
        [Header("[Sholder View Y�˵� Down ���� ��]")]
        [SerializeField] [Range(0, 1)] float sholderAxisY_MaxDown = 0.5f;


        [Header("[���׵� ��, ī�޶� ��鸲 ���� ũ��]")]
        [SerializeField] [Range(0, 5)] float AmplitudeGain = 3f;
        [Header("[���׵� ��, ī�޶� ��鸲 ��]")]
        [SerializeField] [Range(0, 5)] float FrequebctGain = 3f;
        //bool isInitCamera = false;
        List<CinemachineBasicMultiChannelPerlin> listCBMCP;

        bool wasShaked = false;
        bool isShakedFade = true;

        [Header("[�ڶ� �ó׸ӽ� ���]")]
        [SerializeField] CinemachineVirtualCameraBase CineNellaBack;
        [Header("[�ڶ� �ó׸ӽ� �����]")]
        [SerializeField] CinemachineVirtualCameraBase CineNellaSholder;
        [Header("[���׵� �ó׸ӽ� ���]")]
        [SerializeField] CinemachineVirtualCameraBase CineSteadyBack;
        [Header("[���׵� �ó׸ӽ� �����]")]
        [SerializeField] CinemachineVirtualCameraBase CineSteadySholder;


        //[Header("[�Ϲ� ���� ��, ���� ������ ���� �뽬������ ���� �ð�]")]
        float normalJumpLerpTime = 0.2f;


        PlayerState playerState;


        float backViewFOV = 40;


        // >> : ������ Follow, LookAt ����
        Transform followObj;
        Transform lookatBackObj;
        Transform lookatSholderObj;
        float lookatObjOriginY;

        

        [SerializeField] bool isJumping = false;

        float orgLookY;
        float orgFollowY;

        float originLocalFollowY;
        float originLocalLookAtY;


        public bool canShake = true; // �ɼ� ���׵� ��鸲 ����



        void Awake()
        {
            pv = GetComponent<PhotonView>();

            if (pv) pv.ObservedComponents.Add(this);

            camList = new List<CinemachineVirtualCameraBase>();

            InitInstantiate();

            curCam = new CamState();
            preCam = new CamState();



            if (!pv.IsMine)
            {
                //curCam_Clone = CamState.back;
                //OnOffCamera(camList[(int)curCam_Clone]);
            }
            else
            {
                sholderViewMaxY = sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxValue;

                curCam = CamState.back;
                preCam = curCam;
                OnOffCamera(camList[(int)curCam]);
            }

            FindCamera();

            player = this.gameObject.GetComponent<PlayerController>();


            // ī�޶� FOV �ʱ�ȭ (�ش� �ó׸ӽ� ī�޶�� Commone Lens ���� �ʿ�)
            //if (pv.IsMine)
            //    mainCam.fieldOfView = camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView;
            //else
            //    mainCam.fieldOfView = camList[(int)curCam_Clone].GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView;

            mainCam.fieldOfView = backViewFOV;

            // ���� ���̼� ��Ƽ �����ߴµ� Nella Remote ���� ������ Steady Owner�� ���콺 ���� Nella Remote���� ���´ٸ�
            // �þ߰��� ī�޶󳢸��� ���� �Ұ�, �þ߰��� ���� ���� ���� �ʿ�
            if (!pv.IsMine) cinemachineBrain.enabled = false;

            listCBMCP = new List<CinemachineBasicMultiChannelPerlin>();

            InitDefault();

            playerState = this.gameObject.GetComponent<PlayerState>();

        }


        void FixedUpdate()
        {
            if (!pv.IsMine) return;

            if (isJumping && !isAirJump)
                NormalJumpCameraSet();

            SetCamera();
            SetAimYAxis();

            //if (!playerState.IsGrounded && !timer)
            //{
            //    StartCoroutine(Timer());
            //}
            if (player.characterState.IsJumping)
                DetectPlayerInScreen();

            if (player.characterState.IsJumping && isAirJump && endCor)
                AirJumpCameraSet();

            //Debug.Log(followObj.transform.position.y * 100);
        }

        bool wasLerped = false;
        bool timer = false;



        bool endCor = false;
        bool isCor = false;

        bool isJumpAfterSet = false;

        bool isJumpingMin = false;
        void DetectPlayerInScreen()
        {
            
            if (isJumpAfterSet) return;

            if (player.transform.position.y < followObj.transform.position.y) // ���� �÷��̾��� ���̰�, ���� ���� ���� ���̺��� ���ٸ�
            {
                //Debug.Log("ȣ��!");

                lookatBackObj.position = new Vector3(lookatBackObj.transform.position.x, player.transform.position.y + lookatObjOriginY, lookatBackObj.transform.position.z);
                followObj.position = new Vector3(followObj.transform.position.x, player.transform.position.y, followObj.transform.position.z);
                isJumpAfterSet = true;
                isJumpingMin = true;
            }
        }


        bool isAirJump = false;
        void AirJumpCameraSet()
        {
            //Debug.Log("ȣ�� - ���� ���� �ڷ�ƾ �� ī�޶� ����");
            lookatBackObj.position = new Vector3(lookatBackObj.transform.position.x, player.transform.position.y + lookatObjOriginY, lookatBackObj.transform.position.z);
            followObj.position = new Vector3(followObj.transform.position.x, player.transform.position.y, followObj.transform.position.z);
        }



        public void Option_SetShake(bool on)
        {
            canShake = on;
        }

        void InitInstantiate()
        {
            if (!pv.IsMine) return;

            if (this.gameObject.CompareTag("Nella"))
            {
                backCam = Instantiate(CineNellaBack, Vector3.zero, Quaternion.identity).GetComponent<CinemachineVirtualCameraBase>();
                sholderCam = Instantiate(CineNellaSholder, Vector3.zero, Quaternion.identity).GetComponent<CinemachineVirtualCameraBase>();

            }
            else
            {
                backCam = Instantiate(CineSteadyBack, Vector3.zero, Quaternion.identity).GetComponent<CinemachineVirtualCameraBase>();
                sholderCam = Instantiate(CineSteadySholder, Vector3.zero, Quaternion.identity).GetComponent<CinemachineVirtualCameraBase>();
              
            }

            followObj = transform.Find("Cine_followObj").gameObject.transform;
            lookatBackObj = transform.Find("Cine_lookatObj_Back").gameObject.transform;
            lookatObjOriginY = lookatBackObj.transform.position.y;
            lookatSholderObj = transform.Find("Cine_lookatObj_Sholder").gameObject.transform;

       

            backCam.GetComponent<CinemachineFreeLook>().m_Follow = followObj;
            backCam.GetComponent<CinemachineFreeLook>().m_LookAt = lookatBackObj;

            sholderCam.GetComponent<CinemachineFreeLook>().m_Follow = followObj;
            sholderCam.GetComponent<CinemachineFreeLook>().m_LookAt = lookatSholderObj;

            originLocalFollowY = followObj.transform.localPosition.y;
            originLocalLookAtY = lookatBackObj.transform.localPosition.y;


            camList.Add(backCam);
            camList.Add(sholderCam);
        }
        void InitDefault()
        {
            if (!pv.IsMine) return;
            // backView_MouseSensitivity, sholderView_MouseSensitivity �� ������ 1~100�� ���� ���´� (������ ���ӸŴ����� ���� �޾ƿ�)
            // �ó׸ӽſ��� ������ ���콺 ���ǵ�� X : 100 ~ 300, Y : 1 ~ 3 �����̴�.
            // ���� ������ �ó׸ӽſ� �°� ��ȯ�Ͽ� �������ش�       

            // << : ���� ����
            Option_SetSensitivity(backView_MouseSensitivity, sholderView_MouseSensitivity);


            // << : ����� Y�� ����
            if (sholderAxisY_MaxUp == 0) sholderAxisY_MaxUp = 0.2f;
            if (sholderAxisY_MaxDown == 0) sholderAxisY_MaxDown = 0.5f;


            // << : ���׵� �� ��鸲 ���� ����

            if (AmplitudeGain == 0) AmplitudeGain = 1;
            if (FrequebctGain == 0) FrequebctGain = 2;

            if (this.gameObject.CompareTag("Steady"))
            {
                for (int i = 0; i < 3; ++i)
                {
                    listCBMCP.Add(sholderCam.GetComponent<CinemachineFreeLook>().GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>());
                }

                for (int i = 0; i < 3; ++i)
                {
                    sholderCam.GetComponent<CinemachineFreeLook>().GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
                    sholderCam.GetComponent<CinemachineFreeLook>().GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
                }
            }
        }

        public void Option_SetSensitivity(float backSensitivity, float sholderSensitivity)
        {
            // << : ���� ����
            int defaulyX = 100;
            int defaultY = 1;

            if (backView_MouseSensitivity == 0) backSensitivity = 25;

            backCam.GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = defaulyX + backSensitivity * 4;
            backCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = defaultY + backSensitivity * 0.04f;

            if (sholderView_MouseSensitivity == 0) sholderSensitivity = 25;

            sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = defaulyX + sholderSensitivity * 4;
            sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = defaultY + sholderSensitivity * 0.04f;
        } // ���� ���� �Ŵ������� ȣ�� �ϵ���

        void SetAimYAxis() // sholder View���� YAxis Limit ����.
        {
            if (curCam == CamState.sholder)
            {
                AxisState axisY = camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis;

                if (axisY.Value <= sholderAxisY_MaxUp) // Ŀ���� Max ���� �Ѿ
                {
                    // axisY.m_InputAxisValue : Ŀ�� ��(1) ~ �Ʒ�(0)
                    // axisY.Value : Ŀ�� ��(-) ~ �Ʒ� (+)

                    if (axisY.m_InputAxisValue > 0)
                    {
                        axisY.m_MaxSpeed = 0;
                    }
                    else if (axisY.m_InputAxisValue < 0)
                    {
                        axisY.m_MaxSpeed = sholderViewMaxY;

                    }
                }
                else if (axisY.Value >= sholderAxisY_MaxDown) // Ŀ���� Min ������ ������.
                {
                    if (axisY.m_InputAxisValue > 0)
                    {
                        axisY.m_MaxSpeed = sholderAxisY_MaxDown;

                    }
                    else if (axisY.m_InputAxisValue < 0)
                    {
                        axisY.m_MaxSpeed = 0;
                    }
                }
                else
                {
                    axisY.m_MaxSpeed =
                        sholderCam.GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = 1 + backView_MouseSensitivity * 0.02f;
                }
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis = axisY;
            }
        }

        void SetCamera() // �÷��̾� State ����ī�޶� ���� 
        {
            if (GameManager.Instance.isTopView)
                return;


            if (curCam == CamState.back)
            {
                if (player.characterState.aim) // back View -> sholder View
                {
                    AxisState temp = backCam.GetComponent<CinemachineFreeLook>().m_XAxis;

                    preCam = curCam;
                    curCam = CamState.sholder;

                    if (camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value < sholderAxisY_MaxUp)
                        camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value = sholderAxisY_MaxUp;

                    if (camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value > sholderAxisY_MaxDown)
                        camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value = sholderAxisY_MaxDown;


                    camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.Value = camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_XAxis.Value;
                    camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value = camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value;
                    OnOffCamera(sholderCam);

                    sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis = temp;
                }
            }
            else if (curCam == CamState.sholder)
            {
                if (!player.characterState.aim) // sholder View -> back View
                {
                    AxisState temp = sholderCam.GetComponent<CinemachineFreeLook>().m_XAxis;

                    preCam = curCam;
                    curCam = CamState.back;

                    camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.Value = camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_XAxis.Value;
                    camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value = camList[(int)preCam].GetComponent<CinemachineFreeLook>().m_YAxis.Value;

                    if (isShakedFade)
                    {
                        StopCoroutine("ShakeFadeOut");
                        InitShakeFade();
                    }

                    OnOffCamera(backCam);

                    backCam.GetComponent<CinemachineFreeLook>().m_XAxis = temp;
                }
            }
        }

        public void SetDefenseMode()
        {
            if (pv.IsMine)
            {
                topCam = GameObject.FindWithTag("Cine_DefenseCam").GetComponent<CinemachineVirtualCameraBase>();
                topCam.GetComponent<CinemachineVirtualCamera>().enabled = true;
                camList.Add(topCam);

                preCam = curCam;
                curCam = CamState.top;

                OnOffCamera(topCam);

                camList[(int)curCam].GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = 60;
            }
            else
            {
                mainCam.fieldOfView = 60;
            }
        }

        // << : ���׵� �� ����, ���׵� Aim Attack State���� ȣ��
        public void SetSteadyBeam(bool isLock)
        {
            // << : ž���� ����(�ؼ���)
            if (GameManager.Instance.isTopView) return;

            if (!pv.IsMine) return;

            if (isLock)
            {
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.m_InputAxisName = "";
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.m_InputAxisName = "";

                // << : ȸ���ϸ鼭 �� ���� ��� ���� ����
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.m_InputAxisValue = 0;
            }
            else
            {
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_XAxis.m_InputAxisName = "Mouse X";
                camList[(int)curCam].GetComponent<CinemachineFreeLook>().m_YAxis.m_InputAxisName = "Mouse Y";
            }
        }



        float avgDif = 0.5f;
        float totDif = 0;
        float originPlayerY; // �÷��̾ �����ϱ� �� Y ������


        
       
        
        public void NormalJumpCameraInit(bool On)
        {
            
            if (On) // �Ϲ� ���� ����
            {
                isJumping = true;
                isJumpAfterSet = false;
                isJumpingMin = false;
                isAirJump = false;
                orgLookY = lookatBackObj.transform.position.y;
                orgFollowY = followObj.transform.position.y;
                originPlayerY = player.transform.position.y;

                //Debug.Log("ī�޶� - �Ϲ����� ����");

            }
            else if (!On && isJumping)
            {
                if (isJumpAfterSet) return;
                

                //Debug.Log("����");
                if (playerState.IsJumping && playerState.IsAirJumping && !playerState.IsAirDashing) // ���� ����
                {
                    Debug.Log("ȣ�� - ���� ���� ���� ==============================================");
                    if (player.transform.position.y < lookatBackObj.transform.position.y) // ��� ������Ʈ���� ���� ��ġ���� ���� ������ �õ��Ѵٸ� ���
                    {
                        
                        //Debug.Log("�Ա���1");
                        //Debug.Log("Player Y : " + player.transform.position.y);
                        //Debug.Log("Look Y : " + lookatBackObj.transform.position.y);
                        return;
                    }
                    isAirJump = true;
                    isJumping = false;
                    
                    float lerpTime = 2f;
                    StartCoroutine(LerpAfter(lerpTime));
                }
                else
                {
                    
                    //Debug.Log("Follow : " + followObj.transform.position.y);
                    //Debug.Log("origin : " + orgFollowY);
                    //Debug.Log("ī�޶� - �Ϲ����� ����");

                    float curDif = Mathf.Abs(player.transform.position.y - originPlayerY);  // ���� ���� Y��ġ��, ���� ���� Y��ġ�� ��


                    if (curDif > avgDif)
                    {
                        isJumping = false;
                        totDif = 0;

                        float lerpTime = 0.7f;
                        if (!isJumpAfterSet)
                        {
                            StartCoroutine(LerpAfter(lerpTime));
                        }
                      
                        return;
                    }
                    else
                    {
                        lookatBackObj.position = new Vector3(lookatBackObj.transform.position.x, player.transform.position.y + lookatObjOriginY, lookatBackObj.transform.position.z);
                        followObj.position = new Vector3(followObj.transform.position.x, player.transform.position.y, followObj.transform.position.z);
                        isJumping = false;
                    }    
                    //if (playerState.IsJumping && playerState.IsAirJumping && playerState.IsAirDashing) return;
                    //lookatBackObj.localPosition = new Vector3(0, originLocalLookAtY, 0);
                    //followObj.localPosition = new Vector3(0, originLocalFollowY, 0);
                }
            }

            
        }
       

        public void NormalJumpCameraSet()
        {
            //Debug.Log("ȣ�� - �Ϲ� ���� ����");
            if (isJumpingMin || isAirJump) return;

            float lookValue = orgLookY - lookatBackObj.transform.position.y;
            float followValue = orgFollowY - followObj.transform.position.y;

            lookatBackObj.position = 
                        new Vector3(lookatBackObj.transform.position.x, 
                                    lookatBackObj.position.y + lookValue, 
                                    lookatBackObj.transform.position.z);
            followObj.position = 
                        new Vector3(followObj.transform.position.x, 
                                    followObj.position.y + followValue, 
                                    followObj.transform.position.z);
        }
        

        IEnumerator LerpJump(float LerpTime)
        {
            Debug.Log("ȣ�� - ���� �ڷ�ƾ! ==============================================");
            Debug.Log("ȣ�� Ȯ�ο� : " + followObj.position.y * 100);
            float currentTime = 0;

            float initYpos = originPlayerY; // ���� ���۽� �÷��̾� Y ��
            float lerpYpos;  // ������ �̷������ �ִ� y ��
            endCor = false;

            float delayTime = 0.1f;
            yield return new WaitForSeconds(delayTime);

           
       
            lerpYpos = initYpos;
            while (lerpYpos < player.transform.position.y)
            {
                float curPlayerYpos = player.transform.position.y;

                currentTime += Time.deltaTime;
                if (currentTime >= LerpTime) currentTime = LerpTime;

                // << : ���� ���� - SmoothStep
                float t = currentTime / LerpTime;
                t = Mathf.Sin(t * Mathf.PI * 0.5f);

                lerpYpos = Mathf.Lerp(initYpos, curPlayerYpos, t);
                if (lerpYpos > curPlayerYpos) lerpYpos = curPlayerYpos;


                lookatBackObj.position = new Vector3(lookatBackObj.transform.position.x, lerpYpos + lookatObjOriginY, lookatBackObj.transform.position.z);
                followObj.position = new Vector3(followObj.transform.position.x, lerpYpos, followObj.transform.position.z);

                yield return null;
            }
            lookatBackObj.position = new Vector3(lookatBackObj.transform.position.x, player.transform.position.y + lookatObjOriginY, lookatBackObj.transform.position.z);
            followObj.position = new Vector3(followObj.transform.position.x, player.transform.position.y, followObj.transform.position.z);
                  
            endCor = true;
            isJumpAfterSet = true;
        }
     

        IEnumerator LerpAfter(float LerpTime) // ������ �̷���� ��, ������ Player Pos�� Y���� ���� �� Player Pos �� Y���� ������ ������ ������, ������ ����� �ð����� ������ �̷������
        {
            Debug.Log("ȣ�� - ���� �ڷ�ƾ! ==============================================");
            Debug.Log("ȣ�� Ȯ�ο� : " + followObj.position.y * 100);
            float currentTime = 0;

            float initYpos = originPlayerY; // ���� ���۽� �÷��̾� Y ��
            float lerpYpos;  // ������ �̷������ �ִ� y ��
            endCor = false;

            float delayTime = 0.1f;
            yield return new WaitForSeconds(delayTime);
            
            if (initYpos < player.transform.position.y)
            {
                //Debug.Log("ī�޶� - ��� ���� ���� ����!");
                //Debug.Log("ī�޶� - ���� �� �÷��̾� ������ : " + initYpos);
                //Debug.Log("ī�޶� - ���� �� �÷��̾� ������ : " + player.transform.position.y);
                //Debug.Log("======================================================");
                //Debug.Log("ī�޶� - ���� �� ������ look : " + lookatBackObj.position.y);
                //Debug.Log("ī�޶� - ���� �� ������ follow : " + followObj.position.y);
                //Debug.Log("======================================================");

                lerpYpos = initYpos;
                while (lerpYpos < player.transform.position.y)
                {
                    float curPlayerYpos = player.transform.position.y;

                    currentTime += Time.deltaTime;
                    if (currentTime >= LerpTime) currentTime = LerpTime;

                    // << : ���� ���� - SmoothStep
                    float t = currentTime / LerpTime;
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);

                    lerpYpos = Mathf.Lerp(initYpos, curPlayerYpos, t);
                    if (lerpYpos > curPlayerYpos) lerpYpos = curPlayerYpos;

                    //Debug.Log(lerpYpos);

                    lookatBackObj.position = new Vector3(lookatBackObj.transform.position.x, lerpYpos + lookatObjOriginY, lookatBackObj.transform.position.z);
                    followObj.position = new Vector3(followObj.transform.position.x, lerpYpos, followObj.transform.position.z);

                    yield return null;
                }
                lookatBackObj.position = new Vector3(lookatBackObj.transform.position.x, player.transform.position.y + lookatObjOriginY, lookatBackObj.transform.position.z);
                followObj.position = new Vector3(followObj.transform.position.x, player.transform.position.y, followObj.transform.position.z);
                //Debug.Log("======================================================");
                //Debug.Log("ī�޶� - ���� �� ������ look : " + lookatBackObj.position.y);
                //Debug.Log("ī�޶� - ���� �� ������ follow : " + followObj.position.y);

            }   
            else
            {
                //Debug.Log("ī�޶� - �ϰ� ���� ���� ����!");
                //Debug.Log("ī�޶� - ���� �� �÷��̾� ������ : " + initYpos);
                //Debug.Log("ī�޶� - ���� �� �÷��̾� ������ : " + player.transform.position.y);
                //Debug.Log("======================================================");
                //Debug.Log("ī�޶� - ���� �� ������ look : " + lookatBackObj.position.y);
                //Debug.Log("ī�޶� - ���� �� ������ follow : " + followObj.position.y);
                //Debug.Log("======================================================");
                                
                lerpYpos = initYpos;
                while (lerpYpos > player.transform.position.y)
                {
                    float curPlayerYpos = player.transform.position.y;

                    currentTime += Time.deltaTime;
                    if (currentTime >= LerpTime) currentTime = LerpTime;

                    // << : ���� ���� - SmoothStep
                    float t = currentTime / LerpTime;
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);

                    lerpYpos = Mathf.Lerp(initYpos, curPlayerYpos, t);
                    if (lerpYpos < curPlayerYpos) lerpYpos = curPlayerYpos;

                    //Debug.Log(lerpYpos);
                    lookatBackObj.position = new Vector3(lookatBackObj.transform.position.x, lookatObjOriginY + lerpYpos, lookatBackObj.transform.position.z);
                    followObj.position = new Vector3(followObj.transform.position.x, lerpYpos, followObj.transform.position.z);

                    //Debug.Log("ī�޶� - ���� �� ������ look : " + lookatBackObj.position.y);
                    //Debug.Log("ī�޶� - ���� �� ������ follow : " + followObj.position.y);
                    yield return null;
                }
                lookatBackObj.position = new Vector3(lookatBackObj.transform.position.x, player.transform.position.y + lookatObjOriginY, lookatBackObj.transform.position.z);
                followObj.position = new Vector3(followObj.transform.position.x, player.transform.position.y, followObj.transform.position.z);
                //Debug.Log("======================================================");
                //Debug.Log("ī�޶� - ���� �� ������ look : " + lookatBackObj.position.y);
                //Debug.Log("ī�޶� - ���� �� ������ follow : " + followObj.position.y);

            }
            endCor = true;
            isJumpAfterSet = true;
        }

        

        public void ShakeCamera(bool isShake) // MagnifyingGlass ���� ����޽����� ȣ��
        {
            if (!canShake) return;

            if (isShake)
            {
                if (wasShaked) return;

                foreach(CinemachineBasicMultiChannelPerlin CBMCP in listCBMCP)
                {
                    CBMCP.m_AmplitudeGain = AmplitudeGain;
                    CBMCP.m_FrequencyGain = FrequebctGain;
                }
                wasShaked = true;
            }
            else
            {              
                if (!wasShaked) return;

                wasShaked = false;

                isShakedFade = true;

                float fadeLerpTime = 0.3f;

                StartCoroutine(ShakeFadeOut(fadeLerpTime));
            }          
        }

        IEnumerator ShakeFadeOut(float LerpTime)
        {
            float initialVlaue = listCBMCP[0].m_AmplitudeGain;
            float currentTime = 0;

            while (initialVlaue > 0)
            {
                initialVlaue -= Time.deltaTime * (AmplitudeGain);

                currentTime += Time.deltaTime;
                if (currentTime >= LerpTime) currentTime = LerpTime;

                float curValue = Mathf.Lerp(initialVlaue, 0, currentTime / LerpTime);

                if (curValue < 0)
                    curValue = 0;

                foreach (CinemachineBasicMultiChannelPerlin CBMCP in listCBMCP)
                {
                    CBMCP.m_AmplitudeGain = curValue;
                    CBMCP.m_FrequencyGain = curValue;
                }

                yield return null;
            }
            isShakedFade = false;
        }

        void InitShakeFade()
        {
            float initialVlaue = 0;

            foreach (CinemachineBasicMultiChannelPerlin CBMCP in listCBMCP)
            {
                CBMCP.m_AmplitudeGain = initialVlaue;
                CBMCP.m_FrequencyGain = initialVlaue;
            }
            isShakedFade = false;
        }

        void OnOffCamera(CinemachineVirtualCameraBase curCam) // �Ű������� ���� ī�޶� �ܿ� �ٸ� ī�޶�� ��
        {
            foreach (CinemachineVirtualCameraBase cam in camList)
            {
                if (cam == curCam)
                {
                    curCam.enabled = true;
                    continue;
                }
                cam.enabled = false;
            }
        }

        public Camera FindCamera() // ĳ���Ϳ� ���� �ڱ� ī�޶� ã�� ����
        {
            if (this.gameObject.CompareTag("Nella"))
            {           
                if(!mainCam)
                {
                    mainCam = GameObject.FindGameObjectWithTag("NellaCamera").GetComponent<Camera>();
                    mainCam.GetComponent<UniversalAdditionalCameraData>().SetRenderer(0);

                    cinemachineBrain = mainCam.GetComponent<CinemachineBrain>();
                    CameraManager.Instance.cameras[0] = mainCam;
                    return mainCam;
                }
                else
                {
                    return mainCam;
                }
            }
            else if (this.gameObject.CompareTag("Steady"))
            {            
                if (!mainCam)
                {
                    mainCam = GameObject.FindGameObjectWithTag("SteadyCamera").GetComponent<Camera>();
                    mainCam.GetComponent<UniversalAdditionalCameraData>().SetRenderer(1);
                    cinemachineBrain = mainCam.GetComponent<CinemachineBrain>();
                    CameraManager.Instance.cameras[1] = mainCam;
                    return mainCam;
                }
                else
                {
                    return mainCam;
                }
            }
            return mainCam;
        }     

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // Photon q
        {
            //if (stream.IsWriting) {}
            //else {}
        }
    }
}
