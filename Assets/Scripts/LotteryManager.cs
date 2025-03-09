using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LotteryManager : MonoBehaviour
{
    public GameObject lotteryPanel;
    public Button[] lotteryButtons;
    public TMP_Text[] lotteryButtonTexts;
    public TMP_Text timerText;
    public GameObject timerPanel;
    private int chosenNumber;
    public TmpLongPopup popupManager;

    void Start()
    {
        if (popupManager == null)
        {
            popupManager = FindObjectOfType<TmpLongPopup>();
        }

        if (lotteryPanel == null)
        {
            Debug.LogError("LotteryPanel is not assigned!");
            return;
        }

        if (lotteryButtons == null || lotteryButtons.Length == 0)
        {
            Debug.LogError("LotteryButtons are not assigned!");
            return;
        }

        if (lotteryButtonTexts == null || lotteryButtonTexts.Length == 0)
        {
            Debug.LogError("LotteryButtonTexts are not assigned!");
            return;
        }

        if (timerText == null)
        {
            Debug.LogError("TimerText is not assigned!");
            return;
        }

        if (timerPanel == null)
        {
            Debug.LogError("TimerPanel is not assigned!");
            return;
        }

        // Hide the lottery and timer panels at the start
        lotteryPanel.SetActive(false);
        timerPanel.SetActive(false);

        // Assign random 3-digit numbers to the button texts and add click listeners
        for (int i = 0; i < lotteryButtons.Length; i++)
        {
            if (lotteryButtons[i] == null || lotteryButtonTexts[i] == null)
            {
                Debug.LogError("A lottery button or text is not assigned!");
                continue;
            }

            int randomNumber = Random.Range(100, 1000);
            lotteryButtonTexts[i].text = randomNumber.ToString();
            int number = randomNumber; // Capture the current value of randomNumber
            lotteryButtons[i].onClick.AddListener(() => OnLotteryButtonClick(number));
            // lotteryButtons[i].onClick.AddListener(() => Debug.Log("Button Clicked: " + number));
        }
    }

    public void ShowLotteryPanel()
    {
        // Show the lottery panel when the method is called
        lotteryPanel.SetActive(true);
    }

    void OnLotteryButtonClick(int number)
    {
        chosenNumber = number;
        //  Debug.Log("Chosen Number: " + chosenNumber);

        // Hide the lottery panel after selection
        lotteryPanel.SetActive(false);

        // Show the timer panel and start the countdown
        timerPanel.SetActive(true);
        StartCoroutine(TimerCountdown(5));
    }

    IEnumerator TimerCountdown(int seconds)
    {
        int timeLeft = seconds;
        while (timeLeft > 0)
        {
            timerText.text = timeLeft + "s";
            yield return new WaitForSeconds(1);
            timeLeft--;
        }

        // Determine if the player wins or loses
        bool isWinner = Random.value < 0.25f; // 25% probability of winning

        if (isWinner)
        {
            // Debug.Log("Lottery Result: Win!");
            popupManager.ShowPopup($"Your Number {chosenNumber} won, you got 25000", Color.green, new Vector3(450, 1700, 0));
            GameManager.numberOfCoins += 25000;
        }
        else
        {
            //   Debug.Log("Lottery Result: Lose!");
            popupManager.ShowPopup($"Your Number {chosenNumber} did not win", Color.red, new Vector3(500, 1700, 0));
        }

        // Hide the timer panel after the countdown
        timerPanel.SetActive(false);
    }
}