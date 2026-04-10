using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    [Range(0, 360)]
    public int angle = 160;
    public int radius;
    public int camouflage;
    public int stealth;
    // The layer masks for filtering targets and obstructions:
    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeeTarget;
    public Animal animal;
    public int secCntr = 0;
    public bool danger = false;

    private void Start()
    {
        animal = GetComponent<Animal>();
        StartCoroutine(FOVRoutine());
    }
    public void setSensor(int _radius, int _camouflage, int _stealth, int _angle = 160)
    {
        angle = _angle;
        radius = _radius;
        stealth = _stealth;
        camouflage = _camouflage;
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
        GameObject nearestTarget = null;
        float nearestDistance = Mathf.Infinity;

        foreach (var targetCollider in rangeChecks)
        {
            Transform target = targetCollider.transform;
            if (animal.rejectedBy.Contains(target.gameObject))
                continue;

            //If already has been caught
            Animal targetAnimal = target.GetComponent<Animal>();
            if (targetAnimal != null && targetAnimal.status == Status.CAUGHT)
                continue;

            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                // Check if the target is the nearest and is not obstructed
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    float camouflage = targetAnimal != null ? targetAnimal.sensor.camouflage : 0f;
                    float detectionChance = 1f - Mathf.Clamp01(camouflage / 100f);

                    if (Random.value > detectionChance)
                        continue;

                    if (distanceToTarget < nearestDistance)
                    {
                        nearestTarget = target.gameObject;
                        nearestDistance = distanceToTarget;
                    }
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

            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    return true;
                }
            }
        }
        return false;
    }
    public void CheckForPredators()
    {
        Collider[] predatorsInRange = Physics.OverlapSphere(transform.position, radius, animal.getPredatorLayers());
        List<GameObject> toRemove = new List<GameObject>(); // TODO: ha az angle ismét funkciót kap, akkor X ideig még jó lenne menekülni, mert amint hátat fordít megállna a nyúl

        foreach (var predatorCollider in predatorsInRange)
        {
            Transform predator = predatorCollider.transform;
            Vector3 directionToPredator = (predator.position - transform.position).normalized;
            float distanceToPredator = Vector3.Distance(transform.position, predator.position);

            if (!Physics.Raycast(transform.position, directionToPredator, distanceToPredator, obstructionMask))
            {
                Animal predatorAnimal = predator.GetComponent<Animal>();
                int camouflage = predatorAnimal != null ? predatorAnimal.sensor.camouflage : 0;
                int stealth = predatorAnimal != null ? predatorAnimal.sensor.stealth : 0;

                float distanceFactor = Mathf.Clamp01(1f - (distanceToPredator / radius)); // closer -> easier
                float detectionChance = Mathf.Clamp01(distanceFactor * (1f - (camouflage + stealth) / 200f)); // combine (distance + camouflage + stealth)
                float roll = Random.value;

                // --- Perception roll ---
                if (roll < detectionChance)
                {
                    if (!animal.spottedThreats.Contains(predator.gameObject))
                    {
                        animal.spottedThreats.Add(predator.gameObject);
                    }
                }
            }
        }

        // Remove predators that are no longer visible
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

        // Remove no longer visible threats
        foreach (var threat in toRemove)
        {
            animal.spottedThreats.Remove(threat);
        }
    }
}