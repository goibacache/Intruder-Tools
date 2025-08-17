#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ChangePitchInRange : EditorWindow
{
    private static float minPitch = 0.9f;
    private static float maxPitch = 1.1f;

    private static string minPitchKey = "CPIR-Min";
    private static string maxPitchKey = "CPIR-Max";

    [MenuItem("Intruder-Tools/Randomly assign pitch")]
    public static void ShowWindow()
    {
        ChangePitchInRange wnd = GetWindow<ChangePitchInRange>();
        wnd.titleContent = new GUIContent("Randomly assign pitch");

        EditorPrefs.SetFloat(minPitchKey, 0.9f);
        EditorPrefs.SetFloat(maxPitchKey, 1.1f);
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        var titleLabel = new Label("Randomly assign pitch to selected elements that have an AudioSource on them.");
        titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        titleLabel.style.whiteSpace = WhiteSpace.Normal;
        root.Add(titleLabel);

        float minPitchFloat = EditorPrefs.GetFloat(minPitchKey, minPitch);
        UnityEditor.UIElements.FloatField minValue = new UnityEditor.UIElements.FloatField("Min Value:");
        minValue.value = minPitchFloat;
        root.Add(minValue);

        // Register a callback to save the value whenever it changes
        minValue.RegisterValueChangedCallback(evt =>
        {
            EditorPrefs.SetFloat(minPitchKey, evt.newValue);
            minPitch = evt.newValue;
        });

        float maxPitchFloat = EditorPrefs.GetFloat(maxPitchKey, maxPitch);
        UnityEditor.UIElements.FloatField maxValue = new UnityEditor.UIElements.FloatField("Max Value:");
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
        randomizeButton.text = "Randomize pitch of selected objects";
        randomizeButton.clicked += AssignSounds;
        root.Add(randomizeButton);
    }

    public static void AssignSounds()
    {
        GameObject[] currentSelection = Selection.gameObjects;

        if (currentSelection.Length == 0)
        {
            Debug.LogWarning("Please select an object from your hierarchy that has an audio source on it.");
            return;
        }

        minPitch = EditorPrefs.GetFloat(minPitchKey);
        maxPitch = EditorPrefs.GetFloat(maxPitchKey);

        Debug.Log($"{minPitch}-{maxPitch}");

        for (int i = 0; i < currentSelection.Length; i++)
        {
            GameObject current = currentSelection[i];
            AudioSource audio = current.GetComponent<AudioSource>();
            if (audio == null)
            {
                Debug.LogWarning($"ChangePitchInRange: {current.name} has no audio source script");
                continue; // No audio source
            }
            audio.pitch = float.Parse(Random.Range(minPitch, maxPitch).ToString("F2"));
            Debug.Log($"ChangePitchInRange: {current.name} assigned the pitch of {audio.pitch}. From [{minPitch.ToString("F2")}-{maxPitch.ToString("F2")}]");
        }
    }
}

#endif