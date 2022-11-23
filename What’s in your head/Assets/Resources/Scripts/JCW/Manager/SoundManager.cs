using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Collections;

namespace JCW.AudioCtrl
{
    public enum Sound
    {
        TOTAL,
        BGM,
        EFFECT,
        DISTANCE,
        MOVE,
        UI,
        End,
    }

    [RequireComponent(typeof(PhotonView))]
    public class SoundManager : MonoBehaviour
    {
        [Header("�Ÿ� ���� ȿ���� ���")] [SerializeField] List<AudioClip> prevAudioClips = new();
        [Header("�Ÿ��� ���� ȿ���� ���")] [SerializeField] List<AudioClip> prev3DClips = new();
        [Header("UI ȿ���� ���")] [SerializeField] List<AudioClip> prevUIClips = new();
        [Header("�ݺ�����Ǵ� ������ ȿ���� ���")] [SerializeField] List<AudioClip> prevMoveClips = new();

        // ����� �����
        public readonly AudioSource[] audioSources = new AudioSource[(int)Sound.End];

        // ����� Ŭ��
        readonly Dictionary<string, AudioClip> audioClips = new();
        readonly Dictionary<string, AudioClip> UIClips = new();
        readonly Dictionary<string, AudioClip> moveClips = new();
        readonly Dictionary<string, AudioClip> threeDimesionClips = new();

        // ���� ���� �� ID / ���� ����� �ҽ�
        [HideInInspector] public readonly Dictionary<int, AudioSource> dict3D = new();

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

        // Sound ������ �ش��ϴ� ������Ʈ���� ������ְ�, ���� �Ŵ��� ������Ʈ�� �ڽ����� �޾��ش�.
        private void OnEnable()
        {
            soundTypes = System.Enum.GetNames(typeof(JCW.AudioCtrl.Sound));
            for (int i = 0 ; i < (int)Sound.End ; ++i)
            {
                GameObject obj = new() { name = soundTypes[i] };
                audioSources[i] = obj.AddComponent<AudioSource>();
                audioSources[i].GetComponent<AudioSource>().volume = 0.5f;
                obj.transform.parent = this.transform;
            }
            audioSources[(int)Sound.BGM].loop = true;
            audioSources[(int)Sound.MOVE].loop = true;
            SetUp();
        }

