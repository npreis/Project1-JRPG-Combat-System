using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUDScript : MonoBehaviour
{
    public Slider sliderHP;
    public Slider sliderMP;

    public void SetHUD(CharacterStatusScript charac)
    {
        sliderHP.maxValue = charac.maxHealth;
        sliderHP.value = charac.currHealth;

        sliderMP.maxValue = charac.maxMana;
        sliderMP.value = charac.currMana;
    }

    public void SetHP(int hp)
    {
        sliderHP.value = hp;
    }

    public void SetMP(int mp)
    {
        sliderMP.value = mp;
    }
}
