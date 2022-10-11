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
        [Header("효과음 목록")] [SerializeField] List<AudioClip> prevAudioClips = new();

        // 오디오 재생기
        readonly AudioSource[] audioSources = new AudioSource[(int)Sound.End];

        // 오디오 클립
        readonly Dictionary<string, AudioClip> audioClips = new();

        PhotonView photonView;

        private bool isPause = false;

        // 싱글톤
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

        // Sound 종류에 해당하는 오브젝트들을 만들어주고, 사운드 매니저 오브젝트에 자식으로 달아준다.
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
        // 비우기
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

        // 효과음 제거
        public void DeleteEffectClip(string name)
        {
            string clipName = name;
            if (!name.Contains("Sounds/EFFECT"))
                clipName = $"Sounds/EFFECT/{name}";
            if (audioClips.ContainsKey(clipName))
                audioClips.Remove(clipName);
            else
                Debug.Log("삭제하려는 효과음이 존재하지 않습니다.");


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
       
        // 효과음 재생
        [PunRPC]
        public void PlayEffect(string path)
        {
            AudioClip audioClip = GetEffectClips(path);
            if (audioClip == null)
            {
                Debug.Log("NULL로 저장된 오디오 클립입니다");
                return;
            }
            Debug.Log("효과음을 재생합니다");
            audioSources[(int)Sound.Effect].PlayOneShot(audioClip);
        }

        public void PlayBGM_RPC(string path)
        {
            photonView.RPC("PlayBGM", RpcTarget.OthersBuffered, path);
            PlayBGM(path);
        }
        // 배경음 재생
        [PunRPC]
        public void PlayBGM(string path)
        {
            if (!path.Contains("Sounds/BGM"))
                path = $"Sounds/BGM/{path}";

            AudioClip audioClip = Resources.Load<AudioClip>(path);
            if (audioClip == null)
            {
                Debug.Log("BGM :: 해당 파일을 찾지 못했습니다.");
                return;
            }
            else
                Debug.Log(path +"를 실행합니다");
                
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
                Debug.Log("이미 저장된 오디오 클립입니다");
            else
                audioClips.Add(path, Resources.Load<AudioClip>(path));
            //특정 거리에서 사운드 재생 (3D 사운드, 단 재생 후 자동으로 사라짐 )
            //AudioSource.PlayClipAtPoint(audioClip, new Vector3(5, 1, 2));
        }
        
    }
}
