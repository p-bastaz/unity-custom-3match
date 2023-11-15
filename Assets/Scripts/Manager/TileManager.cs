using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    private static TileManager Instance;
    public static TileManager Ins
    {
        get
        {
            return Instance;
        }
    }

    public List<Tile> tile_List = new List<Tile>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

    }

    // 타일 매니저 초기화
    public void Init()
    {
        Tile customTile;
        List<Tile> customTile_List = CustomManager.Ins.customTile_List;

        Tile tile;

        tile_List.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            tile = transform.GetChild(i).GetComponent<Tile>();

            customTile = customTile_List.SingleOrDefault(r => r.coord_X == tile.coord_X && r.coord_Y == tile.coord_Y);

            if (customTile != null)
            {
                if (customTile._isActive)
                {
                    tile.gameObject.SetActive(true);
                    tile_List.Add(tile);
                }
                else
                    tile.gameObject.SetActive(false);
            }
            else
                tile.gameObject.SetActive(false);
        }
    }

    //타일 활성화 체크
    public bool IsEnable(Vector2Int coords)
    {
        return tile_List.SingleOrDefault(r => r.coord_X == coords.x && r.coord_Y == coords.y) != null;
    }
}
