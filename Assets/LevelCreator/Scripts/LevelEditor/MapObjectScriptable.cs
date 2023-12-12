using UnityEngine;

[CreateAssetMenu(fileName ="New Tile",menuName ="Map Tile")]
public class MapObjectScriptable : ScriptableObject
{
    public bool groundLevel = true;
    public ObjectType objectType;
    public Color color;
    public GameObject prefab;
}

public enum ObjectType { none,ground,wall,encounter,interactable,door,key }
