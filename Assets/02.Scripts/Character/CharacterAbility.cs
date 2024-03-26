using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public abstract class CharacterAbility : MonoBehaviour
{
    protected Character _owner {  get; private set; }

    protected void Awake()
    {
        _owner = GetComponent<Character>();
    }
}
