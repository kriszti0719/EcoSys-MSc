using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    [HideInInspector] public Animal animal;
    [Range(0, 360)]
    public int angle = 160;
    public int radius;
    public int camouflage;
    public int stealth;
    
    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeeTarget;
    public bool danger = false;

    public event System.Action OnTargetSpotted;

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

            Animal targetAnimal = target.GetComponent<Animal>();

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

        if (canSeeTarget && animal.targetRef == null)
        {
            animal.targetRef = nearestTarget;
            OnTargetSpotted?.Invoke();
        }
        else
        {
            animal.targetRef = nearestTarget;
        }
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

                if (roll < detectionChance)
                {
                    if (!animal.spottedThreats.Contains(predator.gameObject))
                    {
                        animal.spottedThreats.Add(predator.gameObject);
                    }
                }
            }
        }

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

        foreach (var threat in toRemove)
        {
            animal.spottedThreats.Remove(threat);
        }

        danger = animal.spottedThreats.Any();
    }
}