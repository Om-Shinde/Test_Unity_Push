using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using TMPro;
using System.Collections;

public enum RewardType
{
    Diamonds,
}

[System.Serializable]
public struct Reward
{
    public RewardType rewardType;
    public int amount;

    public Reward(RewardType rewardType, int amount)
    {
        this.rewardType = rewardType;
        this.amount = amount;
    }
}

public class SpinAndWin : MonoBehaviour
{
    #region Initialization
    private float genSpeed;
    private float subSpeed;
    private bool isSpin;
    private int numberOfSegments = 10;
    private int diamondCount;

    private const string LastSpinTimeKey = "LastSpinTime";
    private const double LockDurationHours = 0;

    [Header("Buttons")]
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button spinBtn;
    [SerializeField] private GameObject spinPanel;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI lockMessageText;

    [Header("Audio")]
    [SerializeField] private AudioSource spinAudioSource;
    [SerializeField] private AudioClip spinClip;

    [Header("Rewards")]
    [SerializeField] private Reward[] rewards;

    [Header("AllRewards")]
    [SerializeField] private TextMeshProUGUI reward0;
    [SerializeField] private TextMeshProUGUI reward1;
    [SerializeField] private TextMeshProUGUI reward2;
    [SerializeField] private TextMeshProUGUI reward3;
    [SerializeField] private TextMeshProUGUI reward4;
    [SerializeField] private TextMeshProUGUI reward5;
    [SerializeField] private TextMeshProUGUI reward6;
    [SerializeField] private TextMeshProUGUI reward7;
    [SerializeField] private TextMeshProUGUI reward8;
    [SerializeField] private TextMeshProUGUI reward9;

    private float spinTime;
    private const float maxSpinTime = 10f; // Extended spin time to 10 seconds
    #endregion

    #region Start, Update
    private void Start()
    {
        closeBtn.onClick.AddListener(Close);
        spinBtn.onClick.AddListener(SpinWheel);
        ShowRewardItem();
        CheckSpinAvailability();
    }

    public void ShowRewardItem()
    {
        reward0.text = rewards[0].amount.ToString() + rewards[0].rewardType.ToString();
        reward1.text = rewards[1].amount.ToString() + rewards[1].rewardType.ToString();
        reward2.text = rewards[2].amount.ToString() + rewards[2].rewardType.ToString();
        reward3.text = rewards[3].amount.ToString() + rewards[3].rewardType.ToString();
        reward4.text = rewards[4].amount.ToString() + rewards[4].rewardType.ToString();
        reward5.text = rewards[5].amount.ToString() + rewards[5].rewardType.ToString();
        reward6.text = rewards[6].amount.ToString() + rewards[6].rewardType.ToString();
        reward7.text = rewards[7].amount.ToString() + rewards[7].rewardType.ToString();
        reward8.text = rewards[8].amount.ToString() + rewards[8].rewardType.ToString();
        reward9.text = rewards[9].amount.ToString() + rewards[9].rewardType.ToString();
    }

    private void Update()
    {
        CheckSpinAvailability();

        if (isSpin)
        {
            spinTime += Time.deltaTime;

            // Stop the spin after maxSpinTime seconds or when speed reduces to near zero
            if (spinTime >= maxSpinTime || genSpeed <= 0.1f)
            {
                genSpeed = 0;
                isSpin = false;
                DetermineReward();

                if (spinAudioSource != null)
                {
                    spinAudioSource.Stop();
                }

                SaveLastSpinTime();
            }
            else
            {
                // Rotate the wheel
                transform.Rotate(0, 0, -genSpeed * Time.deltaTime);

                // Gradually decrease speed (slow down more smoothly)
                float slowDownRate = Mathf.Lerp(1f, 0.1f, spinTime / maxSpinTime); // Slow down factor
                genSpeed -= subSpeed * slowDownRate * Time.deltaTime;

                // Ensure speed doesn't go negative
                if (genSpeed < 0)
                    genSpeed = 0;

                // Adjust audio pitch and volume dynamically based on the speed
                if (spinAudioSource != null)
                {
                    spinAudioSource.pitch = Mathf.Lerp(0.5f, 1.5f, genSpeed / 400f);
                    spinAudioSource.volume = Mathf.Lerp(0.2f, 1f, genSpeed / 400f);
                }
            }
        }

        if (isSpin)
        {
            PlayerPrefs.SetInt("SpinAndWin", 1);
            closeBtn.gameObject.SetActive(false);
            BackButton.Instance().SetBackButtonCallback(() =>
            {
                Debug.Log("Spin in progress. Please wait until it finishes.");
            });
        }
        else
        {
            closeBtn.gameObject.SetActive(true);
            //BackButton.Instance().SetBackButtonCallback(CloseOpenSpinAndWin);
        }
    }

