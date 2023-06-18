using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class Car : MonoBehaviour
{

    public float defaultSpeed = 3;
    float speed;

    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    public SpriteRenderer carRenderer;
    public SpriteRenderer indicatorRenderer;

    public Sprite indiKiri;
    public Sprite indiKanan;
    public Sprite indiLurus;


    public GameObject kecelakaanExplosion;


    public Vector3Int direction = Vector3Int.left;
    Vector3Int targetCell;

    GameManager gm;
    Tilemap tilemap;
    Rigidbody2D rb;

    bool balikKanan = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gm = FindObjectOfType<GameManager>();
        tilemap = GameObject.FindWithTag("RoadTilemap").GetComponent<Tilemap>();
        speed = defaultSpeed;

        // snap position
        rb.position = tilemap.CellToWorld(tilemap.WorldToCell(rb.position));

        SetDirection(direction);
        targetCell = tilemap.WorldToCell(rb.position) + direction;

    }


    void Update()
    {
        if (tilemap == null)
        {
            tilemap = GameObject.FindWithTag("RoadTilemap").GetComponent<Tilemap>();
        }

        // MOVE MANAGER

        if (gm.isSimulating)
        {
            if (Vector3.Distance(rb.position, tilemap.CellToWorld(targetCell)) != 0)
            {
                // lanjut jalan
                transform.position = Vector3.MoveTowards(rb.position, tilemap.CellToWorld(targetCell), speed * Time.deltaTime);

                // cek if targetCellnya tidak ada road
                if (!tilemap.HasTile(targetCell) && Vector3.Distance(rb.position, tilemap.CellToWorld(targetCell)) < 0.3f)
                {
                    Kecelakaan();
                }
            }
            else
            {

                Vector3Int kirinya = new Vector3Int(-direction.y, direction.x, direction.z);
                Vector3Int kanannya = new Vector3Int(direction.y, -direction.x, direction.z);
                Vector3Int bawahnya = new Vector3Int(-direction.x, -direction.y, direction.z);

                if (balikKanan)
                {
                    direction = bawahnya;
                    balikKanan = false;
                }
                else
                {
                    // cek apakah ada percabangan jalan
                    int cabangCount = 0;

                    if (tilemap.HasTile(targetCell + Vector3Int.up))
                    {
                        cabangCount++;
                    }
                    if (tilemap.HasTile(targetCell + Vector3Int.down))
                    {
                        cabangCount++;
                    }
                    if (tilemap.HasTile(targetCell + Vector3Int.right))
                    {
                        cabangCount++;
                    }
                    if (tilemap.HasTile(targetCell + Vector3Int.left))
                    {
                        cabangCount++;
                    }

                    if (cabangCount == 1)
                    {

                    }
                    else if (cabangCount > 2 && indicatorRenderer.sprite != null)
                    {
                        // jika bertemu cabang dan punya indikator

                        if (indicatorRenderer.sprite == indiKiri)
                        {
                            direction = kirinya;
                        }
                        else if (indicatorRenderer.sprite == indiKanan)
                        {
                            direction = kanannya;
                        }
                        else if (indicatorRenderer.sprite == indiLurus)
                        {

                        }
                        indicatorRenderer.sprite = null;
                    }
                    else
                    {
                        // normal check
                        if (tilemap.HasTile(targetCell + direction))
                        {

                        }
                        else if (tilemap.HasTile(targetCell + kirinya))
                        {
                            direction = kirinya;
                        }
                        else if (tilemap.HasTile(targetCell + kanannya))
                        {
                            direction = kanannya;
                        }
                        else
                        {
                            direction = bawahnya;
                        }


                    }
                }


                targetCell = targetCell + direction;
                UpdateSpriteRotation();

            }

            
        }

        // SCALE MANAGER
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool isHovered = GetComponent<Collider2D>().OverlapPoint(point);

        if (isHovered && gm.GetEditType() == 3)
        {
            carRenderer.transform.localScale = Vector3.one * 1.3f;

            if (Input.GetMouseButtonDown(0))
            {
                RotateCar();
            }
        }
        else
        {
            carRenderer.transform.localScale = Vector3.one;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("RambuKiri"))
        {
            indicatorRenderer.sprite = indiKiri;

        } else if (collision.CompareTag("RambuKanan"))
        {
            indicatorRenderer.sprite = indiKanan;

        } else if (collision.CompareTag("RambuLurus"))
        {
            indicatorRenderer.sprite = indiLurus;

        }
        else if (collision.CompareTag("Rambu30"))
        {
            speed = 0.33f;

        }
        else if (collision.CompareTag("Rambu60"))
        {
            speed = 0.67f;

        }
        else if (collision.CompareTag("Rambu90"))
        {
            speed = 1;

        }
        else if (collision.CompareTag("RambuAkhir"))
        {
            speed = defaultSpeed;

        } else if (collision.CompareTag("RambuVerboden") && gm.isSimulating)
        {
            balikKanan = true;



        } else if (collision.CompareTag("Transport"))
        {
            Kecelakaan();
        }


    }
    void UpdateSpriteRotation()
    {
        // update car rotation sprite
        if (direction == Vector3Int.up)
        {
            carRenderer.sprite = upSprite;
        }
        else if (direction == Vector3Int.down)
        {
            carRenderer.sprite = downSprite;
        }
        else if (direction == Vector3Int.left)
        {
            carRenderer.sprite = leftSprite;
        }
        else if (direction == Vector3Int.right)
        {
            carRenderer.sprite = rightSprite;
        }
    }

    void RotateCar()
    {
        SetDirection(new Vector3Int(direction.y, -direction.x, direction.z));
    }
    public Vector3Int GetDirection()
    {
        return direction;
    }
    public void SetDirection(Vector3Int dir)
    {
        direction = dir;
        targetCell = tilemap.WorldToCell(rb.position) + direction;
        UpdateSpriteRotation();
    }
    void Kecelakaan()
    {
        if (GameObject.FindGameObjectWithTag("Explosion") == null)
        {
            Instantiate(kecelakaanExplosion, transform.position + Vector3.up * 0.2f, Quaternion.identity);
        }
        
        gm.StopSimulation();
    }
    
}
