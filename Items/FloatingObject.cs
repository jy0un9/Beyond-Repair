using System.Collections;
using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    private const int LayerToIgnore = 7;

    public float floatHeight = 0.3f;
    public float liftForce = 5f;
    public float damping = 0.5f;

    private const float BobFrequency = 2f;
    private const float BobAmplitude = 0.1f;
    private float _initialY;
    private const float Height = 0.6f;
    private bool _collidedWithFloor = false;
    private Rigidbody _rigidbody;
    public Collider meshCollider;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        Transform transform1;
        meshCollider = (transform1 = transform).GetChild(0).GetComponent<Collider>();
        _initialY = transform1.position.y;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var player in players)
        {
            Collider playerCollider = player.GetComponent<Collider>();
            if (playerCollider != null)
            {
                Physics.IgnoreCollision(meshCollider, playerCollider);
            }
        }

        foreach (var enemy in enemies)
        {
            Collider enemyCollider = enemy.GetComponent<Collider>();
            if (enemyCollider != null)
            {
                Physics.IgnoreCollision(meshCollider, enemyCollider);
            }
        }
    }

    void FixedUpdate()
    {
        if (!enabled) return;

        if (_collidedWithFloor)
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            int allLayers = ~0;
            int ignoreLayer = 1 << LayerToIgnore;
            int layerMask = allLayers & ~ignoreLayer;

            if (Physics.Raycast(transform.position, -Vector3.up, out var hit, Mathf.Infinity, layerMask))
            {
                float distanceToGround = hit.distance;
                float heightDiff = floatHeight - distanceToGround;

                if (heightDiff > 0f)
                {
                    float lift = heightDiff * liftForce;
                    _rigidbody.AddForce(Vector3.up * lift);
                }

                _rigidbody.velocity *= (1f - damping * Time.deltaTime);
                float newY = (_initialY+Height) + Mathf.Sin(Time.time * BobFrequency) * BobAmplitude;
                var transform1 = transform;
                var position = transform1.position;
                position = new Vector3(position.x, newY, position.z);
                transform1.position = position;
            }
        }
        else
        {
            _rigidbody.constraints = RigidbodyConstraints.None;

        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!enabled) return;

        if (collision.gameObject.CompareTag("Terrain"))
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }

            _collidedWithFloor = true;
            _initialY = transform.position.y;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.rotation = Quaternion.identity;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
        StartCoroutine(StopObjectAfterDelay(0.5f));

    }
    IEnumerator StopObjectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (_rigidbody != null)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }
}