// #define DEBUG_EIJIS_SCORE_SCREEN
// #define DEBUG_EIJIS_MNBK_AUTOCOUNTER
// #define DEBUG_EIJIS_MNBK_MCB

// #define EIJIS_MNBK_AUTOCOUNTER
// #define EIJIS_MNBK_MCB

using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
#if EIJIS_MNBK_AUTOCOUNTER && EIJIS_MNBK_MCB
using tarsan;
#endif

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class BilliardsScoreScreen : UdonSharpBehaviour
{
    [SerializeField] private PlayerRow headerRow;
    [SerializeField] private TeamPlayers highGroup; // Orange
    [SerializeField] private TeamPlayers lowGroup; // Blue
    [SerializeField] private PlayerRow footerRow;
    private BilliardsModule table;
    
    private TeamPlayers[] teamPlayers = new TeamPlayers[2];
    
#if true // EIJIS_MNBK_AUTOCOUNTER
    private PlayerRow[] scoreSyncRows = new PlayerRow[11]; // footer row
#else
    private PlayerRow[] scoreSyncRows = new PlayerRow[10];
#endif
    
    [SerializeField] private int gameScoreType;
    public const int GAME_SCORE_TYPE_ROTATION = 0;
    public const int GAME_SCORE_TYPE_BOWLARDS = 1;
    
#if EIJIS_MNBK_AUTOCOUNTER
    [UdonSynced(UdonSyncMode.None)]
    private bool EditLocked = true;

    [UdonSynced(UdonSyncMode.None)]
    private bool AutoCountMode = true;

    // [UdonSynced(UdonSyncMode.None)]
    private bool DisplayText = true;

    private readonly string editLockName = "EditLock";
    private Toggle EditLockToggle;
    
    private GameObject AutoCount;
    private readonly string autoCountName = "AutoCount";
    private Toggle AutoCountToggle;

    private readonly string displayTextName = "DisplayText";
    private Toggle DisplayTextToggle;

    private GameObject Reset;
    private readonly string resetName = "Reset";
    //private GameObject ResultText;
    private readonly string resultTextName = "ResultText";
    private InputField ResultTextInputField;
    //private readonly string resultTextInputFieldName = "InputField";

    private GameObject ResultCommentText;
    private readonly string resultCommentTextName = "ResultCommentText";
    private InputField ResultCommentTextInputField;
    
    private GameObject EditButtonPanel;
    private readonly string editButtonPanelName = "EditButtonPanel";

    private GameObject EditSelection;
    private readonly string editSelectionName = "EditSelection";

    // private GameObject CountUpDown;
    // private readonly string countUpDownName = "CountUpDown";
    
    [UdonSynced(UdonSyncMode.None)] private byte editCounterIndex;

    [UdonSynced(UdonSyncMode.None)] private byte inningCountOffset;
    [UdonSynced(UdonSyncMode.None)] private byte player1ScoreOffset;
    [UdonSynced(UdonSyncMode.None)] private byte player1SafetyOffset;
    [UdonSynced(UdonSyncMode.None)] private byte player1GoalOffset;
    [UdonSynced(UdonSyncMode.None)] private byte player2ScoreOffset;
    [UdonSynced(UdonSyncMode.None)] private byte player2SafetyOffset;
    [UdonSynced(UdonSyncMode.None)] private byte player2GoalOffset;
    [UdonSynced(UdonSyncMode.None)] private byte ballDeadCountOffset;
    
    private string[] counterNames;
    private byte[] counterOffsets;
    private PlayerRow[] editCounterIndexToPlayerRow;
    private int[] editCounterIndexToColIndex;
    
    public int[] AdjustOffsets
    {
        get
        {
#if DEBUG_EIJIS_MNBK_AUTOCOUNTER
            table._Log($"EIJIS_DEBUG BilliardsScoreScreen::AdjustOffsets get");
#endif

            int[] offsets = new int[8];
            
            // for (int i = 0; i < counterNames.Length; i++)
            // {
            //     if (ReferenceEquals(null, counters[i]))
            //     {
            //         Debug.Log($"{counterNames[i]} ValueText not set.");
            //         continue;
            //     }
            //
            //     offsets[i] = counters[i].Offset;
            // }
            
            for (int i = 0; i < counterOffsets.Length; i++)
            {
                int unsigned = counterOffsets[i];
                int offsetValue = (127 < unsigned) ? -(256 - unsigned) : unsigned;
                offsets[i] = offsetValue;
            }

            return offsets;
        }
    }
#endif
#if EIJIS_MNBK_AUTOCOUNTER && EIJIS_MNBK_MCB
    public BattleCheckMng battleCheckMng;
    private readonly string toggleMCB_BattledName = "MCB_Battled";
    private Toggle MCB_BattledToggle;
#endif

    // private void Start()
    public void Init(BilliardsModule _table)
    {
        this.table = _table;
#if EIJIS_MNBK_AUTOCOUNTER

        counterNames = new []
        {
            "Counter1 Inning",
            "Counter1",
            "Counter1 Safety1",
            "Counter1 Goal1",
            "Counter2",
            "Counter2 Safety2",
            "Counter2 Goal2",
            "Counter2 BallDead"
        };

        counterOffsets = new[]
        {
            inningCountOffset,
            player1ScoreOffset,
            player1SafetyOffset,
            player1GoalOffset,
            player2ScoreOffset,
            player2SafetyOffset,
            player2GoalOffset,
            ballDeadCountOffset
        };

        editCounterIndexToPlayerRow = new[]
        {
            footerRow,
            highGroup.GetTeamRow(),
            highGroup.GetTeamRow(),
            highGroup.GetTeamRow(),
            lowGroup.GetTeamRow(),
            lowGroup.GetTeamRow(),
            lowGroup.GetTeamRow(),
            footerRow
        };

        editCounterIndexToColIndex = new[]
        {
            2,
            0,
            2,
            1,
            0,
            2,
            1,
            0
        };
        
        Transform editLockTransform = this.transform.Find(editLockName);
        if (ReferenceEquals(null, editLockTransform))
        {
            Debug.Log($"{editLockName} Transform not found.");
        }
        else
        {
            EditLockToggle = editLockTransform.GetComponentInChildren<Toggle>();
        }

        Transform autoCountTransform = this.transform.Find(autoCountName);
        if (ReferenceEquals(null, autoCountTransform))
        {
            Debug.Log($"{autoCountName} Transform not found.");
        }
        else
        {
            AutoCount = autoCountTransform.gameObject;
            AutoCount.SetActive(!EditLocked);
            AutoCountToggle = autoCountTransform.GetComponentInChildren<Toggle>();
        }

        Transform displayTextTransform = this.transform.Find(displayTextName);
        if (ReferenceEquals(null, displayTextTransform))
        {
            Debug.Log($"{displayTextName} Transform not found.");
        }
        else
        {
            DisplayTextToggle = displayTextTransform.GetComponentInChildren<Toggle>();
            DisplayText = DisplayTextToggle.isOn;
        }

        Transform resetTransform = this.transform.Find(resetName);
        if (ReferenceEquals(null, resetTransform))
        {
            Debug.Log($"{resetName} Transform not found.");
        }
        else
        {
            Reset = resetTransform.gameObject;
            Reset.SetActive(!AutoCountMode);
        }

        Transform resultTextTransform = this.transform.Find(resultTextName);
        // if (ReferenceEquals(null, resultTextTransform))
        // {
        //     Debug.Log($"{resultTextName} Transform not found.");
        // }
        // else
        // {
        //     //ResultText = resultTextTransform.gameObject;
        //     ResultTextInputField = resultTextTransform.GetComponentInChildren<InputField>();
        //     if (ReferenceEquals(null, ResultTextInputField))
        //     {
        //         Debug.Log($"InputField component in [ {resultTextName} ] not found.");
        //     }
        // }
        if (!ReferenceEquals(null, resultTextTransform)) resultTextTransform.gameObject.SetActive(false);

        Transform resultCommentTextTransform = this.transform.Find(resultCommentTextName);
        if (ReferenceEquals(null, resultCommentTextTransform))
        {
            Debug.Log($"{resultCommentTextName} Transform not found.");
        }
        else
        {
            ResultCommentText = resultCommentTextTransform.gameObject;
            ResultCommentText.SetActive(DisplayText);

            ResultCommentTextInputField = resultCommentTextTransform.GetComponentInChildren<InputField>();
            if (ReferenceEquals(null, ResultCommentTextInputField))
            {
                Debug.Log($"InputField component in [ {ResultCommentTextInputField} ] not found.");
            }
        }

        Transform editButtonPanelTransform = this.transform.Find(editButtonPanelName);
        EditButtonPanel = editButtonPanelTransform.gameObject;
        
        Transform editSelectionTransform = this.transform.Find(editSelectionName);
        EditSelection = editSelectionTransform.gameObject;

        // Transform countUpDownNameTransform = this.transform.Find(countUpDownName);
        // CountUpDown = countUpDownNameTransform.gameObject;

        SetButtonsEnable(!EditLocked);
#endif
#if EIJIS_MNBK_AUTOCOUNTER && EIJIS_MNBK_MCB
        Transform toggleMCB_BattledTransform = this.transform.Find(toggleMCB_BattledName);
        if (ReferenceEquals(null, toggleMCB_BattledTransform))
        {
            Debug.Log($"{toggleMCB_BattledName} Transform not found.");
        }
        else
        {
            MCB_BattledToggle = toggleMCB_BattledTransform.GetComponentInChildren<Toggle>();
            MCB_BattledToggle.interactable = false;
            MCB_BattledToggle.gameObject.SetActive(false);
        }
#endif

        headerRow.Table = table;
        teamPlayers = new[] { highGroup, lowGroup };
        foreach (var group in teamPlayers)
        {
            if (ReferenceEquals(null, group))
            {
                continue;
            }
            
            group.Init();
            group.Table = table;
            // group.Init();
        }
        //UpdateLeftBallCount(0);
        
        int scoreSyncRowsIndex = 0;
        for (int i = 0; i < teamPlayers.Length; i++)
        {
            if (ReferenceEquals(null, teamPlayers[i]))
            {
                continue;
            }
            
            scoreSyncRows[scoreSyncRowsIndex++] = teamPlayers[i].GetTeamRow();
            for (int j = 0; j < 4; j++)
            {
                scoreSyncRows[scoreSyncRowsIndex++] = teamPlayers[i].GetPlayerRow(j);
            }
        }

        if (!ReferenceEquals(null, footerRow))
        {
            footerRow.Table = table;
            scoreSyncRows[scoreSyncRowsIndex++] = footerRow;
            footerRow.SetNoChangeTextOnZeroByColIndex(true, 0);
            // footerRow.SetNoChangeTextOnZeroByColIndex(true, 1);
            // footerRow.SetNoChangeTextOnZeroByColIndex(true, 2);
        }
        
#if EIJIS_MNBK_AUTOCOUNTER
        editCounterIndex = 2; // Counter1 Safety
        editSelectionUpdate();
#endif
        
        initValues();
    }   
    
    /// <summary>
    /// 55ball系 ( 21ball ～ 120ball ) のエンコード
    /// 14-1 のエンコード
    /// </summary>
    /// <returns></returns>
    public uint[] EncodeScoreSyncValues_ManyBall()
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log("TKCH BilliardsScoreScreen::EncodeScoreSyncValues_ManyBall()");
#endif
        int validRowCount = 0;
        for (int i = 0; i < scoreSyncRows.Length; i++)
        {
            if (!ReferenceEquals(null, scoreSyncRows[i]))
            {
                validRowCount++;
            }
        }
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"  validRowCount = {validRowCount}");
#endif

        uint[] encodeScoreSyncValues = new uint[validRowCount * 2];
        for (int i = 0, j = 0; i < scoreSyncRows.Length; i++)
        {
            if (ReferenceEquals(null, scoreSyncRows[i]))
            {
                continue;
            }
            uint[] encodeScoreSyncValue = scoreSyncRows[i].EncodeScoreSyncValue_ManyBall();
            encodeScoreSyncValues[j*2] = encodeScoreSyncValue[0];
            encodeScoreSyncValues[(j*2)+1] = encodeScoreSyncValue[1];
            j++;
        }

        return encodeScoreSyncValues;
    }
    
    /// <summary>
    /// Rotation のエンコード
    /// </summary>
    /// <returns></returns>
    public uint[] EncodeScoreSyncValues_Rotation()
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log("TKCH BilliardsScoreScreen::EncodeScoreSyncValues_Rotation()");
#endif
        int validRowCount = 0;
        for (int i = 0; i < scoreSyncRows.Length; i++)
        {
            if (!ReferenceEquals(null, scoreSyncRows[i]))
            {
                validRowCount++;
            }
        }
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"  validRowCount = {validRowCount}");
#endif

        uint[] encodeScoreSyncValues = new uint[validRowCount * 2];
        for (int i = 0, j = 0; i < scoreSyncRows.Length; i++)
        {
            if (ReferenceEquals(null, scoreSyncRows[i]))
            {
                continue;
            }
            uint encodeScoreSyncValue = scoreSyncRows[i].EncodeScoreSyncValue_Rotation();
            encodeScoreSyncValues[j] = encodeScoreSyncValue;
            j++;
        }

        return encodeScoreSyncValues;
    }

    /// <summary>
    /// 55ball系 ( 21ball ～ 120ball ) のエンコード ( パラメタ版 )
    /// 14-1 のエンコード ( パラメタ版 )
    /// </summary>
    /// <param name="point"></param>
    /// <param name="scratchCount"></param>
    /// <param name="pocketBallCount"></param>
    /// <param name="shotCount"></param>
    /// <param name="safeNoPocketShotCount"></param>
    /// <param name="invalidPocketBallCount"></param>
    /// <returns></returns>
    public uint[] EncodeScoreParams_ManyBall(int point, int scratchCount, int pocketBallCount, int shotCount, int safeNoPocketShotCount, int invalidPocketBallCount)
    {
        if (scoreSyncRows.Length <= 0)
        {
            return null;
        }

        return scoreSyncRows[0].EncodeScoreParams_ManyBall(point, scratchCount, pocketBallCount, shotCount, safeNoPocketShotCount, invalidPocketBallCount);
    }

    /// <summary>
    /// BankPool, OnePocket のエンコード
    /// 14-1 のエンコード
    /// Mnbk9Ball のエンコード
    /// </summary>
    /// <param name="point"></param>
    /// <param name="shotCount"></param>
    /// <param name="scratchCount"></param>
    /// <param name="safeNoPocketShotCount"></param>
    /// <returns></returns>
    public uint EncodeScoreParams_Mini(int point, int shotCount, int scratchCount, int safeNoPocketShotCount)
    {
        if (scoreSyncRows.Length <= 0)
        {
            return 0xDEADBEAFu;
        }

        return scoreSyncRows[0].EncodeScoreParams_Mini(point, shotCount, scratchCount, safeNoPocketShotCount);
    }

    /// <summary>
    /// Rotation のエンコード
    /// </summary>
    /// <param name="point"></param>
    /// <param name="goal"></param>
    /// <param name="highRun"></param>
    /// <param name="foul"></param>
    /// <returns></returns>
    public uint EncodeScoreParams_Rotation(int point, int goal, int highRun, int foul)
    {
        if (scoreSyncRows.Length <= 0)
        {
            return 0xDEADBEAFu;
        }

        return scoreSyncRows[0].EncodeScoreParams_Rotation(point, goal, highRun, foul);
    }

    /// <summary>
    /// Japan9Ball 5-9 のエンコード
    /// </summary>
    /// <param name="totalPoint"></param>
    /// <param name="scores"></param>
    /// <returns></returns>
    public uint EncodeScoreParams_Frame5(int totalPoint, int[] scores)
    {
        if (scoreSyncRows.Length <= 0)
        {
            return 0xDEADBEAFu;
        }

        return scoreSyncRows[0].EncodeScoreParams_Frame5(totalPoint, scores);
    }

    /// <summary>
    /// 55ball系 ( 21ball ～ 120ball ) のデコード
    /// </summary>
    /// <param name="scoreSyncValues"></param>
    public void DecodeScoreSyncValues_ManyBall(uint[] scoreSyncValues)
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log("TKCH BilliardsScoreScreen::DecodeScoreSyncValues_ManyBall()");
#endif
        for (int i = 0, j = 0 ; i < scoreSyncRows.Length; i++)
        {
            if (ReferenceEquals(null, scoreSyncRows[i]))
            {
                continue;
            }
            uint[] scoreSyncValue = new[] { scoreSyncValues[j*2], scoreSyncValues[(j*2) + 1] };
            scoreSyncRows[i].DecodeSyncValue_ManyBall(scoreSyncValue);
            j++;
        }
    }
    
    /// <summary>
    /// BankPool, OnePocket のデコード
    /// </summary>
    /// <param name="scoreSyncValues"></param>
    public void DecodeScoreSyncValues_Mini(uint[] scoreSyncValues)
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log("TKCH BilliardsScoreScreen::DecodeScoreSyncValues_Mini()");
#endif
        for (int i = 0, j = 0; i < scoreSyncRows.Length; i++)
        {
            if (ReferenceEquals(null, scoreSyncRows[i]))
            {
                continue;
            }
            scoreSyncRows[i].DecodeSyncValue_Mini(scoreSyncValues[j]);
            j++;
        }
    }

    /// <summary>
    /// Japan9Ball 5-9 のデコード
    /// </summary>
    /// <param name="scoreSyncValues"></param>
    public void DecodeScoreSyncValues_Frame5(uint[] scoreSyncValues)
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"TKCH BilliardsScoreScreen::DecodeScoreSyncValues_Frame5( scoreSyncValues.Length = {scoreSyncValues.Length} )");
        // table._Log($"TKCH  vsTeamZeroSumMode = {vsTeamZeroSumMode}");
        table._Log($"TKCH  scoreSyncRows.Length {scoreSyncRows.Length}");
