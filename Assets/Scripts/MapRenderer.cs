using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Windows;

public class MapRenderer : MonoBehaviour
{

    public Tile floor;

    public Tile wall;
    public Tilemap tilemap;
    private int xSize = 100;
    private int ySize = 100;
    public static Nodes[,] graph;
    [SerializeField] GameObject pointer;
    public static Vector3 pointerPos;




    void Start()
    {

        generateGraph();

    }


    public class Nodes
    {
        public List<Nodes> edges;
        public int x;
        public int y;

        public Nodes()
        {
            edges = new List<Nodes>();
        }

        public float distanceTo(Nodes n)
        {
            return Vector2.Distance(new Vector2(x, y), new Vector2(n.x, n.y));
        }

        public Vector3 nodeToVector()
        {
            return new Vector3(x * .64f + .32f, y * .64f + .32f, 0);
        }

        public float costToEnter(Nodes n)
        {
            Tile floor = GameObject.Find("Grid").GetComponent<MapRenderer>().floor;
            Tile wall = GameObject.Find("Grid").GetComponent<MapRenderer>().wall;
            Tilemap tilemap = GameObject.Find("Grid").GetComponent<MapRenderer>().tilemap;
            Vector3Int vector = new Vector3Int(n.x, n.y, 0);
            if (tilemap.GetTile(vector) == floor) { return 1f; }
            else if (tilemap.GetTile(vector).Equals(wall)) { return Mathf.Infinity; }
            else
            {
                return 1f;
            }

        }

    }

    void generateGraph()
    {
        graph = new Nodes[xSize, ySize];

        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                graph[i, j] = new Nodes();
                graph[i, j].x = i;
                graph[i, j].y = j;
            }
        }
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {

                if (i != 0) { graph[i, j].edges.Add(graph[i - 1, j]); }
                if (i != xSize - 1) { graph[i, j].edges.Add(graph[i + 1, j]); }
                if (j != 0) { graph[i, j].edges.Add(graph[i, j - 1]); }
                if (j != ySize - 1) { graph[i, j].edges.Add(graph[i, j + 1]); }


            }

        }
    }


    public void pointerController()
    {
        if (!ActionButton.menu && PlayerController.playerTurn && (PlayerController.characterBeingControlled!=null
            && PlayerController.arrived &&
            !GameObject.Find(PlayerController.characterBeingControlled).GetComponent<PlayerController>().selected
            || PlayerController.characterBeingControlled!=null && !PlayerController.arrived &&
            GameObject.Find(PlayerController.characterBeingControlled).GetComponent<PlayerController>().selected)
            || PlayerController.characterBeingControlled==null) //woohoo complex logic, don't touch this, just trust it works
            {

            UnityEngine.Debug.Log(PlayerController.characterBeingControlled == null);
            int total = PlayerController.characterBeingControlled==null? 9999999: GameObject.Find(PlayerController.characterBeingControlled).GetComponent<PlayerController>().range;
            Vector3Int moved = PlayerController.characterBeingControlled != null ? GameObject.Find(PlayerController.characterBeingControlled).GetComponent<PlayerController>().tilesMoved : new Vector3Int(0,0,0);
            


            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (pointerPos.x != 0 && (PlayerController.characterBeingControlled==null || (moved.x-1+moved.y)<total))
                {
                    pointerPos.x -= 1;

                }
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow) && (PlayerController.characterBeingControlled == null || (moved.x + 1 + moved.y) < total))
            {
                if (pointerPos.x != xSize - 1)
                {
                    pointerPos.x += 1;
                }
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow) && (PlayerController.characterBeingControlled == null || (moved.x + moved.y +1) < total))
            {
                if (pointerPos.y != ySize - 1)
                {
                    pointerPos.y += 1;
                }


            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow) && (PlayerController.characterBeingControlled == null || (moved.x + moved.y-1) < total))
            {

                if (pointerPos.y != 0)
                {
                    pointerPos.y -= 1;
                }
            }
        }





    }


    void Update()
    {
        pointerController();
        pointer.transform.position = pointerPos * 0.64f + new Vector3(0.3f, 0.42f, 0f);


    }

}


