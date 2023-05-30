using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        var dummyPawn = new GameObject().AddComponent<Pawn>();
        bc.InitBoardForTesting();
		Piece pawn = bc.InstantiatePiece(new GameObject().AddComponent<Pawn>(), 8);
        pawn.InitPieceForTesting(bc);
        var expected = new List<int> { 16, 24 };

        // Act
        List<int> listOfAvailableMoves = pawn.GetAvailableMoves();

        // Assert
        // Assert.IsTrue(listOfAvailableMoves.All(expected.Contains));
        Assert.AreEqual(listOfAvailableMoves, expected);

        yield return null;
    }
}
