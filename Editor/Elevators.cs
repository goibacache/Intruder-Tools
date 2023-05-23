using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.Text.RegularExpressions;

#if (UNITY_EDITOR)



public class Elevators : EditorWindow
{
    public static   int                 animationFPS        { get; set; }
    public static   int                 timeBetweenFloors   { get; set; }
    public static   List<GameObject>    selectedGameObject  { get; set; }
    public static   List<GameObject>    allFloors           { get; set; }


    private static  Elevators           window              { get; set; }

    [MenuItem("XixoTools/Create elevator from tree %#r")]
    private static void NewMenuOption()
    {
        animationFPS = 60;
        timeBetweenFloors = 2;
        allFloors = new List<GameObject>();
        window = (Elevators)EditorWindow.GetWindow(typeof(Elevators));

        // Get existing open window or if none, make a new one:
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 190);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Create elevator from tree by Xixo", EditorStyles.boldLabel);
        GUILayout.Space(10);

        animationFPS = Convert.ToInt32(EditorGUILayout.TextField("Animation FPS:", animationFPS + "").Replace(@"[^a-zA-Z0-9 ]", ""));
        EditorGUILayout.LabelField("    * The FPS the animations will be.");

        timeBetweenFloors = Convert.ToInt32(EditorGUILayout.TextField("Time between floor:", timeBetweenFloors + "").Replace(@"[^a-zA-Z0-9 ]", ""));
        EditorGUILayout.LabelField("    * Time between floors is the equivalent of FPS by");
        EditorGUILayout.LabelField("      the number on the field.");

        GUILayout.Space(10);

        if (GUILayout.Button("Create elevators")) CreateElevators();
        if (GUILayout.Button("Close")) this.Close();
    }

    private static void CreateElevators() {
        // Animations
        CreateAnimations();
        // Assign animations
        AssignAnimations();


        DeactivateBase();
        // Create final
    }



    private static void CreateAnimations()
    {
        getFloorsObjects();

        for (int i = 0; i < allFloors.Count; i++)
        {
            GameObject currentFloor = allFloors[i];

            for (int e = 0; e < allFloors.Count; e++)
            {
                if (i != e) // Only create animations to OTHER floors
                {
                    GameObject tagetFloor = allFloors[e];

                    AnimationClip clip = new AnimationClip();

                    string name = $"{currentFloor.name}_to_{tagetFloor.name}";

                    clip.legacy = true;
                    clip.wrapMode = WrapMode.Once;
                    clip.frameRate = animationFPS;
                    clip.name = name;

                    // Get "distance" which multiplies the timeBetweenFloors between distance
                    int distanceToSelectedFloor = Math.Abs(i - e);

                    clip.SetCurve("", typeof(Transform), "localPosition.x", AnimationCurve.Linear(0, currentFloor.transform.localPosition.x, timeBetweenFloors * distanceToSelectedFloor, tagetFloor.transform.localPosition.x));
                    clip.SetCurve("", typeof(Transform), "localPosition.y", AnimationCurve.Linear(0, currentFloor.transform.localPosition.y, timeBetweenFloors * distanceToSelectedFloor, tagetFloor.transform.localPosition.y));
                    clip.SetCurve("", typeof(Transform), "localPosition.z", AnimationCurve.Linear(0, currentFloor.transform.localPosition.z, timeBetweenFloors * distanceToSelectedFloor, tagetFloor.transform.localPosition.z));

                    // Create folder to save animations, if it doesn't exists
                    if (!AssetDatabase.IsValidFolder("Assets/Elevator"))
                    {
                        AssetDatabase.CreateFolder("Assets", "Elevator");
                    }

                    // Create our new anim
                    AssetDatabase.CreateAsset(clip, $"Assets/Elevator/{name}.anim");

                }
            }
        }
    }

    private static void getFloorsObjects()
    {
        selectedGameObject = Selection.gameObjects.ToList();
        allFloors = new List<GameObject>();

        foreach (GameObject currentObject in selectedGameObject.ToList().Where(x => x != null))
        {
            // If it has childrens
            if (currentObject.transform.childCount > 0)
            {
                // Get every children
                List<Transform> allChildrenT = currentObject.transform.GetComponentsInChildren<Transform>().ToList();

                for (int i = 0; i < allChildrenT.Count - 1; i++)
                {
                    if (allChildrenT[i + 1] != null)
                    {
                        allFloors.Add(allChildrenT[i + 1].gameObject); // +1 to avoid the parents' one
                    }
                }

            }
        }
    }

    private static void AssignAnimations()
    {
        // Remove ALL animation components, first.
        foreach (GameObject floor in allFloors)
        {
            Animation component = floor.GetComponent<Animation>();

            if (component != null)
            {
                DestroyImmediate(component);
            }
        }


        for (int i = 0; i < allFloors.Count; i++)
        {
            GameObject currentFloor = allFloors[i];

            // Add animation component
            Animation animation = currentFloor.GetComponent<Animation>();
            if (currentFloor.GetComponent<Animation>() == null)
            {
                animation = currentFloor.AddComponent<Animation>();
            }
            animation.playAutomatically = false;

            // Add mover component
            Mover mover = currentFloor.GetComponent<Mover>();
            if (mover == null)
            {
                mover = currentFloor.AddComponent<Mover>();
            }

            // Add Activator
            Activator activator = currentFloor.GetComponent<Activator>();
            if (activator == null)
            {
                activator = currentFloor.AddComponent<Activator>();
            }


            for (int e = 0; e < allFloors.Count; e++)
            {
                // If it's not the same floor...
                if (i != e)
                {
                    
                    GameObject tagetFloor   = allFloors[e];

                    string nameOnly = $"{currentFloor.name}_to_{tagetFloor.name}";
                    string name     = $"Assets/Elevator/{nameOnly}.anim";

                    AnimationClip clip = (AnimationClip)AssetDatabase.LoadAssetAtPath(name, typeof(AnimationClip));

                    if (clip != null)
                    {
                        animation.AddClip(clip, nameOnly);
                    }

                    

                    /*
                    currentFloor.AddComponent(new Activator()
                    {
                        canRedo = true,
                        redoTime = timeBetweenFloors
                    });
                    */
                }
            }
        }
    }

    private static void DeactivateBase()
    {
        foreach (GameObject go in selectedGameObject)
        {
            go.SetActive(false);
        }
    }
}


#endif