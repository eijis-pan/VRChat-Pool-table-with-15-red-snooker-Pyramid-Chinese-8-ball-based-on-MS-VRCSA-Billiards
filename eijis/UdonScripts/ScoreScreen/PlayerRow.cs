// #define DEBUG_EIJIS_SCORE_SCREEN

using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PlayerRow : UdonSharpBehaviour
{
    [SerializeField] private Text nameText;
    // [SerializeField] private Text pointText;
    // [SerializeField] private Text shotCountText;
    // [SerializeField] private Text safeNoPocketShotCountText;
    // [SerializeField] private Text scratchCountText;
    // [SerializeField] private Text pocketBallCountText;
    // [SerializeField] private Text invalidPocketBallCountText;

    public const int MAX_NUMBER_COL_COUNT = 32; // without nameText
    
    // Array index label for 55(any)Ball Score
    public const int COL_POINT = 0;
    public const int COL_SHOT_COUNT = 1;
    public const int COL_SAFE_COUNT = 2;
    public const int COL_FOUL_COUNT = 3;
    public const int COL_BALL_POCKET_COUNT = 4;
    public const int COL_BALL_DEAD_COUNT = 5;

    // Array index label for Japan9Ball 5-9 Score
    public const int COL_JP9_TOTAL_POINT = 0;
    public const int COL_JP9_POINT_RACK1 = 1;
    public const int COL_JP9_POINT_RACK2 = 2;
    public const int COL_JP9_POINT_RACK3 = 3;
    public const int COL_JP9_POINT_RACK4 = 4;
    public const int COL_JP9_POINT_RACK5 = 5;

    // Array index label for Rotation Score
    public const int COL_ROTATION_POINT = 0;
    public const int COL_ROTATION_GOAL = 1;
    public const int COL_ROTATION_HIGHRUN = 2;
    public const int COL_ROTATION_FOUL = 3;
    public const int COL_ROTATION_FOOT_INNING = 2;

    public const int COL_BOWLARDS_FINAL_SCORE = 30;

    public const int COL_FOOTER_INFO_TEXT = 0;
    public const int COL_FOOTER_REGULATION_TEXT = 1;

    private Text[] allTexts = new Text[MAX_NUMBER_COL_COUNT + 1];
    [SerializeField] Text[] scoreTexts = new Text[MAX_NUMBER_COL_COUNT];

    private int[] scores = new int[MAX_NUMBER_COL_COUNT];
    private bool[] scoreSigned = new bool[MAX_NUMBER_COL_COUNT];
    private bool[] scoreEmptyTextOnZero = new bool[MAX_NUMBER_COL_COUNT];
    private bool[] scoreNoChangeTextOnZero = new bool[MAX_NUMBER_COL_COUNT];

    [SerializeField] GameObject[] mark1 = new GameObject[MAX_NUMBER_COL_COUNT]; // Bowlards spare
    [SerializeField] GameObject[] mark2 = new GameObject[MAX_NUMBER_COL_COUNT]; // Bowlards strike
    [SerializeField] GameObject[] mark3 = new GameObject[MAX_NUMBER_COL_COUNT]; // Current cell indicator
    
    [NonSerialized] private BilliardsModule table;

#if DEBUG_EIJIS_SCORE_SCREEN
    private bool UpdateText_debug = false;
    
#endif
    public BilliardsModule Table
    {
        get
        {
            return table;
        }
        set
        {
#if DEBUG_EIJIS_SCORE_SCREEN
            Debug.Log($"EIJIS PlayerRow::Table set [{GetInstanceID()}]");
#endif
            table = value;
            Init();
        }
    }
    
    /// <summary>
    /// 1セルを4bitとして最大32セルをuint[4]からデコードするサンプル
    /// </summary>
    /// <param name="values">uint[4]</param>
    public void DecodeSyncValue_DefaultSample(uint[] values)
    {
        const int syncValueCount = 4;
        const int colCountByValue = MAX_NUMBER_COL_COUNT / syncValueCount;
        const int bitShitUnit = 4;

        for (int valIdx = 0; valIdx < syncValueCount; valIdx++)
        {
            uint value = values[valIdx];
            for (int i = 0; i < colCountByValue; i++)
            {
                int bitShift = bitShitUnit * i;
                int colIdx = (valIdx * colCountByValue) + i; 
                int decodedValue = (int)((value >> bitShift) & 0xFu);
                if (scoreSigned[colIdx])
                {
                    uint u = (value >> bitShift) & 0x7u;
                    if (0 < ((value >> bitShift) & 0x8u))
                    {
                        u |= 0xFFFFFFF8u;
                        u = ~u;
                        decodedValue = 0 - ((int)u + 1);
                    }
                    else
                    {
                        decodedValue = (int)u;
                    }
                }

                scores[colIdx] = decodedValue;
            }
        }
        
#if DEBUG_EIJIS_SCORE_SCREEN
        int j = 0;
        table._Log($"EIJIS PlayerRow::DecodeSyncValue_DefaultSample [{GetInstanceID()}] scores[{j}] => {scores[j++]}, scores[{j}] => {scores[j++]}, scores[{j}] => {scores[j++]}, scores[{j}] => {scores[j++]}, scores[{j}] => {scores[j++]}, scores[{j}] => {scores[j++]}");
#endif

        UpdateText();
    }

    /// <summary>
    /// 55ball系 ( 21ball ～ 120ball ) のデコード
    /// 14-1 のデコード
    /// </summary>
    /// <param name="values">uint[2]</param>
    public void DecodeSyncValue_ManyBall(uint[] values)
    {
        uint value = values[0];
        int point = (int)((value >> 16) & 0xFFFFu); // 0xFFFF
        if (scoreSigned[COL_POINT])
        {
            uint u = (value >> 16) & 0x7FFFu;
            if (0 < ((value >> 16) & 0x8000u))
            {
                u |= 0xFFFF8000u;
                u = ~u;
                point = 0 - ((int)u + 1);
            }
            else
            {
                point = (int)u;
            }
        }
        int scratchCount = (int)((value >> 8) & 0xFFu); // 0xFF
        int pocketBallCount = (int)((value >> 0) & 0xFFu); // 0xFF

        value = values[1];
        int shotCount = (int)((value >> 20) & 0xFFFu); // 0xFFF
        int safeNoPocketShotCount = (int)((value >> 10) & 0x3FFu); // 0x3FF
        int invalidPocketBallCount = (int)((value >> 0) & 0x3FFu); // 0x3FF
                
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"EIJIS PlayerRow::DecodeSyncValue_ManyBall [{GetInstanceID()}] point => {point}, shotCount => {shotCount}, safeNoPocketShotCount => {safeNoPocketShotCount}, scratchCount => {scratchCount}, pocketBallCount => {pocketBallCount}, invalidPocketBallCount => {invalidPocketBallCount}");
#endif

        scores[COL_POINT] = point; // - 127;
        scores[COL_SHOT_COUNT] = shotCount;
        scores[COL_SAFE_COUNT] = safeNoPocketShotCount;
        scores[COL_FOUL_COUNT] = scratchCount;
        scores[COL_BALL_POCKET_COUNT] = pocketBallCount;
        scores[COL_BALL_DEAD_COUNT] = invalidPocketBallCount;

        UpdateText();
    }

    /// <summary>
    /// BankPool, OnePocket のデコード
    /// </summary>
    /// <param name="value">uint</param>
    public void DecodeSyncValue_Mini(uint value)
    {
        int shotCount = (int)((value >> 24) & 0xFFu);
        int scratchCount = (int)((value >> 16) & 0xFFu);
        int safeNoPocketShotCount = (int)((value >> 8) & 0xFFu);
        int point = (int)((value >> 0) & 0xFFu);
        if (scoreSigned[COL_POINT])
        {
            uint u = (value >> 0) & 0x7Fu;
            if (0 < ((value >> 0) & 0x80u))
            {
                u |= 0xFFFFFF80u;
                u = ~u;
                point = 0 - ((int)u + 1);
            }
            else
            {
                point = (int)u;
            }
        }

        // int pocketBallCount = 0;
        // int invalidPocketBallCount = 0;
                
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"EIJIS PlayerRow::DecodeSyncValue_Mini [{GetInstanceID()}] point => {point}, shotCount => {shotCount}, safeNoPocketShotCount => {safeNoPocketShotCount}, scratchCount => {scratchCount}");
#endif

        scores[COL_POINT] = point; // - 127;
        scores[COL_SHOT_COUNT] = shotCount;
        scores[COL_SAFE_COUNT] = safeNoPocketShotCount;
        scores[COL_FOUL_COUNT] = scratchCount;
        // scores[COL_BALL_POCKET_COUNT] = pocketBallCount;
        // scores[COL_BALL_DEAD_COUNT] = invalidPocketBallCount;
        // if (0 <= Array.IndexOf(scoreTexts, pocketBallCountText)) scores[Array.IndexOf(scoreTexts, pocketBallCountText)] = pocketBallCount;
        // if (0 <= Array.IndexOf(scoreTexts, invalidPocketBallCountText)) scores[Array.IndexOf(scoreTexts, invalidPocketBallCountText)] = invalidPocketBallCount;

        UpdateText();
    }
    
    /// <summary>
    /// Japan9Ball 5-9 のデコード
    /// </summary>
    /// <param name="value">uint</param>
    public void DecodeSyncValue_Frame5(uint value)
    {
        int shotCount = (int)((value >> 27) & 0x1Fu);
        int safeNoPocketShotCount = (int)((value >> 22) & 0x1Fu);
        int scratchCount = (int)((value >> 17) & 0x1Fu);
        int pocketBallCount = (int)((value >> 12) & 0x1Fu);
        int invalidPocketBallCount = (int)((value >> 7) & 0x1Fu);
        int point = (int)((value >> 0) & 0x7Fu);
        if (scoreSigned[COL_POINT])
        {
            uint u = (value >> 0) & 0x3Fu;
            if (0 < ((value >> 0) & 0x40u))
            {
                u |= 0xFFFFFFC0u;
                u = ~u;
                point = 0 - ((int)u + 1);
            }
            else
            {
                point = (int)u;
            }
        }

#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"EIJIS PlayerRow::DecodeSyncValue_Frame5 [{GetInstanceID()}] point => {point}, shotCount => {shotCount}, safeNoPocketShotCount => {safeNoPocketShotCount}, scratchCount => {scratchCount}, pocketBallCount => {pocketBallCount}, invalidPocketBallCount => {invalidPocketBallCount}");
#endif

        // if (0 <= Array.IndexOf(scoreTexts, pointText)) scores[Array.IndexOf(scoreTexts, pointText)] = point; // - 127;
        // if (0 <= Array.IndexOf(scoreTexts, shotCountText)) scores[Array.IndexOf(scoreTexts, shotCountText)] = shotCount;
        // if (0 <= Array.IndexOf(scoreTexts, safeNoPocketShotCountText)) scores[Array.IndexOf(scoreTexts, safeNoPocketShotCountText)] = safeNoPocketShotCount;
        // if (0 <= Array.IndexOf(scoreTexts, scratchCountText)) scores[Array.IndexOf(scoreTexts, scratchCountText)] = scratchCount;
        // if (0 <= Array.IndexOf(scoreTexts, pocketBallCountText)) scores[Array.IndexOf(scoreTexts, pocketBallCountText)] = pocketBallCount;
        // if (0 <= Array.IndexOf(scoreTexts, invalidPocketBallCountText)) scores[Array.IndexOf(scoreTexts, invalidPocketBallCountText)] = invalidPocketBallCount;

        int[] values = new[]
        {
            point, 
            shotCount, 
            safeNoPocketShotCount, 
            scratchCount, 
            pocketBallCount, 
            invalidPocketBallCount
        };

        Array.Copy(values, scores, (values.Length < scores.Length ? values.Length : scores.Length));
        
        UpdateText();
    }

    /*
    /// <summary>
    /// Japan9Ball 5-9 のデコード （スコアの表示更新ではなく値を返す）未使用？
    /// </summary>
    /// <param name="value">uint</param>
    public int[] DecodedSyncValues_Frame5(uint value)
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        if (ReferenceEquals(null, table))
        {
            Debug.Log($"EIJIS PlayerRow::DecodedSyncValues_Frame5() [{GetInstanceID()}] table is null ? {ReferenceEquals(null, table)}");
            Debug.Log($"EIJIS  scoreSigned is null ? {ReferenceEquals(null, scoreSigned)}");
            Debug.Log($"EIJIS  scoreSigned.Length = {scoreSigned.Length}");
            Debug.Log($"EIJIS  scoreTexts is null ? {ReferenceEquals(null, scoreTexts)}");
            Debug.Log($"EIJIS  scoreTexts.Length = {scoreTexts.Length}");
            // Debug.Log($"EIJIS  Array.IndexOf(scoreTexts, pointText) = {Array.IndexOf(scoreTexts, pointText)}");
        }
        else
        {
            table._Log($"EIJIS PlayerRow::DecodedSyncValues_Frame5() [{GetInstanceID()}] table is null ? {ReferenceEquals(null, table)}");
            table._Log($"EIJIS  scoreSigned is null ? {ReferenceEquals(null, scoreSigned)}");
            table._Log($"EIJIS  scoreSigned.Length = {scoreSigned.Length}");
            table._Log($"EIJIS  scoreTexts is null ? {ReferenceEquals(null, scoreTexts)}");
            table._Log($"EIJIS  scoreTexts.Length = {scoreTexts.Length}");
            // table._Log($"EIJIS  Array.IndexOf(scoreTexts, pointText) = {Array.IndexOf(scoreTexts, pointText)}");
        }
#endif
        int shotCount = (int)((value >> 27) & 0x1Fu);
        int safeNoPocketShotCount = (int)((value >> 22) & 0x1Fu);
        int scratchCount = (int)((value >> 17) & 0x1Fu);
        int pocketBallCount = (int)((value >> 12) & 0x1Fu);
        int invalidPocketBallCount = (int)((value >> 7) & 0x1Fu);
        int point = (int)((value >> 0) & 0x7Fu);
        // if (scoreSigned[Array.IndexOf(scoreTexts, pointText)])
        if (scoreSigned[COL_POINT])
        {
            uint u = (value >> 0) & 0x3Fu;
            if (0 < ((value >> 0) & 0x40u))
            {
                u |= 0xFFFFFFC0u;
                u = ~u;
                point = 0 - ((int)u + 1);
            }
            else
            {
                point = (int)u;
            }
        }

        return new[] { point, shotCount, safeNoPocketShotCount, scratchCount, pocketBallCount, invalidPocketBallCount };
    }
    */

    /// <summary>
    /// Rotation のデコード
    /// </summary>
    /// <param name="value">uint</param>
    public void DecodeSyncValue_Rotation(uint value)
    {
        int foul = (int)((value >> 30) & 0x3u);
        int highRun = (int)((value >> 20) & 0x3FFu);
        int goal = (int)((value >> 10) & 0x3FFu);
        int point = (int)((value >> 0) & 0x3FFu);
                
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"EIJIS PlayerRow::DecodeSyncValue_Rotation [{GetInstanceID()}] point => {point}, goal => {goal}, highRun => {highRun}, foul => {foul}");
#endif

        scores[COL_ROTATION_POINT] = point;
        scores[COL_ROTATION_GOAL] = goal;
        scores[COL_ROTATION_HIGHRUN] = highRun;
        scores[COL_ROTATION_FOUL] = foul;

        UpdateText();
    }

    public void Init()
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        if (ReferenceEquals(null, table))
        {
            Debug.Log($"EIJIS PlayerRow::Init() [{GetInstanceID()}] table is null ? {ReferenceEquals(null, table)}");
        }
        else
        {
            table._Log($"EIJIS PlayerRow::Init() [{GetInstanceID()}] table is null ? {ReferenceEquals(null, table)}");
        }
