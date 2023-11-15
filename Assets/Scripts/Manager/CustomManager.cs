using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomManager : MonoBehaviour
{
    private static CustomManager Instance;
    public static CustomManager Ins
    {
        get
        {
            return Instance;
        }
    }

    public List<Tile> customTile_List = new List<Tile>();

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

        Tile tile;

        for (int i = 0; i < transform.childCount; i++)
        {
            tile = transform.GetChild(i).GetComponent<Tile>();
            customTile_List.Add(tile);
        }
    }

    public void Init()
    {
        Tile tile;

        for(int i = 0; i < customTile_List.Count; i++)
        {
            tile = customTile_List[i];

            tile.GetComponent<SpriteRenderer>().color = tile._isActive ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 127);
        }

        gameObject.SetActive(true);
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0) && gameObject.activeInHierarchy)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            Tile tile;

            if (hit.collider != null && hit.collider.gameObject.GetComponent<Tile>() != null)
            {
                SoundManager.Ins.PlaySound(SoundManager.click_Sound);

                tile = hit.collider.gameObject.GetComponent<Tile>();

                tile._isActive = !tile._isActive;

                tile.GetComponent<SpriteRenderer>().color = tile._isActive ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 127);
            }
        }
    }
}
