using System.Collections.Generic;
using UnityEngine;

public class RandomGhost : AbstractGhost
{
    Vector2Int lastPos;

    protected override Vector2Int ChooseDirection()
    {
        List<Vector2Int> possibleDirections = MapController.instance.GetPossiblePaths(pos.x, pos.y);

        possibleDirections.Remove(lastPos);

        Vector2Int nextPos = possibleDirections[Random.Range(0, possibleDirections.Count)];

        if (nextPos == null)
        {
            return lastPos;
        }
        
        return nextPos;
    }
}
