using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayfabController : MonoBehaviour
{
    public static PlayfabController instance { get; private set; }

    private string PlayFabId = "";
    private PlayFabAuthenticationContext playfabAuthenContext;

    [Header("UI")]
    [SerializeField]
    internal InputField emailIntput;
    [SerializeField]
    internal InputField passwordInput;
    public void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = "4DCE9"; 
        }

        //var request = new LoginWithCustomIDRequest
        //{
        //    CustomId = localID,
        //    CreateAccount = true
        //};
        //PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);

        var request = new LoginWithAndroidDeviceIDRequest
        {
            AndroidDeviceId = ReturnMobileID(),
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithAndroidDeviceID(request, OnLoginSuccess, OnLoginFailure);
#if UNITY_ANROID
        

#endif
    }
    #region Login
    private static string ReturnMobileID()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }

    private void OnLoginSuccess(LoginResult result)
    {
        PlayFabId = result.PlayFabId;
        playfabAuthenContext = result.AuthenticationContext;
        Debug.Log("Welcome");
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Register();
    }
    
    public void Register()
    {
        if (emailIntput.text == "" || passwordInput.text == "")
            return;
        var registerRequest = new RegisterPlayFabUserRequest
        {
            Email = emailIntput.text,
            Username = passwordInput.text,
            Password = passwordInput.text,
        };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest,
            OnRegisterSusses, error => { Debug.LogError(error.GenerateErrorReport()); }
            );
    }

    private void OnRegisterSusses(RegisterPlayFabUserResult result)
    {
        Debug.Log("Regis Susses");
    }


    #endregion

    public void SetDisplayName(string playerName)
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest { 
            DisplayName = playerName
        }
        , result => {
            PlayerPrefs.SetString("PlayerName", playerName);
            GameManager.instance.BackToMain();
        }
        , error => { Debug.LogError(error.GenerateErrorReport()); });
    }

    #region Rank

    public void GetRank()
    {
        var requestLeaderboard = new GetLeaderboardRequest
        {
            StartPosition = 0,
            StatisticName = "Score",
            MaxResultsCount = 50,
        };
        PlayFabClientAPI.GetLeaderboard(requestLeaderboard,
            result =>{
                Debug.Log(result.Leaderboard.Count);
                List<PlayerRanking> listPlayer = new List<PlayerRanking>();
                result.Leaderboard.ForEach(v => { 
                    listPlayer.Add(new PlayerRanking(v.DisplayName, v.StatValue)); 
                });
                GameManager.instance.SetRanking(listPlayer);
            },
        error =>{ Debug.LogError(error.GenerateErrorReport()); });

    }
    public void SetStatis(int score)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "Score",
                    Value = score,
                }
            },
        },result => { Debug.Log("Set Score."); },
            error => { Debug.LogError(error.GenerateErrorReport()); });
    }

    #endregion
}
