using UnityEngine;

public enum TileColor {
    RED,
    GREEN,
    BLUE,
    YELLOW,
    ORANGE,
    CYAN,
    PURPLE,
    WHITE,
}

public struct GameDefines {
    public static int SCORE_PER_ROW_CLEARED = 10; 
    public static int ROWS_NEEDED_FOR_LEVEL_ADVANCE = 12;
    public static float DEFAULT_GRAVITY_DELAY = 1f;
    public static float GRAVITY_MULTIPLICAND = 0.1f;
    public static float MIN_GRAVITY_DELAY = 0.1f;
    public static int MIN_LEVEL = 1;
    public static int MAX_LEVEL = 8;
    
    // Tetromino colors
    public static Color COLOR_TANGO     =   GameUtils.NormalizedColor(228, 193, 249);
    public static Color COLOR_OSCAR     =   GameUtils.NormalizedColor(237, 231, 177);
    public static Color COLOR_INDIA     =   GameUtils.NormalizedColor(169, 222, 249);
    public static Color COLOR_JULIETT   =   GameUtils.NormalizedColor(113, 97, 239);
    public static Color COLOR_LIMA      =   GameUtils.NormalizedColor(255, 217, 125);
    public static Color COLOR_SIERRA    =   GameUtils.NormalizedColor(96,  211, 148);
    public static Color COLOR_ZULU      =   GameUtils.NormalizedColor(238,  96,  85);

    // Spawn positions
    public static Vector3 SPAWN_POS_T = new Vector3(4, 0);
    public static Vector3 SPAWN_POS_O = new Vector3(4, 0);
    public static Vector3 SPAWN_POS_I = new Vector3(5, 0);
    public static Vector3 SPAWN_POS_J = new Vector3(6, 0);
    public static Vector3 SPAWN_POS_L = new Vector3(4, 0);
    public static Vector3 SPAWN_POS_SZ = new Vector3(5, 0);
}