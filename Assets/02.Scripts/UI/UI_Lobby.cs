using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class UI_Lobby : MonoBehaviour
{
    public InputField NicknameInputFieldUI;
    public InputField RoomIDInputFieldUI;
    public Button MaleCharacterChoice;
    public Button FemaleCharacterChoice;

    public void OnClickMakeRoomButton()
    {
        string nickname = NicknameInputFieldUI.text;
        string roomID = RoomIDInputFieldUI.text;

        if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(roomID))
        {
            Debug.Log("입력해주세요.");
            return;
        }

        PhotonNetwork.NickName = nickname;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;            // 입장 가능한 최대 플레이어 수
        roomOptions.IsVisible = true;              // 로비에서 방 목록에 노출할 것인가?
        roomOptions.IsOpen = true;                // 방에 다른 플레이어가 들어올 수 있는가?

        PhotonNetwork.JoinOrCreateRoom(roomID, roomOptions, TypedLobby.Default);
    }
}