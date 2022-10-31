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
        UI,
        End,
    }
    
    [RequireComponent(typeof(PhotonView))]
    public class SoundManager : MonoBehaviour
    {
        [Header("거리 무관 효과음 목록")] [SerializeField] List<AudioClip> prevAudioClips = new();
        [Header("거리에 따른 효과음 목록")] [SerializeField] List<AudioClip> prev3DClips = new();
        [Header("UI 효과음 목록")] [SerializeField] List<AudioClip> prevUIClips = new();

        // 오디오 재생기
        readonly AudioSource[] audioSources = new AudioSource[(int)Sound.End];

        // 오디오 클립
        readonly Dictionary<string, AudioClip> audioClips = new();
        readonly Dictionary<string, AudioClip> UIClips = new();
        readonly Dictionary<string, AudioClip> threeDimesionClips = new();

        PhotonView photonView;

        private bool isPause = false;

        public static SoundManager Instance;
        private void Awake()
        {
            if (Instance==null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
                Destroy(this.gameObject);
            photonView = GetComponent<PhotonView>();
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

        //테스트용

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                SoundManager.Instance.PlayBGM("POP");
            }
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                SoundManager.Instance.PlayUI("hoveringtest");
            }
            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                SoundManager.Instance.PauseResumeBGM();
            }
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
            UIClips.Clear();
        }
        public void PauseResumeBGM_RPC()
        {
            photonView.RPC("PauseResumeBGM", RpcTarget.AllViaServer);
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
            for (int i = 0 ; i<prevAudioClips.Count ; ++i)
            {
                SetEffectAudioClip("Sounds/EFFECT/" + prevAudioClips[i].name);
            }
            for (int i = 0 ; i<prevUIClips.Count ; ++i)
            {
                SetEffectAudioClip("Sounds/UI/" + prevUIClips[i].name);
            }
            for (int i = 0 ; i<prev3DClips.Count ; ++i)
            {
                SetEffectAudioClip("Sounds/3D/" + prev3DClips[i].name);
            }
        }

        public void PlayEffect_RPC(string path)
        {
            photonView.RPC(nameof(PlayEffect), RpcTarget.AllViaServer, path);
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
            //Debug.Log("효과음을 재생합니다");
            audioSources[(int)Sound.Effect].PlayOneShot(audioClip);
        }

        public void StopEffect_RPC()
        {
            photonView.RPC(nameof(StopEffect), RpcTarget.AllViaServer);
        }

        [PunRPC]
        public void StopEffect()
        {
            if (audioSources[(int)Sound.Effect].isPlaying)
                audioSources[(int)Sound.Effect].Stop();
        }
       
        

        public void PlayEffectNoOverlap_RPC(string path)
        {
            photonView.RPC(nameof(PlayEffectNO), RpcTarget.AllViaServer, path);
        }

        // 효과음 재생
        [PunRPC]
        public void PlayEffectNO(string path)
        {
            AudioClip audioClip = GetEffectClips(path);
            if (audioClip == null)
            {
                Debug.Log("NULL로 저장된 오디오 클립입니다");
                return;
            }

            if (!audioSources[(int)Sound.Effect].isPlaying)
            {
                //Debug.Log("효과음을 재생합니다");
                audioSources[(int)Sound.Effect].PlayOneShot(audioClip);
            }
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

        public void PlayBGM_RPC(string path)
        {
            photonView.RPC(nameof(PlayBGM), RpcTarget.AllViaServer, path);
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

        public void PlayUI_RPC(string path)
        {
            photonView.RPC(nameof(PlayUI), RpcTarget.AllViaServer, path);
        }

        public void Play3D_RPC(string path)
        {
            photonView.RPC(nameof(Play3D), RpcTarget.AllViaServer, path);
        }
        // 3D 재생
        [PunRPC]
        public void Play3D(string path, Vector3 pos)
        {
            AudioClip audioClip = Get3DClips(path);
            if (audioClip == null)
            {
                Debug.Log("NULL로 저장된 오디오 클립입니다");
                return;
            }

            //특정 거리에서 사운드 재생 (3D 사운드, 단 재생 후 자동으로 사라짐 )
            //AudioSource.PlayClipAtPoint(audioClip, pos);            
            audioSources[(int)Sound.Sound3D].PlayOneShot(audioClip);
        }
        public AudioClip Get3DClips(string name)
        {
            string clipName = name;
            if (!name.Contains("Sounds/3D"))
                clipName = $"Sounds/3D/{name}";

            return threeDimesionClips[clipName];
        }
        void Set3DAudioClip(string path)
        {
            if (!path.Contains("Sounds/3D"))
                path = $"Sounds/3D/{path}";
            if (threeDimesionClips.ContainsKey(path))
                Debug.Log("이미 저장된 오디오 클립입니다");
            else
                threeDimesionClips.Add(path, Resources.Load<AudioClip>(path));
        }

        

        // UI음 재생
        [PunRPC]
        public void PlayUI(string path)
        {
            AudioClip audioClip = GetUIClips(path);
            if (audioClip == null)
            {
                Debug.Log("NULL로 저장된 오디오 클립입니다");
                return;
            }
            //Debug.Log("UI음을 재생합니다");
            audioSources[(int)Sound.UI].PlayOneShot(audioClip);
        }
        public AudioClip GetUIClips(string name)
        {
            string clipName = name;
            if (!name.Contains("Sounds/UI"))
                clipName = $"Sounds/UI/{name}";

            return UIClips[clipName];
        }
        void SetUIAudioClip(string path)
        {
            if (!path.Contains("Sounds/UI"))
                path = $"Sounds/UI/{path}";
            if (UIClips.ContainsKey(path))
                Debug.Log("이미 저장된 오디오 클립입니다");
            else
                UIClips.Add(path, Resources.Load<AudioClip>(path));
            //특정 거리에서 사운드 재생 (3D 사운드, 단 재생 후 자동으로 사라짐 )
            //AudioSource.PlayClipAtPoint(audioClip, new Vector3(5, 1, 2));
        }

    }
}
