using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class CustomAudioClip
{    
    public AudioClip m_audio;

    [Range(0, 2)]
    public float m_volume = 1f;

    public CustomAudioClip() => m_volume = 1f;
};


public class SoundManager : MonoBehaviour
{
    [Serializable]
    private class AdvancedOption
    {
        public int m_iOnceFXPoolSize = 5;   // 한번 재생용 효과음 풀 크기. 동시에 재생 가능한 수가 됨.
        public int m_iLoopFXPoolSize = 5;   // 반복 재생용 효과음 풀 크기. 동시에 재생 가능한 수가 됨.
    };


    private static SoundManager s_oInst;

    private static int s_lastFrameCount = -1;
    private static readonly HashSet<AudioClip> s_playingSymbolSounds = new ();
        
    private int m_sourceIndex = 0;

    [SerializeField]
    [Range(0, 1)]
    private float m_volumeControl = .5f;
    [SerializeField]
    [Range(0, 1)]
    private float m_bgmVolume = .5f;
    private float m_bgmClipVolume = 1f;

    [SerializeField] private Transform m_bgmRoot = null;                   // 브금 루트.
    [SerializeField] private Transform m_fxOnceRoot = null;                // 한번만 재생되는 효과음 루트.
    [SerializeField] private Transform m_fxLoopRoot = null;                // 반복재생 되는 효과음 루트.

    [Space(10f)]
    [SerializeField] private AdvancedOption m_option = null;

    private readonly List<AudioSource> m_listOnceFXAudios = new (); // 한번 재생용 오디오 오브젝트들.
    private readonly List<AudioSource> m_listLoopFXAudios = new (); // 반복 재생용 오디오 오브젝트들.

    private readonly Dictionary<string, CustomAudioClip> m_dicRes = new ();

    bool m_fxMute = false;
    bool m_enableFX = true;

    //fadeout        
    Coroutine m_crtBGMFadeOut;

    float m_bgmVolumeBackup;

    private AudioSource m_bgmAudio;                                        // 브금 오디오. 브금은 하나만 있어도 됨.
    
    public static SoundManager I => s_oInst;

    private AudioSource BGMAudio
    {
        get
        {
            if (m_bgmAudio == null)
                m_bgmAudio = m_bgmRoot.GetComponent<AudioSource>();

            return m_bgmAudio;
        }
    }

    void Awake()
    {
        s_oInst = this;
        _InitFXPool();
    }

    public static bool ExistSound(string key)
    {
        return null != I.FindAudioClip(key);
    }

    public static void PlayBGM(string key, bool loop = true)
    {
        if (I != null)
        {
            CustomAudioClip clip = I.FindAudioClip(key);
            if (clip != null)
                I.PlayBGM(clip, loop);
        }
    }

    public static void PlayFX(string key, bool loop = false, bool bIgnoreIfPlaying = false)
    {
        if (I != null)
        {
            CustomAudioClip clip = I.FindAudioClip(key);
            if (clip != null)
                I._PlayFX(clip, loop, bIgnoreIfPlaying);
        }
    }

    public static void StopFX(string key)
    {
        if (I != null)
        {
            CustomAudioClip clip = I.FindAudioClip(key);
            if (clip != null)
                I.StopFX(clip);
        }
    }

    public static void PlayFXFadeIn(string key, float fadeInDuration, bool loop = false, bool bIgnoreIfPlaying = false)
    {
        if (I != null)
        {
            CustomAudioClip clip = I.FindAudioClip(key);
            if (clip != null)
            {
                I._PlayFX(clip, loop, bIgnoreIfPlaying);
                I._FadeInFX(clip.m_audio, 0f, fadeInDuration);
            }
        }
    }

    //특정 FX사운드가 정지되었다가 muteDuration 지난다음 fadeInDuration동안 fadeIn됨
    public static void FadeInFX(string key, float fadeInDuration, float muteDuration = 0f)
    {
        if (I != null)
        {
            CustomAudioClip clip = I.FindAudioClip(key);
            if (clip != null)
                I._FadeInFX(clip.m_audio, muteDuration, fadeInDuration);
        }
    }

