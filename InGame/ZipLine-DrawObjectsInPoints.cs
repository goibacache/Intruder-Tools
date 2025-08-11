using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ZipLineDrawObjectsInPoints : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Intruder-Tools/Put objects in selected zipLine")]
    private static void DrawObjects()
    {
        List<GameObject> gos = Selection.gameObjects.ToList();

        GameObject go = gos.FirstOrDefault();

        ZiplineProxy zp = go.GetComponent<ZiplineProxy>();
        DrawZipLineInEditor dlie = go.GetComponent<DrawZipLineInEditor>();

        if (zp == null || dlie == null)
        {
            Debug.Log("Zipline doesn't have ZipLineProxy or DrawLineInEditor Script");
            return;
        }

        List<Vector3> points = dlie.points;

        if (points == null)
        {
            Debug.Log("No points where found.");
            return;
        }

        if (dlie.prefabToSpawnAtZiplineIntersections == null)
        {
            Debug.Log("No prefab is set.");
            return;
        }

        GameObject clone;

        if (points.Count < 3) return;

        GameObject parent = dlie.gameObject;

        // Draw cubes
        for (int i = 1; i < points.Count - 1; i++)
        {
            //Get path to nearest (in case of nested) prefab from this gameObject in the scene
            string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(dlie.prefabToSpawnAtZiplineIntersections);
            Object assetPath = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(Object));

            // is prefab
            if (prefabPath != null && assetPath != null)
            {
                clone = PrefabUtility.InstantiatePrefab(dlie.prefabToSpawnAtZiplineIntersections, parent.transform) as GameObject; //Instantiate prefab
            }
            else
            {
                clone = Instantiate(dlie.prefabToSpawnAtZiplineIntersections, points[i], new Quaternion(), parent.transform); //Instantiate GameObject
            }

            if (clone == null) continue;

            clone.transform.position = points[i];
            clone.transform.localScale = new Vector3(1, 1, 1);
            clone.name = $"{dlie.prefabToSpawnAtZiplineIntersections.name}_{i}";
            
        }
    }

#endif
}