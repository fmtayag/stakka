using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is create
    GameGrid gameGrid;
    TetroT tetro = new TetroT();

    void Start()
    {
        GameObject obj = new GameObject("GameGridObject");
        gameGrid = obj.AddComponent<GameGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        // -- Get input
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Pressed Left");
            tetro.position.x -= 1;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Pressed Right");
            tetro.position.x += 1;
        }

        // -- Refresh grid
        for (int y = 0; y < GameGrid.GRID_HEIGHT; y++)
        {
            for (int x = 0; x < GameGrid.GRID_WIDTH; x++)
            {
                bool tileIsNotPlaced = gameGrid.Grid[y, x] != TileState.PLACED;
                if (tileIsNotPlaced)
                {
                    gameGrid.Grid[y, x] = TileState.FREE;
                }
            }
        }

        // -- 'Place' the tetro on the grid
        int[,] blocks = tetro.GetBlocks();
        for (int row = 0; row < 4; row++)
        {
            int x = blocks[row, 0];
            int y = blocks[row, 1];

            // TODO: Add array out-of-bounds checking
            Vector2 pos = tetro.position;
            int gridY = (int)pos.y + y;
            int gridX = (int)pos.x + x;
            gameGrid.Grid[gridY, gridX] = TileState.BLOCK;
            // Debug.Log(gameGrid.Grid[gridY, gridX]);
        }

        // -- 'Draw' the grid
        for (int y = 0; y < GameGrid.GRID_HEIGHT; y++)
        {
            for (int x = 0; x < GameGrid.GRID_WIDTH; x++)
            {
                TileState tile = gameGrid.Grid[y, x];
                switch (tile)
                {
                    case TileState.FREE:
                        gameGrid.DisplayGrid[y, x].GetComponent<Image>().color = Color.white;
                        break;
                    case TileState.BLOCK:
                        gameGrid.DisplayGrid[y, x].GetComponent<Image>().color = Color.red;
                        break;
                    case TileState.PLACED:
                        gameGrid.DisplayGrid[y, x].GetComponent<Image>().color = Color.green;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