#endif
        /*
        if (vsTeamZeroSumMode)
        {
            int[][] scoreMatrix = new[] { new int[6], new int[6], new int[6], new int[6] };
            
            for (int i = 0, j = 0; i < scoreSyncRows.Length; i++)
            {
                if (scoreSyncValues.Length <= j)
                {
                    continue;
                }
                if (ReferenceEquals(null, scoreSyncRows[i]))
                {
                    continue;
                }
                int[] decodedSyncValues = scoreSyncRows[i].DecodedSyncValues_Frame5(scoreSyncValues[j]);
#if DEBUG_EIJIS_SCORE_SCREEN
                table._Log($"TKCH  decodedSyncValues.Length {decodedSyncValues.Length}");
#endif
                Array.Copy(decodedSyncValues, scoreMatrix[j], scoreMatrix[j].Length);
                j++;
            }

            int activeTeamCount = 0;
            for (int i = 0; i < teamPlayers.Length; i++)
            {
                if (ReferenceEquals(null, teamPlayers[i]) || 
                    teamPlayers[i].GetTeamRow().GetName() == "")
                {
                    break;
                }
            
                activeTeamCount++;
            }
#if DEBUG_EIJIS_SCORE_SCREEN
            table._Log($"TKCH  activeTeamCount {activeTeamCount}");
#endif
            
            for (int i = 0; i < 6; i++)
            {
                int[] subTotals = new int[activeTeamCount];
                
                for (int j = 0; j < activeTeamCount; j++)
                {
                    subTotals[j] = scoreMatrix[j][i] * (activeTeamCount - 1);
                    for (int k = 0; k < activeTeamCount; k++)
                    {
                        if (j == k)
                        {
                            continue;
                        }
                        
                        subTotals[j] -= scoreMatrix[k][i];
                    }
                }
                
                for (int j = 0; j < activeTeamCount; j++)
                {
                    scoreMatrix[j][i] = subTotals[j];
                }
            }

            for (int i = 0, j = 0; i < scoreSyncRows.Length; i++)
            {
                if (ReferenceEquals(null, scoreSyncRows[i]))
                {
                    continue;
                }
                scoreSyncRows[i].SetScoreByArray(scoreMatrix[j]);
                j++;
            }
        }
        else
        */
        {
#if DEBUG_EIJIS_SCORE_SCREEN
            table._Log($"TKCH  scoreSyncRows is null ? {ReferenceEquals(null, scoreSyncRows)}");
            table._Log($"TKCH  scoreSyncRows.Length = {scoreSyncRows.Length}");
#endif
            for (int i = 0, j = 0; i < scoreSyncRows.Length; i++)
            {
                if (ReferenceEquals(null, scoreSyncRows[i]))
                {
                    continue;
                }
                scoreSyncRows[i].DecodeSyncValue_Frame5(scoreSyncValues[j]);
                j++;
            }
        }
        
        // if (!ReferenceEquals(null, cascadeScoreScreen))
        // {
        //     cascadeScoreScreen.DecodeScoreSyncValues_Frame5(scoreSyncValues);
        // }
    }

    /// <summary>
    /// Rotation のデコード
    /// </summary>
    /// <param name="scoreSyncValues"></param>
    public void DecodeScoreSyncValues_Rotation(uint[] scoreSyncValues)
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"EIJIS BilliardsScoreScreen::DecodeScoreSyncValues_Rotation(scoreSyncValues.Length = {scoreSyncValues.Length})");
#endif
        for (int i = 0, j = 0; i < scoreSyncRows.Length; i++)
        {
            if (ReferenceEquals(null, scoreSyncRows[i]))
            {
                continue;
            }

            if (scoreSyncValues.Length <= j)
            {
                break;
            }
            
#if DEBUG_EIJIS_SCORE_SCREEN
            table._Log($"EIJIS  i = {i}, j = {j}");
#endif
            scoreSyncRows[i].DecodeSyncValue_Rotation(scoreSyncValues[j]);
            j++;
        }
    }

