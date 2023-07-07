using UnityEngine;

public class SeagulController : MonoBehaviour
{
    public float circleRadius = 5f;
    public float speed = 1f;
    public bool clockwise;
    public float heightAboveTerrain = 2f;
    public float rotationSmoothness = 2f;
    public LayerMask terrainLayer;
    private float angle;
    private Vector3 centerPosition;
    private Vector3 previousPosition;

    void Start()
    {
        centerPosition = transform.position;
        previousPosition = transform.position;
        clockwise = Random.value > 0.5f;
    }

    void Update()
    {
        angle += (clockwise ? 1 : -1) * speed * Time.deltaTime;
        float newX = centerPosition.x + circleRadius * Mathf.Cos(angle);
        float newZ = centerPosition.z + circleRadius * Mathf.Sin(angle);

        RaycastHit hit;
        float terrainHeight = 0f;

        if (Physics.Raycast(new Vector3(newX, 1000f, newZ), Vector3.down, out hit, Mathf.Infinity, terrainLayer))
        {
            terrainHeight = hit.point.y;
        }

        Vector3 newPos = new Vector3(newX, terrainHeight + heightAboveTerrain, newZ);
        transform.position = newPos;
        Vector3 forwardDirection = (transform.position - previousPosition).normalized;

        if (forwardDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(forwardDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothness);
        }

        previousPosition = transform.position;
    }
}