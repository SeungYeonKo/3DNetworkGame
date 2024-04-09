using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterType
{
    Male,       //0
    Female    //1
}


public class UI_Lobby : MonoBehaviour
{
    public static CharacterType SelectedCharacterType = CharacterType.Female;

    public GameObject MaleCharacter;
    public GameObject FemaleChracter;

    public InputField NicknameInputFieldUI;
    public InputField RoomIDInputFieldUI;

    private void Start()
    {
        // 처음 로비에 캐릭터 한 개만 뜨게 하기(초기화)
        MaleCharacter.SetActive(SelectedCharacterType == CharacterType.Male);
        FemaleChracter.SetActive(SelectedCharacterType == CharacterType.Female);
    }

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

    // 캐릭터 타입 버튼을 눌렀을 때 호출되는 함수
    public void OnClickMaleButton() => OnClickCharacterTypeButton(CharacterType.Male);
    public void OnClickFemaleButton() => OnClickCharacterTypeButton(CharacterType.Female);
    public void OnClickCharacterTypeButton(CharacterType characterType)
    {
        SelectedCharacterType = characterType;
        MaleCharacter.SetActive(SelectedCharacterType == CharacterType.Male);
        FemaleChracter.SetActive(SelectedCharacterType == CharacterType.Female);
    }
}