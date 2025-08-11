using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IntruderToolsAbout : MonoBehaviour
{
    #if UNITY_EDITOR
    [MenuItem("Intruder-Tools/About", priority=9999)]
    private static void DisplayAbout()
    {
        bool answer = EditorUtility.DisplayDialog("Intruder-Tools by Xixo", $"Join us on SuperBoss's discord or visit the project's repository to check for updates 💖.\n\n© Xixo {DateTime.Now.Year}", "Ok", "Visit project's repository");
        if (!answer)
        {
            Application.OpenURL("https://github.com/goibacache/Intruder-Tools");
        }
    }

    #endif
}
