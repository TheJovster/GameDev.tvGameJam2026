using System.Collections;
using UnityEngine;

public class FlashComponent : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private GameObject modelToFlash;

    [Header("Flash Settings")]
    [SerializeField] private float flashDuration = 2f;
    [SerializeField] private float flashInterval = 0.1f;

    private Coroutine flashRoutine;

    public void TriggerFlash()
    {
        if (modelToFlash == null)
            return;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            modelToFlash.SetActive(!modelToFlash.activeSelf);

            yield return new WaitForSeconds(flashInterval);

            elapsed += flashInterval;
        }

        modelToFlash.SetActive(true);
        flashRoutine = null;
    }
}