#endif
        allTexts = new Text[scoreTexts.Length + 1];
        allTexts[0] = nameText;
        for (int i = 0; i < scoreTexts.Length; i++)
        {
            allTexts[i + 1] = scoreTexts[i];
        }
        // allTexts = new[] { nameText, pointText, shotCountText, safeNoPocketShotCountText, scratchCountText, pocketBallCountText, invalidPocketBallCountText};
        // scoreTexts = new[] { pointText, shotCountText, safeNoPocketShotCountText, scratchCountText, pocketBallCountText, invalidPocketBallCountText };
        scoreSigned = new bool[scoreTexts.Length];
        scoreEmptyTextOnZero = new bool[scoreTexts.Length];
        scoreNoChangeTextOnZero = new bool[scoreTexts.Length];
    }
    
    /// <summary>
    /// 1セルを4bitとして最大32セルをuint[4]にエンコードするサンプル
    /// </summary>
    /// <returns></returns>
    public uint[] EncodeScoreSyncValue_DefaultSample()
    {
        const int syncValueCount = 4;
        const int colCountByValue = MAX_NUMBER_COL_COUNT / syncValueCount;
        const int bitShitUnit = 4;
        
        uint[] scoreSyncValues = new uint[syncValueCount];

        for (int valIdx = 0; valIdx < scoreSyncValues.Length; valIdx++)
        {
            uint scoreSyncValue = 0x0u;
            for (int i = 0; i < colCountByValue; i++)
            {
                int colIdx = (valIdx * colCountByValue) + i; 
                int bitShift = bitShitUnit * i;
                scoreSyncValue |= (uint)((scores[colIdx] & (scoreSigned[colIdx] ? 0x7u : 0xFu)) | (scoreSigned[colIdx] && scores[colIdx] < 0 ? 0x8u : 0x0u)) << bitShift;
            }
            scoreSyncValues[valIdx] = scoreSyncValue;
        }

        return scoreSyncValues;
    }

    /// <summary>
    /// 55ball系 ( 21ball ～ 120ball ) のエンコード
    /// 14-1 のエンコード
    /// </summary>
    /// <returns></returns>
    public uint[] EncodeScoreSyncValue_ManyBall()
    {
        uint[] scoreSyncValues = new uint[2];

        uint scoreSyncValue = 0x0u;
        
        //scoreSyncValue |= (uint)((scores[COL_POINT] & 0xFFFFu) << 16);
        scoreSyncValue |= (uint)((scores[COL_POINT] & (scoreSigned[COL_POINT] ? 0x7FFFu : 0xFFFFu)) |
                                 (scoreSigned[COL_POINT] && scores[COL_POINT] < 0 ? 0x8000u : 0x0u)) << 16;
        
        scoreSyncValue |= (uint)((scores[COL_FOUL_COUNT] & 0xFFu) << 8);
        scoreSyncValue |= (uint)((scores[COL_BALL_POCKET_COUNT] & 0xFFu) << 0);
        scoreSyncValues[0] = scoreSyncValue;

        scoreSyncValue = 0x0u;
        scoreSyncValue |= (uint)((scores[COL_SHOT_COUNT] & 0xFFFu) << 20);
        scoreSyncValue |= (uint)((scores[COL_SAFE_COUNT] & 0x3FFu) << 10);
        scoreSyncValue |= (uint)((scores[COL_BALL_DEAD_COUNT] & 0x3FFu) << 0);
        scoreSyncValues[1] = scoreSyncValue;
        
        return scoreSyncValues;
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
        uint[] scoreSyncValues = new uint[2];

        uint scoreSyncValue = 0x0u;
        
        scoreSyncValue |= (uint)(
            (
                point &
                (scoreSigned[COL_POINT] ? 0x7FFFu : 0xFFFFu)) |
            (scoreSigned[COL_POINT] && point < 0 ? 0x8000u : 0x0u)
        ) << 16;
        
        scoreSyncValue |= (uint)((scratchCount & 0xFFu) << 8);
        scoreSyncValue |= (uint)((pocketBallCount & 0xFFu) << 0);
        scoreSyncValues[0] = scoreSyncValue;

        scoreSyncValue = 0x0u;
        scoreSyncValue |= (uint)((shotCount & 0xFFFu) << 20);
        scoreSyncValue |= (uint)((safeNoPocketShotCount & 0x3FFu) << 10);
        scoreSyncValue |= (uint)((invalidPocketBallCount & 0x3FFu) << 0);
        scoreSyncValues[1] = scoreSyncValue;
        
        return scoreSyncValues;
    }

    /// <summary>
    /// BankPool, OnePocket のエンコード
    /// Mnbk9Ball のエンコード
    /// </summary>
    /// <returns></returns>
    public uint EncodeScoreSyncValue_Mini()
    {
        return EncodeScoreParams_Mini(
            scores[COL_POINT],
            scores[COL_SHOT_COUNT],
            scores[COL_FOUL_COUNT],
            scores[COL_SAFE_COUNT]
            );
    }

    /// <summary>
    /// BankPool, OnePocket のエンコード ( パラメタ版 )
    /// </summary>
    /// <param name="point"></param>
    /// <param name="shotCount"></param>
    /// <param name="scratchCount"></param>
    /// <param name="safeNoPocketShotCount"></param>
    /// <returns></returns>
    public uint EncodeScoreParams_Mini(int point, int shotCount, int scratchCount, int safeNoPocketShotCount)
    {
        uint scoreSyncValue = 0x0u;
        scoreSyncValue |= (uint)((shotCount & 0xFFu) << 24);
        scoreSyncValue |= (uint)((scratchCount & 0xFFu) << 16);
        scoreSyncValue |= (uint)((safeNoPocketShotCount & 0xFFu) << 8);
        //scoreSyncValue |= (uint)(((point /* + 127 */ ) & 0xFFu) << 0);
        if (scoreSigned[COL_POINT])
        {
            scoreSyncValue |= (uint)(
                point & 0x7Fu | (point < 0 ? 0x80u : 0x0u)
            ) << 0;
        }
        else
        {
            scoreSyncValue |= (uint)(point & 0xFFu) << 0;
        }
        
        return scoreSyncValue;
    }

    /// <summary>
    /// Rotation のエンコード
    /// </summary>
    /// <returns></returns>
    public uint EncodeScoreSyncValue_Rotation()
    {
        return EncodeScoreParams_Rotation(
            scores[COL_ROTATION_POINT],
            scores[COL_ROTATION_GOAL],
            scores[COL_ROTATION_HIGHRUN],
            scores[COL_ROTATION_FOUL]
        );
    }

    /// <summary>
    /// Rotation のエンコード ( パラメタ版 )
    /// </summary>
    /// <param name="point"></param>
    /// <param name="shotCount"></param>
    /// <param name="scratchCount"></param>
    /// <param name="safeNoPocketShotCount"></param>
    /// <returns></returns>
    public uint EncodeScoreParams_Rotation(int point, int goal, int highRun, int foul)
    {
        uint scoreSyncValue = 0x0u;
        scoreSyncValue |= (uint)((foul & 0x3u) << 30);
        scoreSyncValue |= (uint)((highRun & 0x3FFu) << 20);
        scoreSyncValue |= (uint)((goal & 0x3FFu) << 10);
        scoreSyncValue |= (uint)((point & 0x3FFu) << 0);
        return scoreSyncValue;
    }

    /// <summary>
    /// Japan9Ball 5-9 のエンコード
    /// </summary>
    /// <returns></returns>
    public uint EncodeScoreSyncValue_Frame5()
    {
        return EncodeScoreParams_Frame5(
            scores[COL_JP9_TOTAL_POINT],
            new int[]
            {
                scores[COL_JP9_POINT_RACK1],
                scores[COL_JP9_POINT_RACK2],
                scores[COL_JP9_POINT_RACK3],
                scores[COL_JP9_POINT_RACK4],
                scores[COL_JP9_POINT_RACK5]
            }
        );
    }

    public uint EncodeScoreParams_Frame5(int totalPoint, int[] scores)
    {
        int[] frames = new int[5];
        for (int i = 0; i < frames.Length; i++)
        {
            if (i < scores.Length)
            {
                frames[i] = scores[i];
            }
            else
            {
                break;
            }
        }
        
        uint scoreSyncValue = 0x0u;
        scoreSyncValue |= (uint)((frames[0] & 0x1Fu) << 27);
        scoreSyncValue |= (uint)((frames[1] & 0x1Fu) << 22);
        scoreSyncValue |= (uint)((frames[2] & 0x1Fu) << 17);
        scoreSyncValue |= (uint)((frames[3] & 0x1Fu) << 12);
        scoreSyncValue |= (uint)((frames[4] & 0x1Fu) << 7);
        if (scoreSigned[COL_JP9_TOTAL_POINT])
        {
            scoreSyncValue |= (uint)(
                totalPoint & 0x3Fu | (totalPoint < 0 ? 0x40u : 0x0u)
            ) << 0;
        }
        else
        {
            scoreSyncValue |= (uint)(totalPoint & 0x7Fu) << 0;
        }
        
        return scoreSyncValue;
    }
    
    /*
    public override void OnDeserialization()
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log("EIJIS PlayerRow::OnDeserialization()");
#endif
        //UpdateText();
    }
    */

    public void UpdateText()
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        if (UpdateText_debug) table._Log($"EIJIS PlayerRow::UpdateText() start [{GetInstanceID()}]");
#endif
        if (nameText.text == string.Empty)
        {
#if DEBUG_EIJIS_SCORE_SCREEN
            if (UpdateText_debug) table._Log($"EIJIS PlayerRow::UpdateText() return");
#endif
            return;
        }   
        
        foreach (Text textComponent in scoreTexts)
        {
            if (ReferenceEquals(null, textComponent))
            {
                continue;
            }

            int score = scores[Array.IndexOf(scoreTexts, textComponent)];
            
            bool noChangeTextOnZero = scoreNoChangeTextOnZero[Array.IndexOf(scoreTexts, textComponent)];
            if (noChangeTextOnZero && (score == 0))
            {
                continue;
            }
            
            bool emptyTextOnZero = scoreEmptyTextOnZero[Array.IndexOf(scoreTexts, textComponent)];
            textComponent.text = (emptyTextOnZero && (score == 0)) ? "" : $"{score}";
#if DEBUG_EIJIS_SCORE_SCREEN
            if (UpdateText_debug) table._Log($"EIJIS   index = {Array.IndexOf(scoreTexts, textComponent)}, score = {score}");
#endif
        }
