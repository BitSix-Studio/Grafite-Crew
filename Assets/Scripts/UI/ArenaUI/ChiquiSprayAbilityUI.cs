using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChiquiSprayAbilityUI : MonoBehaviour
{
    public static ChiquiSprayAbilityUI Instance;

    [SerializeField] private GameObject imageObject;

    private void Awake()
    {
        Instance = this;

        imageObject.SetActive(false);
    }

    public void ShowEffect(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(ShowRoutine(duration));
    }

    private IEnumerator ShowRoutine(float duration)
    {
        imageObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        imageObject.SetActive(false);
    }
}
