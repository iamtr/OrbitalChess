using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
	[SerializeField] private Piece[] positions;
	[SerializeField] private HighlightSquare[] highlights;
	[SerializeField] private HighlightSquare highlightSquare;
	[SerializeField] private Piece currPiece;

	private GameController gc;

	void Start()
    {
		gc = GameObject.Find("Game Controller").GetComponent<GameController>();
		InstantiatePieces();
    }

	private void InstantiatePieces()
	{
		highlights = new HighlightSquare[64];

		for (int i = 0; i < 64; i++)
		{
			int x = i % 8;
			int y = i / 8;

			highlights[i] = Instantiate(highlightSquare, new Vector3(x, y, 0), Quaternion.identity);
			highlights[i].Position = i;
			highlights[i].transform.parent = GameObject.Find("Highlight Squares").transform;
			highlights[i].gameObject.SetActive(false);

			if (positions[i] != null)
			{
				Piece temp = Instantiate(positions[i], new Vector3(x, y, 0), Quaternion.identity);
				positions[i] = temp;
				temp.transform.parent = GameObject.Find("Pieces").transform;
				temp.SetCoords(x, y);
			}
		}
	}

    public bool IsLegalMove(int x, int y, Piece p) 
	{
        int pos = y * 8 + x;
        if (!IsInBounds(x, y) || positions[pos]?.Player == p.Player)
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

		if (positions[pos] == null)
		{
			highlights[pos].GetComponent<SpriteRenderer>().color = Color.blue;
		}
		else if (positions[pos]?.Player != currPiece.Player)
		{
			highlights[pos].GetComponent<SpriteRenderer>().color = Color.red;
		}

		highlights[pos].gameObject.SetActive(true);
	}

	public void MovePiece(int x, int y, Piece piece)
	{
		int pos = ConvertToPos(x, y);
		positions[piece.CurrPos] = null;
		piece.SetCoords(x, y);
		if (positions[pos] != null && positions[pos].Player != piece.Player)
		{
			Destroy(positions[pos].gameObject);
		}
		positions[pos] = piece;
		positions[pos].OnMove();
		gc.RoundEnd();
	}

	public void UnhighlightAllSqaures()
	{
		foreach(var square in highlights)
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
		if (collider.gameObject.CompareTag("Highlight Square"))
		{
			HighlightSquare h = collider.GetComponent<HighlightSquare>();
			int[] temp = ConvertToXY(h.Position);
			MovePiece(temp[0], temp[1], currPiece);
			UnhighlightAllSqaures();
		}

		if (collider.gameObject.CompareTag("Piece") && collider.GetComponent<Piece>().Player == gc.CurrPlayer)
		{
			UnhighlightAllSqaures();
			currPiece = collider.GetComponent<Piece>();
			currPiece.GetAvailableMoves();
		}
	}

	public bool IsSamePlayer(int pos1, int pos2)
	{
		Piece p1 = GetPieceFromPos(pos1);
		Piece p2 = GetPieceFromPos(pos2);
		if (p1 == null || p2 == null) { return false; }
		return p1.Player == p2.Player;
	}

	public Piece GetPieceFromPos(int pos)
	{
		return positions[pos];
	}

	public bool IsOccupied(int pos)
	{
		return positions[pos] != null;
	}
}
