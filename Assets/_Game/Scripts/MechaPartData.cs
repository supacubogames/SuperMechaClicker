using UnityEngine;

[CreateAssetMenu(fileName = "MechaPartData", menuName = "Scriptable Objects/MechaPartData")]
public class MechaPartData : ScriptableObject
{
    public string partName;
    public double cost;
    public Sprite sprite;
}
