using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class BlockManager : MonoBehaviour
{
    private static BlockManager Instance;
    public static BlockManager Ins
    {
        get
        {
            return Instance;
        }
    }

    public int currentTopSpin = 0;
    public int destroyTopSpin = 0;

    public GameObject block_Prefab;
    public List<Block> block_List = new List<Block>();

    private Block[] backUpBlock = new Block[2];

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

    public void Init()
    {
        CreateBlock();

        //��Ī�Ǵ� ����ġ�� �ȳ��ö����� �ʱ�ȭ
        while (true)
        {
            var matchInfo = MatchManager.Ins.BlockCheckAll();
            if (matchInfo.Count == 0) break;
            InitBlocks();
        }

    }

    //Ÿ�Ͽ� �� ����
    public void CreateBlock()
    {
        Block block;
        Tile tile;

        block_List.Clear();

        foreach (Transform child in transform)
            Destroy(child.gameObject);

        for (int i = 0; i < TileManager.Ins.tile_List.Count; i++)
        {
            block = Instantiate(block_Prefab).GetComponent<Block>();
            tile = TileManager.Ins.tile_List[i];

            int random;

            while (true)
            {
                random = Random.Range(1, GameManager.Ins.blockTypeCount);

                if(random == (int)BlockType.Block)
                {
                    if(currentTopSpin < Data.maxTopSpin)
                    {
                        currentTopSpin++;
                        break;
                    }
                }   
                else
                    break;
            }

            block.Init(random, tile.coord_X, tile.coord_Y);
            block.transform.SetParent(transform);
            block.transform.position = new Vector2(tile.transform.position.x, tile.transform.position.y);
            block_List.Add(block);
        }

        InitBlocks();

    }

    public void InitBlocks()
    {
        currentTopSpin = 0;

        for (int i = 0; i < block_List.Count; i++)
        {
            int random;


            while (true)
            {
                random = Random.Range(1, GameManager.Ins.blockTypeCount);
            
                if (random == (int)BlockType.Block)
                {
                    if (currentTopSpin < Data.maxTopSpin)
                    {
                        currentTopSpin++;
                        break;
                    }
                }
                else
                    break;
            }

            block_List[i].Init(random, block_List[i].coord_X, block_List[i].coord_Y);
        }
    }

    // Block ��������
    public Block GetBlock(int _coord_x, int _coord_y)
    {
        return block_List.SingleOrDefault(r => r.coord_X == _coord_x && r.coord_Y == _coord_y);
    }

    // �̿��� �� ��������
    public Block GetNeighbor(Block origin, Direction dir)
    {
        Vector2Int crood = GameUtil.GetNeighbor(new Vector2Int(origin.coord_X, origin.coord_Y), dir);

        return block_List.Find(x => x.coord_X == crood.x && x.coord_Y == crood.y);
    }

    // �� �� ���� (DoTween Ȱ��)
    public IEnumerator CoSwapBlock(Block blockA, Block blockB)
    {
        if (blockA.coord_X == blockB.coord_X && blockA.coord_Y == blockB.coord_Y) throw new System.Exception("���� ��� ���� �Ұ�");
        Debug.Log($"���Ҹ� ({blockA.coord_X} , {blockA.coord_Y}), ({blockB.coord_X} , {blockB.coord_Y})");
        backUpBlock[0] = blockA;
        backUpBlock[1] = blockB;
        //�̵�

        bool MovingA = true;
        bool MovingB = true;

        SoundManager.Ins.PlaySound(SoundManager.swap_Sound);

        Vector2 blockAPos = new Vector2(blockA.transform.position.x, blockA.transform.position.y);
        Vector2 blockBPos = new Vector2(blockB.transform.position.x, blockB.transform.position.y);

        blockA.transform.DOMove(blockBPos, 0.3f).OnComplete(() => MovingA = false);
        blockB.transform.DOMove(blockAPos, 0.3f).OnComplete(() => MovingB = false);

        yield return new WaitUntil(() => !MovingA && !MovingB);


        //���� ��ǥ����
        Vector2Int temp = new Vector2Int(blockA.coord_X, blockA.coord_Y);
        blockA.coord_X = blockB.coord_X;
        blockA.coord_Y = blockB.coord_Y;
        blockB.coord_X = temp.x;
        blockB.coord_Y = temp.y;
    }
    

    //���� �ѹ�
    public IEnumerator CoRollbackSwap()
    {
        if (backUpBlock[0] == null || backUpBlock[1] == null) yield break;
        yield return StartCoroutine(CoSwapBlock(backUpBlock[0], backUpBlock[1]));
        backUpBlock[0] = null;
        backUpBlock[1] = null;
    }

    //�� ����
    public IEnumerator DeleteBlocks(List<Vector2Int> _matchCoords)
    {
        TryDestroyObstacle(_matchCoords);

        List<Block> _delete_block_list = new List<Block>();

        for (int i = 0; i < _matchCoords.Count; i++)
        {
            Vector2Int coords = _matchCoords[i];
            Block targetBlock = block_List.SingleOrDefault(r => r.coord_X == coords.x && r.coord_Y == coords.y);
            if (targetBlock == null) continue;
            _delete_block_list.Add(targetBlock);
        }

        bool isDeleteComplete = false;

        for (int i = 0; i < _delete_block_list.Count; i++)
        {
            if (_delete_block_list[i] != _delete_block_list.Last())
                _delete_block_list[i].block_Sprite.DOFade(0, 0.5f);
            else
                _delete_block_list[i].block_Sprite.DOFade(0, 0.5f).OnComplete(() => isDeleteComplete = true);
        }

        yield return new WaitUntil(() => isDeleteComplete);

        for (int i = 0; i < _delete_block_list.Count; i++)
        {
            UIManager.Ins.score += 100;
            block_List.Remove(_delete_block_list[i]);
            Destroy(_delete_block_list[i].gameObject);
        }

        UIManager.Ins.UpdateScore(UIManager.Ins.score);
    }

    // �� ä���ֱ�
    public IEnumerator CoRefillBlocks()
    {
        while (true)
        {  
            var emptyBoards = TileManager.Ins.tile_List.FindAll(x => x.isEmpty());
            if (emptyBoards.Count == 0) break;
            bool isFillExists = false;
        
            foreach (var emptyBoard in emptyBoards)
            {
                Block fillBlock = emptyBoard.FindReFillBlock();
                if (fillBlock != null)
                {
                    isFillExists = true;
                    fillBlock.MoveChangeCoords(emptyBoard.coord_X, emptyBoard.coord_Y, 0.8f);
                }
            }
            if (!isFillExists)
            {
                break;
            }
        }
        while (true)
        {
            if (block_List.Count == TileManager.Ins.tile_List.Count) break;
            Vector2Int emptyCoords = FindEmpty();
            Block spawnBlock = CreateNewRandomBlock(emptyCoords);
            spawnBlock.MoveChangeCoords(emptyCoords.x, emptyCoords.y, 0.8f);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);
    }
    
    // �� �ı� �� ���ο� �� ����
    private Block CreateNewRandomBlock(Vector2Int _emptyCoords)
    {
        //Vector2Int spawnCoords = new Vector2Int(_emptyCoords.x, TileManager.Ins.tile_List.Max(r => r.coord_Y));
        Vector2Int spawnCoords = new Vector2Int(_emptyCoords.x, _emptyCoords.y);

        int coord_y = spawnCoords.y;
        Tile spawnTile;

        while (true)
        {
            coord_y++;

            spawnTile = TileManager.Ins.tile_List.SingleOrDefault(r => r.coord_X == spawnCoords.x && r.coord_Y == coord_y);

            if(spawnTile == null)
            {
                coord_y--;
                spawnCoords.y = coord_y;
                break;
            }
        }

        if (block_List.Exists(x => x.coord_X == spawnCoords.x && x.coord_Y == spawnCoords.y)) throw new System.Exception("���� ��ġ�� ����� ����");

        Block block = Instantiate(block_Prefab).GetComponent<Block>();

        int random;

        while (true)
        {
            random = Random.Range(1, GameManager.Ins.blockTypeCount);

            if (random == (int)BlockType.Block)
            {
                if (currentTopSpin < Data.maxTopSpin)
                {
                    currentTopSpin++;
                    break;
                }
            }
            else
                break;
        }

        block.Init(random, spawnCoords.x, spawnCoords.y);
        block.transform.SetParent(transform, false);

        Tile tile = TileManager.Ins.tile_List.SingleOrDefault(r => r.coord_X == spawnCoords.x && r.coord_Y == spawnCoords.y);
        Vector3 position = new Vector3(tile.transform.position.x, tile.transform.position.y, 0);

        block.transform.position = position + new Vector3(0f, 0.5f, 0f);
        block.transform.localScale = Vector3.one;
        block.transform.DOMove(position, 0.8f);
        block_List.Add(block);
        return block;
    }

    //��� �ִ� Ÿ�� ã��
    private Vector2Int FindEmpty()
    {
        List<Tile> noBlockTileList = TileManager.Ins.tile_List.Where(r => GetBlock(r.coord_X, r.coord_Y) == null).ToList();

        List<Vector2Int> coordList = new List<Vector2Int>();

        foreach (Tile tile in noBlockTileList)
            coordList.Add(new Vector2Int(tile.coord_X, tile.coord_Y));


        coordList.Sort((c1, c2) => 
        (10 * c1.y + (Mathf.Abs(TileManager.Ins.tile_List.Max(r => r.coord_X) / 2) - c1.x)) - (10 * c2.y + (Mathf.Abs(TileManager.Ins.tile_List.Max(r => r.coord_X) / 2) - c2.x)));
        return coordList[0];
    }

    //��ֹ� �ı� �õ�
    private void TryDestroyObstacle(List<Vector2Int> targetCoords)
    {
        List<Block> topSpins = new List<Block>();
        foreach (var targetCoord in targetCoords)
        {
            foreach (var coords in GameUtil.GetNeighborAll(targetCoord))
            {
                var block = GetBlock(coords.x, coords.y);
                if (block != null && block.type == BlockType.Block)
                {
                    Block topspin = block;
                    if (!topSpins.Exists(x => x == topspin))
                    {
                        topSpins.Add(topspin);
                    }
                }
            }
        }
        foreach (var obstacle in topSpins)
        {
            obstacle.TryDestroy();
        }
    }

}
