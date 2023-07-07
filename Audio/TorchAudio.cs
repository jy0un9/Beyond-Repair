using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchAudio : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.Play("Fire", transform);
    }
    
}
