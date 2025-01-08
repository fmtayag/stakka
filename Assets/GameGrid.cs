using UnityEngine;
using UnityEngine.UI;

public enum TileState
{
    FREE, PLACED, BLOCK
}

public class GameGrid : MonoBehaviour
{
    public TileState[,] Grid { get; set; }
    public GameObject[,] DisplayGrid { get; set; }
    public const short GRID_WIDTH = 10;
    public const short GRID_HEIGHT = 20;

    public GameObject prefab;
    Transform canvas;

    void Start()
    {
        Grid = new TileState[GRID_HEIGHT, GRID_WIDTH];
        DisplayGrid = new GameObject[GRID_HEIGHT, GRID_WIDTH];
        prefab = GameObject.Find("Image");
        canvas = GameObject.Find("Canvas").transform;

        CreateDisplayGrid();
    }

    void Update() {}

    private void CreateDisplayGrid() {
        // Create game grid
        for (int y = 0; y < 20; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                Vector2 origPos = new Vector2(-100, -500);
                GameObject newImg = Instantiate(prefab, Vector3.zero, Quaternion.identity, canvas);

                RectTransform rectTransform = newImg.GetComponent<RectTransform>();
                float size = rectTransform.rect.height;
                float offset = 10;
                rectTransform.anchoredPosition = new Vector2((x * size) + (x * offset), y * (size + offset)) + origPos;

                DisplayGrid[y, x] = newImg;
                // Grid.SetObject(y, x, newImg);
            }
        }
    }
}
