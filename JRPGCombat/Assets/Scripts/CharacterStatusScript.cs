using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatusScript : MonoBehaviour
{
    public int maxHealth, currHealth;
    public int maxMana, currMana;
    public int damageOutput;
    // Start is called before the first frame update

    public bool TakeDamage(int damage)
    {
        currHealth -= damage;

        if(currHealth <= 0)
        {
            return true;
        }
        return false;
    }

    public void Heal(int hp, int mana)
    {
        currHealth += hp;
        currMana -= mana;

        if(currHealth >= maxHealth)
        {
            currHealth = maxHealth;
        }
    }
}
