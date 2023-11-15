using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int coord_X;
    public int coord_Y;

    public bool _isActive;

    private void Awake()
    {
        coord_X = (int)transform.localPosition.x;
        coord_Y = (int)transform.localPosition.y;
    }

    public bool isEmpty()
    {
        return !BlockManager.Ins.block_List.Exists(r => r.coord_X == coord_X && r.coord_Y == coord_Y);
    }


    public Block FindReFillBlock()
    {
        Vector2Int nextCoords = new Vector2Int(coord_X, coord_Y);
        Vector2Int maxUpCoords = new Vector2Int(coord_X, coord_Y);
        
        // À§ÂÊ
        while (true)
        {
            nextCoords = GameUtil.GetNeighbor(nextCoords, Direction.UP);
            if (!TileManager.Ins.IsEnable(nextCoords))
            {
                maxUpCoords = GameUtil.GetNeighbor(nextCoords, Direction.DOWN);
                break;
            }
            Block nextBlock = BlockManager.Ins.GetBlock(nextCoords.x, nextCoords.y);
            if (nextBlock != null)
            {
                return nextBlock;
            }
        }
                                                
        return null;
    }


}