    //모든 FX사운드가 정지되었다가 duration 지난다음 fadeIn됨
    public static void FadeInAllFX(float muteDuration, float fadeInDuration)
    {
        if (I != null)
            I._FadeInAll(muteDuration, fadeInDuration);
    }

    public static void FadeOutFX(string key, float duration)
    {
        if (I != null)
        {
            CustomAudioClip clip = I.FindAudioClip(key);
            if (clip != null)
                I._FadeOutFX(clip.m_audio, duration);
        }
    }

    public static void StopAllFX()
    {
        if (I != null)
            I._StopAllFX();
    }

    public static void StopBGM()
    {
        if (I != null)
            I._StopBGM();
    }

    public static void RegisterSoundRes(string key, CustomAudioClip aClip)
    {
        if (I != null)
            I._RegisterSoundRes(key, aClip);
    }

    public static void unregisterSoundRes(string key)
    {
        if (I != null)
            I._UnregisterSoundRes(key);
    }

    public static void FadeOutBGM(float duration, float targetVolume, bool bStop)
    {
        if (I != null)
            I._FadeOutBGM(duration, targetVolume, bStop);
    }

    public static void FadeOutBGM(float duration)
    {
        if (I != null)
            I._FadeOutBGM(duration, 0f, true);
    }

    public static void FadeInBGM(float delay, float duration, float targetVolume)
    {
        if (I != null)
            I._FadeInBGM(delay, duration, targetVolume);
    }

    public static void EnableBGM(bool bEnable)
    {
        if (I != null)
            I._MuteBGM(!bEnable);
    }

    //세팅에서 FX ON / OFF 할때 사용
    public static void EnableFX(bool bEnable)
    {
        if (I != null)
            I._EnableFX(bEnable);
    }

    //Focus On / OFF 될때 볼륨만 0으로 만듬
    public static void MuteFX(bool bEnable)
    {
        if (I != null)
            I._MuteFX(bEnable);
    }

    public static bool CheckMuteBGM()
    {
        if (I != null)
            return I.IsMuteBGM;

        return false;
    }

    public static bool CheckMuteFX()
    {
        if (I != null)
            return I.IsMuteFX;

        return false;
    }

    //(주의) Volume의 범위는 0~1 사이이고 기본볼룸이 0.5이다.
    public static void SetBGMVolume(float volume)
    {
        if (I != null)
            I._SetBGMVolume(volume);
    }


    public void PlayBGM(CustomAudioClip bgm, bool loop)
    {
        if (bgm == null || bgm.m_audio == null)
            return;

        //BGM은 이미 플레이중이면 그대로 플레이시킨다.
        if (m_bgmAudio.clip == bgm.m_audio)
            return;

        _CancelFadeOut();

        m_bgmClipVolume = bgm.m_volume;
        m_bgmAudio.clip = bgm.m_audio;
        m_bgmAudio.volume = m_bgmVolume * m_bgmClipVolume;
        m_bgmAudio.loop = loop;

        m_bgmAudio.Play();
    }

    public void PlayBGM(AudioClip bgm)
    {
        if (bgm == null)
            return;

        m_bgmClipVolume = 1f;
        m_bgmAudio.clip = bgm;
        m_bgmAudio.volume = m_bgmVolume;

        m_bgmAudio.Play();
    }

    public void PlayBGM()
    {
        if (m_bgmAudio.isPlaying)
            return;

        m_bgmAudio.Play();
    }

    public void SetFXVolume(float vol)
    {
        static void FxVolumConfig(Transform root, float targetVol)
        {
            AudioSource[] audios = root.GetComponentsInChildren<AudioSource>(true);
            for (int a = 0; a < audios.Length; a++)
                audios[a].volume = targetVol;
        }

        FxVolumConfig(m_fxOnceRoot, vol);
        FxVolumConfig(m_fxLoopRoot, vol);
    }

