using System.Collections;
using UnityEngine;

public class FloatAndPulse : MonoBehaviour
{
    public float floatHeight = 0.4f;
    public float floatSpeed = 3f;
    public float pulseScale = 0.1f;
    public float pulseSpeed = 3f;
    public float extraHeight = 0f;

    private Vector3 initialScale;
    private Vector2 floatingPosition;
    private bool hasCollidedWithTerrain;

    private void Start()
    {
        initialScale = transform.localScale;
        Rigidbody rb = GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            rb.useGravity = false;
        }

        IgnoreCollisionsWithOtherObjects();
        StartCoroutine(DestroyAfterTwoMinutes());
    }

    IEnumerator DestroyAfterTwoMinutes()
    {
        yield return new WaitForSeconds(40);
        Destroy(gameObject);
    }


    IEnumerator DoFloatAndPulse()
    {
        while (true)
        {
            RaycastHit hit;
            
            if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
            {
                float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
                float clampedY = Mathf.Clamp(hit.point.y + floatHeight + yOffset + extraHeight, hit.point.y + floatHeight + extraHeight, Mathf.Infinity);
                transform.position = new Vector3(floatingPosition.x, clampedY, floatingPosition.y);
            }

            float scaleFactor = Mathf.Sin(Time.time * pulseSpeed) * pulseScale;
            transform.localScale = initialScale * (1 + scaleFactor);

            yield return null;
        }
    }

    private void IgnoreCollisionsWithOtherObjects()
    {
        FloatAndPulse[] otherObjects = FindObjectsOfType<FloatAndPulse>();
        foreach (FloatAndPulse otherObject in otherObjects)
        {
            if (otherObject != this)
            {
                Physics.IgnoreCollision(GetComponentInChildren<Collider>(), otherObject.GetComponentInChildren<Collider>());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        int terrainLayer = LayerMask.NameToLayer("Terrain");

        if (other.gameObject.layer == terrainLayer && !hasCollidedWithTerrain)
        {
            floatingPosition = new Vector2(transform.position.x, transform.position.z);
            StartCoroutine(DoFloatAndPulse());
            hasCollidedWithTerrain = true;
        }
        else
        {
            floatingPosition = new Vector2(transform.position.x, transform.position.z);
            StartCoroutine(DoFloatAndPulse());
            hasCollidedWithTerrain = true;
        }
    }

}