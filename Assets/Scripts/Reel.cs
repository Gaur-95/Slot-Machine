using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Reel : MonoBehaviour
{
    public Image symbolImage;       
    public Sprite[] symbols;       
    public float spinSpeed = 0.05f; 
    private bool isSpinning = false;

    private Sprite finalSymbol;     



    public void SetFinalSymbol(Sprite symbol)
    {
        finalSymbol = symbol;
        symbolImage.sprite = finalSymbol; 
    }

    

    
    public IEnumerator SpinReel(float spinDuration)
    {
        isSpinning = true;
        float elapsed = 0f;

        while (elapsed < spinDuration)
        {
            symbolImage.sprite = symbols[Random.Range(0, symbols.Length)];
            yield return new WaitForSeconds(spinSpeed);
            elapsed += spinSpeed;
        }


        finalSymbol = symbols[Random.Range(0, symbols.Length)];
        symbolImage.sprite = finalSymbol;

        isSpinning = false;
    }

    
    public Sprite GetFinalSymbol()
    {
        return finalSymbol;
    }

    
    public bool IsSpinning()
    {
        return isSpinning;
    }
}
