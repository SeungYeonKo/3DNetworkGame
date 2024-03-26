using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterStat : MonoBehaviour
{
    protected Character Owner { get; private set; }
    

    [Header("스태미나 슬라이더 UI")]
    public Slider StaminaSliderUI;
    [Header("체력 슬라이더 UI")]
    public Slider HealthSliderUI;

    private void Awake()
    {
        Owner = GetComponent<Character>();
    }
    private void Update()
    {
        HealthSliderUI.value = (float)Owner.Stat.Health / (float)Owner.Stat.MaxHealth;
        StaminaSliderUI.value = Owner.Stat.Stamina / Owner.Stat.MaxStamina;
    }
}
