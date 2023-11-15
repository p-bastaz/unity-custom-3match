using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    public static int maxTopSpin = 10;
    public static readonly Direction[] Neighbor_Directions = new Direction[4] { Direction.UP, Direction.RIGHT, Direction.DOWN, Direction.RIGHT};

    public const float DRAG_DISTANCE = 0.4f;
}

public enum BlockType
{
    None = 0,
    Blue = 1,
    Green = 2,
    Grey = 3,
    Purple = 4,
    Red = 5,
    Yellow = 6,
    Block = 7,
}

public enum Direction
{
    NONE = 0,
    LEFT = 1,
    DOWN,
    UP,
    RIGHT,
}


public enum MatchType
{
    None = 0,
    StraightLine = 1,
}
public enum MatchDirection
{
    None = 0,
    Vertical = 1,
    horizontal = 2
}

[System.Serializable]
public class MatchInfo
{
    public BlockType blockType;
    public MatchType matchType;
    public MatchDirection matchDir;
    public List<Vector2Int> coords = new List<Vector2Int>();

    public MatchInfo(BlockType _blockType, MatchType _matchType, MatchDirection _matchDir, List<Vector2Int> _coords)
    {
        blockType = _blockType;
        matchType = _matchType;
        matchDir = _matchDir;
        coords = _coords;
    }
}
