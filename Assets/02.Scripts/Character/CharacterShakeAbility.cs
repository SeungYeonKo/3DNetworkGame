using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterShakeAbility : CharacterAbility
{
    public Transform TargetTransform;

    public float Duration = 0.5f;
    public float Strength = 0.2f;

    public void Shake()
    {
        StopAllCoroutines();
        StartCoroutine(Shake_Coroutine());
    }

    private IEnumerator Shake_Coroutine()
    {
        float elapsedTime = 0;
        Vector3 startPosition = TargetTransform.localPosition;

        while (elapsedTime <= Duration)
        {
            elapsedTime += Time.deltaTime;
            TargetTransform.localPosition += Random.insideUnitSphere.normalized * Strength;
            yield return null;
        }
        TargetTransform.localPosition = startPosition;
    }
}