#if DEBUG_EIJIS_SCORE_SCREEN
        if (UpdateText_debug) table._Log($"EIJIS PlayerRow::UpdateText() end");
#endif
    }

    public void Clear(bool setScoreTextZero)
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        if (ReferenceEquals(null, table))
        {
            Debug.Log($"EIJIS PlayerRow::Clear( setScoreTextZero = {setScoreTextZero} ) [{GetInstanceID()}] table is null ? {ReferenceEquals(null, table)}");
        }
        else
        {
            table._Log($"EIJIS PlayerRow::Clear( setScoreTextZero = {setScoreTextZero} ) [{GetInstanceID()}] table is null ? {ReferenceEquals(null, table)}");
        }
#endif
        if (setScoreTextZero)
        {
            foreach (Text textComponent in scoreTexts)
            {
                if (ReferenceEquals(null, textComponent))
                {
                    continue;
                }
                textComponent.text = "0";
                //scoreDict[textComponent] = 0;
            }
        }
        else
        {
            foreach (Text textComponent in allTexts)
            {
                if (ReferenceEquals(null, textComponent))
                {
                    continue;
                }
                textComponent.text = string.Empty;
            }
        }
        for (int i = 0; i < scores.Length; i++)
        {
            scores[i] = 0;
        }
    }
    
    public void SetName(string str)
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        if (ReferenceEquals(null, table))
        {
            Debug.Log($"EIJIS PlayerRow::SetName() [{GetInstanceID()}] table is null ? {ReferenceEquals(null, table)}, nameText => {nameText.text}, str => {str}");
        }
        else
        {
            table._Log($"EIJIS PlayerRow::SetName() [{GetInstanceID()}] table is null ? {ReferenceEquals(null, table)}, nameText => {nameText.text}, str => {str}");
        }
