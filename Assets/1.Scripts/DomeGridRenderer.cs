using System;
using System.Collections.Generic;
using UnityEngine;

public class SphereVector3
{
    private float rho;
    private float phi;
    private float theta;

    public SphereVector3()
    {
        rho = 0f;
        phi = 0f;
        theta = 0f;
    }

    public SphereVector3(float incomingRho, float incomingPhi, float incomingTheta)
    {
        rho = incomingRho;
        phi = incomingPhi;
        theta = incomingTheta;
    }

    public SphereVector3 GetSphereCoords()
    {
        return this;
    }

    public float GetRho()
    {
        return rho;
    }

    public float GetPhi()
    {
        return phi;
    }

    public float GetTheta()
    {
        return theta;
    }

    public void SetSphereCoords(float incomingRho, float incomingPhi, float incomingTheta)
    {
        rho = incomingRho;
        phi = incomingPhi;
        theta = incomingTheta;
    }

    public void SetRho(float incomingRho)
    {
        rho = incomingRho;
    }

    public void SetPhi(float incomingPhi)
    {
        phi = incomingPhi;
    }

    public void SetTheta(float incomingTheta)
    {
        theta = incomingTheta;
    }
}

public class DomeGridRenderer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int meridians;
    [SerializeField] private int parallels;
    [Tooltip("Line Resolution")]
    [Range(4, 64)]
    [SerializeField] private int lineSegments = 32;
    [Tooltip("Line Width")]
    [Range(0.01f, 2f)]
    [SerializeField] private float lineWidth = 0.03f;
    [SerializeField] private Color lineColor = Color.magenta;

    // --- SECCIÓN NUEVA: Visibilidad ---
    [Header("Visibility & UI Control")]
    [SerializeField] private GameObject visualDomeModel; // Asigna aquí el mesh del domo para ocultarlo
    [SerializeField] private bool showGrid = true;
    // ---------------------------------

    [Header("Object References")]
    [SerializeField] private GameObject domeCenterPoint;
    [SerializeField] private GameObject domeRadiusPoint;

    public event System.Action OnGridUpdated;

    private Vector3 centerPos;
    private Vector3 radiusPos;
    private List<LineRenderer> gridLines = new();
    public List<Vector3> SnapPoints { get; private set; } = new();

    private int prevMeridians;
    private int prevParallels;
    private int prevSegments;
    private float prevWidth;
    private Color prevColor;
    private Vector3 prevCenterPos;
    private Vector3 prevRadiusPos;

    // --- SECCIÓN NUEVA: Métodos Públicos para UI (Sliders y Toggles) ---

    // Conectar al Slider (Dynamic float). Rango recomendado: 2 a 32
    public void SetMeridians(float value)
    {
        meridians = Mathf.RoundToInt(value);
        // Tu Update() detectará el cambio automáticamente
    }

    // Conectar al Slider (Dynamic float). Rango recomendado: 2 a 32
    public void SetParallels(float value)
    {
        parallels = Mathf.RoundToInt(value);
    }

    // Conectar al Slider (Dynamic float). Rango recomendado: 4 a 64
    public void SetLineSegments(float value)
    {
        lineSegments = Mathf.RoundToInt(value);
    }

    // Conectar al Slider (Dynamic float). Rango recomendado: 0.01 a 0.2
    public void SetLineWidth(float value)
    {
        lineWidth = value;
    }

    // Conectar al Slider (Dynamic float). Rango recomendado: 1 a 10 (metros)
    public void SetDomeRadius(float value)
    {
        if (domeRadiusPoint != null)
        {
            // Movemos el punto de control de radio en el eje Z local
            domeRadiusPoint.transform.localPosition = new Vector3(0, 0, value);
        }
    }

    // Conectar al Toggle (Dynamic bool)
    public void ToggleGridVisibility(bool state)
    {
        showGrid = state;
        foreach (var line in gridLines)
        {
            if (line != null) line.enabled = showGrid;
        }
    }

    // Conectar al Toggle (Dynamic bool)
    public void ToggleDomeMeshVisibility(bool state)
    {
        if (visualDomeModel != null)
        {
            visualDomeModel.SetActive(state);
        }
    }
    // ------------------------------------------------------------------

    public Vector3 GetCenterPos()
    {
        return centerPos;
    }

    public int GetParallels()
    {
        return parallels;
    }

    public int GetMeridians()
    {
        return meridians;
    }

    public float GetRadius()
    {
        return CalcRadius();
    }

    private bool SettingsChanged()
    {
        centerPos = domeCenterPoint.transform.position;
        radiusPos = domeRadiusPoint.transform.position;

        bool changed =
            prevMeridians != meridians ||
            prevParallels != parallels ||
            prevSegments != lineSegments ||
            prevWidth != lineWidth ||
            prevColor != lineColor ||
            prevCenterPos != centerPos ||
            prevRadiusPos != radiusPos;

        if (changed)
        {
            prevMeridians = meridians;
            prevParallels = parallels;
            prevSegments = lineSegments;
            prevWidth = lineWidth;
            prevColor = lineColor;
            prevCenterPos = centerPos;
            prevRadiusPos = radiusPos;
        }

        return changed;
    }

    private float CalcRadius()
    {
        return Vector3.Distance(centerPos, radiusPos);
    }

    private Vector3 ConvertSpherical(float radius, float theta, float phi)
    {
        return new Vector3(radius * Mathf.Sin(phi) * Mathf.Cos(theta), radius * Mathf.Cos(phi), radius * MathF.Sin(phi) * Mathf.Sin(theta)) + centerPos;
    }

    private SphereVector3 ConvertCartesian(Vector3 incomingCoordinate)
    {
        float newRho = incomingCoordinate.magnitude;
        float newPhi = Mathf.Acos(incomingCoordinate.y / newRho);
        float newTheta = MathF.Atan2(incomingCoordinate.z, incomingCoordinate.x);

        return new SphereVector3(newRho, newPhi, newTheta);
    }

    private void ClearGrid()
    {
        foreach (var line in gridLines) if (line) Destroy(line.gameObject);
        gridLines.Clear();
    }

    private LineRenderer SpawnLine(string lineName)
    {
        GameObject lineObject = new GameObject(lineName);
        lineObject.transform.SetParent(transform);

        LineRenderer renderer = lineObject.AddComponent<LineRenderer>();

        renderer.useWorldSpace = true;
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.startWidth = renderer.endWidth = lineWidth;
        renderer.positionCount = lineSegments;
        renderer.startColor = renderer.endColor = lineColor;

        // MODIFICACIÓN: Respetar visibilidad inicial al crear la línea
        renderer.enabled = showGrid;

        return renderer;
    }

    private void GridGenerator()
    {
        ClearGrid();
        SnapPoints.Clear();

        centerPos = domeCenterPoint.transform.position;
        radiusPos = domeRadiusPoint.transform.position;
        float radius = CalcRadius();
        Debug.Log("Radius: " + radius);

        for (int parallelCounter = 1; parallelCounter <= parallels; parallelCounter++)
        {
            float phi = (parallelCounter / (float)(parallels + 1)) * (Mathf.PI / 2f);

            for (int meridianCounter = 1; meridianCounter <= meridians; meridianCounter++)
            {
                float theta = (meridianCounter / (float)meridians) * Mathf.PI * 2f;
                Vector3 snapPoint = ConvertSpherical(radius, theta, phi);
                SnapPoints.Add(snapPoint);
            }
        }

        for (int counter = 0; counter < meridians; counter++)
        {
            float theta = (counter / (float)meridians) * MathF.PI * 2f;
            LineRenderer line = SpawnLine("Meridian_" + counter);

            for (int resCounter = 0; resCounter < lineSegments; resCounter++)
            {
                float phi = (resCounter / (float)(lineSegments - 1)) * (Mathf.PI / 2f);
                Vector3 point = ConvertSpherical(radius, theta, phi);
                line.SetPosition(resCounter, point);
            }

            gridLines.Add(line);
        }

        for (int counter = 1; counter <= parallels; counter++)
        {
            float phi = (counter / (float)(parallels + 1)) * (Mathf.PI / 2F);
            LineRenderer line = SpawnLine("Parallel_" + counter);

            for (int resCounter = 0; resCounter < lineSegments; resCounter++)
            {
                float theta = (resCounter / (float)(lineSegments - 1)) * MathF.PI * 2f;
                Vector3 point = ConvertSpherical(radius, theta, phi);
                line.SetPosition(resCounter, point);
            }

            gridLines.Add(line);
        }

        OnGridUpdated?.Invoke();
    }

    private void Start()
    {
        if (domeCenterPoint == null)
        {
            domeCenterPoint = new GameObject("Center");
            domeCenterPoint.transform.position = transform.position;
            domeCenterPoint.transform.parent = transform;
        }

        if (domeRadiusPoint == null)
        {
            domeRadiusPoint = new GameObject("Radius Handler");
            domeRadiusPoint.transform.position = transform.position + new Vector3(0, 0, 3);
            domeRadiusPoint.transform.parent = transform;
        }

        GridGenerator();
    }

    private void Update()
    {
        if (SettingsChanged())
        {
            Debug.Log("Called SettingsChanged() @ DomeGridRenderer");
            GridGenerator();
        }
    }
}