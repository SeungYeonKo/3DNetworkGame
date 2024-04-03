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

    // 플레이어의 커스텀 프로퍼티가 변경되면 호출되는 함수
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Refresh();
        // 최적화를 위해선 바뀌는 사람만 호출되게 해야함
    }

    public override void OnJoinedRoom()
    {
        Refresh();
    }

    private void Refresh()
    {
        Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;

        // 자동 정렬
        List<Player> playerList = players.Values.ToList();
        playerList.RemoveAll(player => !player.CustomProperties.ContainsKey("Score"));
        playerList.Sort((player1, player2) =>
        {
            int player1Score = (int)player1.CustomProperties["Score"];
            int player2Score = (int)player2.CustomProperties["Score"];
            return player2Score.CompareTo(player1Score);
        });


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