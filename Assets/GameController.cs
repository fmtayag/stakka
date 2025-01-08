using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is create
    GameGrid gameGrid;
    TetroT tetro = new TetroT();
    int axisHori = 0;
    int axisVerti = 0;

    void Start()
    {
        GameObject obj = new GameObject("GameGridObject");
        gameGrid = obj.AddComponent<GameGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        PlaceTetro();
        DrawGrid();
    }

    void GetInput()
    {
        axisHori = 0;
        axisVerti = 0;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Pressed Left");
            axisHori = -1;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Pressed Right");
            axisHori = 1;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("Pressed Down");
            axisVerti = -1;
        }
    }

    void PlaceTetro()
    {
        if (axisHori != 0 || axisVerti != 0)
        {
            gameGrid.Grid = tetro.Move(gameGrid.Grid, axisHori, axisVerti);

        }
        else
        {
            int[,] blocks = tetro.GetBlocks();

            for (int row = 0; row < 4; row++)
            {
                int bx = blocks[row, 0];
                int by = blocks[row, 1];
                gameGrid.Grid[(int)(tetro.position.y + by), (int)(tetro.position.x + bx)] = TileState.BLOCK;
            }
        }
    }

    void DrawGrid()
    {
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
