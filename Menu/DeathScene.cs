using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScene : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance.Play("DeathMusic", transform);
        StartCoroutine(LoadScene());
    }
    
    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Island_New");
    }

    public void FemaleDeath()
    {
        AudioManager.Instance.PlayOneShot("FemaleDeath");
    }
    
    public void HitGround()
    {
        AudioManager.Instance.PlayOneShot("DeathFall");
    }
}
