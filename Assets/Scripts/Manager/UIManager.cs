using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager Instance;
    public static UIManager Ins
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
        }
        else
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    [Header("score")]
    public int score;
    public GameObject score_UI;
    [SerializeField] TextMeshProUGUI score_Text;

    [Header("BlockTypeCount")]
    public GameObject blockTypeCount_UI;
    public TextMeshProUGUI blockTypeCount_Text;
    public GameObject plusButton;
    public GameObject minusButton;

    [Header("Buttons")]
    public GameObject backButton;
    public GameObject customButton;
    public GameObject playButton;
    

    private void Start()
    {
        score_Text.text = "0";
    }


    public void UpdateScore(int _score)
    {
        score_Text.text = _score.ToString();
    }

    public void ClickPlusMinusButton(GameObject go)
    {
        SoundManager.Ins.PlaySound(SoundManager.click_Sound);

        int count = GameManager.Ins.blockTypeCount;

        if (go == plusButton)
        {
            int max = System.Enum.GetValues(typeof(BlockType)).Length - 1;

            if (count < max) count++;

        }
        else
        {
            if (count > 5) count--;
        }

        GameManager.Ins.blockTypeCount = count;
        blockTypeCount_Text.text = (count - 1).ToString();
    }
}
