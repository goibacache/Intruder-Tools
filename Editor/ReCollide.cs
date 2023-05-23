using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

#if (UNITY_EDITOR)



public class ReCollide : Editor
{
    [MenuItem("IntruderTools/ReCollide selected object %#r")]
    private static void NewMenuOption()
    {
        List<GameObject> gos = Selection.gameObjects.ToList();

        RecursiveRecollide(gos);
        //PlayerPrefs.DeleteAll();
    }

    private static void RecursiveRecollide(List<GameObject> currentLevel, bool root = true)
    {
        // Recollide the current one
        if (!root)
        {
            RecollideCurrent(currentLevel);
        }

        // Get childrens and do the same for each
        foreach (GameObject currentObject in currentLevel.ToList().Where(x => x != null))
        {
            if (currentObject.transform.childCount > 0)
            {
                List<GameObject> allChildrenGO = new List<GameObject>();

                List<Transform> allChildrenT = currentObject.transform.GetComponentsInChildren<Transform>().ToList();

                for (int i = 0; i < allChildrenT.Count - 1; i++)
                {
                    if (allChildrenT[i+1] != null)
                    {
                        allChildrenGO.Add(allChildrenT[i + 1].gameObject); // +1 to avoid the parents' one
                    }
                }

                RecursiveRecollide(allChildrenGO, false);
            }
        }
    }

    private static void RecollideCurrent(List<GameObject> currentLevel)
    {
        foreach (GameObject currentObject in currentLevel)
        {
            // Set as static
            var flags =
                            StaticEditorFlags.BatchingStatic |
                            StaticEditorFlags.ContributeGI |
                            StaticEditorFlags.ContributeGI |
                            StaticEditorFlags.NavigationStatic |
                            StaticEditorFlags.OccludeeStatic |
                            StaticEditorFlags.OccluderStatic |
                            StaticEditorFlags.OffMeshLinkGeneration |
                            StaticEditorFlags.ReflectionProbeStatic;
            GameObjectUtility.SetStaticEditorFlags(currentObject, flags);


            if (currentObject.name.StartsWith("nc_"))
            {
                continue;
            }

            Collider col = currentObject.GetComponent<Collider>();

            if (col)
            {
                Debug.Log("Quitando collider del tipo " + col.GetType());
                Object.DestroyImmediate(col);

                if (col.GetType() == typeof(BoxCollider))
                    currentObject.AddComponent<BoxCollider>();
                if (col.GetType() == typeof(MeshCollider))
                    currentObject.AddComponent<MeshCollider>();
                if (col.GetType() == typeof(CapsuleCollider))
                    currentObject.AddComponent<CapsuleCollider>();
                if (col.GetType() == typeof(SphereCollider))
                    currentObject.AddComponent<SphereCollider>();
            }else{
                // If no collider, add the mesh one ;)
                currentObject.AddComponent<MeshCollider>();
            }
        }
    }
}


#endif