#if EIJIS_MNBK_AUTOCOUNTER
    public override void OnDeserialization()
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log("TKCH BilliardsScoreScreen::OnDeserialization()");
#endif

        EditLockToggle.isOn = EditLocked;
        EditButtonPanel.SetActive(EditLockToggle.interactable && !EditLocked);
        if (EditLockToggle.interactable && !EditLocked && editCounterIndex < 8)
        {
            EditSelection.SetActive(true);
            editSelectionUpdate();
        }
        else
        {
            EditSelection.SetActive(false);
        }

        SyncValueToOffsetArray();

        for (int i = 0; i < scoreSyncRows.Length; i++)
        {
            if (ReferenceEquals(null, scoreSyncRows[i]))
            {
                continue;
            }

            scoreSyncRows[i].UpdateText();
        }
        
#if DEBUG_EIJIS_MNBK_AUTOCOUNTER
        dumpMnbkScoreOffset();
        dumpMnbkScoreOffsetInArray();
#endif
    }
#endif
    
    public void Clear()
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log("TKCH BilliardsScoreScreen::Clear()");
#endif
        foreach (var group in teamPlayers)
        {
            if (ReferenceEquals(null, group))
            {
                continue;
            }

            group.Clear();
        }

#if EIJIS_MNBK_AUTOCOUNTER
        for (int i = 0; i < counterOffsets.Length; i++)
        {
            counterOffsets[i] = 0;
        }
        OffsetArrayToSyncValue();
