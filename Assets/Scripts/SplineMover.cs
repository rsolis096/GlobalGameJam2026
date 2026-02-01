using UnityEngine;
using UnityEngine.Splines;

public class SplineMover : MonoBehaviour
{
    public SplineContainer spline;

    [Range(0f, 1f)] public float t = 0f;
    public float speedT = 0.15f;

    public bool useStartY = true;
    public float lockedY = 0f;

    public bool followTangent = true;

    void Start()
    {
        if (useStartY) lockedY = transform.position.y;
    }

    void Update()
    {
        if (!spline) return;

        t = Mathf.Repeat(t + speedT * Time.deltaTime, 1f);

        // IMPORTANT: use container eval directly (likely already world space)
        Vector3 worldPos = spline.EvaluatePosition(t);
        Vector3 worldTan = spline.EvaluateTangent(t);

        worldPos.y = lockedY;
        transform.position = worldPos;

        if (followTangent)
        {
            worldTan.y = 0f;
            if (worldTan.sqrMagnitude > 1e-6f)
                transform.rotation = Quaternion.LookRotation(worldTan.normalized, Vector3.up);
        }
    }
}
