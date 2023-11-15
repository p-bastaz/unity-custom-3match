using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager Instance;
    public static GameManager Ins
    {
        get
        {
            return Instance;
        }
    }

    [SerializeField] private Block current_Block;
    [SerializeField] private List<MatchInfo> curMatchInfos = new List<MatchInfo>();
    [SerializeField] private bool isLock = false;

    public int blockTypeCount;

    [Header("Manager")]
    public CustomManager customManager;

    public TileManager tileManager;
    public BlockManager blockManager;


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

    private void Start()
    {
        customManager.gameObject.SetActive(false);
        tileManager.gameObject.SetActive(false);
        blockManager.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isLock) return;
        if (current_Block != null) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);


            if (hit.collider != null && hit.collider.gameObject.GetComponent<Block>() != null)
            {
                current_Block = hit.collider.gameObject.GetComponent<Block>();
                StartCoroutine(CoWaitDrag(current_Block.transform.position));
            }
        }
    }

    IEnumerator CoWaitDrag(Vector3 initPos)
    {
        isLock = true;
        while (true)
        {
            if (Input.GetMouseButtonUp(0))
            {
                break;
            }
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float distance = Vector2.Distance(initPos, mousePos);

            if (distance > Data.DRAG_DISTANCE)
            {
                Debug.LogError("Drag!");


                Direction dir = GameUtil.Get6Direction(initPos, mousePos);
                Block changeBlock = BlockManager.Ins.GetNeighbor(current_Block, dir);
                if (changeBlock == null) break;

                yield return StartCoroutine(BlockManager.Ins.CoSwapBlock(current_Block, changeBlock));
                List<MatchInfo> selectInfos = MatchManager.Ins.BlockCheck(current_Block);
                List<MatchInfo> targetInfos = MatchManager.Ins.BlockCheck(changeBlock);
                curMatchInfos = selectInfos.Union(targetInfos).ToList();

                //실패로 인해 위치 롤백
                if (curMatchInfos.Count == 0)
                {
                    yield return StartCoroutine(BlockManager.Ins.CoRollbackSwap());
                    break;
                }

                

                //블럭 파괴 후 재배치
                while (true)
                {
                    if (curMatchInfos.Count == 0) break;
                    SoundManager.Ins.PlaySound(SoundManager.match_Sound);
                    yield return StartCoroutine(BlockManager.Ins.DeleteBlocks(GameUtil.GetCoordsAll(curMatchInfos)));
                    yield return StartCoroutine(BlockManager.Ins.CoRefillBlocks());
                    curMatchInfos = MatchManager.Ins.BlockCheckAll();
                }
                break;
            }
           

            yield return null;

        }


        //if (BlockManager.instance.totalTopCount == 0)
        //{
        //    Debug.Log("Win!");
        //}

        //초기화
        current_Block = null;
        isLock = false;
    }

    
    public void Back()
    {
        SoundManager.Ins.PlaySound(SoundManager.back_Sound);

        UIManager.Ins.score_UI.gameObject.SetActive(false);
        UIManager.Ins.blockTypeCount_UI.gameObject.SetActive(false);

        UIManager.Ins.backButton.gameObject.SetActive(false);
        UIManager.Ins.customButton.gameObject.SetActive(true);
        UIManager.Ins.playButton.gameObject.SetActive(true);

        customManager.gameObject.SetActive(false);
        tileManager.gameObject.SetActive(false);
        blockManager.gameObject.SetActive(false);
    }

    public void SettingCustom()
    {
        SoundManager.Ins.PlaySound(SoundManager.click_Sound);

        UIManager.Ins.blockTypeCount_UI.gameObject.SetActive(true);
        UIManager.Ins.blockTypeCount_Text.text = (blockTypeCount - 1).ToString();

        UIManager.Ins.backButton.gameObject.SetActive(true);
        UIManager.Ins.customButton.gameObject.SetActive(false);
        UIManager.Ins.playButton.gameObject.SetActive(false);

        customManager.gameObject.SetActive(true);
        customManager.Init();
    }

    public void SettingPlay()
    {
        SoundManager.Ins.PlaySound(SoundManager.click_Sound);

        UIManager.Ins.score = 0;
        UIManager.Ins.UpdateScore(UIManager.Ins.score);
        UIManager.Ins.score_UI.gameObject.SetActive(true);

        UIManager.Ins.backButton.gameObject.SetActive(true);
        UIManager.Ins.customButton.gameObject.SetActive(false);
        UIManager.Ins.playButton.gameObject.SetActive(false);

        tileManager.gameObject.SetActive(true);
        tileManager.Init();
        blockManager.gameObject.SetActive(true);
        blockManager.Init();
    }
}
