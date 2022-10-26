using KSU;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YC.CameraManager_;

namespace JCW.UI.InGame
{
    [RequireComponent(typeof(PhotonView))]
    public class AimUI : MonoBehaviour
    {
        [Header("���� Ÿ���� �ӵ�")] [Range(0,5f)] [SerializeField] float targetingTime;
        Camera mainCamera;
        Camera curCam;
        RectTransform imgTransform;
        bool wasTopView;

        bool isNella;

        Transform targetTopviewTF;
        Transform targetTF;
        Transform playerTF;

        Image aimImage;

        bool isTargeting = false;
        bool isNormal = true;
        float curTargetTime = 0f;
        float curNormalTime = 0f;
        float detectAngle;
       
        private void Awake()
        {
            imgTransform = transform.GetChild(0).GetComponent<RectTransform>();
            if (!GetComponent<PhotonView>().IsMine)
            {
                Destroy(this.gameObject);
                return;
            }
            StartCoroutine(nameof(WaitForPlayer));
            playerTF = this.transform.parent;
            aimImage = imgTransform.GetComponent<Image>();

        }

        private void Update()
        {
            if (targetTopviewTF == null)
                return;
            aimImage.enabled = playerTF.GetComponent<PlayerController>().characterState.aim;
            // ���� ž���� ��
            if (GameManager.Instance.isTopView)
            {
                if (!wasTopView)
                {
                    wasTopView = true;
                    this.transform.parent = targetTopviewTF;

                    // ���� ķ�� ���� ��ҵ� ��� �ٸ� ķ���� ����.
                    if(curCam.rect.width <= 0.1f)
                        curCam = isNella ? CameraManager.Instance.cameras[1] : CameraManager.Instance.cameras[0];
                }
                imgTransform.position = curCam.WorldToScreenPoint(targetTopviewTF.position);                
            }
            // ������ ž�俴���� ������ �ƴ� ���
            else if (wasTopView)
            {
                wasTopView = false;
                this.transform.parent = playerTF;
                imgTransform.position = new Vector3((curCam.rect.x + curCam.rect.width + curCam.rect.x) * 960f, 540f, 0);
            }
            else
            {
                // Ÿ���� ����
                if(targetTF != null)
                {
                    isNormal = false;
                    curNormalTime = 0f;
                    if (!isTargeting)
                    {
                        imgTransform.position = Vector3.Lerp(imgTransform.position, curCam.WorldToScreenPoint(targetTF.position), Time.deltaTime * (detectAngle * 0.5f + 3.5f));
                        curTargetTime += Time.deltaTime;
                        if(curTargetTime >= targetingTime)
                        {
                            curTargetTime = 0f;
                            isTargeting = true;
                        }
                    }
                    else
                        imgTransform.position = curCam.WorldToScreenPoint(targetTF.position);
                }
                else
                {
                    isTargeting = false;
                    curTargetTime = 0f;
                    if(!isNormal)
                    {
                        imgTransform.position = Vector3.Lerp(imgTransform.position, new Vector3((curCam.rect.x + curCam.rect.width + curCam.rect.x) * 960f, 540f, 0), Time.deltaTime * (detectAngle*0.5f+3.5f));
                        curNormalTime += Time.deltaTime;
                        if (curNormalTime >= targetingTime)
                        {
                            curNormalTime = 0f;
                            isNormal = true;
                        }
                    }
                    else
                        imgTransform.position = new Vector3((curCam.rect.x + curCam.rect.width + curCam.rect.x) * 960f, 540f, 0);
                }
            }
        }

        public void SetTarget(Transform posTF, float angle)
        {
            targetTF = posTF;
            detectAngle = angle;
        }

        protected IEnumerator WaitForPlayer()
        {
            while (GameManager.Instance.characterOwner.Count <= 1)
                yield return new WaitForSeconds(0.2f);

            isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];
            targetTopviewTF = isNella ? GameObject.FindWithTag("NellaMousePoint").transform : GameObject.FindWithTag("SteadyMousePoint").transform;

            mainCamera = isNella ? CameraManager.Instance.cameras[0] : CameraManager.Instance.cameras[1];
            curCam = mainCamera;
            imgTransform.position = new Vector3((curCam.rect.x + curCam.rect.width + curCam.rect.x) * 960f, 540f, 0);
            targetTF = null;
            yield break;
        }
    }
}

