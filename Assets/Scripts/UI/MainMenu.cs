using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Image blackImage;
    public void DoFadeToBlack()
    {
        StartCoroutine(FadeToBlack());
    }

    private IEnumerator FadeToBlack()
    {
        float fadeSpeed = 2.0f;
        float a = blackImage.color.a;
        Color fadeColor = blackImage.color;
        
        while(a < 1.0f)
        {
            a += fadeSpeed * Time.deltaTime;
            fadeColor.a = a;
            blackImage.color = fadeColor;
            yield return new WaitForEndOfFrame();
        }
        blackImage.color = fadeColor;
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Playground");
    }
}
