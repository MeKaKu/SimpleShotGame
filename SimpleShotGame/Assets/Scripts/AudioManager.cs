using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//声音管理
public class AudioManager : MonoBehaviour
{
    public enum AudioType {Main, Sound, Music};
    public float mainVolumePercent{get;private set;}//主音量
    public float soundVolumePercent{get;private set;}//音效音量
    public float musicVolumePercent{get;private set;}//音乐声音大小

    AudioSource[] musicSources;//音源数组，用来播放音乐
    AudioSource soundSource2D;//2D音效音源
    int currentMusicSourceIndex;//当前正在播放音乐的音源

    Transform listener;//音频接收对象
    Transform player;//玩家
    AudioLibrary audioLibrary;

    public static AudioManager instance;//单例模式

    private void Awake() {
        if(instance == null){ //单例模式
            instance = this;
            DontDestroyOnLoad(gameObject);//切换Scene时不摧毁
            musicSources = new AudioSource[2];//使用两个音源，方便实现音乐之间的平滑过渡
            for(int i=0; i<2; i++){
                GameObject newAudioSource = new GameObject("MusicSource"+i);
                musicSources[i] = newAudioSource.AddComponent<AudioSource>();
                newAudioSource.transform.parent = transform;
                musicSources[i].loop = true;
                musicSources[i].playOnAwake = false;
            }
            //2D音效音源
            GameObject newSoundSource = new GameObject("SoundSource2D");
            soundSource2D = newSoundSource.AddComponent<AudioSource>();
            newSoundSource.transform.parent = transform;

            if(FindObjectOfType<Player>()){
                player = FindObjectOfType<Player>().transform;//玩家
            }
            listener = transform.Find("AudioListener");//声音接收对象
            audioLibrary = GetComponent<AudioLibrary>();//音频库
            //读取用户对音量设置
            mainVolumePercent = PlayerPrefs.GetFloat("mainVolumePercent",1f);
            soundVolumePercent = PlayerPrefs.GetFloat("soundVolumePercent", 1f);
            musicVolumePercent = PlayerPrefs.GetFloat("musicVolumePercent", 1f);
        }
        else{
            Destroy(gameObject);
        }
    }
    //修改音量
    public void SetAudioVolume(AudioType audioType, float volumePercent){
        switch(audioType){
            case AudioType.Main: //主音量
                mainVolumePercent = volumePercent;
                break;
            case AudioType.Sound: //音效音量
                soundVolumePercent = volumePercent;
                break;
            case AudioType.Music: //音乐音量
                musicVolumePercent = volumePercent;
                break;
        }
        //修改音源的音量
        for(int i=0; i<2; i++){
            musicSources[i].volume = mainVolumePercent * musicVolumePercent;
        }
        //存下用户对音量设置
        PlayerPrefs.SetFloat("mainVolumePercent", mainVolumePercent);
        PlayerPrefs.SetFloat("soundVolumePercent", soundVolumePercent);
        PlayerPrefs.SetFloat("musicVolumePercent", musicVolumePercent);
        PlayerPrefs.Save();//保存玩家的设置
    }

    private void Update() {
        if(player != null){
            listener.position = player.position;//音频接收器跟随玩家移动
        }
        else{
            if(FindObjectOfType<Player>()){
                player = FindObjectOfType<Player>().transform;
            }
        }
    }

    //在指定地方播放音效
    public void PlaySound(AudioClip audioClip, Vector3 pos){
        if(audioClip != null){
            AudioSource.PlayClipAtPoint(audioClip, pos, mainVolumePercent * soundVolumePercent);
        }
    }
    //提过字符串（音频ID）获取音频，在指定位置播放音效
    public void PlaySound(string audioID, Vector3 pos){
        AudioClip audioClip = audioLibrary.GetAudioClipByID(audioID);//从音频库获取音频
        PlaySound(audioClip, pos);
    }
    //播放2D音效
    public void PlaySound2D(string audioID){
        AudioClip audioClip = audioLibrary.GetAudioClipByID(audioID);//从音频库获取音频
        if(audioClip != null){
            soundSource2D.clip = audioClip;
            soundSource2D.Play();
        }
    }
    //播放音乐，音乐之间平滑过渡
    public void PlayMusic(AudioClip audioClip, float fadeTime = 1f){
        if(audioClip == null) return;
        currentMusicSourceIndex ^= 1;
        musicSources[currentMusicSourceIndex].clip = audioClip;
        musicSources[currentMusicSourceIndex].Play();
        StartCoroutine(MusicCrossFade(fadeTime));//淡入淡出
    }
    //音乐之间淡入淡出效果的协程
    IEnumerator MusicCrossFade(float fadeTime){
        float percent = 0;
        float speed = 1f/fadeTime;
        float musicVolume = mainVolumePercent * musicVolumePercent;

        while(percent < 1){
            percent += Time.deltaTime * speed;
            musicSources[currentMusicSourceIndex].volume = Mathf.Lerp(0f, musicVolume, percent);//当前音乐淡入
            musicSources[currentMusicSourceIndex ^ 1].volume = Mathf.Lerp(musicVolume, 0f, percent);//当前音乐淡出
            yield return null;
        }
        musicSources[currentMusicSourceIndex ^ 1].Stop();
    }

}
