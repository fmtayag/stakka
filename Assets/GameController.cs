using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is create
    GameGrid gameGrid;
    Tetromino tetro;
    Tetromino[] dbgTetros;
    int dbgIndex;
    int axisHori = 0;
    int axisVerti = 0;
    float goDownTimer;
    float goDownDelay = 1f;
    bool canLock = false;


    void Start()
    {
        GameObject obj = new GameObject("GameGridObject");
        gameGrid = obj.AddComponent<GameGrid>();

        dbgIndex = 0;
        dbgTetros = new Tetromino[] {
            new TetroTango(), 
            new TetroOscar(), 
            new TetroIndia(), 
            new TetroJuliett(), 
            new TetroLima(),
            new TetroSierra(),
            new TetroZulu()
        };
        tetro = dbgTetros[dbgIndex];

        goDownTimer = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        PlaceTetro();
        LineClear();
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
            axisVerti = 1;
            goDownTimer = Time.time; // Reset timer
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            Debug.Log("Pressed 'Rotate' key");
            tetro.Rotate(gameGrid.Grid);
        }
        else if (Input.GetKeyDown(KeyCode.F1)) {
            Debug.Log("Pressed 'Switch' key");
            dbgIndex++;

            if (dbgIndex > dbgTetros.Length - 1){
                dbgIndex = 0;
            }
            
            int oldXPos = (int) tetro.position.x;
            int oldYPos = (int) tetro.position.y; 
            
            // Remove the blocks in current position
            int[,] blocks = tetro.GetBlocks(tetro.rotation);
            for(int row=0; row<4; row++) {
                int bx = blocks[row, 0];
                int by = blocks[row, 1];

                gameGrid.Grid[oldYPos+by, oldXPos+bx] = TileState.FREE;
            }

            tetro = dbgTetros[dbgIndex];
            tetro.position.x = oldXPos;
            tetro.position.y = oldYPos;
            
        }   
    }

    void PlaceTetro()
    {   
        // Automatically go down after 1000ms
        float now = Time.time;

        if (now - goDownTimer >= goDownDelay) {
            axisVerti = 1;
            goDownTimer = now;
            canLock = true;
        }

        if (axisHori != 0 || axisVerti != 0)
        {
            gameGrid.Grid = tetro.Move(gameGrid.Grid, axisHori, axisVerti, canLock);
        }
        else
        {
            int[,] blocks = tetro.GetBlocks(tetro.rotation);

            for (int row = 0; row < 4; row++)
            {
                int bx = blocks[row, 0];
                int by = blocks[row, 1];
                gameGrid.Grid[(int)(tetro.position.y + by), (int)(tetro.position.x + bx)] = TileState.BLOCK;
            }
        }
    }

    void LineClear() {
        // DONE: Check for cleared rows
        // TODO: Shift down the rows above the cleared rows
        // Optimization idea: Check only from the topmost row to the bottom row
        // Optimization idea: Run only when there is input
        
        // Scan for cleared rows
        bool isLineClear = true;
        bool[] clearedRows = new bool[20];

        if (axisVerti != 0) {
            for(int row=0; row<GameGrid.GRID_HEIGHT; row++) {
                for(int col=0; col<GameGrid.GRID_WIDTH; col++) {
                    if (gameGrid.Grid[row, col] != TileState.PLACED) {
                        isLineClear = false;
                        break;
                    }
                    else {
                        isLineClear = true;
                    }
                }

                if (isLineClear) {  
                    Debug.Log($"Cleared row: {row}");
                    clearedRows[row] = true;
                    // TODO: Test remove
                    for(int col=0; col<GameGrid.GRID_WIDTH; col++) 
                    {
                        gameGrid.Grid[row, col] = TileState.FREE;
                    }
                }
            }
        }
        

        // TODO: Remove cleared rows


        // TODO: Shift down rows to fill the gaps created by removed rows
            
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
