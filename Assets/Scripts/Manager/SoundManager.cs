using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //sound
    static public string click_Sound = "coin";
    static public string back_Sound = "cancel1";
    static public string match_Sound = "holy5";
    static public string swap_Sound = "wind07";


    //bgm
    static public string Main_Bgm = "Theme 133 8bit ver";

    private static SoundManager Instance;
    public static SoundManager Ins
    {
        get
        {
            return Instance;
        }
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    [SerializeField] AudioSource bgm;
    [SerializeField] List<AudioSource> sounds;
    
    [SerializeField] List<AudioClip> sound_Clips;
    [SerializeField] List<AudioClip> bgm_Clips;

    private bool isOnFade = false;
    float fadeTime = 0.5f;

    public void PlaySound(string _soundStr, bool _loop = false)
    {
        AudioClip clip = sound_Clips.SingleOrDefault(r => r.name == _soundStr);

        if (clip != null)
        {
            for(int i = 0; i < sounds.Count; i++)
            {
                if(!sounds[i].isPlaying)
                {
                    sounds[i].clip = clip;
                    sounds[i].loop = _loop;
                    sounds[i].Play();
                    break;
                }
            }
        }
    }

    public void StopSound()
    {
        for (int i = 0; i < sounds.Count; i++)
        {
            if (!sounds[i].isPlaying)
            {
                sounds[i].Stop();
                break;
            }
        }
    }

    public void PlayBGM(string _bgmStr)
    {
        AudioClip clip = bgm_Clips.SingleOrDefault(r => r.name == _bgmStr);

        if (clip != null && bgm.clip != clip)
        {
            if (isOnFade)
            {
                StopCoroutine(CoStopBGM());
                StopCoroutine("CoPlayBGM");
            }

            StartCoroutine(CoPlayBGM(clip));
        }

    }

    IEnumerator CoPlayBGM(AudioClip _clip)
    {
        isOnFade = true;

        while (bgm.volume > 0)
        {
            bgm.volume -= Time.deltaTime * 0.5f;
            yield return new WaitForEndOfFrame();
        }

        bgm.volume = 0.25f;
        bgm.clip = _clip;
        bgm.Play();

        isOnFade = false;
    }

    public void StopBGM()
    {
        if (isOnFade)
        {
            StopCoroutine(CoStopBGM());
            StopCoroutine("CoPlayBGM");
        }

        StartCoroutine(CoStopBGM());
    }

    IEnumerator CoStopBGM()
    {
        isOnFade = true;
        while (bgm.volume > 0)
        {
            bgm.volume -= Time.deltaTime * 0.5f;
            yield return new WaitForEndOfFrame();
        }

        bgm.clip = null;
        isOnFade = false;
    }
}
