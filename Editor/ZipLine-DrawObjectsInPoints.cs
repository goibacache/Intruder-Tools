using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ZipLineDrawObjectsInPoints : MonoBehaviour
{
    [MenuItem("Intruder-Tools/Put objects in selected zipLine")]
    private static void DrawObjects()
    {
        List<GameObject> gos = Selection.gameObjects.ToList();

        GameObject go = gos.FirstOrDefault();

        ZiplineProxy zp = go.GetComponent<ZiplineProxy>();
        DrawLineInEditor dlie = go.GetComponent<DrawLineInEditor>();

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

        if (dlie.gameObjectToSpawn == null)
        {
            Debug.Log("No gameobject is set.");
            return;
        }

        GameObject clone;

        if (points.Count < 3) return;

        // Draw cubes
        for (int i = 1; i < points.Count - 1; i++)
        {
            //Get path to nearest (in case of nested) prefab from this gameObject in the scene
            string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(dlie.gameObjectToSpawn);
            Object assetPath = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(Object));

            // is prefab
            if (prefabPath != null && assetPath != null)
            {
                clone = PrefabUtility.InstantiatePrefab(dlie.gameObjectToSpawn) as GameObject; //Instantiate prefab
            }
            else
            {
                clone = Instantiate(dlie.gameObjectToSpawn, points[i], new Quaternion()); //Instantiate GameObject
            }

            if (clone == null) continue;

            clone.transform.position = points[i];
            clone.transform.localScale = new Vector3(1, 1, 1);
            clone.name = $"{dlie.gameObjectToSpawn.name}_{i}";
        }
    }

    
}
