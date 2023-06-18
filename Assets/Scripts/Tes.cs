using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tes : MonoBehaviour
{
    public GameObject ob;
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(wowo());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator wowo()
    {
        yield return new WaitForSeconds(3);
        Instantiate(ob);
    }
}
