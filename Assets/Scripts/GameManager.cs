using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public class TransportMemory
    {
        public GameObject prefab;
        public Vector3 pos;
        public Vector3Int direction;

    }

    public LayerMask blockerMask;
    Tilemap selectedTileTilemap;
    Tilemap decorTilemap;
    Tilemap roadTilemap;

    public Tile selectedTile;
    public RuleTile roadTile;

    public GameObject rambu; // rambu default -> bisa untuk rambu & transportasi
    [SerializeField]
    public Tile decorTile;

    public GameObject mobil;
    public GameObject motor;
    public GameObject truk;

    public float handToolSensitivity = 0.005f;
    public bool eraseRambuOnly;


    public bool isSimulating;
    //public GameObject transportations;



    public GameObject lantaiHighlight;
    public GameObject kendaraanHighlight;
    public GameObject rambuHighlight;

    public GameObject[] levelTemplates;


    int editType = 0;
    // 0 -> place rambu (mode main, but no car)
    // 1 -> paint road
    // 2 -> erase road
    // 3 -> rotate car                          (no selectedTile)
    // 4 -> move tool   (mode main)  (no selectedTile)
    // 5 -> paint decor 

    List<TransportMemory> transportMemories = new List<TransportMemory>();
    GameObject[] levelEditors;

    Vector3 offset;


    void Start()
    {
        selectedTileTilemap = GameObject.FindWithTag("SelectedTileTilemap").GetComponent<Tilemap>();
        Begin(1);

    }
    void Begin(int level)
    {
        StartCoroutine(Beginn(level));
    }

    IEnumerator Beginn(int level)
    {
        if (eraseRambuOnly)
        {
            // destroy existing level
            DestroyAllWithTag("LevelTemplate");

            /*
            DestroyAllWithTag("Transport");
            DestroyAllWithTag("RambuKiri");
            DestroyAllWithTag("RambuKanan");
            DestroyAllWithTag("RambuLurus");
            DestroyAllWithTag("Rambu30");
            DestroyAllWithTag("Rambu60");
            DestroyAllWithTag("Rambu90");
            DestroyAllWithTag("RambuAkhir");
            DestroyAllWithTag("RambuVerboden");*/

            // instantiate new level
            yield return new WaitForEndOfFrame();
            Instantiate(levelTemplates[level - 1]);
        }

        yield return new WaitForEndOfFrame();

        roadTilemap = GameObject.FindWithTag("RoadTilemap").GetComponent<Tilemap>();
        decorTilemap = GameObject.FindWithTag("DecorTilemap").GetComponent<Tilemap>();
        
        levelEditors = GameObject.FindGameObjectsWithTag("LevelEditor");

        editType = 4;
        rambu = null;
        decorTile = null;
        UpdateTab("Rambu");
    }


    void Update()
    {
    if (roadTilemap != null)
    {
            // select tile when mouse over
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int mouseCell = selectedTileTilemap.WorldToCell(mousePos);

            selectedTileTilemap.ClearAllTiles();
            if (editType != 3 && editType != 4 && !isMouseOnUI() &&
                (editType != 0 || roadTilemap.HasTile(mouseCell)) &&
                (editType != 1 || !roadTilemap.HasTile(mouseCell)) &&
                (editType != 2 || roadTilemap.HasTile(mouseCell) || decorTilemap.HasTile(mouseCell))
                )
            {
                selectedTileTilemap.SetTile(mouseCell, selectedTile);
            }




            if (editType == 0)
            {
                // spawn rambu/transportation when clicked

                Collider2D[] cols = Physics2D.OverlapPointAll(roadTilemap.CellToWorld(mouseCell) + Vector3.up * 0.25f);
                bool containsRambu = false;
                bool containsTransport = false;

                for (int i = 0; i < cols.Length; i++)
                {
                    Collider2D c = cols[i];

                    if (IsRambu(c.gameObject))
                    {
                        containsRambu = true;
                    }

                    if (c.CompareTag("Transport"))
                    {
                        containsTransport = true;
                    }
                }

                if (Input.GetMouseButtonDown(0) && roadTilemap.HasTile(mouseCell))
                {
                    if (IsRambu(rambu))
                    {
                        // place rambu
                        if (containsRambu)
                        {
                            EraseRambutAt(mouseCell);
                        }
                        GameObject a = Instantiate(rambu, selectedTileTilemap.CellToWorld(mouseCell), Quaternion.identity);
                        a.transform.SetParent(GameObject.FindWithTag("LevelTemplate").transform);
                    }
                    else
                    {
                        // place transport
                        if (containsTransport)
                        {
                            EraseTransportAt(mouseCell);
                        }
                        GameObject a = Instantiate(rambu, selectedTileTilemap.CellToWorld(mouseCell), Quaternion.identity);
                        a.transform.SetParent(GameObject.FindWithTag("LevelTemplate").transform);
                    }

                }

            }
            else if (editType == 1)
            {
                // place road
                if (!isMouseOnUI() && Input.GetMouseButton(0))
                {
                    roadTilemap.SetTile(mouseCell, roadTile);
                    decorTilemap.SetTile(mouseCell, null);
                }


            }
            else if (editType == 2)
            {

                if (Input.GetMouseButton(0) && !isMouseOnUI())
                {
                    EraseRambutAt(mouseCell);
                    if (!eraseRambuOnly)
                    {
                        // erase road
                        roadTilemap.SetTile(mouseCell, null);
                        decorTilemap.SetTile(mouseCell, null);
                        EraseTransportAt(mouseCell);
                    }


                }

            }
            else if (editType == 4)
            {
                // hand tool
                if (Input.GetMouseButtonDown(0))
                {
                    offset = Camera.main.transform.position + Input.mousePosition * handToolSensitivity;
                }
                if (Input.GetMouseButton(0))
                {
                    Camera.main.transform.position = -Input.mousePosition * handToolSensitivity + offset;
                }

            }
            else if (editType == 5)
            {
                // place decor
                if (Input.GetMouseButton(0) && !isMouseOnUI())
                {
                    decorTilemap.SetTile(mouseCell, decorTile);
                    roadTilemap.SetTile(mouseCell, null);

                    EraseRambutAt(mouseCell);
                    EraseTransportAt(mouseCell);
                }
            }
        }
    }
    public void PlaceRambu()
    {
        editType = 0;
    }
    public void PaintRoad()
    {
        editType = 1;
        rambu = null;
    }
    public void EraseRoad()
    {
        editType = 2;
        rambu = null;
    }
    public void RotateCar()
    {
        editType = 3;
        rambu = null;
    }
    public void HandTool()
    {
        editType = 4;
        rambu = null;
    }
    public void PlaceDecor()
    {
        editType = 5;
    }
    public int GetEditType()
    {
        return editType;
    }
    public void ToggleSimulation()
    {
        if (isSimulating)
        {
            StopSimulation();
        }
        else
        {
            StartSimulation();
        }
    }
    public void StartSimulation()
    {
        isSimulating = true;

        // clear memory
        transportMemories = new List<TransportMemory>();

        // save to memory
        GameObject[] transports = GameObject.FindGameObjectsWithTag("Transport");

        for (int i = 0; i < transports.Length; i++)
        {
            GameObject t = transports[i];

            TransportMemory tm = new TransportMemory();

            float defSpeed = t.GetComponent<Car>().defaultSpeed;
            if (defSpeed > 0.8f)
            {
                tm.prefab = motor;
            } else if (defSpeed > 0.5f)
            {
                tm.prefab = mobil;
            }
            else
            {
                tm.prefab = truk;
            }

            tm.direction = t.GetComponent<Car>().GetDirection();
            tm.pos = t.transform.position;

            transportMemories.Add(tm);
            
        }

        // Disable all level editors button
        editType = 4;
        for (int i = 0; i < levelEditors.Length; i++)
        {
            levelEditors[i].SetActive(false);
            
        }
    }
    public void StopSimulation()
    {
        isSimulating = false;

        // Destroy all transport
        GameObject[] transports = GameObject.FindGameObjectsWithTag("Transport");
        for (int i = 0; i < transports.Length; i++)
        {
            Destroy(transports[i]);
        }

        // Spawn all transport
        for (int i = 0; i < transportMemories.Count; i++)
        {
            GameObject prefab = transportMemories[i].prefab;
            Vector3Int direction = transportMemories[i].direction;
            Vector3 pos = transportMemories[i].pos;

            GameObject newTransport = Instantiate(transportMemories[i].prefab, pos, Quaternion.identity);
            newTransport.GetComponent<Car>().SetDirection(direction);
            newTransport.transform.SetParent(GameObject.FindWithTag("LevelTemplate").transform);
            
        }

        // enable all level editors button
        editType = 4;
        for (int i = 0; i < levelEditors.Length; i++)
        {
            levelEditors[i].SetActive(true);
        }
    }
    bool IsRambu(GameObject collision)
    {
        return collision.CompareTag("RambuKiri") || collision.CompareTag("RambuKanan") ||
                        collision.CompareTag("RambuLurus") ||
                        collision.CompareTag("Rambu30") || collision.CompareTag("Rambu60") ||
                        collision.CompareTag("Rambu90") || collision.CompareTag("RambuAkhir") ||
                        collision.CompareTag("RambuVerboden");
    }
    public void UpdateTab(string tag)
    {
        if (lantaiHighlight != null && kendaraanHighlight != null && rambuHighlight != null)
        {
            switch (tag)
            {
                case "Lantai":
                    lantaiHighlight.SetActive(true);
                    kendaraanHighlight.SetActive(false);
                    rambuHighlight.SetActive(false);
                    break;

                case "Kendaraan":
                    lantaiHighlight.SetActive(false);
                    kendaraanHighlight.SetActive(true);
                    rambuHighlight.SetActive(false);
                    break;

                case "Rambu":
                    lantaiHighlight.SetActive(false);
                    kendaraanHighlight.SetActive(false);
                    rambuHighlight.SetActive(true);
                    break;
            }
        }
        
    }
    void EraseRambutAt(Vector3Int cell)
    {
        Collider2D[] cols = Physics2D.OverlapPointAll(roadTilemap.CellToWorld(cell) + Vector3.up * 0.25f);
        for (int i = 0; i < cols.Length; i++)
        {
            Collider2D collision = cols[i];
            if (IsRambu(collision.gameObject))
            {
                Destroy(collision.gameObject);
            }
        }
    }
    void EraseTransportAt(Vector3Int cell)
    {
        Collider2D[] cols = Physics2D.OverlapPointAll(roadTilemap.CellToWorld(cell) + Vector3.up * 0.25f);
        for (int i = 0; i < cols.Length; i++)
        {
            Collider2D collision = cols[i];
            if (collision.CompareTag("Transport"))
            {
                Destroy(collision.gameObject);
            }
        }
    }
    bool isMouseOnUI()
    {
        if (Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition), blockerMask))
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    void DestroyAllWithTag(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

        for (int i = 0; i < objects.Length; i++)
        {
            Destroy(objects[i]);
        }
    }
}