#endif
    }

    public void ScoreUpdate_ManyBall(
        int teamId,
        int playerId,
        bool isScratch,
        bool isFoul,
        int pocketCount,
        int pointCount,
        int shotCount
    )
    {
        teamPlayers[teamId].TeamScoreUpdate_ManyBall(
            (playerId < 0 ? playerId : (playerId / 2)),
            isScratch,
            isFoul,
            pocketCount,
            pointCount,
            shotCount
            );
    }
    
    public void UpdateLeftBallCount(int leftBallCount)
    {
        //headerRow.SetName($"残り：{leftBallCount}");
        headerRow.SetName($"　　　{leftBallCount}");
    }
    
    public void UpdateGameNameWithNumber(string ganeName, int number)
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"EIJIS_DEBUG BilliardsScoreScreen::UpdateGameNameWithNumber() ganeName => {ganeName}, number => {number}");
#endif

        headerRow.SetName($"{ganeName} [{number}]");
        
        // if (!ReferenceEquals(null, cascadeScoreScreen))
        // {
        //     cascadeScoreScreen.UpdateGameNameWithNumber(ganeName, number);
        // }
    }

    public void UpdateHeaderTextByColIndex(string text, int colIndex)
    {
        headerRow.SetTextByColIndex(text, colIndex);
        
        // if (!ReferenceEquals(null, cascadeScoreScreen))
        // {
        //     cascadeScoreScreen.UpdateHeaderTextByColIndex(text, colIndex);
        // }
    }

    public void UpdateTeamName(int teamIndex, string teamName)
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"TKCH BilliardsScoreScreen::UpdateTeamName() teamIndex => {teamIndex}, teamName => {teamName}");
        table._Log($"  teamPlayers.Length => {teamPlayers.Length}");
        table._Log($"  ReferenceEquals(null, teamPlayers[{teamIndex}]) => {ReferenceEquals(null, teamPlayers[teamIndex])}");
        table._Log($"  ReferenceEquals(null, teamPlayers[{teamIndex}].GetTeamRow()) => {ReferenceEquals(null, teamPlayers[teamIndex].GetTeamRow())}");
#endif
        teamPlayers[teamIndex].GetTeamRow().SetName(teamName);
        
        // if (!ReferenceEquals(null, cascadeScoreScreen))
        // {
        //     cascadeScoreScreen.UpdateTeamName(teamIndex, teamName);
        // }
    }

    public void UpdatePlayer(int playerId, string playerName)
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"TKCH BilliardsScoreScreen::UpdatePlayer() playerId => {playerId}, playerName => {playerName}");
#endif
        
#if DEBUG_EIJIS_SCORE_SCREEN
        //table._Log($"TKCH BilliardsScoreScreen::UpdatePlayer() playerId => {playerId}, playerName => {playerName}, rowSyncOverrideLock => {rowSyncOverrideLock}");
