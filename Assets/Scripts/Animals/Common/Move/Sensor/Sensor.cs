using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    /// The radius of the field of view
    public int radius;
    /// The angle of the field of view (limited between 0 and 360 degrees)
    [Range(0, 360)]
    public int angle = 160;
    /// Reference to the target GameObject: food, water, mate...
    /// The layer mask for filtering targets
    public LayerMask targetMask;
    /// The layer mask for filtering obstructions
    public LayerMask obstructionMask;
    /// Boolean indicating whether the player is within the field of view
    public bool canSeeTarget;
    public Animal animal;


    private void Start()
    {
        animal = GetComponent<Animal>();
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        /// We only calculate just 5 times per second
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    public void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        // Initialize variables to keep track of the nearest target and its distance
        GameObject nearestTarget = null;
        float nearestDistance = Mathf.Infinity;

        foreach (var targetCollider in rangeChecks)
        {
            Transform target = targetCollider.transform;
            if (animal.noTargetRefs.Contains(target.gameObject))
            {
                continue;
            }

            Vector3 directionToTarget = (target.position - transform.position).normalized;

            // Check if the target is within the angle of the field of view
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                // Check if the target is the nearest and is not obstructed
                if (distanceToTarget < nearestDistance &&
                    !Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    nearestTarget = target.gameObject;
                    nearestDistance = distanceToTarget;
                }
            }
        }

        canSeeTarget = nearestTarget != null;
        animal.targetRef = canSeeTarget ? nearestTarget : null;
    }

    public bool FieldOfViewCheck(LayerMask _targetMask, GameObject _targetRef)
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, _targetMask);

        foreach (var targetCollider in rangeChecks)
        {
            Transform target = targetCollider.transform;

            if (target.gameObject != _targetRef)
            {
                continue;
            }

            // Check if the target is in the field of view
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                // Check if there is no obstruction between the AI and the target
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    // The specified target is within the field of view and range
                    return true;
                }
            }
        }

        // The specified target is not within the field of view or range
        return false;
    }
}