#endif
        if (nameText.text == string.Empty && str != string.Empty)
        {
            Clear(true);
        }
        else if (nameText.text != string.Empty && str == string.Empty)
        {
            Clear(false);
        }
        nameText.text = str;
    }

    public string GetName()
    {
        return nameText.text;
    }
    
    /// <summary>
    /// 55ball系 ( 21ball ～ 120ball ) のスコア更新
    /// </summary>
    /// <param name="isScratch"></param>
    /// <param name="isFoul"></param>
    /// <param name="pocketCount"></param>
    /// <param name="pointCount"></param>
    /// <param name="shotCount"></param>
    public void ScoreUpdate_ManyBall(
        bool isScratch,
        bool isFoul,
        int pocketCount,
        int pointCount,
        int shotCount
    )
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"EIJIS PlayerRow::ScoreUpdate_ManyBall() [{GetInstanceID()}]");
        //Debug.Log("EIJIS PlayerRow::ScoreUpdate_ManyBall()");
#endif
        if (scores[COL_SHOT_COUNT] < 0xFFF)
        {
            scores[COL_SHOT_COUNT] += shotCount;
        }
        
        if (scores[COL_POINT] < 0xFFFF)
        {
            scores[COL_POINT] += pointCount;
        }
        
        if (isScratch || isFoul) // とりあえず、ノータッチをファウルとしてここにカウントする
        {
            if (scores[COL_FOUL_COUNT] < 0xFF)
            {
                scores[COL_FOUL_COUNT]++;
            }
        }

        if (isFoul)
        {
            if (scores[COL_BALL_DEAD_COUNT] < 0x3FF)
            {
                scores[COL_BALL_DEAD_COUNT] += pocketCount;
            }
        }
        else
        {
            if (scores[COL_BALL_POCKET_COUNT] < 0x3FF)
            {
                scores[COL_BALL_POCKET_COUNT] += pocketCount;
            }
        }

        if (0 == pocketCount && !isFoul)
        {
            if (scores[COL_SAFE_COUNT] < 0x3FF)
            {
                scores[COL_SAFE_COUNT]++;
            }
        }

        UpdateText();
    }

    public void setFrameLength_Bowlards(int goalFrameNumber)
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"EIJIS PlayerRow::setFrameLength_Bowlards(goalFrameNumber = {goalFrameNumber}) [{GetInstanceID()}]");
#endif

