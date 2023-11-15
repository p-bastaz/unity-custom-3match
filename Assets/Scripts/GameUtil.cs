using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameUtil
{
    public static Direction Get6Direction(Vector2 Begin, Vector2 End)
    {
        Vector3 direction = End - Begin;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;

        if (angle >= 315 || angle <= 45) return Direction.RIGHT;
        else if (angle >= 45 && angle <= 135) return Direction.UP;
        else if (angle >= 135 && angle <= 225) return Direction.LEFT;
        else if (angle >= 225 && angle <= 315) return Direction.DOWN;
        else return Direction.NONE;
    }

    public static Vector2Int GetNeighbor(Vector2Int origin, Direction dir)
    {
        switch (dir)
        {
            case Direction.LEFT:
                return new Vector2Int(origin.x - 1, origin.y);
            case Direction.UP:
                return new Vector2Int(origin.x, origin.y + 1);
            case Direction.RIGHT:
                return new Vector2Int(origin.x + 1, origin.y);
            case Direction.DOWN:
                return new Vector2Int(origin.x, origin.y - 1);

        }
        return Vector2Int.zero;
    }

    public static IEnumerable<Vector2Int> GetNeighborAll(Vector2Int origin)
    {
        return Data.Neighbor_Directions.Select(x => GetNeighbor(origin, x));
    }

    /// <summary>
    /// MatchInfo List 중복 제거
    /// </summary>
    public static List<MatchInfo> Distinct(List<MatchInfo> result)
    {
        return result.Distinct(new ListOfMatchInfoComparer()).ToList();
    }

    public static List<Vector2Int> GetCoordsAll(List<MatchInfo> results)
    {
        List<Vector2Int> coordsAll = new List<Vector2Int>();
        
        for(int i = 0; i < results.Count; i++)
        {
            for(int j = 0; j < results[i].coords.Count; j++)
            {
                if (!coordsAll.Exists(r => r.Equals(results[i].coords[j])))
                    coordsAll.Add(results[i].coords[j]);
            }
        }

        return coordsAll;
    }
}

public class ListOfMatchInfoComparer : IEqualityComparer<MatchInfo>
{
    public bool Equals(MatchInfo a, MatchInfo b)
    {
        return
            a.coords.SequenceEqual(b.coords);
    }

    public int GetHashCode(MatchInfo l)
    {
        unchecked
        {
            int hash = 0;
            foreach (var it in l.coords)
            {
                hash += (it.x * it.x) + (it.y * it.y) * 31;
            }
            return hash;
        }
    }
}
