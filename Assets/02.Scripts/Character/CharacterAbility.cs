using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterAbility : MonoBehaviour
{
    protected Character Owner {  get; private set; }

    protected void Awake()
    {
        Owner = GetComponent<Character>();
    }
}
