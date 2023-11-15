using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class Block : MonoBehaviour
{
    public int coord_X;
    public int coord_Y;

    public BlockType type;

    public SpriteRenderer block_Sprite;
    [SerializeField] SpriteAtlas block_Atlas;

    public bool isBroke = false;
    

    //블럭 초기화
    public void Init(int type_Num, int coord_x, int coord_y)
    {
        switch(type_Num)
        {
            case (int)BlockType.Blue:
                type = BlockType.Blue;
                block_Sprite.sprite = block_Atlas.GetSprite("element_blue_diamond");
                break;
            case (int)BlockType.Green:
                type = BlockType.Green;
                block_Sprite.sprite = block_Atlas.GetSprite("element_green_diamond");
                break;
            case (int)BlockType.Grey:
                type = BlockType.Grey;
                block_Sprite.sprite = block_Atlas.GetSprite("element_grey_diamond");
                break;
            case (int)BlockType.Purple:
                type = BlockType.Purple;
                block_Sprite.sprite = block_Atlas.GetSprite("element_purple_diamond");
                break;
            case (int)BlockType.Red:
                type = BlockType.Red;
                block_Sprite.sprite = block_Atlas.GetSprite("element_red_diamond");
                break;
            case (int)BlockType.Yellow:
                type = BlockType.Yellow;
                block_Sprite.sprite = block_Atlas.GetSprite("element_yellow_diamond");
                break;
            case (int)BlockType.Block:
                type = BlockType.Block;
                block_Sprite.sprite = block_Atlas.GetSprite("tile_grey");
                break;
        }

        coord_X = coord_x;
        coord_Y = coord_y;
    }


    //블럭 위치 변경 (DoTween 활용)
    public void MoveChangeCoords(int _coord_x, int _coord_y, float speed)
    {
        Tile tile = TileManager.Ins.tile_List.SingleOrDefault(r => r.coord_X == _coord_x && r.coord_Y == _coord_y);
        Vector2 position = new Vector2(tile.transform.position.x, tile.transform.position.y);
        transform.DOMove(position, speed);
        coord_X = _coord_x;
        coord_Y = _coord_y;
    }

    //장애물 블럭 파괴하기
    public void TryDestroy()
    {
        if (isBroke)
        {
            BlockManager.Ins.destroyTopSpin++;

            UIManager.Ins.score += 500;
            UIManager.Ins.UpdateScore(UIManager.Ins.score);

            BlockManager.Ins.block_List.Remove(this);
            Destroy(gameObject);
        }
        else
            isBroke = true;
    }
}
