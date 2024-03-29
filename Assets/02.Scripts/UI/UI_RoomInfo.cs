using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_RoomInfo : MonoBehaviourPunCallbacks
{
    public Text RoomNameTextUI;
    public Text PlayerCountTextUI;
    public Text LogTextUI;

    private bool _init;

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

    private void Refresh()
    {
        PlayerCountTextUI.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
    }


    // 새로운 플레이어가 룸에 입장했을 때 호출되는 콜백 함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
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
    }
}
