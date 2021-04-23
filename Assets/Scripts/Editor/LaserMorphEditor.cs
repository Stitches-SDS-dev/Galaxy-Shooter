using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LaserMorph))]
public class LaserMorphEditor : Editor
{/*
    public override void OnInspectorGUI() {

        DrawDefaultInspector();

        LaserMorph custom = (LaserMorph)target;

        if (GUILayout.Button("Draw Debug")) {
            custom.DrawDebug();
        }

        if (GUILayout.Button("Change Sprite")) {
            custom.ChangeSprite();
        }
    }*/
}
