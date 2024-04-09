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

        // [룸 옵션 설정]
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;                         // 입장 가능한 최대 플레이어 수
        roomOptions.IsVisible = true;                           // 로비에서 방 목록에 노출할 것인가?
        roomOptions.IsOpen = true;                             // 방에 다른 플레이어가 들어올 수 있는가?
        roomOptions.EmptyRoomTtl = 1000 * 20;       //Ttl : Time to live
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()  // 룸 커스텀 프로퍼티( 사용방법 : 플레이어 커스텀 프로퍼티와 같다)
        {
            {"MasterNickname", nickname}
        };
        // 로비에서 공개적으로 표시될 룸 커스텀 프로퍼티의 키를 정의
        // -> 방을 검색하거나 선택할 때 사용자에게 유용한 정보를 제공하기 위해 사용
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "MasterNickname"};

        // 방이 있다면 입장하고 없다면 만들겠다
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

    // 닉네임을 바꾸면 그대로 설정되게하는 함수
    public void OnNicknameValueChanged(string newValue)
    {
        if (string.IsNullOrEmpty(newValue))
        {
            return;
        }
        PhotonNetwork.NickName = newValue;
    }
}