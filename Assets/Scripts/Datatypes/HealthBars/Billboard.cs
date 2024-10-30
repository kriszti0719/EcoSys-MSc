using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    // Ref to our camera
    public Transform cam;
    void LateUpdate()
    {
        // LateUpdate: regular update won't work - if our cam moves inside of the update func --> jitter
        // Always called after the regular update func, cam does all it's improvements, and THEN we point towards it
        transform.LookAt(transform.position + cam.forward);
    }
}
