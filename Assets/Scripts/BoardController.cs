using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
	[SerializeField] private Piece[] positions;
	[SerializeField] private GameObject[] blueHighlight;
	[SerializeField] private GameObject highlightSquare;
    void Start()
    {
		InstantiatePieces();
    }

    private void InstantiatePieces()
	{
		blueHighlight = new GameObject[64];

		for (int i = 0; i < 64; i++)
		{
			int x = i % 8;
			int y = i / 8;

			blueHighlight[i] = Instantiate(highlightSquare, new Vector3(x, y, 0), Quaternion.identity);
			blueHighlight[i].SetActive(false);

			if (positions[i] != null)
			{
                Piece currPiece = Instantiate(positions[i], new Vector3(x, y, 0), Quaternion.identity);
				currPiece.SetCoords(x, y);
			}
		}
	}

	private void Update()
	{
		
	}

    public bool IsLegalMove(int x, int y) 
	{
        int pos = y * 8 + x;
        if (positions[pos] != null || !IsInBounds(x, y))
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

	public void Highlight(int x, int y) 
	{
		Debug.Log(blueHighlight);
		Debug.Log("Highlight");
		int pos = ConvertToPos(x, y);
		blueHighlight[pos].SetActive(true);
		Debug.Log("Completed");
	}

	// Converts (x, y) coords to 0 - 63
	public int ConvertToPos(int x, int y)
	{
		return y * 8 + x;
	}

	public int[] ConvertToXY(int pos)
	{
		return new int[] { pos % 8, pos / 8 };
	}
}