#if DEBUG_EIJIS_SCORE_SCREEN
        bool[] debugVisibles = new bool[scoreTexts.Length];
#endif
        int count = 0;
        for (int i = scoreTexts.Length - 1; 0 <= i; i--)
        {
            bool hide = goalFrameNumber <= count;
#if DEBUG_EIJIS_SCORE_SCREEN
            // table._Log($"EIJIS_DEBUG  i = {i}, hide = {hide}");
            debugVisibles[i] = !hide;
#endif
            Text scoreText = scoreTexts[i];
            if (ReferenceEquals(null, scoreText))
            {
                continue;
            }
            scoreText.transform.parent.parent.gameObject.SetActive(!hide);
            count++;
            if (10 <= count)
            {
                break;
            }
        }
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log(
            (debugVisibles[0]?"*":"-")+"  "+
            (debugVisibles[1]?"*":"-")+"  "+
            (debugVisibles[2]?"*":"-")+"  "+
            (debugVisibles[3]?"*":"-")+"  "+
            (debugVisibles[4]?"*":"-")+"  "+
            (debugVisibles[5]?"*":"-")+"  "+
            (debugVisibles[6]?"*":"-")+"  "+
            (debugVisibles[7]?"*":"-")+"  "+
            (debugVisibles[8]?"*":"-")+"  "+
            (debugVisibles[9]?"*":"-")
        );
#endif
    }   
    
    /// <summary>
    /// Bowlards のスコア更新
    /// </summary>
    public void ScoreUpdate_Bowlards(int frameCount, int throwInningCount, byte[] framePoints)
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"EIJIS PlayerRow::ScoreUpdate_Bowlards(frameCount = {frameCount}, throwInningCount = {throwInningCount}, framePoints.Length = {framePoints.Length}) [{GetInstanceID()}]");
#endif
        
        int[] pointByFrameNoBonus = new int[12];
        int[] bonusTypeByFrame = new int[12];
        byte[] pointByInning = new byte[21];
        byte additional = (byte)((framePoints[10] & 0x0F) >> 0);
#if DEBUG_EIJIS_SCORE_SCREEN
        // table._Log($"EIJIS_DEBUG  additional = {additional}");
