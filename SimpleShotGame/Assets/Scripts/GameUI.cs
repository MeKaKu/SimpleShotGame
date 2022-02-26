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
    Player player;//玩家

    public RectTransform waveBanner;//关卡提示UI
    public Text currentWaveInfo;//关卡信息
    public Text weaponInfo;//武器信息

    public Text ScoreUI;//得分
    public Image healthBar;//玩家血条
    public Image healthBarDis;
    Spawner spawner;

    private void Awake() {
        player = GameObject.FindObjectOfType<Player>();
        player.OnDeath += OnPlayerDeath;
        player.OnTakeDamage += OnPlayerTakeDamage;
        spawner = GameObject.FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    private void Update() {
        float healthPercent = 0;
        if(player != null){
            Gun gun = player.GetComponent<GunController>().equippedGun;
            if(gun != null){
                weaponInfo.text = gun.projectsRemainedInMagazine + "/" + gun.magazineCapacity;
            }

            ScoreUI.text = ScoreManager.score.ToString("D6"); //分数

            healthPercent = player.health / player.startingHealth;
        }
        healthBar.transform.localScale = new Vector3(healthPercent, 1, 1);//玩家血条
    }

    //玩家受伤血条变化
    void OnPlayerTakeDamage(){
        //healthBarDis.transform.localScale = healthBar.transform.localScale;
        StopCoroutine("AnimateHealthBar");
        StartCoroutine("AnimateHealthBar");
    }
    //血条变化的动画
    IEnumerator AnimateHealthBar(){
        healthBarDis.CrossFadeAlpha(1, .1f, true);
        yield return new WaitForSeconds(.1f);
        healthBarDis.CrossFadeAlpha(0, 1, true);
        float fadingTime = 1f;
        float percent = 0;
        Vector3 originScale = healthBarDis.transform.localScale;
        Vector3 targetScale = healthBar.transform.localScale;
        while(percent < 1){
            percent += Time.deltaTime / fadingTime;
            healthBarDis.transform.localScale = Vector3.Lerp(originScale, targetScale, percent);
            yield return null;
        }
    }
    //游戏结束
    void OnPlayerDeath(){
        float speed = .5f;
        StartCoroutine(Fade(Color.clear, Color.black, speed));
        ScoreUI.rectTransform.anchoredPosition = Vector2.zero;
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
        currentWaveInfo.text = "- 关卡 " + waveNumber.ToString() + " -";
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
    public void BackToMenu(){
        SceneManager.LoadScene("MenuScene");
    }
}
