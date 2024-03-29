using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_RoomInfo : MonoBehaviourPunCallbacks
{
    public static UI_RoomInfo Instance { get; private set; }

    public Text RoomNameTextUI;
    public Text PlayerCountTextUI;
    public Text LogTextUI;

    private string _logText = string.Empty;
    private bool _init;

    private void Awake()
    {
        Instance = this;
    }

    // 방에 입장
    public override void OnJoinedRoom()
    {
        if (!_init)
        {
            Init();
        }
    }

    void Start()
    {
        if(!_init && PhotonNetwork.InRoom)
        {
            Init();
        }
    }

    // 새로운 플레이어가 룸에 입장했을 때 호출되는 콜백 함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        _logText += $"\n{newPlayer.NickName}님이 입장했습니다";
        Refresh();
    }

    // 플레이어가 룸에서 퇴장했을 때 호출되는 콜백 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Refresh();
    }

    private void Init()
    {
        _init = true;

        RoomNameTextUI.text = PhotonNetwork.CurrentRoom.Name;
        PlayerCountTextUI.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

        _logText += "방에 입장했습니다!";
        Refresh();
    }

    private void Refresh()
    {
        LogTextUI.text = _logText;
        PlayerCountTextUI.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
    }

    public void AddLog(string logMessage)
    {
        _logText += logMessage;
        Refresh();
    }
}