    public void PlayFX(AudioClip clip, bool loop = false)
    {
        if (clip == null)
            return;

        if (s_lastFrameCount != Time.frameCount)
        {
            s_playingSymbolSounds.Clear();
            s_lastFrameCount = Time.frameCount;
        }

        if (clip != null)
        {
            if (s_playingSymbolSounds.Contains(clip))
                return;

            s_playingSymbolSounds.Add(clip);
        }

        // FXoff이면 Clip세팅까지는 하고 Play는 막는다.
        if (m_enableFX == false)
            return;

        AudioSource audioPlayer = _GetUsableAudio(clip, loop);
        if (audioPlayer != null)
        {
            audioPlayer.clip = clip;
            audioPlayer.volume = m_volumeControl;
            audioPlayer.Play();
        }
    }

    public void StopFX(AudioClip fx)
    {
        if (m_enableFX == false)
            return;

        if (fx == null)
            return;

        s_playingSymbolSounds.Remove(fx);

        foreach (AudioSource source in m_listLoopFXAudios)
        {
            if (source.clip == fx)
                source.Stop();
        }

        foreach (AudioSource source in m_listOnceFXAudios)
        {
            if (source.clip == fx)
                source.Stop();
        }
    }

    public void StopFX(CustomAudioClip cFx)
    {
        if (m_enableFX == false)
            return;

        if (cFx == null || cFx.m_audio == null)
            return;
                
        s_playingSymbolSounds.Remove(cFx.m_audio);

        foreach (var source in m_listLoopFXAudios)
        {
            if (source.clip == cFx.m_audio)
                source.Stop();
        }

        foreach (AudioSource source in m_listOnceFXAudios)
        {
            if (source.clip == cFx.m_audio)
                source.Stop();
        }
    }

    AudioSource FindFxSource(AudioClip ac)
    {
        foreach (AudioSource source in m_listLoopFXAudios)
        {
            if (source.clip == ac)
                return source;
        }

        foreach (AudioSource source in m_listOnceFXAudios)
        {
            if (source.clip == ac)
                return source;
        }

        return null;
    }

    public void _StopBGM()
    {
        m_bgmAudio.Stop();
        m_bgmAudio.clip = null;
    }

    public void _SetBGMVolume(float volume)
    {
        if (m_crtBGMFadeOut == null)
        {
            m_bgmVolume = volume;
            m_bgmAudio.volume = m_bgmClipVolume * volume;
        }
        else
        {
            m_bgmVolumeBackup = volume;
        }
    }

    public void _MuteBGM(bool bFlag) => m_bgmAudio.mute = bFlag;

    public void _EnableFX(bool bFlag) => m_enableFX = bFlag;

    public void _MuteFX(bool bFlag)
    {
        m_fxMute = bFlag;

        foreach (var l in m_listLoopFXAudios)
            l.mute = bFlag;
        
        foreach (var o in m_listOnceFXAudios)
            o.mute = bFlag;
    }

    public bool IsMuteBGM => m_bgmAudio.mute;

    public bool IsMuteFX => m_fxMute;

    public AudioSource _PlayFX(CustomAudioClip cClip, bool loop = false, bool bIgnoreIfPlaying = false)
    {
        if (cClip == null || cClip.m_audio == null)
            return null;

        CustomAudioClip clip = cClip;

        //중복 플레이 금지
        if (bIgnoreIfPlaying)
        {
            if (clip.m_audio != null)
            {
                if (s_playingSymbolSounds.Contains(clip.m_audio))
                    return null;
            }
        }

        if (s_lastFrameCount != Time.frameCount)
        {
            s_playingSymbolSounds.Clear();
            s_lastFrameCount = Time.frameCount;
        }

        if (clip.m_audio != null)
        {
            if (s_playingSymbolSounds.Contains(clip.m_audio))
                return null;

            s_playingSymbolSounds.Add(clip.m_audio);
        }

        // FXoff이면 Clip세팅까지는 하고 Play는 막는다.
        if (m_enableFX == false)
            return null;

        AudioSource audioPlayer = _GetUsableAudio(clip.m_audio, loop);
        if (audioPlayer != null)
        {
            audioPlayer.clip = clip.m_audio;
            audioPlayer.volume = m_volumeControl * clip.m_volume;
            audioPlayer.Play();

            return audioPlayer;
        }

        return null;
    }

