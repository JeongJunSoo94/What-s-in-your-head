using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


namespace JCW.UI.InGame
{
    public class InteractableRange : MonoBehaviour
    {
        [Header("��ȣ�ۿ� ���� ��ġ")] [SerializeField] [Range(0, 100)] float range;

        [Header("���� �÷��̾�")] [SerializeField] VideoPlayer videoPlayer;
        [Header("����<->��ȣ�ۿ� ��������Ʈ�� ������ �̹���")] [SerializeField] Image sourceImage;

        [Header("�ڶ� - ���� & ��ȣ�ۿ� ��������Ʈ �� Ŭ��")]
        [SerializeField] Sprite     nella_DetectSprite;
        [SerializeField] Sprite     nella_InteractableSprite;
        [SerializeField] VideoClip  nella_ActiveClip;
        [SerializeField] VideoClip  nella_InactiveClip;
        [Header("���׵� - ���� & ��ȣ�ۿ� ��������Ʈ �� Ŭ��")]
        [SerializeField] Sprite     steady_DetectSprite;
        [SerializeField] Sprite     steady_InteractableSprite;
        [SerializeField] VideoClip  steady_ActiveClip;
        [SerializeField] VideoClip  steady_InactiveClip;



        // ���� ĳ���Ͱ� �ڶ�����
        bool isNella;
        private void Awake()
        {
            transform.localScale = new Vector3(range, range, range);

            // �������� ����� ���� �Ʒ� �ڵ� ����
            //isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];

            // �ӽ�
            isNella = false;
        }

        // ������ �ӽ÷� Ʈ���� Enter/Exit���� �ϰ� ������
        // �������� ����� ���� �÷��̾ ���̸� ���� �Ÿ��� ���� �� ���� ���Ѿ���.
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
            // �̹����� ��� ���ְ� ������ ���ֱ�
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

            // ������ ���� �̹��� ���ֱ�
            videoPlayer.gameObject.GetComponent<RawImage>().enabled = false;
            sourceImage.enabled = true;


            yield return null;
        }
    }
}
