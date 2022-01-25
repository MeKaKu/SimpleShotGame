using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
    public GameObject mainMenu;//主菜单
    public GameObject optionsMenu;//设置菜单

    public Slider[] sliders;//音量滑条
    public Dropdown resolutionsDropdown;//分辨率下拉菜单
    public int[] resolutionWidths;//分辨率的宽

    private void Start() {
        //获取玩家的设置
        sliders[0].value = AudioManager.instance.mainVolumePercent;//主音量
        sliders[1].value = AudioManager.instance.musicVolumePercent;//音乐
        sliders[2].value = AudioManager.instance.soundVolumePercent;//音效

        resolutionsDropdown.value = PlayerPrefs.GetInt("ResolutionIndex", 0);//分辨率
        SetResolution(resolutionsDropdown.value);
    }

    //开始游戏
    public void Play(){
        SceneManager.LoadScene("SampleScene");
    }

    //主菜单跳转到设置菜单
    public void MenuToOptions(){
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    //退出游戏
    public void Quit(){
        Application.Quit();
    }

    //设置菜单跳转到主菜单
    public void OptionsToMenu(){
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    //设置主音量
    public void SetMainVolume(float _volume){
        AudioManager.instance.SetAudioVolume(AudioManager.AudioType.Main, _volume);
    }

    //设置音乐音量
    public void SetMusicVolume(float _volume){
        AudioManager.instance.SetAudioVolume(AudioManager.AudioType.Music, _volume);
    }

    //设置音效音量
    public void SetSoundVolume(float _volume){
        AudioManager.instance.SetAudioVolume(AudioManager.AudioType.Sound, _volume);
    }

    //设置分辨率
    public void SetResolution(int index){
        PlayerPrefs.SetInt("ResolutionIndex", index);//保存玩家对分辨率的设置
        PlayerPrefs.Save();
        if(index == resolutionWidths.Length - 1){
            //全屏
            Resolution[] allResolutions = Screen.resolutions;//屏幕的分辨率
            Resolution maxResolution = allResolutions[allResolutions.Length - 1];//最大分辨率
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);//全屏
        }
        else{
            Screen.SetResolution(resolutionWidths[index], resolutionWidths[index]/16*9, false);
        }
    }
}
