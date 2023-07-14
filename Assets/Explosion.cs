using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public IEnumerable ExplodeCoroutine()
    {
        Animation explosionAnimation = this.GetComponent<Animation>();
        explosionAnimation.Play();

        yield return new WaitForSeconds(explosionAnimation.clip.length);

        Destroy(this);
    }

    public void Explode()
    {
        StartCoroutine("ExplodeCoroutine");
    }
}
