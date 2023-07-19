using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeCloud : MonoBehaviour
{
    public IEnumerable SmokeCloudCoroutine()
    {
        Animation smokeCloudAnimation = this.GetComponent<Animation>();
        smokeCloudAnimation.Play();

        yield return new WaitForSeconds(smokeCloudAnimation.clip.length);

        Destroy(this);
    }

    public void StartSmokeCloud()
    {
        StartCoroutine("SmokeCloudCoroutine");
    }
}
