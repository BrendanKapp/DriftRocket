using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    private void OnEnable()
    {
        StopCoroutine("Resize");
        StartCoroutine("Resize");
    }
    private IEnumerator Resize ()
    {
        for (int scale = 300; scale > 0; scale -= 10)
        {
            transform.localScale = new Vector3(1, 1, 0) * scale + Vector3.one;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
