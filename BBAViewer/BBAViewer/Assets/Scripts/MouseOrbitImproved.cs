using UnityEngine;
using System.Collections;


/// <summary>
/// This class enables mouse wheel control of the model view zoom level.
/// Usage: Just add the script to the ModelCamera.
/// </summary>

public class MouseOrbitImproved : MonoBehaviour
{
    public Transform target;
    public float distance = 5.0f;
    public float xSpeed = 60.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;

    private Rigidbody rigidbody;

    float x = 0.0f;
    float y = 0.0f;

    public bool pressed = false;

    float minFov = 15f;
    float maxFov = 90f;
    float sensitivity = -30f;


    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        rigidbody = GetComponent<Rigidbody>();

        // Make the rigid body not change rotation
        if (rigidbody != null)
        {
            rigidbody.freezeRotation = true;
        }
    }

    private void Update()
    {
        if (modelTexture.CanScroll())
        {
            float fov = GetComponent<Camera>().fieldOfView;
            fov += Input.GetAxis("Mouse ScrollWheel") * sensitivity;
            fov = Mathf.Clamp(fov, minFov, maxFov);
            GetComponent<Camera>().fieldOfView = fov;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}