    public void _StopAllFX()
    {
        foreach (var source in m_listLoopFXAudios)
        {
            source.Stop();
        }

        foreach (AudioSource source in m_listOnceFXAudios)
        {
            source.Stop();
        }
    }

    public void _FadeInAll(float muteDuration, float fadeInDuration)
    {
        foreach (var l in m_listLoopFXAudios)
        {
            if (l.clip != null)
            {
                if (l.isPlaying)
                {
                    float fOriginalVol = l.volume;

                    l.volume = 0f;
                    StartCoroutine(_FadeInCore(l, muteDuration, fadeInDuration, fOriginalVol));
                }
            }
        }

        foreach (var o in m_listOnceFXAudios)
        {
            if (o.clip != null)
            {
                if (o.isPlaying)
                {
                    float fOriginalVol = o.volume;

                    o.volume = 0f;
                    StartCoroutine(_FadeInCore(o, muteDuration, fadeInDuration, fOriginalVol));
                }
            }
        }
    }

    public void _FadeInFX(AudioClip ac, float muteDuration, float fadeInduration)
    {
        foreach (var l in m_listLoopFXAudios)
        {
            if (l.clip == ac)
            {
                if (l.isPlaying)
                {
                    float fOriginalVol = l.volume;

                    l.volume = 0f;
                    StartCoroutine(_FadeInCore(l, muteDuration, fadeInduration, fOriginalVol));
                }
            }
        }

        foreach (var o in m_listOnceFXAudios)
        {
            if (o.clip == ac)
            {
                if (o.isPlaying)
                {
                    float fOriginalVol = o.volume;

                    o.volume = 0f;
                    StartCoroutine(_FadeInCore(o, muteDuration, fadeInduration, fOriginalVol));

                }
            }
        }
    }

    public void _FadeOutFX(AudioClip ac, float duration)
    {
        foreach (var source in m_listLoopFXAudios)
        {
            if (source != null && source.clip == ac)
            {
                if (source.isPlaying)
                    StartCoroutine(_FadeOutCore(source, duration, 0f, true));
            }
        }

        foreach (var source in m_listOnceFXAudios)
        {
            if (source != null && source.clip == ac)
            {
                if (source.isPlaying)
                    StartCoroutine(_FadeOutCore(source, duration, 0f, true));
            }
        }
    }

    public void _FadeInBGM(float delay, float duration, float targetVolume)
    {
        m_bgmVolumeBackup = m_bgmAudio.volume;

        _CancelFadeOut();

        if (m_crtBGMFadeOut == null)
            StartCoroutine(_FadeInBGMCore(m_bgmAudio, delay, duration, targetVolume));
    }

    IEnumerator _FadeInBGMCore(AudioSource a, float delay, float duration, float targetVolume)
    {
        float fModifiedTargetVol = m_bgmClipVolume * targetVolume;
        m_crtBGMFadeOut = StartCoroutine(_FadeInCore(a, delay, duration, fModifiedTargetVol));

        yield return m_crtBGMFadeOut;
        
        m_crtBGMFadeOut = null;
    }

    public void _FadeOutBGM(float duration, float targetVolume, bool bStop)
    {
        _CancelFadeOut();

        if (m_crtBGMFadeOut == null)
        {
            m_bgmVolumeBackup = m_bgmAudio.volume;
            StartCoroutine(_FadeOutBGMCore(m_bgmAudio, duration, targetVolume, bStop));
        }
    }

    void _CancelFadeOut()
    {
        if (m_crtBGMFadeOut != null)
        {
            StopCoroutine(m_crtBGMFadeOut);

            m_bgmAudio.volume = m_bgmVolumeBackup;
            m_bgmVolume = m_bgmVolumeBackup;
            m_crtBGMFadeOut = null;
        }
    }

