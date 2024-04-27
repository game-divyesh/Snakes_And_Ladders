using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardGenerator : MonoBehaviour
{
    public static BoardGenerator Instance;

    public SnakeAndLadderGenerator snakeAndLadderGenerator;
    [Space]
    public int width = 10;
    public int height = 10;
    [SerializeField] private Tile tilePrefab;
    [Space(10)]
    public List<Tile> tiles = new List<Tile>();

    [Space(5)]
    public List<TeleportTiles> ladderTiles = new List<TeleportTiles>();
    [Space(5)]
    public List<TeleportTiles> snakeTiles = new List<TeleportTiles>();


    /*[Space]
    [SerializeField] private GameObject snakePrefab;
    [Space]
    [SerializeField] private int lineResolution = 60;
    [SerializeField] private int numWaves = 3;
    [SerializeField] private float waveFrequency = 1f;
    [SerializeField] private float waveAmplitude = 0.5f;*/

    private float startX, startZ = 0;
    private float tileSize = 0;


    [Space]
    [SerializeField] private List<Tile> availableTiles = new List<Tile>();

    private void Awake() => Instance = this;


    #region ----------- BOARD TILE SETUP -----------

    // Function to generate the board based on user input
    public void GenerateBoard()
    {
        // Clear existing tiles
        foreach (Tile tile in tiles)
            DestroyImmediate(tile.gameObject);
        tiles.Clear();
        ladderTiles.Clear();
        snakeTiles.Clear();


        tileSize = tilePrefab.transform.localScale.x;


        // Calculate Center Position Of Grid
        if (width % 2 == 0)
            startX = transform.position.x - ((width / 2 * tileSize) - (tileSize / 2));
        else
            startX = transform.position.x - (width / 2 * tileSize);


        if (height % 2 == 0)
            startZ = transform.position.z - ((height / 2 * tileSize) - (tileSize / 2));
        else
            startZ = transform.position.z - (height / 2 * tileSize);


        // Generate tiles
        for (int row = 0; row < height; row++)
        {
            // Determine the direction (left to right or right to left)
            // Even rows go left to right, odd rows go right to left
            bool leftToRight = (row % 2 == 0);


            // Calculate the start and end indices for the current row
            int startCol = leftToRight ? 0 : width - 1;
            int endCol = leftToRight ? width : -1;
            int stepX = leftToRight ? 1 : -1;

            for (int col = startCol; col != endCol; col += stepX)
            {
                Tile tile = Instantiate(tilePrefab, transform);

                tile.transform.position = GetTilePos(col, row);

                tile.SetTileData(col, row, TileType.Normal, tiles.Count + 1);

                tiles.Add(tile);
            }
        }
    }

    private Vector3 GetTilePos(int col, int row)
    {
        return new Vector3((col * tileSize) + startX, transform.position.y, (row * tileSize) + startZ);
    }


    #endregion


    public void ResetAllTileType()
    {
        for (int index = 0; index < tiles.Count; index++)
            tiles[index].ResetTileType();
    }

    /*#region ----------- SNAKE SETUP -----------
    public void GenerateSnake(Vector3 startPos, Vector3 endPos)
    {
        GameObject snake = Instantiate(snakePrefab, snakesParent);
        LineRenderer lineRenderer = snake.GetComponent<LineRenderer>();

        Vector3[] points = new Vector3[lineResolution];

        for (int i = 0; i < lineResolution; i++)
        {
            float t = (float)i / (lineResolution - 1); // Interpolation factor
            Vector3 lerpedPosition = Vector3.Lerp(startPos, endPos, t); // Linear interpolation between start and end points

            // Calculate the wave offset using sine function
            float waveOffset = Mathf.Sin(t * Mathf.PI * 2 * numWaves * waveFrequency) * waveAmplitude;

            // Apply wave offset based on selected plane
            switch (objectPlane)
            {
                case Plane.XY:
                    lerpedPosition += transform.up * waveOffset; // Apply wave offset along the up direction of the object (XY plane)
                    break;
                case Plane.XZ:
                    lerpedPosition += transform.right * waveOffset; // Apply wave offset along the right direction of the object (XZ plane)
                    break;
                case Plane.YZ:
                    lerpedPosition += transform.forward * waveOffset; // Apply wave offset along the forward direction of the object (YZ plane)
                    break;
            }

            points[i] = lerpedPosition;
        }

        lineRenderer.positionCount = lineResolution;
        lineRenderer.SetPositions(points);
    }
    #endregion*/



    public void GenerateRandomLadderAndSnake()
    {
        if (ladderTiles.Count > 0)
        {
            foreach (TeleportTiles ladder in ladderTiles)
            {
                ladder.ResetTileData();
                DestroyImmediate(ladder.visualObject);
            }
            ladderTiles.Clear();
        }

        if (snakeTiles.Count > 0)
        {
            foreach (TeleportTiles snake in snakeTiles)
            {
                snake.ResetTileData();
                DestroyImmediate(snake.visualObject);
            }
            snakeTiles.Clear();
        }

        availableTiles.AddRange(tiles);

        snakeAndLadderGenerator.GenerateSnakesAndLadders(width, height, availableTiles, ladderTiles, snakeTiles);
    }

    public Tile GetEndTile(Tile startTile, List<TeleportTiles> searchList)
    {
        for (int index = 0; index < searchList.Count; index++)
        {
            if (searchList[index].startTile == startTile)
                return searchList[index].endTile;

        }
        return null;
    }

}// CLASS

[Serializable]
public struct TeleportTiles
{
    public Tile startTile;
    public Tile endTile;
    public GameObject visualObject;

    public List<Tile> areaTilesList;
    public void SetLadderData()
    {
        startTile.type = TileType.LadderStart;
        endTile.type = TileType.LadderEnd;
    }

    public void SetSnakeData()
    {
        startTile.type = TileType.SnakeHead;
        endTile.type = TileType.SnakeTail;
    }

    public void ResetTileData()
    {
        startTile.type = TileType.Normal;
        endTile.type = TileType.Normal;
    }



}