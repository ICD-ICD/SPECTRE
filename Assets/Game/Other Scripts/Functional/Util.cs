using UnityEngine;

public static class Util
{
    public static Vector2 DirectionTo(Vector2 to, Vector2 from)
    {
        return (to - from).normalized;
    }
}
