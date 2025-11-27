using UnityEngine;

public class DomeModelConformer : MonoBehaviour
{
   
    [Header("Grid")]
    [SerializeField] private DomeGridRenderer domeGrid;
    [SerializeField] private float scale = 1f;

    private void HandleGridUpdated()
    {
        
        float radius = domeGrid.GetRadius();
        float scaledRadius = radius * scale;

        transform.position = domeGrid.GetCenterPos();
        transform.localScale = Vector3.one * ((scaledRadius * 2f) + 0.5f);

    }

    private void Start()
    {
        
        if (domeGrid == null)
        {
            
            domeGrid = FindAnyObjectByType<DomeGridRenderer>();

        }

        domeGrid.OnGridUpdated += HandleGridUpdated;

    }

    private void OnDestroy()
    {
        
        if(domeGrid != null)
        {
            
            domeGrid.OnGridUpdated -= HandleGridUpdated;

        }

    }

}