#endif
        for (int i = 0; i < 10; i++)
        {
            bool isLastFrame = (i == 9);
            byte first = (byte)((framePoints[i] & 0x0F) >> 0);
            byte second = (byte)((framePoints[i] & 0xF0) >> 4);
            byte third = (byte)(isLastFrame ? additional : 0);
            
            pointByInning[i * 2] = first;
            pointByInning[(i * 2) + 1] = second;
            if (isLastFrame) pointByInning[20] = third;

            int subTotal = first + second;
#if DEBUG_EIJIS_SCORE_SCREEN
            // table._Log($"EIJIS_DEBUG  i = {i}, isLastFrame = {isLastFrame}, first = {first}, second = {second}, third = {third}, subTotal = {subTotal}");
#endif
            if (10 <= first)
            {
                bonusTypeByFrame[i] = 2;
                if (isLastFrame)
                {
                    subTotal = first;
                    pointByFrameNoBonus[10] = second;
                    pointByFrameNoBonus[11] = third;
                    if (10 <= second)
                    {
                        // pointByFrameNoBonus[10] = second;
                        // pointByFrameNoBonus[11] = third;
                        bonusTypeByFrame[10] = 2;
                        if (10 <= third)
                        {
                            bonusTypeByFrame[11] = 2;
                        }
                    }
                    else if (10 <= second + third)
                    {
                        // pointByFrameNoBonus[10] = second + third;
                        bonusTypeByFrame[11] = 1;
                    }
                }
            }
            else if (10 <= subTotal)
            {
                if (isLastFrame)
                {
                    pointByFrameNoBonus[10] = third;
                    bonusTypeByFrame[10] = 1;
                    if (10 <= third)
                    {
                        bonusTypeByFrame[11] = 2;
                    }
                }
                else
                {
                    bonusTypeByFrame[i] = 1;
                }
            }
            else
            {
                bonusTypeByFrame[i] = 0;
            }

            pointByFrameNoBonus[i] = subTotal;
        }

        int fixedFrame = frameCount - 1;
        if (fixedFrame < 9)
        {
            if (1 <= fixedFrame && 2 == bonusTypeByFrame[fixedFrame] && 2 == bonusTypeByFrame[fixedFrame - 1])
            {
                fixedFrame -= 2;
            }
            else if (0 <= fixedFrame)
            {
                if (2 == bonusTypeByFrame[fixedFrame] || (1 == bonusTypeByFrame[fixedFrame] && throwInningCount < 1))
                {
                    fixedFrame -= 1;
                }
            }
        }
            
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"EIJIS_DEBUG  fixedFrame = {fixedFrame}");
#endif

        int[] pointByFrame = new int[10];
        for (int i = 0; i <= fixedFrame; i++)
        {
            if (pointByFrame.Length <= i) break;
            
            pointByFrame[i] = pointByFrameNoBonus[i];
            if (1 == bonusTypeByFrame[i])
            {
                pointByFrame[i] += pointByInning[((i + 1) * 2)];
            }
            else if (2 == bonusTypeByFrame[i])
            {
                // pointByFrame[i] += pointByFrameNoBonus[i + 1] + pointByFrameNoBonus[i + 2];
                pointByFrame[i] += pointByFrameNoBonus[i + 1];
                if (2 == bonusTypeByFrame[i + 1])
                {
                    if (2 == bonusTypeByFrame[i + 2])
                    {
                        pointByFrame[i] += pointByFrameNoBonus[i + 2];
                    }
                    else if (i < 9)
                    {
                        pointByFrame[i] += pointByInning[((i + 2) * 2)];
                    }
                }
            }

            if (9 <= i)
            {
                if (1 == bonusTypeByFrame[10])
                {
                    pointByFrame[9] += pointByFrameNoBonus[10];
                }
                else if (0 == bonusTypeByFrame[10] || (2 == bonusTypeByFrame[10] && pointByFrameNoBonus[11] < 10))
                {
                    pointByFrame[9] += pointByFrameNoBonus[11];
                }
            }
        }
        
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log(
            $"{pointByInning[0]:X}{pointByInning[1]:X} "+
            $"{pointByInning[2]:X}{pointByInning[3]:X} "+
            $"{pointByInning[4]:X}{pointByInning[5]:X} "+
            $"{pointByInning[6]:X}{pointByInning[7]:X} "+
            $"{pointByInning[8]:X}{pointByInning[9]:X} "+

            $"{pointByInning[10]:X}{pointByInning[11]:X} "+
            $"{pointByInning[12]:X}{pointByInning[13]:X} "+
            $"{pointByInning[14]:X}{pointByInning[15]:X} "+
            $"{pointByInning[16]:X}{pointByInning[17]:X} "+
            $"{pointByInning[18]:X}{pointByInning[19]:X}{pointByInning[20]:X}"
        );
        table._Log(
            $"{pointByFrameNoBonus[0],2:D} "+
            $"{pointByFrameNoBonus[1],2:D} "+
            $"{pointByFrameNoBonus[2],2:D} "+
            $"{pointByFrameNoBonus[3],2:D} "+
            $"{pointByFrameNoBonus[4],2:D} "+
            $"{pointByFrameNoBonus[5],2:D} "+
            $"{pointByFrameNoBonus[6],2:D} "+
            $"{pointByFrameNoBonus[7],2:D} "+
            $"{pointByFrameNoBonus[8],2:D} "+
            $"{pointByFrameNoBonus[9],2:D}("+
            $"{pointByFrameNoBonus[10],2:D} "+
            $"{pointByFrameNoBonus[11],2:D})"
        );
        table._Log(
            $"{pointByFrame[0],2:D} "+
            $"{pointByFrame[1],2:D} "+
            $"{pointByFrame[2],2:D} "+
            $"{pointByFrame[3],2:D} "+
            $"{pointByFrame[4],2:D} "+
            $"{pointByFrame[5],2:D} "+
            $"{pointByFrame[6],2:D} "+
            $"{pointByFrame[7],2:D} "+
            $"{pointByFrame[8],2:D} "+
            $"{pointByFrame[9],2:D}"
        );