    IEnumerator _FadeOutBGMCore(AudioSource a, float duration, float targetVolume, bool bStop)
    {
        float fModifiedTargetVol = m_bgmClipVolume * targetVolume;
        m_crtBGMFadeOut = StartCoroutine(_FadeOutCore(a, duration, fModifiedTargetVol, bStop));

        yield return m_crtBGMFadeOut;
        
        m_crtBGMFadeOut = null;

        if (bStop)
            m_bgmAudio.clip = null;
    }

    IEnumerator _FadeOutCore(AudioSource a, float duration, float targetVolume, bool bStop)
    {
        float startVolume = a.volume;
        while (a.volume > targetVolume)
        {
            a.volume -= (startVolume - targetVolume) * Time.deltaTime / duration;
            yield return new WaitForEndOfFrame();
        }

        a.volume = targetVolume;

        if (bStop)
        {
            a.Stop();
            a.volume = startVolume;
        }
    }

    IEnumerator _FadeInCore(AudioSource a, float delay, float fadeInDuration, float targetVolume)
    {
        if (!a.isPlaying)
            a.volume = 0f;

        yield return new WaitForSeconds(delay);

        while (a.volume < targetVolume)
        {
            a.volume += targetVolume * Time.deltaTime / fadeInDuration;
            yield return new WaitForEndOfFrame();
        }

        a.volume = targetVolume;
    }

    public void PlayAllFX(bool bOnlyLoopSound = false)
    {
        foreach (var l in m_listLoopFXAudios)
            l.Play();

        if (false == bOnlyLoopSound)
        {
            foreach (var o in m_listOnceFXAudios)
                o.Play();
        }
    }

    private void _InitFXPool()
    {
        SetFXVolume(m_volumeControl);

        for (int o = 0 ; o < m_option.m_iOnceFXPoolSize ; o++)
        {
            GameObject audioObject = new (string.Format("{0:00}", o));
            AudioSource audioInst = audioObject.AddComponent<AudioSource>();
            audioInst.playOnAwake = false;

            _AddChild(m_fxOnceRoot, audioObject.transform);
            m_listOnceFXAudios.Add(audioInst);
        }

        for (int l = 0 ; l < m_option.m_iLoopFXPoolSize ; l++)
        {
            GameObject audioObject = new (string.Format("{0:00}", l));
            AudioSource audioInst = audioObject.AddComponent<AudioSource>();
            audioInst.playOnAwake = false;
            audioInst.loop = true;

            _AddChild(m_fxLoopRoot, audioObject.transform);
            m_listLoopFXAudios.Add(audioInst);
        }
    }

    private void _AddChild(Transform _oParent, Transform _oChild)
    {
        _oChild.parent = _oParent;
        _oChild.localPosition = Vector3.zero;
        _oChild.localScale = Vector3.one;
        _oChild.localRotation = Quaternion.identity;
    }

    private AudioSource _GetUsableAudio(AudioClip clip, bool loop)
    {
        List<AudioSource> sources = loop ? m_listLoopFXAudios : m_listOnceFXAudios;
        if (loop)
        {
            // 루핑 사운드는 동시에 하나만 재생되도록 한다
            AudioSource notPlayingSource = null;
            foreach (var s in sources)
            {
                if (s.isPlaying == false)
                {
                    notPlayingSource = s;
                    if (s.clip == clip)
                        return s;
                }
            }

            if (notPlayingSource != null)
                return notPlayingSource;

            int randIdx = UnityEngine.Random.Range(0, sources.Count);
            sources[randIdx].Stop();
            return sources[randIdx];
        }
        else
        {
            foreach (var s in sources)
            {
                if (s.isPlaying == false)
                    return s;
            }

            return sources[m_sourceIndex++ % sources.Count];
        }
    }

    public void _RegisterSoundRes(string key, CustomAudioClip aClip) => m_dicRes.Add(key, aClip);

    public void _UnregisterSoundRes(string key) => m_dicRes.Remove(key);

    public CustomAudioClip FindAudioClip(string key)
    {
        if(m_dicRes.TryGetValue(key, out var clip) == true)
            return clip;

        return null;
    }
}

