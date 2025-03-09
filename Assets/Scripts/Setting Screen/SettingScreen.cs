using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingScreen : MonoBehaviour
{
   
    [SerializeField] private Button btnVibration;
    [SerializeField] private GameObject vibrationOn;
    [SerializeField] private GameObject vibrationOff;

    private void Start()
    {

        if (!PlayerPrefs.HasKey("Vibration"))
        {
            PlayerPrefs.SetInt("Vibration", 1);
            PlayerPrefs.Save();
        }
        UpdateImage();
        btnVibration.onClick.AddListener(ToggleVibration);
    }

    private void ToggleVibration()
    {
        int currentVibration = PlayerPrefs.GetInt("Vibration", 1);
        int newVibration = (currentVibration == 0) ? 1 : 0;
        PlayerPrefs.SetInt("Vibration", newVibration);
        PlayerPrefs.Save();
        UpdateImage();
    }

    private void UpdateImage()
    {
        bool isVibrationOn = PlayerPrefs.GetInt("Vibration") == 1;
        btnVibration.GetComponent<Image>().color = isVibrationOn ? Color.green : Color.red;
        vibrationOn.SetActive(isVibrationOn);
        vibrationOff.SetActive(!isVibrationOn);
    }
}
