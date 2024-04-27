using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerCount totalPlayer;
    public int whosTurn = 0;
    public bool canRollDice = false;
    public bool skipAnimation = false;
    [Space]
    [SerializeField] private Player playerPrefab;
    [Space]
    [SerializeField] private float radius;


    [Space(10)]
    public List<Player> playerList = new List<Player>();
    public List<Player> winnerPlayers = new List<Player>();

    [Space(10)]
    [Header("Materials")]
    public List<Material> materialList = new List<Material>();

    private Dictionary<int, List<Player>> playerPosOnTile_Dic = new Dictionary<int, List<Player>>();


    #region ---------------- UNITY METHODS ----------------

    private void Awake() => Instance = this;

    private void OnEnable()
    {
        Dice.onDiceStopped += ActionOnDiceStopped;
        Player.onPlayerStopMoving += ActionOnPlayerStop;
    }

    private void OnDisable()
    {
        Dice.onDiceStopped -= ActionOnDiceStopped;
        Player.onPlayerStopMoving -= ActionOnPlayerStop;
    }

    #endregion

    public void LoadGamePlayScene() => SceneManager.LoadScene("GamePlay", LoadSceneMode.Additive);

    public void UnloadGamePlayScene() => SceneManager.UnloadSceneAsync("GamePlay");


    public void SetDataOnGamePlayStart()
    {
        whosTurn = 0;
        BoardGenerator.Instance.GenerateRandomLadderAndSnake();
        SetupPlayers();

        canRollDice = true;
    }

    public void ResetGameOnComplete()
    {
        foreach (Player player in playerList)
            Destroy(player.gameObject);
        playerList.Clear();
        winnerPlayers.Clear();
        whosTurn = 0;
        canRollDice= false;
        totalPlayer = PlayerCount.TwoPlayer;
    }

    private void SetupPlayers()
    {
        Tile tile = BoardGenerator.Instance.tiles.FirstOrDefault();
        for (int index = 0; index < (int)totalPlayer; index++)
        {
            Player player = Instantiate(playerPrefab, transform);
            player.name = $"Player_{index + 1}";

            player.transform.position = tile.transform.position;

            Vector3 scale = new Vector3(0.2f, 1, 0.2f);
            player.transform.localScale = scale;

            player.GetComponent<Renderer>().material = materialList[index];

            player.currentPos = 0;
            playerList.Add(player);
        }

        playerPosOnTile_Dic.Add(0, playerList.ToList());
        UpdateScale_Pos(0);
        playerList[0].TurnIndicateAnim();
    }

    private void ActionOnDiceStopped(int diceCount)
    {
        if (playerList[whosTurn].isSkipTurn && !playerList[whosTurn].isSkipUsed)
        {
            playerList[whosTurn].StopScaleUpTween();
            playerList[whosTurn].isSkipUsed = true;
            playerList[whosTurn].previousDiceNum = diceCount;
            DOVirtual.DelayedCall(.05f, () =>
            {
                ActionOnPlayerStop();
            });
        }
        else
        {
            if (diceCount == 6 && playerList[whosTurn].previousDiceNum != 6)
            {
                playerList[whosTurn].isTurnAgain = true;
                //playerList[whosTurn].previousDiceNum = 6;
                playerList[whosTurn].MovePlayer(diceCount);
            }
            else if (diceCount == 6 && playerList[whosTurn].previousDiceNum == 6)
            {
                playerList[whosTurn].StopScaleUpTween();
                DOVirtual.DelayedCall(.05f, () =>
                {
                    ActionOnPlayerStop();
                    playerList[whosTurn].isTurnAgain = false;
                    playerList[whosTurn].previousDiceNum = -1;
                });
            }
            else
                playerList[whosTurn].MovePlayer(diceCount);
        }
    }

    private void ActionOnPlayerStop()
    {
        ManagePlayerPosTileDic(playerList[whosTurn]);

        if (playerList[whosTurn].isTurnAgain && playerList[whosTurn].previousDiceNum == -1)
        {
            //playerList[whosTurn].isTurnAgain = false;
            playerList[whosTurn].previousDiceNum = 6;
            whosTurn--;
        }
        else if (playerList[whosTurn].isTurnAgain && playerList[whosTurn].previousDiceNum == 6)
        {
            playerList[whosTurn].isTurnAgain = false;
            playerList[whosTurn].previousDiceNum = -1;
        }

        whosTurn++;
        whosTurn = whosTurn >= (int)totalPlayer ? 0 : whosTurn;

        if (playerList[whosTurn].isReachEnd)
        {
            ActionOnPlayerStop();
            return;
        }

        playerList[whosTurn].TurnIndicateAnim();

        DiceThrower.Instance.ResetDice();
        canRollDice = true;
    }



    public void ManagePlayerPosTileDic(Player player)
    {
        // Remove player from old tile
        if (playerPosOnTile_Dic.ContainsKey(player.previousPos))
        {
            if (playerPosOnTile_Dic[player.previousPos].Contains(player))
            {
                playerPosOnTile_Dic[player.previousPos].Remove(player);
                UpdateScale_Pos(player.previousPos);
            }
        }

        // Add player to new tile
        if (!playerPosOnTile_Dic.ContainsKey(player.currentPos))
            playerPosOnTile_Dic[player.currentPos] = new List<Player>();
        if (!playerPosOnTile_Dic[player.currentPos].Contains(player))
        {
            playerPosOnTile_Dic[player.currentPos].Add(player);
            UpdateScale_Pos(player.currentPos);
        }



        // Remove Empty list Key Value Pair
        // First, find all keys that should be removed
        /*List<int> keysToRemove = new List<int>();
        foreach (var kvp in playerPosOnTile_Dic)
        {
            if (kvp.Value.Count == 0)
                keysToRemove.Add(kvp.Key);
        }

        // Then, remove them from the dictionary
        foreach (int key in keysToRemove)
            playerPosOnTile_Dic.Remove(key);*/

        //PrintAllPlayerPositions();
    }

    // Update the scale of players on a specific tile
    void UpdateScale_Pos(int tileIndex)
    {
        if (playerPosOnTile_Dic.ContainsKey(tileIndex))
        {
            /*string data = string.Empty;

            data += $"<color=yellow>Index : {tileIndex}</color> \n";*/

            List<Player> players = playerPosOnTile_Dic[tileIndex];
            float scaleSize = players.Count > 1 ? 0.25f : 0.4f; // Smaller if more than one player, otherwise normal size

            Vector3 tilePos = BoardGenerator.Instance.tiles[tileIndex].transform.position;

            for (int i = 0; i < players.Count; i++)
            {
                //data += $"<color=white>P : {players[i].name} -> C_{players[i].currentPos} - P_{players[i].previousPos}</color> \n";

                Vector3 scale = new Vector3(scaleSize, 1, scaleSize);
                players[i].currentScale = scale;
                players[i].transform.localScale = scale;

                if (players.Count > 1)
                {
                    float angle = i * Mathf.PI * 2 / players.Count;
                    Vector3 offset = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * radius;
                    // Set player position
                    Vector3 pos = tilePos + offset;
                    pos.y = 0;
                    players[i].transform.position = pos;
                }
                else
                {
                    Vector3 pos = new Vector3(tilePos.x, 0, tilePos.z);
                    players[i].transform.position = pos;
                }
            }

            //Debug.Log(data);
        }
    }

    public void PrintAllPlayerPositions()
    {
        foreach (KeyValuePair<int, List<Player>> entry in playerPosOnTile_Dic)
        {
            string playerNames = string.Empty;

            Debug.Log($"<color=green>Tile Number: {entry.Key}</color>");
            foreach (Player player in entry.Value)
            {
                playerNames += $"{player.name} C_{player.currentPos} P_{player.previousPos}\n";
            }
            Debug.Log($"<color=yellow>Player List: {playerNames}</color>");
        }
    }


    public void CheckGameComplete()
    {

        if(winnerPlayers.Count == (int)totalPlayer - 1)
            UIManager.Instance.ShowSinglePage(UIPageType.WIN);
    }


}// CLASS
