using UnityEngine;

[CreateAssetMenu(fileName = "GridPerlinWormSettings", menuName = "GridPerlinWormSettings", order = 0)]
public class GridPerlinWormSettings : ScriptableObject 
{
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
}