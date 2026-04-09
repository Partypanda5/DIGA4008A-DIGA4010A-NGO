using UnityEngine;

public class BillboardToCamera : MonoBehaviour
{
    private Camera cam;
    private void LateUpdate()
    {
        if (!cam) cam = Camera.main;
        if (!cam) return;

        // Face the camera
        transform.forward = cam.transform.forward;
    }
}