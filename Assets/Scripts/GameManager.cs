using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class GameManager : MonoBehaviour
{
    [Header("Reels")]
    public Reel[] reels;

    [Header("UI Button")]
    public Button spinButton;  

    [Header("UI Elements")]
    public TMP_Text coinText;   
    public TMP_Text resultText; 

    [Header("Player Coins")]
    public int playerCoins = 100; 
    public int betAmount = 10;    

    [Header("Multipliers")]
    public int normalMultiplier = 3;   
    public int jackpotMultiplier = 10; 

    [Header("Jackpot Symbol")]
    public Sprite jackpotSymbol;       

    private bool isSpinning = false;   


    

    [ContextMenu("Force Jackpot")] 



    public void ForceJackpotButton()
    {
        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].SetFinalSymbol(jackpotSymbol); 
        }
    
        CheckWin(); 
    }
    

    private void Start()
    {
        UpdateCoinUI();
        UpdateResultUI(""); 
    }

    

   
    public void Spin()
    {
        if (isSpinning)
        {
            
            return;
        }

        if (playerCoins < betAmount)
        {
           
            return;
        }
   
       
        playerCoins -= betAmount;
        UpdateCoinUI();
        UpdateResultUI(""); 
       

       
        isSpinning = true;
        if (spinButton != null)
            spinButton.interactable = false;

        StartCoroutine(SpinAllReels());
    }


    



    private IEnumerator SpinAllReels()
    {
        float baseDuration = 2.0f;
        float staggerDelay = 0.5f;

        
        Coroutine[] spinCoroutines = new Coroutine[reels.Length];
        for (int i = 0; i < reels.Length; i++)
        {
            spinCoroutines[i] = StartCoroutine(reels[i].SpinReel(baseDuration + (i * staggerDelay)));
        }

       
        for (int i = 0; i < spinCoroutines.Length; i++)
            yield return spinCoroutines[i];

        
        CheckWin();

     
        isSpinning = false;
        if (spinButton != null)
            spinButton.interactable = true;
    }








    private void CheckWin()
    {
       
        string firstName = reels[0].GetFinalSymbol().name;
        bool allMatch = true;

        foreach (var reel in reels)
        {
            if (reel.GetFinalSymbol().name != firstName)
            {
                allMatch = false;
                break;
            }
        }

        if (!allMatch)
        {
            UpdateResultUI("Try Again!");
          
            return;
        }

        Sprite firstSymbol = reels[0].GetFinalSymbol(); 
        int multiplier = (firstSymbol == jackpotSymbol) ? jackpotMultiplier : normalMultiplier;
        int payout = betAmount * multiplier;
        playerCoins += payout;
        UpdateCoinUI();

        if (multiplier == jackpotMultiplier)
            UpdateResultUI($" JACKPOT! Coins Won: {payout}");
        else
            UpdateResultUI($" You Won: {payout}");

        

        
        
        bool isJackpot = (firstSymbol == jackpotSymbol);
        StartCoroutine(HighlightWinningSymbols(isJackpot));

    }



















    
    private IEnumerator HighlightWinningSymbols(bool isJackpot = false)
    {
        int repeatCount = isJackpot ? 3 : 1; 

        for (int r = 0; r < repeatCount; r++)
        {
            foreach (var reel in reels)
            {
                Vector3 originalScale = reel.symbolImage.transform.localScale;
                reel.symbolImage.transform.localScale = originalScale * 1.2f;
            }

            yield return new WaitForSeconds(0.5f);

            foreach (var reel in reels)
            {
                reel.symbolImage.transform.localScale = Vector3.one; 
            }

            if (isJackpot)
                yield return new WaitForSeconds(0.2f); 
        }
    }














    
    private void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = "$ -  " + playerCoins;
    }

   
    private void UpdateResultUI(string message)
    {
        if (resultText != null)
            resultText.text = message;
    }
}
