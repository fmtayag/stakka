using UnityEngine;

class GameUtils {
    public static Color NormalizedColor(float r, float g, float b) {
        return new Color(r/255f, g/255f, b/255f);
    }
}