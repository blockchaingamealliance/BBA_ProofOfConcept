using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to set rotation of the displayed 3D Model (if there is one)
/// </summary>
public class ObjectsRotation : MonoBehaviour
{
    public float rotation = 0f;

    // Update is called once per frame
    void Update()
    {
        float rot_speed = 60f;

        rotation += rot_speed * Time.deltaTime;

        Quaternion euler = Quaternion.Euler(0, rotation, 0);

        GetComponent<Transform>().rotation = euler;

    }
}
