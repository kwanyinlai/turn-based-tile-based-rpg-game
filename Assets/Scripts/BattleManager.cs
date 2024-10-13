using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class BattleManager : MonoBehaviour
{

    UnitStats enemy;
    UnitStats initiator;
    bool playerTurn;
    bool inconclusive;
    int enemyAttacks;
    int playerAttacks;
    int enemyRealAtk;
    int playerRealAtk;
    Vector3 selected;
    Vector3[] enemiesInRange;
    enum direction { LEFT=0, RIGHT=1, UP=2, DOWN=3};

    private void Start()
    {
        enemiesInRange = new Vector3[4];
    }



    public void FindOpoonent()
    {
        GameObject.Find("Canvas").GetComponent<ActionButton>().actionMenu.SetActive(false);
        ActionButton.fightMenu = true;
        Vector3 playerPosition = GameObject.Find(PlayerController.characterBeingControlled).GetComponent<PlayerController>().currentPos;
        enemiesInRange = new Vector3[4];

        foreach (Vector3 enemyPosition in CharacterManager.enemyPositions)
        {
            if ((playerPosition + new Vector3Int(-1, 0, 0)) == enemyPosition)
            {
                enemiesInRange[(int)direction.LEFT] = enemyPosition;
               
                
            } else if ((playerPosition + new Vector3Int(1, 0, 0)) == enemyPosition)
            {
                enemiesInRange[(int)direction.RIGHT] = enemyPosition;
            }
            else if ((playerPosition + new Vector3Int(0, 1, 0)) == enemyPosition)
            {
                enemiesInRange[(int)direction.UP] = enemyPosition;
            }
            else if ((playerPosition + new Vector3Int(0, -1, 0)) == enemyPosition)
            {
                enemiesInRange[(int)direction.DOWN] = enemyPosition;
            }

        }

    }





    public void Fight(UnitStats initiator, UnitStats enemy)
    {
        playerRealAtk = 0;
        enemyRealAtk = 0;
        if (initiator.type == "physical") { playerRealAtk = initiator.atk - enemy.def; }
        else if (initiator.type == "magic") { playerRealAtk = initiator.atk - enemy.res; }

        if (enemy.type == "physical") { enemyRealAtk = enemy.atk - initiator.def; }
        else if (enemy.type == "magic") { enemyRealAtk = enemy.atk - initiator.res; }

        playerAttacks = 1;
        enemyAttacks = 1;

        if (enemy.spd - initiator.spd >= 5)
        {
            playerAttacks = 2;
        }
        else if (enemy.spd - initiator.spd <= -5)
        {
            enemyAttacks = 2;
        }

        UnityEngine.Debug.Log("player real attack = "+playerRealAtk);
        UnityEngine.Debug.Log("enemy real attack = " + enemyRealAtk);
        UnityEngine.Debug.Log("player attacks = " + playerAttacks);

        playerTurn = true;
        inconclusive = true;
        
        while (inconclusive)
        {
            if (playerTurn == true && playerAttacks != 0)
            {

                StartCoroutine(PlayerPhase(enemy));
            }
            else if (playerTurn == false && enemyAttacks != 0)
            {

                StartCoroutine(EnemyPhase(initiator));

            }
            else if (playerTurn == false && enemyAttacks == 0 && playerAttacks != 0)
            {
                playerTurn = true;
            }

            else if (playerTurn == true && playerAttacks == 0 && enemyAttacks != 0)
            {
                playerTurn = false;
            }


            if ((playerAttacks == 0 && enemyAttacks == 0) || (initiator.currentHP == 0 || enemy.currentHP == 0))
            {
                inconclusive = false;
            }



        }
        


        UnityEngine.Debug.Log("Enemy's HP is " + enemy.currentHP);
        UnityEngine.Debug.Log("Player's HP is " + initiator.currentHP);

        if (enemy.currentHP <= 0)
        {
            enemy.Kill();
        }
        else if (initiator.currentHP <= 0)
        {
            initiator.Kill();
            
        }
        
        ActionButton.fightMenu = false;
        
        ActionButton.menu = false;
        CharacterManager.Wait();



    }

    IEnumerator PlayerPhase(UnitStats enemy) //now workign on animating battles, then i can probably think about just building maps, generic enemies, game music, game mechanics, much more chill
    {
        UnityEngine.Debug.Log(enemy.currentHP);
        UnityEngine.Debug.Log(playerRealAtk);
        enemy.currentHP -= playerRealAtk;
        playerAttacks -= 1;
        playerTurn = false;
        //play animation
        yield return new WaitForSeconds(1f);
    }

    IEnumerator EnemyPhase(UnitStats initiator)
    {
        initiator.currentHP -= enemyRealAtk;
        enemyAttacks -= 1;
        playerTurn = true;
        //play animation
        yield return new WaitForSeconds(1f);
    }
    public void initCombat(UnitStats enemy) //checking which way combat is initiated towards
    {
        UnitStats initiator = GameObject.Find(PlayerController.characterBeingControlled).GetComponent<UnitStats>();
        if (enemy.attacksFirst)
        {
            Fight(enemy, initiator);
        }
        else
        {
            Fight(initiator,enemy);
        }
    }

    private void Update()
    {
        if (ActionButton.fightMenu)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (enemiesInRange[(int)direction.RIGHT] != Vector3.zero)
                {
                    selected = enemiesInRange[(int)direction.RIGHT];
                    MapRenderer.pointerPos = selected;
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (enemiesInRange[(int)direction.LEFT] != Vector3.zero)
                {
                    selected = enemiesInRange[(int)direction.LEFT];
                    MapRenderer.pointerPos = selected;
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow)) {

                if (enemiesInRange[(int)direction.UP] != Vector3.zero)
                {
                    selected = enemiesInRange[(int)direction.UP];
                    MapRenderer.pointerPos = selected;
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (enemiesInRange[(int)direction.DOWN] != Vector3.zero)
                {
                    selected = enemiesInRange[(int)direction.DOWN];
                    MapRenderer.pointerPos = selected;
                }
            }

        }
        if (Input.GetKey(KeyCode.Z) && selected!=Vector3.zero)
        {
            
            int index = -1;
            for (int i = 0; i < CharacterManager.enemyPositions.Count; i++)
            {
                if (selected == CharacterManager.enemyPositions[i])
                {
                    index = i;
                }
            }
            initCombat(GameObject.Find(CharacterManager.enemyNames[index]).GetComponent<UnitStats>());
            selected = Vector3.zero;
        }
    }

}

