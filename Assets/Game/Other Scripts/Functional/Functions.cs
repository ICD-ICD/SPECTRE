using System;
using R3;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Collections.Generic;

public static class Functions
{
    public static bool AreEqual(double a, double b) => Math.Abs(a - b) < double.Epsilon;
    public static bool AreEqual(float a, float b) => Math.Abs(a - b) < float.Epsilon;
    public static float GetPercent(float maxValue, float value) => value / maxValue;
    public static double GetPercent(double maxValue, double value) => value / maxValue;
    public static float LinearFunc(float x, float k, float b) => k*x + b;
    public static float LinearFunc(float x, float minValue, float maxValue, float minX, float maxX)
    {
        if (Math.Abs(minX - maxX) < float.Epsilon)
            throw new ArgumentException("minX и maxX не могут быть равны (вертикальна¤ лини¤ Ч это не функци¤).");

        float k = (maxValue - minValue) / (maxX - minX);
        float b = minValue - k * minX;

        return LinearFunc(x, k, b);
    }
    public static float LinearFunc(float x, float minValue, float maxValue, float minX, float maxX, Func<bool> floorCondition, float floor)
    {
        if (Math.Abs(minX - maxX) < float.Epsilon)
            throw new ArgumentException("minX и maxX не могут быть равны (вертикальна¤ лини¤ Ч это не функци¤).");
        if (floorCondition == null)
            throw new ArgumentNullException(nameof(floorCondition) + "is Null");

        if (floorCondition()) return floor;

        float k = (maxValue - minValue) / (maxX - minX);
        float b = minValue - k * minX;

        return LinearFunc(x, k, b);
    }

    public static float HyperbolicFuncNormalized(float x, float zeroValuePoint)
    {
        return zeroValuePoint * 2f / (x + zeroValuePoint) - 1;
    }

    #region DEBUG
    public static void DrawBox(Vector2 point, Vector2 size, Color color)
    {
        Debug.DrawLine(point, new(point.x, point.y + size.y), color);
        Debug.DrawLine(new(point.x, point.y + size.y), new(point.x + size.x, point.y + size.y), color);
        Debug.DrawLine(new(point.x + size.x, point.y + size.y), new(point.x + size.x, point.y), color);
        Debug.DrawLine(new(point.x + size.x, point.y), point, color);
    }

    public static void DrawBoxCentered(Vector2 point, Vector2 size, Color color)
    {
        point.x -= size.x / 2f;
        point.y -= size.y / 2f;
        Debug.DrawLine(point, new(point.x, point.y + size.y), color);
        Debug.DrawLine(new(point.x, point.y + size.y), new(point.x + size.x, point.y + size.y), color);
        Debug.DrawLine(new(point.x + size.x, point.y + size.y), new(point.x + size.x, point.y), color);
        Debug.DrawLine(new(point.x + size.x, point.y), point, color);
    }

    public static void PrintList<T>(IEnumerable<T> collection, Func<int, object> whatToString = null)
    {
        var strBuilder = new StringBuilder();
        strBuilder.Append("[");

        if (whatToString != null)
            for (int i = 0; i < collection.Count(); i++)
            {
                strBuilder.Append(whatToString.Invoke(i));
                if (i + 1 != collection.Count())
                {
                    strBuilder.Append(", ");
                }
            }
        else
            for (int i = 0; i < collection.Count(); i++)
            {
                strBuilder.Append(collection.ElementAt(i));
                if (i + 1 != collection.Count())
                {
                    strBuilder.Append(", ");
                }
            }
        Debug.Log(strBuilder.Append("]").ToString());
    }
    #endregion
}
