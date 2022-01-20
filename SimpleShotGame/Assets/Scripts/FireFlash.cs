using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFlash : MonoBehaviour
{
    public GameObject flashHolder;//闪光对象
    public Sprite[] flashSprites;//闪光贴图
    public SpriteRenderer[] flashRenderers;//贴图显示器
    public float flashTime = 0.1f;//闪光持续时间

    private void Start() {
        Deactivate();
    }

    //闪光
    public void Activate(){
        flashHolder.SetActive(true);
        int index = Random.Range(0, flashSprites.Length - 1);
        for(int i = 0; i < flashRenderers.Length; i++){
            flashRenderers[i].sprite = flashSprites[index];
        }

        Invoke("Deactivate", flashTime);
    }
    //取消闪光
    public void Deactivate(){
        flashHolder.SetActive(false);
    }
}
