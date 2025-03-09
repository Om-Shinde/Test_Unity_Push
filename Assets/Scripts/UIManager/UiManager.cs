using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }

    [Header("Diamond Text")]
    [SerializeField] private TextMeshProUGUI diamondHomeText;
    [SerializeField] private TextMeshProUGUI diamondShopText;
    private int diamondCount;


    //[Header("panels")]
    //[SerializeField] private GameObject settingPanel;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void DiamondUpdate(int count)
    {
        
        diamondCount += count;
        diamondHomeText.text = count.ToString();
        diamondShopText.text = count.ToString();
  
    }
    



}
