using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseSettings : MonoBehaviour
{
    public enum NoiseType
    {
        Perlin, Perlin_Abs, Simplex, Fractal
    }

    public NoiseType noiseType = NoiseType.Perlin;
    public bool enabled = false;
    public uint interations = 1;
    public float strength = 1f;
    public float roughness = 1f;
    public float scale = 1f;
    public float persistance = 0.5f;
    public float lacunarity = 1;
    public float minValue = 0f;
}
