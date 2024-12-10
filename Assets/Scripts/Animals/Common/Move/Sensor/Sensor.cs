using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public int secCntr = 0;
    public bool danger = false;

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
            if (animal.rejectedBy.Contains(target.gameObject))
            {
                continue;
            }

            // Check if the target is a valid animal and get its status
            Animal targetAnimal = target.GetComponent<Animal>();
            if (targetAnimal != null && targetAnimal.status == Status.CAUGHT)
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

            if (target.gameObject != _targetRef) { continue; }

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
    public void CheckForPredators()
    {
        secCntr++;

        if (secCntr < 40)
            return;
        secCntr = 0;

        // Ellenõrizzük a látómezõn belüli összes objektumot a ragadozó rétegen
        Collider[] predatorsInRange = Physics.OverlapSphere(transform.position, radius, animal.getPredatorLayers());

        // Azokat a ragadozókat, akiket már nem látunk, eltávolítjuk
        List<GameObject> toRemove = new List<GameObject>();

        foreach (var predatorCollider in predatorsInRange)
        {
            Transform predator = predatorCollider.transform;
            Vector3 directionToPredator = (predator.position - transform.position).normalized;
            float distanceToPredator = Vector3.Distance(transform.position, predator.position);

            if (!Physics.Raycast(transform.position, directionToPredator, distanceToPredator, obstructionMask))
            {
                // Ha a ragadozó még nincs a spottedThreats listában, hozzáadjuk
                if (!animal.spottedThreats.Contains(predator.gameObject))
                {
                    animal.spottedThreats.Add(predator.gameObject);
                }
            }
        }

        // Azok a ragadozók, akiket már nem látunk, eltávolítjuk a spottedThreats listából
        foreach (var threat in animal.spottedThreats)
        {
            bool isStillVisible = predatorsInRange.Any(predatorCollider =>
                predatorCollider.gameObject == threat &&
                !Physics.Raycast(transform.position, (predatorCollider.transform.position - transform.position).normalized,
                    Vector3.Distance(transform.position, predatorCollider.transform.position), obstructionMask)
            );

            if (!isStillVisible)
            {
                toRemove.Add(threat);
            }
        }

        // Az eltávolítandó ragadozók törlése a listából
        foreach (var threat in toRemove)
        {
            animal.spottedThreats.Remove(threat);
        }
    }
}
