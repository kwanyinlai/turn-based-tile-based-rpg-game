using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEngine;
using System.Linq;

public class EnemyController : MonoBehaviour
{
    private readonly float moveSpeed = 2.56f;
    public Vector3Int enemyPosition;
    public bool yourTurn = false;
    private int mvt = 8;
    public List<MapRenderer.Nodes> pathFinder = new List<MapRenderer.Nodes>();
    private int i;
    public bool withinAttack = false;
    public static int notYetActed = 0;
    public bool hasActed = false;


    //find out why "player turn" isnt starting after all enemies move



    // Start is called before the first frame update
    void Start()
    {
        enemyPosition = Vector3Int.FloorToInt((this.transform.position - new Vector3(0.32f, 0.32f, 0)) / 0.64f);
        
        CharacterManager.enemyNames.Add(this.name);
        CharacterManager.enemyPositions.Add(enemyPosition);
        notYetActed++;
        
        
    }

    public float prioritizationCalc()
    {

        return 0f;
    }


    



    void Update()
    {
        if (yourTurn == true)
        {
            hasActed = false;

            pathFinder = CharacterManager.moveUnitTo(enemyPosition, CharacterManager.playerPositions[0], false);
            transform.position = pathFinder[0].nodeToVector();
            yourTurn = false;
            i = 0;
            if (pathFinder.Count == 1)
            {
                withinAttack = true;
            }
        }
        if (pathFinder != null)
        {
            if (pathFinder.Count != 0)
            {

                transform.position = Vector3.MoveTowards(transform.position, pathFinder[0].nodeToVector(), moveSpeed * Time.deltaTime);
                enemyPosition = Vector3Int.FloorToInt(pathFinder[0].nodeToVector() - new Vector3(0.32f, 0.32f, 0)/ 0.64f);
                if (pathFinder.Count != 1 && i != mvt)
                {
                    if (transform.position.Equals(pathFinder[0].nodeToVector()))
                    {

                        pathFinder.RemoveAt(0);

                        i++;
                    }
                }
                else
                {
                    pathFinder = null;

                    notYetActed -= 1;


                    if (notYetActed == 0)
                    {
                        PlayerController.playerTurn = true;

                        PlayerController.numNotYetActed = CharacterManager.playerNames.Count;
                        foreach (string player in CharacterManager.playerNames)
                        {
                            GameObject.Find(player).GetComponent<PlayerController>().hasActed = false;
                        }

                    }

                }


            }
        }
        else
        {

            if (!PlayerController.playerTurn && !hasActed) {
                Debug.Log(CharacterManager.enemyNames.Count);
                Debug.Log(PlayerController.playerTurn);
                EnemyController nextEnemy = GameObject.Find(CharacterManager.enemyNames[CharacterManager.enemyNames.IndexOf(this.name) + 1]).GetComponent<EnemyController>();
                nextEnemy.yourTurn = true;
                hasActed = true;
                
            }
            
        }


        
        

        if (pathFinder != null)
        {
            int j = 0;
            while (j < pathFinder.Count - 1)
            {


                Debug.DrawLine(pathFinder[j].nodeToVector(), pathFinder[j+1].nodeToVector());
                j++;
            }

        }


    }
}
