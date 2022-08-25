using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(FluidInteractor))]
public class FluidInteractorEditor : Editor
{
    private const string USE_BOUNDING_BOX = "Use Bounding Box Corners";
    private const string CORNER_FLOATER = "CornerFloater";

    [Range(0.1f, 2f)]
    public float boundsScaler = 1;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        FluidInteractor fluidInteractor = target as FluidInteractor;

        if (GUILayout.Button(USE_BOUNDING_BOX))
        {
            Collider collider = fluidInteractor.GetComponent<Collider>();
            Vector3[] corners = collider.GetBoundingBoxCorners();

            if (fluidInteractor.Floaters is not null)
            {
                foreach (Transform floater in fluidInteractor.Floaters)
                {
                    DestroyImmediate(floater.gameObject);
                }

                fluidInteractor.Floaters.Clear();
            }

            foreach (Vector3 corner in corners)
            {
                GameObject cornerFloater = new GameObject(CORNER_FLOATER);
                cornerFloater.transform.position = fluidInteractor.transform.position + corner;
                cornerFloater.transform.parent = fluidInteractor.transform;

                fluidInteractor.Floaters.Add(cornerFloater.transform);
            }
        }
    }
}
#endif