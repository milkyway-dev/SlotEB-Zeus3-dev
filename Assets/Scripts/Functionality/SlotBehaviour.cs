using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;

public class SlotBehaviour : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField]
    private Sprite[] myImages;  //images taken initially
    [SerializeField]
    private Sprite[] KTRImages;  
    [SerializeField]
    private List<Sprite> Box_Sprites;

    [Header("Slot Images")]
    [SerializeField]
    private List<SlotImage> images;     //class to store total images

    [Header("Slots Transforms")]
    [SerializeField]
    private Transform[] Slot_Transform;
    [SerializeField]
    private Transform[] KTRSlot_Transform;
    [SerializeField]
    private double[] stopIndices;

    private Dictionary<int, string> y_string = new Dictionary<int, string>();

    [Header("Animated Sprites")]
    [SerializeField]
    private Sprite[] Ship_Sprite;
    [SerializeField]
    private Sprite[] Coin_Sprite;
    [SerializeField]
    private Sprite[] Wild_Sprite;
    [SerializeField]
    private Sprite[] Feature_Sprite;

    [Header("Miscellaneous UI")]
    [SerializeField]
    private TMP_Text Balance_text;
    [SerializeField]
    private TMP_Text TotalBet_text;
    [SerializeField]
    private TMP_Text LineBet_text;
    [SerializeField]
    private TMP_Text TotalWin_text;

    [Header("Audio Management")]
    [SerializeField]
    private AudioController audioController;

    [SerializeField]
    private UIManager uiManager;

    [Header("BonusGame Popup")]
    [SerializeField]
    private BonusController _bonusManager;

    [SerializeField]
    int tweenHeight = 2700;  //calculate the height at which tweening is done

    [SerializeField]
    private PayoutCalculation PayCalculator;

    private List<Tweener> alltweens = new List<Tweener>();

    private Tweener WinTween = null;

    [SerializeField]
    private List<ImageAnimation> TempList;  //stores the sprites whose animation is running at present 

    [SerializeField]
    private SocketIOManager SocketManager;

    private Coroutine tweenroutine;

    internal bool IsFreeSpin = false;
    private bool IsSpinning = false;
    private bool CheckSpinAudio = false;
    internal bool CheckPopups = false;

    private int BetCounter = 0;
    private int FreeSpinCounter = 0;
    private double currentBalance = 0;
    private double currentTotalBet = 0;
    protected int Lines = 20;
    [SerializeField]
    private int IconSizeFactor = 100;       //set this parameter according to the size of the icon and spacing
    private int numberOfSlots = 6;          //number of columns


    private void Start()
    {

    }

    private void CompareBalance()
    {
        if (currentBalance < currentTotalBet)
        {
            uiManager.LowBalPopup();
            if (uiManager) uiManager.ToggleStartButton(false);
        }
        else
        {
            if (uiManager) uiManager.ToggleStartButton(true);
        }
    }

    #region LinesCalculation
    //Fetch Lines from backend
    internal void FetchLines(string LineVal, int count)
    {
        y_string.Add(count, LineVal);
    }

    #endregion


    internal void ChangeBet(bool IncDec)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (IncDec)
        {
            if (BetCounter < SocketManager.initialData.Bets.Count - 1)
            {
                BetCounter++;
            }
            else
            {
                BetCounter = 0;
            }
        }
        else
        {
            if (BetCounter > 0)
            {
                BetCounter--;
            }
            else
            {
                BetCounter = SocketManager.initialData.Bets.Count - 1;
            }
        }
        if (LineBet_text) LineBet_text.text = SocketManager.initialData.Bets[BetCounter].ToString();
        if (TotalBet_text) TotalBet_text.text = (SocketManager.initialData.Bets[BetCounter] * Lines).ToString();
        currentTotalBet = SocketManager.initialData.Bets[BetCounter] * Lines;
        CompareBalance();
    }

    internal void ToggleTotalBet(bool IncDec)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (IncDec)
        {
            if (BetCounter < SocketManager.initialData.Bets.Count - 1)
            {
                BetCounter++;
            }
            else
            {
                BetCounter = 0;
            }
        }
        else
        {
            if (BetCounter > 0)
            {
                BetCounter--;
            }
            else
            {
                BetCounter = SocketManager.initialData.Bets.Count - 1;
            }
        }
        if (LineBet_text) LineBet_text.text = SocketManager.initialData.Bets[BetCounter].ToString();
        if (TotalBet_text) TotalBet_text.text = (SocketManager.initialData.Bets[BetCounter] * Lines).ToString("f2");
        currentTotalBet = SocketManager.initialData.Bets[BetCounter] * Lines;
        CompareBalance();
    }

    #region InitialFunctions
    internal void shuffleInitialMatrix()
    {
        for (int i = 0; i < images.Count; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                int randomIndex = UnityEngine.Random.Range(0, 11);
                images[i].slotImages[j].transform.GetChild(1).GetComponent<Image>().sprite = myImages[randomIndex];
                if (randomIndex == 5)
                {
                    images[i].slotImages[j].transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    images[i].slotImages[j].transform.GetChild(0).gameObject.SetActive(false);
                }
                if (randomIndex == 0 || randomIndex == 10)
                {
                    images[i].slotImages[j].transform.GetChild(1).GetComponent<RectTransform>().offsetMin = new Vector2(-60, -60);
                    images[i].slotImages[j].transform.GetChild(1).GetComponent<RectTransform>().offsetMax = new Vector2(60, 60);
                    images[i].slotImages[j].transform.SetAsLastSibling();
                }
                else if (randomIndex == 11)
                {
                    images[i].slotImages[j].transform.GetChild(1).GetComponent<RectTransform>().offsetMin = new Vector2(-79, -100);
                    images[i].slotImages[j].transform.GetChild(1).GetComponent<RectTransform>().offsetMax = new Vector2(121, 100);
                    images[i].slotImages[j].transform.SetAsLastSibling();
                }
                else
                {
                    images[i].slotImages[j].transform.GetChild(1).GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
                    images[i].slotImages[j].transform.GetChild(1).GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
                }
            }
        }
    }

    internal void SetInitialUI()
    {
        BetCounter = 0;
        if (LineBet_text) LineBet_text.text = SocketManager.initialData.Bets[BetCounter].ToString();
        if (TotalBet_text) TotalBet_text.text = (SocketManager.initialData.Bets[BetCounter] * Lines).ToString();
        if (TotalWin_text) TotalWin_text.text = "0.00";
        if (Balance_text) Balance_text.text = SocketManager.playerdata.Balance.ToString("f2");
        currentBalance = SocketManager.playerdata.Balance;
        currentTotalBet = SocketManager.initialData.Bets[BetCounter] * Lines;
        CompareBalance();
        uiManager.InitialiseUIData(SocketManager.initUIData.paylines);
    }
    #endregion

    private void OnApplicationFocus(bool focus)
    {
        audioController.CheckFocusFunction(focus, CheckSpinAudio);
    }

    //function to populate animation sprites accordingly
    private void PopulateAnimationSprites(ImageAnimation animScript, int val)
    {
        animScript.textureArray.Clear();
        animScript.textureArray.TrimExcess();
        animScript.AnimationSpeed = 25f;
        switch (val)
        {
            case 2:
                for (int i = 0; i < Ship_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Ship_Sprite[i]);
                }
                animScript.AnimationSpeed = 110f;
                break;
            case 5:
                for (int i = 0; i < Coin_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Coin_Sprite[i]);
                }
                animScript.AnimationSpeed = 25f;
                break;
            case 10:
                for (int i = 0; i < Wild_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Wild_Sprite[i]);
                }
                animScript.AnimationSpeed = 31f;
                break;
            case 11:
                for (int i = 0; i < Feature_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Feature_Sprite[i]);
                }
                animScript.AnimationSpeed = 20f;
                break;
        }
    }

    #region SlotSpin
    //starts the spin process
    internal void StartSlots()
    {
        if (audioController) audioController.PlaySpinButtonAudio();

        WinningsAnim(false);
        if (TempList.Count > 0)
        {
            StopGameAnimation();
        }
        PayCalculator.ResetLines();
        tweenroutine = StartCoroutine(TweenRoutine());
    }

    //manage the Routine for spinning of the slots
    private IEnumerator TweenRoutine()
    {
        if (currentBalance < currentTotalBet && !IsFreeSpin) 
        {
            CompareBalance();
            yield return new WaitForSeconds(1);
            ToggleButtonGrp(true);
            yield break;
        }
        if(IsFreeSpin)
        {
            FreeSpinCounter--;
        }
        if (audioController) audioController.PlayWLAudio("spin");
        CheckSpinAudio = true;

        IsSpinning = true;

        ToggleButtonGrp(false);

        for (int i = 0; i < numberOfSlots; i++)
        {
            if (!IsFreeSpin)
            {
                InitializeTweening(Slot_Transform[i], i);
                yield return new WaitForSeconds(0.1f);
            }
            //else
            //{
            //    InitializeTweening(KTRSlot_Transform[i]);
            //    yield return new WaitForSeconds(0.1f);
            //}
        }

        if (!IsFreeSpin)
        {
            BalanceDeduction();
        }
        SocketManager.AccumulateResult(BetCounter);

        yield return new WaitUntil(() => SocketManager.isResultdone);

        for (int j = 0; j < SocketManager.resultData.ResultReel.Count; j++)
        {
            List<int> resultnum = SocketManager.resultData.FinalResultReel[j]?.Split(',')?.Select(Int32.Parse)?.ToList();
            for (int i = 0; i < 6; i++)
            {
                if (!IsFreeSpin)
                {
                    if (images[i].slotImages[j]) images[i].slotImages[j].transform.GetChild(1).GetComponent<Image>().sprite = myImages[resultnum[i]];
                    if (resultnum[i] == 5)
                    {
                        images[i].slotImages[j].transform.GetChild(0).gameObject.SetActive(true);
                    }
                    else
                    {
                        images[i].slotImages[j].transform.GetChild(0).gameObject.SetActive(false);
                    }
                    if (resultnum[i] == 0 || resultnum[i] == 10)
                    {
                        images[i].slotImages[j].transform.GetChild(1).GetComponent<RectTransform>().offsetMin = new Vector2(-60, -60);
                        images[i].slotImages[j].transform.GetChild(1).GetComponent<RectTransform>().offsetMax = new Vector2(60, 60);
                        images[i].slotImages[j].transform.SetAsLastSibling();
                    }
                    else if (resultnum[i] == 11)
                    {
                        images[i].slotImages[j].transform.GetChild(1).GetComponent<RectTransform>().offsetMin = new Vector2(-79, -100);
                        images[i].slotImages[j].transform.GetChild(1).GetComponent<RectTransform>().offsetMax = new Vector2(121, 100);
                        images[i].slotImages[j].transform.SetAsLastSibling();
                    }
                    else
                    {
                        images[i].slotImages[j].transform.GetChild(1).GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
                        images[i].slotImages[j].transform.GetChild(1).GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
                    }
                    PopulateAnimationSprites(images[i].slotImages[j].transform.GetChild(1).GetComponent<ImageAnimation>(), resultnum[i]);
                }
            }
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < numberOfSlots; i++)
        {
            if (!IsFreeSpin)
            {
                yield return StopTweening(Slot_Transform[i], i);
            }
            //else
            //{
            //    yield return StopTweening(5, KTRSlot_Transform[i], i);
            //}

        }

        yield return new WaitForSeconds(0.3f);
        CheckPayoutLineBackend(SocketManager.resultData.linesToEmit, SocketManager.resultData.FinalsymbolsToEmit);
        KillAllTweens();

        //CheckPopups = true;


        //if(IsFreeSpin)
        //{
        //    uiManager.UpdateUI(FreeSpinCounter, SocketManager.playerdata.currentWining);
        //}

        if (Balance_text) Balance_text.text = SocketManager.playerdata.Balance.ToString("f2");

        currentBalance = SocketManager.playerdata.Balance;

        //if (SocketManager.resultData.bonusData.isBonus)
        //{
        //    CheckBonusGame();
        //}
        //else
        //{
        //    CheckWinPopups();
        //}

        //yield return new WaitUntil(() => !CheckPopups);
        //if (SocketManager.resultData.freeSpin.isNewAdded)
        //{
        //    FreeSpinCounter = (int)SocketManager.resultData.freeSpin.freeSpinCount;
        //    if (IsFreeSpin)
        //    {
        //        ToggleButtonGrp(true);
        //        uiManager.FreeSpinProcessStart((int)SocketManager.resultData.freeSpin.freeSpinCount, currentTotalBet);
        //    }
        //    else
        //    {
        //        IsFreeSpin = true;
        //        shuffleInitialMatrixKTR();
        //        if (uiManager) uiManager.ToggleBonusRText(true);
        //        if (KTRStart_Button) KTRStart_Button.gameObject.SetActive(true);
        //    }
        //}
        //else
        //{
        //    ToggleButtonGrp(true);
        //}
        //if (uiManager) uiManager.ToggleSquirrel(false);
        //if (IsFreeSpin && FreeSpinCounter <= 0)
        //{
        //    IsFreeSpin = false;
        //    uiManager.FreeSpinProcessStop();
        //    DOVirtual.DelayedCall(2f, () =>
        //    {
        //        ToggleButtonGrp(true);
        //    });
        //}
        ToggleButtonGrp(true);
        IsSpinning = false;
    }

    private void BalanceDeduction()
    {
        double bet = 0;
        double balance = 0;
        try
        {
            bet = double.Parse(TotalBet_text.text);
        }
        catch (Exception e)
        {
            Debug.Log("Error while conversion " + e.Message);
        }

        try
        {
            balance = double.Parse(Balance_text.text);
        }
        catch (Exception e)
        {
            Debug.Log("Error while conversion " + e.Message);
        }
        double initAmount = balance;

        balance = balance - bet;

        DOTween.To(() => initAmount, (val) => initAmount = val, balance, 0.8f).OnUpdate(() =>
        {
            if (Balance_text) Balance_text.text = initAmount.ToString("f2");
        });
    }

    internal void CheckWinPopups()
    {
        if (TotalWin_text) TotalWin_text.text = SocketManager.playerdata.currentWining.ToString();
        if (SocketManager.resultData.WinAmout >= currentTotalBet * 10 && SocketManager.resultData.WinAmout < currentTotalBet * 15)
        {
            uiManager.PopulateWin(1, SocketManager.resultData.WinAmout);
        }
        else if (SocketManager.resultData.WinAmout >= currentTotalBet * 15 && SocketManager.resultData.WinAmout < currentTotalBet * 20)
        {
            uiManager.PopulateWin(2, SocketManager.resultData.WinAmout);
        }
        else if (SocketManager.resultData.WinAmout >= currentTotalBet * 20)
        {
            uiManager.PopulateWin(3, SocketManager.resultData.WinAmout);
        }
        else
        {
            CheckPopups = false;
        }
    }

    internal void CheckBonusGame()
    {
        _bonusManager.StartBonus(SocketManager.tempBonus);
    }

    //generate the payout lines generated 
    private void CheckPayoutLineBackend(List<int> LineId, List<string> points_AnimString)
    {
        List<int> y_points = null;
        List<int> points_anim = null;
        if (LineId.Count > 0 || points_AnimString.Count > 0)
        {
            if (audioController) audioController.PlayWLAudio("win");
            for (int i = 0; i < LineId.Count; i++)
            {
                y_points = y_string[LineId[i]]?.Split(',')?.Select(Int32.Parse)?.ToList();
                PayCalculator.GeneratePayoutLinesBackend(y_points, y_points.Count, LineId[i] % 10);
            }

            for (int i = 0; i < points_AnimString.Count; i++)
            {
                points_anim = points_AnimString[i]?.Split(',')?.Select(Int32.Parse)?.ToList();

                for (int k = 0; k < points_anim.Count; k++)
                {
                    if (points_anim[k] >= 10)
                    {
                        StartGameAnimation(images[(points_anim[k] / 10) % 10].slotImages[points_anim[k] % 10].transform.GetChild(1).gameObject);
                    }
                    else
                    {
                        StartGameAnimation(images[0].slotImages[points_anim[k]].transform.GetChild(1).gameObject);
                    }
                }
            }

            WinningsAnim(true);
            PayCalculator.StartLinesRoutine();
        }
        else
        {
            if (audioController) audioController.StopWLAaudio();
        }
        CheckSpinAudio = false;
    }

    //private IEnumerator BoxRoutine(List<int> LineIDs, int BonusCount, int scatterCount)
    //{
    //    yield return new WaitForSeconds(2f);
    //    PayCalculator.DontDestroyLines.Clear();
    //    PayCalculator.DontDestroyLines.TrimExcess();
    //    PayCalculator.ResetLines();
    //    while (true)
    //    {
    //        List<int> y_points = null;
    //        if (LineIDs.Count > 0)
    //        {
    //            for (int i = 0; i < LineIDs.Count; i++)
    //            {
    //                y_points = y_string[LineIDs[i] + 1]?.Split(',')?.Select(Int32.Parse)?.ToList();
    //                PayCalculator.GeneratePayoutLinesBackend(y_points, y_points.Count, LineIDs[i] % 10);
    //                PayCalculator.DontDestroyLines.Add(LineIDs[i]);
    //                for (int s = 0; s < 5; s++)
    //                {
    //                    if (TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].isAnim && !TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].isBonus && !TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].isScatter)
    //                    {
    //                        TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].SetBG(Box_Sprites[LineIDs[i] % 10]);
    //                        TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].transform.parent.gameObject.SetActive(true);
    //                    }
    //                }
    //                if (LineIDs.Count < 2)
    //                {
    //                    yield break;
    //                }
    //                yield return new WaitForSeconds(2f);
    //                for (int s = 0; s < 5; s++)
    //                {
    //                    if (TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].isAnim && !TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].isBonus && !TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].isScatter)
    //                    {
    //                        TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].ResetBG();
    //                        TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].transform.parent.gameObject.SetActive(false);
    //                    }
    //                }
    //                PayCalculator.DontDestroyLines.Clear();
    //                PayCalculator.DontDestroyLines.TrimExcess();
    //                PayCalculator.ResetLines();
    //            }
    //        }

    //        if (BonusCount > 0)
    //        {
    //            for (int i = 0; i < TempList.Count; i++)
    //            {
    //                if (TempList[i].GetComponent<BoxScripting>().isBonus)
    //                {
    //                    TempList[i].transform.parent.gameObject.SetActive(true);
    //                }
    //            }
    //            yield return new WaitForSeconds(2f);

    //            for (int i = 0; i < TempList.Count; i++)
    //            {
    //                if (TempList[i].GetComponent<BoxScripting>().isBonus)
    //                {
    //                    TempList[i].transform.parent.gameObject.SetActive(false);
    //                }
    //            }
    //        }

    //        if (scatterCount > 0)
    //        {
    //            for (int i = 0; i < TempList.Count; i++)
    //            {
    //                if (TempList[i].GetComponent<BoxScripting>().isScatter)
    //                {
    //                    TempList[i].transform.parent.gameObject.SetActive(true);
    //                }
    //            }
    //            yield return new WaitForSeconds(2f);

    //            for (int i = 0; i < TempList.Count; i++)
    //            {
    //                if (TempList[i].GetComponent<BoxScripting>().isScatter)
    //                {
    //                    TempList[i].transform.parent.gameObject.SetActive(false);
    //                }
    //            }
    //        }

    //        for (int i = 0; i < LineIDs.Count; i++)
    //        {
    //            y_points = y_string[LineIDs[i] + 1]?.Split(',')?.Select(Int32.Parse)?.ToList();
    //            PayCalculator.GeneratePayoutLinesBackend(y_points, y_points.Count, LineIDs[i] % 10);
    //            PayCalculator.DontDestroyLines.Add(LineIDs[i]);
    //        }
    //        yield return new WaitForSeconds(2f);
    //        PayCalculator.DontDestroyLines.Clear();
    //        PayCalculator.DontDestroyLines.TrimExcess();
    //        PayCalculator.ResetLines();
    //    }
    //}

    private void WinningsAnim(bool IsStart)
    {
        if (IsStart)
        {
            WinTween = TotalWin_text.gameObject.GetComponent<RectTransform>().DOScale(new Vector2(1.5f, 1.5f), 1f).SetLoops(-1, LoopType.Yoyo).SetDelay(0);
        }
        else
        {
            WinTween.Kill();
            //TotalWin_text.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }

    #endregion

    internal void CallCloseSocket()
    {
        SocketManager.CloseSocket();
    }


    void ToggleButtonGrp(bool toggle)
    {
        if (uiManager) uiManager.ToggleButtonGrp(toggle);
    }

    //start the icons animation
    private void StartGameAnimation(GameObject animObjects)
    {
        ImageAnimation temp = animObjects.GetComponent<ImageAnimation>();
        if (temp.textureArray.Count > 0)
        {
            temp.StartAnimation();
            TempList.Add(temp);
        }
    }

    //stop the icons animation
    private void StopGameAnimation()
    {
        for (int i = 0; i < TempList.Count; i++)
        {
            TempList[i].StopAnimation();
        }
        TempList.Clear();
        TempList.TrimExcess();
    }


    #region TweeningCode
    private void InitializeTweening(Transform slotTransform, int index)
    {
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, -(float)stopIndices[index]);
        Tweener tweener = slotTransform.DOLocalMoveY(-3014 + (float)stopIndices[index], 0.4f).SetLoops(-1, LoopType.Restart).SetDelay(0).SetEase(Ease.Linear);
        tweener.Play();
        alltweens.Add(tweener);
    }



    private IEnumerator StopTweening(Transform slotTransform, int index)
    {
        alltweens[index].Pause();
        alltweens[index] = slotTransform.DOLocalMoveY(0 - (float)stopIndices[index] - 274, 0.5f).SetEase(Ease.OutSine);
        yield return new WaitForSeconds(0.2f);
    }


    private void KillAllTweens()
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            alltweens[i].Kill();
        }
        alltweens.Clear();

    }
    #endregion

}

[Serializable]
public class SlotImage
{
    public List<Image> slotImages = new List<Image>(10);
}

[Serializable]
public class BoxScript
{
    public List<BoxScripting> boxScripts = new List<BoxScripting>(10);
}

