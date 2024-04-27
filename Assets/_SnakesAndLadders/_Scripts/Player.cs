using DG.Tweening;
using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public delegate void OnPlayerStopMoving();
    public static OnPlayerStopMoving onPlayerStopMoving;
    //public static event Action OnPlayerStopMoving;

    public bool isReachEnd = false;
    [Space]
    public int currentPos = 0;
    public int previousPos = 0;
    public bool isTurnAgain = false;
    [Space(5)]
    [Header("Skip Turn")]
    public bool isSkipUsed = false;
    public bool isSkipTurn = false;
    public int previousDiceNum = -1;

    [HideInInspector]
    public Vector3 currentScale = Vector3.zero;

    private Tween scaleUpDownTween = null;


    public void MovePlayer(int moveStep)
    {
        //Debug.Log($"<color=white><b>OnDiceStopped call -> {Time.realtimeSinceStartup}</b></color>");
        StopScaleUpTween();

        Vector3 scale = new Vector3(0.4f, 1, 0.4f);
        transform.localScale = scale;

        // Apply previous Turn Count
        if (isSkipUsed && isSkipTurn)
            moveStep += previousDiceNum;



        if (GameManager.Instance.skipAnimation)
        {
            if (currentPos + moveStep < BoardGenerator.Instance.tiles.Count)
            {
                Vector3 pos = BoardGenerator.Instance.tiles[currentPos + moveStep].transform.position;
                pos.y = 0;
                transform.position = pos;

                previousPos = currentPos;
                currentPos += moveStep;
                // Check Is Reach End Tile
                if (currentPos == BoardGenerator.Instance.tiles.Count - 1)
                {
                    isReachEnd = true;
                    GameManager.Instance.winnerPlayers.Add(this as Player);
                    GameManager.Instance.CheckGameComplete();
                }

                CheckIsSnakeOrLadder();
            }
            else
                onPlayerStopMoving?.Invoke();
        }
        else
        {
            if (currentPos + moveStep < BoardGenerator.Instance.tiles.Count)
                JumpOnNextTile(moveStep);
            else
                onPlayerStopMoving?.Invoke();
        }

        // Set Is Skip Used
        if (isSkipUsed && isSkipTurn)
        {
            isSkipTurn = false;
            previousDiceNum = -1;
        }
    }

    private void JumpOnNextTile(int targetPos)
    {
        Sequence jumpSeq = DOTween.Sequence();
        for (int index = currentPos + 1; index < currentPos + targetPos + 1; index++)
        {
            Vector3 jumpPos = BoardGenerator.Instance.tiles[index].transform.position;
            jumpPos.y = 0;
            jumpSeq.Append(transform.DOJump(jumpPos, 1.5f, 1, 0.5f)
                .OnComplete(() => { AudioManager.Instance.PlayAudioClip(ClipName.TokenMove); }));
        }
        jumpSeq.Play().OnComplete(() =>
        {
            previousPos = currentPos;
            currentPos += targetPos;
            // Check Is Reach End Tile
            if (currentPos == BoardGenerator.Instance.tiles.Count - 1)
            {
                isReachEnd = true;
                GameManager.Instance.winnerPlayers.Add(this as Player);
                GameManager.Instance.CheckGameComplete();
            }

            if (!isTurnAgain)
                CheckIsSnakeOrLadder();
            else
                onPlayerStopMoving?.Invoke();
        });
    }

    private void CheckIsSnakeOrLadder()
    {
        Tile currentTile = BoardGenerator.Instance.tiles[currentPos];
        //Debug.Log(currentTile.name);
        Tile destinationTile = null;

        if (currentTile.type == TileType.LadderStart)
        {
            destinationTile = BoardGenerator.Instance.GetEndTile(currentTile, BoardGenerator.Instance.ladderTiles);
            AudioManager.Instance.PlayAudioClip(ClipName.Ladder);
        }

        else if (currentTile.type == TileType.SnakeHead)
        {
            destinationTile = BoardGenerator.Instance.GetEndTile(currentTile, BoardGenerator.Instance.snakeTiles);
            AudioManager.Instance.PlayAudioClip(ClipName.Snake);
        }

        if (destinationTile is not null)
        {
            Vector3 pos = destinationTile.transform.position;
            pos.y = 0;
            transform.DOMove(pos, 0.5f).OnComplete(() =>
            {
                currentPos = BoardGenerator.Instance.tiles.IndexOf(destinationTile);
                onPlayerStopMoving?.Invoke();

            });
        }
        else
        {
            onPlayerStopMoving?.Invoke();
        }
    }

    public void TurnIndicateAnim()
    {
        Vector3 defaultScale = transform.localScale;
        scaleUpDownTween = transform.DOScale(defaultScale * 1.5f, 0.75f)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void StopScaleUpTween()
    {
        if (scaleUpDownTween is not null)
            scaleUpDownTween.Kill();
        transform.localScale = currentScale;
    }
}// CLASS

public enum PlayerCount
{
    TwoPlayer = 2,
    ThreePlayer = 3,
    FourPlayer = 4,
}
