using UnityEngine;
using UniStorm;
using TMPro;

public class Clock : MonoBehaviour
{
    public TextMeshProUGUI TimeText;

    void Update()
    {
        int hours = UniStormSystem.Instance.Hour;
        int minutes = UniStormSystem.Instance.Minute;
        string am_pm = hours >= 12 ? "PM" : "AM";
            
        hours = hours % 12;
        if (hours == 0) hours = 12;

        TimeText.text = $"{hours.ToString("00")}:{minutes.ToString("00")} {am_pm}";
    }
}
