using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRoll : MonoBehaviour
{
    public IEnumerable RollDiceCoroutine()
    {
        Animation rollDiceAnimation = this.GetComponent<Animation>();
        rollDiceAnimation.Play();

        yield return new WaitForSeconds(rollDiceAnimation.clip.length);

        Destroy(this);
    }

    public void Roll()
    {
        StartCoroutine("RollDiceCoroutine");
    }
}
