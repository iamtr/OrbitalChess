using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
	[SerializeField] private Piece[] positions;
	[SerializeField] private HighlightSquare[] blueHighlight;
	[SerializeField] private HighlightSquare highlightSquare;
	[SerializeField] private Piece currPiece;

	void Start()
    {
		InstantiatePieces();
    }

	private void InstantiatePieces()
	{
		blueHighlight = new HighlightSquare[64];

		for (int i = 0; i < 64; i++)
		{
			int x = i % 8;
			int y = i / 8;

			blueHighlight[i] = Instantiate(highlightSquare, new Vector3(x, y, 0), Quaternion.identity);
			blueHighlight[i].Position = i;
			blueHighlight[i].transform.parent = GameObject.Find("Highlight Squares").transform;
			blueHighlight[i].gameObject.SetActive(false);

			if (positions[i] != null)
			{
                Piece currPiece = Instantiate(positions[i], new Vector3(x, y, 0), Quaternion.identity);
				currPiece.transform.parent = GameObject.Find("Pieces").transform;
				currPiece.SetCoords(x, y);
			}
		}
	}

    public bool IsLegalMove(int x, int y) 
	{
        int pos = y * 8 + x;
        if (!IsInBounds(x, y) || positions[pos] != null)
		{
            // change based on black or white;
            return false;
		} 

        return true;
	}

	public bool IsInBounds(int x, int y)
	{
		if (x < 0 || x > 7 || y < 0 || y > 7)
		{
			return false;
		}

		return true;
	}

	public void Highlight(int x, int y, Piece currPiece) 
	{
		this.currPiece = currPiece;
		int pos = ConvertToPos(x, y);
		blueHighlight[pos].gameObject.SetActive(true);
	}

	public void MovePiece(int x, int y, Piece piece)
	{
		int pos = ConvertToPos(x, y);
		positions[piece.CurrPos] = null;
		piece.SetCoords(x, y);
		positions[pos] = piece;
	}

	public void UnhighlightAllSqaures()
	{
		foreach(var square in blueHighlight)
		{
			square.gameObject.SetActive(false);
		}
	}

	// Converts (x, y) coords to 0 - 63
	public int ConvertToPos(int x, int y)
	{
		return y * 8 + x;
	}

	// Converts 0 - 63 to (x, y)
	public int[] ConvertToXY(int pos)
	{
		return new int[] { pos % 8, pos / 8 };
	}

	public void HandleColliderClicked(Collider2D collider)
	{
		if (collider.gameObject.tag == "Highlight Square")
		{
			HighlightSquare h = collider.GetComponent<HighlightSquare>();
			int[] temp = ConvertToXY(h.Position);
			MovePiece(temp[0], temp[1], currPiece);
			UnhighlightAllSqaures();
		}

		if (collider.gameObject.tag == "Piece")
		{
			UnhighlightAllSqaures();
			currPiece = collider.GetComponent<Piece>();
			currPiece.GetAvailableMoves();
		}
	}
}