#endif
        
        int teamIndex = playerId % 2;
        if (teamIndex < teamPlayers.Length && !ReferenceEquals(null, teamPlayers[teamIndex]))
        {
            var playerRow = teamPlayers[teamIndex].GetPlayerRow((int)(playerId / 2));
            if (!ReferenceEquals(null, playerRow))
            {
                playerRow.SetName(playerName);
            }
        }
    }

    public int WinnerTeamId()
    {
        // if (highGroup.GetTeamPoint() > lowGroup.GetTeamPoint())
        // {
        //     return 0;
        // }
        // else if (highGroup.GetTeamPoint() < lowGroup.GetTeamPoint())
        // {
        //     return 1;
        // }
        //
        // return -1;

        int winnerTeamId = -1;
        int maxPoint = Int32.MinValue;
        
        for (int i = 0; i < teamPlayers.Length; i++)
        {
            if (ReferenceEquals(null, teamPlayers[i]))
            {
                continue;
            }

            int teamPoint = teamPlayers[i].GetTeamPoint();
            if (maxPoint < teamPoint)
            {
                winnerTeamId = i;
                maxPoint = teamPoint;
            }
            else if (maxPoint == teamPoint)
            {
                return -1;
            }
        }

        return winnerTeamId;
    }
    
    public int GetTeamPoint(int teamIndex)
    {
        return teamPlayers[teamIndex].GetTeamPoint();
    }

    public int GetTeamPocketBallCount(int teamIndex)
    {
        return teamPlayers[teamIndex].GetTeamPocketBallCount();
    }

    public int GetTeamShotCount(int teamIndex)
    {
        return teamPlayers[teamIndex].GetTeamShotCount();
    }
    
    public int GetTeamSafeNoPocketShotCount(int teamIndex)
    {
        return teamPlayers[teamIndex].GetTeamSafeNoPocketShotCount();
    }
    
    public int GetTeamScratchCount(int teamIndex)
    {
        return teamPlayers[teamIndex].GetTeamScratchCount();
    }
    
    public int GetTeamInvalidPocketBallCount(int teamIndex)
    {
        return teamPlayers[teamIndex].GetTeamInvalidPocketBallCount();
    }

    public int GetTeamRotationPoint(int teamIndex)
    {
        return teamPlayers[teamIndex].GetTeamRotationPoint();
    }

    public int GetTeamRotationGoal(int teamIndex)
    {
        return teamPlayers[teamIndex].GetTeamRotationGoal();
    }

    public int GetTeamRotationHighRun(int teamIndex)
    {
        return teamPlayers[teamIndex].GetTeamRotationHighRun();
    }

    public int GetTeamRotationFoul(int teamIndex)
    {
        return teamPlayers[teamIndex].GetTeamRotationFoul();
    }

    public int[] GetTeamScores(int teamIndex)
    {
        return teamPlayers[teamIndex].GetTeamRow().GetScores();
    }

    public int GetTeamScoreByColIndex(int teamIndex, int colIndex)
    {
        return teamPlayers[teamIndex].GetTeamRow().GetScoreByColIndex(colIndex);
    }

    public void SetTeamSafeNoPocketShotCountEmptyTextOnZero(int teamIndex, bool emptyTextOnZero)
    {
        teamPlayers[teamIndex].GetTeamRow().SetSafeNoPocketShotCountEmptyTextOnZero(emptyTextOnZero);
    }
    
    public void SetTeamInvalidPocketBallCountEmptyTextOnZero(int teamIndex, bool emptyTextOnZero)
    {
        teamPlayers[teamIndex].GetTeamRow().SetInvalidPocketBallCountEmptyTextOnZero(emptyTextOnZero);
    }

    public void SetPointSigned(bool signed)
    {
        for (int i = 0; i < teamPlayers.Length; i++)
        {
            teamPlayers[i].GetTeamRow().SetPointSigned(signed);
        }
    }

    /// <summary>
    /// Rotation のスコア表示更新
    /// </summary>
    /// <param name="team0Point"></param>
    /// <param name="team0Goal"></param>
    /// <param name="team0HighRun"></param>
    /// <param name="team0Foul"></param>
    /// <param name="team1Point"></param>
    /// <param name="team1Goal"></param>
    /// <param name="team1HighRun"></param>
    /// <param name="team1Foul"></param>
    /// <param name="infoText"></param>
    /// <param name="inningCount"></param>
    public void updateValues_Rotation(
        int team0Point,
        int team0Goal,
        int team0HighRun,
        int team0Foul,
        int team1Point,
        int team1Goal,
        int team1HighRun,
        int team1Foul,
        // string infoText,
        int inningCount
    )
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"EIJIS_DEBUG BilliardsScoreScreen::updateValues_Rotation()");
#endif
        uint[] values = new uint[3];
        values[0] = EncodeScoreParams_Rotation(team0Point, team0Goal, team0HighRun, team0Foul);
        values[1] = EncodeScoreParams_Rotation(team1Point, team1Goal, team1HighRun, team1Foul);
        values[2] = EncodeScoreParams_Rotation(0, 0, 0, inningCount);
        DecodeScoreSyncValues_Rotation(values);
    }
    
    public void updateGoalPoint_Rotation(int team0Goal, int team1Goal)
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"TKCH BilliardsScoreScreen::updateGoalPoint_Rotation(team0Goal = {team0Goal}, team1Goal = {team1Goal})");
#endif
        teamPlayers[0].GetTeamRow().SetGoalPoint_Rotation(team0Goal);
        teamPlayers[1].GetTeamRow().SetGoalPoint_Rotation(team1Goal);
    }

    public void updateInningCount_Rotation(int inningCount)
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"TKCH BilliardsScoreScreen::updateInningCount_Rotation(inningCount = {inningCount})");
#endif
        footerRow.SetInningCount_Rotation(inningCount);
    }

    private int[] messageKeys = new[] { -1, -1 };
    private string[] messages = new string[2];
    
    public void clearInfoText()
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"EIJIS_DEBUG BilliardsScoreScreen::clearInfoText()");
#endif
        messageKeys = new[] { -1, -1 };
        messages = new string[2];
        footerRow.SetInfoText(" " /* String.Empty */);
    }

    public void updateInfoText(int messageKey, string message)
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"EIJIS_DEBUG BilliardsScoreScreen::updateInfoText(messageKey = {messageKey}, message = {message})");
#endif
        bool appended = false;
        for (int i = 0; i < messageKeys.Length; i++)
        {
            if (messageKeys[i] == messageKey)
            {
                messages[i] += message;
                appended = true;
                break;                
            }
        }

        if (!appended)
        {
            bool writed = false;
            for (int i = 0; i < messageKeys.Length; i++)
            {
                if (messageKeys[i] < 0)
                {
                    messageKeys[i] = messageKey;
                    messages[i] = message;
                    writed = true;
                    break;                
                }
            }

            if (!writed)
            {
                if (messageKeys[0] < messageKeys[1])
                {
                    messageKeys[0] = messageKey;
                    messages[0] = message;
                }
                else
                {
                    messageKeys[1] = messageKey;
                    messages[1] = message;
                }
            }
        }

        string[] coloredMessages = new string[messages.Length];
        for (int i = 0; i < messages.Length; i++)
        {
            if (i < messages.Length && messageKeys[i] < 0)
            {
                continue;
            }

            if (messages[i].StartsWith("[Red ]"))
            {
                coloredMessages[i] = $"<color=\"#B05050\">{messages[i]}</color>";
            }
            else if (messages[i].StartsWith("[Blue]"))
            {
                coloredMessages[i] = $"<color=\"#5070C0\">{messages[i]}</color>";
            }
        }

        string msg = String.Empty;
        if (messageKeys[1] < 0)
            msg = $"{coloredMessages[0]}\n";
        else if (messageKeys[0] < messageKeys[1])
            msg = $"{coloredMessages[0]}\n{coloredMessages[1]}";
        else
            msg = $"{coloredMessages[1]}\n{coloredMessages[0]}";
        
        footerRow.SetInfoText(msg);
    }

    public void updateRegulationText(string tableName, uint timer, bool requireCallShot, bool noGuideline, bool noLocking, 
        bool isPractice, byte undoCount, bool wasLoadUsed, bool wasPlayerChanged)
    {
        string shotClock = timer <= 0 ? "<color=\"red\">none</color>" : timer.ToString();
        string callShot = requireCallShot ? "ON" : "<color=\"red\">none</color>";
        string guideline = noGuideline ? "none" : "<color=\"red\">ON</color>";
        string cueLock = noLocking ? "none" : "<color=\"red\">ON</color>";
        string usedUndoCount = 0 < undoCount ? $"<color=\"red\">{undoCount}</color>" : undoCount.ToString();
        string practiceMode = isPractice ? "  |  <color=\"red\">Practice mode</color>" : String.Empty;
        string playerChanged = wasPlayerChanged ? "  |  <color=\"magenta\">Player changed</color>" : String.Empty;
        string continueFromSavedCode = wasLoadUsed ? "  |  <color=\"lime\">Continue from saved code</color>" : String.Empty;
        string msg = 
            $"TableModel : {tableName}  |  " + 
            $"UndoCount : {usedUndoCount}" +
            practiceMode +
            $"\nShotClock : {shotClock}  |  " + 
            $"CueGuideLine : {guideline}" + 
            playerChanged +
            $"\nCallShot : {callShot}  |  " +
            $"CueLocking : {cueLock}" +
            continueFromSavedCode;
        footerRow.SetRegulationText(msg);
    }

    public void setFrameLength_Bowlards(int goalFrameNumber)
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"EIJIS_DEBUG BilliardsScoreScreen::setFrameLength_Bowlards(goalFrameNumber = {goalFrameNumber})");
#endif
        
        headerRow.setFrameLength_Bowlards(goalFrameNumber);
        int hideFrame = 10 - goalFrameNumber;
        byte[] frameNumbers = new byte[11];
        int j = 1;
        for (int i = 0; i < 10; i++)
        {
            frameNumbers[i] = (byte)(i < hideFrame ? 0 : 1);
        }
        headerRow.ScoreUpdate_Bowlards(11, -1, frameNumbers);

        foreach (PlayerRow scoreSyncRow in scoreSyncRows)
        {
            if (ReferenceEquals(null, scoreSyncRow))
            {
                continue;
            }

            if (ReferenceEquals(footerRow, scoreSyncRow))
            {
                continue;
            }
            
            scoreSyncRow.setFrameLength_Bowlards(goalFrameNumber);
        }
    }

    /// <summary>
    /// Bowlards のスコア表示更新
    /// </summary>
    public void updateValues_Bowlards(int teamId, int[] frameCounts, int throwInningCount, byte[][] framePoints)
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"EIJIS_DEBUG BilliardsScoreScreen::updateValues_Bowlards(teamId = {teamId}, frameCounts[] = ({frameCounts[0]}, {frameCounts[1]}), throwInningCount = {throwInningCount})");
#endif
        for (int i = 0, j = 0; i < scoreSyncRows.Length; i++)
        {
            if (ReferenceEquals(null, scoreSyncRows[i]))
            {
                continue;
            }

            if (ReferenceEquals(footerRow, scoreSyncRows[i]))
            {
                continue;
            }

            if (framePoints.Length <= j)
            {
                break;
            }
            
            PlayerRow scoreSyncRow = scoreSyncRows[i];
            byte[] framePointsByRow = framePoints[j];
            int frameCount = frameCounts[j];
            // byte lastFrame = framePointsByRow[framePointsByRow.Length - 1];
            // int frameCount = (lastFrame & 0xF0) >> 4;

#if DEBUG_EIJIS_SCORE_SCREEN
            // table._Log($"EIJIS_DEBUG  i = {i}, j = {j}");
            // // table._Log($"EIJIS_DEBUG  framePoints[{j}][0] = {framePoints[j][0]}");
            // table._Log($"EIJIS_DEBUG  framePointsByRow.Length = {framePointsByRow.Length}");
            // // table._Log($"EIJIS_DEBUG  frameCount = {frameCount}");
#endif

            scoreSyncRow.ScoreUpdate_Bowlards(frameCount, (j == teamId ? throwInningCount : -1), framePointsByRow);
            j++;
        }
    }

    public int[] getScore_Bowlards()
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log("EIJIS_DEBUG BilliardsScoreScreen::getScore_Bowlards()");
#endif
        int[] scores = new int[2];
        for (int i = 0, j = 0; i < scoreSyncRows.Length; i++)
        {
            if (ReferenceEquals(null, scoreSyncRows[i]))
            {
                continue;
            }

            if (ReferenceEquals(footerRow, scoreSyncRows[i]))
            {
                continue;
            }

            if (scores.Length <= j)
            {
                break;
            }

            scores[j++] = scoreSyncRows[i].GetBowlardsFinalScore();
        }

        return scores;
    }
    
