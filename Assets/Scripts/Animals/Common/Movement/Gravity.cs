using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    RaycastHit hit;

    void Update()
    {
        Ray downRay = new Ray(transform.position + new Vector3(0f, 100f, 0f), Vector3.down);

        int groundLayerMask = LayerMask.GetMask("Island");

        if (Physics.Raycast(downRay, out hit, Mathf.Infinity, groundLayerMask))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 100f - hit.distance, transform.position.z);
            }
        }
    }
}