        // ����
        public void Clear()
        {
            for (int i = 0 ; i < audioSources.Length ; ++i)
            {
                audioSources[i].clip = null;
                audioSources[i].Stop();
            }
            audioClips.Clear();
            UIClips.Clear();
            moveClips.Clear();
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
                case Sound.MOVE:
                    return moveClips[name];
                case Sound.UI:
                    return UIClips[name];
                default:
                    return audioClips[name];
            }
        }
        /*
        void Set3DAudioClip(string path, JCW.AudioCtrl.Sound soundType)
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
                        Debug.Log("�̹� ����� ����� Ŭ���Դϴ�");
                    else
                        threeDimesionClips.Add(path, Resources.Load<AudioClip>(path));
                    break;

                case Sound.UI:
                    if (UIClips.ContainsKey(path))
                        Debug.Log("�̹� ����� ����� Ŭ���Դϴ�");
                    else
                        UIClips.Add(path, Resources.Load<AudioClip>(path));
                    break;

                case Sound.EFFECT:
                    if (audioClips.ContainsKey(path))
                        Debug.Log("�̹� ����� ����� Ŭ���Դϴ�");
                    else
                        audioClips.Add(path, Resources.Load<AudioClip>(path));
                    break;
            }
        }
        */
        void Set3DAudioClip(AudioClip clip, JCW.AudioCtrl.Sound soundType)
        {
            switch (soundType)
            {
                case Sound.BGM:
                default:
                    break;

                case Sound.DISTANCE:
                    if (threeDimesionClips.ContainsKey(clip.name))
                        Debug.Log("�̹� ����� ����� Ŭ���Դϴ�");
                    else
                        threeDimesionClips.Add(clip.name, clip);
                    break;

                case Sound.MOVE:
                    if (moveClips.ContainsKey(clip.name))
                        Debug.Log("�̹� ����� ����� Ŭ���Դϴ�");
                    else
                        moveClips.Add(clip.name, clip);
                    break;

                case Sound.UI:
                    if (UIClips.ContainsKey(clip.name))
                        Debug.Log("�̹� ����� ����� Ŭ���Դϴ�");
                    else
                        UIClips.Add(clip.name, clip);
                    break;

                case Sound.EFFECT:
                    if (audioClips.ContainsKey(clip.name))
                        Debug.Log("�̹� ����� ����� Ŭ���Դϴ�");
                    else
                        audioClips.Add(clip.name, clip);
                    break;
            }
        }
        public void SetUp()
        {
            for (int i = 0 ; i < prevAudioClips.Count ; ++i)
            {
                Set3DAudioClip(prevAudioClips[i], Sound.EFFECT);
            }
            for (int i = 0 ; i < prevUIClips.Count ; ++i)
            {
                Set3DAudioClip(prevUIClips[i], Sound.UI);
            }
            for (int i = 0 ; i < prev3DClips.Count ; ++i)
            {
                Set3DAudioClip(prev3DClips[i], Sound.DISTANCE);
            }
            for (int i = 0 ; i < prevMoveClips.Count ; ++i)
            {
                Set3DAudioClip(prevMoveClips[i], Sound.MOVE);
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
            if (audioSources[(int)Sound.MOVE].isPlaying)
                audioSources[(int)Sound.MOVE].Stop();
        }

        // EFFECT ==========================================================================
        #region 

        public void PlayEffect_RPC(string path)
        {
            photonView.RPC(nameof(PlayEffect), RpcTarget.AllViaServer, path);
        }

        // ȿ���� ���
        [PunRPC]
        public void PlayEffect(string path)
        {
            AudioClip audioClip = GetAudioClips(path, Sound.EFFECT);
            if (audioClip == null)
            {
                Debug.Log("NULL�� ����� ����� Ŭ���Դϴ�");
                return;
            }
            audioSources[(int)Sound.EFFECT].PlayOneShot(audioClip);
        }

        public void StopEffect_RPC()
        {
            photonView.RPC(nameof(StopEffect), RpcTarget.AllViaServer);
        }

        public void StopEffect_RPC(string path)
        {
            photonView.RPC(nameof(StopEffectName), RpcTarget.AllViaServer);
        }

        [PunRPC]
        public void StopEffect()
        {
            if (audioSources[(int)Sound.EFFECT].isPlaying)
                audioSources[(int)Sound.EFFECT].Stop();
        }


        [PunRPC]
        public void StopEffectName(string path)
        {

            if (audioSources[(int)Sound.EFFECT].isPlaying
                && audioSources[(int)Sound.EFFECT].clip.name.Equals(path))
                audioSources[(int)Sound.EFFECT].Stop();
        }


        public void PlayEffectNoOverlap_RPC(string path)
        {
            photonView.RPC(nameof(PlayEffectNO), RpcTarget.AllViaServer, path);
        }

        // ȿ���� ���
        [PunRPC]
        public void PlayEffectNO(string path)
        {
            AudioClip audioClip = GetAudioClips(path, Sound.EFFECT);
            if (audioClip == null)
            {
                Debug.Log("NULL�� ����� ����� Ŭ���Դϴ�");
                return;
            }

            if (!audioSources[(int)Sound.EFFECT].isPlaying)
                audioSources[(int)Sound.EFFECT].PlayOneShot(audioClip);
        }

        public void PlayMoveEffect(string path)
        {
            AudioClip audioClip = GetAudioClips(path, Sound.MOVE);
            if (audioClip == null)
            {
                Debug.Log("NULL�� ����� ����� Ŭ���Դϴ�");
                return;
            }

            if (!audioSources[(int)Sound.MOVE].isPlaying)
                audioSources[(int)Sound.MOVE].PlayOneShot(audioClip);
        }

        public void StopMoveEffect()
        {
            if (audioSources[(int)Sound.MOVE].isPlaying)
                audioSources[(int)Sound.MOVE].Stop();
        }

        // ȿ���� ����
        public void DeleteEffectClip(string name)
        {
            if (audioClips.ContainsKey(name))
                audioClips.Remove(name);
            else
                Debug.Log("�����Ϸ��� ȿ������ �������� �ʽ��ϴ�.");
        }

        #endregion


        // BGM ==========================================================================
        #region
        public void PlayBGM_RPC(string path)
        {
            photonView.RPC(nameof(PlayBGM), RpcTarget.AllViaServer, path);
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
                Debug.Log(path + "�� �����մϴ�");

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

        public void Play3D_RPC(string path, int id)
        {
            photonView.RPC(nameof(Play3D), RpcTarget.AllViaServer, path, id);
        }

        [PunRPC]
        public void Play3D(string path, int id)
        {
            dict3D[id].clip = GetAudioClips(path, Sound.DISTANCE);
            if (dict3D[id].clip == null)
            {
                Debug.Log("NULL�� ����� ����� Ŭ���Դϴ�");
                return;
            }
            // �ش� ���� ��ġ���� Ŭ�� ���
            dict3D[id].PlayOneShot(dict3D[id].clip);
        }

        [PunRPC]
        public void PlayIndirect3D_RPC(string path, int id)
        {
            photonView.RPC(nameof(PlayIndirect3D), RpcTarget.AllViaServer, path, id);
        }

        [PunRPC]
        public void PlayIndirect3D(string path, int id)
        {
            if (!dict3D.ContainsKey(id))
            {
                Debug.Log(id + " �� Ű���� �����ϴ�");
                return;
            }
            Debug.Log(path + " ���� ���� �õ�");
            if (dict3D[id].isPlaying)
            {
                Debug.Log(path + " ���� ���� �õ�");
                return;
            }
            dict3D[id].clip = GetAudioClips(path, Sound.DISTANCE);
            if (dict3D[id].clip == null)
            {
                Debug.Log("NULL�� ����� ����� Ŭ���Դϴ�");
                return;
            }
            // �ش� ���� ��ġ���� Ŭ�� ���
            
            dict3D[id].Play();
        }

        public void Stop3D_RPC(int id)
        {
            photonView.RPC(nameof(Stop3D), RpcTarget.AllViaServer, id);
        }

        [PunRPC]
        public void Stop3D(int id)
        {
            dict3D[id].Stop();
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
                Debug.Log("NULL�� ����� ����� Ŭ���Դϴ�");
                return;
            }
            //Debug.Log("UI���� ����մϴ�");
            audioSources[(int)Sound.UI].PlayOneShot(audioClip);
        }
        #endregion


        public static void Set3DAudio(int pvID, AudioSource audioSource, float volume = 1f, float maxDist = 120f, bool isLoop = false)
        {
            audioSource.rolloffMode = AudioRolloffMode.Custom;
            audioSource.spatialBlend = 1f;
            audioSource.playOnAwake = false;
            audioSource.maxDistance = maxDist;
            audioSource.loop = isLoop;
            audioSource.dopplerLevel = 0f;
            audioSource.volume = volume * Instance.audioSources[(int)Sound.DISTANCE].volume;          
            if(!Instance.dict3D.ContainsKey(pvID))
                Instance.dict3D.Add(pvID, audioSource);
        }

    }
}