#if EIJIS_MNBK_AUTOCOUNTER
    public void clearOffsets()
    {
#if DEBUG_EIJIS_MNBK_AUTOCOUNTER
        table._Log("EIJIS_DEBUG BilliardsScoreScreen::clearOffsets()");
#endif
        
        // if (!AutoCountMode)
        // {
        //     return;
        // }
        //
        // for (int i = 0; i < counterNames.Length; i++)
        // {
        //     if (ReferenceEquals(null, counters[i]))
        //     {
        //         Debug.Log($"{counterNames[i]} ValueText not set.");
        //         continue;
        //     }
        //
        //     counters[i].clearOffset();
        // }
        
        for (int i = 0; i < counterOffsets.Length; i++)
        {
            counterOffsets[i] = 0;
        }
        OffsetArrayToSyncValue();
        
        for (int i = 0; i < scoreSyncRows.Length; i++)
        {
            if (ReferenceEquals(null, scoreSyncRows[i]))
            {
                continue;
            }

            scoreSyncRows[i].SetScoreAdjustOffsetByArray(new []{ 0, 0, 0, 0, 0, 0, 0});
        }
    }

    /// <summary>
    /// Mnbk9Ball のスコア表示更新
    /// </summary>
    /// <param name="inningCount"></param>
    /// <param name="player1Score"></param>
    /// <param name="player1Safety"></param>
    /// <param name="player1Goal"></param>
    /// <param name="player2Score"></param>
    /// <param name="player2Safety"></param>
    /// <param name="player2Goal"></param>
    /// <param name="ballDeadCount"></param>
    public void updateValues_Mnbk(
        int inningCount,
        int player1Score,
        int player1Safety,
        int player1Goal,
        int player2Score,
        int player2Safety,
        int player2Goal,
        int ballDeadCount
    )
    {
#if DEBUG_EIJIS_MNBK_AUTOCOUNTER
        table._Log($"EIJIS_DEBUG BilliardsScoreScreen::updateValues_Mnbk()");
#endif
        if (ReferenceEquals(null, counterNames))
        {
            return;
        }
        
        if (!AutoCountMode)
        {
            return;
        }

        // int[] values =
        // {
        //     inningCount,
        //     player1Score,
        //     player1Safety,
        //     player1Goal,
        //     player2Score,
        //     player2Safety,
        //     player2Goal,
        //     ballDeadCount
        // };

        // for (int i = 0; i < counterNames.Length; i++)
        // {
        //     if (ReferenceEquals(null, counters[i]))
        //     {
        //         Debug.Log($"{counterNames[i]} ValueText not set.");
        //         continue;
        //     }
        //
        //     counters[i].updateValue(values[i]);
        // }

        uint[] values = new uint[3];
        values[0] = EncodeScoreParams_Mini(player1Score, player1Goal, 0, player1Safety);
        values[1] = EncodeScoreParams_Mini(player2Score, player2Goal, 0, player2Safety);
        values[2] = EncodeScoreParams_Mini(ballDeadCount, 0, 0, inningCount);
        DecodeScoreSyncValues_Mini(values);
        // footerRow.SetTextByColIndex("Inning →", 1);
    }

    public void setPlayers(string[] players)
    {
#if DEBUG_EIJIS_MNBK_AUTOCOUNTER
        table._Log($"EIJIS_DEBUG BilliardsScoreScreen::setPlayers players.Length={players.Length}");
#endif
        /* if (ReferenceEquals(null, playerNames[0]))
        {
            Debug.Log($"{playerTextNames[0]} Text not set.");
        }
        else */ if (players[2] == "")
        {
            UpdateTeamName(0, players[0]);
            // playerNames[0].fontSize = 17;
            // playerNames[0].text = players[0];
        }
        else
        {
            UpdateTeamName(0, players[0] + "\n" + players[2]);
            // playerNames[0].fontSize = 13;
            // playerNames[0].text =players[0] + "\n" + players[2];
        }

        if (players[1] == "")
        {
            UpdateTeamName(1, "[Blue]");
            // playerNames[1].fontSize = 17;
            // playerNames[1].text = "";
        }
        else
        {
            /* if (ReferenceEquals(null, playerNames[1]))
            {
                Debug.Log($"{playerTextNames[1]} Text not set.");
            }
            else */ if (players[3] == "")
            {
                UpdateTeamName(1, players[1]);
                // playerNames[1].fontSize = 17;
                // playerNames[1].text = players[1];
            }
            else
            {
                UpdateTeamName(1, players[1] + "\n" + players[3]);
                // playerNames[1].fontSize = 13;
                // playerNames[1].text =players[1] + "\n" + players[3];
            }
        }
#if EIJIS_MNBK_MCB
        UpdateMCB_Battled(players);
#endif
    }
    
    public void SetButtonsEnable(bool enable)
    {
#if DEBUG_EIJIS_MNBK_AUTOCOUNTER
        table._Log($"EIJIS_DEBUG BilliardsScoreScreen::SetButtonsEnable enable={enable}");
#endif
        
        // for (int i = 0; i < counterNames.Length; i++)
        // {
        //     // ignore "Counter1 Safety1", "Counter2 Safety2"
        //     if (i == 2 || i == 5)
        //     {
        //         continue;
        //     }
        //     
        //     if (!ReferenceEquals(null, countUpButtons[i]))
        //     {
        //         countUpButtons[i].SetActive(enable);
        //     }
        //     if (!ReferenceEquals(null, countDownButtons[i]))
        //     {
        //         countDownButtons[i].SetActive(enable);
        //     }
        // }

        // GetComponentInChildren<Canvas>().enabled = enable; // Panel
        EditButtonPanel.SetActive(enable);
        EditSelection.SetActive(enable);
        // CountUpDown.SetActive(enable);
    }
    
    public void ToggleButtonsEnable()
    {
#if DEBUG_EIJIS_MNBK_AUTOCOUNTER
        table._Log($"EIJIS_DEBUG BilliardsScoreScreen::ToggleButtonsEnable() EditLocked={EditLocked}, isOn={EditLockToggle.isOn}");
#endif
        if (!ReferenceEquals(null, EditLockToggle) && EditLockToggle.isOn == EditLocked) return;

        EditLocked = !EditLocked;
        SetButtonsEnable(!EditLocked);
        if (ReferenceEquals(null, AutoCount))
        {
            Debug.Log("AutoCount GameObject not set.");
        }
        else
        {
            // AutoCount.SetActive(!EditLocked);
        }

        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        RequestSerialization();
    }

    public void ToggleAutoCount()
    {
#if DEBUG_EIJIS_MNBK_AUTOCOUNTER
        table._Log($"EIJIS_DEBUG BilliardsScoreScreen::ToggleAutoCount() AutoCountMode={AutoCountMode}, isOn={AutoCountToggle.isOn}");
#endif
        if (!ReferenceEquals(null, AutoCountToggle) && AutoCountToggle.isOn == AutoCountMode) return;

        AutoCountMode = !AutoCountMode;
        if (ReferenceEquals(null, Reset))
        {
            Debug.Log("Reset GameObject not set.");
        }
        else
        {
            Reset.SetActive(!AutoCountMode);
        }
        
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        RequestSerialization();
    }

    public void ResetCounters()
    {
        // for (int i = 0; i < counterNames.Length; i++)
        // {
        //     if (ReferenceEquals(null, counters[i]))
        //     {
        //         Debug.Log($"{counterNames[i]} ValueText not set.");
        //         continue;
        //     }
        //
        //     counters[i].reset();
        // }
        
        Clear();
    }

    public void OutputResultText(bool tsv)
    {
        if (ReferenceEquals(null, ResultTextInputField))
        {
            Debug.Log($"InputField component in [ {resultTextName} ] not found.");
            return;
        }
        
        if (ReferenceEquals(null, ResultTextInputField.textComponent))
        {
            Debug.Log($"Text component in [ {ResultTextInputField.name} ] not found.");
            return;
        }

        PlayerRow p1row = teamPlayers[0].GetTeamRow();
        PlayerRow p2row = teamPlayers[1].GetTeamRow();
        string player1 = p1row.GetName();
        string player2 = p2row.GetName();
        // string player1 = (ReferenceEquals(null, playerNames[0])) ? "" : playerNames[0].text;
        // string player2 = (ReferenceEquals(null, playerNames[1])) ? "" : playerNames[1].text;

        string[] values = new string[counterNames.Length];
        // for (int i = 0; i < counterNames.Length; i++)
        // {
        //     values[i] = (ReferenceEquals(null, counters[i])) ? "" : $"{counters[i].text.text}";
        // }
        int i = 0;
        string[] p1scores = p1row.GetScoreTextStrings();
        string[] p2scores = p2row.GetScoreTextStrings();
        values[i++] = footerRow.GetSafeNoPocketShotCount().ToString();
        values[i++] = p1scores[0];
        values[i++] = p1scores[2];
        values[i++] = p1scores[1];
        values[i++] = p2scores[0];
        values[i++] = p2scores[2];
        values[i++] = p2scores[1];
        values[i++] = footerRow.GetPoint().ToString();

        /*
        // プレイヤー1,プレイヤー2,セーフティ1,得点1,得点2,セーフティ2,イニング,ボールデッド
        if (tsv)
        {
            ResultTextInputField.text = $"{player1}\t{player2}\t{values[2]}\t{values[1]}\t{values[4]}\t{values[5]}\t{values[0]}\t{values[7]}";
        }
        else
        {
            ResultTextInputField.text = $"{player1},{player2},{values[2]},{values[1]},{values[4]},{values[5]},{values[0]},{values[7]}";
        }
        */
        
        // プレイヤー1,プレイヤー2,イニング,ボールデッド,スキル1,得点1,セーフティ1,スキル2,得点2,セーフティ2
        if (tsv)
        {
            ResultTextInputField.text = $"{player1}\t{player2}\t{values[0]}\t{values[7]}\t{values[3]}\t{values[1]}\t{values[2]}\t{values[6]}\t{values[4]}\t{values[5]}";
        }
        else
        {
            ResultTextInputField.text = $"{player1},{player2},{values[0]},{values[7]},{values[3]},{values[1]},{values[2]},{values[6]},{values[4]},{values[5]}";
        }
    }

    public void OutputResultTsvText()
    {
        OutputResultText(true);
    }

    public void OutputResultCsvText()
    {
        OutputResultText(false);
    }

    public void ClearResultText()
    {
        if (ReferenceEquals(null, ResultTextInputField))
        {
            Debug.Log($"InputField component in [ {resultTextName} ] not found.");
            return;
        }
        
        if (ReferenceEquals(null, ResultTextInputField.textComponent))
        {
            Debug.Log($"Text component in [ {ResultTextInputField.name} ] not found.");
            return;
        }

        ResultTextInputField.text = string.Empty;
    }

    public void ToggleDisplayText()
    {
        DisplayText = !DisplayText;
        if (ReferenceEquals(null, ResultCommentText))
        {
            Debug.Log("ResultCommentText GameObject not set.");
        }
        else
        {
            ResultCommentText.SetActive(DisplayText);
        }

        // Networking.SetOwner(Networking.LocalPlayer, gameObject);
        // RequestSerialization();
    }
    
    public void OutputResultCommentText()
    {
        if (ReferenceEquals(null, ResultCommentTextInputField))
        {
            Debug.Log($"InputField component in [ {resultCommentTextName} ] not found.");
            return;
        }
        
        if (ReferenceEquals(null, ResultCommentTextInputField.textComponent))
        {
            Debug.Log($"Text component in [ {ResultCommentTextInputField.name} ] not found.");
            return;
        }

        PlayerRow p1row = teamPlayers[0].GetTeamRow();
        PlayerRow p2row = teamPlayers[1].GetTeamRow();
        string player1 = p1row.GetName();
        string player2 = p2row.GetName();
        // string player1 = (ReferenceEquals(null, playerNames[0])) ? "" : playerNames[0].text;
        // string player2 = (ReferenceEquals(null, playerNames[1])) ? "" : playerNames[1].text;

        string[] values = new string[counterNames.Length];
        // for (int i = 0; i < counterNames.Length; i++)
        // {
        //     values[i] = (ReferenceEquals(null, counters[i])) ? "" : $"{counters[i].text.text}";
        // }
        int i = 0;
        string[] p1scores = p1row.GetScoreTextStrings();
        string[] p2scores = p2row.GetScoreTextStrings();
        values[i++] = footerRow.GetSafeNoPocketShotCount().ToString();
        values[i++] = p1scores[0];
        values[i++] = p1scores[2];
        values[i++] = p1scores[1];
        values[i++] = p2scores[0];
        values[i++] = p2scores[2];
        values[i++] = p2scores[1];
        values[i++] = footerRow.GetPoint().ToString();

        ResultCommentTextInputField.text = $"{player1} ({values[1]}/{values[3]}) vs {player2} ({values[4]}/{values[6]})\nイニング{values[0]} セーフティ{values[2]}-{values[5]} ボールデッド{values[7]}";
    }

    public void ClearResultCommentText()
    {
        if (ReferenceEquals(null, ResultCommentTextInputField))
        {
            Debug.Log($"InputField component in [ {resultCommentTextName} ] not found.");
            return;
        }
        
        if (ReferenceEquals(null, ResultCommentTextInputField.textComponent))
        {
            Debug.Log($"Text component in [ {ResultCommentTextInputField.name} ] not found.");
            return;
        }

        ResultCommentTextInputField.text = string.Empty;
    }

    private void editTargetChanged(int index)
    {
        if (editCounterIndex == (byte)index)
            return;
        
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        editCounterIndex = (byte)index;
        RequestSerialization();
        editSelectionUpdate();
    }
    
    public void EditTarget_0_0()
    {
        editTargetChanged(1);
    }

    public void EditTarget_0_1()
    {
        editTargetChanged(3);
    }

    public void EditTarget_0_2()
    {
        editTargetChanged(2);
    }

    public void EditTarget_1_0()
    {
        editTargetChanged(4);
    }
    
    public void EditTarget_1_1()
    {
        editTargetChanged(6);
    }

    public void EditTarget_1_2()
    {
        editTargetChanged(5);
    }

    public void EditTarget_2_0()
    {
        editTargetChanged(7);
    }
    
    // public void EditTarget_2_1()
    // {
    //     
    // }

    public void EditTarget_2_2()
    {
        editTargetChanged(0);
    }

    private void offsetValueChanged(int modify)
    {
#if DEBUG_EIJIS_MNBK_AUTOCOUNTER
        table._Log($"EIJIS_DEBUG BilliardsScoreScreen::offsetValueChanged( modify={modify} )");
#endif

        if (modify == 0)
            return;
        
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        int unsigned = counterOffsets[editCounterIndex];
        int offsetValue = (127 < unsigned) ? -(256 - unsigned) : unsigned;
        offsetValue += modify;
        offsetValue = 127 < offsetValue ? 127 : (offsetValue < -128 ? -128 : offsetValue);
        unsigned = (offsetValue < 0) ? offsetValue + 256 : offsetValue;
        counterOffsets[editCounterIndex] = (byte)unsigned;
        OffsetArrayToSyncValue();
#if DEBUG_EIJIS_MNBK_AUTOCOUNTER
        table._Log($"  editCounterIndex={editCounterIndex} (counterName {counterNames[editCounterIndex]})");
        dumpMnbkScoreOffset();
        dumpMnbkScoreOffsetInArray();
#endif
        RequestSerialization();

        PlayerRow editRow = editCounterIndexToPlayerRow[editCounterIndex];
        int editColIndex = editCounterIndexToColIndex[editCounterIndex];
        // int value = editRow.GetScoreByColIndex(editColIndex);
        // editRow.SetScoreByColIndex(value + offsetValue, editColIndex);
        editRow.SetScoreAdjustOffsetByColIndex(offsetValue, editColIndex);
    }

    public void CountUp()
    {
#if DEBUG_EIJIS_MNBK_AUTOCOUNTER
        table._Log($"EIJIS_DEBUG BilliardsScoreScreen::CountUp()");
#endif
        offsetValueChanged(1);
    }

    public void CountDown()
    {
#if DEBUG_EIJIS_MNBK_AUTOCOUNTER
        table._Log($"EIJIS_DEBUG BilliardsScoreScreen::CountDown()");
#endif
        offsetValueChanged(-1);
    }
    
    private void editSelectionUpdate()
    {
        // if (ReferenceEquals(null, EditSelection))
        //     return;
        
        Transform editSelectionTransform = EditSelection.transform;
        PlayerRow editRow = editCounterIndexToPlayerRow[editCounterIndex];
        int editColIndex = editCounterIndexToColIndex[editCounterIndex];
        Text text = editRow.GetScoreTextByColIndex(editColIndex);
        Transform textTransform = text.transform;
        editSelectionTransform.position = textTransform.position;
    }

    private void OffsetArrayToSyncValue()
    {
        int i = 0;
        inningCountOffset = counterOffsets[i++];
        player1ScoreOffset = counterOffsets[i++];
        player1SafetyOffset = counterOffsets[i++];
        player1GoalOffset = counterOffsets[i++];
        player2ScoreOffset = counterOffsets[i++];
        player2SafetyOffset = counterOffsets[i++];
        player2GoalOffset = counterOffsets[i++];
        ballDeadCountOffset = counterOffsets[i++];
    }

    private void SyncValueToOffsetArray()
    {
        int i = 0;
        counterOffsets[i++] = inningCountOffset;
        counterOffsets[i++] = player1ScoreOffset;
        counterOffsets[i++] = player1SafetyOffset;
        counterOffsets[i++] = player1GoalOffset;
        counterOffsets[i++] = player2ScoreOffset;
        counterOffsets[i++] = player2SafetyOffset;
        counterOffsets[i++] = player2GoalOffset;
        counterOffsets[i++] = ballDeadCountOffset;

        UpdateUnsignedScoreAdjustOffsets();
    }

    private void UpdateUnsignedScoreAdjustOffsets()
    {
        highGroup.GetTeamRow().SetScoreAdjustOffsetByArray(new []{ ByteToSignedOffsetValue(player1ScoreOffset), ByteToSignedOffsetValue(player1GoalOffset), ByteToSignedOffsetValue(player1SafetyOffset), 0, 0, 0, 0});
        lowGroup.GetTeamRow().SetScoreAdjustOffsetByArray(new []{ ByteToSignedOffsetValue(player2ScoreOffset), ByteToSignedOffsetValue(player2GoalOffset), ByteToSignedOffsetValue(player2SafetyOffset), 0, 0, 0, 0});
        footerRow.SetScoreAdjustOffsetByArray(new []{ ByteToSignedOffsetValue(ballDeadCountOffset), 0, ByteToSignedOffsetValue(inningCountOffset), 0, 0, 0, 0});
    }

    private int ByteToSignedOffsetValue(byte b)
    {
        int unsigned = b;
        int offsetValue = (127 < unsigned) ? -(256 - unsigned) : unsigned;
        return offsetValue;
    }

    public void SetEdit_Interactable(bool enable)
    {
#if DEBUG_EIJIS_MNBK_AUTOCOUNTER
        table._Log($"EIJIS_DEBUG BilliardsScoreScreen::SetEdit_Interactable(enable = {enable})");
#endif

        EditLockToggle.interactable = enable;
        if (EditLockToggle.interactable && !EditLocked && editCounterIndex < 8)
        {
            EditSelection.SetActive(true);
            editSelectionUpdate();
        }
        else
        {
            EditSelection.SetActive(false);
        }
    }
