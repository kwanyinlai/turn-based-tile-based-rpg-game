using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CharacterManager : MonoBehaviour
{
    public static List<Vector3> enemyPositions = new List<Vector3>();
    public static List<Vector3> playerPositions = new List<Vector3>();
    public static List<string> enemyNames = new List<string>();
    public static List<string> playerNames = new List<string>();
    public Cinemachine.CinemachineVirtualCameraBase virtualCamera;
    [SerializeField] GameObject actionMenu;
    [SerializeField] GameObject pointer;


    public static List<MapRenderer.Nodes> moveUnitTo(Vector3 enemyPosition, Vector3 playerPosition, bool player)
    {
        //implementation of Djikstra's algorithm
        List<MapRenderer.Nodes> pathFinder = null; //list of nodes tracing the shortest path from source to target

        MapRenderer.Nodes[,] graph = MapRenderer.graph;
        Dictionary<MapRenderer.Nodes, float> dist = new Dictionary<MapRenderer.Nodes, float>(); //stores the distance from source to a node
        Dictionary<MapRenderer.Nodes, MapRenderer.Nodes> prev = new Dictionary<MapRenderer.Nodes, MapRenderer.Nodes>(); //stores the previous node which led to the shortest path
        MapRenderer.Nodes source = graph[(int)enemyPosition.x, (int)enemyPosition.y]; //source node
        MapRenderer.Nodes target = graph[(int)playerPosition.x, (int)playerPosition.y]; //target node
        List<MapRenderer.Nodes> unvisited = new List<MapRenderer.Nodes>(); //list of univisted nodes


        dist[source] = 0; //distance from source node to source node is zero
        prev[source] = null; //no previous node for source node

        foreach (MapRenderer.Nodes v in graph)
        {
            if (v != source)
            {
                dist[v] = Mathf.Infinity; //distance to all nodes other than source node is infinity
                prev[v] = null; //no previous node
            }

            unvisited.Add(v); //add all nodes to unvisited list
        }

        while (unvisited.Count > 0) //while there are still nodes unvisited...
        {
            MapRenderer.Nodes u = null;
            foreach (MapRenderer.Nodes a in unvisited)
            {
                if (u == null || dist[a] < dist[u])
                {
                    u = a;
                }
            }
            //selecting the unvisited node with shortest distance to source, so if the selected node is the target node then the path
            //generated is guaranteed to be shrotest

            if (u == target)
            {
                break;
            }

            unvisited.Remove(u); //because we visit the node, we remove it from unvisited list

            foreach (MapRenderer.Nodes v in u.edges) //check all neighbouring nodes
            {
                float alt = dist[u] + u.costToEnter(v); //calculate distance to that node
                if (alt < dist[v]) //if calculated distance is better than previous distance
                {
                    dist[v] = alt; //replace distance
                    prev[v] = u; //mark the previous node
                }

            }
        }

        if (prev[target] != null)
        {
            MapRenderer.Nodes curr = target;
            pathFinder = new List<MapRenderer.Nodes>(); //list of nodes that led to target node
            while (curr != null) //while there is still a previous node to trace back to
            {
                pathFinder.Add(curr); //add node to list of nodes
                curr = prev[curr]; //repeat
            }

            pathFinder.Reverse(); //reverse the order because it is path from source to target


            // trying to make it so that two enemies cant go on the same space

            /*if (!player)
            {
                foreach (Vector3 ks in CharacterManager.enemyPositions)
                {
                    if (pathFinder[pathFinder.Count - 1].nodeToVector().Equals(enemyPositions))
                    {
                        pathFinder.RemoveAt(pathFinder.Count - 1);
                    }
                }


            }
            */
            
        }

        else
        {
            return null; //no path possible
        }



        return pathFinder;

    }





    public static void Wait()
    {

        bool foundNext = false;
        PlayerController character = GameObject.Find(PlayerController.characterBeingControlled).GetComponent<PlayerController>();

        PlayerController.numNotYetActed -= 1;
        PlayerController.characterBeingControlled = null;

        for (int j = 0; j < playerNames.Count; j++)
        {
            if (CharacterManager.playerNames[j] == character.name)
            {
                character.index = j;
            }

        }

        playerPositions[character.index] = character.currentPos;
        character.previousPos = character.currentPos;
        character.hasActed = true;
        character.selected = false;

        if (PlayerController.numNotYetActed == 0)
        {
            
            PlayerController.playerTurn = false;
            EnemyController.notYetActed = enemyNames.Count;
            PlayerController.numNotYetActed = playerNames.Count;
            GameObject.Find(enemyNames[0]).GetComponent<EnemyController>().yourTurn = true;

        }
        else
        {
            int i = character.index;
            while (foundNext == false && i < playerNames.Count)
            {

                bool characterActed = GameObject.Find(playerNames[i]).GetComponent<PlayerController>().hasActed;

                if (characterActed == false)
                {
                    foundNext = true;
                    i -= 1;

                }
                i++;


            }

            if (foundNext == false)
            {
                i = 0;
                while (!foundNext)
                {


                    bool characterActed = GameObject.Find(playerNames[i]).GetComponent<PlayerController>().hasActed;
                    i++;
                    if (characterActed == false)
                    {
                        foundNext = true;
                        i -= 1;

                    }
                }
            }
            
            PlayerController nextCharacterActing = GameObject.Find(playerNames[i]).GetComponent<PlayerController>();
            MapRenderer.pointerPos = (Vector3)(nextCharacterActing.previousPos);

            PlayerController.characterBeingControlled = null;





        }
        
        GameObject.Find("Canvas").GetComponent<ActionButton>().endAction();






    }



    void Update()
    {
        if (PlayerController.playerTurn)
        {

            if (PlayerController.arrived==false && PlayerController.characterBeingControlled!=null
                && GameObject.Find(PlayerController.characterBeingControlled).GetComponent<PlayerController>().selected || PlayerController.characterBeingControlled==null)
            { //camera's not following when player is moving!!!!

                virtualCamera.Follow = pointer.GetComponent<Transform>();
            }
            else
            {
                virtualCamera.Follow = GameObject.Find(PlayerController.characterBeingControlled).GetComponent<Transform>();
               
            }
        }
        else
        {

            foreach (string enemy in CharacterManager.enemyNames){
                if (GameObject.Find(enemy).GetComponent<EnemyController>().yourTurn)
                {
                    string name = enemy;
                }
            }
            Debug.Log(name);
            virtualCamera.Follow = GameObject.Find(name).GetComponent<Transform>();
        }
    }


}
