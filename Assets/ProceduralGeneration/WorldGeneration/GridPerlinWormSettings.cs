using UnityEngine;

[CreateAssetMenu(fileName = "GridPerlinWormSettings", menuName = "GridPerlinWormSettings", order = 0)]
public class GridPerlinWormSettings : ScriptableObject 
{
    public int Seed = 1337;
    public Vector3 Scale;
    public Vector2 StartingPoint;
    public Vector2 ConvergencePoint;
    public float Weight;
    public float Range;
    public float Frequency = 0.1f;

    public int Length;
    public bool Converge;

    public bool SwitchDirection;
    public int SwitchDirectionPeriod = 100;

    /// <summary>
    /// Frequency of the noise used to create the river.
    /// </summary>
    public float NoiseFrequency;
    public int Octaves;
    public float Persistance;
}