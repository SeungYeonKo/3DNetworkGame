using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class UI_PlayerRanking : MonoBehaviourPunCallbacks
{
    public List<UI_PlayerRankingSlot> Slots;
    public UI_PlayerRankingSlot MySlot;

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Refresh();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Refresh();
    }

    public override void OnJoinedRoom()
    {
        Refresh();
    }

    private void Refresh()
    {
        Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
        List<Player> playerList = players.Values.ToList();
        int playerCount = Mathf.Min(playerList.Count, 5);

        foreach (UI_PlayerRankingSlot slot in Slots)
        {
            slot.gameObject.SetActive(false);
        }
        for (int i = 0; i < playerCount; i++)
        {
            Slots[i].gameObject.SetActive(true);    // 들어와 있는 사람 수 많큼 슬롯이 켜짐
            Slots[i].Set(playerList[i]);                     // 정보를 넘겨줌
        }
        MySlot.Set(PhotonNetwork.LocalPlayer);  // 내 정보, 나 자신을 의미
    }
}