#if DEBUG_EIJIS_MNBK_AUTOCOUNTER
    public void dumpMnbkScoreOffset()
    {
        table._LogInfo($"dumpMnbkScoreOffset inningCountOffset={inningCountOffset} player1ScoreOffset={player1ScoreOffset} player1SafetyOffset={player1SafetyOffset} player1GoalOffset={player1GoalOffset}");
        table._LogInfo($"                  ballDeadCountOffset={ballDeadCountOffset} player2ScoreOffset={player2ScoreOffset} player2SafetyOffset={player2SafetyOffset} player2GoalOffset={player2GoalOffset}");
    }
    
    public void dumpMnbkScoreOffsetInArray()
    {
        for (int i = 0; i < counterOffsets.Length; i++)
        {
            table._Log($"  counterOffsets[{i}({counterNames[i]})]={counterOffsets[i]} )");
        }
    }
#endif
#endif
#if EIJIS_MNBK_AUTOCOUNTER && EIJIS_MNBK_MCB
    public void ToggleMCB_Battled()
    {
#if DEBUG_EIJIS_MNBK_MCB
        table._Log("EIJIS_DEBUG BilliardsScoreScreen::ToggleMCB_Battled()");
#endif
        if (ReferenceEquals(null, battleCheckMng) || !MCB_BattledToggle.interactable) return;
        
        string playerAName = teamPlayers[0].GetTeamRow().GetName();
        string playerBName = teamPlayers[1].GetTeamRow().GetName();
        if(Networking.LocalPlayer.displayName != playerAName){battleCheckMng.SetHasBattled(playerAName, true);}
        if(Networking.LocalPlayer.displayName != playerBName){battleCheckMng.SetHasBattled(playerBName, true);}
    }
    
    public void SetMCB_Battled(string playerAName, string playerBName)
    {
#if DEBUG_EIJIS_MNBK_MCB
        table._Log($"EIJIS_DEBUG BilliardsScoreScreen::SetMCB_Battled(playerAName = {playerAName}, playerBName = {playerBName})");
#endif
        if (ReferenceEquals(null, battleCheckMng)) return;

        if (Networking.LocalPlayer.displayName != playerAName && Networking.LocalPlayer.displayName != playerBName)
        {
#if DEBUG_EIJIS_MNBK_MCB
            table._Log($"EIJIS_DEBUG 　not player ( Networking.LocalPlayer.displayName = {Networking.LocalPlayer.displayName} )");
#endif
            return;
        }
        
        if(Networking.LocalPlayer.displayName != playerAName){battleCheckMng.SetHasBattled(playerAName);}
        if(Networking.LocalPlayer.displayName != playerBName){battleCheckMng.SetHasBattled(playerBName);}
        
#if DEBUG_EIJIS_MNBK_MCB
        UpdateMCB_Battled(new string[] {playerAName, playerBName, "", ""});
#else
        UpdateMCB_Battled(new string[] {playerAName, playerBName});
#endif
    }
    
    public void UpdateMCB_Battled(string[] players)
    {
#if DEBUG_EIJIS_MNBK_MCB
        table._Log($"EIJIS_DEBUG BilliardsScoreScreen::UpdateMCB_Battled(players = {(4<=players.Length?players[0]+players[1]+players[2]+players[3]:string.Empty)})");
#endif
        if (ReferenceEquals(null, battleCheckMng) || ReferenceEquals(null, MCB_BattledToggle)) return;

        bool hasBattled = false;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != "" && Networking.LocalPlayer.displayName != players[i])
            {
                hasBattled = battleCheckMng.HasBattled(players[i]);
                if (hasBattled)
                {
                    break;
                }
            }
        }
        
        bool isPlayer = false;
        for (int i = 0; i < players.Length; i++)
        {
            if (Networking.LocalPlayer.displayName == players[i])
            {
                isPlayer = true;
                break;
            }
        }

        MCB_BattledToggle.interactable = false;
        MCB_BattledToggle.isOn = hasBattled;
        MCB_BattledToggle.interactable = isPlayer;
    }

    public void SetActiveToggleMCB_Battled(bool active)
    {
#if DEBUG_EIJIS_MNBK_MCB
        table._Log($"EIJIS_DEBUG BilliardsScoreScreen::SetActiveToggleMCB_Battled(active = {active})");
#endif
        if (ReferenceEquals(null, battleCheckMng) || ReferenceEquals(null, MCB_BattledToggle)) return;

        MCB_BattledToggle.gameObject.SetActive(active);
    }
