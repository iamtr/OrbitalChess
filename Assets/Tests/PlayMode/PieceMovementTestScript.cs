using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PieceMovementTestScript
{
    [UnityTest]
    public IEnumerator Piece_TestMovement()
    {
        var board = new GameObject().AddComponent<BoardController>();   

        yield return null;
    }
}
