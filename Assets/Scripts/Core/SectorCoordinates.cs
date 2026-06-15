using UnityEngine;

public static class SectorCoordinates
{
    public static Vector2Int LabelToGrid(char letter, int number) =>
        new Vector2Int(number - 1, letter - 'A');

    public static Vector2Int LabelToGrid(string label)
    {
        if (string.IsNullOrEmpty(label) || label.Length < 2)
            return Vector2Int.zero;
        return LabelToGrid(label[0], label[1] - '0' + (label.Length > 2 ? int.Parse(label.Substring(1)) : label[1] - '0'));
    }

    public static string GridToLabel(Vector2Int grid) =>
        $"{(char)('A' + grid.y)}{grid.x + 1}";
}
