using System;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;//黑屏
    public GameObject gameOverUI;//游戏结束的UI
    People player;//玩家

    public RectTransform waveBanner;//关卡提示UI
    public Text currentWaveInfo;//关卡信息
    Spawner spawner;

    private void Awake() {
        player = GameObject.FindObjectOfType<People>();
        player.OnDeath += OnPlayerDeath;
        spawner = GameObject.FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    //游戏结束
    void OnPlayerDeath(){
        float speed = .5f;
        StartCoroutine(Fade(Color.clear, Color.black, speed));
        gameOverUI.SetActive(true);
    }
    //黑屏动画
    IEnumerator Fade(Color _from, Color _to, float _speed){
        float percent = 0;
        while(percent < 1){
            fadePlane.color = Color.Lerp(_from, _to, percent);
            percent += Time.deltaTime * _speed;
            yield return null;
        }
    }
    //关卡信息
    void OnNewWave(int waveNumber){
        currentWaveInfo.text = "- Wave " + waveNumber.ToString() + " -";
        StartCoroutine(AnimateNewWave());
    }
    IEnumerator AnimateNewWave(){
        float percent = 0;
        float upSpeed = 1f/.3f;
        float stopTime = 1.5f;
        int dir = 1;
        while(percent >= 0){
            percent += dir * upSpeed * Time.deltaTime;
            if(percent >= 1){
                percent = 1;
            }
            waveBanner.anchoredPosition = new Vector2(0, Mathf.Lerp(-60, 200, percent));
            if(percent == 1){
                dir = -1;
                yield return new WaitForSeconds(stopTime);
            }
            yield return null;
        }
    }

    //UI Input
    public void StartNewGame(){
        SceneManager.LoadScene("SampleScene");
    }
}
