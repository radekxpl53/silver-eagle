using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private Sector sectorPrefab;
    [SerializeField] private Transform gridParent;

    private void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        int width = gridSystem.Col;
        int height = gridSystem.Row;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Sector sector = Instantiate(sectorPrefab, gridParent);
                sector.gridPosition = new Vector2Int(x, y);
                sector.name = $"Sector {x + 1} {y + 1}";
            }
        }
    }
}
