using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PieceMovementTestScript
{
    [UnityTest]
    public IEnumerator MovePiece_Pawn_MoveTwoSteps()
    {
        // Arrange
        var bc = new GameObject().AddComponent<BoardController>();   
        var pawn = bc.InstantiatePiece(new GameObject().AddComponent<Pawn>(), 8);

        // Act
        List<int> listOfAvailableMoves = pawn.GetAvailableMoves();

        // Assert
        Assert.AreEqual(listOfAvailableMoves, new List<int>() { 16, 24 });

        yield return null;
    }
}
