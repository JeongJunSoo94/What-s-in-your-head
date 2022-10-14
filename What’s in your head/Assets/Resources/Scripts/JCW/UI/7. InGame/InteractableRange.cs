using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


namespace JCW.UI.InGame
{
    public class InteractableRange : MonoBehaviour
    {
        [Header("상호작용 범위 수치")] [SerializeField] [Range(0, 100)] float range;

        [Header("비디오 플레이어")] [SerializeField] VideoPlayer videoPlayer;
        [Header("감지<->상호작용 스프라이트를 저장할 이미지")] [SerializeField] Image sourceImage;

        [Header("넬라 - 감지 & 상호작용 스프라이트 및 클립")]
        [SerializeField] Sprite     nella_DetectSprite;
        [SerializeField] Sprite     nella_InteractableSprite;
        [SerializeField] VideoClip  nella_ActiveClip;
        [SerializeField] VideoClip  nella_InactiveClip;
        [Header("스테디 - 감지 & 상호작용 스프라이트 및 클립")]
        [SerializeField] Sprite     steady_DetectSprite;
        [SerializeField] Sprite     steady_InteractableSprite;
        [SerializeField] VideoClip  steady_ActiveClip;
        [SerializeField] VideoClip  steady_InactiveClip;



        // 현재 캐릭터가 넬라인지
        bool isNella;
        private void Awake()
        {
            transform.localScale = new Vector3(range, range, range);

            // 정식으로 사용할 때엔 아래 코드 쓸것
            //isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];

            // 임시
            isNella = false;
        }

        // 지금은 임시로 트리거 Enter/Exit으로 하고 있지만
        // 정식으로 사용할 때엔 플레이어가 레이를 쏴서 거리에 따라 온 오프 시켜야함.
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.CompareTag("Nella") || other.gameObject.CompareTag("Steady"))
            {
                if (other.gameObject.GetComponent<PhotonView>().IsMine)
                    ConvertVideo(true);
            }
            
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Nella") || other.gameObject.CompareTag("Steady"))
            {
                Debug.Log("IsMine : " + other.gameObject.GetComponent<PhotonView>().IsMine);
                if (other.gameObject.GetComponent<PhotonView>().IsMine)
                    ConvertVideo(false);
            }
        }

        public void ConvertVideo(bool isActive = false)
        {            
            // 이미지를 잠깐 꺼주고 동영상 켜주기
            sourceImage.enabled = false;
            videoPlayer.gameObject.GetComponent<RawImage>().enabled = true;
            videoPlayer.Stop();
            StopCoroutine(nameof(PlayVideoClip));
            if (isNella)
                videoPlayer.clip = isActive ? nella_ActiveClip : nella_InactiveClip;
            else
                videoPlayer.clip = isActive ? steady_ActiveClip : steady_InactiveClip;

            StartCoroutine(nameof(PlayVideoClip), isActive);
        }

        IEnumerator PlayVideoClip(bool isActive)
        {
            videoPlayer.Play();
            yield return new WaitForSeconds(1f);
            videoPlayer.Stop();
            if (isNella)
                sourceImage.sprite = isActive ? nella_InteractableSprite : nella_DetectSprite;
            else
                sourceImage.sprite = isActive ? steady_InteractableSprite : steady_DetectSprite;

            // 동영상 끄고 이미지 켜주기
            videoPlayer.gameObject.GetComponent<RawImage>().enabled = false;
            sourceImage.enabled = true;


            yield return null;
        }
    }
}
