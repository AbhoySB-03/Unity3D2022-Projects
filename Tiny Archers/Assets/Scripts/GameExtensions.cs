using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameExtensions
{
    public static bool ContainsLayer(this LayerMask layermask, int otherLayer)
    {
        return layermask == (layermask | (1 << otherLayer));
    }
}
