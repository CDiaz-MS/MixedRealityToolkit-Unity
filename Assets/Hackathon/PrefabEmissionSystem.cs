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
        Cone
    }

    //public bool attachTrailRenderer;
    //private TrailRenderer trailRenderer;

    public UnityEngine.GameObject prefab;

    public int emitCount = 1;

    public EmissionSpace emissionSpace;
    public EmissionShape emissionShape;
    public EmissionPattern emissionPattern;

    public bool useGravityOnPrefabs;

    [Range(0.0f, 90.0f)]
    [Tooltip("Only used when the Emission Shape is a Cone")]
    public float coneAngle;


    [Range(0.0f, 360.0f)]
    [Tooltip("Only used when the Emission Shape is an Arc")]
    public float arcAngle;

    public float radius;
    public float speed;

    private void Start()
    {
        // Ensure that the trail renderer attached to this object is deactivated

        //trailRenderer = GetComponent<TrailRenderer>();
        //trailRenderer.enabled = false;
    }

    // Emit in a hemi-sphere by default for now
    public UnityEngine.GameObject[] Emit(int count, Vector3? origin = null)
    {
        UnityEngine.GameObject[] emittedObjects = new UnityEngine.GameObject[count];

        if (!origin.HasValue)
        {
            origin = transform.position;
        }

        for (int i = 0; i < count; i++)
        {
            Vector3 direction = Vector3.one;
            UnityEngine.GameObject obj = Instantiate(prefab);

            emittedObjects[i] = obj;

            // Spawn position and velocity initialization
            if (emissionPattern == EmissionPattern.Random)
            {
                if (emissionShape == EmissionShape.Sphere)
                {
                    direction = Random.insideUnitSphere;
                    obj.transform.position = origin.Value + direction.normalized * radius;
                }
                else if (emissionShape == EmissionShape.Cone)
                {
                    Vector2 planarVelocity = Random.insideUnitCircle;
                    Vector2 displacement = planarVelocity * Mathf.Sin(coneAngle * Mathf.Deg2Rad);
                    float xVel = displacement.x;
                    float zVel = displacement.y;
                    float yVel = 1 - displacement.magnitude;
                    direction = new Vector3(xVel, yVel, zVel);

                    obj.transform.position = origin.Value;
                }
                else if (emissionShape == EmissionShape.Edge)
                {
                    direction = Vector3.up;
                    obj.transform.position = origin.Value + Vector3.right * Random.Range(-radius, radius);
                }
                else if (emissionShape == EmissionShape.Arc)
                {
                    float planarAngle = Random.Range(0, arcAngle) * Mathf.Deg2Rad;
                    Vector2 planarVelocity = Vector2.up * Mathf.Sin(planarAngle) + Vector2.right * Mathf.Cos(planarAngle);

                    float xVel = planarVelocity.x;
                    float zVel = planarVelocity.y;
                    direction = new Vector3(xVel, 0, zVel);

                    obj.transform.position = origin.Value;
                }
            }
            else if (emissionPattern == EmissionPattern.Uniform)
            {
                // No change in behavior for uniform emission on unit sphere
                if (emissionShape == EmissionShape.Sphere)
                {
                    direction = Random.insideUnitSphere;
                    obj.transform.position = origin.Value + direction.normalized * radius;
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

                    obj.transform.position = origin.Value;
                }
                // Edge uniform emission is as expected
                else if (emissionShape == EmissionShape.Edge)
                {
                    direction = Vector3.up;
                    obj.transform.position = origin.Value + Vector3.right * (2 * radius / count) * (count * 0.5f - i);
                }
                // Arc uniform emission is as expected
                else if (emissionShape == EmissionShape.Arc)
                {
                    float planarAngle = i * (arcAngle / count) * Mathf.Deg2Rad;
                    Vector2 planarVelocity = Vector2.up * Mathf.Sin(planarAngle) + Vector2.right * Mathf.Cos(planarAngle);

                    float xVel = planarVelocity.x;
                    float zVel = planarVelocity.y;
                    direction = new Vector3(xVel, 0, zVel);

                    obj.transform.position = origin.Value;
                }
            }

            // Orienting the velocity based on rotation
            Vector3 itemVelocity = direction.normalized * speed;
            if (emissionSpace == EmissionSpace.Local)
            {
                itemVelocity = transform.localToWorldMatrix * itemVelocity;
            }

            //copying over trail renderer values
            //if (attachTrailRenderer)
            //{
            //    TrailRenderer trail = obj.AddComponent<TrailRenderer>();
            //    trail.material = trailRenderer.material;
            //    trail.time = trailRenderer.time;
            //    trail.minVertexDistance = trailRenderer.minVertexDistance;
            //    trail.widthMultiplier = trailRenderer.widthMultiplier;
            //    trail.colorGradient = trailRenderer.colorGradient;
            //    trail.numCornerVertices = trailRenderer.numCornerVertices;
            //}

            Rigidbody animatedBody = obj.GetComponent<Rigidbody>();
            animatedBody.velocity = itemVelocity;
            animatedBody.useGravity = useGravityOnPrefabs;
        }

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

                UnityEditor.Handles.DrawWireDisc(transform.position + Vector3.up * yVel * speed, transform.up, xVel * speed);
                Gizmos.DrawLine(Vector3.zero, direction * speed);
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