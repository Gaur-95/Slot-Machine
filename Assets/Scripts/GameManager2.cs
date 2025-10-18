using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager2 : MonoBehaviour
{
    [Header("Start Screen")]
    public GameObject startScreen;
    public TMP_InputField player_Name_input;
    public TMP_InputField bet_amount_input;
    public Button Play_Button;
    public GameObject gameUI;       

    [Header("Reels")]
    public Reel2[] reels;

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


    public GameObject retryButton;

    
   



    [Header("Cash Out / Withdraw")]
    public Button withdrawButton;
    public GameObject cashOutScreen;    
    public TMP_Text cashOutMessage;     


    [Header("Audio")]
    public AudioSource backgroundMusicSource; 
    public AudioClip backgroundMusicClip;    

    public AudioClip spinButtonClickClip;     
    public AudioClip spinningClip;            
    public AudioClip winClip;                 
    public AudioClip loseClip;                
    public AudioClip withdrawClip;            

    private AudioSource sfxSource;            


    



    [Header("Player Info UI")]
    public TMP_Text playerNameText;  
    private string playerName = "Player"; 






    [ContextMenu("Force Jackpot")]
    public void ForceJackpotButton()
    {
        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].ForceFinalSymbol(jackpotSymbol);
        }

        CheckWin();
    }

    private void Start()
    {
        
        startScreen.SetActive(true);
        if (coinText != null) coinText.text = "$ - 0";
        if (resultText != null) resultText.text = "";

        
        Play_Button.onClick.AddListener(StartGame);



        UpdateCoinUI();
        UpdateResultUI("");

      
        if (backgroundMusicSource != null && backgroundMusicClip != null)
        {
            backgroundMusicSource.clip = backgroundMusicClip;
            backgroundMusicSource.loop = true;
            backgroundMusicSource.Play();
        }

        
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;




    }



    public void Retryoption()
    {
        SceneManager.LoadScene(0);


    }













    private void StartGame()
    {
       

        


    

        
        if (!string.IsNullOrEmpty(player_Name_input.text))
            playerName = player_Name_input.text;

     
        if (!int.TryParse(bet_amount_input.text, out playerCoins))
            playerCoins = 100; 

    
        startScreen.SetActive(false);

       
        UpdateCoinUI();
        UpdateResultUI("");
        UpdatePlayerNameUI();







    }




    private void UpdatePlayerNameUI()
    {
        if (playerNameText != null)
            playerNameText.text = playerName;
    }





    public void Spin()
    {
       

        if (isSpinning) return;
        if (playerCoins < betAmount) return;

       
        if (sfxSource != null && spinButtonClickClip != null)
            sfxSource.PlayOneShot(spinButtonClickClip);

        playerCoins -= betAmount;
        UpdateCoinUI();
        UpdateResultUI("");

        isSpinning = true;
        if (spinButton != null) spinButton.interactable = false;

       
        if (sfxSource != null && spinningClip != null)
            sfxSource.PlayOneShot(spinningClip);

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
        if (spinButton != null) spinButton.interactable = true;
    }





    






    private void CheckWin()
    {
        Sprite centerSymbol = null;
        bool allMatch = true;

        for (int i = 0; i < reels.Length; i++)
        {
            Reel2 reel = reels[i];
            Sprite y0Symbol = GetSymbolAtY0(reel); 

            if (i == 0)
            {
                centerSymbol = y0Symbol; 
            }
            else
            {
                if (y0Symbol != centerSymbol)
                {
                    allMatch = false;
                }
            }
        }

        if (!allMatch)
        {
            UpdateResultUI("Try Again!");
            if (sfxSource != null && loseClip != null)
                sfxSource.PlayOneShot(loseClip);

            return;
        }

        int multiplier = (centerSymbol == jackpotSymbol) ? jackpotMultiplier : normalMultiplier;
        int payout = betAmount * multiplier;
        playerCoins += payout;
        UpdateCoinUI();

        if (multiplier == jackpotMultiplier)
            UpdateResultUI($"JACKPOT! Coins Won: {payout}");
        else
            UpdateResultUI($"You Won: {payout}");


        if (sfxSource != null && winClip != null)
            sfxSource.PlayOneShot(winClip);

        bool isJackpot = (centerSymbol == jackpotSymbol);
        StartCoroutine(HighlightWinningSymbols(isJackpot));


        


    }











    public void Withdraw()
    {
      
        isSpinning = true;
        if (spinButton != null) spinButton.interactable = false;
        if (withdrawButton != null) withdrawButton.interactable = false;

       
        if (sfxSource != null && withdrawClip != null)
            sfxSource.PlayOneShot(withdrawClip);

        
        if (cashOutScreen != null) cashOutScreen.SetActive(true);

        if (cashOutMessage != null)
            cashOutMessage.text = $"You Cashed Out: ${playerCoins}";
    }









    public void CloseCashOut()
    {
       
        if (cashOutScreen != null) cashOutScreen.SetActive(false);

        
        if (startScreen != null) startScreen.SetActive(true);
        if (gameUI != null) gameUI.SetActive(false);

        playerCoins = 0;  
        UpdateCoinUI();
        UpdateResultUI("");

       
        isSpinning = false;
        if (spinButton != null) spinButton.interactable = true;
        if (withdrawButton != null) withdrawButton.interactable = true;
    }










    private Sprite GetSymbolAtY0(Reel2 reel)
    {
        foreach (var slot in reel.symbolSlots)
        {
            float y = slot.rectTransform.anchoredPosition.y;
            if (Mathf.Approximately(y, 0f)) 
            {
                return slot.sprite;
            }
        }

        
        Sprite closest = reel.symbolSlots[0].sprite;
        float minDist = Mathf.Abs(reel.symbolSlots[0].rectTransform.anchoredPosition.y);
        foreach (var slot in reel.symbolSlots)
        {
            float dist = Mathf.Abs(slot.rectTransform.anchoredPosition.y);
            if (dist < minDist)
            {
                minDist = dist;
                closest = slot.sprite;
            }
        }
        return closest;
    }

    private IEnumerator HighlightWinningSymbols(bool isJackpot = false)
    {
        int repeatCount = isJackpot ? 3 : 1;

        for (int r = 0; r < repeatCount; r++)
        {
            foreach (var reel in reels)
            {
                reel.Highlight();
            }

            yield return new WaitForSeconds(0.5f);

            foreach (var reel in reels)
            {
                reel.ResetHighlight();
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
