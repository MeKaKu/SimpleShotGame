using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectButton : MonoBehaviour, IPointerEnterHandler {
    public RectTransform followingUIHolder;
    RectTransform self;

    private void Awake() {
        self = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData data){
        followingUIHolder.anchoredPosition = self.anchoredPosition + Vector2.left * (self.rect.width + followingUIHolder.rect.width)/2f ;
    }
}
