using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerNeeds : MonoBehaviour, IDamageble
{
    public GameObject transitionManager;
    public Need health;
    public Need hunger;
    public Need thirst;
    public Need sleep;
    public float hungerHealthdecay;
    public float thirstHealthdecay;
    public float sleedHealthdecay;
    public UnityEvent onTakeDamage;
    public static PlayerNeeds instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        health.currentValue = health.startValue;
        hunger.currentValue = hunger.startValue;
        thirst.currentValue = thirst.startValue;
        sleep.currentValue = sleep.startValue;
    }

    private void Update()
    {
        hunger.Subtract(hunger.decayRate * Time.deltaTime);
        thirst.Subtract(thirst.decayRate * Time.deltaTime);
        sleep.Subtract(sleep.decayRate * Time.deltaTime);

        if(hunger.currentValue <= 0.0f)
        {
            health.Subtract(hungerHealthdecay * Time.deltaTime);
        }

        if (thirst.currentValue <= 0.0f)
        {
            health.Subtract(thirstHealthdecay * Time.deltaTime);
        }
        
        if (sleep.currentValue <= 0.0f)
        {
            health.Subtract(sleedHealthdecay * Time.deltaTime);
        }        
        if (sleep.currentValue <= 400f)
        {
            TutorialManager.Instance.TaskCompleted("BED",0.2f);
        }

        if(health.currentValue == 0.0f)
        {
            Die();
        }

        health.uiBar.fillAmount = health.GetPercentage();
        hunger.uiBar.fillAmount = hunger.GetPercentage();
        thirst.uiBar.fillAmount = thirst.GetPercentage();
        sleep.uiBar.fillAmount = sleep.GetPercentage();
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);

    }

    public void Drink(float amount)
    {
        thirst.Add(amount);

    }

    public void Sleep(float amount)
    {
        sleep.Subtract(amount);
    }

    public void TakePhysicalDamage(int amount)
    {
        health.Subtract(amount);
        onTakeDamage?.Invoke();
    }

    public void Die()
    {
        transitionManager.GetComponent<SceneTransitions>().LoadScene();
    }
}


[System.Serializable]

public class Need
{
    [HideInInspector]
    public float currentValue;
    public float maxValue;
    public float startValue;
    public float regeneration;
    public float decayRate;
    public Image uiBar;

    public void Add(float amount)
    {
        currentValue = Mathf.Min(currentValue + amount, maxValue);
    }

    public void Subtract(float amount)
    {
        currentValue = Mathf.Max(currentValue - amount, 0.0f);
    }

    public float GetPercentage()
    {
        return currentValue / maxValue;
    }
}
public interface IDamageble
{
    void TakePhysicalDamage(int damageAmount);
}