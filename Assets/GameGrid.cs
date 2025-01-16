using UnityEngine;
using UnityEngine.UI;

public struct Tile {
    public TileState State { get; set; }
    public TileColor TileColor { get; set; }

    public Tile(TileState state, TileColor color)
    {
        State = state;
        TileColor = color;
    }
};

public enum TileState
{
    FREE, PLACED, BLOCK
}

public class GameGrid : MonoBehaviour
{
    public Tile[,] Grid { get; set; }
    public GameObject[,] DisplayGrid { get; set; }
    public const short GRID_WIDTH = 10;
    public const short GRID_HEIGHT = 20;

    public GameObject prefab;
    Transform canvas;

    void Start()
    {
        Grid = new Tile[GRID_HEIGHT, GRID_WIDTH];
        DisplayGrid = new GameObject[GRID_HEIGHT, GRID_WIDTH];
        prefab = GameObject.Find("Tile");
        canvas = GameObject.Find("Canvas").transform;

        for(int row=0; row < GameGrid.GRID_HEIGHT; row++) {
            for (int col=0; col < GameGrid.GRID_WIDTH; col++) {
                Grid[row, col] = new Tile(TileState.FREE, TileColor.WHITE);
            }
        }

        for(int i=0; i < GameGrid.GRID_WIDTH; i++) {
            Grid[19, i] = new Tile(TileState.PLACED, TileColor.RED); // TODO: Test
            Grid[18, i] = new Tile(TileState.PLACED, TileColor.BLUE); // TODO: Test
            Grid[17, i] = new Tile(TileState.PLACED, TileColor.GREEN); // TODO: Test
            Grid[16, i] = new Tile(TileState.PLACED, TileColor.ORANGE); // TODO: Test
        }

        Grid[15, 7] = new Tile(TileState.FREE, TileColor.BLUE);
        Grid[15, 6] = new Tile(TileState.FREE, TileColor.BLUE);

        Grid[16, 0] = new Tile(TileState.FREE, TileColor.BLUE);
        Grid[16, 1] = new Tile(TileState.FREE, TileColor.BLUE);
        Grid[16, 2] = new Tile(TileState.FREE, TileColor.BLUE);

        Grid[17, 0] = new Tile(TileState.FREE, TileColor.BLUE);
        Grid[17, 1] = new Tile(TileState.FREE, TileColor.BLUE);
        Grid[17, 6] = new Tile(TileState.FREE, TileColor.BLUE);
        Grid[17, 7] = new Tile(TileState.FREE, TileColor.BLUE);

        Grid[18, 0] = new Tile(TileState.FREE, TileColor.BLUE);
        Grid[18, 7] = new Tile(TileState.FREE, TileColor.BLUE);

        Grid[19, 0] = new Tile(TileState.FREE, TileColor.BLUE);

        CreateDisplayGrid();
    }

    void Update() {}

    private void CreateDisplayGrid() {
        // Create the display grid
        for (int y = 2; y < GameGrid.GRID_HEIGHT; y++)
        {
            for (int x = 0; x < GameGrid.GRID_WIDTH; x++)
            {
                // Gonna cheat here a little bit, and make the Tile prefab serve as the origin point
                // Note that this assumes that Tile prefab's pivot point is x=0, y=1
                float size = 32; // This is the size of the tile
                float origX = prefab.GetComponent<RectTransform>().anchoredPosition.x;
                float origY = prefab.GetComponent<RectTransform>().anchoredPosition.y;

                Vector2 origPos = new Vector2(origX, -origY);
                GameObject newImg = Instantiate(prefab, Vector3.zero, Quaternion.identity, canvas);

                RectTransform rectTransform = newImg.GetComponent<RectTransform>();
                // float size = rectTransform.rect.height;
                float spacing = 0;
                float offset_y = 16;
                rectTransform.anchoredPosition = new Vector2(
                    (x * size) + (x * spacing), 
                    (GRID_HEIGHT-y) * (size + spacing)
                    // (GRID_HEIGHT-y) explanation: Invert the y axis such that the tiles are drawn from the Top-Left
                ) + origPos;

                newImg.transform.SetSiblingIndex(y); // for draw ordering, makes it so that a row overlaps the row below it

                // TODO: Just for messing around
                // rectTransform.localScale = new Vector3(1f + (y*0.1f), 1f + (y*0.1f), 0);

                DisplayGrid[y, x] = newImg;
                // Grid.SetObject(y, x, newImg);
            }
        }
    }
}
