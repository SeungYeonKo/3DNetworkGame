using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterStat : MonoBehaviour
{
    public static UI_CharacterStat Instance { get; private set; }
    public Character MyCharacter;
   
    [Header("체력 슬라이더 UI")]
    public Slider HealthSliderUI;
    [Header("스태미나 슬라이더 UI")]
    public Slider StaminaSliderUI;
    [Header("스코어 Text")]
    public Text ScoreText;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if(MyCharacter == null)
        {
            return;
        }
        HealthSliderUI.value = (float)MyCharacter.Stat.Health / MyCharacter.Stat.MaxHealth;
        StaminaSliderUI.value = MyCharacter.Stat.Stamina / MyCharacter.Stat.MaxStamina;
    }
}
