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

    [Header("Screen Shake")]
    public float shakeIntensity = 0f;
    public float shakeDecay = 2f;

    void LateUpdate()
    {
        if (target == null) return;

        // Follow target
        Vector3 desiredPosition = target.position + offset;

        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        Vector3 finalPosition = new Vector3(
            smoothedPosition.x,
            smoothedPosition.y,
            transform.position.z
        );

        // SCREEN SHAKE
        Vector2 shakeOffset = Random.insideUnitCircle * shakeIntensity;
        finalPosition += new Vector3(shakeOffset.x, shakeOffset.y, 0);

        transform.position = finalPosition;

        // Zoom logic
        float targetZoom = bossFight ? bossZoom : normalZoom;

        Camera.main.orthographicSize = Mathf.Lerp(
            Camera.main.orthographicSize,
            targetZoom,
            zoomSpeed * Time.deltaTime
        );

        // decay shake over time
        shakeIntensity = Mathf.Lerp(shakeIntensity, 0f, shakeDecay * Time.deltaTime);
    }

    // Called externally (boss script)
    public void Shake(float intensity)
    {
        shakeIntensity = intensity;
    }
}