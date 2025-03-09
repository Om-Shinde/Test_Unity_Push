using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google;
using System.Threading.Tasks;
using TMPro;

public class GoogleController : MonoBehaviour
{
 
    public static GoogleController instance;

    [SerializeField] private string webClientId = "187364048875-m9m1ht2gg5qqjqof72ig7qpnsrsaci3t.apps.googleusercontent.com";

    private GoogleSignInConfiguration configuration;

    private LoginCallBack loginCallback;

    public void SetLoginCallback(LoginCallBack loginCallback)
    {
        this.loginCallback = loginCallback;
    }

    void Awake()
    {
        instance = this;
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true
        };

    }

    private void Start()
    {
        if(PlayerPrefs.GetInt("GoogleLoggedIn") ==1)
        {
            OnSignInSilently();
        }
    }

    public void OnSignInSilently()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
 
        GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnAuthenticationFinished);
    }

    public void OnSignIn()
    {
        Debug.Log("Google Sine In Success");
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
        OnAuthenticationFinished);
    }

    public void OnSignOut()
    {
        Debug.Log("LogOut Methad Call");
        PlayerPrefs.SetInt("GoogleLoggedIn", 0);
        GoogleSignIn.DefaultInstance.SignOut();
        loginCallback.OnLogoutComplete();
      
    }
    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {

        Debug.LogError("OnAuthentication Finished");
        if (task.IsFaulted)
        {
            Debug.LogError("Is Faulted");
            using (IEnumerator<System.Exception> enumerator =
                    task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error =
                            (GoogleSignIn.SignInException)enumerator.Current;
                    Debug.LogError("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    Debug.LogError("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("Canceled");
        }
        else
        {
             
            Debug.Log("Welcome: " + task.Result.DisplayName);
            PlayerPrefs.SetInt("GoogleLoggedIn", 1);
            PlayerPrefs.SetString("GoogleUserName", task.Result.DisplayName);
        
            //UiManager.Instance.UpadteUserName(task.Result.DisplayName);
            //ResourceManager.Instance().AddData("Coin", 30);
            //Toast.Instance.ShowMessage("You Get 30 Coins");
            Debug.Log("Name"+task.Result.DisplayName);
            
           // MenuController.Instance().UpdateResources();
           loginCallback.OnGoogleLoginComplete(task.Result.DisplayName);

        }
    }

}

