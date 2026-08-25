using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private Slider timerSlider;
    [SerializeField] private Image circleTimer;
    [SerializeField] private bool countsUp = false; // If true, the timer counts up instead of down

    private bool isCircleTimer = false;
    private bool isTimerRunning = false;

    private float timerDuration = 0f;
    private float timerRemaining = 0f;

    // Update is called once per frame
    void Update()
    {
        if (isTimerRunning)
        {
            if (countsUp)
            {
                timerRemaining += Time.deltaTime;
            }
            else
            {
                timerRemaining -= Time.deltaTime;
            }
            if (isCircleTimer)
            {
                circleTimer.fillAmount = timerRemaining / timerDuration;
            }
            else
            {
                timerSlider.value = timerRemaining / timerDuration;
            }
            if ((timerRemaining <= 0f && !countsUp) || (timerRemaining >= timerDuration && countsUp))
            {
                isTimerRunning = false;
                if (isCircleTimer)
                {
                    circleTimer.fillAmount = countsUp ? 1f : 0f;
                    circleTimer.gameObject.SetActive(false);
                }
                else
                {
                    timerSlider.value = countsUp ? 1f : 0f;
                    timerSlider.gameObject.SetActive(false);
                }
                // Timer has finished, you can trigger an event or call a method here
            }
        }
    }

    public void SetTimer(float time)
    {
        if (circleTimer == null && timerSlider == null)
        {
            Debug.LogWarning("No timer UI elements assigned.");
            return;
        }

        if (circleTimer == null) 
        {
            isCircleTimer = false;
        }
        else if (timerSlider == null)
        {
            isCircleTimer = true;
        }

        timerDuration = time;
        timerRemaining = countsUp ? 0f : time;
        if (isCircleTimer)
        {
            circleTimer.fillAmount = countsUp ? 0f : 1f;
            circleTimer.gameObject.SetActive(true);
        }
        else
        {
            timerSlider.value = countsUp ? 0f : 1f;
            timerSlider.gameObject.SetActive(true);
        }
        isTimerRunning = true;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
        if (isCircleTimer)
        {
            circleTimer.fillAmount = countsUp ? 1f : 0f;
            circleTimer.gameObject.SetActive(false);
        }
        else
        {
            timerSlider.value = countsUp ? 1f : 0f;
            timerSlider.gameObject.SetActive(false);
        }
    }
}
