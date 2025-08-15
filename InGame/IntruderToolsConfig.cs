using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[InitializeOnLoad]
public class IntruderToolsConfig : MonoBehaviour
{
    public static bool ShowColliders = false;
}
#endif
