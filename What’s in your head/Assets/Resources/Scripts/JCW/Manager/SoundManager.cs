using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace JCW.AudioCtrl
{
    public enum Sound
    {
        TOTAL,
        BGM,
        EFFECT,
        DISTANCE,
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
        public readonly AudioSource[] audioSources = new AudioSource[(int)Sound.End];

        // 오디오 클립
        readonly Dictionary<string, AudioClip> audioClips = new();
        readonly Dictionary<string, AudioClip> UIClips = new();
        readonly Dictionary<string, AudioClip> threeDimesionClips = new();

        PhotonView photonView;
        AudioSource otherSource;

        private bool isPause = false;

        string[] soundTypes;

        public static SoundManager Instance;
        private void Awake()
        {
            if (Instance == null)
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
            soundTypes = System.Enum.GetNames(typeof(JCW.AudioCtrl.Sound));
            for (int i = 0; i < (int)Sound.End; ++i)
            {
                GameObject obj = new() { name = soundTypes[i] };
                audioSources[i] = obj.AddComponent<AudioSource>();
                audioSources[i].GetComponent<AudioSource>().volume = 0.5f;
                obj.transform.parent = this.transform;
            }
            audioSources[(int)Sound.BGM].loop = true;
            SetUp();
        }

        // 비우기
        public void Clear()
        {
            for (int i = 0; i < audioSources.Length; ++i)
            {
                audioSources[i].clip = null;
                audioSources[i].Stop();
            }
            audioClips.Clear();
            UIClips.Clear();
            threeDimesionClips.Clear();
        }
        /*
        public AudioClip GetAudioClips(string name, JCW.AudioCtrl.Sound soundType)
        {
            string clipName = name;
            if (!name.Contains($"Sounds/{soundTypes[(int)soundType]}"))
                clipName = $"Sounds/{soundTypes[(int)soundType]}/{name}";

            switch (soundType)
            {
                case Sound.EFFECT:
                    return audioClips[clipName];
                case Sound.DISTANCE:
                    return threeDimesionClips[clipName];
                case Sound.UI:
                    return UIClips[clipName];
                default:
                    return audioClips[clipName];
            }
        }
        */
        public AudioClip GetAudioClips(string name, JCW.AudioCtrl.Sound soundType)
        {
            switch (soundType)
            {
                case Sound.EFFECT:
                    return audioClips[name];
                case Sound.DISTANCE:
                    return threeDimesionClips[name];
                case Sound.UI:
                    return UIClips[name];
                default:
                    return audioClips[name];
            }
        }
        void SetAudioClip(string path, JCW.AudioCtrl.Sound soundType)
        {
            if (!path.Contains($"Sounds/{soundTypes[(int)soundType]}"))
                path = $"Sounds/{soundTypes[(int)soundType]}/{path}";
            switch (soundType)
            {
                case Sound.BGM:
                default:
                    break;

                case Sound.DISTANCE:
                    if (threeDimesionClips.ContainsKey(path))
                        Debug.Log("이미 저장된 오디오 클립입니다");
                    else
                        threeDimesionClips.Add(path, Resources.Load<AudioClip>(path));
                    break;

                case Sound.UI:
                    if (UIClips.ContainsKey(path))
                        Debug.Log("이미 저장된 오디오 클립입니다");
                    else
                        UIClips.Add(path, Resources.Load<AudioClip>(path));
                    break;

                case Sound.EFFECT:
                    if (audioClips.ContainsKey(path))
                        Debug.Log("이미 저장된 오디오 클립입니다");
                    else
                        audioClips.Add(path, Resources.Load<AudioClip>(path));
                    break;
            }
        }

        void SetAudioClip(AudioClip clip, JCW.AudioCtrl.Sound soundType)
        {
            switch (soundType)
            {
                case Sound.BGM:
                default:
                    break;

                case Sound.DISTANCE:
                    if (threeDimesionClips.ContainsKey(clip.name))
                        Debug.Log("이미 저장된 오디오 클립입니다");
                    else
                        threeDimesionClips.Add(clip.name, clip);
                    break;

                case Sound.UI:
                    if (UIClips.ContainsKey(clip.name))
                        Debug.Log("이미 저장된 오디오 클립입니다");
                    else
                        UIClips.Add(clip.name, clip);
                    break;

                case Sound.EFFECT:
                    if (audioClips.ContainsKey(clip.name))
                        Debug.Log("이미 저장된 오디오 클립입니다");
                    else
                        audioClips.Add(clip.name, clip);
                    break;
            }
        }
        public void SetUp()
        {
            for (int i = 0; i < prevAudioClips.Count; ++i)
            {
                //SetAudioClip("Sounds/EFFECT/" + prevAudioClips[i].name, Sound.EFFECT);
                SetAudioClip(prevAudioClips[i], Sound.EFFECT);
            }
            for (int i = 0; i < prevUIClips.Count; ++i)
            {
                //SetAudioClip("Sounds/UI/" + prevUIClips[i].name, Sound.UI);
                SetAudioClip(prevUIClips[i], Sound.UI);
            }
            for (int i = 0; i < prev3DClips.Count; ++i)
            {
                //SetAudioClip("Sounds/DISTANCE/" + prev3DClips[i].name, Sound.DISTANCE);
                SetAudioClip(prev3DClips[i], Sound.DISTANCE);
            }
        }
        public void StopAllSound_RPC()
        {
            photonView.RPC(nameof(StopAllSound), RpcTarget.AllViaServer);
        }

        [PunRPC]
        public void StopAllSound()
        {
            if (audioSources[(int)Sound.EFFECT].isPlaying)
                audioSources[(int)Sound.EFFECT].Stop();
            if (audioSources[(int)Sound.BGM].isPlaying)
                audioSources[(int)Sound.BGM].Stop();
            if (audioSources[(int)Sound.DISTANCE].isPlaying)
                audioSources[(int)Sound.DISTANCE].Stop();
            if (audioSources[(int)Sound.UI].isPlaying)
                audioSources[(int)Sound.UI].Stop();
        }

        // EFFECT ==========================================================================
        #region 

        public void PlayEffect_RPC(string path)
        {
            photonView.RPC(nameof(PlayEffect), RpcTarget.AllViaServer, path);
        }

        // 효과음 재생
        [PunRPC]
        public void PlayEffect(string path)
        {
            AudioClip audioClip = GetAudioClips(path, Sound.EFFECT);
            if (audioClip == null)
            {
                Debug.Log("NULL로 저장된 오디오 클립입니다");
                return;
            }
            //Debug.Log("효과음을 재생합니다");
            audioSources[(int)Sound.EFFECT].PlayOneShot(audioClip);
        }

        public void StopEffect_RPC()
        {
            photonView.RPC(nameof(StopEffect), RpcTarget.AllViaServer);
        }

        [PunRPC]
        public void StopEffect()
        {
            if (audioSources[(int)Sound.EFFECT].isPlaying)
                audioSources[(int)Sound.EFFECT].Stop();
        }



        public void PlayEffectNoOverlap_RPC(string path)
        {
            photonView.RPC(nameof(PlayEffectNO), RpcTarget.AllViaServer, path);
        }

        // 효과음 재생
        [PunRPC]
        public void PlayEffectNO(string path)
        {
            AudioClip audioClip = GetAudioClips(path, Sound.EFFECT);
            if (audioClip == null)
            {
                Debug.Log("NULL로 저장된 오디오 클립입니다");
                return;
            }

            if (!audioSources[(int)Sound.EFFECT].isPlaying)
                audioSources[(int)Sound.EFFECT].PlayOneShot(audioClip);
        }

        // 효과음 제거
        public void DeleteEffectClip(string name)
        {
            //string clipName = name;
            //if (!name.Contains("Sounds/EFFECT"))
            //    clipName = $"Sounds/EFFECT/{name}";
            if (audioClips.ContainsKey(name))
                audioClips.Remove(name);
            else
                Debug.Log("삭제하려는 효과음이 존재하지 않습니다.");
        }

        #endregion


        // BGM ==========================================================================
        #region
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
                Debug.Log(path + "를 실행합니다");

            AudioSource curAudio = audioSources[(int)Sound.BGM];
            if (curAudio.isPlaying)
                curAudio.Stop();

            curAudio.clip = audioClip;
            curAudio.Play();
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

        public void StopBGM_RPC()
        {
            photonView.RPC(nameof(StopBGM), RpcTarget.AllViaServer);
        }

        [PunRPC]
        public void StopBGM()
        {
            audioSources[(int)Sound.BGM].Stop();
        }

        #endregion


        // 3D Sound ==========================================================================
        #region

        public void Play3D_RPC(string path, AudioSource source)
        {
            otherSource = source;
            photonView.RPC(nameof(Play3D), RpcTarget.AllViaServer, path);
        }

        [PunRPC]
        public void Play3D(string path)
        {
            AudioClip audioClip = GetAudioClips(path, Sound.DISTANCE);
            if (audioClip == null)
            {
                Debug.Log("NULL로 저장된 오디오 클립입니다");
                return;
            }
            // 해당 음원 위치에서 클립 재생
            otherSource.PlayOneShot(audioClip);
        }

        public void PlayIndirect3D_RPC(string path, AudioSource source)
        {
            otherSource = source;
            photonView.RPC(nameof(PlayIndirect3D), RpcTarget.AllViaServer, path);
        }

        [PunRPC]
        public void PlayIndirect3D(string path)
        {
            otherSource.clip = GetAudioClips(path, Sound.DISTANCE);
            if (otherSource.clip == null)
            {
                Debug.Log("NULL로 저장된 오디오 클립입니다");
                return;
            }
            // 해당 음원 위치에서 클립 재생
            otherSource.Play();
        }

        public void Stop3D_RPC(AudioSource source)
        {
            otherSource = source;
            photonView.RPC(nameof(Stop3D), RpcTarget.AllViaServer);
        }

        [PunRPC]
        public void Stop3D()
        {
            otherSource.Stop();
        }

        public void StopIndirect3D_RPC(AudioSource source)
        {
            otherSource = source;
            photonView.RPC(nameof(StopIndirect3D), RpcTarget.AllViaServer);
        }

        [PunRPC]
        public void StopIndirect3D()
        {
            otherSource.Stop();
        }

        #endregion


        // UI ==========================================================================
        #region
        public void PlayUI_RPC(string path)
        {
            photonView.RPC(nameof(PlayUI), RpcTarget.AllViaServer, path);
        }

        [PunRPC]
        public void PlayUI(string path)
        {
            AudioClip audioClip = GetAudioClips(path, Sound.UI);
            if (audioClip == null)
            {
                Debug.Log("NULL로 저장된 오디오 클립입니다");
                return;
            }
            //Debug.Log("UI음을 재생합니다");
            audioSources[(int)Sound.UI].PlayOneShot(audioClip);
        }
        #endregion

    }
}