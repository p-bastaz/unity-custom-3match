using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchManager : MonoBehaviour
{

    private static MatchManager Instance;
    public static MatchManager Ins
    {
        get
        {
            return Instance;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    public List<MatchInfo> BlockCheck(Block _block)
    {
        if (_block.type == BlockType.Block) return new List<MatchInfo>();

        List<MatchInfo> matchInfos = CheckStraightAll(_block);
        
        return matchInfos;
    }

    public List<MatchInfo> BlockCheckAll()
    {
        var result = new List<MatchInfo>();

        for(int i = 0; i < BlockManager.Ins.block_List.Count; i++)
        {
            List<MatchInfo> block_Result = BlockCheck(BlockManager.Ins.block_List[i]);

            if (block_Result != null)
            {
                result = result.Union(block_Result).ToList();
            }
        }

        result = GameUtil.Distinct(result);
        return result;
    }

    //���� ��� �˻�
    private List<MatchInfo> CheckStraightAll(Block _block)
    {
        var matchInfos = new List<MatchInfo>();
        BlockType originType = _block.type;

        // ����
        MatchDirection matchDir = MatchDirection.Vertical;
        MatchInfo matchInfo = CheckStraight(_block, matchDir);
        if (matchInfo != null)
        {
            matchInfos.Add(matchInfo);
        }

        // ����
        matchDir = MatchDirection.horizontal;
        matchInfo = CheckStraight(_block, matchDir);
        if (matchInfo != null)
        {
            matchInfos.Add(matchInfo);
        }


        return matchInfos;
    }


    //��Ī �˻� ���� (����, ������)
    private MatchInfo CheckStraight(Block block, MatchDirection matchDir)
    {
        var originType = block.type;
        var firstBlock = GetFirstBlock(block, matchDir);

        Direction dir = Direction.NONE;

        if (matchDir == MatchDirection.Vertical)
        {
            dir = Direction.UP;
        }
        else if (matchDir == MatchDirection.horizontal)
        {
            dir = Direction.RIGHT;
        }


        var result = CheckMatch(firstBlock, dir);
        if (result.Count == 0) return null;
        var matchInfo = new MatchInfo(originType, MatchType.StraightLine, matchDir, result);
        return matchInfo;
    }



    // 3�� �̻� ��Ī�Ǵ��� Ȯ��
    private List<Vector2Int> CheckMatch(Block block, Direction dir)
    {
        if ((int)dir > 4) throw new System.Exception("�Ķ������ ��ȿ������ �ƴմϴ�.");
        var result = new List<Vector2Int>();
        BlockType originType = block.type;
        int match = 1;
        var nextBlock = block;
        result.Add(new Vector2Int(block.coord_X, block.coord_Y));
        while (true)
        {
            nextBlock = BlockManager.Ins.GetNeighbor(nextBlock, dir);
            if (nextBlock == null || nextBlock.type != originType)
            {
                break;
            }
            match++;
            result.Add(new Vector2Int(nextBlock.coord_X, nextBlock.coord_Y));
        }
        bool isMatch = (3 <= match);
        if (!isMatch)
        {
            result.Clear();
        }
        return result;
    }


    //��Ī �˻縦 ������ ��� �������� (����, ����)
    private Block GetFirstBlock(Block block, MatchDirection matchDir)
    {
        Block firstBlock = block;
        Block nextBlock = block;

        Direction dir = Direction.NONE;
        if (matchDir == MatchDirection.Vertical)
        {
            dir = Direction.DOWN;
        }
        else if (matchDir == MatchDirection.horizontal)
        {
            dir = Direction.LEFT;
        }


        while (true)
        {
            nextBlock = BlockManager.Ins.GetNeighbor(nextBlock, dir);
            if (nextBlock == null || nextBlock.type != block.type)
            {
                break;
            }
            firstBlock = nextBlock;
        }
        return firstBlock;
    }



}
