using UnityEngine;

public class GameMap : MonoBehaviour
{
    public int width = 10;
    public int length = 10;

    void Start()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                CreateMapTile(x, y);
            }
        }
    }

    private GameObject CreateMapTile(int x , int y)
    {
        var tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
        tile.transform.parent = transform;
        tile.transform.position = new Vector3(x, 0, y);
        tile.AddComponent<MapTile>();
        tile.transform.localScale = new Vector3(0.95f, 0.95f, 1f);
        tile.transform.rotation = Quaternion.Euler(90f, 0, 0);
        tile.name = "(" + x.ToString() +  ", " + y.ToString() + ")";

        tile.GetComponent<MeshRenderer>().material.color = Color.grey;

        return tile;
    }
}
