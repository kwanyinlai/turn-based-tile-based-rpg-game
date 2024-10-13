using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using Cinemachine;



public class PlayerController : MonoBehaviour
{
    
    public Transform point;
    private readonly float moveSpeed = 2.56f;
    public LayerMask collisionObjects;
    [SerializeField] public bool selected = false;
    [SerializeField] public bool hasActed;
    public static bool playerTurn = true;
    public UnityEvent menu;
    static public string characterBeingControlled;
    static public int numNotYetActed;
    public Animator animator;
    public int index;
    public Vector3Int currentPos; 
    public Vector3Int previousPos;
    public Tilemap tilemap;
    public Tile floorTile;
    public bool first; // ?
    public bool moveTowards;
    public List<MapRenderer.Nodes> pathFinder = new List<MapRenderer.Nodes>();
    public static bool arrived = true;
    public Vector3Int tilesMoved;

    [SerializeField] public int range=8;





    void Start()
    {
        tilemap = GameObject.Find("Grid").GetComponent<MapRenderer>().tilemap;
        floorTile = GameObject.Find("Grid").GetComponent <MapRenderer>().floor;
        point.parent = null;
        currentPos = Vector3Int.FloorToInt((this.point.position - new Vector3(0.32f, 0.32f, 0))/0.64f);
        
        previousPos = currentPos;
        CharacterManager.playerNames.Add(this.name);
        CharacterManager.playerPositions.Add(previousPos);
        numNotYetActed += 1;

    void Update()
    {

        currentPos = Vector3Int.FloorToInt((this.point.position - new Vector3(0.32f, 0.32f, 0)) / 0.64f);
        tilesMoved = currentPos - previousPos;

        if (moveTowards == true)
        {
            pathFinder = CharacterManager.moveUnitTo(previousPos , MapRenderer.pointerPos,true);
            selected = false;
            moveTowards = false;
            

        }



        if (characterBeingControlled == this.name) {
            point.position = MapRenderer.pointerPos*0.64f + new Vector3(0.32f,0.32f,0);
        }



        if (pathFinder != null && pathFinder.Count != 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, pathFinder[0].nodeToVector(), moveSpeed * Time.deltaTime);

            if (pathFinder.Count != 0)
            {


                if (transform.position.Equals(pathFinder[0].nodeToVector()))
                {

                    pathFinder.RemoveAt(0);

                }


            }


        }

        if (arrived = false && characterBeingControlled == this.name && transform.position == point.position)
        {

            arrived = true;
            selected = true;
        }
        else
        {
            // Character Tor can move but Edith cant
            {
                arrived = false;
            }
            
        }








    }
}

