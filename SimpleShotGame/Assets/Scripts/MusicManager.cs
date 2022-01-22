using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip mainMusic;//主音乐
    public AudioClip menuMusic;//菜单音乐

    private void Start() {
        AudioManager.instance.PlayMusic(menuMusic);
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space)){
            AudioManager.instance.PlayMusic(mainMusic);
        }
    }
}
