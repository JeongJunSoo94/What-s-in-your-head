using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoConverter : MonoBehaviour
{
    [Header("감지 범위 수치")] [SerializeField] [Range(0, 100)] float range;
    [Header("오브젝트")] [SerializeField] GameObject obj;


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

    VideoPlayer videoPlayer;

    // 감지<->상호작용 스프라이트를 저장할 이미지
    Image sourceImage;

    // 현재 캐릭터가 넬라인지
    bool isNella;
    private void Awake()
    {
        videoPlayer = transform.GetComponent<VideoPlayer>();
        transform.localScale = new Vector3(range, range, range);
        sourceImage = transform.parent.gameObject.GetComponent<Image>();
        //isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];
        isNella = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        ConvertVideo(true);
    }

    private void OnTriggerExit(Collider other)
    {
        ConvertVideo(false);
    }

    void ConvertVideo(bool isActive = false)
    {
        sourceImage.enabled = false;
        videoPlayer.gameObject.GetComponent<RawImage>().enabled = true;
        if (isNella)
            videoPlayer.clip = isActive ? nella_ActiveClip : nella_InactiveClip;
        else
            videoPlayer.clip = isActive ? steady_ActiveClip : steady_InactiveClip;

        StartCoroutine(nameof(PlayVideoClip), isActive);
    }

    IEnumerator PlayVideoClip(bool isActive)
    {
        videoPlayer.Stop();
        videoPlayer.Play();
        //yield return new WaitForEndOfFrame();
        ////yield return new WaitForSeconds(0.01f);
        //while (videoPlayer.isPlaying)
        //{
        //    yield return new WaitForSeconds(0.05f);
        //}
        yield return new WaitForSeconds(1f);
        videoPlayer.Stop();
        if (isNella)
            sourceImage.sprite = isActive ? nella_InteractableSprite : nella_DetectSprite;
        else
            sourceImage.sprite = isActive ? steady_InteractableSprite : steady_DetectSprite;

        videoPlayer.gameObject.GetComponent<RawImage>().enabled = false;
        sourceImage.enabled = true;


        yield return null;
    }
}
