using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveEntrance : MonoBehaviour
{
    public Material materialToFade;
    public float fadeStartDistance = 10f;
    public float fadeEndDistance = 5f;

    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        float alpha = Mathf.InverseLerp(fadeStartDistance, fadeEndDistance, distance);
        materialToFade.color = new Color(materialToFade.color.r, materialToFade.color.g, materialToFade.color.b, Mathf.Min(1 - alpha, 0.55f));
    }
}