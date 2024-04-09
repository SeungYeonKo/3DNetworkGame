using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_RoomList : MonoBehaviourPunCallbacks    // 포톤에서 일어나는 일을 감지하는 클래스
{
    public List<UI_Room> UIRooms;

    public void Start()
    {
        Clear();
    }

    private void Clear()
    {
        foreach (UI_Room roomUI in UIRooms)
        {
            roomUI.gameObject.SetActive(false);
        }
    }

    // 룸(방)의 정보가 변경되었을 때 호출되는 콜백 함수
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Clear();

        List<RoomInfo> liveRoomList = roomList.FindAll(r => r.RemovedFromList == false);
        int roomCount = liveRoomList.Count;
        for(int i = 0; i < roomCount; i++)
        {
            UIRooms[i].Set(roomList[i]);
            UIRooms[i].gameObject.SetActive(true);
        }
    }
}
