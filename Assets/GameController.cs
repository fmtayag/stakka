using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    GameGrid gameGrid;
    Tetromino tetro;
    Tetromino[] dbgTetros;
    int dbgIndex;
    int axisHori = 0;
    int axisVerti = 0;
    float goDownTimer;
    float goDownDelay = 1f;
    bool canLock = false;

    const float initInputDelay = 0.6f;
    const float fastInputDelay = 0.05f;
    float inputDelay = initInputDelay;
    float inputTimer;

    float inputVertDelay = initInputDelay;
    float inputVertTimer;
    bool isHardDropping = false;

    void Start()
    {
        GameObject obj = new GameObject("GameGridObject");
        gameGrid = obj.AddComponent<GameGrid>();

        dbgIndex = 3;
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
        inputTimer = 0;
        inputVertTimer = 0;
    }

    void Update()
    {
        GetInput();
        PlaceTetro();
        if (axisVerti != 0) // Change condition later
        {
            gameGrid.Grid = LineClearHandler.Handle(gameGrid.Grid);
        }
        DrawGrid();
    }

    void GetInput()
    {
        axisVerti = 0;
        isHardDropping = false;
        float now = Time.time;
        // TODO: Precise inputs (DONE, me thinks)
        // TODO: Hold inputs (DONE, me thinks)
        // TODO: Hard drops

        // -- Horizontal movement
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (now - inputTimer >= inputDelay)
            {
                inputTimer = now;
                inputDelay = fastInputDelay;
                axisHori = -1;
            }
            else
            {
                axisHori = 0;
            }
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (now - inputTimer >= inputDelay)
            {
                inputTimer = now;
                inputDelay = fastInputDelay;
                axisHori = 1;
            }
            else
            {
                axisHori = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            axisHori = -1;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            axisHori = 1;

        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            inputDelay = initInputDelay;
            inputTimer = Time.time;
        }

        // -- Vertical movement
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (now - inputVertTimer >= inputVertDelay)
            {
                inputVertTimer = now;
                inputVertDelay = fastInputDelay;
                axisVerti = 1;
                // goDownTimer = Time.time; // Reset timer
                canLock = true; // TODO: notice me if things go to shit
            }
            else
            {
                axisVerti = 0;
            }
        }

        // -- Drops
        if (Input.GetKeyDown(KeyCode.DownArrow)) // Soft drop
        {
            axisVerti = 1;
            goDownTimer = Time.time; // Reset timer
            canLock = true; // TODO: notice me if things go to shit
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            inputVertDelay = initInputDelay;
            inputVertTimer = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.Space)) // Hard Drop
        {
            Debug.Log("Pressed 'Hard drop' key");
            isHardDropping = true;
        }

        // -- Rotate
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("Pressed 'Rotate' key");
            tetro.Rotate(gameGrid.Grid);
        }
        // if (Input.GetKeyDown(KeyCode.F1)) {
        //     // For debugging only
        //     Debug.Log("Pressed 'Switch' key");
        //     dbgIndex++;

        //     if (dbgIndex > dbgTetros.Length - 1){
        //         dbgIndex = 0;
        //     }

        //     int oldXPos = (int) tetro.position.x;
        //     int oldYPos = (int) tetro.position.y; 

        //     // Remove the blocks in current position
        //     int[,] blocks = tetro.GetBlocks(tetro.rotation);
        //     for(int row=0; row<4; row++) {
        //         int bx = blocks[row, 0];
        //         int by = blocks[row, 1];

        //         gameGrid.Grid[oldYPos+by, oldXPos+bx].State = TileState.FREE;
        //     }

        //     tetro = dbgTetros[dbgIndex];
        //     tetro.position.x = oldXPos;
        //     tetro.position.y = oldYPos;

        //     inputTimer = now;

        // }   
    }

    void PlaceTetro()
    {
        // Automatically go down after some specified time
        float now = Time.time;

        if (now - goDownTimer >= goDownDelay)
        {
            axisVerti = 1;
            // axisVerti = 0;
            goDownTimer = now;
            canLock = true;
        }

        if (axisHori != 0 || axisVerti != 0 || isHardDropping)
        {
            if (!isHardDropping)
                gameGrid.Grid = tetro.Move(gameGrid.Grid, axisHori, axisVerti, canLock);
            else
            {
                // Convert tetro into a "segment"
                List<(int, int)> myTetro = new List<(int, int)>();
                for (int i = 0; i < 4; i++)
                {
                    int bx = (int)tetro.position.x + tetro.GetBlocks(tetro.rotation)[i, 0];
                    int by = (int)tetro.position.y + tetro.GetBlocks(tetro.rotation)[i, 1];

                    myTetro.Add((by, bx));
                }

                gameGrid.Grid = LineClearHandler.MoveSegmentDown(gameGrid.Grid, myTetro);

                isHardDropping = false;

                // TODO: 'Reset the tetromino'
                tetro.position = new Vector2(5, 3); 
            }
        }
        else
        {
            int[,] blocks = tetro.GetBlocks(tetro.rotation);

            for (int row = 0; row < 4; row++)
            {
                int bx = blocks[row, 0];
                int by = blocks[row, 1];
                gameGrid.Grid[(int)(tetro.position.y + by), (int)(tetro.position.x + bx)] = new Tile(TileState.BLOCK, tetro.myColor);
            }
        }
    }

    void DrawGrid()
    {
        for (int y = 0; y < GameGrid.GRID_HEIGHT; y++)
        {
            for (int x = 0; x < GameGrid.GRID_WIDTH; x++)
            {
                Tile tile = gameGrid.Grid[y, x];
                switch (tile.State)
                {
                    case TileState.FREE:
                        gameGrid.DisplayGrid[y, x].GetComponent<Image>().color = Color.white;
                        break;
                    case TileState.BLOCK:
                        gameGrid.DisplayGrid[y, x].GetComponent<Image>().color = tile.Color;
                        break;
                    case TileState.PLACED:
                        gameGrid.DisplayGrid[y, x].GetComponent<Image>().color = tile.Color;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
