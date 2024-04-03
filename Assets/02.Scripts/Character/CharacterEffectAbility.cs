using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterEffectAbility : CharacterAbility
{
    public List<ParticleSystem> Effects;

    public void RequestPlay(int effectIndex)
    {
        _owner.PhotonView.RPC(nameof(Play), RpcTarget.All, effectIndex);
    }

    [PunRPC]
    private void Play(int effectIndex)
    {
        Effects[effectIndex].Play();
    }
}
