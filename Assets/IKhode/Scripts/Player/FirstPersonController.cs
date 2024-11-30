using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    public float sensitivity = 100f;

    public float xRotationMax = 90f;
    public float xRotationMin = -90f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // Rotate the camera around the y-axis (left and right)
        yRotation += mouseX;

        // Rotate the camera around the x-axis (up and down), with clamping
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, xRotationMin, xRotationMax);

        _camera.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}