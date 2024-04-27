using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SnakeAndLadderGenerator : MonoBehaviour
{
    public LayerMask tileLayer;
    [Space]
    public int gapBetweenItem = 1;

    [Space(10)]
    [Header("Ladder")]

    [SerializeField] private Transform laddersParent;
    [SerializeField] private LineRenderer ladderPrefab;
    [Space]
    public int numberOfLadder = 0;
    public int maxLengthOfLadder = 1;

    [Space(10)]
    [Header("Snake")]
    [SerializeField] private Transform snakesParent;
    [SerializeField] private LineRenderer snakePrefab;
    [Space]
    public int numberOfSnake = 0;
    public int maxLengthOfSnake = 1;
    public void GenerateSnakesAndLadders(int width, int height, List<Tile> _AvailableTiles, List<TeleportTiles> _LadderTiles, List<TeleportTiles> _SnakeTiles)
    {
        // Limit the Gap between item
        if (gapBetweenItem >= width - 2)
            gapBetweenItem = width - 2;

        GenerateLadders(width, height, _AvailableTiles, _LadderTiles, _SnakeTiles);

        GenerateSnakes(width, height, _AvailableTiles, _LadderTiles, _SnakeTiles);
    }

    private void GenerateLadders(int width, int height, List<Tile> _AvailableTiles, List<TeleportTiles> _LadderTiles, List<TeleportTiles> _SnakeTiles)
    {

        // Limit Length of Snake
        if (maxLengthOfSnake <= 0 || maxLengthOfSnake > height - (height / 2))
            maxLengthOfSnake = height - (height / 2);

        // Limit number of Ladder
        if (numberOfLadder > (width + height) / 3)
            numberOfLadder = (width + height) / 3;

        for (int i = 0; i < numberOfLadder; i++)
            CheckIsSameTeleportTileExist(width, height, true, _AvailableTiles, _LadderTiles, _SnakeTiles);
        Debug.Log($"<color=green>Ladders Completed</color>");
    }


    private void GenerateSnakes(int width, int height, List<Tile> _AvailableTiles, List<TeleportTiles> _LadderTiles, List<TeleportTiles> _SnakeTiles)
    {
        // Limit Length of Snake
        if (maxLengthOfSnake <= 0 || maxLengthOfSnake > height - (height / 3))
            maxLengthOfSnake = height - (height / 3);

        // Limit number of Snake
        if (numberOfSnake > (width + height) / 3)
            numberOfSnake = (width + height) / 3;

        for (int i = 0; i < numberOfSnake; i++)
            CheckIsSameTeleportTileExist(width, height, false, _AvailableTiles, _LadderTiles, _SnakeTiles);
        Debug.Log($"<color=green>Snakes Completed</color>");
    }

    private TeleportTiles SelectLadder_SnakeTiles(bool isLadder, int width, int height, List<Tile> _AvailableTiles)
    {
        // -------------------- Select Starting Tile --------------------
        SelectStartingTile:

        int startTileIndex = Random.Range(0, _AvailableTiles.Count);
        Tile startTile = null;

        if (isLadder)
        {
            // Check starting tile is less than height
            if (_AvailableTiles[startTileIndex].y >= height - 1 ||
                (_AvailableTiles[startTileIndex].x == 0 && _AvailableTiles[startTileIndex].y == 0))
                goto SelectStartingTile;
            else
                startTile = _AvailableTiles[startTileIndex];
        }
        else
        {
            if (_AvailableTiles[startTileIndex].y <= 0 ||
                (_AvailableTiles[startTileIndex].y == height - 1 &&
                ((height % 2 == 0 && _AvailableTiles[startTileIndex].x == 0) ||
                (height % 2 != 0 && _AvailableTiles[startTileIndex].x == width - 1)))
                )
                goto SelectStartingTile;
            else
                startTile = _AvailableTiles[startTileIndex];
        }


        //Debug.Log($"{isLadder}-StartTile -> {startTile}");

        // -------------------- Select Ending Tile --------------------
        SelectEndingTile:

        int endTileIndex = Random.Range(0, _AvailableTiles.Count);
        Tile endTile = null;


        if (isLadder)
        {
            // Check end tile is always have higher row then start tile and it's not a board last tile
            if (_AvailableTiles[endTileIndex].y <= startTile.y ||
                (_AvailableTiles[endTileIndex].y == height - 1 &&
                ((height % 2 == 0 && _AvailableTiles[endTileIndex].x == 0) ||
                (height % 2 != 0 && _AvailableTiles[endTileIndex].x == width - 1))))
                goto SelectEndingTile;
            else
                endTile = _AvailableTiles[endTileIndex];
        }
        else
        {
            if (_AvailableTiles[endTileIndex].y >= startTile.y)
                goto SelectEndingTile;
            else
                endTile = _AvailableTiles[endTileIndex];
        }

        //Debug.Log($"{isLadder} - EndTile -> {endTile}");


        if (isLadder && MathF.Abs(startTile.y - endTile.y) > maxLengthOfLadder)
            goto SelectStartingTile;
        if (!isLadder && (MathF.Abs(startTile.y - endTile.y) > maxLengthOfSnake || MathF.Abs(startTile.y - endTile.y) == 1))
            goto SelectStartingTile;


        // Calculate direction 
        Vector3 direction = endTile.transform.position - startTile.transform.position;

        // Calculate the angle using arctan2
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;


        if (isLadder && (angle >= 135 || angle <= 45))
            goto SelectStartingTile;

        if (!isLadder && (angle <= -135 || angle >= -45))
            goto SelectStartingTile;

        //Debug.Log($"<color=yellow>Angle {angle} </color>");

        // Make Final TeleportTile
        TeleportTiles teleportTiles = new TeleportTiles();
        teleportTiles.startTile = startTile;
        teleportTiles.endTile = endTile;
        if (isLadder)
            teleportTiles.SetLadderData();
        else
            teleportTiles.SetSnakeData();

        teleportTiles.areaTilesList = GetListOfPathTiles(startTile.transform.position, endTile.transform.position);
        teleportTiles.areaTilesList.Add(startTile);
        teleportTiles.areaTilesList.Add(endTile);
        return teleportTiles;
    }

    private void CheckIsSameTeleportTileExist(int width, int height, bool isLadder, List<Tile> _AvailableTiles, List<TeleportTiles> _LadderTiles, List<TeleportTiles> _SnakeTiles)
    {
        CheckAgain:
        bool hasCommonItems = false;

        List<TeleportTiles> compareList = new List<TeleportTiles>();

        compareList.AddRange(_LadderTiles);
        compareList.AddRange(_SnakeTiles);

        TeleportTiles teleportTiles = SelectLadder_SnakeTiles(isLadder, width, height, _AvailableTiles);

        if (compareList.Count > 0)
        {
            foreach (TeleportTiles item in compareList)
            {
                // Avoid Generating Same length Ladder Or Snake in same rows
                if (item.startTile.y == teleportTiles.startTile.y && item.endTile.y == teleportTiles.endTile.y)
                    goto CheckAgain;

                hasCommonItems = teleportTiles.areaTilesList.Intersect(item.areaTilesList).Any();
                if (hasCommonItems)
                    goto CheckAgain;
            }
            if (!hasCommonItems)
                CreateLineRenderer(isLadder, teleportTiles, _LadderTiles, _SnakeTiles, _AvailableTiles);
        }
        else
            CreateLineRenderer(isLadder, teleportTiles, _LadderTiles, _SnakeTiles, _AvailableTiles);

    }

    private List<Tile> GetListOfPathTiles(Vector3 startTile, Vector3 endTile)
    {
        List<Tile> areaTiles = new List<Tile>();
        RaycastHit[] hits = Physics.RaycastAll(startTile, endTile - startTile, Vector3.Distance(startTile, endTile), tileLayer);

        foreach (RaycastHit hit in hits)
        {
            Tile tile = hit.transform.parent.GetComponent<Tile>();
            areaTiles.Add(tile);
        }

        return areaTiles;
    }

    private void CreateLineRenderer(bool isLadder, TeleportTiles teleportTiles, List<TeleportTiles> _LadderTiles, List<TeleportTiles> _SnakeTiles, List<Tile> _AvailableTiles)
    {

        LineRenderer lineRenderer = null;

        if (isLadder)
            lineRenderer = Instantiate(ladderPrefab, laddersParent);
        else
            lineRenderer = Instantiate(snakePrefab, snakesParent);

        Vector3 startPos = new Vector3(teleportTiles.startTile.transform.position.x, 0.05f, teleportTiles.startTile.transform.position.z);

        Vector3 endPos = new Vector3(teleportTiles.endTile.transform.position.x, 0.05f, teleportTiles.endTile.transform.position.z);

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        teleportTiles.visualObject = lineRenderer.gameObject;


        string nameOfTile = string.Empty;
        foreach (var tile in teleportTiles.areaTilesList)
        {
            nameOfTile += $"{tile.name}=>";
            if (_AvailableTiles.Contains(tile))
                _AvailableTiles.Remove(tile);
        }
        //Debug.Log("ALL -> " + nameOfTile);

        if (isLadder)
            _LadderTiles.Add(teleportTiles);
        else
            _SnakeTiles.Add(teleportTiles);


        _AvailableTiles.Remove(teleportTiles.startTile);
        _AvailableTiles.Remove(teleportTiles.endTile);

        // Remove Side Tiles To create Gap
        RemoveAdjacentTiles(teleportTiles.startTile.x, teleportTiles.startTile.y, _AvailableTiles);
        RemoveAdjacentTiles(teleportTiles.endTile.x, teleportTiles.endTile.y, _AvailableTiles);
        //Debug.Log($"Teleport -> {teleportTiles.startTile}_ {teleportTiles.endTile}");
    }


    private void RemoveAdjacentTiles(int targetX, int targetY, List<Tile> _AvailableTiles)
    {
        List<int> indicesToRemove = _AvailableTiles
            .Select((tile, index) => new { Tile = tile, Index = index })
            .Where(item =>
                ((item.Tile.x <= targetX + gapBetweenItem && item.Tile.x >= targetX - gapBetweenItem)
                 && item.Tile.y == targetY))
            .Select(item => item.Index)
            .ToList();

        // Remove tiles from the list using the indices stored in indicesToRemove list
        foreach (int index in indicesToRemove.OrderByDescending(i => i))
            _AvailableTiles.RemoveAt(index);
    }
}// CLASS
