using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLibrary : MonoBehaviour{
    public AudioGroup[] audioGroups;//音频数组
    Dictionary<string, AudioClip[]> audioDictionary;//Audio Map

    private void Awake() {
        audioDictionary = new Dictionary<string, AudioClip[]>();
        foreach(AudioGroup audioGroup in audioGroups){
            audioDictionary.Add(audioGroup.audioID, audioGroup.audioClips);
        }
    }
    //提过audioID获取音频
    public AudioClip GetAudioClipByID(string _audioID){
        if(audioDictionary.ContainsKey(_audioID)){
            AudioClip[] audioClips = audioDictionary[_audioID];
            return audioClips[Random.Range(0, audioClips.Length - 1)];
        }
        return null;
    }

    [System.Serializable]
    public class AudioGroup{
        public string audioID;//音频id
        public AudioClip[] audioClips;//音频
    }
}
