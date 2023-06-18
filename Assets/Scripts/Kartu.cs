using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class Kartu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject rambuObject;
    public Tile decorTile;

    public bool editMode = true; // creative mode

    public int buttonType;

    // 0 -> rambu & kendaraan
    // 1 -> place road
    // 2 -> erase road
    // 3 -> rotate car
    // 4 -> hand tool
    // 5 -> paint decor
    // 6 -> simulate

    public Sprite notSelectedSprite;
    public Sprite selectedSprite;

    RectTransform rt;
    Image img;
    GameManager gm;

    

    void Start()
    {
        rt = GetComponent<RectTransform>();
        img = GetComponent<Image>();
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // onSelected effect
        if (editMode)
        {
            img.color = Color.white;

            if ((gm.rambu == rambuObject && buttonType == 0 && gm.GetEditType() == 0)
                || (buttonType == 1 && gm.GetEditType() == 1)
                || (buttonType == 2 && gm.GetEditType() == 2)
                || (buttonType == 3 && gm.GetEditType() == 3)
                || (buttonType == 4 && gm.GetEditType() == 4)
                || (buttonType == 5 && gm.GetEditType() == 5 && gm.decorTile == decorTile)
                || (buttonType == 6 && gm.isSimulating)
                )
            {
                img.sprite = selectedSprite;
            } else
            {
                img.sprite = notSelectedSprite;
            }


        } else if (gm.rambu == rambuObject && gm.GetEditType() == 0)
        {
            img.color = Color.yellow;
        }
        else
        {
            img.color = Color.white;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        // hover effect
        if (editMode)
        {

        }
        else
        {
            rt.position = new Vector3(rt.position.x, 0, 0);
        }
       

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        // hover effect
        if (editMode)
        {

        }
        else
        {
            rt.position = new Vector3(rt.position.x, -24, 0);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (buttonType == 0)
        {
            gm.PlaceRambu();
            gm.rambu = rambuObject;
        }
        else if (buttonType == 1)
        {
            gm.PaintRoad();
        }
        else if (buttonType == 2)
        {
            gm.EraseRoad();
        }
        else if (buttonType == 3)
        {
            gm.RotateCar();
        }
        else if (buttonType == 4)
        {
            gm.HandTool();
        }
        else if (buttonType == 5)
        {
            gm.PlaceDecor();
            gm.decorTile = decorTile;
        }
        else if (buttonType == 6)
        {
            gm.ToggleSimulation();
        }
        
    }
}
