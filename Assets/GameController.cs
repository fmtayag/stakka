using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        Transform canvas = GameObject.Find("Canvas").transform;
        GameObject prefab = GameObject.Find("Image");
        List<GameObject> grid_images = new List<GameObject>();
        // Create UI image
        for(int y=0; y < 20; y++) {
            for(int x=0; x < 10; x++) {
                Vector2 origPos = new Vector2(-100, -500);
                GameObject newImg = Instantiate(prefab, Vector3.zero, Quaternion.identity, canvas);
                
                RectTransform rectTransform = newImg.GetComponent<RectTransform>();
                float size = rectTransform.rect.height;
                float offset = 10;
                rectTransform.anchoredPosition = new Vector2((x*size) + (x * offset), y * (size + offset)) + origPos;

                grid_images.Add(newImg);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
