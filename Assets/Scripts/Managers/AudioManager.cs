using System.Collections.Generic;
using UnityEngine;

public enum Sound { BGM, SFX }

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Source")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Volume")]
    [SerializeField][Range(0f, 1f)] private float bgmVolume = 0.2f;
    [SerializeField][Range(0f, 1f)] private float sfxVolume = 0.2f;

    [Header("Clips")]
    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] private AudioClip[] sfxClips;

    private float prevBgmVolume;
    private float prevSfxVolume;

    public event System.Action<Sound, float> OnVolumeChanged;

#if UNITY_EDITOR
    private void OnValidate()
    {
        var bgmList = new List<AudioClip>();
        LoadSound(bgmList, Sound.BGM);
        bgmClips = bgmList.ToArray();

        var sfxList = new List<AudioClip>();
        LoadSound(sfxList, Sound.SFX);
        sfxClips = sfxList.ToArray();
    }

    private static void LoadSound(List<AudioClip> _list, Sound _type)
    {
        _list.Clear();
        string path = "Sounds/" + (_type == Sound.BGM ? "BGMs" : "SFXs");
        var clips = Resources.LoadAll<AudioClip>(path);
        if (clips != null && clips.Length > 0)
            _list.AddRange(clips);
    }
#endif

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SetAudio();
    }

    private void Start()
    {
        PlayBGM();
    }

    #region �����
    public void PlayBGM(AudioClip _clip = null)
    {
        if (_clip != null) bgmSource.clip = _clip;
        bgmSource.Play();
    }
    public void PlayBGM(string _name)
    {
        for (int i = 0; i < bgmClips.Length; i++)
        {
            var c = bgmClips[i];
            if (c != null && c.name == _name)
            {
                PlayBGM(c);
                return;
            }
        }
    }

    public void StopBGM() => bgmSource.Stop();

    public void ToggleBGM()
    {
        if (!IsBGMMuted() && bgmVolume > 0f)
        {
            prevBgmVolume = bgmVolume;
            SetBGMVolume(0f);
        }
        else
        {
            float target = prevBgmVolume > 0.2f ? prevBgmVolume : 0.2f;
            SetBGMVolume(target);
        }
    }
    #endregion

    #region ȿ����
    public void PlaySFX(AudioClip _clip) => sfxSource.PlayOneShot(_clip);
    public void PlaySFX(string _name)
    {
        for (int i = 0; i < sfxClips.Length; i++)
        {
            var c = sfxClips[i];
            if (c != null && c.name == _name)
            {
                PlaySFX(c);
                return;
            }
        }
    }

    public void ToggleSFX()
    {
        if (!IsSFXMuted() && sfxVolume > 0f)
        {
            prevSfxVolume = sfxVolume;
            SetSFXVolume(0f);
        }
        else
        {
            float target = prevSfxVolume > 0.2f ? prevSfxVolume : 0.2f;
            SetSFXVolume(target);
        }
    }

    public void GameOverSound() => PlaySFX("GameOver");

    public void ShootSound() => PlaySFX("Shoot");
    public void MergeSound(int _id = 0) => PlaySFX(_id != SpawnManager.Instance.GetFinal() ? "Merge" : "Boom");
    
    public void ButtonSound() => PlaySFX("Button");
    #endregion

    #region SET
    private void SetAudio()
    {
        if (bgmSource == null) bgmSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();

        bgmSource.playOnAwake = false;
        bgmSource.loop = true;
        bgmSource.clip = bgmClips[0];
        sfxSource.playOnAwake = false;

        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);
    }

    public void SetBGMVolume(float _volume = 1)
    {
        bgmVolume = Mathf.Clamp01(_volume);
        bgmSource.volume = bgmVolume;
        bgmSource.mute = (bgmVolume <= 0f);

        if (bgmVolume > 0f) prevBgmVolume = bgmVolume;

        OnVolumeChanged?.Invoke(Sound.BGM, bgmVolume);
    }

    public void SetSFXVolume(float _volume = 1)
    {
        sfxVolume = Mathf.Clamp01(_volume);
        sfxSource.volume = sfxVolume;
        sfxSource.mute = (sfxVolume <= 0f);

        if (sfxVolume > 0f) prevSfxVolume = sfxVolume;

        OnVolumeChanged?.Invoke(Sound.SFX, sfxVolume);
    }
    #endregion

    #region GET
    public float GetBGMVolume() => bgmVolume;
    public float GetSFXVolume() => sfxVolume;

    public bool IsBGMMuted() => bgmSource != null && bgmSource.mute;
    public bool IsSFXMuted() => sfxSource != null && sfxSource.mute;
    #endregion
}