#endif

        // scoreSyncRow.SetEmptyTextOnZero(true);
        bool[] scoreEmptyTextOnZeroArray = new bool[31]; // scoreTexts.Length
        for (int i = 0; i < 10; i++)
        {
#if true // 0_POINT_TO_G
            scoreEmptyTextOnZeroArray[i * 2] = !(fixedFrame < i && i == frameCount && 0 == throwInningCount);
            scoreEmptyTextOnZeroArray[(i * 2) + 1] = !(fixedFrame < i && i == frameCount && 1 == throwInningCount);
#else
            scoreEmptyTextOnZeroArray[i * 2] = !((i < frameCount || (i == frameCount && 0 <= throwInningCount)) && (bonusTypeByFrame[i] != 2));
            scoreEmptyTextOnZeroArray[(i * 2) + 1] = !((i < frameCount || (i == frameCount && 1 <= throwInningCount)) && (bonusTypeByFrame[i] < 1));
#endif
            scoreEmptyTextOnZeroArray[i + 21] = !(i <= fixedFrame);
        }
        scoreEmptyTextOnZeroArray[20] = !(9 <= frameCount && 2 <= throwInningCount);
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log(
            (scoreEmptyTextOnZeroArray[0]?"_":"*")+(scoreEmptyTextOnZeroArray[1]?"_":"*")+" "+
            (scoreEmptyTextOnZeroArray[2]?"_":"*")+(scoreEmptyTextOnZeroArray[3]?"_":"*")+" "+
            (scoreEmptyTextOnZeroArray[4]?"_":"*")+(scoreEmptyTextOnZeroArray[5]?"_":"*")+" "+
            (scoreEmptyTextOnZeroArray[6]?"_":"*")+(scoreEmptyTextOnZeroArray[7]?"_":"*")+" "+
            (scoreEmptyTextOnZeroArray[8]?"_":"*")+(scoreEmptyTextOnZeroArray[9]?"_":"*")+" "+

            (scoreEmptyTextOnZeroArray[10]?"_":"*")+(scoreEmptyTextOnZeroArray[11]?"_":"*")+" "+
            (scoreEmptyTextOnZeroArray[12]?"_":"*")+(scoreEmptyTextOnZeroArray[13]?"_":"*")+" "+
            (scoreEmptyTextOnZeroArray[14]?"_":"*")+(scoreEmptyTextOnZeroArray[15]?"_":"*")+" "+
            (scoreEmptyTextOnZeroArray[16]?"_":"*")+(scoreEmptyTextOnZeroArray[17]?"_":"*")+" "+
            (scoreEmptyTextOnZeroArray[18]?"_":"*")+(scoreEmptyTextOnZeroArray[19]?"_":"*")+(scoreEmptyTextOnZeroArray[20]?"_":"*")
            );
        table._Log(
        "  "+
           (scoreEmptyTextOnZeroArray[21]?"_":"*")+"  "+
           (scoreEmptyTextOnZeroArray[22]?"_":"*")+"  "+
           (scoreEmptyTextOnZeroArray[23]?"_":"*")+"  "+
           (scoreEmptyTextOnZeroArray[24]?"_":"*")+"  "+
           (scoreEmptyTextOnZeroArray[25]?"_":"*")+"  "+
           (scoreEmptyTextOnZeroArray[26]?"_":"*")+"  "+
           (scoreEmptyTextOnZeroArray[27]?"_":"*")+"  "+
           (scoreEmptyTextOnZeroArray[28]?"_":"*")+"  "+
           (scoreEmptyTextOnZeroArray[29]?"_":"*")+"  "+
           (scoreEmptyTextOnZeroArray[30]?"_":"*")
        );
        table._Log(
            $" {bonusTypeByFrame[0]} "+
            $" {bonusTypeByFrame[1]} "+
            $" {bonusTypeByFrame[2]} "+
            $" {bonusTypeByFrame[3]} "+
            $" {bonusTypeByFrame[4]} "+
            $" {bonusTypeByFrame[5]} "+
            $" {bonusTypeByFrame[6]} "+
            $" {bonusTypeByFrame[7]} "+
            $" {bonusTypeByFrame[8]} "+
            $" {bonusTypeByFrame[9]} "+
            $"( {bonusTypeByFrame[10]}  {bonusTypeByFrame[11]})"
        );
#endif
        SetEmptyTextOnZeroByArray(scoreEmptyTextOnZeroArray);

        int j = 0;
        for (int i = 0; i < pointByInning.Length; i++)
        {
            if (ReferenceEquals(null, scores[j])) continue;
            if (scoreTexts.Length <= j) break;

            int frame = i / 2;
            int inningByFrame = i % 2;
            int displayInningPoint = pointByInning[i];
                
            if ((i <= 18 && bonusTypeByFrame[frame] == 1 && inningByFrame == 1) || 
                (i <= 18 && bonusTypeByFrame[frame] == 2 && inningByFrame == 0) ||
                (i == 19 && (bonusTypeByFrame[10] == 1 || bonusTypeByFrame[10] == 2)) ||
                (i == 20 && (bonusTypeByFrame[11] == 1 || bonusTypeByFrame[11] == 2)))
            {
                displayInningPoint = 0;
            }

            scores[j++] = displayInningPoint;
        }

        int total = 0;
        for (int i = 0; i <= fixedFrame; i++)
        {
            if (pointByFrame.Length <= i) break;
            if (ReferenceEquals(null, scores[j])) continue;
            if (scoreTexts.Length <= j) break;

            total += pointByFrame[i];
            scores[j++] = total;
        }

        while (j < scores.Length)
        {
            if (ReferenceEquals(null, scores[j])) continue;
            scores[j++] = 0;
        }
        
        j = 0;
        for (int i = 0; i < bonusTypeByFrame.Length; i++)
        {
            if (ReferenceEquals(null, mark1[j])) continue;
            if (mark1.Length <= j) break;

#if DEBUG_EIJIS_SCORE_SCREEN
            // table._Log($"EIJIS_DEBUG  mark1[{j}] bonusTypeByFrame[{i}] = {bonusTypeByFrame[i]}");
#endif
            mark1[j++].SetActive(bonusTypeByFrame[i] == 1);
        }
        
        j = 0;
        for (int i = 0; i < bonusTypeByFrame.Length; i++)
        {
            if (ReferenceEquals(null, mark2[j])) continue;
            if (mark2.Length <= j) break;

#if DEBUG_EIJIS_SCORE_SCREEN
            // table._Log($"EIJIS_DEBUG  mark2[{j}] bonusTypeByFrame[{i}] = {bonusTypeByFrame[i]}");
#endif
            mark2[j++].SetActive(bonusTypeByFrame[i] == 2);
        }

        int currentInning = (frameCount * 2) + throwInningCount;
        for (int i = 0; i < mark3.Length; i++)
        {
            if (ReferenceEquals(null, mark3[i])) continue;
            mark3[i].SetActive(0 <= throwInningCount && i == currentInning);
        }

#if DEBUG_EIJIS_SCORE_SCREEN
        // UpdateText_debug = true;
#endif
        UpdateText();
#if DEBUG_EIJIS_SCORE_SCREEN
        // UpdateText_debug = false;