#endif
    
    public void initValues()
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log("EIJIS_DEBUG BilliardsScoreScreen::initValues()");
#endif
        switch (gameScoreType)
        {
            case GAME_SCORE_TYPE_ROTATION:
                updateValues_Rotation(
                    0, 0, 0, 0,
                    0, 0, 0, 0,
                    0);
                break;
            case GAME_SCORE_TYPE_BOWLARDS:
                for (int i = 0; i < scoreSyncRows.Length; i++)
                {
                    if (ReferenceEquals(null, scoreSyncRows[i]))
                    {
                        continue;
                    }

                    scoreSyncRows[i].SetEmptyTextOnZero(true);
                }
                updateValues_Bowlards(0, new int[] {-1, -1}, 0, new byte[][]{ new byte[11], new byte[11]});
                break;
        }
    }
    
    public uint[] EncodeScoreSyncValues()
    {
        uint[] encodeScoreSyncValues = new uint[0];

        switch (gameScoreType)
        {
            case GAME_SCORE_TYPE_ROTATION:
                encodeScoreSyncValues = EncodeScoreSyncValues_Rotation();
                break;
            case GAME_SCORE_TYPE_BOWLARDS:
                encodeScoreSyncValues = EncodeScoreSyncValues_Rotation();
                break;
        }

        return encodeScoreSyncValues;
    }

    public void DecodeScoreSyncValues(uint[] scoreSyncValues)
    {
        switch (gameScoreType)
        {
            case GAME_SCORE_TYPE_ROTATION:
                DecodeScoreSyncValues_Rotation(scoreSyncValues);
                break;
            case GAME_SCORE_TYPE_BOWLARDS:
                DecodeScoreSyncValues_Rotation(scoreSyncValues);
                break;
        }
    }
}
