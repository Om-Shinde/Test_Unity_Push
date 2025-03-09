using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Toast : MonoBehaviour
{
   public static Toast Instance { get; private set; }

    [Header("Spin And Win")]
    [SerializeField] private GameObject messageSpinPanel;
    [SerializeField] private TextMeshProUGUI spinTxtMessage;
    [SerializeField] private Button btnClose;



    private void Awake()
    {
        if (Instance) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        
        btnClose.onClick.AddListener(CloseSpin);
       
    }

    public void ShowSpinMessage(string message)
    {
        //AudioManager.Instance().PanelOpen();
        messageSpinPanel.SetActive(true);
        
        spinTxtMessage.text = message;
        BackButton.Instance().SetBackButtonCallback(CloseSpin);
    }
    

  

    public void CloseSpin()
    {

        // AudioManager.Instance().PanleClose();
        messageSpinPanel.SetActive(false);
    }
  
}

