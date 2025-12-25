using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    public static PlayerTeleport Instance;

    [SerializeField] private Transform player;
    [SerializeField] private GridSystem gridSystem;

    private void Awake()
    {
        Instance = this;
    }

    public void Teleport(Vector2Int gridPos)
    {
        float side = gridSystem.Side;
        Vector3 worldPosition = new Vector3(
            gridPos.x * side,
            player.position.y,
            gridPos.y * side
        );

        player.position = worldPosition;
    }
}
