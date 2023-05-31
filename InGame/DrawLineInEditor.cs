using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DrawLineInEditor : MonoBehaviour
{
    #if UNITY_EDITOR    

    public List<Vector3> points;
    public GameObject gameObjectToSpawn;
    ZiplineProxy zipLineProxy;
    List<Color> colors;
    private Vector3 start;
    private Vector3 end;
    private float vertices;
    private float maxDangle;

    private void loadColors()
    {
        // DEBUG:
        colors = new List<Color>();
        colors.Add(Color.red);
        colors.Add(Color.white);
        colors.Add(Color.blue);
        colors.Add(Color.green);
        // DEBUG:
        //colors = new List<Color>();
        //colors.Add(Color.black);
        //colors.Add(Color.black);
        //colors.Add(Color.black);
        //colors.Add(Color.black);
    }

    private void OnDrawGizmos()
    {
        
        if (colors == null || colors.Count == 0)
        {
            loadColors();
        }

        if (zipLineProxy == null)
        {
            zipLineProxy = GetComponent<ZiplineProxy>();
        }

        // If it haven't been touched, don't calculate, just draw.
        if (start == zipLineProxy.startPoint.transform.position && end == zipLineProxy.endPoint.transform.position && vertices == zipLineProxy.numberOfVertices && maxDangle == zipLineProxy.maxGravityDangle)
        {
            DrawPoints();
            return;
        }

        // Clean the list when re-procesing
        points = new List<Vector3>();

        start   = zipLineProxy.startPoint.transform.position;
        end     = zipLineProxy.endPoint.transform.position;

        vertices = zipLineProxy.numberOfVertices-1;
        maxDangle = zipLineProxy.maxGravityDangle;

        float distance = Vector3.Distance(start, end);
        float step = distance / vertices;

        // Start
        points.Add(start);

        // Rest (minus start & end)
        for (int i = 1; i <= vertices; i++)
        {
            Vector3 middlePoint = LerpByDistance(start, end, step * i);

            middlePoint.y -= Mathf.Sin(i * (step / (distance / Mathf.PI))) * maxDangle;

            points.Add(middlePoint);
        }

        DrawPoints();
    }

    private void DrawPoints()
    {
        if (points == null)
        {
            return;
        }
        // Draw points
        for (int i = 1; i < points.Count; i++)
        {
            Gizmos.color = colors[i % 4];
            Gizmos.DrawLine(points[i - 1], points[i]);
            Gizmos.DrawWireSphere(points[i - 1], 0.1f);
        }

        // Start
        Gizmos.DrawWireSphere(points[points.Count - 1], 0.1f);
    }

    private Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
    {
        Vector3 P = x * Vector3.Normalize(B - A) + A;
        return P;
    }

    #endif
}