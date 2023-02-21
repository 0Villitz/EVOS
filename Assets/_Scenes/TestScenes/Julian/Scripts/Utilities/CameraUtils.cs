
using UnityEngine;

public static class CameraUtils
{
    public static Vector3 GetMouseWorldPoint(Camera camera, Vector2 mousePosition, float canvasScaleFactor)
    {
        Vector2 rawMousePointPosition = mousePosition;
        Vector3 screenMousePos        = rawMousePointPosition / canvasScaleFactor;
        Vector3 cameraPosition        = camera.transform.position;

        // Set the z value so when we use 'ScreenToWorldPoint',
        // that position is projected out into 3d space away from the camera
        screenMousePos.z = Mathf.Abs(cameraPosition.z);

        Vector2 worldMousePos = (Vector2)camera.ScreenToWorldPoint(screenMousePos);
        return worldMousePos;
    }
}

