using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPromotable
{ 
	bool IsAvailableForPromotion();
	void ShowPromotions();
	void Promote(Piece newPiece);
}
