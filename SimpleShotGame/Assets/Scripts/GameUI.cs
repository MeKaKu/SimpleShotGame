using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;
    public GameObject gameOverUI;

    People player;

    private void Start() {
        player = GameObject.FindObjectOfType<People>();
        player.OnDeath += OnPlayerDeath;
    }

    void OnPlayerDeath(){
        float speed = .5f;
        StartCoroutine(Fade(Color.clear, Color.black, speed));
        gameOverUI.SetActive(true);
    }

    IEnumerator Fade(Color _from, Color _to, float _speed){
        float percent = 0;
        while(percent < 1){
            fadePlane.color = Color.Lerp(_from, _to, percent);
            percent += Time.deltaTime * _speed;
            yield return null;
        }
    }
}