#endif
        
        j = 0;
        for (int i = 0; i < pointByInning.Length; i++)
        {
            if (ReferenceEquals(null, scores[j])) continue;
            if (scoreTexts.Length <= j) break;

            int frame = i / 2;
            int inningByFrame = i % 2;
            int displayInningPoint = pointByInning[i];
                
            // if (displayInningPoint == 0 && (frame <= fixedFrame || (frame == frameCount && inningByFrame < throwInningCount)))
            if (displayInningPoint == 0 && (frame < frameCount || (frame == frameCount && inningByFrame < throwInningCount) || 10 <= frameCount))
            {
                if (!ReferenceEquals(null, scoreTexts[j]))
                {
                    if (inningByFrame == 0 && frame < 10)
                    {
                        scoreTexts[j].text = "G";
                    }
                    else if (bonusTypeByFrame[frame] != 2 && inningByFrame == 1)
                    {
                        scoreTexts[j].text = "-";
                    }
                    else if (j == 20 && bonusTypeByFrame[frame] != 0)
                    {
                        scoreTexts[j].text = "-";
                    }
                    else
                    {
                        scoreTexts[j].text = String.Empty;
                    }
                }
            }

            j++;
        }
    }

    public int GetPoint()
    {
        return scores[COL_POINT];
    }

    public int GetScratchCount()
    {
        return scores[COL_FOUL_COUNT];
    }
    
    public int GetPocketBallCount()
    {
        return scores[COL_BALL_POCKET_COUNT];
    }

    public int GetShotCount()
    {
        return scores[COL_SHOT_COUNT];
    }

    public int GetSafeNoPocketShotCount()
    {
        return scores[COL_SAFE_COUNT];
    }

    public int GetInvalidPocketBallCount()
    {
        return scores[COL_BALL_DEAD_COUNT];
    }

    public int GetRotationPoint()
    {
        return scores[COL_ROTATION_POINT];
    }

    public int GetRotationGoal()
    {
        return scores[COL_ROTATION_GOAL];
    }

    public int GetRotationHighRun()
    {
        return scores[COL_ROTATION_HIGHRUN];
    }

    public int GetRotationFoul()
    {
        return scores[COL_ROTATION_FOUL];
    }

    public int[] GetScores()
    {
        return (int[])(scores.Clone());
    }

    public string[] GetScoreTextStrings()
    {
        string[] scoreTextStrings = new string[scoreTexts.Length];
        int i = 0;
        foreach (Text textComponent in scoreTexts)
        {
            scoreTextStrings[i++] = ReferenceEquals(null, textComponent) ? "" : textComponent.text;
        }
        return scoreTextStrings;
    }

    public Text GetScoreTextByColIndex(int colIndex)
    {
        return scoreTexts[colIndex];
    }

    public int GetScoreByColIndex(int colIndex)
    {
        return scores[colIndex];
    }

    public void SetScoreByColIndex(int value, int colIndex)
    {
        scores[colIndex] = value;
        UpdateText();
    }
    
    public void SetTextByColIndex(string text, int colIndex)
    {
        scoreTexts[colIndex].text = text;
    }

    public void SetPointSigned(bool signed)
    {
        scoreSigned[COL_POINT] = signed;
    }

    public void SetPointSignedByArray(bool[] signedArray)
    {
        Array.Copy(signedArray, scoreSigned, scoreSigned.Length);
        // UpdateText();
    }

    public void SetScoreByArray(int[] scoreArray)
    {
        Array.Copy(scoreArray, scores, scores.Length);
        UpdateText();
    }

    public void SetSafeNoPocketShotCountEmptyTextOnZero(bool emptyTextOnZero)
    {
        scoreEmptyTextOnZero[COL_SAFE_COUNT] = emptyTextOnZero;
        UpdateText();
    }
    
    public void SetInvalidPocketBallCountEmptyTextOnZero(bool emptyTextOnZero)
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"EIJIS PlayerRow::SetInvalidPocketBallCountEmptyTextOnZero() [{GetInstanceID()}]");
        //Debug.Log("EIJIS PlayerRow::SetInvalidPocketBallCountEmptyTextOnZero()");
#endif
        scoreEmptyTextOnZero[COL_BALL_DEAD_COUNT] = emptyTextOnZero;
        UpdateText();
    }
    
    public void SetEmptyTextOnZero(bool emptyTextOnZero)
    {
        for (int i = 0; i < scoreEmptyTextOnZero.Length; i++)
        {
            scoreEmptyTextOnZero[i] = emptyTextOnZero;
        }
        UpdateText();
    }

    public void SetEmptyTextOnZeroByColIndex(bool emptyTextOnZero, int colIndex)
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"EIJIS PlayerRow::SetEmptyTextOnZeroByColIndex(emptyTextOnZero = {colIndex}, emptyTextOnZero = {colIndex}) [{GetInstanceID()}]");
        table._Log($"EIJIS  scoreTexts.name = {scoreTexts[colIndex].name}");
#endif
        scoreEmptyTextOnZero[colIndex] = emptyTextOnZero;
        UpdateText();
    }
    
    public void SetEmptyTextOnZeroByArray(bool[] emptyTextOnZeroArray)
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"EIJIS PlayerRow::SetEmptyTextOnZeroByArray(emptyTextOnZeroArray = {emptyTextOnZeroArray}) [{GetInstanceID()}]");
#endif
        scoreEmptyTextOnZero = emptyTextOnZeroArray;
        UpdateText();
    }
    
    public void SetNoChangeTextOnZero(bool noChangeTextOnZero)
    {
        for (int i = 0; i < scoreNoChangeTextOnZero.Length; i++)
        {
            scoreNoChangeTextOnZero[i] = noChangeTextOnZero;
        }
    }

    public void SetNoChangeTextOnZeroByColIndex(bool noChangeTextOnZero, int colIndex)
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"EIJIS PlayerRow::SetNoChangeTextOnZeroByColIndex(noChangeTextOnZero = {noChangeTextOnZero}, colIndex = {colIndex}) [{GetInstanceID()}]");
        table._Log($"EIJIS  scoreTexts.Length = {scoreTexts.Length}");
        table._Log($"EIJIS  scoreTexts[colIndex = {colIndex}] is null = {ReferenceEquals(null, scoreTexts[colIndex])}");
        table._Log($"EIJIS  scoreTexts[colIndex = {colIndex}].name = {scoreTexts[colIndex].name}");
#endif
        scoreNoChangeTextOnZero[colIndex] = noChangeTextOnZero;
    }
    
    public void SetNoChangeTextOnZeroByArray(bool[] noChangeTextOnZeroArray)
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"EIJIS PlayerRow::SetNoChangeTextOnZeroByArray(noChangeTextOnZeroArray = {noChangeTextOnZeroArray}) [{GetInstanceID()}]");
#endif
        scoreNoChangeTextOnZero = noChangeTextOnZeroArray;
    }

    public void SetGoalPoint_Rotation(int goalPoint)
    {
        scores[COL_ROTATION_GOAL] = goalPoint;
        UpdateText();
    }

    public void SetInningCount_Rotation(int inningCount)
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"EIJIS PlayerRow::SetInningCount_Rotation(inningCount = {inningCount}) [{GetInstanceID()}]");
#endif
        scores[COL_ROTATION_FOOT_INNING] = inningCount;
#if DEBUG_EIJIS_SCORE_SCREEN
        UpdateText_debug = true;
#endif
        UpdateText();
#if DEBUG_EIJIS_SCORE_SCREEN
        UpdateText_debug = false;
#endif
    }
    
    public void SetInfoText(string message)
    {
        scoreTexts[COL_FOOTER_INFO_TEXT].text = message;
    }

    public void SetRegulationText(string message)
    {
        scoreTexts[COL_FOOTER_REGULATION_TEXT].text = message;
    }
    
    public int GetBowlardsFinalScore()
    {
#if DEBUG_EIJIS_SCORE_SCREEN
        table._Log($"EIJIS PlayerRow::GetBowlardsFinalScore() [{GetInstanceID()}]");
        table._Log($"EIJIS  scores.Length = {scores.Length}");
        for (int i = 0; i < scores.Length; i++)
        {
            // table._Log($"EIJIS  scores[i = {i}] is null = {ReferenceEquals(null, scores[i])}");
            table._Log($"EIJIS  scores[i = {i}] = {scores[i]}");
        }
#endif

        if (scores.Length <= COL_BOWLARDS_FINAL_SCORE || ReferenceEquals(null, scores[COL_BOWLARDS_FINAL_SCORE])) return -1;
        return scores[COL_BOWLARDS_FINAL_SCORE];
    }
}
