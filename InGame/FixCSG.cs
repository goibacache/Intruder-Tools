using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class FixCSG : MonoBehaviour
{
    /*
     * Big thanks to Zapan15 in the unity forums for comming up with the code below.
     * I just modified it to run on the parent of a CSG
     * https://forum.unity.com/threads/progressive-gpu-error-failed-to-add-geometry-for-mesh-stud-mesh-is-missing-required-attribute-s.976230/#post-7092433
     */

    #if UNITY_EDITOR
    [MenuItem("XixoTools/CSG/Find & ping broken CSG")]
    private static void FindBrokenCSG()
    {
        ActuallyFind(ping: true, deactivate: false);
    }

    [MenuItem("XixoTools/CSG/Deactivate broken CSG")]
    private static void DeactivateBrokenCSG()
    {
        ActuallyFind(ping: false, deactivate: true);
    }

    private static void ActuallyFind(bool ping, bool deactivate) {
        try
        {
            // Let's save all of the GameObjects here.
            List<GameObject> allChildrenGO = new List<GameObject>();

            //Get the current one
            GameObject currentObject = Selection.gameObjects.FirstOrDefault();

            // Get all of the transforms
            List<Transform> allChildrenT = currentObject.transform.GetComponentsInChildren<Transform>().ToList();

            // Fill the children with gameobjects
            for (int i = 1; i < allChildrenT.Count - 1; i++)
            {
                if (allChildrenT[i] != null)
                {
                    allChildrenGO.Add(allChildrenT[i].gameObject); // +1 to avoid the parents' one
                }
            }

            double ammountDeactivated = 0;
            bool objectWasDeactivated = false;

            foreach (GameObject go in allChildrenGO.Where(
                x =>
                    !x.name.ToLower().Contains("meshgroup") &&
                    !x.name.ToLower().Contains("materialmesh") &&
                    !x.name.ToLower().Contains("collisionmesh")
                ))
            {
                if (go.activeInHierarchy == false) continue;

                MeshFilter meshFilter = go.GetComponent<MeshFilter>();

                if (meshFilter == null) continue;

                Mesh sm = meshFilter.sharedMesh;

                objectWasDeactivated = false;

                // Check for bad vertexs
                Vector3[] v3Verts = sm.vertices;
                for (int i = 0; i < v3Verts.Length; i++)
                {
                    if (float.IsNaN(v3Verts[i].x) || float.IsNaN(v3Verts[i].y) || float.IsNaN(v3Verts[i].z))
                    {
                        if (ping)
                        {
                            EditorGUIUtility.PingObject(go); // Ping the object, then dies.
                            return;
                        }
                        if (deactivate)
                        {
                            objectWasDeactivated = true;
                            go.SetActive(false);
                        }
                    }
                }

                // Check the UVs!
                Vector2[] verts = sm.uv;
                for (int i = 0; i < verts.Length; i++)
                {
                    if (float.IsNaN(verts[i].x) || float.IsNaN(verts[i].y))
                    {
                        if (ping)
                        {
                            EditorGUIUtility.PingObject(go); // Ping the object, then dies.
                            return;
                        }
                        if (deactivate)
                        {
                            objectWasDeactivated = true;
                            go.SetActive(false);
                        }
                    }
                }

                if (objectWasDeactivated)
                {
                    ammountDeactivated++;
                }
            }

            if (ammountDeactivated == 0)
            {
                Debug.Log("Your CSG is good to go! Bake away!");
            }
            else
            {
                Debug.Log($"{ammountDeactivated} CSG brushes where deactivated. Your CSG is good to go! Bake away!");
            }

            
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    #endif
}