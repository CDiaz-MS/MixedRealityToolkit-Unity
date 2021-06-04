using UnityEngine;

public class PrefabEmissionSystem : MonoBehaviour
{
    public enum EmissionSpace
    {
        Local,
        World
    }

    public enum EmissionPattern
    {
        Uniform,
        Random
    }

    public enum EmissionShape
    {
        Edge,
        Arc,
        Sphere,
        Cone,
        UseExisting
    }

    public RadialMenuElement prefab;

    public int emitCount = 1;

    public EmissionSpace emissionSpace;
    public EmissionShape emissionShape;
    public EmissionPattern emissionPattern;

    [Range(0.0f, 90.0f)]
    [Tooltip("Only used when the Emission Shape is a Cone")]
    public float coneAngle;


    [Range(0.0f, 360.0f)]
    [Tooltip("Only used when the Emission Shape is an Arc")]
    public float arcAngle;

    public float radius;
    public float emissionDistance;

    public void EmitExisting(GameObject[] menuElements, Vector3? origin, Transform rootTransform)
    {
        int count = menuElements.Length;
        for (int i = 0; i < menuElements.Length; i++)
        {
            var obj = menuElements[i];

            if (emissionShape == EmissionShape.UseExisting)
            {
                obj.GetComponent<RadialMenuElement>().menuElementOrigin = rootTransform;
                continue;
            }

            Vector3 direction = Vector3.one;
            Vector3 objectPosition = origin.Value;

            // Spawn position and velocity initialization
            if (emissionPattern == EmissionPattern.Random)
            {
                if (emissionShape == EmissionShape.Sphere)
                {
                    direction = Random.insideUnitSphere;
                    objectPosition = origin.Value + direction.normalized * radius;
                }
                else if (emissionShape == EmissionShape.Cone)
                {
                    Vector2 planarVelocity = Random.insideUnitCircle;
                    Vector2 displacement = planarVelocity * Mathf.Sin(coneAngle * Mathf.Deg2Rad);
                    float xVel = displacement.x;
                    float zVel = displacement.y;
                    float yVel = 1 - displacement.magnitude;
                    direction = new Vector3(xVel, yVel, zVel);

                    objectPosition = origin.Value;
                }
                else if (emissionShape == EmissionShape.Edge)
                {
                    direction = Vector3.up;
                    objectPosition = origin.Value + Vector3.right * Random.Range(-radius, radius);
                }
                else if (emissionShape == EmissionShape.Arc)
                {
                    float planarAngle = Random.Range(0, arcAngle) * Mathf.Deg2Rad;
                    Vector2 planarVelocity = Vector2.up * Mathf.Sin(planarAngle) + Vector2.right * Mathf.Cos(planarAngle);

                    float xVel = planarVelocity.x;
                    float zVel = planarVelocity.y;
                    direction = new Vector3(xVel, 0, zVel);

                    objectPosition = origin.Value;
                }
            }
            else if (emissionPattern == EmissionPattern.Uniform)
            {
                // No change in behavior for uniform emission on unit sphere
                if (emissionShape == EmissionShape.Sphere)
                {
                    direction = Random.insideUnitSphere;
                    objectPosition = origin.Value + direction.normalized * radius;
                }
                // Cone uniform emission is as expected
                else if (emissionShape == EmissionShape.Cone)
                {
                    float planarAngle = i * (360.0f / count) * Mathf.Deg2Rad;
                    Vector2 planarVelocity = Vector2.up * Mathf.Sin(planarAngle) + Vector2.right * Mathf.Cos(planarAngle);

                    Vector2 displacement = planarVelocity.normalized * Mathf.Sin(coneAngle * Mathf.Deg2Rad);
                    float xVel = displacement.x;
                    float zVel = displacement.y;
                    float yVel = 1 - displacement.magnitude;
                    direction = new Vector3(xVel, yVel, zVel);

                    objectPosition = origin.Value;
                }
                // Edge uniform emission is as expected
                else if (emissionShape == EmissionShape.Edge)
                {
                    direction = Vector3.up;
                    objectPosition = origin.Value + Vector3.right * (2 * radius / count) * (count * 0.5f - i);
                }
                // Arc uniform emission is as expected
                else if (emissionShape == EmissionShape.Arc)
                {
                    float planarAngle = i * (arcAngle / count) * Mathf.Deg2Rad;
                    Vector2 planarVelocity = Vector2.up * Mathf.Sin(planarAngle) + Vector2.right * Mathf.Cos(planarAngle);

                    float xVel = planarVelocity.x;
                    float zVel = planarVelocity.y;
                    direction = new Vector3(xVel, 0, zVel);

                    objectPosition = origin.Value;
                }
            }

            // Orienting the velocity based on rotation
            Vector3 itemVelocity = direction.normalized * emissionDistance;
            if (emissionSpace == EmissionSpace.Local)
            {
                itemVelocity = rootTransform.localToWorldMatrix * itemVelocity;
            }

            obj.transform.position = objectPosition;

            obj.GetComponent<RadialMenuElement>().menuElementOrigin = this.transform;
            obj.GetComponent<RadialMenuElement>().targetLocation = objectPosition + itemVelocity;
        }
    }

    // Emit in a hemi-sphere by default for now
    public GameObject[] EmitFromEmissionSystemSource(int count, Vector3? origin = null)
    {
        GameObject[] emittedObjects = new GameObject[count];

        if (!origin.HasValue)
        {
            origin = transform.position;
        }

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab.gameObject);
            emittedObjects[i] = obj;
        }

        EmitExisting(emittedObjects, origin, this.transform);

        return emittedObjects;
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.green;
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.green;
        Gizmos.matrix = this.transform.localToWorldMatrix;

        if (emissionShape == EmissionShape.Sphere)
        {
            Gizmos.DrawSphere(Vector3.zero, radius);
        }
        else if (emissionShape == EmissionShape.Cone)
        {
            //Gizmos.DrawCube(Vector3.up, new Vector3(1, 3, 1));

            for (int i = 0; i < 4; i++)
            {
                float planarAngle = i * 90.0f * Mathf.Deg2Rad;
                Vector2 planarVelocity = Vector2.up * Mathf.Sin(planarAngle) + Vector2.right * Mathf.Cos(planarAngle);

                Vector2 displacement = planarVelocity.normalized * Mathf.Sin(coneAngle * Mathf.Deg2Rad);
                float xVel = displacement.x;
                float zVel = displacement.y;
                float yVel = 1 - displacement.magnitude;
                Vector3 direction = new Vector3(xVel, yVel, zVel);

                UnityEditor.Handles.DrawWireDisc(transform.position + Vector3.up * yVel * emissionDistance, transform.up, xVel * emissionDistance);
                Gizmos.DrawLine(Vector3.zero, direction * emissionDistance);
            }
        }
        else if (emissionShape == EmissionShape.Arc)
        {
            UnityEditor.Handles.DrawWireArc(transform.position, transform.up, transform.right, -arcAngle, radius);
        }
        else if (emissionShape == EmissionShape.Edge)
        {
            Gizmos.DrawLine(Vector3.left * radius, Vector3.right * radius);
        }
#endif
    }
}