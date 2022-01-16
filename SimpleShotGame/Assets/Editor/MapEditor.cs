using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]//编辑器针对的对象
public class MapEditor : Editor{ //扩展编辑器
    public override void OnInspectorGUI(){
        base.OnInspectorGUI();
        MapGenerator map = target as MapGenerator;
        map.GenerateMap();
    }
}
