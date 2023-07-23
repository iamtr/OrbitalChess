using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MultiplayerUIManager : UIManager
{
    protected override void Start()
    {
        base.Start();

        foreach (PromotionButton sprite in promotingWhite)
        {
            sprite.gameObject.transform.eulerAngles = new Vector3(0, 0, 180);
        }
    }
}
