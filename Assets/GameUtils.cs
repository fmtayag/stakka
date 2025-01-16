using UnityEngine;
using System;
using System.Collections.Generic;

class GameUtils {
    public static Color NormalizedColor(float r, float g, float b) {
        return new Color(r/255f, g/255f, b/255f);
    }
}

public static class ListExtension {
    private static System.Random rng = new System.Random();  

    public static void Shuffle<T>(this IList<T> list)  
    {  
        // Credits: https://stackoverflow.com/a/1262619
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }
}