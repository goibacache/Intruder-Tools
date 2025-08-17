#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class RandomDelayInActivator : EditorWindow
{
    private static float minDelay = 0.0f;
    private static float maxDelay = 1f;

    private static string minDelayKey = "RDIA-Min";
    private static string maxDelayKey = "RDIA-Max";

    [MenuItem("Intruder-Tools/Random delay in activator")]
    public static void ShowWindow()
    {
        RandomDelayInActivator wnd = GetWindow<RandomDelayInActivator>();
        wnd.titleContent = new GUIContent("Randomly delay in activator");

        EditorPrefs.SetFloat(minDelayKey, 0.0f);
        EditorPrefs.SetFloat(maxDelayKey, 1f);
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        var titleLabel = new Label("Randomly assign a delay to selected elements activators. This will iterate through all activators and their children");
        titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        titleLabel.style.whiteSpace = WhiteSpace.Normal;
        root.Add(titleLabel);

        float minDelayFloat = EditorPrefs.GetFloat(minDelayKey, minDelay);
        UnityEditor.UIElements.FloatField minValue = new UnityEditor.UIElements.FloatField("Min Value:");
        minValue.value = minDelayFloat;
        root.Add(minValue);

        // Register a callback to save the value whenever it changes
        minValue.RegisterValueChangedCallback(evt =>
        {
            EditorPrefs.SetFloat(minDelayKey, evt.newValue);
            minDelay = evt.newValue;
        });

        float maxDelayFloat = EditorPrefs.GetFloat(maxDelayKey, maxDelay);
        UnityEditor.UIElements.FloatField maxValue = new UnityEditor.UIElements.FloatField("Max Value:");
        maxValue.value = maxDelayFloat;
        maxValue.name = maxDelayKey;
        root.Add(maxValue);

        // Register a callback to save the value whenever it changes
        maxValue.RegisterValueChangedCallback(evt =>
        {
            EditorPrefs.SetFloat(maxDelayKey, evt.newValue);
            maxDelay = evt.newValue;
        });

        Button randomizeButton = new Button();
        randomizeButton.text = "Randomize delay of selected activators & childs";
        randomizeButton.clicked += AssignDelay;
        root.Add(randomizeButton);
    }

    public static void AssignDelay()
    {
        GameObject[] currentSelection = Selection.gameObjects;

        if (currentSelection.Length == 0)
        {
            Debug.LogWarning("Please select an object from your hierarchy.");
            return;
        }

        minDelay = EditorPrefs.GetFloat(minDelayKey);
        maxDelay = EditorPrefs.GetFloat(maxDelayKey);

        Debug.Log($"{minDelay}-{maxDelay}");

        ProcessDelay(currentSelection);
    }

    private static void ProcessDelay(GameObject[] gameObjects)
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            GameObject current = gameObjects[i];
            Activator activator = current.GetComponent<Activator>();
            if (activator != null)
            {
                activator.delayTime = float.Parse(Random.Range(minDelay, maxDelay).ToString("F2"));
                Debug.Log($"RandomDelayInActivator: {current.name} assigned delay of {activator.delayTime}. From [{minDelay.ToString("F2")}-{maxDelay.ToString("F2")}]");
            }

            // Get all children
            GameObject[] allChildrenGO = current.transform.GetComponentsInChildren<Transform>(true)
                                            .Select((transform, index) => new { Transform = transform, Index = index }) // anonymous object that also has the index
                                            .Where(item => item.Index > 0) // Remove index 0 because unity is weird and gives you the parent as the first element
                                            .Select(item => item.Transform.gameObject) // Get only the GameObjects
                                            .ToArray();

            if (allChildrenGO.Length > 0)
            {
                ProcessDelay(allChildrenGO);
            }
        }
    }
}

#endif