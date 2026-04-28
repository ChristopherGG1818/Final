using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset;

    [Header("Zoom")]
    public float normalZoom = 5f;
    public float bossZoom = 7f;
    public float zoomSpeed = 2f;

    public bool bossFight = false;

    void LateUpdate()
    {
        if (target == null) return;

        // Follow player
        Vector3 desiredPosition = target.position + offset;

        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.position = new Vector3(
            smoothedPosition.x,
            smoothedPosition.y,
            transform.position.z
        );

        // Zoom logic
        float targetZoom = bossFight ? bossZoom : normalZoom;

        Camera.main.orthographicSize = Mathf.Lerp(
            Camera.main.orthographicSize,
            targetZoom,
            zoomSpeed * Time.deltaTime
        );
    }
}