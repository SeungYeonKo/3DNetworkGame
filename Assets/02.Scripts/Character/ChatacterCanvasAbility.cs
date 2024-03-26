using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatacterCanvasAbility : CharacterAbility
{
    public Canvas MyCanvas;
    public Text NicknameTextUI;

    private void Start()
    {
        NicknameTextUI.text = _owner.PhotonView.Controller.NickName;
    }

    private void Update()
    {
        // Todo. 빌보드 구현
        transform.forward = Camera.main.transform.forward;
    }
}
