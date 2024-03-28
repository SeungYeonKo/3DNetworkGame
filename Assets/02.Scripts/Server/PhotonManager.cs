using System;
using UnityEngine;
// Photon API를 사용하기 위한 네임스페이스
using Photon.Pun;
using Photon.Realtime;

// 역할: 포톤 서버 연결 관리자
public class PhotonManager : MonoBehaviourPunCallbacks // PUN의 다양한 서버 이벤트(콜백 함수)를 받는다.
{
    private void Start()
    {
        // 목적: 연결을 하고 싶다.
        // 순서:
        // 1. 게임 버전을 설정한다.
        PhotonNetwork.GameVersion = "0.0.1";
        // <전체를 뒤엎을 변화>, <기능 수정, 추가>, <버그, 내부적 코드 수정>

        // 2. 닉네임을 설정한다.
        PhotonNetwork.NickName = $"승연_{UnityEngine.Random.Range(0, 100)}";

        // 3. 씬을 설정한다.
        // 4. 연결한다.
        PhotonNetwork.ConnectUsingSettings();

        PhotonNetwork.SendRate = 50;
        PhotonNetwork.SerializationRate = 30;

    }

    // 포톤 서버에 접속 후 호출되는 콜백 함수
    public override void OnConnected()
    {
        Debug.Log("서버 접속 성공");
        Debug.Log(PhotonNetwork.CloudRegion);
    }

    // 포톤 서버 연결 해재 후 호출되는 콜백 함수
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("서버 연결 해재");
    }

    // 포톤 마스터 서버에 접속 후 호출되는 콜백 함수
    // 어느 호텔의 로비에 들어갈 지 선택하는 장소 
    public override void OnConnectedToMaster()
    {
        Debug.Log("마스터 서버 접속 성공");
        Debug.Log($"InLobby?: {PhotonNetwork.InLobby}");

        // 기본 호텔의 로비에 들어가겠다.
        // 로비: 매치매이킹:(방 목록, 방 생성, 방 입장)
        // - 로비 이름
        // - 룸 목록, 룸 개수
        // - 플레이어 수 
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    // 로비에 접속한 후 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        Debug.Log("로비에 입장했습니다.");
        Debug.Log($"InLobby?: {PhotonNetwork.InLobby}");

        //PhotonNetwork.CreateRoom()              // 방을 만드는 것
        //PhotonNetwork.JoinRoom()                // 방에 입장하는 것
        //PhotonNetwork.JoinRandomRoom()          // 랜덤한 방에 입장하는 것
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;   // 입장 가능한 최대 플레이어 수
        roomOptions.IsVisible = true; // 로비에서 방 목록에 노출할 것인가?
        roomOptions.IsOpen = true;
        PhotonNetwork.JoinOrCreateRoom("test", roomOptions, TypedLobby.Default); // 방이 있다면 입장하고 없다면 만드는 것
        //PhotonNetwork.JoinRandomOrCreateRoom(); // 랜덤한 방에 들어가거나 없다면 만드는 것
    }

    // 방 생성에 성공했을 때 호출되는 콜백 함수
    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 성공!");
        Debug.Log($"RoomName: {PhotonNetwork.CurrentRoom.Name}");
    }

    // 방에 들어갔을 때 호출되는 함수
    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 성공!");
        Debug.Log($"RoomName: {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"PlayerCount: {PhotonNetwork.CurrentRoom.PlayerCount}");
        Debug.Log($"MaxCount: {PhotonNetwork.CurrentRoom.MaxPlayers}");

        PhotonNetwork.Instantiate(nameof(Character), Vector3.zero, Quaternion.identity);
    }

    // 방 생성에 실패했을 때 호출되는 콜백 함수
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("방 생성 실패!");
        Debug.Log(message);
    }

}