    public void CloseOpenSpinAndWin()
    {
        Debug.Log("Calling Close Button");
        spinPanel.SetActive(false);
    }
    #endregion

    #region SpinWheel
    public void SpinWheel()
    {
        if (!isSpin && IsSpinAvailable())
        {
            genSpeed = Random.Range(300f, 400f);
            subSpeed = Random.Range(20f, 40f);
            isSpin = true;
            spinTime = 0;

            // Play spin sound
            if (spinAudioSource != null && spinClip != null)
            {
                spinAudioSource.clip = spinClip;
                spinAudioSource.pitch = 1.5f;
                spinAudioSource.volume = 1f;
                spinAudioSource.Play();
            }
        }
    }
    #endregion

    #region DetermineReward
    private void DetermineReward()
    {
        float finalAngle = transform.eulerAngles.z % 360;
        float segmentAngle = 360f / numberOfSegments;
        int segmentIndex = Mathf.FloorToInt((finalAngle + segmentAngle / 2f) / segmentAngle) % numberOfSegments;
        segmentIndex = (segmentIndex + numberOfSegments - 2) % numberOfSegments;

        Reward reward = rewards[segmentIndex];

        switch (reward.rewardType)
        {
            case RewardType.Diamonds:
                diamondCount = PlayerPrefs.GetInt("UserGems");
                PlayerPrefs.SetInt("UserGems", reward.amount);
                diamondCount += reward.amount;
                PlayerPrefs.SetInt("UserGems", diamondCount);
                UiManager.Instance.DiamondUpdate(diamondCount);
                StartCoroutine(WaitForPoPup());
                Toast.Instance.ShowSpinMessage("Diamonds collected! Your balance just increased by " + reward.amount);
                break;
            default:
                Debug.LogError("Unknown reward type");
                break;
        }
    }
    #endregion

    IEnumerator WaitForPoPup()
    {
        Debug.Log("corutine is call");
        yield return new WaitForSeconds(3f);
    }

    #region SpinAvailability
    private void SaveLastSpinTime()
    {
        PlayerPrefs.SetString(LastSpinTimeKey, DateTime.Now.ToString());
        PlayerPrefs.Save();
    }

    private bool IsSpinAvailable()
    {
        if (!PlayerPrefs.HasKey(LastSpinTimeKey))
        {
            return true;
        }

        DateTime lastSpinTime = DateTime.Parse(PlayerPrefs.GetString(LastSpinTimeKey));
        TimeSpan timeSinceLastSpin = DateTime.Now - lastSpinTime;

        return timeSinceLastSpin.TotalHours >= LockDurationHours;
    }

    private void CheckSpinAvailability()
    {
        if (IsSpinAvailable())
        {
            spinBtn.interactable = true;
            lockMessageText.text = "";
        }
        else
        {
            spinBtn.interactable = false;
            DateTime lastSpinTime = DateTime.Parse(PlayerPrefs.GetString(LastSpinTimeKey));
            TimeSpan timeUntilNextSpin = lastSpinTime.AddHours(LockDurationHours) - DateTime.Now;
            lockMessageText.text = $"Next spin available in {timeUntilNextSpin.Hours}h : {timeUntilNextSpin.Minutes}m : {timeUntilNextSpin.Seconds}s";
        }
    }
    #endregion

    #region ClosePanel
    public void Close()
    {
        spinPanel.gameObject.SetActive(false);
    }
    #endregion
}
