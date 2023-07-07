using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAudio : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PlayRandomLooped("Water/Wind", "Water/Wind2", "Water/Wind3", "Water/Wind4", "Water/Wind5");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
