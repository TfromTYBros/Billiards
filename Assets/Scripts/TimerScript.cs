using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    public Slider timer;
    HandBallScript HBS;
    // Start is called before the first frame update
    void Start()
    {
        HBS = FindObjectOfType<HandBallScript>();
        ResetTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (HBS.GetCoroutineNow())
        {
            TimerAdvances();
        }
    }

    public void ResetTimer()
    {
        timer.value = 1400;
    }

    void TimerAdvances()
    {
        if (0 < timer.value) timer.value -= 1;
    }
}
