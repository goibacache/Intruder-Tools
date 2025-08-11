using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DrawZipLineInEditor : MonoBehaviour
{
#if UNITY_EDITOR

    /// <summary>
    /// Public variable
    /// </summary>
    [SerializeField]
    public bool DisplayColliders;

    public GameObject prefabToSpawnAtZiplineIntersections;

    [HideInInspector]
    public List<Vector3> points;
    ZiplineProxy zipLineProxy;
    List<Color> colors;
    private Vector3 start;
    private Vector3 end;
    private float numberOfVertices;
    private float maxDangle;

    // Dismount calc:
    private float forceDismountAtPercent;
    private float whereInZipLine;
    private int whereInZipLineInt;
    private Vector3 dismountPoint;

    private void Start()
    {
        DisplayColliders = IntruderToolsConfig.ShowColliders;
    }

    private void Awake()
    {
        DisplayColliders = false;
    }

    // OnValidate is called in the editor when the script is loaded or a value is changed in the Inspector.
    private void OnValidate()
    {
        IntruderToolsConfig.ShowColliders = DisplayColliders;
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
        // Check it's synced to the main one
        DisplayColliders = IntruderToolsConfig.ShowColliders;

        if (colors == null || colors.Count == 0)
        {
            loadColors();
        }

        if (zipLineProxy == null)
        {
            zipLineProxy = GetComponent<ZiplineProxy>();
        }

        if (zipLineProxy == null)
        {
            return;
        }

        // If it haven't been touched, don't calculate, just draw.
        if (start == zipLineProxy.startPoint.transform.position && end == zipLineProxy.endPoint.transform.position && numberOfVertices == zipLineProxy.numberOfVertices && maxDangle == zipLineProxy.maxGravityDangle)
        {
            DrawPoints();
            return;
        }

        // Clean the list when re-procesing
        points = new List<Vector3>();

        start = zipLineProxy.startPoint.transform.position;
        end = zipLineProxy.endPoint.transform.position;

        numberOfVertices = zipLineProxy.numberOfVertices - 1;
        maxDangle = zipLineProxy.maxGravityDangle;
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

        // Draw colliders if on
        if (IntruderToolsConfig.ShowColliders)
        {
            // Draw start.
            Gizmos.DrawSphere(points[0], 0.75f);

            // Draw points
            for (int i = 1; i < points.Count; i++)
            {
                float length = Vector3.Distance(points[i-1], points[i]);
                Gizmos.color = colors[i % 4];
                Gizmos.DrawLine(points[i - 1], points[i]);

                Vector3 direction = points[i - 1] - points[i];
                Quaternion rotation = Quaternion.LookRotation(direction);
                rotation *= Quaternion.Euler(-90, 0, 0); // this adds a 90 degrees Y rotation
                
                DrawWireCapsule(points[i - 1], rotation, 0.75f/2f, length, colors[i % 4]);
            }

            return; // Why draw the rest?
        }

        // Draw points
        for (int i = 1; i < points.Count; i++)
        {
            Gizmos.color = colors[i % 4];
            Gizmos.DrawLine(points[i - 1], points[i]);
            Gizmos.DrawWireSphere(points[i - 1], 0.1f);
        }

        // Draw Start
        Gizmos.DrawWireSphere(points[points.Count - 1], 0.1f);

        // Draw dismount at percent, if calculated...
        if (dismountPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(dismountPoint, new Vector3(0.25f, 0.25f, 0.25f));
        }
    }

    public static void DrawWireCapsule(Vector3 _pos, Quaternion _rot, float _radius, float _height, Color _color = default(Color))
    {
        if (_color != default(Color))
            Handles.color = _color;

        // Calculate the new position to offset the pivot.
        Vector3 baseOffset = _rot * new Vector3(0, _height/2, 0);

        Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos + baseOffset, _rot, Handles.matrix.lossyScale);
        //Gizmos.matrix = Matrix4x4.TRS(points[i-1] + baseOffset, rotation, gizmoScale);


        using (new Handles.DrawingScope(angleMatrix))
        {
            var pointOffset = (_height - (_radius * 2)) / 2;

            //draw sideways
            Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _radius);
            Handles.DrawLine(new Vector3(0, pointOffset, -_radius), new Vector3(0, -pointOffset, -_radius));
            Handles.DrawLine(new Vector3(0, pointOffset, _radius), new Vector3(0, -pointOffset, _radius));
            Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _radius);
            //draw frontways
            Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _radius);
            Handles.DrawLine(new Vector3(-_radius, pointOffset, 0), new Vector3(-_radius, -pointOffset, 0));
            Handles.DrawLine(new Vector3(_radius, pointOffset, 0), new Vector3(_radius, -pointOffset, 0));
            Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _radius);
            //draw center
            Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, _radius);
            Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, _radius);

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
        if (points.Count - 1 == whereInZipLineInt)
        {
            dismountPoint = points[points.Count - 1];
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