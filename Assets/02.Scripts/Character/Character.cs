using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(CharacterMoveAbility))]
[RequireComponent(typeof(CharacterRotateAbility))]
[RequireComponent(typeof(CharacterAttackAbility))]
public class Character : MonoBehaviour
{
    public PhotonView PhotonView { get; private set; }
    public Stat Stat;

    private void Awake()
    {
        Stat.Init();

        PhotonView = GetComponent<PhotonView>();

        if(PhotonView.IsMine)
        {
            UI_CharacterStat.Instance.MyCharacter = this;
        }
    }
}
