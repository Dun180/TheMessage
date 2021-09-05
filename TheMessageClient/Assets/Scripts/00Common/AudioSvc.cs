//音频服务


using UnityEngine;



public class AudioSvc : MonoBehaviour
{

    public static AudioSvc Instance;

    public AudioSource bgAudio;
    public AudioSource effectAudio;
    public AudioSource uiAudio;
    public AudioSource operateAudio;

    // Start is called before the first frame update
    public void Awake()
    {
        Instance = this;
    }

    public void PlayUIAudio(string name){
        AudioClip audio = Resources.Load<AudioClip>("Audio/UI/"+name);
        uiAudio.clip = audio;
        uiAudio.Play();
    }

}