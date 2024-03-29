﻿using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SwitchShaders : EditorWindow
{
    [MenuItem("Tools/Shader Occurence")]
    public static void Open()
    {
        GetWindow<SwitchShaders>();
    }

    Shader shader;
    Shader newshader;

    List<string> materials = new List<string>();
    Vector2 scroll;

    void OnGUI()
    {
        Shader prev = shader;
        shader = EditorGUILayout.ObjectField(shader, typeof(Shader), false) as Shader;
        newshader = EditorGUILayout.ObjectField(newshader, typeof(Shader), false) as Shader;
        if (shader != prev)
        {
            string shaderPath = AssetDatabase.GetAssetPath(shader);
            string[] allMaterials = AssetDatabase.FindAssets("t:Material");
            materials.Clear();
            for (int i = 0; i < allMaterials.Length; i++)
            {
                allMaterials[i] = AssetDatabase.GUIDToAssetPath(allMaterials[i]);
                string[] dep = AssetDatabase.GetDependencies(allMaterials[i]);
                if (ArrayUtility.Contains(dep, shaderPath))
                    materials.Add(allMaterials[i]);
            }
        }

        scroll = GUILayout.BeginScrollView(scroll);
        {
            for (int i = 0; i < materials.Count; i++)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(Path.GetFileNameWithoutExtension(materials[i]));
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Show"))
                        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(materials[i], typeof(Material)));
                }
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndScrollView();

        if (GUILayout.Button("Apply"))
        {
            for (int i = 0; i < materials.Count; i++)
            {
                var mat = AssetDatabase.LoadAssetAtPath(materials[i], typeof(Material)) as Material;
                mat.shader = newshader;
            }
        }
    }
}