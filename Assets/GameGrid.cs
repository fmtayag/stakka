using UnityEngine;
using UnityEngine.UI;

public struct Tile {
    public TileState State { get; set; }
    public Color Color { get; set; }

    public Tile(TileState state, Color color)
    {
        State = state;
        Color = color;
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
        prefab = GameObject.Find("Image");
        canvas = GameObject.Find("Canvas").transform;

        for(int row=0; row < GameGrid.GRID_HEIGHT; row++) {
            for (int col=0; col < GameGrid.GRID_WIDTH; col++) {
                Grid[row, col] = new Tile(TileState.FREE, Color.white);
            }
        }

        for(int i=0; i < GameGrid.GRID_WIDTH; i++) {
            Grid[19, i] = new Tile(TileState.PLACED, GameDefines.COLOR_LIMA); // TODO: Test
            Grid[18, i] = new Tile(TileState.PLACED, GameDefines.COLOR_LIMA); // TODO: Test
            Grid[17, i] = new Tile(TileState.PLACED, GameDefines.COLOR_LIMA); // TODO: Test
            Grid[16, i] = new Tile(TileState.PLACED, GameDefines.COLOR_LIMA); // TODO: Test
        }

        Grid[15, 7] = new Tile(TileState.FREE, GameDefines.COLOR_LIMA);
        Grid[15, 6] = new Tile(TileState.FREE, GameDefines.COLOR_LIMA);

        Grid[16, 0] = new Tile(TileState.FREE, GameDefines.COLOR_INDIA);
        Grid[16, 1] = new Tile(TileState.FREE, GameDefines.COLOR_INDIA);
        Grid[16, 2] = new Tile(TileState.FREE, GameDefines.COLOR_JULIETT);

        Grid[17, 0] = new Tile(TileState.FREE, GameDefines.COLOR_JULIETT);
        Grid[17, 1] = new Tile(TileState.FREE, GameDefines.COLOR_SIERRA);
        Grid[17, 6] = new Tile(TileState.FREE, GameDefines.COLOR_SIERRA);
        Grid[17, 7] = new Tile(TileState.FREE, GameDefines.COLOR_TANGO);

        Grid[18, 0] = new Tile(TileState.FREE, GameDefines.COLOR_TANGO);
        Grid[18, 7] = new Tile(TileState.FREE, GameDefines.COLOR_OSCAR);

        Grid[19, 0] = new Tile(TileState.FREE, GameDefines.COLOR_OSCAR);

        CreateDisplayGrid();
    }

    void Update() {}

    private void CreateDisplayGrid() {
        // Create the display grid
        for (int y = 0; y < GameGrid.GRID_HEIGHT; y++)
        {
            for (int x = 0; x < GameGrid.GRID_WIDTH; x++)
            {
                Vector2 origPos = new Vector2(-100, -300);
                GameObject newImg = Instantiate(prefab, Vector3.zero, Quaternion.identity, canvas);

                RectTransform rectTransform = newImg.GetComponent<RectTransform>();
                // float size = rectTransform.rect.height;
                float size = 32;
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
