using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionButton : MonoBehaviour
{
    public static bool menu=false;
    public GameObject actionMenu;
    public GameObject nullButton;
    private bool battlePossible;
    public UnityEngine.UI.Button FightButton;
    [SerializeField] public UnityEngine.UI.Button WaitButton;
    private bool onTop;
    private Vector3 characterPosition;
    private bool pendingArrival;
    enum UnitType { EMPTY, PLAYER, ENEMY};
    public static bool fightMenu;

    void Update()

    {





        if (Input.GetKeyDown(KeyCode.Z) && !menu)
        {
            int overlapIndex = -1;
            UnitType overlapUnit = UnitType.EMPTY;

            
            for (int i =0; i<CharacterManager.playerPositions.Count ;i++)
            {
                if (CharacterManager.playerPositions[i] == MapRenderer.pointerPos)
                {

                    overlapIndex = i;
                    overlapUnit = UnitType.PLAYER;

                }
            }
            if (overlapUnit != UnitType.PLAYER)
            {
                for (int i = 0; i < CharacterManager.enemyPositions.Count; i++)
                {
                    if (CharacterManager.enemyPositions[i] == MapRenderer.pointerPos)
                    {


                        overlapUnit = UnitType.ENEMY;

                    }
                }

            }
            

            if (PlayerController.characterBeingControlled != null) { 
                

                if (overlapIndex >= 0) {
                    if (CharacterManager.playerNames[overlapIndex] == PlayerController.characterBeingControlled &&
                        !GameObject.Find(PlayerController.characterBeingControlled).GetComponent<PlayerController>().first)
                    {
                        openMenu();
                        
                    }
                    

                }
                else if (overlapUnit==UnitType.EMPTY)
                {
                    GameObject.Find(PlayerController.characterBeingControlled).GetComponent<PlayerController>().moveTowards = true;
                    StartCoroutine(waitForArrived());
                }
               


            }
            else 
            {
                if (overlapIndex >= 0)
                {
                    if (!GameObject.Find(CharacterManager.playerNames[overlapIndex]).GetComponent<PlayerController>().hasActed)
                    {
                        PlayerController.characterBeingControlled = CharacterManager.playerNames[overlapIndex];
                        GameObject.Find(PlayerController.characterBeingControlled).GetComponent<PlayerController>().selected = true;

                    }
                    
                } 
                else
                {
                    openMenu();
                }
            }


        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pendingArrival == false)
            {
                if (menu && !fightMenu)
                {
                    endAction();
                }
                else if (!menu && PlayerController.characterBeingControlled != null)
                {
                    GameObject.Find(PlayerController.characterBeingControlled).GetComponent<PlayerController>().selected = false;
                    PlayerController.characterBeingControlled = null;

                }
                else if (fightMenu)
                {
                    fightMenu = false;

                }

            }
            
            
        }

    }

    public void endAction()
    {
        actionMenu.SetActive(false);
        menu = false;
        if (PlayerController.characterBeingControlled != null)
        {

            Vector3 previousPos = (Vector3)(GameObject.Find(PlayerController.characterBeingControlled).GetComponent<PlayerController>().previousPos)*0.64f + new Vector3(0.32f, 0.32f, 0f);
            GameObject.Find("point-" + PlayerController.characterBeingControlled).transform.position = previousPos;
            GameObject charControlling = GameObject.Find(PlayerController.characterBeingControlled);
            charControlling.transform.position = previousPos;
            charControlling.GetComponent<PlayerController>().pathFinder = null;
            MapRenderer.pointerPos = (Vector3)(GameObject.Find(PlayerController.characterBeingControlled).GetComponent<PlayerController>().previousPos);
            PlayerController.characterBeingControlled = null;



        }
       
    }

    IEnumerator waitForArrived()
    {

        pendingArrival = true;
        yield return new WaitUntil(() =>  GameObject.Find(PlayerController.characterBeingControlled).GetComponent<Transform>().position
        == GameObject.Find("point-"+PlayerController.characterBeingControlled).GetComponent<Transform>().position);

        openMenu();
        pendingArrival = false;

    }
    void openMenu()
    {
        if (PlayerController.characterBeingControlled != null)
        {
            battlePossible = false;
            Vector3 playerPosition = GameObject.Find(PlayerController.characterBeingControlled).GetComponent<PlayerController>().currentPos;

            foreach (Vector3 enemyPosition in CharacterManager.enemyPositions)
            {
                if (playerPosition + new Vector3Int(1,0,0) == enemyPosition || playerPosition + new Vector3Int(-1, 0, 0) == enemyPosition ||
                    playerPosition + new Vector3Int(0, 1, 0) == enemyPosition || playerPosition + new Vector3Int(0, -1, 0) == enemyPosition)
                {

                    battlePossible = true;
                    break;
                }
            }

            if (battlePossible)
            {
                FightButton.interactable = true;
            }
            else
            {
                FightButton.interactable = false;
            }


        }

        EventSystem.current.SetSelectedGameObject(nullButton);
        actionMenu.SetActive(true);
        menu = true;
    }
}
