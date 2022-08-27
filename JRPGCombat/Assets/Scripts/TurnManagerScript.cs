using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState
{
    START,
    PLAYERTURN,
    ENEMYTURN,
    WIN,
    LOSE
}

public enum BattleType
{
    Enemy,
    Mini_Boss,
    Boss,
    Final_Boss
}

public enum MagicType
{
    FIRE,
    ICE,
    LIGHTNING,
    LIFESTEAL
}
public class TurnManagerScript : MonoBehaviour
{
    public BattleState state;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    CharacterStatusScript playerCharacter;
    CharacterStatusScript enemyCharacter;

    public BattleHUDScript playerHUD;
    public BattleHUDScript enemyHUD;

    public Text dialogue;

    [SerializeField]
    private BattleType battleType = BattleType.Enemy;

    int fireTurn = 0;
    int iceTurn = 0;
    int elecTurn = 0;

    int hpMultiplier = 1;
    int mpMultiplier = 1;
    int dmgMultiplier = 1;

    bool isPhaseTwo = false;

    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;

        switch(battleType)
        {
            case BattleType.Enemy:
                hpMultiplier = 1;
                mpMultiplier = 1;
                dmgMultiplier = 1;
                break;

            case BattleType.Mini_Boss:
                hpMultiplier = 2;
                mpMultiplier = 2;
                dmgMultiplier = 1;
                break;

            case BattleType.Boss:
                hpMultiplier = 10;
                mpMultiplier = 8;
                dmgMultiplier = 6;
                break;

            case BattleType.Final_Boss:
                hpMultiplier = 50;
                mpMultiplier = 50;
                dmgMultiplier = 30;
                break;
        }
        StartCoroutine(SetUpBattle());
    }

    IEnumerator SetUpBattle()
    {
        GameObject player = Instantiate(playerPrefab, playerBattleStation);
        playerCharacter = player.GetComponent<CharacterStatusScript>();

        GameObject enemy = Instantiate(enemyPrefab, enemyBattleStation);
        enemyCharacter = enemy.GetComponent<CharacterStatusScript>();

        dialogue.text = "The " + battleType + " approaches...";

        playerCharacter.maxHealth *= hpMultiplier;
        playerCharacter.currHealth *= hpMultiplier;
        playerCharacter.maxMana *= mpMultiplier;
        playerCharacter.currMana *= mpMultiplier;

        enemyCharacter.maxHealth *= hpMultiplier;
        enemyCharacter.currHealth *= hpMultiplier;
        enemyCharacter.maxMana *= mpMultiplier;
        enemyCharacter.currMana *= mpMultiplier;

        playerHUD.SetHUD(playerCharacter);
        enemyHUD.SetHUD(enemyCharacter);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        dialogue.text = "Choose an action.";
    }

    public void OnAttackButton()
    {
        if(state != BattleState.PLAYERTURN)
        {
            return;
        }

        StartCoroutine(PlayerAttack());
    }

    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }

        StartCoroutine(PlayerHeal());
    }

    public void OnMagicButton(int magicNum)
    {
        if(state != BattleState.PLAYERTURN)
        {
            return;
        }

        StartCoroutine(MagicAttack(magicNum));
    }

    public IEnumerator MagicAttack(int magicNum)
    {
        bool isDead = false;
        int manaCost;
        float dialogueTimer = 0.0f;
        switch ((MagicType)magicNum)
        {
            case MagicType.FIRE:
                manaCost = 5;

                if(playerCharacter.currMana >= manaCost)
                {
                    int fireDamage = Random.Range(5, 7);
                    int statusChance = Random.Range(0, 10);

                    isDead = enemyCharacter.TakeDamage(fireDamage * dmgMultiplier);
                    playerCharacter.currMana -= manaCost;

                    enemyHUD.SetHP(enemyCharacter.currHealth);
                    playerHUD.SetMP(playerCharacter.currMana);

                    dialogue.text = "The enemy has taken " + (fireDamage * dmgMultiplier) + " damage.";
                    yield return new WaitForSeconds(2f);

                    if (statusChance < 3 && (iceTurn == 0 && elecTurn == 0))
                    {
                        dialogue.text = "The enemy is on fire";
                        fireTurn = 3;

                        dialogueTimer = 2.0f;
                    }
                    
                }

                else
                {
                    dialogue.text = "You don't have enough mana.";

                    yield return new WaitForSeconds(2f);
                    PlayerTurn();
                }
                break;
            case MagicType.ICE:
                manaCost = 6;

                if (playerCharacter.currMana >= manaCost)
                {
                    int iceDamage = Random.Range(3, 5);
                    int statusChance = Random.Range(0, 10);

                    isDead = enemyCharacter.TakeDamage(iceDamage * dmgMultiplier);
                    playerCharacter.currMana -= manaCost;

                    enemyHUD.SetHP(enemyCharacter.currHealth);
                    playerHUD.SetMP(playerCharacter.currMana);

                    dialogue.text = "The enemy has taken " + (iceDamage * dmgMultiplier) + " damage.";
                    yield return new WaitForSeconds(2f);

                    if (statusChance < 4 && (fireTurn == 0 && elecTurn == 0))
                    {
                        dialogue.text = "The enemy is frozen.";
                        iceTurn = 3;

                        dialogueTimer = 2.0f;
                    }
                }

                else
                {
                    dialogue.text = "You don't have enough mana.";

                    yield return new WaitForSeconds(2f);
                    PlayerTurn();
                }
                break;
            case MagicType.LIGHTNING:
                manaCost = 8;

                if (playerCharacter.currMana >= manaCost)
                {
                    int elecDamage = Random.Range(10, 14);
                    int statusChance = Random.Range(0, 10);

                    isDead = enemyCharacter.TakeDamage(elecDamage * dmgMultiplier);
                    playerCharacter.currMana -= manaCost;

                    enemyHUD.SetHP(enemyCharacter.currHealth);
                    playerHUD.SetMP(playerCharacter.currMana);

                    dialogue.text = "The enemy has taken " + (elecDamage * dmgMultiplier) + " damage.";
                    yield return new WaitForSeconds(2f);

                    if (statusChance < 1 && (fireTurn == 0 && iceTurn == 0))
                    {
                        dialogue.text = "The enemy is paralyzed.";
                        iceTurn = 3;

                        dialogueTimer = 2.0f;
                    }
                }

                else
                {
                    dialogue.text = "You don't have enough mana.";

                    yield return new WaitForSeconds(2f);
                    PlayerTurn();
                }
                break;
            case MagicType.LIFESTEAL:
                manaCost = 10;

                if(playerCharacter.currMana >= manaCost)
                {
                    int lifeDamage = Random.Range(7, 10);

                    isDead = enemyCharacter.TakeDamage(lifeDamage * dmgMultiplier);
                    playerCharacter.Heal((lifeDamage * dmgMultiplier), manaCost);

                    enemyHUD.SetHP(enemyCharacter.currHealth);
                    playerHUD.SetHP(playerCharacter.currHealth);
                    playerHUD.SetMP(playerCharacter.currMana);

                    dialogue.text = "You have stolen " + (lifeDamage * dmgMultiplier) + " hp.";

                    dialogueTimer = 2.0f;
                }

                else
                {
                    dialogue.text = "You don't have enough mana.";

                    yield return new WaitForSeconds(2f);
                    PlayerTurn();
                }
                break;
        }
        yield return new WaitForSeconds(dialogueTimer);

        if(isDead)
        {
            state = BattleState.WIN;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(CheckStatus());
        }
    }

    public IEnumerator CheckStatus()
    {
        bool isDead = false;
        if (fireTurn > 0)
        {
            isDead = enemyCharacter.TakeDamage(5);
            dialogue.text = "Enemy has been burned";
            fireTurn--;
            yield return new WaitForSeconds(2f);
            enemyHUD.SetHP(enemyCharacter.currHealth);
        }
        if(iceTurn > 0)
        {
            dialogue.text = "The enemy is frozen";
            iceTurn--;
            yield return new WaitForSeconds(2f);
            state = BattleState.PLAYERTURN;
        }
        if(elecTurn > 0)
        {
            dialogue.text = "The enemy is paralyzed";
            elecTurn--;
            yield return new WaitForSeconds(2f);
            state = BattleState.PLAYERTURN;
        }

        if(isDead)
        {
            state = BattleState.WIN;
            EndBattle();
        }
        else
        {
            if(state == BattleState.ENEMYTURN)
            {
                StartCoroutine(EnemyTurn());
            }
            else
            {
                PlayerTurn();
            }
        }
    }

    IEnumerator PlayerAttack()
    {
        float hitChance = Random.Range(0, 100);
        bool isDead = false;

        if(hitChance <= 10)
        {
            dialogue.text = "You missed!";
        }

        else
        {
            int dmgAmt = Random.Range(10, 17);
            dmgAmt *= dmgMultiplier;

            float critChance = Random.Range(0, 100);
            if(critChance >= 95)
            {
                dialogue.text = "Critical hit!";
                dmgAmt *= 2;
                yield return new WaitForSeconds(2f);
            }
            isDead = enemyCharacter.TakeDamage(dmgAmt);
            dialogue.text = "The enemy has taken " + dmgAmt + " damage.";
        }
        
        enemyHUD.SetHP(enemyCharacter.currHealth);

        yield return new WaitForSeconds(2f);

        if(isDead)
        {
            state = BattleState.WIN;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(CheckStatus());
        }
    }

    IEnumerator PlayerHeal()
    {
        int healAmt = Random.Range(6, 13);
        healAmt *= dmgMultiplier;
        int manaCost = 5;
        if(playerCharacter.currMana >= manaCost)
        {
            playerCharacter.Heal(healAmt, manaCost);
            dialogue.text = "You have healed " + healAmt + " hp.";

            playerHUD.SetHP(playerCharacter.currHealth);
            playerHUD.SetMP(playerCharacter.currMana);

            yield return new WaitForSeconds(2f);

            state = BattleState.ENEMYTURN;
            StartCoroutine(CheckStatus());
        }

        else
        {
            dialogue.text = "You don't have enough mana.";

            yield return new WaitForSeconds(2f);
            PlayerTurn();
        }
    }

    IEnumerator EnemyTurn()
    {
        int turnAction = Random.Range(1, 100);
        if(turnAction <= 10 && enemyCharacter.currHealth <= (enemyCharacter.maxHealth / 2))
        {
            int healAmt = Random.Range(8, 15);
            healAmt *= dmgMultiplier;
            int manaCost = 5;
            if(enemyCharacter.currMana >= manaCost)
            {
                enemyCharacter.Heal(healAmt, manaCost);
                enemyHUD.SetHP(enemyCharacter.currHealth);
                enemyHUD.SetMP(enemyCharacter.currMana);

                dialogue.text = "The enemy has healed " + healAmt + " hp.";

                yield return new WaitForSeconds(1f);

                state = BattleState.PLAYERTURN;
                PlayerTurn();
            }
            else
            {
                StartCoroutine(EnemyTurn());
            }
        }

        else
        {
            dialogue.text = "The enemy is attacking";
            yield return new WaitForSeconds(1f);

            float hitChance = Random.Range(0, 100);
            bool isDead = false;

            if (hitChance <= 10)
            {
                dialogue.text = "The enemy missed!";
            }

            else
            {
                int dmgAmt = Random.Range(7, 15);
                dmgAmt *= dmgMultiplier;

                float critChance = Random.Range(0, 100);
                if (critChance >= 95)
                {
                    dialogue.text = "Critical hit!";
                    dmgAmt *= 2;
                    yield return new WaitForSeconds(2f);
                }
                isDead = playerCharacter.TakeDamage(dmgAmt);
                dialogue.text = "You have taken " + dmgAmt + " damage.";
            }

            playerHUD.SetHP(playerCharacter.currHealth);

            yield return new WaitForSeconds(1f);

            if (isDead)
            {
                state = BattleState.LOSE;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;
                PlayerTurn();
            }
        }
    }

    void EndBattle()
    {
        if(state == BattleState.WIN)
        {
            dialogue.text = "Congrats! You win!";
        }
        else if(state == BattleState.LOSE)
        {
            dialogue.text = "You have fallen in battle.";
        }
    }

    IEnumerator BeginPhase2()
    {
        dialogue.text = "...Welcome to Hell.";
        yield return new WaitForSeconds(2.0f);
    }
}
