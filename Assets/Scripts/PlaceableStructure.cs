using UnityEngine;

public class PlaceableStructure : MonoBehaviour
{
    public enum StructureType
    {
        Bookshelf,
        Desk,
        Chair,
        Decor,
        Utility
    }

    [Header("Structure Info")]
    public StructureType type = StructureType.Decor;
    public bool blocksTile = true;
    public Vector2Int size = new Vector2Int(1, 1); // support for larger objects later

    [TextArea]
    public string description; // optional info for later tooltips
}