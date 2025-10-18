using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Reel2 : MonoBehaviour
{
    [Header("Setup")]
    public RectTransform reelMask;
    public Image[] symbolSlots;
    public Sprite[] symbols;
    public float symbolHeight = 100f;      
    public float spinSpeed = 2f;            
    public int slowDownSteps = 80;         

    private bool isSpinning = false;
    private Sprite finalSymbol;
    private bool forceResult = false;
    private Sprite forcedSymbol;

   
    private float[] validPositions = { -200f, -100f, 0f, 100f, 200f };

    private void Start()
    {


      
        for (int i = 0; i < symbolSlots.Length; i++)
        {
            symbolSlots[i].sprite = symbols[Random.Range(0, symbols.Length)];
            symbolSlots[i].rectTransform.anchoredPosition = new Vector2(0, -i * symbolHeight);
        }
    }

    public IEnumerator SpinReel(float spinDuration)
    {
        isSpinning = true;
        float elapsed = 0f;

       
        float highSpeed = spinSpeed * 10f;

        while (elapsed < spinDuration)
        {
            ScrollSymbols(highSpeed); 
            elapsed += Time.deltaTime;
            yield return null;
        }

        
        if (forceResult)
        {
            finalSymbol = forcedSymbol;
            CenterFinalSymbol(forcedSymbol);
            forceResult = false;
        }
        else
        {
            finalSymbol = symbols[Random.Range(0, symbols.Length)];
            CenterFinalSymbol(finalSymbol);
        }

        isSpinning = false;
    }


    private void ScrollSymbols(float speed)
    {
        float moveAmount = speed * symbolHeight * Time.deltaTime;
        ScrollSymbolsCustom(moveAmount);
    }

   
    private void ScrollSymbolsCustom(float moveAmount)
    {
        foreach (var slot in symbolSlots)
        {
            Vector2 pos = slot.rectTransform.anchoredPosition;
            pos.y -= moveAmount;

            
            if (pos.y < -symbolHeight * (symbolSlots.Length - 1))
            {
                float topY = GetHighestY() + symbolHeight;
                pos.y = topY;
                slot.sprite = symbols[Random.Range(0, symbols.Length)];
            }

            
            if (!isSpinning)
                pos.y = GetClosestValidY(pos.y);

            slot.rectTransform.anchoredPosition = pos;
        }
    }

    private float GetHighestY()
    {
        float highest = float.MinValue;
        foreach (var slot in symbolSlots)
        {
            if (slot.rectTransform.anchoredPosition.y > highest)
                highest = slot.rectTransform.anchoredPosition.y;
        }
        return highest;
    }

    private float GetClosestValidY(float y)
    {
        float closest = validPositions[0];
        float minDist = Mathf.Abs(y - validPositions[0]);

        for (int i = 1; i < validPositions.Length; i++)
        {
            float dist = Mathf.Abs(y - validPositions[i]);
            if (dist < minDist)
            {
                minDist = dist;
                closest = validPositions[i];
            }
        }

        return closest;
    }

    private void CenterFinalSymbol(Sprite symbol)
    {
        symbolSlots[1].sprite = symbol; 

       
        float centerY = 0f;

        for (int i = 0; i < symbolSlots.Length; i++)
        {
            float offset = (i - 1) * symbolHeight; 
            symbolSlots[i].rectTransform.anchoredPosition = new Vector2(0, centerY + offset);
        }

        finalSymbol = symbol;
    }

    public Sprite GetFinalSymbol()
    {
        return finalSymbol;
    }

    public void ForceFinalSymbol(Sprite symbol)
    {
        forcedSymbol = symbol;
        forceResult = true;
    }

    public void Highlight()
    {
        symbolSlots[1].rectTransform.localScale = Vector3.one * 1.2f;
    }

    public void ResetHighlight()
    {
        symbolSlots[1].rectTransform.localScale = Vector3.one;
    }

    public bool IsSpinning()
    {
        return isSpinning;
    }
}
