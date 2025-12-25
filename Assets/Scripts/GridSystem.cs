using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private float side = 7000;
    private int row = 6;
    private int col = 6;
    public float Side => side;
    public int Row => row;
    public int Col => col;
    private float maxX;
    private float maxZ;
    private float maxY;

    void Start()
    {
        maxX = col * side;
        maxZ = row * side;
        maxY = side;
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;

        if (currentPosition.x < maxX && currentPosition.x >= 0
            && currentPosition.z < maxZ && currentPosition.z >= 0
            && currentPosition.y < maxY && currentPosition.y >= 0)
        {
            Debug.Log("Sector [" + (Mathf.Floor(currentPosition.x / side) + 1) + "][" + (Mathf.Floor(currentPosition.z / side) + 1) + "]");
        }
        else 
        {
            Debug.Log("Block");
        }
    }

    void LateUpdate()
    {
        Vector3 currentPosition = transform.position;
        currentPosition.x = Mathf.Clamp(currentPosition.x, 0, maxX);
        currentPosition.y = Mathf.Clamp(currentPosition.y, 0, maxY);
        currentPosition.z = Mathf.Clamp(currentPosition.z, 0, maxZ);
        transform.position = currentPosition;
    }
}
