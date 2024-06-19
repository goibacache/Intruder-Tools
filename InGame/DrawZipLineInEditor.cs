using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DrawZipLineInEditor : MonoBehaviour
{
    #if UNITY_EDITOR    

    private static List<Vector3> points;
    ZiplineProxy zipLineProxy;
    List<Color> colors;
    private Vector3 start;
    private Vector3 end;
    private float numberOfVertices;
    private float maxDangle;

    // Dismount calc:
    private float forceDismountAtPercent;
    private float whereInZipLine;
    private int   whereInZipLineInt;
    private Vector3 dismountPoint;

    private static void DrawObjects()
    {
        if (points == null)
        {
            Debug.Log("No points where found.");
            return;
        }

        GameObject go;
        float scale = 0.1f;

        // Draw cubes
        for (int i = 0; i < points.Count; i++)
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.position = points[i];
            go.transform.localScale = new Vector3(scale, scale, scale);
            go.name = $"Point {i}";
        }
    }

    private void loadColors()
    {
        colors = new List<Color>();
        colors.Add(Color.red);
        colors.Add(Color.white);
        colors.Add(Color.blue);
        colors.Add(Color.green);
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
        if (start == zipLineProxy.startPoint.transform.position && end == zipLineProxy.endPoint.transform.position && numberOfVertices == zipLineProxy.numberOfVertices && maxDangle == zipLineProxy.maxGravityDangle)
        {
            DrawPoints();
            return;
        }

        // Clean the list when re-procesing
        points = new List<Vector3>();

        start   = zipLineProxy.startPoint.transform.position;
        end     = zipLineProxy.endPoint.transform.position;

        numberOfVertices = zipLineProxy.numberOfVertices-1;
        maxDangle = zipLineProxy.maxGravityDangle;
        //forceDismountAtPercent = zipLineProxy.forceDismountAtPercent;
        forceDismountAtPercent = Mathf.Round(zipLineProxy.forceDismountAtPercent * 100.0f) / 100.0f;

        float distance = Vector3.Distance(start, end);
        float step = distance / numberOfVertices;

        // Start
        points.Add(start);

        // Rest (minus start & end)
        for (int i = 1; i <= numberOfVertices; i++)
        {
            Vector3 middlePoint = LerpByDistance(start, end, step * i);

            middlePoint.y -= Mathf.Sin(i * (step / (distance / Mathf.PI))) * maxDangle;

            points.Add(middlePoint);
        }

        CalculateDismountPercent();

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

        // Draw dismount at percent, if calculated...
        if (dismountPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(dismountPoint, new Vector3(0.25f, 0.25f, 0.25f));
        }
    }

    private void CalculateDismountPercent()
    {
        // If no points or forced dismount is below 0 or higher than 1
        if (points.Count == 0 || points.Count == 1 || forceDismountAtPercent < 0 || forceDismountAtPercent > 1)
        {
            return;
        }

        // We check how many vertices we have and transform the percentage into one of the points that we've got setup
        whereInZipLine = Mathf.Round((points.Count - 1) * forceDismountAtPercent * 100.0f) / 100.0f;
        whereInZipLineInt = (int)Mathf.Min(whereInZipLine);

        float restPercent = Mathf.Round(Mathf.Abs(whereInZipLineInt - whereInZipLine) * 100.0f) / 100.0f;

        // If it is the last one, just use the last position.
        if (points.Count-1 == whereInZipLineInt)
        {
            dismountPoint = points[points.Count-1];
        }
        else
        {
            dismountPoint = Vector3.Lerp(points[whereInZipLineInt], points[whereInZipLineInt + 1], restPercent);
        }
    }

    private Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
    {
        Vector3 P = x * Vector3.Normalize(B - A) + A;
        return P;
    }

    #endif
}