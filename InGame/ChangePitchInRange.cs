#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ChangePitchInRange : EditorWindow
{
    private static float minPitch = 0.9f;
    private static float maxPitch = 1.1f;

    private static string minPitchKey = "CPIR-Min";
    private static string maxPitchKey = "CPIR-Max";

    [MenuItem("Intruder-Tools/Random/Random Audio Source Pitch")]
    public static void ShowWindow()
    {
        ChangePitchInRange wnd = GetWindow<ChangePitchInRange>();
        wnd.titleContent = new GUIContent("Randomly assign pitch");
        wnd.minSize = new Vector2(490, 100);
        wnd.maxSize = wnd.minSize;

        EditorPrefs.SetFloat(minPitchKey, 0.9f);
        EditorPrefs.SetFloat(maxPitchKey, 1.1f);
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        var titleLabel = new Label("Randomly assign pitch to selected elements that have an AudioSource on them.\nNote: This will iterate through all the selected GameObjects and their children");
        titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        titleLabel.style.whiteSpace = WhiteSpace.Normal;
        root.Add(titleLabel);

        float minPitchFloat = EditorPrefs.GetFloat(minPitchKey, minPitch);
        UnityEditor.UIElements.FloatField minValue = new UnityEditor.UIElements.FloatField("Min pitch value:");
        minValue.value = minPitchFloat;
        root.Add(minValue);

        // Register a callback to save the value whenever it changes
        minValue.RegisterValueChangedCallback(evt =>
        {
            EditorPrefs.SetFloat(minPitchKey, evt.newValue);
            minPitch = evt.newValue;
        });

        float maxPitchFloat = EditorPrefs.GetFloat(maxPitchKey, maxPitch);
        UnityEditor.UIElements.FloatField maxValue = new UnityEditor.UIElements.FloatField("Max pitch value:");
        maxValue.value = maxPitchFloat;
        maxValue.name = maxPitchKey;
        root.Add(maxValue);

        // Register a callback to save the value whenever it changes
        maxValue.RegisterValueChangedCallback(evt =>
        {
            EditorPrefs.SetFloat(maxPitchKey, evt.newValue);
            maxPitch = evt.newValue;
        });

        Button randomizeButton = new Button();
        randomizeButton.text = "Randomize pitch of Audio Source on selected objects & childs";
        randomizeButton.clicked += AssignSounds;
        root.Add(randomizeButton);
    }

    public static void AssignSounds()
    {
        GameObject[] currentSelection = Selection.gameObjects;

        if (currentSelection.Length == 0)
        {
            Debug.LogWarning("Please select an object from your hierarchy");
            return;
        }

        minPitch = EditorPrefs.GetFloat(minPitchKey);
        maxPitch = EditorPrefs.GetFloat(maxPitchKey);

        Debug.Log($"ChangePitchInRange: from {minPitch} to {maxPitch}");

        ProcessPitchChange(currentSelection);
    }

    private static void ProcessPitchChange(GameObject[] gameObjects)
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            GameObject current = gameObjects[i];
            AudioSource audioSource = current.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.pitch = float.Parse(Random.Range(minPitch, maxPitch).ToString("F2"));
                Debug.Log($"ChangePitchInRange: {current.name} assigned pitch of {audioSource.pitch}. From [{minPitch.ToString("F2")}-{maxPitch.ToString("F2")}]");
            }

            // Get all children
            GameObject[] allChildrenGO = current.transform.GetComponentsInChildren<Transform>(true)
                                            .Select((transform, index) => new { Transform = transform, Index = index }) // anonymous object that also has the index
                                            .Where(item => item.Index > 0) // Remove index 0 because unity is weird and gives you the parent as the first element
                                            .Select(item => item.Transform.gameObject) // Get only the GameObjects
                                            .ToArray();

            if (allChildrenGO.Length > 0)
            {
                ProcessPitchChange(allChildrenGO);
            }
        }
    }
}

#endif