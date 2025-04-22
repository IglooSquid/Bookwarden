using UnityEngine;

public class TileProperties : MonoBehaviour
{
    public bool isExpansionZone = false;
    public bool isBuilt = false;

    public Material builtMaterial;
    public Material unbuiltMaterial;
    public Material expansionMaterial;
    public bool hasPlacedStructure => placedStructure != null;

    public enum TileOccupancy
    {
        Empty,
        RoomChunk,
        Reserved,
        Decoration
    }

    public TileOccupancy occupancy = TileOccupancy.Empty;
    public PlaceableStructure placedStructure;

    public void UpdateVisual()
    {
        var rend = GetComponent<Renderer>();
        if (rend != null)
        {
            if (isBuilt && builtMaterial != null)
                rend.material = builtMaterial;
            else if (isExpansionZone && expansionMaterial != null)
                rend.material = expansionMaterial;
            else if (unbuiltMaterial != null)
                rend.material = unbuiltMaterial;
        }
    }
}