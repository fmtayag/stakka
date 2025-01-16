using System.Collections.Generic;
using UnityEngine;

class GameInputNew {
    Vector2 velocity = Vector2.zero;
    int currentDirection = 0;
    float keyLeftTimer  = 0f;
    float keyRightTimer  = 0f;
    float keyDownTimer = 0f;

    const float keyHoldDelay = 0.3f;
    const float keyRepeatRate = 0.25f;

    float gravityTimer;
    private float _gravityDelay;
    public float GravityDelay
    {
        get
        {
            return _gravityDelay;
        }
        set
        {
            _gravityDelay = Mathf.Clamp(value, GameDefines.MIN_GRAVITY_DELAY, GameDefines.DEFAULT_GRAVITY_DELAY);
        }
    }
    bool canLock = false;

    const float initInputDelay = 0.6f;
    const float fastInputDelay = 0.05f;
    float inputDelay = initInputDelay;
    float inputTimer;

    float inputVertDelay = initInputDelay;
    float inputVertTimer;
    bool isHardDropping = false;

    public GameInputNew () {
        gravityTimer = Time.time;
        GravityDelay = GameDefines.DEFAULT_GRAVITY_DELAY;
        inputTimer = 0;
        inputVertTimer = 0;
    }

    public void GetInput(GameGrid gameGrid, Tetromino tetro)
    {
        velocity = Vector2.zero;
        float now = Time.time;

        // -- Horizontal movement
        // ---- LEFT
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Pressed 'Left' key");
            velocity.x = -1;
            currentDirection = -1;
            keyLeftTimer = now;
        }
        if (Input.GetKey(KeyCode.LeftArrow) && currentDirection == -1)
        {
            Debug.Log(now - keyLeftTimer);
            if (now - keyLeftTimer >= keyHoldDelay)
            {
                velocity.x = -1;
                keyLeftTimer = now - keyRepeatRate;
                // Debug.Log($"Now: {now} KeyLeftTimer: {keyLeftTimer}");
            }
        }

        // ---- RIGHT
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Pressed 'Right' key");
            velocity.x = 1;
            currentDirection = 1;
            keyRightTimer = now;
        }
        if (Input.GetKey(KeyCode.RightArrow) && currentDirection == 1)
        {
            Debug.Log(now - keyRightTimer);
            if (now - keyRightTimer >= keyHoldDelay)
            {
                velocity.x = 1;
                keyRightTimer = now - keyRepeatRate;
                // Debug.Log($"Now: {now} KeyRightTimer: {keyRightTimer}");
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow) && currentDirection == -1) {
            Debug.Log("Key up Left!");
            currentDirection = 1;
            keyRightTimer = now;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow) && currentDirection == 1) {
            Debug.Log("Key up Right!");
            currentDirection = -1;
            keyLeftTimer = now;
        }

        // -- Vertical movement
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            Debug.Log("Pressed 'Down' key");
            velocity.y = 1;
            keyDownTimer = now;
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            if (now - keyDownTimer >= keyHoldDelay)
            {
                velocity.y = 1;
                keyDownTimer = now - keyRepeatRate;
            }
        }

        // -- Rotate
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("Pressed 'Rotate' key");
            tetro.Rotate(gameGrid.Grid);
        }

        // -- Hard drop

    }

    public void PlaceTetro(GameGrid gameGrid, Tetromino tetro)
    {

        gameGrid.Grid = tetro.Move(gameGrid.Grid, velocity, canLock);

        // // Automatically go down after some specified time
        // float now = Time.time;

        // if (now - gravityTimer >= GravityDelay)
        // {
        //     // axisVerti = 1;
        //     axisVerti = 0;
        //     gravityTimer = now;
        //     canLock = true;
        // }

        // if (a|| isHardDropping)
        // {
        //     if (!isHardDropping) 
        //         gameGrid.Grid = tetro.Move(gameGrid.Grid, velocity, canLock);
        //     else
        //     {
        //         Debug.Log("Doing hard drop");
        //         DoHardDrop(gameGrid, tetro);
        //     }
        // }
        // else
        // {
        //     int[,] blocks = tetro.GetBlocks(tetro.rotation);

        //     for (int row = 0; row < 4; row++)
        //     {
        //         int bx = blocks[row, 0];
        //         int by = blocks[row, 1];
        //         gameGrid.Grid[(int)(tetro.position.y + by), (int)(tetro.position.x + bx)] = new Tile(TileState.BLOCK, tetro.myColor);
        //     }
        // }
    }

    void DoHardDrop(GameGrid gameGrid, Tetromino tetro) {
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

        tetro.Reset();
    }
}