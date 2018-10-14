// File: Utilities.cs
// Author: Brendan Robinson
// Date Created: 01/01/2018
// Date Last Modified: 07/31/2018
// Description: 

using UnityEngine;

// Helper class
public static class Utilities
{
    public static int Mod(int k, int n)
    {
        return (k %= n) < 0 ? k + n : k;
    }

    public static Int2 GetMousePosition()
    {
        return new Int2(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
    public static float PerlinNoise(float x, float y, float scale, float height, float power)
    {
        float rValue;
        rValue = Noise.GetNoise(x / scale, y / scale, 0);
        rValue *= height;

        if (power != 0) rValue = Mathf.Pow(rValue, power);

        return rValue;
    }
    public static Vector2 RandomPointInCircle(float radius)
    {
        float t = Mathf.PI * 2 * Random.value;
        float u = Random.value + Random.value;
        float r;
        if (u > 1)
            r = 2 - u;
        else
            r = u;
        return new Vector2(radius * r * Mathf.Cos(t * Mathf.Rad2Deg), radius * r * Mathf.Sin(t * Mathf.Rad2Deg));
    }

    public static float NextGaussianDouble(float mean, float sigma)
    {
        float u, v, s;

        do
        {
            u = 2.0f * Random.value - 1.0f;
            v = 2.0f * Random.value - 1.0f;
            s = u * u + v * v;
        }
        while (s >= 1.0);

        float fac = Mathf.Sqrt(-2.0f * Mathf.Log(s) / s);

        return u * fac * sigma + mean;
    }

    public static float NormalizedRandom(float minValue, float maxValue)
    {
        float mean = (minValue + maxValue) / 2f;
        float sigma = (maxValue - mean) / 3f;
        float result;
        do
        {
            result = NextGaussianDouble(mean, sigma);
        } while (result > maxValue || result < minValue);
        return result;
    }
}