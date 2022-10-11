using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace JCW.AudioCtrl
{
    public enum Sound
    {
        BGM,
        Effect,
        Sound3D,
        End,
    }

    public class SoundManager : MonoBehaviour
    {
        [Header("ȿ���� ���")] [SerializeField] List<AudioClip> prevAudioClips = new();

        // ����� �����
        readonly AudioSource[] audioSources = new AudioSource[(int)Sound.End];

        // ����� Ŭ��
        readonly Dictionary<string, AudioClip> audioClips = new();

        PhotonView photonView;

        private bool isPause = false;

        // �̱���
        public static SoundManager instance = null;
        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
            if (instance==null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (instance != this)
            {
                Destroy(this.gameObject);                
            }
        }

        // Sound ������ �ش��ϴ� ������Ʈ���� ������ְ�, ���� �Ŵ��� ������Ʈ�� �ڽ����� �޾��ش�.
        private void OnEnable()
        {
            string[] soundNames = System.Enum.GetNames(typeof(JCW.AudioCtrl.Sound));
            for (int i = 0 ; i < (int)Sound.End ; ++i)
            {
                GameObject obj = new() { name = soundNames[i] };
                audioSources[i] = obj.AddComponent<AudioSource>();
                obj.transform.parent = this.transform;
            }
            audioSources[(int)Sound.BGM].loop = true;
            SetUp();
        }
        // ����
        public void Clear()
        {
            for (int i = 0 ; i<audioSources.Length ; ++i)
            {
                audioSources[i].clip = null;
                audioSources[i].Stop();                
            }
            audioClips.Clear();
        }
        public void PauseResumeBGM_RPC()
        {
            photonView.RPC("PauseResumeBGM", RpcTarget.OthersBuffered);
            PauseResumeBGM();
        }

        [PunRPC]
        public void PauseResumeBGM()
        {
            if (isPause)
            {
                audioSources[(int)Sound.BGM].UnPause();
                isPause = false;
            }
            else
            {
                audioSources[(int)Sound.BGM].Pause();
                isPause = true;
            }                
        }

        // ȿ���� ����
        public void DeleteEffectClip(string name)
        {
            string clipName = name;
            if (!name.Contains("Sounds/EFFECT"))
                clipName = $"Sounds/EFFECT/{name}";
            if (audioClips.ContainsKey(clipName))
                audioClips.Remove(clipName);
            else
                Debug.Log("�����Ϸ��� ȿ������ �������� �ʽ��ϴ�.");


        }

        public void SetUp()
        {
            foreach (AudioClip clip in prevAudioClips)
            {
                SetEffectAudioClip("Sounds/EFFECT/" + clip.name);
            }
        }

        public void PlayEffect_RPC(string path)
        {
            photonView.RPC("PlayEffect", RpcTarget.OthersBuffered, path);
            PlayEffect(path);
        }
       
        // ȿ���� ���
        [PunRPC]
        public void PlayEffect(string path)
        {
            AudioClip audioClip = GetEffectClips(path);
            if (audioClip == null)
            {
                Debug.Log("NULL�� ����� ����� Ŭ���Դϴ�");
                return;
            }
            Debug.Log("ȿ������ ����մϴ�");
            audioSources[(int)Sound.Effect].PlayOneShot(audioClip);
        }

        public void PlayBGM_RPC(string path)
        {
            photonView.RPC("PlayBGM", RpcTarget.OthersBuffered, path);
            PlayBGM(path);
        }
        // ����� ���
        [PunRPC]
        public void PlayBGM(string path)
        {
            if (!path.Contains("Sounds/BGM"))
                path = $"Sounds/BGM/{path}";

            AudioClip audioClip = Resources.Load<AudioClip>(path);
            if (audioClip == null)
            {
                Debug.Log("BGM :: �ش� ������ ã�� ���߽��ϴ�.");
                return;
            }
            else
                Debug.Log(path +"�� �����մϴ�");
                
            AudioSource curAudio = audioSources[(int)Sound.BGM];
            if (curAudio.isPlaying)
                curAudio.Stop();

            curAudio.clip = audioClip;
            curAudio.Play();
        }

        public AudioClip GetEffectClips(string name)
        {
            string clipName = name;
            if (!name.Contains("Sounds/EFFECT"))
                clipName = $"Sounds/EFFECT/{name}";

            return audioClips[clipName];
        }
        void SetEffectAudioClip(string path)
        {
            if (!path.Contains("Sounds/EFFECT"))
                path = $"Sounds/EFFECT/{path}";
            if (audioClips.ContainsKey(path))
                Debug.Log("�̹� ����� ����� Ŭ���Դϴ�");
            else
                audioClips.Add(path, Resources.Load<AudioClip>(path));
            //Ư�� �Ÿ����� ���� ��� (3D ����, �� ��� �� �ڵ����� ����� )
            //AudioSource.PlayClipAtPoint(audioClip, new Vector3(5, 1, 2));
        }
        
    }
}
