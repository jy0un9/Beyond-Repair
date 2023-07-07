using UnityEngine;
using System.Collections;

public class ShakeHit : MonoBehaviour
{
    [Header("Info")]
    private Vector3 startPos;
    private float timer;
    private Vector3 randomPos;

    [Header("Settings")]
    [Range(0f, 2f)]
    public float shakeDuration = 0.2f;
    [Range(0f, 2f)]
    public float shakeDistance = 0.01f;
    [Range(0f, 0.1f)]
    public float delayBetweenShakes = 0f;

    private void Awake()
    {
        startPos = transform.position;
    }

    private void OnValidate()
    {
        if (delayBetweenShakes > shakeDuration)
            delayBetweenShakes = shakeDuration;
    }

    public void Begin()
    {
        StopAllCoroutines();
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        timer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;

            randomPos = startPos + (Random.insideUnitSphere * shakeDistance);

            transform.position = randomPos;

            if (delayBetweenShakes > 0f)
            {
                yield return new WaitForSeconds(delayBetweenShakes);
            }
            else
            {
                yield return null;
            }
        }

        transform.position = startPos;
    }
}
