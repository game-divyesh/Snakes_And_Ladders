using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x, y;
    public TileType type;

    [Space(10)]
    [SerializeField] private Renderer renderer;
    [SerializeField] private TextMeshProUGUI tileCountText;

    public void SetTileData(int _X, int _Y, TileType _Type, int tileCount)
    {
        x = _X;
        y = _Y;
        type = _Type;
        tileCountText.text = tileCount.ToString();

        transform.name = $"Tile_{_Y}_{_X}";
    }

    public void ResetTileType() => type = TileType.Normal;

    public void ApplyMaterial(Material _Mat) => renderer.material = _Mat;
    
}// CLASS

public enum TileType
{
    Normal,
    LadderStart,
    LadderEnd,
    SnakeHead,
    SnakeTail
}
