using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningsBoardController : BoardController
{
    [SerializeField] private GameObject piecesGameObject;
    public override void Start()
    {
        base.Start();
        piecesGameObject.gameObject.SetActive(false);
    }
}
