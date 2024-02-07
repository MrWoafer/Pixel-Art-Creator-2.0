using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public int rows = 64;
    public int columns = 64;
    public GameObject testPrefab;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < rows * columns; i++)
        {
            GameObject obj = Instantiate(testPrefab, transform);
            obj.transform.localPosition = new Vector3(i % rows - rows / 2, i / columns - columns / 2, 0f);
            obj.GetComponent<SpriteRenderer>().color = new Color((float)i / (rows * columns), (float)i / (rows * columns), (float)i / (rows * columns));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
