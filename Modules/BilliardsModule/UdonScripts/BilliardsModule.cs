﻿#define EIJIS_ISSUE_FIX
#define EIJIS_TABLE_LABEL
#define EIJIS_MANY_BALLS
#define EIJIS_SNOOKER15REDS
#define EIJIS_PYRAMID
#define EIJIS_CUEBALLSWAP
#define EIJIS_CAROM
#define EIJIS_CUSHION_EFFECT
#define EIJIS_GUIDELINE2TOGGLE
#define EIJIS_PUSHOUT
#define EIJIS_CALLSHOT
// #define EIJIS_CALL_MARKER_TO_GRAY_ON_CUE_TRIGGER_ACTIVATE
#define EIJIS_CALL_DEFAULT_LOCKING
#define EIJIS_SEMIAUTOCALL
#define EIJIS_10BALL
#define CHEESE_ISSUE_FIX
#define EIJIS_BANKING
#define EIJIS_LOG_PREFIX_COLOR_OFF
#define EIJIS_EXTERNAL_SCORE_SCREEN
#define EIJIS_EXTRA_GAMES
#define EIJIS_ROTATION
#define EIJIS_BOWLARDS

// #define EIJIS_DEFAULT_MODE_CHANGE

// #define EIJIS_DEBUG_INITIALIZERACK
// #define EIJIS_DEBUG_BALLCHOICE
// #define EIJIS_DEBUG_PIRAMIDSCORE
// #define EIJIS_DEBUG_CUSHIONTOUCH
// #define EIJIS_DEBUG_PUSHOUT
// #define EIJIS_DEBUG_AFTERBREAK
// #define EIJIS_DEBUG_CALLSHOT_BALL
// #define EIJIS_DEBUG_CALLSHOT_MARKER
#define EIJIS_DEBUG_BREAKINGFOUL
//#define EIJIS_DEBUG_SNOOKER_COLOR_POINT
// #define EIJIS_DEBUG_10BALL_WPA_RULE
// #define EIJIS_DEBUG_SEMIAUTO_CALL
// #define EIJIS_DEBUG_SEMIAUTO_CALL_FINDLOGIC
// #define EIJIS_DEBUG_SEMIAUTO_CALL_AFTER_REPOSITION
// #define EIJIS_DEBUG_BANKING
// #define EIJIS_CALLSHOT_ALLOW_UNSELECT
// #define EIJIS_DEBUG_SCORE_SCREEN
// #define EIJIS_DEBUG_FOUL_STATE
// #define EIJIS_DEBUG_CALL_SAFETY
// #define EIJIS_DEBUG_CALLSHOT_TURNPASS_OPTION
// #define EIJIS_DEBUG_UNDO_REDO
// #define EIJIS_DEBUG_BALL_TOUCHING
// #define EIJIS_DEBUG_TEAM_ID
// #define EIJIS_DEBUG_BOWLARDS
// #define EIJIS_DEBUG_CALLCLEAR

#if UNITY_ANDROID
#define HT_QUEST
#endif

#if !HT_QUEST || true
#define HT8B_DEBUGGER
#endif

#if UDON_CHIPS
using UCS;
#endif

using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using System;
using Metaphira.Modules.CameraOverride;
using TMPro;
using Cheese;
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class BilliardsModule : UdonSharpBehaviour
{
    [NonSerialized] public readonly string[] DEPENDENCIES = new string[] { nameof(CameraOverrideModule) };
#if EIJIS_SNOOKER15REDS || EIJIS_PYRAMID || EIJIS_CAROM || EIJIS_10BALL || EIJIS_ROTATION || EIJIS_BOWLARDS
    [NonSerialized] public readonly string VERSION = "6.0.0 (15Reds|Pyramid|Carom|10Ball|Rotation|Bowlards)";
#else
    [NonSerialized] public readonly string VERSION = "6.0.0";
#endif

    #region PhysicsVariables

    // table model properties
    [NonSerialized] public float k_TABLE_WIDTH; // horizontal span of table
    [NonSerialized] public float k_TABLE_HEIGHT; // vertical span of table
    [NonSerialized] public float k_CUSHION_RADIUS; // The roundess of colliders
    [NonSerialized] public float k_POCKET_WIDTH_CORNER; // Radius of pockets
    [NonSerialized] public float k_POCKET_HEIGHT_CORNER; // Radius of pockets
    [NonSerialized] public float k_POCKET_RADIUS_SIDE; // Radius of side pockets
    [NonSerialized] public float k_POCKET_DEPTH_SIDE; // Depth of side pockets
    [NonSerialized] public float k_INNER_RADIUS_CORNER; // Pocket 'hitbox' cylinder
    [NonSerialized] public float k_INNER_RADIUS_SIDE; // Pocket 'hitbox' cylinder for corner pockets
    [NonSerialized] public float k_FACING_ANGLE_CORNER; // Angle of corner pocket inner walls
    [NonSerialized] public float k_FACING_ANGLE_SIDE; // Angle of side pocket inner walls
    [NonSerialized] public float K_BAULK_LINE; // Snooker baulk line distance from end of table
    [NonSerialized] public float K_BLACK_SPOT; // Snooker Black ball distance from end of table
    [NonSerialized] public float k_SEMICIRCLERADIUS; // Snooker, radius of D
    [NonSerialized] public float k_RAIL_HEIGHT_UPPER;
    [NonSerialized] public float k_RAIL_HEIGHT_LOWER;
    [NonSerialized] public float k_RAIL_DEPTH_WIDTH;
    [NonSerialized] public float k_RAIL_DEPTH_HEIGHT;
    // advanced physics  variables
    [NonSerialized] public float k_F_SLIDE; // bt_CoefSlide
    [NonSerialized] public float k_F_ROLL; // bt_CoefRoll
    [NonSerialized] public float k_F_SPIN; // bt_CoefSpin
    [NonSerialized] public float k_F_SPIN_RATE; // bt_CoefSpinRate
    [NonSerialized] public bool useRailLower; // useRailHeightLower
    [NonSerialized] public bool isDRate; // bt_isDRate
    [NonSerialized] public float K_BOUNCE_FACTOR; // BounceFactor
    [NonSerialized] public float k_POCKET_RESTITUTION; // Reduces bounce inside of pockets
    [Header("Cushion Model:")]
    [NonSerialized] public bool isHanModel; // bc_UseHan05
    [NonSerialized] public float k_E_C; // bc_CoefRestitution
    [NonSerialized] public bool isDynamicRestitution; // bc_DynRestitution
    [NonSerialized] public bool isCushionFrictionConstant; // bc_UseConstFriction
    [NonSerialized] public float k_Cushion_MU; // bc_ConstFriction
    [Header("Ball Set Configuration:")]
    [NonSerialized] public float k_BALL_E; // bs_CoefRestitution
    [NonSerialized] public float muFactor; // bs_Friction
    [NonSerialized] public float k_BALL_RADIUS; // Radius of balls
    [NonSerialized] public float k_BALL_MASS; // Mass of balls
    [NonSerialized] public float k_BALL_DIAMETRE; // Diameter of balls
    [NonSerialized] public Vector3 k_vE; // corner pocket data
    [NonSerialized] public Vector3 k_vF; // side pocket data
    [NonSerialized] public Vector3 k_rack_position = new Vector3();
    private Vector3 k_rack_direction = new Vector3();
    private GameObject auto_rackPosition;
    [NonSerialized] public GameObject auto_pocketblockers;
    private GameObject auto_colliderBaseVFX;
    [NonSerialized] public MeshRenderer[] tableMRs;
#if EIJIS_CALLSHOT
    [NonSerialized] public GameObject[] pointPocketMarkers;
    [NonSerialized] public GameObject[] pointPocketMarkerCalled;
    [NonSerialized] public GameObject[] pointPocketMarkerNoCall;
#if EIJIS_SEMIAUTOCALL
    private float findNearestPocket_x;
    private float findNearestPocket_n;
    private readonly float semiAutoCallDelay = 0.2f;
#endif
#endif

    #endregion

    // cue guideline
    private readonly Color k_aimColour_aim = new Color(0.7f, 0.7f, 0.7f, 1.0f);
    private readonly Color k_aimColour_locked = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    // textures
    [SerializeField] public Texture[] textureSets;
    [SerializeField] public ModelData[] tableModels;
    [SerializeField] public Texture2D[] tableSkins;
    [SerializeField] public Texture2D[] cueSkins;

    // hooks
    [NonSerialized] public UdonBehaviour tableSkinHook;//no need to use

    [Header("CBC_Plug")]
    [SerializeField] public TableHook tableHook;
    [Space(3)]
    [SerializeField] public UdonBehaviour nameColorHook;
    [SerializeField] public ScoreManagerV2 ScoreManager;
    [SerializeField] public Translations _translations;
    [SerializeField] public PersonalDataCounter personalData;
    [SerializeField] public UdonBehaviour DG_LAB;    //芝士郊狼联动

    // globals
    [NonSerialized] public AudioSource aud_main;
    [NonSerialized] public UdonBehaviour callbacks;
#if EIJIS_PYRAMID || EIJIS_CAROM || EIJIS_10BALL || EIJIS_BANKING || EIJIS_ROTATION || EIJIS_BOWLARDS
    private Vector3[][] initialPositions = new Vector3[20][];
    private uint[] initialBallsPocketed = new uint[20];
#else
    private Vector3[][] initialPositions = new Vector3[5][];
    private uint[] initialBallsPocketed = new uint[5];
#endif

#if UDON_CHIPS
    //udon Chips
    [Header("Udon Chips")]
    [SerializeField] public int Enter_cost;
    [SerializeField] public int winner_gain;
    [SerializeField] public int loser_lose;
    private UCS.UdonChips udonChips = null;
#endif


    #region BallModeSetting

    // constants
#if EIJIS_MANY_BALLS
    [NonSerialized] public const int MAX_BALLS = 32;
#endif
#if EIJIS_PYRAMID
    [NonSerialized] public const int PYRAMID_BALLS = 16;
#endif
    private const float k_RANDOMIZE_F = 0.0001f;
    private float k_SPOT_POSITION_X = 0.5334f; // First X position of the racked balls
    private const float k_SPOT_CAROM_X = 0.8001f; // Spot position for carom mode
#if EIJIS_ROTATION
    private float k_SPOT_RORATION_FREEBALL_X = 0.5334f / 2; // k_SPOT_POSITION_X; // Rotationの完全フリーボールの初期位置はセンターからずらす
#endif
#if EIJIS_SNOOKER15REDS
    private readonly int[] sixredsnooker_ballpoints =
    {
        0, 7, 2, 5, 1, 6, 1, 3,
        4, 1, 1, 1, 1, 1, 1, 1,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 1, 1, 1, 1, 1, 1, 0
    };
    private readonly uint SNOOKER_BALLS_MASK = 0x7E00FFFEu;
    private readonly uint SNOOKER_REDS_MASK = 0x7E00FE50u;
    private const int SNOOKER_REDS_COUNT = 15;
    [NonSerialized]
    public readonly int[] break_order_sixredsnooker =
    {
        4, 6, 9, 10, 11,
        12, 13, 14, 15, 25,
        26, 27, 28, 29, 30,
        2, 7, 8, 3, 5,
        1
    };
#else
    private readonly int[] sixredsnooker_ballpoints = { 0, 7, 2, 5, 1, 6, 1, 3, 4, 1, 1, 1, 1 };
    private readonly int[] break_order_sixredsnooker = { 4, 6, 9, 10, 11, 12, 2, 7, 8, 3, 5, 1 };
#endif
    private readonly int[] break_order_8ball = { 9, 2, 10, 11, 1, 3, 4, 12, 5, 13, 14, 6, 15, 7, 8 };
    private readonly int[] break_order_9ball = { 2, 3, 4, 5, 9, 6, 7, 8, 1 };
    private readonly int[] break_rows_9ball = { 0, 1, 2, 1, 0 };
#if EIJIS_10BALL
    private readonly int[] break_order_10ball = { 2, 1, 9, 8, 10, 5, 3, 6, 7, 4 };
#endif
#if EIJIS_ROTATION
    private readonly int[] break_order_rotation_15ball = { 2, 8, 1, 11, 12, 13, 9, 14, 15, 10, 3, 5, 6, 7, 4 };
    private readonly int[] break_order_rotation_10ball = { 2, 7, 8, 1, 10, 9, 3, 5, 6, 4 };
    private readonly int[] break_order_rotation_9ball = { 2, 6, 7, 8, 9, 1, 3, 4, 5 };
    private readonly int[] break_order_rotation_6ball = { 2, 5, 6, 3, 7, 4 };
    private readonly int[] pocketed_ball_upon_pool_order = { 2, 3, 4, 5, 6, 7, 8, 1, 9, 10, 11, 12, 13, 14, 15 };
#endif
#if EIJIS_CALLSHOT
    private readonly uint pocket_mask_8ball = 0xFFFEu;
    private readonly uint pocket_mask_9ball = 0x03FEu;
#if EIJIS_10BALL
    private readonly uint pocket_mask_10ball = 0x07FEu;
#endif
#if EIJIS_ROTATION
    private readonly uint[] rotation_pocket_masks = { 0xFFFEu, 0x07FEu, 0x3FEu, 0xFCu };
    private readonly uint[] rotation_initial_pocketed = { 0x0000u, 0xF800u, 0xFC00u, 0xFF02u };
#endif
    private uint pocketMask = 0x0;
    [NonSerialized] public int ballsLengthByPocketGame = 15;
#endif

    #region InspectorValues
#if EIJIS_TABLE_LABEL
    [Header("Table Label")]
    [SerializeField] public string logLabel;

#endif
    [Header("Managers")]
    [SerializeField] public NetworkingManager networkingManager;
    [SerializeField] public PracticeManager practiceManager;
    [SerializeField] public RepositionManager repositionManager;
    [SerializeField] public DesktopManager desktopManager;
    [SerializeField] public CameraManager cameraManager;
    [SerializeField] public GraphicsManager graphicsManager;
    [SerializeField] public MenuManager menuManager;
    [SerializeField] public UdonSharpBehaviour[] PhysicsManagers;

    [Header("Camera Module")]
    [SerializeField] public UdonSharpBehaviour cameraModule;

    [Space(10)]
    [Header("Sound Effects")]
    [SerializeField] AudioClip snd_Intro;
    [SerializeField] AudioClip snd_Sink;
    [SerializeField] AudioClip snd_OutOfBounds;
    [SerializeField] AudioClip snd_NewTurn;
    [SerializeField] AudioClip snd_PointMade;
    [SerializeField] public AudioClip snd_btn;
    [SerializeField] public AudioClip snd_spin;
    [SerializeField] public AudioClip snd_spinstop;
    [SerializeField] AudioClip snd_hitball;

    [Space(10)]
    [Header("Other")]
    public float LoDDistance = 10;
    [Tooltip("Shuffle positions of ball spawn points in 8ball and 9ball?")]
    public bool RandomizeBallPositions = true;

    [Space(10)]
    [Header("Table Light Colors")]
    // table colors
    [SerializeField] public Color k_colour_foul;        // v1.6: ( 1.2, 0.0, 0.0, 1.0 )
    [SerializeField] public Color k_colour_default;     // v1.6: ( 1.0, 1.0, 1.0, 1.0 )
    [SerializeField] public Color k_colour_off = new Color(0.01f, 0.01f, 0.01f, 1.0f);

    // 8/9 ball
    [SerializeField] public Color k_teamColour_spots;   // v1.6: ( 0.00, 0.75, 1.75, 1.0 )
    [SerializeField] public Color k_teamColour_stripes; // v1.6: ( 1.75, 0.25, 0.00, 1.0 )

    // Snooker
    [SerializeField] public Color k_snookerTeamColour_0;   // v1.6: ( 0.00, 0.75, 1.75, 1.0 )
    [SerializeField] public Color k_snookerTeamColour_1; // v1.6: ( 1.75, 0.25, 0.00, 1.0 )

    // 4 ball
    [SerializeField] public Color k_colour4Ball_team_0; // v1.6: ( )
    [SerializeField] public Color k_colour4Ball_team_1; // v1.6: ( 2.0, 1.0, 0.0, 1.0 )

    // fabrics
    [SerializeField][HideInInspector] public Color k_fabricColour_8ball; // v1.6: ( 0.3, 0.3, 0.3, 1.0 )
    [SerializeField][HideInInspector] public Color k_fabricColour_9ball; // v1.6: ( 0.1, 0.6, 1.0, 1.0 )
    [SerializeField][HideInInspector] public Color k_fabricColour_4ball; // v1.6: ( 0.15, 0.75, 0.3, 1.0 )

    [Space(10)]
    [Header("Internal (no touching!)")]
    // Other scripts
    [SerializeField] public CueController[] cueControllers;

    // GameObjects
    [SerializeField] public GameObject[] balls;
    [SerializeField] public GameObject guideline;
    [SerializeField] public GameObject guideline2;
    [SerializeField] public GameObject devhit;
    [SerializeField] public GameObject markerObj;
    [SerializeField] public GameObject marker9ball;
    [NonSerialized] public Transform tableSurface;

    // Texts
    [SerializeField] Text ltext;
    [SerializeField] TextMeshProUGUI infReset;

    public ReflectionProbe reflection_main;
#if EIJIS_CALLSHOT
    
    [Header("Pocket Billiard Call-shot")]
    [SerializeField] public GameObject markerCalledBall;
    [SerializeField] Material calledBallMarkerBlue;
    [SerializeField] Material calledBallMarkerOrange;
    [SerializeField] Material calledBallMarkerWhite;
    [SerializeField] Material calledBallMarkerGray;
#endif
#if EIJIS_EXTRA_GAMES
#if EIJIS_EXTERNAL_SCORE_SCREEN
    
    [Header("Ext Score Screen")]
    [SerializeField] public BilliardsScoreScreen scoreScreenRotation;
    [SerializeField] public BilliardsScoreScreen scoreScreenBowlards;
    private BilliardsScoreScreen[] scoreScreens = new BilliardsScoreScreen[2];
    private BilliardsScoreScreen scoreScreen;
#endif 

    [Header("Games Using Ext Score Screen")]
    [SerializeField] public GameObject markerHeadSpot;
    [SerializeField] public GameObject markerCenterSpot;
    [SerializeField] public GameObject markerFootSpot;
    [SerializeField] public GameObject requestBreakOrange;
    [SerializeField] public GameObject requestBreakBlue;
#endif
    #endregion

    #endregion

    #region DebugSetting

    // debugger
    [NonSerialized] public int PERF_MAIN = 0;
    [NonSerialized] public int PERF_PHYSICS_MAIN = 1;
    [NonSerialized] public int PERF_PHYSICS_VEL = 2;
    [NonSerialized] public int PERF_PHYSICS_BALL = 3;
    [NonSerialized] public int PERF_PHYSICS_CUSHION = 4;
    [NonSerialized] public int PERF_PHYSICS_POCKET = 5;

    [NonSerialized] public const int PERF_MAX = 6;
    private string[] perfNames = new string[] {
      "main",
      "physics",
      "physicsVel",
      "physicsBall",
      "physicsCushion",
      "physicsPocket"
   };
    private float[] perfCounters = new float[PERF_MAX];
    private float[] perfTimings = new float[PERF_MAX];
    private float[] perfStart = new float[PERF_MAX];
    private const int LOG_MAX = 32;
    private int LOG_LEN = 0;
    private int LOG_PTR = 0;
    private string[] LOG_LINES = new string[32];

    #endregion

    // cached copies of networked data, may be different from local game state
    [NonSerialized] public int[] playerIDsCached = { -1, -1, -1, -1 };//the 4 is MAX_PLAYERS from NetworkingManager

    #region LocalState

    // local game state
    [NonSerialized] public bool lobbyOpen;
    [NonSerialized] public bool gameLive;
    [NonSerialized] public uint gameModeLocal;
    [NonSerialized] public uint timerLocal;
    [NonSerialized] public bool teamsLocal;
    [NonSerialized] public bool noGuidelineLocal;
#if EIJIS_GUIDELINE2TOGGLE
    [NonSerialized] public bool noGuideline2Local;
#endif
    [NonSerialized] public bool noLockingLocal;
#if EIJIS_10BALL
    [NonSerialized] public bool wpa10BallRuleLocal;
#endif
#if EIJIS_CALLSHOT
    [NonSerialized] public bool requireCallShotLocal;
    [NonSerialized] public bool callPassOptionLocal;
#if EIJIS_SEMIAUTOCALL
    [NonSerialized] public bool semiAutoCallLocal;
#endif
    private bool cueBallFixed;
    private int cueBallRepositionCount = 0;
    private int semiAutoCallDelayBase = 0;
#endif
    [NonSerialized] public uint ballsPocketedLocal;
#if EIJIS_CALLSHOT
    [NonSerialized] public uint targetPocketed;
    [NonSerialized] public uint otherPocketed;
    [NonSerialized] public uint pointPocketsLocal;
    [NonSerialized] public bool safetyCalledLocal;
    [NonSerialized] public GameObject callSafetyOrb;
#if EIJIS_CALL_DEFAULT_LOCKING
    [NonSerialized] public GameObject callClearOrb;
#else
    [NonSerialized] public GameObject callShotLockOrb;
#endif
    [NonSerialized] public GameObject skipTurnOrb;
#endif
#if EIJIS_PUSHOUT
    [NonSerialized] public GameObject pushOutOrb;
#endif
#if EIJIS_CUEBALLSWAP || EIJIS_CALLSHOT
    [NonSerialized] public uint calledBallsLocal;
#endif
    [NonSerialized] public uint teamIdLocal;
    [NonSerialized] public uint fourBallCueBallLocal;
    [NonSerialized] public bool isTableOpenLocal;
    [NonSerialized] public uint teamColorLocal;
    [NonSerialized] public int numPlayersCurrent = 0;
    [NonSerialized] public int numPlayersCurrentOrange = 0;
    [NonSerialized] public int numPlayersCurrentBlue = 0;
    [NonSerialized] public int[] playerIDsLocal = { -1, -1, -1, -1 };
    [NonSerialized] public byte[] fbScoresLocal = new byte[2];
#if EIJIS_ROTATION
    [NonSerialized] public ushort[] goalPointsLocal = new ushort[2];
    [NonSerialized] public ushort[] totalPointsLocal = new ushort[2];
    [NonSerialized] public ushort[] highRunsLocal = new ushort[2];
    [NonSerialized] public ushort[] chainedPointsLocal = new ushort[2];
    [NonSerialized] public byte[] chainedFoulsLocal = new byte[2];
#endif
#if EIJIS_BOWLARDS
    [NonSerialized] public ushort[] framePointsLocal = new ushort[2];
    [NonSerialized] public ushort[] framePointsLocal2 = new ushort[2];
    [NonSerialized] public bool practiceModeMenuToggleLocal;
    [NonSerialized] public uint denyBalls;
    [NonSerialized] public bool loadUsedLocal;
    [NonSerialized] public byte undoCountLocal;
    [NonSerialized] public bool playerChangedLocal;
#endif
    [NonSerialized] public uint winningTeamLocal;
    [NonSerialized] public int activeCueSkin;
    [NonSerialized] public int tableSkinLocal;
    [NonSerialized] public byte gameStateLocal;
    private byte turnStateLocal;
    private int timerStartLocal;
    [NonSerialized] public uint foulStateLocal;
    [NonSerialized] public int tableModelLocal;
    [NonSerialized] public bool colorTurnLocal;
#if EIJIS_ROTATION
    private uint nextBallRepositionStateLocal; // 0x01:的玉の移動選択可能, 0x02:手玉の移動選択可能, 0x04:再ブレイク要求選択可能, 0x08:的玉をフットに移動選択済み, 0x10:的玉をセンターに移動選択済み
    private int inningCountLocal;
    // private int[] winRackCountLocal = new int[2];
    public readonly ushort[] ROTATION_GOAL_POINTS =
    {
        40, 50, 60, 70, 80, 90, 100, 
        120, 140, 160, 180, 200, 
        240, 300
    };
#endif

    //Cheese Addition
    [NonSerialized] public bool BreakFinish;
    [NonSerialized] public int ShotCounts;
    [NonSerialized] public int HeightBreak;
#if EIJIS_PYRAMID
    [NonSerialized] public const uint GAMEMODE_PYRAMID = 5u;
#endif
#if EIJIS_CAROM
    [NonSerialized] public const uint GAMEMODE_3CUSHION = 6u;
    [NonSerialized] public const uint GAMEMODE_2CUSHION = 7u;
    [NonSerialized] public const uint GAMEMODE_1CUSHION = 8u;
    [NonSerialized] public const uint GAMEMODE_0CUSHION = 9u;
#endif
#if EIJIS_10BALL
    [NonSerialized] public const uint GAMEMODE_10BALL = 10u;
#endif
#if EIJIS_BANKING
    [NonSerialized] public const uint GAMEMODE_BANKING = 11u;
#endif
#if EIJIS_ROTATION
    [NonSerialized] public const uint GAME_MODE_ROTATION_MASK = 0x3u;// 0011
    [NonSerialized] public const uint GAMEMODE_ROTATION_15 = 12u; // 1100b 0xC
    [NonSerialized] public const uint GAMEMODE_ROTATION_10 = 13u; // 1101b 0xD
    [NonSerialized] public const uint GAMEMODE_ROTATION_9 = 14u; // 1110b 0xE
    [NonSerialized] public const uint GAMEMODE_ROTATION_6 = 15u; // 1111b 0xF
#endif
#if EIJIS_BOWLARDS
    [NonSerialized] public const uint GAMEMODE_BOWLARDS_10 = 16u;
    [NonSerialized] public const uint GAMEMODE_BOWLARDS_5 = 17u;
    [NonSerialized] public const uint GAMEMODE_BOWLARDS_1 = 18u;
#endif
#if EIJIS_CUEBALLSWAP || EIJIS_PUSHOUT || EIJIS_CALLSHOT
    [NonSerialized] public int stateIdLocal;
#endif
#if EIJIS_CUEBALLSWAP || EIJIS_CALLSHOT
    private bool calledBallOff = false;
    private int calledBallId = -2;
    private float calledBallIdDelayTimestamp = 0;
    private float callDelay = 0.4f;
#endif
#if EIJIS_CALLSHOT
    private bool calledPocketOff = false;
    private int calledPocketId = -2;
    private float calledPocketIdDelayTimestamp = 0;
    private bool callShotLockLocal;
#if EIJIS_SEMIAUTOCALL
    private bool semiAutoCalledPocket;
    private float semiAutoCalledTimeBall;
    private bool semiAutoCallTick;
#endif
#endif
#if EIJIS_CALLSHOT && EIJIS_PUSHOUT
    private bool cantSkipNextTurnOnCallShotOption;
#endif
#if EIJIS_PUSHOUT
    [NonSerialized] public byte pushOutStateLocal;
    [NonSerialized] public readonly byte PUSHOUT_BEFORE_BREAK = 0;
    // [NonSerialized] public readonly byte PUSHOUT_ILLEGAL_REACTIONING = 1;
    [NonSerialized] public readonly byte PUSHOUT_DONT = 2;
    [NonSerialized] public readonly byte PUSHOUT_DOING = 3;
    [NonSerialized] public readonly byte PUSHOUT_REACTIONING = 4;
    [NonSerialized] public readonly byte PUSHOUT_ENDED = 5;
#if EIJIS_DEBUG_PUSHOUT
    private string[] PushOutState = new string[] {
        "BEFORE_BREAK",
        "ILLEGAL_REACTIONING",
        "DONT",
        "DOING",
        "REACTIONING",
        "ENDED"
    }; 
#endif
#endif

    #endregion

    // physics simulation data, must be reset before every simulation
    [NonSerialized] public bool isLocalSimulationRunning;
    [NonSerialized] public bool waitingForUpdate;
    [NonSerialized] public bool isLocalSimulationOurs = false;
    [NonSerialized] public int simulationOwnerID;
    private uint numBallsHitCushion = 0; // used to check if 9ball break was legal (4 balls must hit cushion)
    private bool[] ballhasHitCushion;
    private bool ballBounced;//tracks if any ball has touched the cushion after initial ball collision
    private uint ballsPocketedOrig;
    private int firstHit = 0;
    private int secondHit = 0;
    private int thirdHit = 0;
    private bool jumpShotFoul;
    private bool fallOffFoul;
#if EIJIS_CAROM
    private int cushionBeforeSecondBall = 0;
    private int cushionHitGoal = 3;
#endif

    private bool fbMadePoint = false;
    private bool fbMadeFoul = false;

    // game state data
#if EIJIS_MANY_BALLS
    [NonSerialized] public Vector3[] ballsP = new Vector3[MAX_BALLS];
    [NonSerialized] public Vector3[] ballsV = new Vector3[MAX_BALLS];
    [NonSerialized] public Vector3[] ballsW = new Vector3[MAX_BALLS];
#else
    [NonSerialized] public Vector3[] ballsP = new Vector3[16];
    [NonSerialized] public Vector3[] ballsV = new Vector3[16];
    [NonSerialized] public Vector3[] ballsW = new Vector3[16];
#endif

    [NonSerialized] public bool canPlayLocal;
    [NonSerialized] public bool isGuidelineValid;
    [NonSerialized] public bool canHitCueBall = false;
    [NonSerialized] public bool isReposition = false;
    [NonSerialized] public float repoMaxX;
    [NonSerialized] public bool timerRunning = false;

    [NonSerialized] public int localPlayerId = -1;
    [NonSerialized] public uint localTeamId = uint.MaxValue;

    [NonSerialized] public UdonSharpBehaviour currentPhysicsManager;
    [NonSerialized] public CueController activeCue;

    // some udon optimizations
    [NonSerialized] public bool is8Ball = false;
    [NonSerialized] public bool is9Ball = false;
#if EIJIS_10BALL
    [NonSerialized] public bool is10Ball = false;
#endif
    [NonSerialized] public bool isCarom = false;
    [NonSerialized] public bool isJp4Ball = false;
    [NonSerialized] public bool isKr4Ball = false;
#if EIJIS_SNOOKER15REDS
    [NonSerialized] public bool isSnooker = false;
    [NonSerialized] public bool isSnooker15Red = false;
#endif
    [NonSerialized] public bool isSnooker6Red = false;
#if EIJIS_PYRAMID
    [NonSerialized] public bool isPyramid = false;
    [NonSerialized] public bool isChinese8Ball = false;
#endif
#if EIJIS_CAROM
    [NonSerialized] public bool is3Cusion = false;
    [NonSerialized] public bool is2Cusion = false;
    [NonSerialized] public bool is1Cusion = false;
    [NonSerialized] public bool is0Cusion = false;
#endif
#if EIJIS_BANKING
    [NonSerialized] public bool isBanking = false;
#endif
#if EIJIS_ROTATION
    [NonSerialized] public bool isRotation = false;
    [NonSerialized] public bool isRotation15Balls = false;
    [NonSerialized] public bool isRotation10Balls = false;
    [NonSerialized] public bool isRotation9Balls = false;
    [NonSerialized] public bool isRotation6Balls = false;
#endif
#if EIJIS_BOWLARDS
    [NonSerialized] public bool isBowlards = false;
    [NonSerialized] public bool isBowlards10Frame = false;
    [NonSerialized] public bool isBowlards5Frame = false;
    [NonSerialized] public bool isBowlards1Frame = false;
#endif
    [NonSerialized] public bool isPracticeMode = false;
    [NonSerialized] public bool isPlayer = false;
    [NonSerialized] public bool isOrangeTeamFull = false;
    [NonSerialized] public bool isBlueTeamFull = false;
    [NonSerialized] public bool localPlayerDistant = false;
#if EIJIS_CALLSHOT
    [NonSerialized] public Vector3[] pocketLocations = new Vector3[6];
#if EIJIS_SEMIAUTOCALL
    private Vector3[] findEasiestBallAndPocketConditions = new Vector3[]
    {
        // x:deg, y:t2p, z:c2t
        new Vector3(60.0f, 0.09f, 0.09f), // 0.3f, 0.3f // 0.06 * 5
        new Vector3(60.0f, 0.09f, 0.36f), // 0.3f, 0.6f // 0.06 * 5
        new Vector3(45.0f, 0.36f, 0.36f), // 0.6f, 0.6f // 0.06 * 10
        new Vector3(30.0f, 0.81f, 1.42f), // 0.9f, 1.2f // 0.06 * 12
        new Vector3(60.0f, 0.09f, float.MaxValue), // 0.3f, - // 0.06 * 5
        new Vector3(30.0f, 0.81f, 0.81f), // 0.9f, 0.9f // 0.06 * 12
        new Vector3(45.0f, 0.36f, float.MaxValue), // 0.6f, - // 0.06 * 10
        new Vector3(30.0f, 0.81f, float.MaxValue), // 0.9f, - // 0.06 * 12
        new Vector3(15.0f, float.MaxValue, float.MaxValue),
        new Vector3(30.0f, float.MaxValue, float.MaxValue),
        new Vector3(45.0f, float.MaxValue, float.MaxValue),
        new Vector3(60.0f, float.MaxValue, float.MaxValue),
        new Vector3(float.MaxValue, float.MaxValue, float.MaxValue)
    };
#if EIJIS_DEBUG_SEMIAUTO_CALL_FINDLOGIC || EIJIS_DEBUG_SEMIAUTO_CALL_AFTER_REPOSITION
    private bool debugLogFlg = false;
    private int debugLogStart = 0;
#endif
#endif
#endif

    // use this to make sure max simulation is always visible
    [System.NonSerializedAttribute] public bool noLOD;
    // Add 1 to noLOD_ using SetProgramVariable() to prevent LoD check, subtract to undo
    // this allows more than one other script to disable LoD simultaniously
    [System.NonSerializedAttribute, FieldChangeCallback(nameof(noLOD__))] public int noLOD_ = 0;
    public int noLOD__
    {
        set
        {
            noLOD = value > 0;
            noLOD_ = value;
        }
        get => noLOD_;
    }
    bool checkingDistant;
    GameObject debugger;
    [NonSerialized] public CameraOverrideModule cameraOverrideModule;
    public string[] moderators = new string[0];
    [NonSerialized] public const float ballMeshDiameter = 0.06f;//the ball's size as modeled in the mesh file
    private void OnEnable()
    {
#if EIJIS_EXTERNAL_SCORE_SCREEN
        if (ReferenceEquals(null, scoreScreenRotation))
        {
            _LogInfo("  scoreScreenRotation object not set.");
        }
        else
        {
            scoreScreens[0] = scoreScreenRotation;
        }
        if (ReferenceEquals(null, scoreScreenBowlards))
        {
            _LogInfo("  scoreScreenBowlards object not set.");
        }
        else
        {
            scoreScreens[1] = scoreScreenBowlards;
        }

        for (int i = 0; i < scoreScreens.Length; i++)
        {
            if (ReferenceEquals(null, scoreScreens[i])) continue;
            scoreScreens[i].Init(this);
        }
#endif
#if EIJIS_TABLE_LABEL
        logLabel = string.IsNullOrEmpty(logLabel) ? string.Empty : " " + logLabel;
#endif
        _LogInfo("initializing billiards module");

        cameraOverrideModule = (CameraOverrideModule)_GetModule(nameof(CameraOverrideModule));

        resetCachedData();

        currentPhysicsManager = PhysicsManagers[0];

        for (int i = 0; i < balls.Length; i++)
        {
            ballsP[i] = balls[i].transform.localPosition;
            balls[i].GetComponentInChildren<Repositioner>(true)._Init(this, i);

            Rigidbody ballRB = balls[i].GetComponent<Rigidbody>();
            ballRB.maxAngularVelocity = 999;
        }

#if EIJIS_GUIDELINE2TOGGLE
        noGuideline2Local = true;
#endif
#if EIJIS_CALLSHOT
#if EIJIS_10BALL && EIJIS_ROTATION
        pocketMask = is10Ball || isRotation10Balls ? pocket_mask_10ball
            : (is9Ball || isRotation9Balls ? pocket_mask_9ball
                : (isRotation6Balls ? rotation_pocket_masks[GAMEMODE_ROTATION_6 & GAME_MODE_ROTATION_MASK]
                    : pocket_mask_8ball));
        ballsLengthByPocketGame = (is9Ball || isRotation9Balls ? break_order_9ball.Length
            : (is10Ball || isRotation10Balls ? break_order_10ball.Length
                : (isRotation6Balls ? break_order_rotation_6ball.Length : break_order_8ball.Length)));
#else
        pocketMask = is9Ball ? pocket_mask_9ball : pocket_mask_8ball;
        ballsLengthByPocketGame = (is9Ball
             ? break_order_9ball.Length
             : break_order_8ball.Length);
#endif
        requireCallShotLocal = false;
        callPassOptionLocal = false;
#if EIJIS_SEMIAUTOCALL
        // semiAutoCallLocal = true;
#endif
        pointPocketsLocal = 0;
        calledBallsLocal = 0;

#endif
        aud_main = this.GetComponent<AudioSource>();
        cueControllers[1].TeamBlue = true;
        for (int i = 0; i < cueControllers.Length; i++)
        { cueControllers[i]._Init(); }
        networkingManager._Init(this);
#if EIJIS_DEFAULT_MODE_CHANGE
        tableModelLocal = 1; // 9ft
        networkingManager.tableModelSynced = (byte)tableModelLocal;
        // networkingManager.gameModeSynced = (byte)GAMEMODE_ROTATION_15;
        networkingManager.gameModeSynced = (byte)GAMEMODE_BOWLARDS_10;
        // timerLocal = 60u; // 60sec
        timerLocal = 180u; // 3min
        networkingManager.timerSynced = (byte)timerLocal;
        requireCallShotLocal = true;
#endif
#if EIJIS_GUIDELINE2TOGGLE
        networkingManager.noGuideline2Synced = noGuideline2Local;
#endif
#if EIJIS_CALLSHOT
        networkingManager.requireCallShotSynced = requireCallShotLocal;
        networkingManager.callPassOptionSynced = callPassOptionLocal;
#if EIJIS_SEMIAUTOCALL
        networkingManager.semiAutoCallSynced = semiAutoCallLocal;
#endif
        networkingManager.pointPocketsSynced = pointPocketsLocal;
        networkingManager.calledBallsSynced = calledBallsLocal;
#endif
        practiceManager._Init(this);
        repositionManager._Init(this);
        desktopManager._Init(this);
        cameraManager._Init(this);
        graphicsManager._Init(this);
        cameraOverrideModule._Init();
        menuManager._Init(this);
        for (int i = 0; i < tableModels.Length; i++)
        {
            tableModels[i].gameObject.SetActive(false);
            tableModels[i]._Init();
        }

        tableSurface = transform.Find("intl.balls");
#if EIJIS_CALLSHOT
        pointPocketMarkers = new GameObject[6];
        pointPocketMarkerCalled = new GameObject[pointPocketMarkers.Length];
        pointPocketMarkerNoCall = new GameObject[pointPocketMarkers.Length];
        for (int i = 0; i < pointPocketMarkers.Length; i++)
        {
            Transform pointPocketMarker = tableSurface.Find($"PointPocketMarker_{i}");
            pointPocketMarkers[i] = pointPocketMarker.gameObject;
            pointPocketMarkerCalled[i] = pointPocketMarker.Find("Sphere").gameObject;
            pointPocketMarkerNoCall[i] = pointPocketMarker.Find("Plane").gameObject;
        }
#endif
        for (int i = 0; i < PhysicsManagers.Length; i++)
        {
            PhysicsManagers[i].SetProgramVariable("table_", this);
            PhysicsManagers[i].SendCustomEvent("_Init");
        }

        currentPhysicsManager.SendCustomEvent("_InitConstants");


#if EIJIS_ISSUE_FIX
        setTableModel(tableModelLocal);
#else
        setTableModel(0);
#endif

        infReset.text = string.Empty;

        debugger = this.transform.Find("debugger").gameObject;
        debugger.SetActive(true);

        Transform gdisplay = guideline.transform.GetChild(0);
        if (gdisplay)
            gdisplay.GetComponent<MeshRenderer>().material.SetMatrix("_BaseTransform", this.transform.worldToLocalMatrix);
        Transform gdisplay2 = guideline2.transform.GetChild(0);
        if (gdisplay2)
            gdisplay2.GetComponent<MeshRenderer>().material.SetMatrix("_BaseTransform", this.transform.worldToLocalMatrix);

        if (LoDDistance > 0 && !checkingDistant)
        {
            checkingDistant = true;
            SendCustomEventDelayedSeconds(nameof(checkDistanceLoop), UnityEngine.Random.Range(0, 1f));
        }

        //init table hook
        if(tableHook == null)
        {
            tableHook = GameObject.Find("TableHook (replica) 2").GetComponent<TableHook>();
            if (tableHook == null)
                Debug.Log("Please put table hook in scene! 请把Tablehook放入场景");
        }
        if (tableHook != null)
        {
            tableHook.AddTranslation(_translations);
            tableHook.AddBilliardsModule(this);
        }
    }

    private void OnDisable()
    {
        checkingDistant = false;
    }

    private void FixedUpdate()
    {
        currentPhysicsManager.SendCustomEvent("_FixedTick");
    }

    private void Update()
    {
        if (localPlayerDistant) { return; }
        desktopManager._Tick(gameModeLocal);
        // menuManager._Tick();

        _BeginPerf(PERF_MAIN);
        practiceManager._Tick();
        repositionManager._Tick();
        cameraManager._Tick();
        graphicsManager._Tick();
        tickTimer();
#if EIJIS_CALLSHOT
        _UpdateCalledBallMarker();
#if EIJIS_SEMIAUTOCALL
        if (semiAutoCallTick) tickSemiAutoCall();
#endif
#endif

        networkingManager._FlushBuffer();
        _EndPerf(PERF_MAIN);

        if (perfCounters[PERF_MAIN] % 500 == 0) _RedrawDebugger();
    }

    public UdonSharpBehaviour _GetModule(string type)
    {
        string[] parts = cameraModule.GetUdonTypeName().Split('.');
        if (parts[parts.Length - 1] == type)
        {
            return cameraModule;
        }
        return null;
    }

    #region Triggers
    public void _TriggerLobbyOpen()
    {
#if UDON_CHIPS
        udonChips = GameObject.Find("UdonChips").GetComponent<UdonChips>();
        if (udonChips.money >= Enter_cost)
        {
            udonChips.money -= Enter_cost;
        }
        else
        {
            _LogWarn("u need money to play");
            return;
        }
 
#endif
        if (lobbyOpen) return;
#if EIJIS_EXTERNAL_SCORE_SCREEN
        if (!ReferenceEquals(null, scoreScreen))
        {
            scoreScreen.Clear();
        }
#endif
        menuManager._EnableLobbyMenu();
        networkingManager._OnLobbyOpened();

        Debug.Log("_TriggerLobbyOpen");

    }

    public void _TriggerTeamsChanged(bool teamsEnabled)
    {
        networkingManager._OnTeamsChanged(teamsEnabled);
    }

    public void _TriggerNoGuidelineChanged(bool noGuidelineEnabled)
    {
        networkingManager._OnNoGuidelineChanged(noGuidelineEnabled);
    }

#if EIJIS_GUIDELINE2TOGGLE
    public void _TriggerNoGuideline2Changed(bool noGuideline2Enabled)
    {
        networkingManager._OnNoGuideline2Changed(noGuideline2Enabled);
    }
    
#endif
    public void _TriggerNoLockingChanged(bool noLockingEnabled)
    {
        networkingManager._OnNoLockingChanged(noLockingEnabled);
    }

#if EIJIS_10BALL
    public void _TriggerWpa10BallRuleChanged(bool wpa10BallRuleEnabled)
    {
        networkingManager._OnWpa10BallRuleChanged(wpa10BallRuleEnabled);
    }
    
#endif
#if EIJIS_CALLSHOT
    public void _TriggerRequireCallShotChanged(bool callShotEnabled)
    {
        networkingManager._OnRequireCallShotChanged(callShotEnabled);
    }

    public void _TriggerCallPassOptionChanged(bool callPassOptionEnabled)
    {
        networkingManager._OnCallPassOptionChanged(callPassOptionEnabled);
    }

#if EIJIS_SEMIAUTOCALL
    public void _TriggerSemiAutoCallChanged(bool semiAutoCallEnabled)
    {
        networkingManager._OnSemiAutoCallChanged(semiAutoCallEnabled);
    }

#endif
#if EIJIS_BOWLARDS
    public void _TriggerPracticeModeChanged(bool practiceModeEnabled)
    {
        networkingManager._OnPracticeModeChanged(practiceModeEnabled);
    }

#endif
#endif
    public void _TriggerTimerChanged(byte timerSelected)
    {
        networkingManager._OnTimerChanged(timerSelected);
    }

    public void _TriggerTableModelChanged(uint TableModelSelected)
    {
        networkingManager._OnTableModelChanged(TableModelSelected);
    }

    public void _TriggerPhysicsChanged(uint PhysicsSelected)
    {
        networkingManager._OnPhysicsChanged(PhysicsSelected);
    }

    public void _TriggerGameModeChanged(uint newGameMode)
    {
        networkingManager._OnGameModeChanged(newGameMode);
    }

#if EIJIS_ROTATION
    public void _TriggerGoalPointChanged(uint teamId, uint goalPointIndex)
    {
        networkingManager._OnGoalPointChanged(teamId, goalPointIndex);
    }
    
#endif
    public void _TriggerGlobalSettingsUpdated(int newPhysicsMode, int newTableModel)
    {
        networkingManager._OnGlobalSettingsChanged((byte)newPhysicsMode, (byte)newTableModel);
    }

    public void _TriggerCueBallHit()
    {
        if (!isMyTurn()) return;

        _LogWarn("trying to propagate cue ball hit, linear velocity is " + ballsV[0].ToString("F4") + " and angular velocity is " + ballsW[0].ToString("F4"));

        if (float.IsNaN(ballsV[0].x) || float.IsNaN(ballsV[0].y) || float.IsNaN(ballsV[0].z) || float.IsNaN(ballsW[0].x) || float.IsNaN(ballsW[0].y) || float.IsNaN(ballsW[0].z))
        {
            ballsV[0] = Vector3.zero;
            ballsW[0] = Vector3.zero;
            return;
        }

        _TriggerCueDeactivate();

        if (foulStateLocal == 5)//free ball
        {
            if (SixRedCheckObjBlocked(ballsPocketedLocal, colorTurnLocal, true) > 0)
            {
                _LogInfo("6RED: Free ball turn. First hit ball is counted as current objective ball.");
            }
        }

        networkingManager._OnHitBall(ballsV[0], ballsW[0]);
    }
#if EIJIS_CUEBALLSWAP || EIJIS_CALLSHOT
    public void _TriggerOtherBallHit(int ballId, bool desktop)
    {
        if (localTeamId != teamIdLocal && !isPracticeMode) return; // is there a better way to do this?

#if EIJIS_CALLSHOT
        if (!desktop && callShotLockLocal)
        {
            return;
        }

#endif
        if (!desktop && calledBallId == ballId)
        {
            return;
        }

        if (!desktop && Time.time < calledBallIdDelayTimestamp + callDelay)
        {
            return;
        }
        else
        {
#if EIJIS_DEBUG_BALLCHOICE
            _LogInfo($"  calledBallId = {calledBallId}, calledBallIdDelayTimestamp = {calledBallIdDelayTimestamp}, callDelay = {callDelay}");
#endif
            calledBallIdDelayTimestamp = Time.time;
            calledBallId = ballId;
        }

        int id = calledBallId;

#if EIJIS_DEBUG_BALLCHOICE
        _LogInfo($"  id = {id}, ballsPocketedLocal = {ballsPocketedLocal:X4}, (0x1 << id) = {(0x1 << id):X4}");
#endif
        if (0 < id && 0 != (ballsPocketedLocal & (0x1 << id)))
        {
#if EIJIS_DEBUG_BALLCHOICE
            _LogInfo("  return");
#endif
            return;
        }

        if (id < 0)
        {
            calledBallOff = true;
            return;
        }

        if (!calledBallOff && !desktop)
        {
            return;
        }

        uint calledBalls = calledBallsLocal;
        calledBalls |= 0x1u << id;
        if (calledBalls == calledBallsLocal)
        {
#if EIJIS_CALLSHOT_ALLOW_UNSELECT
            calledBalls ^= 0x1u << id;
#else
            return;
#endif
        }
        
#if EIJIS_CALLSHOT
#if false // callShotOprationOverwriteMode
        if (!callShotOprationOverwriteModeLocal){
            if (calledBallsLocal != 0 && calledBalls != 0 && !desktop)
            {
#if EIJIS_DEBUG_SEMIAUTO_CALL
                // _LogInfo("TKCH   other chiced exists (return)");
#endif
                return;
            }
        }
#endif
#else 
        if (calledBallsLocal != 0 && calledBalls != 0 && !desktop)
        {
            return;
        }
#endif

        if (Networking.LocalPlayer == null || Networking.GetOwner(activeCue.gameObject) != Networking.LocalPlayer)
        {
#if EIJIS_DEBUG_SEMIAUTO_CALL
            _LogInfo($"TKCH   not cue owner {Networking.GetOwner(activeCue.gameObject)}");
#endif
            if (localPlayerId != teamIdLocal)
            {
#if EIJIS_DEBUG_SEMIAUTO_CALL
                _LogInfo($"TKCH   not team leader localPlayerId = {localPlayerId}, teamIdLocal = {teamIdLocal} (return)");
#endif
                return;
            }
        }

        bool enable = (calledBallsLocal < calledBalls);

        networkingManager._OnCalledBallChanged(enable, (uint)id);
        calledBallOff = false;

        //aud_main.PlayOneShot(snd_btn);
    }
#endif
#if EIJIS_CALLSHOT
    
    public void _TriggerPocketHit(int pocketId, bool desktop)
    {
        if (localTeamId != teamIdLocal && !isPracticeMode) return; // is there a better way to do this?

        if (!desktop && callShotLockLocal)
        {
            return;
        }
        
        if (!desktop && calledPocketId == pocketId)
        {
            return;
        }

        if (!desktop && Time.time < calledPocketIdDelayTimestamp + callDelay)
        {
            return;
        }
        else
        {
#if EIJIS_DEBUG_CALLSHOT_DELAY
            _LogInfo($"  calledPocketId = {calledPocketId}, calledPocketIdDelayTimestamp = {calledPocketIdDelayTimestamp}, callShotDelay = {callShotDelay}");
#endif
            calledPocketIdDelayTimestamp = Time.time;
            calledPocketId = pocketId;
        }

        int id = calledPocketId;

        if (id < 0)
        {
            calledPocketOff = true;
            return;
        }

        if (!calledPocketOff && !desktop)
        {
            return;
        }

        uint pointPockets = pointPocketsLocal;
        pointPockets |= 0x1u << id;
        if (pointPockets == pointPocketsLocal)
        {
#if EIJIS_CALLSHOT_ALLOW_UNSELECT
            pointPockets ^= 0x1u << id;
#else
            return;
#endif
        }

#if false // callShotOprationOverwriteMode
        if (!callShotOprationOverwriteModeLocal){
            if (pointPocketsLocal != 0 && pointPockets != 0 && !desktop)
            {
                return;
            }
        }
#endif

        if (Networking.LocalPlayer == null || Networking.GetOwner(activeCue.gameObject) != Networking.LocalPlayer)
        {
            if (localPlayerId != teamIdLocal)
            {
                return;
            }
        }

        bool enable = (pointPocketsLocal < pointPockets);
        
        networkingManager._OnPocketChanged(enable, (uint)id);
        calledPocketOff = false;
        
        //aud_main.PlayOneShot(snd_btn);
    }
#endif

    public void _TriggerCueActivate()
    {
        if (!isMyTurn() || !activeCue) return;

        if (Vector3.Distance(activeCue._GetCuetip().transform.position, ballsP[0]) < k_BALL_RADIUS)
        {
            _TriggerCueDeactivate();
            return;
        }

        canHitCueBall = true;
        this._TriggerOnPlayerPrepareShoot();

#if !HT_QUEST
        this.transform.Find("intl.balls/guide/guide_display").GetComponent<MeshRenderer>().material.SetColor("_Colour", k_aimColour_locked);
#endif
#if EIJIS_CALLSHOT && EIJIS_CALL_MARKER_TO_GRAY_ON_CUE_TRIGGER_ACTIVATE
        if (!isLocalSimulationRunning && !callShotLockLocal)
        {
            graphicsManager._ChangePointPocketMarkerMaterial(true);
        }
#endif
    }

    public void _TriggerCueDeactivate()
    {
        canHitCueBall = false;

#if !HT_QUEST
        guideline.gameObject.transform.Find("guide_display").GetComponent<MeshRenderer>().material.SetColor("_Colour", k_aimColour_aim);
#endif
#if EIJIS_CALLSHOT && EIJIS_CALL_MARKER_TO_GRAY_ON_CUE_TRIGGER_ACTIVATE
        if (!callShotLockLocal)
        {
            graphicsManager._ChangePointPocketMarkerMaterial(false);
        }
#endif
    }

    public void _OnPickupCue()
    {
        if (!Networking.LocalPlayer.IsUserInVR()) desktopManager._OnPickupCue();
    }

    public void _OnDropCue()
    {
#if EIJIS_ISSUE_FIX
        if (ReferenceEquals(null, Networking.LocalPlayer)) return; // キューを持った状態でVRChatを終了するとエラーになる
#endif
        if (!Networking.LocalPlayer.IsUserInVR()) desktopManager._OnDropCue();
    }

    public void _TriggerOnPlayerPrepareShoot()
    {
        networkingManager._OnPlayerPrepareShoot();
    }

    public void _OnPlayerPrepareShoot()
    {
        cameraManager._OnPlayerPrepareShoot();
    }

    public void _TriggerPlaceBall(int idx)
    {
        if (!canPlayLocal) return; // in case player was forced to drop ball since someone else took the shot

        // practiceManager._Record();

#if EIJIS_SEMIAUTOCALL || EIJIS_ROTATION
        if (idx == 0)
        {
#if EIJIS_ROTATION
            if (isRotation && (nextBallRepositionStateLocal & 0x2u) > 0)
            {
                networkingManager.nextBallRepositionStateSynced = (byte)(nextBallRepositionStateLocal & ~0x2u);
                foulStateLocal = networkingManager.foulStateSynced = 2;
            }
#endif
#if EIJIS_SEMIAUTOCALL
            // cueBallFixed = true;
            cueBallRepositionCount++;
            // semiAutoCalledTimeBall = 0;
            semiAutoCallDelayBase = Networking.GetServerTimeInMilliseconds();
#if EIJIS_DEBUG_SEMIAUTO_CALL_AFTER_REPOSITION
            _LogInfo($"TKCH EIJIS_DEBUG_SEMIAUTO_CALL_AFTER_REPOSITION semiAutoCallDelayBase = {semiAutoCallDelayBase}, cueBallRepositionCount = {cueBallRepositionCount}");
#endif
            if (foulStateLocal == 1 || foulStateLocal == 2)
            {
                if (semiAutoCallLocal)
                {
                    networkingManager.calledBallsSynced = 0;
                    networkingManager.pointPocketsSynced = 0;
                }
            }
        }
#endif
#endif
        networkingManager._OnRepositionBalls(ballsP);
    }

    public void _TriggerGameStart()
    {

        if (playerIDsLocal[0] == -1)
        {
            _LogWarn("Cannot start without first player");
            return;
        }
        else
        {
            _LogYes("starting game");
        }
#if EIJIS_ROTATION
        if (isRotation)
        {
            Array.Copy(goalPointsLocal, networkingManager.goalPointsSynced, goalPointsLocal.Length);
            initializeRack_Rotation();
            initialPositions[GAMEMODE_ROTATION_15][0] = initialPositions[0][0];
            initialPositions[GAMEMODE_ROTATION_10][0] = initialPositions[0][0];
            initialPositions[GAMEMODE_ROTATION_9][0] = initialPositions[0][0];
            initialPositions[GAMEMODE_ROTATION_6][0] = initialPositions[0][0];
        }
#endif
        //0 is 8ball, 1 is 9ball, 2 is jp4b, 3 is kr4b, 4 is Snooker6Red)
#if EIJIS_MANY_BALLS
        Vector3[] randomPositions = new Vector3[MAX_BALLS];
        Array.Copy(initialPositions[gameModeLocal], randomPositions, MAX_BALLS);
#else
        Vector3[] randomPositions = new Vector3[16];
        Array.Copy(initialPositions[gameModeLocal], randomPositions, 16);
#endif
        if (RandomizeBallPositions)
        {
            switch (gameModeLocal)
            {
                case 0:
                    // 8ball
                    for (int i = 2; i < 16; i++)
                    {
                        // 8 and 14 are the far corner balls, don't randomize them so that one is always orange and one is always blue
                        if (i == 8 || i == 14) continue;
                        Vector3 temp = randomPositions[i];
                        int rand = UnityEngine.Random.Range(2, 16);
                        while (rand == 8 || rand == 14)
                            rand = UnityEngine.Random.Range(2, 16);

                        randomPositions[i] = randomPositions[rand];
                        randomPositions[rand] = temp;
                    }
                    // random swap of the corner colors
                    if (UnityEngine.Random.Range(0, 2) == 1)
                    {
                        Vector3 temp = randomPositions[8];
                        randomPositions[8] = randomPositions[14];
                        randomPositions[14] = temp;
                    }
                    break;
                case 1:
                    // 9ball
                    for (int i = 1; i < 9; i++)
                    {
                        // don't move the 1 or 9 balls
                        if (i == 2 || i == 9) continue;
                        Vector3 temp = randomPositions[i];
                        int rand = UnityEngine.Random.Range(1, 9);
                        while (rand == 2 || rand == 9)
                            rand = UnityEngine.Random.Range(1, 9);

                        randomPositions[i] = randomPositions[rand];
                        randomPositions[rand] = temp;
                    }
                    break;
#if EIJIS_10BALL
                case GAMEMODE_10BALL:
                    // 10ball
                    for (int i = 1; i < 10; i++)
                    {
                        // don't move the 1,2,3 or 10 balls
                        if (i == 2 || i == 3 || i == 4 || i == 10) continue;
                        Vector3 temp = randomPositions[i];
                        int rand = UnityEngine.Random.Range(1, 10);
                        while (rand == 2 || rand == 3 || rand == 4 || rand == 10)
                            rand = UnityEngine.Random.Range(1, 10);

                        randomPositions[i] = randomPositions[rand];
                        randomPositions[rand] = temp;
                    }
                    break;
#endif
#if EIJIS_BOWLARDS
                case GAMEMODE_BOWLARDS_10:
                case GAMEMODE_BOWLARDS_5:
                case GAMEMODE_BOWLARDS_1:
                    // Bowlards
                    int frameNumberBallId = pocketed_ball_upon_pool_order[0];
                    initializeRack_Bowlards(frameNumberBallId);
                    randomPositions = randomizeBallPositions_Bowlards(frameNumberBallId);
                    break;
#endif
            }
        }

        Debug.Log("_TriggerGameStart");

#if EIJIS_ROTATION
        networkingManager.inningCountSynced = 0;
#endif

        networkingManager._OnGameStart(initialBallsPocketed[gameModeLocal], randomPositions);
    }

    //LocalJoinTeam
    public void _TriggerJoinTeam(int teamId)
    {
        if (networkingManager.gameStateSynced == 0 || networkingManager.gameStateSynced == 3) return;

        _LogInfo("joining team " + teamId);
        //Udon chips
#if UDON_CHIPS
        udonChips = GameObject.Find("UdonChips").GetComponent<UdonChips>();
        VRCPlayerApi localplayer = Networking.LocalPlayer;
        int curSlottmp = _GetPlayerSlot(localplayer, playerIDsLocal);
        if (!(curSlottmp != -1))
        {
            if (udonChips.money >= Enter_cost)
            {
                udonChips.money -= Enter_cost;
            }
            else
            {

                _LogWarn("u need money to play");
                return;
            }
        }
#endif
        int newslot = networkingManager._OnJoinTeam(teamId);
        if (newslot != -1)
        {
#if EIJIS_BOWLARDS
            playerChangedLocal = true;
#endif
            //for responsive menu prediction. These values will be overwritten in deserialization
            isPlayer = true;
            VRCPlayerApi lp = Networking.LocalPlayer;
            int curSlot = _GetPlayerSlot(lp, playerIDsLocal);
            if (curSlot != -1)
            {
                playerIDsLocal[curSlot] = -1;
                if (curSlot % 2 == 0) { numPlayersCurrentOrange--; }
                else { numPlayersCurrentBlue--; }
            }
            int[] playerIDsLocal_new = new int[4];
            Array.Copy(playerIDsLocal, playerIDsLocal_new, 4);
            playerIDsLocal_new[newslot] = lp.playerId;

            Debug.Log("_TriggerJoinTeam");

            onRemotePlayersChanged(playerIDsLocal_new);
        }
        else
        {
            _LogWarn("failed to join team " + teamId + ", did someone else beat you to it?");
        }
    }

    //LocalLeaveLobby
    public void _TriggerLeaveLobby()
    {
#if UDON_CHIPS
        udonChips = GameObject.Find("UdonChips").GetComponent<UdonChips>();
        udonChips.money += Enter_cost;
#endif
        if (localPlayerId == -1) return;
        _LogInfo("leaving lobby");

        networkingManager._OnLeaveLobby(localPlayerId);

        //for responsive menu prediction, will be overwritten in deserialization
        isPlayer = false;
        int[] playerIDsLocal_new = new int[4];
        Array.Copy(playerIDsLocal, playerIDsLocal_new, 4);
        if (localPlayerId != -1) // true if lobby was closed
        {
            playerIDsLocal_new[localPlayerId] = -1;
        }

        Debug.Log("_TriggerLeaveLobby");
        onRemotePlayersChanged(playerIDsLocal_new);
    }
    private float lastActionTime;
    private float lastResetTime;
    public void _TriggerGameReset()
    {
        int self = Networking.LocalPlayer.playerId;

        int[] allowedPlayers = playerIDsLocal;

        bool allPlayersOffline = true;
        bool isAllowedPlayer = false;
        foreach (int allowedPlayer in allowedPlayers)
        {
            if (allPlayersOffline && Utilities.IsValid(VRCPlayerApi.GetPlayerById(allowedPlayer))) allPlayersOffline = false;

            if (allowedPlayer == self) isAllowedPlayer = true;
        }

        float nearestPlayer = float.MaxValue;
        for (int i = 0; i < allowedPlayers.Length; i++)
        {
            VRCPlayerApi player = VRCPlayerApi.GetPlayerById(allowedPlayers[i]);
            if (!Utilities.IsValid(player)) continue;
            float playerDist = Vector3.Distance(transform.position, player.GetPosition());
            if (playerDist < nearestPlayer)
                nearestPlayer = playerDist;
        }
        bool allPlayersAway = nearestPlayer < 20f ? false : true;

        if (Time.time - lastResetTime > 0.3f)
        {
            //infReset.text = "Double Click To Reset"; ClearResetInfo();
            infReset.text = _translations.Get("Double Click To Reset"); ClearResetInfo();
        }
        else if (allPlayersOffline || isAllowedPlayer || _IsModerator(Networking.LocalPlayer) || (Time.time - lastActionTime > 300) || allPlayersAway)
        {
            _LogInfo("force resetting game");
            //infReset.text = "Game Reset!"; ClearResetInfo();

            Debug.Log("_TriggerGameReset");

            infReset.text = _translations.Get("Game Reset!"); ClearResetInfo();
            networkingManager._OnGameReset();
        }
        else
        {
            string playerStr = "";
            bool has = false;
            foreach (int allowedPlayer in allowedPlayers)
            {
                if (allowedPlayer == -1) continue;
                if (has) playerStr += "\n";
                has = true;

                playerStr += graphicsManager._FormatName(VRCPlayerApi.GetPlayerById(allowedPlayer));
            }

            //infReset.text = "<size=60%>Only these players may reset:\n" + playerStr; ClearResetInfo();
            infReset.text = _translations.Get("<size=60%>Only these players may reset:\n") + playerStr; ClearResetInfo();
        }
        lastResetTime = Time.time;


    }

    int resetInfoCount = 0;
    private void ClearResetInfo()
    {
        resetInfoCount++;
        SendCustomEventDelayedSeconds(nameof(_ClearResetInfo), 3f);
    }

    public void _ClearResetInfo()
    {
        resetInfoCount--;
        if (resetInfoCount != 0) return;
        infReset.text = string.Empty;
    }
    #endregion

    #region NetworkingClient
    // the order is important, unfortunately
    public void _OnRemoteDeserialization()
    {
        _LogInfo("processing latest remote state ("/*packet="  + networkingManager.packetIdSynced + " ,*/+ "state=" + networkingManager.stateIdSynced + ")");
#if EIJIS_TABLE_LABEL
        Debug.Log("[BilliardsModule" + logLabel + "] latest game state is " + networkingManager._EncodeGameState());
#endif
#if EIJIS_DEBUG_TEAM_ID
        _LogInfo($"EIJIS_DEBUG BilliardsModule::_OnRemoteDeserialization() teamIdLocal = {teamIdLocal:X2}, teamIdSynced = {networkingManager.teamIdSynced:X2}, colorTurnLocal={colorTurnLocal}, colorTurnSynced={networkingManager.colorTurnSynced}");
#endif
#if EIJIS_DEBUG_FOUL_STATE
        _LogInfo($"EIJIS_DEBUG BilliardsModule::_OnRemoteDeserialization() foulStateLocal = {foulStateLocal}, foulStateSynced = {networkingManager.foulStateSynced}");
#endif
#if EIJIS_DEBUG_CALL_SAFETY
        _LogInfo($"EIJIS_DEBUG BilliardsModule::_OnRemoteDeserialization() safetyCalledLocal = {safetyCalledLocal}, safetyCalledSynced = {networkingManager.safetyCalledSynced}");
#endif
#if EIJIS_DEBUG_UNDO_REDO
        _LogInfo($"EIJIS_DEBUG BilliardsModule::_OnRemoteDeserialization() inningCountLocal = {inningCountLocal}, inningCountSynced = {networkingManager.inningCountSynced}");
#endif
#if EIJIS_DEBUG_SCORE_SCREEN
        _LogInfo($"EIJIS_DEBUG BilliardsModule::_OnRemoteDeserialization() gameStateLocal = {gameStateLocal}, gameStateSynced = {networkingManager.gameStateSynced}");
#endif
#if EIJIS_DEBUG_BOWLARDS
        _LogInfo($"EIJIS_DEBUG BilliardsModule::_OnRemoteDeserialization() INNING -> inningCountLocal = {inningCountLocal}, inningCountSynced = {networkingManager.inningCountSynced}");
        // _LogInfo($"                                                        FRAME_COUNT -> fbScoresLocal = {fbScoresLocal[0]}, {fbScoresLocal[1]}, fourBallScoresSynced = {networkingManager.fourBallScoresSynced[0]}, {networkingManager.fourBallScoresSynced[1]}");
        // _LogInfo($"                                                        goalPointsLocal = {goalPointsLocal[0]}, {goalPointsLocal[1]}, goalPointsSynced = {networkingManager.goalPointsSynced[0]}, {networkingManager.goalPointsSynced[1]}");
        // _LogInfo($"                                                        chainedFoulsLocal = {chainedFoulsLocal[0]}, {chainedFoulsLocal[1]}, chainedFoulsSynced = {networkingManager.chainedFoulsSynced[0]}, {networkingManager.chainedFoulsSynced[1]}");
        // _LogInfo($"                                                        playerChangedLocal = {playerChangedLocal}, playerChangedSynced = {networkingManager.playerChangedSynced}");
#endif
#if EIJIS_EXTERNAL_SCORE_SCREEN && EIJIS_ROTATION && EIJIS_BOWLARDS
        updateInfoText();
#endif
#if EIJIS_BOWLARDS
        if (isBowlards && gameLive)
        {
            bool canChoiceFoot = false;
            denyBalls = 0x0u;
            if (!networkingManager.colorTurnSynced && (networkingManager.foulStateSynced == 1 || networkingManager.foulStateSynced == 2))
            {
                bool allowBallFound = false;
                uint ball_bit = 0x2u;
                uint ballsInKitchen = 0x0u;
                for (int k = 0; k < break_order_10ball.Length; k++)
                {
                    int i = k + 1;
                    if ((ballsPocketedLocal & ball_bit) == 0x0u)
                    {
                        if (ballsP[i].x <= -(k_TABLE_WIDTH / 2))
                        {
                            ballsInKitchen |= 0x1u << i;
                        }
                        else
                        {
                            allowBallFound = true;
                        }
                    }
                    ball_bit <<= 1;
                }
                denyBalls = ballsInKitchen;
                canChoiceFoot = !allowBallFound;
            }
            graphicsManager._SetBallsDenyMark(denyBalls);
            networkingManager.nextBallRepositionStateSynced = (byte)(canChoiceFoot ? 1 : 0);
#if EIJIS_DEBUG_BOWLARDS
            // _LogInfo($"EIJIS_DEBUG  nextBallRepositionStateSynced = 0x{networkingManager.nextBallRepositionStateSynced:X02}, nextBallRepositionStateLocal = 0x{nextBallRepositionStateLocal:X02}");
#endif
        }
#endif

        lastActionTime = Time.time;
        waitingForUpdate = false;

        // propagate game settings first
        onRemoteGlobalSettingsUpdated(
            (byte)networkingManager.physicsSynced, (byte)networkingManager.tableModelSynced
        );
        onRemoteGameSettingsUpdated(
            networkingManager.gameModeSynced,
            networkingManager.timerSynced,
            networkingManager.teamsSynced,
            networkingManager.noGuidelineSynced,
#if EIJIS_GUIDELINE2TOGGLE
            networkingManager.noGuideline2Synced,
#endif
            networkingManager.noLockingSynced
#if EIJIS_10BALL
            , networkingManager.wpa10BallRuleSynced
#endif
#if EIJIS_CALLSHOT
            , networkingManager.requireCallShotSynced
            , networkingManager.callPassOptionSynced
#if EIJIS_SEMIAUTOCALL
            , networkingManager.semiAutoCallSynced
#endif
#endif
#if EIJIS_BOWLARDS
            , networkingManager.practiceModeMenuToggleSynced
#endif
        );
#if EIJIS_EXTERNAL_SCORE_SCREEN && EIJIS_BOWLARDS
        loadUsedLocal = networkingManager.loadUsedSynced;
        undoCountLocal = networkingManager.undoCountSynced;
        playerChangedLocal = networkingManager.playerChangedSynced;
        if (!ReferenceEquals(null, scoreScreen) && isBowlards)
        {
            scoreScreen.updateRegulationText(
                (string)tableModels[tableModelLocal].GetProgramVariable("TABLENAME"),
                timerLocal, requireCallShotLocal, noGuidelineLocal, noLockingLocal, practiceModeMenuToggleLocal,
                undoCountLocal, loadUsedLocal, playerChangedLocal
                );
        }
#endif

        // propagate valid players second
        onRemotePlayersChanged(networkingManager.playerIDsSynced);
#if EIJIS_CUEBALLSWAP || EIJIS_PUSHOUT
        bool stateIdChanged = (networkingManager.stateIdSynced != stateIdLocal);
#endif
#if EIJIS_BOWLARDS
        bool inningChanged = inningCountLocal != networkingManager.inningCountSynced;
        if (isBowlards && gameLive && inningChanged && teamIdLocal == networkingManager.teamIdSynced) aud_main.PlayOneShot(snd_NewTurn, 1.0f); 
        Array.Copy(networkingManager.framePointsSynced, framePointsLocal, framePointsLocal.Length);
        Array.Copy(networkingManager.framePointsSynced2, framePointsLocal2, framePointsLocal2.Length);
#endif
#if EIJIS_ROTATION
        Array.Copy(networkingManager.totalPointsSynced, totalPointsLocal, totalPointsLocal.Length);
        Array.Copy(networkingManager.highRunsSynced, highRunsLocal, highRunsLocal.Length);
        Array.Copy(networkingManager.chainedPointsSynced, chainedPointsLocal, chainedPointsLocal.Length);
        inningCountLocal = networkingManager.inningCountSynced;
        // Array.Copy(networkingManager.winRackCountSynced, winRackCountLocal, winRackCountLocal.Length);
#endif
#if EIJIS_EXTERNAL_SCORE_SCREEN
        bool scoreUpdate = true;
        if (lobbyOpen || gameLive)
        {
            if (networkingManager.stateIdSynced <= 1)
            {
                if (!ReferenceEquals(null, scoreScreen))
                {
                    scoreScreen.Clear();
                    if (isRotation) scoreScreen.updateGoalPoint_Rotation(networkingManager.goalPointsSynced[0], networkingManager.goalPointsSynced[1]);
                }
                if (0 < networkingManager.stateIdSynced)
                {
                    scoreUpdate = false;
                }
                else
                {
                    graphicsManager._OnGameStarted();
                }
            }
        }
#if EIJIS_DEBUG_SCORE_SCREEN
        _LogInfo($"scoreUpdate = {scoreUpdate}");
#endif
        
        if (scoreUpdate)
        {
            if (!ReferenceEquals(null, scoreScreen))
            {
                if (isRotation)
                {
                    scoreScreen.updateValues_Rotation(
                        totalPointsLocal[0], 
                        goalPointsLocal[0],
                        highRunsLocal[0],
                        chainedFoulsLocal[0], 
                        totalPointsLocal[1],
                        goalPointsLocal[1],
                        highRunsLocal[1],
                        chainedFoulsLocal[1],
                        inningCountLocal
                    );
                }
                else if (isBowlards)
                {
                    int frameLength = isBowlards1Frame ? 1 : (isBowlards5Frame ? 5 : 10);
                    if (gameLive || networkingManager.gameStateSynced == 2 || networkingManager.gameStateSynced == 4)
                    {
                        byte[][] framePoints = generateBowlardsScoreByteArrays(frameLength);
#if EIJIS_DEBUG_SCORE_SCREEN && EIJIS_DEBUG_BOWLARDS
                        for (int t = 0; t < 2; t++)
                        {
                            _LogInfo(
                                $"{framePoints[t][0]:X2} "+
                                $"{framePoints[t][1]:X2} "+
                                $"{framePoints[t][2]:X2} "+
                                $"{framePoints[t][3]:X2} "+
                                $"{framePoints[t][4]:X2} "+
                                $"{framePoints[t][5]:X2} "+
                                $"{framePoints[t][6]:X2} "+
                                $"{framePoints[t][7]:X2} "+
                                $"{framePoints[t][8]:X2} "+
                                $"{framePoints[t][9]:X2} "+
                                $"{framePoints[t][10]:X}"
                            );
                        }
#endif
                        int[] frameCounts = new int[] {networkingManager.fourBallScoresSynced[0] + (10 - frameLength), networkingManager.fourBallScoresSynced[1] + (10 - frameLength)};
                        int throwInningCount = inningCountLocal;
                        int teamId = networkingManager.teamIdSynced &= 0x0F;
                        scoreScreen.updateValues_Bowlards(teamId, frameCounts, throwInningCount, framePoints);
                    }
                }
            }
        }
#endif
#if EIJIS_PUSHOUT
        onRemotePushOutStateChanged(networkingManager.pushOutStateSynced, stateIdChanged);
#endif
#if EIJIS_ROTATION
        onRemoteGoalPointChanged(networkingManager.goalPointsSynced);
#endif
        // apply state transitions if needed
        onRemoteGameStateChanged(networkingManager.gameStateSynced);

        // now update game state
        onRemoteBallPositionsChanged(networkingManager.ballsPSynced);
        onRemoteTeamIdChanged(networkingManager.teamIdSynced);
        onRemoteFourBallCueBallChanged(networkingManager.fourBallCueBallSynced);
        onRemoteColorTurnChanged(networkingManager.colorTurnSynced);
        onRemoteBallsPocketedChanged(networkingManager.ballsPocketedSynced);
        onRemoteFoulStateChanged(networkingManager.foulStateSynced);
        onRemoteFourBallScoresUpdated(networkingManager.fourBallScoresSynced);
#if EIJIS_ROTATION
        Array.Copy(networkingManager.chainedFoulsSynced, chainedFoulsLocal, chainedFoulsLocal.Length);
#endif
        onRemoteIsTableOpenChanged(networkingManager.isTableOpenSynced, networkingManager.teamColorSynced);
#if EIJIS_CALLSHOT
        onRemoteTurnStateChanged(networkingManager.turnStateSynced, stateIdChanged);
#else
        onRemoteTurnStateChanged(networkingManager.turnStateSynced);
#endif
#if EIJIS_CUEBALLSWAP || EIJIS_CALLSHOT
        onRemoteCallStateChanged(networkingManager.calledBallsSynced, networkingManager.pointPocketsSynced, networkingManager.callShotLockSynced, networkingManager.safetyCalledSynced, stateIdChanged);
#endif
#if EIJIS_ROTATION
        onRemoteNextBallRepositionStateChanged(networkingManager.nextBallRepositionStateSynced);
#endif
#if EIJIS_CALLSHOT
        graphicsManager._UpdateCueGrip();
        graphicsManager._UpdateDevhit();
#endif

        // finally, take a snapshot
        practiceManager._Record();

#if EIJIS_CUEBALLSWAP
        if (networkingManager.stateIdSynced == 0)
        {
            networkingManager.stateIdSynced = 2;
        }
        stateIdLocal = networkingManager.stateIdSynced;
#endif
        redrawDebugger();
#if EIJIS_SEMIAUTOCALL
        semiAutoCallTick = (requireCallShotLocal && semiAutoCallLocal);
#if EIJIS_DEBUG_SEMIAUTO_CALL_AFTER_REPOSITION
        debugLogFlg = true;
        debugLogStart = Networking.GetServerTimeInMilliseconds();
        _LogInfo($"TKCH EIJIS_DEBUG_SEMIAUTO_CALL_AFTER_REPOSITION semiAutoCallTick = {semiAutoCallTick}, cueBallFixed = {cueBallFixed}");
#endif
#endif
    }

    private void onRemoteGlobalSettingsUpdated(byte physicsSynced, byte tableModelSynced)
    {
        // if (gameLive) return;

        _LogInfo($"onRemoteGlobalSettingsUpdated physicsMode={physicsSynced} tableModel={tableModelSynced}");

        if (currentPhysicsManager != PhysicsManagers[physicsSynced])
        {
            currentPhysicsManager = PhysicsManagers[physicsSynced];
            currentPhysicsManager.SendCustomEvent("_InitConstants");
            menuManager._RefreshPhysics();
            desktopManager._RefreshPhysics();
        }
        if (tableModelLocal != tableModelSynced)
        {
            setTableModel(tableModelSynced);
            menuManager._RefreshGameMode();
        }
    }

#if EIJIS_GUIDELINE2TOGGLE || EIJIS_CALLSHOT || EIJIS_SEMIAUTOCALL
    private void onRemoteGameSettingsUpdated(uint gameModeSynced, uint timerSynced, bool teamsSynced, bool noGuidelineSynced
#if EIJIS_GUIDELINE2TOGGLE
        , bool noGuideline2Synced
#endif
        , bool noLockingSynced
#if EIJIS_10BALL
        , bool wpa10BallRuleSynced
#endif
#if EIJIS_CALLSHOT
        , bool requireCallShotSynced
        , bool callPassOptionSynced
#if EIJIS_SEMIAUTOCALL
        , bool semiAutoCallSynced
#endif
#endif
#if EIJIS_BOWLARDS
        , bool practiceModeMenuToggleSynced
#endif
    )
#else
    private void onRemoteGameSettingsUpdated(uint gameModeSynced, uint timerSynced, bool teamsSynced, bool noGuidelineSynced, bool noLockingSynced)
#endif
    {
        if (
            gameModeLocal == gameModeSynced &&
            timerLocal == timerSynced &&
            teamsLocal == teamsSynced &&
            noGuidelineLocal == noGuidelineSynced &&
#if EIJIS_GUIDELINE2TOGGLE
            noGuideline2Local == noGuideline2Synced &&
#endif
            noLockingLocal == noLockingSynced
#if EIJIS_10BALL
            && wpa10BallRuleLocal == wpa10BallRuleSynced
#endif
#if EIJIS_CALLSHOT
            && requireCallShotLocal == requireCallShotSynced
            && callPassOptionLocal == callPassOptionSynced
#if EIJIS_SEMIAUTOCALL
            && semiAutoCallLocal == semiAutoCallSynced
#endif
#endif
#if EIJIS_BOWLARDS
            && practiceModeMenuToggleLocal == practiceModeMenuToggleSynced
#endif
        )
        {
            return;
        }

#if EIJIS_GUIDELINE2TOGGLE && EIJIS_CALLSHOT && EIJIS_SEMIAUTOCALL && EIJIS_10BALL
        _LogInfo($"onRemoteGameSettingsUpdated gameMode={gameModeSynced} timer={timerSynced} teams={teamsSynced} guideline={!noGuidelineSynced} guideline2={!noGuideline2Synced} locking={!noLockingSynced}");
        _LogInfo($"                            wpa10BallRule={wpa10BallRuleSynced} callShot={requireCallShotSynced} callPass={callPassOptionSynced} semiAutCcall={semiAutoCallSynced}");
#else
        _LogInfo($"onRemoteGameSettingsUpdated gameMode={gameModeSynced} timer={timerSynced} teams={teamsSynced} guideline={!noGuidelineSynced} locking={!noLockingSynced}");
#endif

        if (gameModeLocal != gameModeSynced)
        {
            gameModeLocal = gameModeSynced;

            is8Ball = gameModeLocal == 0u;
            is9Ball = gameModeLocal == 1u;
            isJp4Ball = gameModeLocal == 2u;
            isKr4Ball = gameModeLocal == 3u;
#if EIJIS_SNOOKER15REDS
            isSnooker15Red = gameModeLocal == 4u;
#else
            isSnooker6Red = gameModeLocal == 4u;
#endif
#if EIJIS_PYRAMID
            isPyramid = gameModeLocal == GAMEMODE_PYRAMID;
#endif
#if EIJIS_CAROM
            is3Cusion = gameModeLocal == GAMEMODE_3CUSHION;
            is2Cusion = gameModeLocal == GAMEMODE_2CUSHION;
            is1Cusion = gameModeLocal == GAMEMODE_1CUSHION;
            is0Cusion = gameModeLocal == GAMEMODE_0CUSHION;
#if EIJIS_BANKING
            isBanking = gameModeLocal == GAMEMODE_BANKING;
            cushionHitGoal = is0Cusion ? 0 : ((is1Cusion || isBanking) ? 1 : (is2Cusion ? 2 : 3));
            isCarom = isJp4Ball || isKr4Ball || is3Cusion || is2Cusion || is1Cusion || is0Cusion || isBanking;
#else
            cushionHitGoal = is0Cusion ? 0 : (is1Cusion ? 1 : (is2Cusion ? 2 : 3));
            isCarom = isJp4Ball || isKr4Ball || is3Cusion || is2Cusion || is1Cusion || is0Cusion;
#endif
#else
            isCarom = isJp4Ball || isKr4Ball;
#endif
#if EIJIS_SNOOKER15REDS
            isSnooker = isSnooker6Red || isSnooker15Red;
#endif
#if EIJIS_10BALL
            is10Ball = gameModeLocal == GAMEMODE_10BALL;
#endif
#if EIJIS_ROTATION
            isRotation15Balls = gameModeLocal == GAMEMODE_ROTATION_15;
            isRotation10Balls = gameModeLocal == GAMEMODE_ROTATION_10;
            isRotation9Balls = gameModeLocal == GAMEMODE_ROTATION_9;
            isRotation6Balls = gameModeLocal == GAMEMODE_ROTATION_6;
            isRotation = isRotation15Balls || isRotation10Balls || isRotation9Balls || isRotation6Balls;
            if (isRotation)
            {
                goalPointsLocal = new ushort[] {120, 120};
                Array.Copy(goalPointsLocal, networkingManager.goalPointsSynced, goalPointsLocal.Length);
                menuManager._RefreshGoalPoint();
            }
#endif
#if EIJIS_BOWLARDS
            isBowlards10Frame = gameModeLocal == GAMEMODE_BOWLARDS_10;
            isBowlards5Frame = gameModeLocal == GAMEMODE_BOWLARDS_5;
            isBowlards1Frame = gameModeLocal == GAMEMODE_BOWLARDS_1;
            isBowlards = isBowlards10Frame || isBowlards5Frame || isBowlards1Frame;
#endif
#if EIJIS_10BALL && EIJIS_ROTATION && EIJIS_BOWLARDS
            pocketMask = is10Ball || isRotation10Balls || isBowlards ? pocket_mask_10ball
                : (is9Ball || isRotation9Balls ? pocket_mask_9ball
                    : (isRotation6Balls ? rotation_pocket_masks[GAMEMODE_ROTATION_6 & GAME_MODE_ROTATION_MASK]
                        : pocket_mask_8ball));
            ballsLengthByPocketGame = (is9Ball || isRotation9Balls ? break_order_9ball.Length
                : (is10Ball || isRotation10Balls || isBowlards ? break_order_10ball.Length
                    : (isRotation6Balls ? break_order_rotation_6ball.Length : break_order_8ball.Length)));
#endif
#if EIJIS_EXTERNAL_SCORE_SCREEN
            
            string gameName = String.Empty;
            int gameNameDisplayNumber = 0;
            if (isRotation)
            {
                scoreScreen = scoreScreenRotation;
                gameName = "Rotation";
                gameNameDisplayNumber = ballsLengthByPocketGame;
            }
            else if (isBowlards)
            {
                int frameLength = isBowlards1Frame ? 1 : (isBowlards5Frame ? 5 : 10);

                scoreScreen = scoreScreenBowlards;
                gameName = "Bowlards";
                gameNameDisplayNumber = frameLength;
                
                scoreScreen.setFrameLength_Bowlards(frameLength);
            }
            else
            {
                scoreScreen = null;
            }

            if (!ReferenceEquals(null, scoreScreen))
            {
                scoreScreen.UpdateGameNameWithNumber(gameName, gameNameDisplayNumber);
            }
            for (int i = 0; i < scoreScreens.Length; i++)
            {
                if (ReferenceEquals(null, scoreScreens[i])) continue;
                scoreScreens[i].gameObject.SetActive(scoreScreen == scoreScreens[i]);
            }
#endif

            menuManager._RefreshGameMode();
        }

        if (timerLocal != timerSynced)
        {
            timerLocal = timerSynced;

            menuManager._RefreshTimer();
        }

        bool refreshToggles = false;
        if (teamsLocal != teamsSynced)
        {
            teamsLocal = teamsSynced;
            refreshToggles = true;
            isOrangeTeamFull = teamsLocal ? playerIDsLocal[0] != -1 && playerIDsLocal[2] != -1 : playerIDsLocal[0] != -1;
            isBlueTeamFull = teamsLocal ? playerIDsLocal[1] != -1 && playerIDsLocal[3] != -1 : playerIDsLocal[1] != -1;
            menuManager._RefreshMenu();
        }

        if (noGuidelineLocal != noGuidelineSynced)
        {
            noGuidelineLocal = noGuidelineSynced;
            refreshToggles = true;
        }

#if EIJIS_GUIDELINE2TOGGLE
        if (noGuideline2Local != noGuideline2Synced)
        {
            noGuideline2Local = noGuideline2Synced;
            refreshToggles = true;
        }
        
#endif
        if (noLockingLocal != noLockingSynced)
        {
            noLockingLocal = noLockingSynced;
            refreshToggles = true;
        }

#if EIJIS_10BALL
        if (wpa10BallRuleLocal != wpa10BallRuleSynced)
        {
            wpa10BallRuleLocal = wpa10BallRuleSynced;
            refreshToggles = true;
        }
        
#endif
#if EIJIS_CALLSHOT
        if (requireCallShotLocal != requireCallShotSynced)
        {
            requireCallShotLocal = requireCallShotSynced;
            refreshToggles = true;
        }
        
        if (callPassOptionLocal != callPassOptionSynced)
        {
            callPassOptionLocal = callPassOptionSynced;
            refreshToggles = true;
        }
        
#if EIJIS_SEMIAUTOCALL
        if (semiAutoCallLocal != semiAutoCallSynced)
        {
            semiAutoCallLocal = semiAutoCallSynced;
            refreshToggles = true;
        }
        
#endif
#endif
#if EIJIS_BOWLARDS
        if (practiceModeMenuToggleLocal != practiceModeMenuToggleSynced)
        {
            practiceModeMenuToggleLocal = practiceModeMenuToggleSynced;
            refreshToggles = true;

            if (isBowlards)
            {
                isPracticeMode = practiceModeMenuToggleLocal;
            }
        }
        
#endif
        if (refreshToggles)
        {
            menuManager._RefreshToggleSettings();
            menuManager._RefreshPlayerList();
        }
    }

    private void onRemotePlayersChanged(int[] playerIDsSynced)
    {
        // int myOldSlot = _GetPlayerSlot(Networking.LocalPlayer, playerIDsLocal);

        if (intArrayEquals(playerIDsLocal, playerIDsSynced)) return;

        Array.Copy(playerIDsLocal, playerIDsCached, playerIDsLocal.Length);
        Array.Copy(playerIDsSynced, playerIDsLocal, playerIDsLocal.Length);

        if (networkingManager.gameStateSynced != 3) // don't set practice mode to true after the players are kicked when the game ends
#if EIJIS_BOWLARDS
        {
            isPracticeMode = playerIDsLocal[1] == -1 && playerIDsLocal[3] == -1 && (!isBowlards || (isBowlards && practiceModeMenuToggleLocal));

            if (networkingManager.gameStateSynced == 2 || networkingManager.gameStateSynced == 4)
            {
                if (!teamsLocal && (playerIDsCached[1] == -1 && playerIDsCached[1] != playerIDsLocal[1]))
                {
                    clearBowlardsScoreVariables(1);
                }
            }
        }
#if EIJIS_DEBUG_BOWLARDS
        _LogInfo($"EIJIS_DEBUG  practiceModeMenuToggleLocal = {practiceModeMenuToggleLocal}, isPracticeMode = {isPracticeMode}");
#endif
#else
            isPracticeMode = playerIDsLocal[1] == -1 && playerIDsLocal[3] == -1;
#endif

        string[] playerDetails = new string[4];
        for (int i = 0; i < 4; i++)
        {
            VRCPlayerApi plyr = VRCPlayerApi.GetPlayerById(playerIDsSynced[i]);
            playerDetails[i] = (playerIDsSynced[i] == -1 || plyr == null) ? "none" : plyr.displayName;
        }
        _LogInfo($"onRemotePlayersChanged newPlayers={string.Join(",", playerDetails)}");

        localPlayerId = Array.IndexOf(playerIDsLocal, Networking.LocalPlayer.playerId);
        if (localPlayerId != -1) localTeamId = (uint)(localPlayerId & 0x1u);
        else localTeamId = uint.MaxValue;

        cueControllers[0]._SetAuthorizedOwners(new int[] { playerIDsLocal[0], playerIDsLocal[2] });
        cueControllers[1]._SetAuthorizedOwners(new int[] { playerIDsLocal[1], playerIDsLocal[3] });
        cueControllers[1]._RefreshRenderer();// 2nd cue is invisible in practice mode

        if (playerIDsLocal[0] == -1 && playerIDsLocal[2] == -1)
        {
            cueControllers[0]._ResetCuePosition();
        }

        if (playerIDsLocal[1] == -1 && playerIDsLocal[3] == -1)
        {
            cueControllers[1]._ResetCuePosition();
        }

        applyCueAccess();

        if (networkingManager.gameStateSynced != 3) { graphicsManager._SetScorecardPlayers(playerIDsLocal); } // don't remove player names when match is won

        int myNewSlot = _GetPlayerSlot(Networking.LocalPlayer, playerIDsLocal);
        isPlayer = myNewSlot != -1;

        isOrangeTeamFull = teamsLocal ? playerIDsLocal[0] != -1 && playerIDsLocal[2] != -1 : playerIDsLocal[0] != -1;
        isBlueTeamFull = teamsLocal ? playerIDsLocal[1] != -1 && playerIDsLocal[3] != -1 : playerIDsLocal[1] != -1;
        menuManager._RefreshLobby();

        // return gameLive && myOldSlot != myNewSlot;//if our slot changed, we left, or we joined, return true
    }

    private void onRemoteGameStateChanged(byte gameStateSynced)
    {
        if (gameStateLocal == gameStateSynced) return;

        gameStateLocal = gameStateSynced;
        _LogInfo($"onRemoteGameStateChanged newState={gameStateSynced}");

        if (gameStateLocal == 1)
        {
            onRemoteLobbyOpened();
        }
        else if (gameStateLocal == 0)
        {
            onRemoteLobbyClosed();
        }
        else if (gameStateLocal == 2)
        {
            onRemoteGameStarted();
        }
        else if (gameStateLocal == 3)
        {
            onRemoteGameEnded(networkingManager.winningTeamSynced);
        }
#if EIJIS_ROTATION
        else if (gameStateSynced == 4) // continue next break
        {
            onRemoteGameStarted();
        }
#endif
        for (int i = 0; i < cueControllers.Length; i++) cueControllers[i]._RefreshRenderer();
    }

    private void onRemoteLobbyOpened()
    {
        _LogInfo($"onRemoteLobbyOpened");

        lobbyOpen = true;
        graphicsManager._OnLobbyOpened();
        menuManager._RefreshLobby();
        cueControllers[0].resetScale();
        cueControllers[1].resetScale();
#if EIJIS_EXTERNAL_SCORE_SCREEN
        if (!ReferenceEquals(null, scoreScreen))
        {
            scoreScreen.initValues();
            scoreScreen.clearInfoText();
        }
#endif

        if (callbacks != null) callbacks.SendCustomEvent("_OnLobbyOpened");
    }

    private void onRemoteLobbyClosed()
    {
        _LogInfo($"onRemoteLobbyClosed");

        lobbyOpen = false;
        localPlayerId = -1;
        graphicsManager._OnLobbyClosed();
        menuManager._RefreshLobby();

        if (networkingManager.winningTeamSynced == 2)
        {
            _LogWarn("game reset");
            graphicsManager._OnGameReset();
        }
        gameLive = false;

        disablePlayComponents();
        resetCachedData();

        if (callbacks != null) callbacks.SendCustomEvent("_OnLobbyClosed");
    }

    private void onRemoteGameStarted()
    {

        _LogInfo($"onRemoteGameStarted");

        HeightBreak = 0;
        ShotCounts = 0;

        lobbyOpen = false;
        gameLive = true;

        Array.Clear(perfCounters, 0, PERF_MAX);
        Array.Clear(perfStart, 0, PERF_MAX);
        Array.Clear(perfTimings, 0, PERF_MAX);

#if EIJIS_BOWLARDS
        isPracticeMode = playerIDsLocal[1] == -1 && playerIDsLocal[3] == -1 && (!isBowlards || (isBowlards && practiceModeMenuToggleLocal));
        networkingManager.practiceModeMenuToggleSynced = isPracticeMode;
        practiceModeMenuToggleLocal = !isPracticeMode;
#if EIJIS_DEBUG_BOWLARDS
        _LogInfo($"EIJIS_DEBUG  stateIdLocal = {stateIdLocal}, stateIdSynced = {networkingManager.stateIdSynced}");
        // _LogInfo($"EIJIS_DEBUG  practiceModeMenuToggleLocal = {practiceModeMenuToggleLocal}, isPracticeMode = {isPracticeMode}");
#endif
#else
        isPracticeMode = playerIDsLocal[1] == -1 && playerIDsLocal[3] == -1;
#endif

        menuManager._RefreshLobby();
#if EIJIS_ROTATION
        if (isRotation)
        {
            graphicsManager._SetUSColors(true);
        }
#endif
        graphicsManager._OnGameStarted();
        desktopManager._OnGameStarted();
        applyCueAccess();
#if EIJIS_ROTATION || EIJIS_BOWLARDS
        if (stateIdLocal <= 3) practiceManager._Clear();
#else 
        practiceManager._Clear();
#endif
        repositionManager._OnGameStarted();
        for (int i = 0; i < cueControllers.Length; i++) cueControllers[i]._RefreshRenderer();

        Array.Clear(fbScoresLocal, 0, 2);
        auto_pocketblockers.SetActive(isCarom);
#if EIJIS_PYRAMID || EIJIS_10BALL || EIJIS_ROTATION || EIJIS_BOWLARDS
        marker9ball.SetActive(is9Ball || is10Ball || isPyramid || isRotation || isBowlards);
#else
        marker9ball.SetActive(is9Ball);
#endif
#if EIJIS_ROTATION
        if (!ReferenceEquals(null, markerHeadSpot)) markerHeadSpot.SetActive(false);
        if (!ReferenceEquals(null, markerCenterSpot)) markerCenterSpot.SetActive(false);
        if (!ReferenceEquals(null, markerFootSpot)) markerFootSpot.SetActive(false);
        if (!ReferenceEquals(null, requestBreakOrange)) requestBreakOrange.SetActive(false);
        if (!ReferenceEquals(null, requestBreakBlue)) requestBreakBlue.SetActive(false);
#endif
#if EIJIS_CUEBALLSWAP || EIJIS_CALLSHOT
        calledBallsLocal = 0;
        calledBallId = -2;
#if EIJIS_CALLSHOT
        pointPocketsLocal = 0;
        calledPocketId = -2;
#if EIJIS_SEMIAUTOCALL
        semiAutoCalledPocket = false;
        semiAutoCalledTimeBall = 0;
#endif
#endif
#endif
#if EIJIS_DEBUG_BOWLARDS
        denyBalls = 0x0u;
        graphicsManager._SetBallsDenyMark(denyBalls);
#endif
#if EIJIS_PUSHOUT
        pushOutStateLocal = PUSHOUT_BEFORE_BREAK;
        graphicsManager._UpdatePushOut(pushOutStateLocal);
#endif
#if EIJIS_CALLSHOT
        graphicsManager._UpdateCallSafety(safetyCalledLocal);
#endif
#if EIJIS_CALLSHOT && EIJIS_PUSHOUT
        cantSkipNextTurnOnCallShotOption = false;
#endif

        // Reflect game state
        graphicsManager._UpdateScorecard();
        isReposition = false;
        markerObj.SetActive(false);

        // Effects
#if EIJIS_ROTATION
        graphicsManager._PlayIntroAnimation((isRotation && networkingManager.foulStateSynced == 3)? rotation_pocket_masks[gameModeLocal & GAME_MODE_ROTATION_MASK] : 0xFFFFu);
#else
        graphicsManager._PlayIntroAnimation();
#endif
        aud_main.PlayOneShot(snd_Intro, 1.0f);

        timerRunning = false;

#if EIJIS_ISSUE_FIX
        activeCue = cueControllers[(isPracticeMode ? 0 : teamIdLocal)];
#else
        activeCue = cueControllers[0];
#endif
#if EIJIS_DEBUG_INITIALIZERACK
        _LogInfo($"  initialPositions[GAMEMODE_ROTATION_15][0] x = {initialPositions[GAMEMODE_ROTATION_15][0].x}, z = {initialPositions[GAMEMODE_ROTATION_15][0].z}");
        _LogInfo($"  initialPositions[GAMEMODE_ROTATION_10][0] x = {initialPositions[GAMEMODE_ROTATION_10][0].x}, z = {initialPositions[GAMEMODE_ROTATION_10][0].z}");
        _LogInfo($"  initialPositions[GAMEMODE_ROTATION_9][0] x = {initialPositions[GAMEMODE_ROTATION_9][0].x}, z = {initialPositions[GAMEMODE_ROTATION_9][0].z}");
        _LogInfo($"  initialPositions[GAMEMODE_ROTATION_6][0] x = {initialPositions[GAMEMODE_ROTATION_6][0].x}, z = {initialPositions[GAMEMODE_ROTATION_6][0].z}");
#endif

    }

    private void onRemoteBallPositionsChanged(Vector3[] ballsPSynced)
    {
        if (vector3ArrayEquals(ballsP, ballsPSynced)) return;

        _LogInfo($"onRemoteBallPositionsChanged");

        Array.Copy(ballsPSynced, ballsP, ballsP.Length);

        _Update9BallMarker();
    }

    private void onRemoteGameEnded(uint winningTeamSynced)
    {

        _LogInfo($"onRemoteGameEnded winningTeam={winningTeamSynced}");

        isLocalSimulationRunning = false;

        winningTeamLocal = winningTeamSynced;

        if (winningTeamLocal < 2)
        {
            string p1str = "No one";
            string p2str = "No one";
            VRCPlayerApi winner1 = VRCPlayerApi.GetPlayerById(playerIDsCached[winningTeamLocal]);
            if (Utilities.IsValid(winner1))
                p1str = winner1.displayName;
            VRCPlayerApi winner2 = VRCPlayerApi.GetPlayerById(playerIDsCached[winningTeamLocal + 2]);
            if (Utilities.IsValid(winner2))
                p2str = winner2.displayName;
            // All players are kicked from the match when it's won, so use the previous turn's player names to show the winners (playerIDsCached)
            _LogWarn("game over, team " + winningTeamLocal + " won (" + p1str + " and " + p2str + ")");
            graphicsManager._SetWinners(/* isPracticeMode ? 0u :  */winningTeamLocal, playerIDsCached);
#if UDON_CHIPS
            VRCPlayerApi LocalPlayer = Networking.LocalPlayer;
            udonChips = GameObject.Find("UdonChips").GetComponent<UdonChips>();
            if (LocalPlayer == winner1 ||  LocalPlayer == winner2)
            {
                udonChips.money += winner_gain;
            }
            else
            {
                udonChips.money -= loser_lose;
            }
#endif
            //_LogYes("shotcounts"+ShotCounts);
            if (personalData != null && !isPracticeMode && ShotCounts != 0)
            {
                VRCPlayerApi localPlayer = Networking.LocalPlayer;
                if (localPlayer == winner1 || localPlayer == winner2 )
                {
                    if (isSnooker) personalData.gameCountSnooker++;
                    else personalData.gameCount++;
                    personalData.winCount++;
                }
                else
                {
                    if(localPlayer != winner1 && localPlayer != winner2 )
                    {
                        if (isSnooker) personalData.gameCountSnooker++;
                        else personalData.gameCount++;
                        personalData.loseCount++;
                        
                    }
                }
                personalData.SaveData();
            }
            if(DG_LAB != null)
            {
                if(Networking.LocalPlayer != winner1 &&  Networking.LocalPlayer != winner2 )
                {
                    DG_LAB.SendCustomEvent("JustShock");
                }
            }
        }

        //UP 24/6/15  重构by cheese  24/9/26 难以想象居然撑了三个月
        if (ScoreManager != null && !isPracticeMode)
        {
            if (!BreakFinish)  //Breakfinish是由复用的参数计算的
                ScoreManager.AddScore(playerIDsCached[0], playerIDsCached[1], playerIDsCached[winningTeamLocal], isSnooker15Red  && (string)tableModels[tableModelLocal].GetProgramVariable("TABLENAME") ==  "Snooker 12ft" );
        }
        //这段代码必须在resetCachedData前面,不然gamemode被重置了,不过用snooker简化就没事,放这里以防万一

        gameLive = false;
        isPracticeMode = false;

        Array.Copy(networkingManager.fourBallScoresSynced, fbScoresLocal, 2);
        graphicsManager._UpdateTeamColor(winningTeamSynced);
        graphicsManager._UpdateScorecard();
        graphicsManager._RackBalls();
#if EIJIS_CALLSHOT
        graphicsManager._DisablePointPocketMarker();
#else
        graphicsManager._UpdatePointPocketMarker(0, callShotLockLocal);
#endif

        disablePlayComponents();
#if EIJIS_CALLSHOT
        if (!ReferenceEquals(null, callSafetyOrb)) callSafetyOrb.SetActive(false);
#if EIJIS_CALL_DEFAULT_LOCKING
        if (!ReferenceEquals(null, callClearOrb)) callClearOrb.SetActive(false);
#else
        if (!ReferenceEquals(null, callShotLockOrb)) callShotLockOrb.SetActive(false);
#endif
        safetyCalledLocal = false;
#endif
#if EIJIS_ROTATION
        nextBallRepositionStateLocal = 0;
        _UpdateNextBallRepositionSpotMarker();
#endif

        localPlayerId = -1;
        localTeamId = uint.MaxValue;
        applyCueAccess();

        lobbyOpen = false;

        for (int i = 0; i < cueControllers.Length; i++) cueControllers[i]._RefreshRenderer();

        infReset.text = string.Empty;

        resetCachedData();

        menuManager._RefreshLobby();

    }

    private void onRemoteBallsPocketedChanged(uint ballsPocketedSynced)
    {
        if (!gameLive) return;

        // todo: actually use a separate variable to track local modifications to balls pocketed
        if (ballsPocketedLocal != ballsPocketedSynced) _LogInfo($"onRemoteBallsPocketedChanged ballsPocketed={ballsPocketedSynced:X}");

        ballsPocketedLocal = ballsPocketedSynced;

        graphicsManager._UpdateScorecard();
        graphicsManager._RackBalls();

        refreshBallPickups();
    }

    private void onRemoteFourBallScoresUpdated(byte[] fbScoresSynced)
    {
        if (!gameLive) return;

        if (fbScoresLocal[0] == fbScoresSynced[0] && fbScoresLocal[1] == fbScoresSynced[1])
        {
            _LogInfo($"onRemoteFourBallScoresUpdated team1={fbScoresSynced[0]} team2={fbScoresSynced[1]}");
            //don't escape, as this will always be true for the sender, and they may need to run the rest.
        }
#if EIJIS_SNOOKER15REDS || EIJIS_PYRAMID || EIJIS_BOWLARDS
        if (!isSnooker && !isCarom && !isPyramid && !isBowlards) { return; }   //cheese fix
#else
        if (!isSnooker6Red && !isCarom) { return; }
#endif

        Array.Copy(fbScoresSynced, fbScoresLocal, 2);
        graphicsManager._UpdateScorecard();
    }

    private void onRemoteTeamIdChanged(uint teamIdSynced)
    {
#if EIJIS_DEBUG_TEAM_ID
        _LogInfo($"EIJIS BilliardsModule::onRemoteTeamIdChanged(teamIdSynced = 0x{teamIdSynced:X}) teamIdLocal = 0x{teamIdLocal:X}");
#endif
        if (!gameLive) return;

        if (teamIdLocal != teamIdSynced)
        {
#if EIJIS_BOWLARDS
            networkingManager.teamIdSynced &= 0x0F;
            teamIdLocal = networkingManager.teamIdSynced;
#else
            teamIdLocal = teamIdSynced;
#endif
            aud_main.PlayOneShot(snd_NewTurn, 1.0f);
            _LogInfo($"onRemoteTeamIdChanged newTeam={teamIdSynced}");
        }

        graphicsManager._UpdateTeamColor(teamIdLocal);

        // always use first cue if practice mode
        activeCue = cueControllers[isPracticeMode ? 0 : (int)teamIdLocal];
    }

    private void onRemoteFourBallCueBallChanged(uint fourBallCueBallSynced)
    {
        if (!gameLive) return;

        if (fourBallCueBallLocal != fourBallCueBallSynced)
        {
            _LogInfo($"onRemoteFourBallCueBallChanged cueBall={fourBallCueBallSynced}");
        }

#if EIJIS_SNOOKER15REDS
        if (isSnooker)//reusing this variable for the number of fouls/repeated shots in a row in snooker
#else
        if (isSnooker6Red)//reusing this variable for the number of fouls/repeated shots in a row in snooker
#endif
        {
            fourBallCueBallLocal = fourBallCueBallSynced;
        }
        if (!isCarom) return;

        fourBallCueBallLocal = fourBallCueBallSynced;

        graphicsManager._UpdateFourBallCueBallTextures(fourBallCueBallLocal);
    }

    private void onRemoteIsTableOpenChanged(bool isTableOpenSynced, uint teamColorSynced)
    {
        if (!gameLive) return;

        if ((teamColorLocal != teamColorSynced || isTableOpenLocal != isTableOpenSynced))
        {
            _LogInfo($"onRemoteIsTableOpenChanged isTableOpen={isTableOpenSynced} teamColor={teamColorSynced}");
        }
        isTableOpenLocal = isTableOpenSynced;
        teamColorLocal = teamColorSynced;

        if (!isTableOpenLocal)
        {
            string color = (teamIdLocal ^ teamColorLocal) == 0 ? "blues" : "oranges";
            _LogInfo($"table closed, team {teamIdLocal} is {color}");
        }

        graphicsManager._UpdateTeamColor(teamIdLocal);
        graphicsManager._UpdateScorecard();
    }
    private void onRemoteColorTurnChanged(bool ColorTurnSynced)
    {
        if (!gameLive) return;

        if (colorTurnLocal == ColorTurnSynced) return;

        _LogInfo($"onRemoteColorTurnChanged colorTurn={ColorTurnSynced}");
        colorTurnLocal = ColorTurnSynced;
    }

    private void onRemoteFoulStateChanged(uint foulStateSynced)
    {
        if (!gameLive) return;

        if (foulStateLocal != foulStateSynced)
        {
            _LogInfo($"onRemoteFoulStateChanged foulState={foulStateSynced}");
            // should not escape here because it can stay the same turn to turn while whos turn it is changes (especially with Undo/SnookerUndo)
        }

        foulStateLocal = foulStateSynced;
        bool myTurn = isMyTurn();
#if EIJIS_SNOOKER15REDS
        if (isSnooker)//enable SnookerUndo button if foul
#else
        if (isSnooker6Red)//enable SnookerUndo button if foul
#endif
        {
            if (fourBallCueBallLocal > 0 && foulStateLocal > 0 && foulStateLocal != 6 && myTurn && networkingManager.turnStateSynced != 1)
            {
                menuManager._EnableSnookerUndoMenu();
            }
            else
            {
                menuManager._DisableSnookerUndoMenu();
            }
        }

#if EIJIS_ROTATION
        if (!myTurn || foulStateLocal == 0 || (isRotation && foulStateLocal == 3))
#else
        if (!myTurn || foulStateLocal == 0)
#endif
        {
            isReposition = false;
            setFoulPickupEnabled(false);
            return;
        }

        if (foulStateLocal > 0 && foulStateLocal < 4)
        {
            isReposition = true;

#if EIJIS_ROTATION || EIJIS_BOWLARDS
            uint foulState = foulStateLocal;
#endif
#if EIJIS_BOWLARDS
            if (isBowlards)
            {
                foulState = 1;
            }
#endif
#if EIJIS_ROTATION
            if (isRotation && (foulStateLocal == 1 || foulStateLocal == 2))
            {
                if (chainedFoulsLocal[teamIdLocal ^ 0x1u] < 3)
                {
                    foulState = 1;
                }
                else
                {
                    foulState = 2;
                }
            }
            switch (foulState)
#else
            switch (foulStateLocal)
#endif
            {
                case 1://kitchen
                    repoMaxX = -(k_TABLE_WIDTH - k_CUSHION_RADIUS) / 2;
                    break;
                case 2://anywhere
#if CHEESE_ISSUE_FIX
                    repoMaxX = k_TABLE_WIDTH - k_BALL_RADIUS;
#else
                    Vector3 k_pR = (Vector3)currentPhysicsManager.GetProgramVariable("k_pR");
                    repoMaxX = k_pR.x;
#endif
                    break;
                case 3://snooker D
                    repoMaxX = K_BAULK_LINE;
                    break;
            }
            setFoulPickupEnabled(true);
        }
    }
#if EIJIS_ROTATION

    private void onRemoteNextBallRepositionStateChanged(uint nextBallRepositionStateSynced)
    {
        if (!gameLive) return;

        if (nextBallRepositionStateLocal == nextBallRepositionStateSynced) return;

#if EIJIS_DEBUG_SEMIAUTO_CALL_AFTER_REPOSITION
        _LogInfo($"TKCH EIJIS_DEBUG_SEMIAUTO_CALL_AFTER_REPOSITION onRemoteNextBallRepositionStateChanged() nextBallRepositionStateLocal = 0x{nextBallRepositionStateLocal:X02}");
#endif
        _LogInfo($"onRemoteNextBallRepositionStateChanged nextBallRepositionState=0x{nextBallRepositionStateSynced:X02}");
        nextBallRepositionStateLocal = nextBallRepositionStateSynced;

        if (semiAutoCallLocal)
        {
            if ((nextBallRepositionStateLocal & 0x1u) == 0 || (nextBallRepositionStateLocal & 0x2u) == 0)
            {
                calledBallId = -2;
                semiAutoCalledTimeBall = 0;
                calledPocketId = -2;
                semiAutoCalledPocket = false;
            }
        }

        if (isRotation)
        {
            bool isOurTurnVar = isMyTurn();
            if (isOurTurnVar)
            {
                menuManager._EnableSkipTurnMenu();
            }
            else
            {
                menuManager._DisableSkipTurnMenu();
            }
        }

        _UpdateNextBallRepositionSpotMarker();
    }
#endif
#if EIJIS_CALLSHOT
    private void onRemoteTurnBegin(int timerStartSynced, bool stateIdChanged)
#else
    private void onRemoteTurnBegin(int timerStartSynced)
#endif
    {
        _LogInfo("onRemoteTurnBegin");

        canPlayLocal = true;
        timerStartLocal = timerStartSynced;
#if EIJIS_CALLSHOT && EIJIS_SEMIAUTOCALL
        // semiAutoCallDelayBase = (semiAutoCallDelayBase < timerStartLocal ? timerStartLocal : semiAutoCallDelayBase);
        semiAutoCallDelayBase = Networking.GetServerTimeInMilliseconds();
#endif

        enablePlayComponents();
        Array.Clear(ballsV, 0, ballsV.Length);
        Array.Clear(ballsW, 0, ballsW.Length);
#if EIJIS_CALLSHOT
#if EIJIS_SEMIAUTOCALL
#if EIJIS_DEBUG_SEMIAUTO_CALL_AFTER_REPOSITION
        _LogInfo($"TKCH EIJIS_DEBUG_SEMIAUTO_CALL_AFTER_REPOSITION onRemoteTurnBegin() cueBallFixed = {cueBallFixed}, cueBallRepositionCount = {cueBallRepositionCount}");
#endif
        // cueBallFixed = !isReposition || (0 < cueBallRepositionCount);
        cueBallFixed = true; // !isReposition || (0 < cueBallRepositionCount);
#if EIJIS_DEBUG_SEMIAUTO_CALL_AFTER_REPOSITION
        _LogInfo($"TKCH EIJIS_DEBUG_SEMIAUTO_CALL_AFTER_REPOSITION onRemoteTurnBegin() cueBallFixed = {cueBallFixed}");
#endif
#endif
        if ((is8Ball || is10Ball || isRotation || isBowlards) && requireCallShotLocal && (!colorTurnLocal || !stateIdChanged))
        {
            graphicsManager._UpdatePointPocketMarker(pointPocketsLocal, callShotLockLocal);
        }
        else
        {
            graphicsManager._DisablePointPocketMarker();
            if (!ReferenceEquals(null, callSafetyOrb)) callSafetyOrb.SetActive(false);
#if EIJIS_CALL_DEFAULT_LOCKING
            if (!ReferenceEquals(null, callClearOrb)) callClearOrb.SetActive(false);
#else
            if (!ReferenceEquals(null, callShotLockOrb)) callShotLockOrb.SetActive(false);
#endif
            if (!ReferenceEquals(null, skipTurnOrb)) skipTurnOrb.SetActive(false);
        }
#endif
    }

    private void onRemoteTurnSimulate(Vector3 cueBallV, Vector3 cueBallW, bool fake = false)
    {
        VRCPlayerApi owner = Networking.GetOwner(networkingManager.gameObject);
        simulationOwnerID = Utilities.IsValid(owner) ? owner.playerId : -1;
        bool isOwner = owner == Networking.LocalPlayer || fake;
        _LogInfo($"onRemoteTurnSimulate cueBallV={cueBallV.ToString("F4")} cueBallW={cueBallW.ToString("F4")} owner={simulationOwnerID}");

        float lerpVolume = cueBallV.magnitude > 6 ? 6 :cueBallV.magnitude;
        if (!fake)
            balls[0].GetComponent<AudioSource>().PlayOneShot(snd_hitball, lerpVolume / 6); //remap
            //balls[0].GetComponent<AudioSource>().PlayOneShot(snd_hitball, 1.0f);

        canPlayLocal = false;
        disablePlayComponents();
#if EIJIS_CALLSHOT && EIJIS_PUSHOUT
        cantSkipNextTurnOnCallShotOption = networkingManager.safetyCalledSynced;
#endif

        bool TableVisible = !localPlayerDistant;
        if (TableVisible)
        {
            for (int i = 0; i < tableMRs.Length; i++)
            {
                if (tableMRs[i].isVisible)
                {
                    TableVisible = true;
                    break;
                }
            }
        }
        if (!_IsPlayer(Networking.LocalPlayer) && !isOwner && (!TableVisible || localPlayerDistant))
        {
            // don't bother simulating if the table isn't even visible
            _LogWarn("skipping simulation");
            return;
        }

#if EIJIS_BOWLARDS
        bool isOnBreakShot = colorTurnLocal;
        if (!isOnBreakShot && 0 < denyBalls)
        {
            currentPhysicsManager.SetProgramVariable("cueBallKichenLineOverCheck", true);
        }

#endif
        isLocalSimulationRunning = true;
        firstHit = 0;
        secondHit = 0;
        thirdHit = 0;
#if EIJIS_CAROM
        cushionBeforeSecondBall = 0;
#endif
        fbMadePoint = false;
        fbMadeFoul = false;
        ballBounced = false;
        numBallsHitCushion = 0;
        ballhasHitCushion = new bool[MAX_BALLS];
        ballsPocketedOrig = ballsPocketedLocal;
#if EIJIS_CALLSHOT
        targetPocketed = 0x0u;
        otherPocketed = 0x0u;
#endif
        jumpShotFoul = false;
        fallOffFoul = false;
        currentPhysicsManager.SendCustomEvent("_ResetSimulationVariables");
        numBallsPocketedThisTurn = 0;

        if (Networking.LocalPlayer.playerId == simulationOwnerID || fake)
        {
            isLocalSimulationOurs = true;
        }

        for (int i = 0; i < ballsV.Length; i++)
        {
            ballsV[i] = Vector3.zero;
            ballsW[i] = Vector3.zero;
        }
        ballsV[0] = cueBallV;
        ballsW[0] = cueBallW;

        auto_colliderBaseVFX.SetActive(true);
    }

#if EIJIS_CALLSHOT
    private void onRemoteTurnStateChanged(byte turnStateSynced, bool stateIdChanged)
#else
    private void onRemoteTurnStateChanged(byte turnStateSynced)
#endif
    {
        if (!gameLive) return;
        // should not escape because it can stay the same turn to turn while whos turn it is changes (especially with Undo/SnookerUndo)
        bool stateChanged = false;
        if (turnStateSynced != turnStateLocal)
        {
            _LogInfo($"onRemoteTurnStateChanged turnStateSynced={turnStateSynced}");
            stateChanged = true;
        }
        turnStateLocal = turnStateSynced;

#if EIJIS_CALLSHOT && EIJIS_PUSHOUT
        if (turnStateLocal == 2 && networkingManager.foulStateSynced == 0 && (networkingManager.nextBallRepositionStateSynced & 0x1u) == 0)
        {
            cantSkipNextTurnOnCallShotOption = true;
        }

#endif
        if (turnStateLocal == 0 || turnStateLocal == 2)
        {
            /* if (turnStateLocal == 2) */
            turnStateLocal = 0; // synthetic state

#if EIJIS_CALLSHOT
            onRemoteTurnBegin(networkingManager.timerStartSynced, stateIdChanged);
#else
            onRemoteTurnBegin(networkingManager.timerStartSynced);
#endif
            // practiceManager._Record();
            auto_colliderBaseVFX.SetActive(false);
        }
        else if (turnStateLocal == 1)
        {
            // prevent simulation from running twice if a serialization was sent during sim
            if (stateChanged || networkingManager.isUrgentSynced == 2)
                onRemoteTurnSimulate(networkingManager.cueBallVSynced, networkingManager.cueBallWSynced);
            // practiceManager._Record();
        }
#if EIJIS_ROTATION
        else if (turnStateLocal == 3) // choice options after foul
        {
            turnStateLocal = 0; // synthetic state
            onRemoteTurnBegin(networkingManager.timerStartSynced, stateIdChanged);
        }
#endif
        else
        {
            canPlayLocal = false;
            disablePlayComponents();
        }
    }
#if EIJIS_CUEBALLSWAP || EIJIS_CALLSHOT

    private void onRemoteCallStateChanged(uint calledBallsSynced, uint pointPocketsSynced, bool callShotLockSynced, bool safetyCalledSynced, bool stateIdChanged)
    {
        if (!gameLive) return;

        if (calledBallsLocal == calledBallsSynced && pointPocketsLocal == pointPocketsSynced && callShotLockLocal == callShotLockSynced && safetyCalledLocal == safetyCalledSynced && 0 < stateIdLocal ) return;

        _LogInfo($"onRemoteCallStateChanged calledBalls={calledBallsSynced:X4}, pointPockets={pointPocketsSynced:X2}, callShotLock={callShotLockSynced}, safetyCalled={safetyCalledSynced}");

        if (calledBallsLocal != calledBallsSynced)
        {
            semiAutoCallDelayBase = Networking.GetServerTimeInMilliseconds();
        }
        
#if EIJIS_CALLSHOT
        if (isPyramid)
        {
            calledBallsLocal = 0; //calledBallsSynced;
            calledBallId = -2;
        }
        else
        {
            calledBallsLocal = calledBallsSynced;
#if EIJIS_SEMIAUTOCALL
            if (semiAutoCallLocal && (0 < cueBallRepositionCount))
            {
                cueBallRepositionCount = 0;
                calledBallId = -2;
                semiAutoCalledTimeBall = 0;
                calledPocketId = -2;
                semiAutoCalledPocket = false;
            }
#endif
        }
#else
        calledBallsLocal = 0; //calledBallsSynced;
        calledBallId = -2;
#endif
        pointPocketsLocal = pointPocketsSynced;
        callShotLockLocal = callShotLockSynced;
        safetyCalledLocal = safetyCalledSynced;

#if EIJIS_CUEBALLSWAP
        if (isPyramid)
        {
            marker9ball.GetComponent<MeshRenderer>().material =
                callShotLockLocal ? calledBallMarkerGray : calledBallMarkerWhite;
        }

#endif
#if EIJIS_10BALL || EIJIS_ROTATION || EIJIS_BOWLARDS
        if ((is8Ball || is10Ball || isRotation || isBowlards) && requireCallShotLocal && (!colorTurnLocal || !stateIdChanged))
#else
        if (is8Ball && requireCallShotLocal && (!colorTurnLocal || !stateIdChanged))
#endif
        {
            if (safetyCalledLocal || calledBallsLocal != 0 || pointPocketsLocal != 0)
            {
                graphicsManager._UpdatePointPocketMarker(pointPocketsLocal, callShotLockLocal);
            }
            else
            {
                graphicsManager._DisablePointPocketMarker();
            }
            if (!safetyCalledLocal && (calledBallsLocal == 0 || pointPocketsLocal == 0))
            {
                guideline.SetActive(false);
            }
            graphicsManager._UpdateCallSafety(safetyCalledLocal);
        }

        if (!stateIdChanged)
        {
            aud_main.PlayOneShot(snd_btn);
        }
        
#if EIJIS_ROTATION
        _UpdateNextBallRepositionSpotMarker();
#endif
    }

#endif
#if EIJIS_PUSHOUT

    private void onRemotePushOutStateChanged(byte pushOutStateSynced, bool stateIdChanged)
    {
        if (!gameLive) return;

        if (pushOutStateLocal == pushOutStateSynced /* && 0 < stateIdLocal */) return;

        _LogInfo($"onRemotePushOutStateChanged pushOutState={pushOutStateSynced}");
        pushOutStateLocal = pushOutStateSynced;
        graphicsManager._UpdatePushOut(pushOutStateLocal);
        if (!stateIdChanged)
        {
#if EIJIS_DEBUG_PUSHOUT
            _LogInfo("  onRemotePushOutStateChanged() !stateIdChanged PlayOneShot");
#endif
            aud_main.PlayOneShot(snd_btn);
        }
    }
#endif
#if EIJIS_ROTATION
    
    private void onRemoteGoalPointChanged(ushort[] goalPointsSynced)
    {
        if (goalPointsSynced[0] == goalPointsLocal[0] && goalPointsSynced[1] == goalPointsLocal[1]) return;

        _LogInfo($"onRemoteGoalPointChanged goalPointsSynced={goalPointsSynced[0]}, {goalPointsSynced[1]}");

        bool refreshGoalMenu = false;
        for (int teamId = 0; teamId < 2; teamId++)
        {
            if (goalPointsLocal[teamId] != goalPointsSynced[teamId])
            {
                goalPointsLocal[teamId] = goalPointsSynced[teamId];
                refreshGoalMenu = true;
            }
        }        
        
        if (refreshGoalMenu)
        {
            menuManager._RefreshGoalPoint();
        }
        
#if EIJIS_EXTERNAL_SCORE_SCREEN
        if (!ReferenceEquals(null, scoreScreen)) if (isRotation) scoreScreen.updateGoalPoint_Rotation(goalPointsLocal[0], goalPointsLocal[1]);
#endif
    }
#endif
#endregion

    #region PhysicsEngineCallbacks
#if EIJIS_CUSHION_EFFECT
    public void _TriggerBounceCushion(int ball, Vector3 pos)
#else
    public void _TriggerBounceCushion(int ball)
#endif
    {
        if (!ballhasHitCushion[ball] && ball != 0)
        {
            numBallsHitCushion++;
            ballhasHitCushion[ball] = true;
        }
        if (firstHit != 0)
        { ballBounced = true; }
#if EIJIS_CAROM
#if EIJIS_DEBUG_CUSHIONTOUCH
        _LogInfo($"EIJIS_DEBUG BilliardsModule::_TriggerCushionAtPosition(ball = {ball}, pos = {pos})");
#endif
        if ((is3Cusion || is2Cusion || is1Cusion /* || is0Cusion */) && ball == 0 && secondHit == 0)
        {
            if (cushionBeforeSecondBall < cushionHitGoal)
            {
                graphicsManager._SpawnCushionTouch(pos, cushionBeforeSecondBall);
            }
            cushionBeforeSecondBall++;
        }
#endif
#if EIJIS_BANKING
        if (isBanking && ball == 0 && firstHit == 0)
        {
#if EIJIS_DEBUG_BANKING
            _LogInfo($"  _TriggerBounceCushion() pos={pos} k_TABLE_WIDTH={k_TABLE_WIDTH}");
            _LogInfo($"  _TriggerBounceCushion() diff={k_TABLE_WIDTH - Math.Abs(pos.x)} k_BALL_RADIUS={k_BALL_RADIUS}");
#endif
            if (k_TABLE_WIDTH - pos.x < k_BALL_RADIUS * 1.1)
            {
                if (cushionBeforeSecondBall < cushionHitGoal)
                {
                    graphicsManager._SpawnCushionTouch(pos, cushionBeforeSecondBall);
                }
                cushionBeforeSecondBall++;
            }
        }
#endif
    }
    public void _TriggerCollision(int srcId, int dstId)
    {
        if (dstId < srcId)
        {
            int tmp = dstId;
            dstId = srcId;
            srcId = tmp;
        }
        if (srcId != 0) return;

        switch (gameModeLocal)
        {
            case 0:
            case 1:
#if EIJIS_PYRAMID
            case GAMEMODE_PYRAMID:
#endif
#if EIJIS_10BALL
            case GAMEMODE_10BALL:
#endif
#if EIJIS_ROTATION
            case GAMEMODE_ROTATION_15:
            case GAMEMODE_ROTATION_10:
            case GAMEMODE_ROTATION_9:
            case GAMEMODE_ROTATION_6:
#endif
                if (firstHit == 0) firstHit = dstId;
                break;
            case 2:
                if (firstHit == 0)
                {
                    firstHit = dstId;
                    break;
                }
                if (secondHit == 0)
                {
                    if (dstId != firstHit)
                    {
                        secondHit = dstId;
                        handle4BallHit(ballsP[dstId], true);
                    }
                    break;
                }
                if (thirdHit == 0)
                {
                    if (dstId != firstHit && dstId != secondHit)
                    {
                        thirdHit = dstId;
                        handle4BallHit(ballsP[dstId], true);
                    }
                    break;
                }
                break;
            case 3:
                if (dstId == 13)
                {
                    handle4BallHit(ballsP[dstId], false);
                    break;
                }
                if (firstHit == 0)
                {
                    firstHit = dstId;
                    break;
                }
                if (secondHit == 0)
                {
                    if (dstId != firstHit)
                    {
                        secondHit = dstId;
                        handle4BallHit(ballsP[dstId], true);
                    }
                    break;
                }
                break;
            case 4:
                //Snooker
                if (firstHit == 0) firstHit = dstId;
                break;
#if EIJIS_BANKING
            case GAMEMODE_BANKING:
#endif
#if EIJIS_CAROM
            case GAMEMODE_3CUSHION:
            case GAMEMODE_2CUSHION:
            case GAMEMODE_1CUSHION:
            case GAMEMODE_0CUSHION:
                if (firstHit == 0)
                {
                    firstHit = dstId;
                    break;
                }
                if (secondHit == 0)
                {
                    if (dstId != firstHit)
                    {
                        secondHit = dstId;
                        if (cushionHitGoal <= cushionBeforeSecondBall)
                        {
                            handle4BallHit(ballsP[dstId], true);
                        }
                    }
                }
                break;
#endif
#if EIJIS_BOWLARDS
            case GAMEMODE_BOWLARDS_10:
            case GAMEMODE_BOWLARDS_5:
            case GAMEMODE_BOWLARDS_1:
                if (firstHit == 0)
                {
                    firstHit = dstId;
                    if (0 < denyBalls)
                    {
                        if ((denyBalls & (0x1u << firstHit)) != 0x0u)
                        {
                            pointPocketsLocal = 0;
                            calledBallsLocal = 0;
                            markerCalledBall.SetActive(false);
                        }
                        else
                        {
                            graphicsManager._SetBallsDenyMark(0);
                        }
                    }
                }
                break;
#endif
        }
    }

    private int numBallsPocketedThisTurn;
#if EIJIS_CALLSHOT
    public void _TriggerPocketBall(int id, int pocketId)
#else
    public void _TriggerPocketBall(int id, bool outOfBounds)
#endif
    {
        uint total = 0U;

        // Get total for X positioning
#if EIJIS_10BALL
#if EIJIS_SNOOKER15REDS
#if EIJIS_ROTATION
#if EIJIS_BOWLARDS
        int count_extent = isRotation6Balls ? 7
            : (is9Ball || isRotation9Balls ? 10 
                : (is10Ball || isRotation10Balls || isBowlards ? 11 
                    : (isSnooker15Red ? 31 : 16)));
#else
        int count_extent = isRotation6Balls ? 7
            : (is9Ball || isRotation9Balls ? 10 
                : (is10Ball || isRotation10Balls ? 11 
                    : (isSnooker15Red ? 31 : 16)));
#endif
#else
        int count_extent = is9Ball ? 10 : (is10Ball ? 11 : (isSnooker15Red ? 31 : 16));
#endif
#else
        int count_extent = is9Ball ? 10 : (is10Ball ? 11 : 16);
#endif
#else
#if EIJIS_SNOOKER15REDS
        int count_extent = is9Ball ? 10 : (isSnooker15Red ? 31 : 16);
#else
        int count_extent = is9Ball ? 10 : 16;
#endif
#endif
#if EIJIS_ROTATION
        for (int i = (isRotation6Balls ? 2 : 1); i < count_extent; i++)
#else
        for (int i = 1; i < count_extent; i++)
#endif
        {
            total += (ballsPocketedLocal >> i) & 0x1U;
        }

        // place ball on the rack
        if (isChinese8Ball)
        {
            int i = 0, j = 0;
            int count = 0;
            bool breaktime = false;
            for (i = 0; i < 5; i++)
            {
                for (j = 5; j > i; j--)
                {
                    if (count == (int)total)
                    {
                        breaktime = true; break;
                    }
                    count++;

                }
                if (breaktime) break;
            }
            float k_BALL_PL_X = -k_BALL_RADIUS; // break placement X
            float k_BALL_PL_Y = Mathf.Sin(60 * Mathf.Deg2Rad) * k_BALL_DIAMETRE; // break placement Y
            //Vector3 result = new Vector3(k_rack_position.x, k_rack_position.y + col_offset * k_BALL_DIAMETRE, k_rack_position.z + row  * k_BALL_DIAMETRE);
            ballsP[id] = new Vector3
                    (
                        (k_rack_position.x - i * k_BALL_PL_Y),
                        5 - i * 5,
                        ((-i + j * 2) * k_BALL_PL_X) + 0.17f
                        );
            //_LogInfo($"i={i},j={j},total={total},pos={balls[id]}");
        }
        else
        {
            ballsP[id] = k_rack_position + (float)total * k_BALL_DIAMETRE * k_rack_direction;
        }
        ballsPocketedLocal ^= 1U << id;

#if EIJIS_CALLSHOT
        bool callSuccess = ((calledBallsLocal & (0x1u << id)) != 0) && ((pointPocketsLocal & (0x1u << pocketId)) != 0);
#endif
        bool foulPocket = false;
#if EIJIS_SNOOKER15REDS
        if (isSnooker) // recreation of the rules in _TriggerSimulationEnded()
#else
        if (isSnooker6Red) // recreation of the rules in _TriggerSimulationEnded()
#endif
        {
#if EIJIS_SNOOKER15REDS
            uint bmask = SNOOKER_REDS_MASK;
#else
            uint bmask = 0x1E50u;// reds
#endif
            int nextcolor = sixRedFindLowestUnpocketedColor(ballsPocketedOrig);
            bool redOnTable = sixRedCheckIfRedOnTable(ballsPocketedOrig, false);
            bool freeBall = foulStateLocal == 5;
            if (colorTurnLocal)
            {
                bmask = 0x1AE; // color balls
                bmask &= 1u << firstHit; // only firsthit is legal
            }
            else if (!redOnTable)
            {
                bmask = 1u << break_order_sixredsnooker[nextcolor];
                if (freeBall)
                {
                    bmask |= 1u << firstHit;
                }
            }
            else
            {
                if (freeBall)
                {
                    bmask |= 1u << firstHit;
                }
            }
            if (((0x1U << id) & bmask) > 0)
            {
                if (colorTurnLocal)
                {
                    if (numBallsPocketedThisTurn > 0)//potting 2 colors is always a foul
                    {
                        foulPocket = true;
                    }
                    numBallsPocketedThisTurn++;
                }
            }
            else
            {
                foulPocket = true;
            }
        }
        else if (is8Ball)
        {
            uint bmask = 0x1FCU << ((int)(teamIdLocal ^ teamColorLocal) * 7);
            if (colorTurnLocal)
                bmask |= 2u; // add black to mask in case of golden break (colorturnlocal = break in 8/9ball)
            if (!(((0x1U << id) & ((bmask) | (isTableOpenLocal ? 0xFFFCU : 0x0000U) | ((bmask & ballsPocketedLocal) == bmask ? 0x2U : 0x0U))) > 0))
            {
                foulPocket = true;
            }
#if EIJIS_CALLSHOT
            if ((!requireCallShotLocal || callSuccess) &&
                (((0x1U << id) & ((bmask) | (isTableOpenLocal ? 0xFFFCU : 0x0000U) | ((bmask & ballsPocketedLocal) == bmask ? 0x2U : 0x0U))) > 0))
            {
                targetPocketed |= 1U << id;
            }
            else
            {
                if (0 != id)
                {
                    otherPocketed |= 1U << id;
                }
            }
#endif
        }
#if EIJIS_10BALL && EIJIS_ROTATION
        else if (is9Ball || is10Ball || isRotation)
#else
        else if (is9Ball)
#endif
        {
            foulPocket = !(findLowestUnpocketedBall(ballsPocketedOrig) == firstHit) || id == 0;
#if EIJIS_CALLSHOT
            if (is9Ball || !requireCallShotLocal || callSuccess)
            {
                targetPocketed |= 1U << id;
                calledBallsLocal = 0;
            }
            else
            {
                if (0 != id)
                {
                    otherPocketed |= 1U << id;
                }
            }
#endif
        }
#if EIJIS_BOWLARDS
        else if (isBowlards)
        {
#if EIJIS_CALLSHOT
            if (!requireCallShotLocal || callSuccess)
            {
                targetPocketed |= 1U << id;
                calledBallsLocal = 0;
            }
            else
            {
                if (0 != id)
                {
                    otherPocketed |= 1U << id;
                }
            }
#endif
        }
#endif
        foulPocket |= fallOffFoul;
        if (foulPocket)
        {
            graphicsManager._FlashTableError();
        }
        else
        {
            graphicsManager._FlashTableLight();
        }
#if EIJIS_CALLSHOT
        if (pocketId < 0)
#else
        if (outOfBounds)
#endif
        { if (snd_OutOfBounds) aud_main.PlayOneShot(snd_OutOfBounds, 1.0f); }
        else
        { if (snd_Sink) aud_main.PlayOneShot(snd_Sink, 1.0f); }

        // VFX ( make ball move )
        Rigidbody body = balls[id].GetComponent<Rigidbody>();
        body.isKinematic = false;
        body.velocity = transform.TransformDirection(ballsV[id]);
        body.angularVelocity = transform.TransformDirection(ballsW[id].normalized) * -ballsW[id].magnitude;

        ballsV[id] = Vector3.zero;
        ballsW[id] = Vector3.zero;
    }

#if EIJIS_BOWLARDS
    public void _TriggerCueBallKichenLineOver()
    {
#if EIJIS_DEBUG_CUEBALL_OVER_KICHENLINE
        _LogInfo("EIJIS BilliardsModule::_TriggerCueBallKichenLineOver()");
#endif
        denyBalls = 0;
        graphicsManager._SetBallsDenyMark(denyBalls);
    }

#endif
    public void _TriggerJumpShotFoul() { jumpShotFoul = true; }
    public void _TriggerBallFallOffFoul() { fallOffFoul = true; }

    public void _TriggerSimulationEnded(bool forceScratch, bool forceRun = false)
    {
        if (!isLocalSimulationRunning && !forceRun) return;

        isLocalSimulationRunning = false;
        waitingForUpdate = !isLocalSimulationOurs;

        if (!isLocalSimulationOurs && networkingManager.delayedDeserialization)
            networkingManager.OnDeserialization();

        cameraManager._OnLocalSimEnd();

        auto_colliderBaseVFX.SetActive(false);

#if EIJIS_PUSHOUT
#if EIJIS_DEBUG_AFTERBREAK
        _LogInfo($"  afterBreak {afterBreak}");
#endif
#if EIJIS_DEBUG_PUSHOUT
        _LogInfo($"  pushOutState {PushOutState[pushOutStateLocal]}({pushOutStateLocal})");
#endif
        bool pushOut = false;
        if (pushOutStateLocal == PUSHOUT_BEFORE_BREAK)
        {
#if EIJIS_DEBUG_PUSHOUT
            _LogInfo($"  set DONT pushOutState {PushOutState[pushOutStateLocal]}({pushOutStateLocal})");
#endif
            pushOutStateLocal = PUSHOUT_DONT;
        }
        else if (pushOutStateLocal == PUSHOUT_DOING)
        {
            pushOut = true;
            pushOutStateLocal = PUSHOUT_REACTIONING;
        }
        else if (pushOutStateLocal == PUSHOUT_DONT || pushOutStateLocal == PUSHOUT_REACTIONING /* || pushOutStateLocal == PUSHOUT_ILLEGAL_REACTIONING */)
        {
#if EIJIS_DEBUG_PUSHOUT
            _LogInfo($"  set ENDED pushOutState {PushOutState[pushOutStateLocal]}({pushOutStateLocal})");
#endif
            pushOutStateLocal = PUSHOUT_ENDED;
        }

#endif
        // Make sure we only run this from the client who initiated the move
        if (isLocalSimulationOurs || forceRun)
        {
            isLocalSimulationOurs = false;

            // Common informations
            bool isScratch = (ballsPocketedLocal & 0x1U) == 0x1U || forceScratch;
            bool nextTurnBlocked = false;

            ballsPocketedLocal = ballsPocketedLocal & ~(0x1U);
            if (isScratch) ballsP[0] = Vector3.zero;
            //keep moving ball down the table until it's not touching any other balls
            moveBallInDirUntilNotTouching(0, Vector3.right * k_BALL_RADIUS * .051f);

            // These are the resultant states we can set for each mode
            // then the rest is taken care of
            bool
                isObjectiveSink,
                isOpponentSink,
                winCondition,
                foulCondition,
                deferLossCondition
            ;
            bool snookerDraw = false;

#if EIJIS_CALLSHOT
            bool isOnBreakShot = colorTurnLocal;
            bool isAnyPocketSink = (ballsPocketedLocal & pocketMask) > (ballsPocketedOrig & pocketMask);
#if EIJIS_10BALL
            uint gameBallMask = is8Ball ? 0x2u : (is9Ball ? 0x200u : (is10Ball ? 0x400u : 0));
            int gameBallId = is8Ball ? 1 : (is9Ball ? 9 : (is10Ball ? 10 : -1));
#endif

#endif
#if EIJIS_ROTATION
            bool repositionOnFoul = true;
            bool winnerBreak = true;
            bool reBreakAllowed = false;
            bool matchWinCondition = false;
#endif
#if EIJIS_BOWLARDS
            bool matchLossCondition = false;
#endif
#if EIJIS_PUSHOUT
            if (pushOut && isScratch)
            {
                pushOutStateLocal = PUSHOUT_ENDED;
            }

#endif
            if (is8Ball)
            {
                // 8ball rules are based on APA, some rules are not yet implemented.

                uint bmask = 0xFFFCu;
                uint emask = 0x0u;

                // Quash down the mask if table has closed
                if (!isTableOpenLocal)
                {
                    bmask = bmask & (0x1FCu << ((int)(teamIdLocal ^ teamColorLocal) * 7));
                    emask = 0x1FCu << ((int)(teamIdLocal ^ teamColorLocal ^ 0x1U) * 7);
                }

                bool isSetComplete = (ballsPocketedOrig & bmask) == bmask;

                // Append black to mask if set is done or it's the break (Golden break rule)
                if (isSetComplete || colorTurnLocal)
                {
                    bmask |= 0x2U;
                }

                isObjectiveSink = (ballsPocketedLocal & bmask) > (ballsPocketedOrig & bmask);
                isOpponentSink = (ballsPocketedLocal & emask) > (ballsPocketedOrig & emask);

                // Calculate if objective was not hit first
                bool isWrongHit = ((0x1U << firstHit) & bmask) == 0;

                bool is8Sink = (ballsPocketedLocal & 0x2U) == 0x2U;

                winCondition = (isSetComplete || colorTurnLocal) && is8Sink;

#if EIJIS_PUSHOUT
                if (is8Sink && ((isPracticeMode && !winCondition) || pushOut))
#else
                if (is8Sink && isPracticeMode && !winCondition)
#endif
                {
                    is8Sink = false;

                    ballsPocketedLocal = ballsPocketedLocal & ~(0x2U);
                    ballsP[1] = Vector3.zero;
                    moveBallInDirUntilNotTouching(1, Vector3.right * k_BALL_RADIUS * .051f);
                }

                foulCondition = isScratch || isWrongHit || fallOffFoul || ((!isObjectiveSink && !isOpponentSink) && (!ballBounced || (colorTurnLocal && numBallsHitCushion < 4)));
#if EIJIS_DEBUG_BREAKINGFOUL
                if ((!isObjectiveSink && !isOpponentSink) && colorTurnLocal && numBallsHitCushion < 4)
                {
                    _LogInfo($"  BREAKING FOUL numBallsHitCushion = {numBallsHitCushion}");
                }
#endif

                if (isScratch && colorTurnLocal)
                {
                    nextTurnBlocked = true; // re-using snooker variable for reposition to kitchen
                    ballsP[0].x = -k_TABLE_WIDTH / 2;
                }

                deferLossCondition = is8Sink;

                if (personalData != null && !isPracticeMode)
                {
                    if (colorTurnLocal && is8Sink)                //黄金开球
                    {
                        personalData.goldenBreak++;
                        if (!foulCondition)
                        {
                            personalData.breakClearance--;
                            personalData.clearance--;
                        }
                    }
                        if (isScratch)
                            personalData.scratchCount++;
                    personalData.SaveData();
                }

                if (is8Sink && isChinese8Ball && colorTurnLocal)//中式开球复位
                {
                    is8Sink = false;
                    winCondition = false;           //赢不了一点
                    deferLossCondition = false;     //也别想输
                    ballsPocketedLocal = ballsPocketedLocal & ~(0x2U);
                    ballsP[1] = initialPositions[1][1]; //初始点
                    moveBallInDirUntilNotTouching(1, Vector3.right * .051f);
                }
                if (is8Sink && colorTurnLocal) BreakFinish = true;
                else BreakFinish = false;

#if EIJIS_CALLSHOT
                // call-shot additional rules
                {
#if EIJIS_DEBUG_CALLSHOT_BALL
                    _LogInfo($"  isObjectiveSink = {isObjectiveSink}, isOpponentSink = {isOpponentSink}");
#endif
                    if (requireCallShotLocal)
                    {
#if EIJIS_DEBUG_CALLSHOT_BALL
                        _LogInfo($"  requireCallShot targetPocketed = {targetPocketed:X4}");
                        _LogInfo($"           masked targetPocketed = {(targetPocketed & bmask):X4}");
                        _LogInfo($"                  otherPocketed = {otherPocketed:X4}, otherPocketed = {otherPocketed:x4}");
                        _LogInfo($"           masked otherPocketed = {(otherPocketed & pocketMask):X4}");
#endif
                        isObjectiveSink = (targetPocketed & bmask) > 0;
                        isOpponentSink = isOpponentSink | (otherPocketed & pocketMask) > 0;
#if EIJIS_DEBUG_CALLSHOT_BALL
                        _LogInfo($"  isObjectiveSink = {isObjectiveSink}, isOpponentSink = {isOpponentSink}");
#endif
                    }

                    if (isObjectiveSink && isOpponentSink)
                    {
                        isOpponentSink = false;
                    }

                    if (is8Sink && (pushOut || (isOnBreakShot && !winCondition && deferLossCondition)))
                    {
                        moveBallInDirUntilNotTouching(9, Vector3.right * .051f);
                        deferLossCondition = false;
                    }

                    if (isOnBreakShot && isAnyPocketSink)
                    {
                        isObjectiveSink = true;
                        isOpponentSink = false;
                    }
#if EIJIS_DEBUG_CALLSHOT_BALL
                    _LogInfo($"  isObjectiveSink = {isObjectiveSink}, isOpponentSink = {isOpponentSink}");
#endif
                }

#endif
                if (personalData != null && foulCondition && colorTurnLocal && !isPracticeMode)
                    personalData.breakFoul++;
                // try and close the table if possible
                if (!foulCondition && isTableOpenLocal)
                {
                    uint sink_orange = 0;
                    uint sink_blue = 0;
#if EIJIS_CALLSHOT
                    uint pmask = (requireCallShotLocal ? targetPocketed : (ballsPocketedLocal ^ ballsPocketedOrig)) >> 2;
#else
                    uint pmask = (ballsPocketedLocal ^ ballsPocketedOrig) >> 2; // only check balls that were pocketed this turn
#endif

                    for (int i = 0; i < 7; i++)
                    {
                        if ((pmask & 0x1u) == 0x1u)
                            sink_blue++;

                        pmask >>= 1;
                    }
                    for (int i = 0; i < 7; i++)
                    {
                        if ((pmask & 0x1u) == 0x1u)
                            sink_orange++;

                        pmask >>= 1;
                    }

                    bool closeTable = false;
                    if (sink_blue > 0 && sink_orange == 0)
                    {
                        teamColorLocal = teamIdLocal;
                        closeTable = true;
                    }
                    else if (sink_orange > 0 && sink_blue == 0)
                    {
                        teamColorLocal = teamIdLocal ^ 0x1u;
                        closeTable = true;
                    }
                    if (isChinese8Ball && colorTurnLocal) closeTable = false; //中式开球不判球

                    if (personalData != null && !isPracticeMode)
                    {
                        if (colorTurnLocal && (foulCondition || (!isObjectiveSink && !isOpponentSink))) personalData.lossOfChange++;
                    }
                    if (closeTable)
                    {
                        networkingManager._OnTableClosed(teamColorLocal);
                    }
                }
                colorTurnLocal = false; // colorTurnLocal tracks if it's the break
            }
#if EIJIS_10BALL
            else if (is9Ball || is10Ball)
#else
            else if (is9Ball)
#endif
            {
                // Rule #1: Cueball must strike the lowest number ball, first
                bool isWrongHit = !(findLowestUnpocketedBall(ballsPocketedOrig) == firstHit);

                // Rule #2: Pocketing cueball, is a foul
#if EIJIS_CALLSHOT
                isObjectiveSink = (targetPocketed & pocketMask) > 0;
#else
                isObjectiveSink = (ballsPocketedLocal & 0x3FEu) > (ballsPocketedOrig & 0x3FEu);
#endif
#if EIJIS_DEBUG_CALLSHOT_BALL
                if (requireCallShotLocal)
                {
                    _LogInfo($"  requireCallShot targetPocketed = {targetPocketed:X4}");
                    _LogInfo($"           masked targetPocketed = {(targetPocketed & pocketMask):X4}");
                    _LogInfo($"                  otherPocketed = {otherPocketed:X4}, otherPocketed = {otherPocketed:x4}");
                    _LogInfo($"           masked otherPocketed = {(otherPocketed & pocketMask):X4}");
                }
#endif

#if EIJIS_CALLSHOT
                isOpponentSink = (otherPocketed & pocketMask) > 0;
                if (isObjectiveSink || isOnBreakShot)
                {
                    isOpponentSink = false;
                }
#else
                isOpponentSink = false;
#endif
#if EIJIS_DEBUG_CALLSHOT_BALL
                _LogInfo($"  isObjectiveSink = {isObjectiveSink}, isOpponentSink = {isOpponentSink}");
#endif
                deferLossCondition = false;

                foulCondition = isWrongHit || isScratch || fallOffFoul || (!isAnyPocketSink && (!ballBounced || (colorTurnLocal && numBallsHitCushion < 4)));
#if EIJIS_DEBUG_BREAKINGFOUL
                if (!isAnyPocketSink && colorTurnLocal && numBallsHitCushion < 4)
                {
                    _LogInfo($"  BREAKING FOUL numBallsHitCushion = {numBallsHitCushion}");
                }
#endif

                colorTurnLocal = false;// colorTurnLocal tracks if it's the break,

#if EIJIS_10BALL

#if EIJIS_DEBUG_10BALL_WPA_RULE
                if (is10Ball)
                {
                    _LogInfo($"  isOnBreakShot = {isOnBreakShot}, wpa10BallRuleLocal(mustPocket10BallLast) = {wpa10BallRuleLocal}, isObjectiveSink = {isObjectiveSink}");
                    _LogInfo($"  (ballsPocketedLocal & pocketMask) = {(ballsPocketedLocal & pocketMask):x8}, pocketMask = {pocketMask:x8}");
                }
#endif
                // Win condition: Pocket game ball ( and do not foul )
#if EIJIS_CALLSHOT
                winCondition = ((targetPocketed & gameBallMask) == gameBallMask) && !foulCondition;
#else
                winCondition = ((ballsPocketedLocal & gameBallMask) == gameBallMask) && !foulCondition;
#endif

                bool isGameBallSink = (ballsPocketedLocal & gameBallMask) == gameBallMask;

                if (is9Ball)
                {
                    winCondition |= isOnBreakShot && isGameBallSink && !foulCondition;
                }
                else if (is10Ball)
                {
                    winCondition &= !(
                        (isOnBreakShot && isGameBallSink) ||
                        (wpa10BallRuleLocal && (ballsPocketedLocal & pocketMask) != pocketMask)
                        );
                }

#if EIJIS_PUSHOUT
                if (isGameBallSink /* && isPracticeMode */ && (!winCondition || pushOut))
#else
                if (isGameBallSink /* && isPracticeMode */ && !winCondition)
#endif
                {
                    ballsPocketedLocal = ballsPocketedLocal & ~(gameBallMask);
                    ballsP[gameBallId] = initialPositions[gameModeLocal][gameBallId];
                    //keep moving ball down the table until it's not touching any other balls
                    moveBallInDirUntilNotTouching(gameBallId, Vector3.right * .051f);
                }
#else
                // Win condition: Pocket 9 ball ( and do not foul )
                winCondition = ((ballsPocketedLocal & 0x200u) == 0x200u) && !foulCondition;

                bool is9Sink = (ballsPocketedLocal & 0x200u) == 0x200u;

                if (is9Sink /* && isPracticeMode */ && !winCondition)
                {
                    is9Sink = false;
                    ballsPocketedLocal = ballsPocketedLocal & ~(0x200u);
                    ballsP[9] = initialPositions[1][9];
                    //keep moving ball down the table until it's not touching any other balls
                    moveBallInDirUntilNotTouching(9, Vector3.right * .051f);
                }
#endif
#if EIJIS_CALLSHOT

                if (isOnBreakShot && isAnyPocketSink)
                {
                    isObjectiveSink = true;
                }
#if EIJIS_DEBUG_CALLSHOT_BALL
                _LogInfo($"  isObjectiveSink = {isObjectiveSink}, isOpponentSink = {isOpponentSink}");
#endif
#endif
            }
#if EIJIS_ROTATION
            else if (isRotation)
            {
                bool isWrongHit = !(findLowestUnpocketedBall(ballsPocketedOrig) == firstHit);
                bool isNoTouch = firstHit <= 0;

                bool isNoCushon = !isAnyPocketSink && !ballBounced;
                
                reBreakAllowed = !isAnyPocketSink && 
                                 isOnBreakShot && (numBallsHitCushion < (isRotation6Balls ? 2 : (isRotation15Balls ? 4 : 3)));
#if EIJIS_DEBUG_BREAKINGFOUL
                if (reBreakAllowed)
                {
                    _LogInfo($"  BREAKING FOUL numBallsHitCushion = {numBallsHitCushion}");
                }
#endif

                foulCondition = isScratch || isWrongHit || isNoTouch || isNoCushon;
                isObjectiveSink = (targetPocketed & pocketMask) > 0;
                isOpponentSink = (otherPocketed & pocketMask) > 0;
                if (isObjectiveSink || isOnBreakShot)
                {
                    isOpponentSink = false;
                }

                if (isScratch)
                {
                    ballsP[0] = new Vector3(-k_TABLE_WIDTH / 2, 0.0f, 0.0f);
                }
                // else if (isNoTouch || isNoCushon)
                // {
                //     noCushionLocal = true;
                // }
                
                deferLossCondition = false;
                
                uint ballsPocketedCurrent = ballsPocketedLocal & ~ballsPocketedOrig;

                int point = 0;
                if (isObjectiveSink || isOnBreakShot)
                {
                    for (int i = (isRotation6Balls ? 2 : 1); i < (isRotation6Balls ? balls.Length + 1 : balls.Length); i++)
                    {
                        if (0 < ((ballsPocketedCurrent >> i) & 0x1u))
                        {
                            int ballNumber = i == 1 ? 8 : (i < 9 ? i - 1 : i);
                            point += ballNumber;
                        }
                    }
                }
                
                if (isOpponentSink || foulCondition)
                {
                    int uponBallsCount = pocketedballUponPool(ballsPocketedCurrent, k_TABLE_WIDTH / 2);
                    if (uponBallsCount <= 0)
                    {
                        pocketedballUponPool(ballsPocketedCurrent, 0);
                    }
                }
                else
                {
                    int totalPoint = totalPointsLocal[teamIdLocal] + point;
                    totalPointsLocal[teamIdLocal] = (ushort)(1023 < totalPoint ? 1023 : totalPoint);
                    int chainedPoint = chainedPointsLocal[teamIdLocal] + point;
                    chainedPointsLocal[teamIdLocal] = (ushort)(1023 < chainedPoint ? 1023 : chainedPoint);

                    if (highRunsLocal[teamIdLocal] < chainedPointsLocal[teamIdLocal])
                    {
                        highRunsLocal[teamIdLocal] = chainedPointsLocal[teamIdLocal];
                    }
                }

                winCondition = (ballsPocketedLocal & pocketMask) == pocketMask;
                if (winCondition)
                {
                    float kitchen_x = -k_SPOT_POSITION_X;
                    winnerBreak = ballsP[0].x < kitchen_x;
                    
                    // winRackCountLocal[teamIdLocal]++;
                }

                if (point == 0)
                {
                    chainedPointsLocal[teamIdLocal] = 0;
                }

                bool cueBallInKitchen = true;
                chainedFoulsLocal[teamIdLocal] = (byte)(foulCondition ? chainedFoulsLocal[teamIdLocal] + 1 : 0);
                if (3 <= chainedFoulsLocal[teamIdLocal])
                {
                    cueBallInKitchen = false;
                    if (isScratch) ballsP[0] = new Vector3(-k_SPOT_RORATION_FREEBALL_X, 0.0f, 0.0f); // Rotationの完全フリーボールの初期位置はセンターからずらす
                }
                if (3 <= chainedFoulsLocal[teamIdLocal ^ 0x1u])
                {
                    chainedFoulsLocal[teamIdLocal ^ 0x1u] = 0;
                }
                
                if (foulCondition && !isScratch && (isNoTouch || isNoCushon || isWrongHit))
                {
                    repositionOnFoul = false;
                }
                
                if (isScratch && cueBallInKitchen)
                {
                    // next ballがkitchen内の場合はセンターに移動させる
                    checkNextInKitchenThenMoveToCenter();
                }

                // if (callPassOptionLocal)
                // {
                //     if (!foulCondition && !isObjectiveSink && 0 < pointPocketsLocal && 0 < calledBallsLocal)
                //     {
                //         if (pushOutStateLocal == PUSHOUT_ENDED)
                //         {
                //             pushOutStateLocal = PUSHOUT_REACTIONING;
                //         }
                //     }
                // }

                if (isOnBreakShot && 0 < point)
                {
                    isObjectiveSink = true;
                }
                
                if (goalPointsLocal[teamIdLocal] <= networkingManager.totalPointsSynced[teamIdLocal])
                {
                    matchWinCondition = true;
                }

                colorTurnLocal = false;// colorTurnLocal tracks if it's the break,
            }
#endif
#if EIJIS_BOWLARDS
            else if (isBowlards)
            {
                // re-use variables
                byte[] frameCounts = fbScoresLocal;
                byte frameCount = frameCounts[teamIdLocal];
                
                int frameNumberBallId = pocketed_ball_upon_pool_order[frameCount];
                bool isWrongHit = !(!isOnBreakShot || (isOnBreakShot && frameNumberBallId == firstHit)); // !(findLowestUnpocketedBall(ballsPocketedOrig) == firstHit);
                isWrongHit |= (denyBalls & (0x1u << firstHit)) != 0x0u;
                bool isNoTouch = firstHit <= 0;
                bool isNoCushon = !isAnyPocketSink && !ballBounced;
#if EIJIS_DEBUG_BOWLARDS
                _LogInfo($"EIJIS_DEBUG  isNoTouch = {isNoTouch}, isNoCushon = {isNoCushon}, isWrongHit = {isWrongHit}, firstHit = {firstHit}, denyBalls = {denyBalls:X4}");
#endif
                
                foulCondition = isScratch || isWrongHit || isNoTouch || isNoCushon;
                isObjectiveSink = (targetPocketed & pocketMask) > 0;
                isOpponentSink = (otherPocketed & pocketMask) > 0;
#if EIJIS_DEBUG_BOWLARDS
                _LogInfo($"             isObjectiveSink = {isObjectiveSink}, isOpponentSink = {isOpponentSink}, isAnyPocketSink = {isAnyPocketSink}, ballBounced = {ballBounced}");
                _LogInfo($"  (ballsPocketedLocal & pocketMask) = {(ballsPocketedLocal & pocketMask):x8}, pocketMask = {pocketMask:x8}");
                _LogInfo($"  targetPocketed = {targetPocketed:x8}, targetPocketedOrig = {targetPocketedOrig:x8}");
                _LogInfo($"  (targetPocketed & pocketMask) = {(targetPocketed & pocketMask):x8}, (targetPocketedOrig & pocketMask) = {(targetPocketedOrig & pocketMask):x8}");
#endif
                if (isObjectiveSink || isOnBreakShot)
                {
                    isOpponentSink = false;
                }

                if (isScratch)
                {
                    ballsP[0] = new Vector3(-k_TABLE_WIDTH / 2, 0.0f, 0.0f);
                }
                
                deferLossCondition = false;
                
                uint ballsPocketedCurrent = ballsPocketedLocal & ~ballsPocketedOrig;
                
                int point = 0;
                if (!foulCondition && (isObjectiveSink || isOnBreakShot))
                {
                    point = (int)SoftwareFallback(ballsPocketedCurrent & pocket_mask_10ball);
                }
                
                if (isOpponentSink || foulCondition)
                {
                    int uponBallsCount = pocketedballUponPool(ballsPocketedCurrent, k_TABLE_WIDTH / 2);
                    if (uponBallsCount <= 0)
                    {
                        pocketedballUponPool(ballsPocketedCurrent, 0);
                    }
                }
                
                winCondition = (ballsPocketedLocal & pocketMask) == pocketMask;

                if (foulCondition && !isScratch && (isNoTouch || isNoCushon || isWrongHit))
                {
                    repositionOnFoul = false;
                }
                
                if (isOnBreakShot && !foulCondition)
                {
                    isObjectiveSink = true;
                }

                {
                    int frameLength = isBowlards1Frame ? 1 : (isBowlards5Frame ? 5 : 10);

                    // re-use variables
                    // uint goalFrame = goalPointsLocal[teamIdLocal];
                    bool lastFrame = ((frameCounts[teamIdLocal] + 1) == frameLength);
                    int throwInningCount = inningCountLocal;

#if EIJIS_DEBUG_BOWLARDS
                    _LogInfo($"EIJIS_DEBUG  frameCount = {frameCount}, lastFrame = {lastFrame}, throwInningCount = {throwInningCount}");
#endif

                    updateBowlardsScore(teamIdLocal, frameCount, throwInningCount, point, lastFrame);
                    
                    bool lastFrameFailed = false;
                    if (lastFrame)
                    {
                        int lastFrameFirstInningPoint = getBowlardsScore(teamIdLocal, frameLength - 1, 0, true);
                        int lastFrameSecondInningPoint = getBowlardsScore(teamIdLocal, frameLength - 1, 1, true);
                        if (0 < throwInningCount && (!isObjectiveSink || foulCondition) &&
                            lastFrameFirstInningPoint < 10 && 
                            (lastFrameFirstInningPoint + lastFrameSecondInningPoint) < 10)
                        {
                            lastFrameFailed = true;
                        }
                    }
                    
                    bool rackClear = winCondition;
                    if ((!isObjectiveSink || foulCondition) || (lastFrame && rackClear))
                    {
                        throwInningCount++;
                        networkingManager.teamIdSynced = (byte)(teamIdLocal | 0x10);
                    }

                    bool inningOver = ((lastFrame ? 3 : 2) <= throwInningCount);
                    if ((lastFrame && (inningOver || lastFrameFailed)) || (!lastFrame && (inningOver || rackClear)))
                    {
                        frameCounts[teamIdLocal]++;
#if EIJIS_DEBUG_BOWLARDS
                        _LogInfo($"EIJIS_DEBUG  FRAME CHANGED frameLength = {frameLength}, frameCount = {frameCount}, fbScoresLocal[{teamIdLocal}] = {fbScoresLocal[teamIdLocal]}");
#endif
                        throwInningCount = 0;
                        
                        if (frameLength <= frameCounts[0] && (frameLength <= frameCounts[1] || (playerIDsLocal[1] == -1 && playerIDsLocal[3] == -1)))
                        {
                            byte[][] framePoints = generateBowlardsScoreByteArrays(frameLength);
                            scoreScreen.updateValues_Bowlards((int)teamIdLocal, new int[] {frameCounts[0] + (10 - frameLength), frameCounts[1] + (10 - frameLength)}, throwInningCount, framePoints);
                            int[] scores = scoreScreen.getScore_Bowlards();
                            if (scores[teamIdLocal] < scores[teamIdLocal ^ 0x1U])
                            {
                                matchLossCondition = true;
                            }
                            else
                            {
                                matchWinCondition = true;
                            }
#if EIJIS_DEBUG_BOWLARDS
                            _LogInfo($"EIJIS_DEBUG  matchWinCondition = {matchWinCondition}, matchLossCondition = {matchLossCondition}, scores = {scores[0]}, {scores[1]}");
#endif
                        }
                        else if (frameCounts[teamIdLocal ^ 0x1U] < frameLength)
                        {
                            winCondition = false;
                            deferLossCondition = true;
                        }
                    }
                    
                    // re-use variables
                    inningCountLocal = throwInningCount;
                }

                isObjectiveSink = true;
                isOpponentSink = false;
                foulCondition = false;

                colorTurnLocal = false;// colorTurnLocal tracks if it's the break,
            }
#endif
#if EIJIS_BANKING
            else if (isBanking)
            {
                isObjectiveSink = false;
                isOpponentSink = false;
                foulCondition = teamIdLocal == 0;
                bool success = (!isScratch && firstHit == 0 && (0 < cushionBeforeSecondBall));
                fbScoresLocal[teamIdLocal] = (byte)(success ? 1 : 0);                    
                winCondition = (teamIdLocal != 0 && success && ((0 == fbScoresLocal[0]) || (k_TABLE_WIDTH + ballsP[0].x < k_TABLE_WIDTH + ballsP[13].x)));
                deferLossCondition = (teamIdLocal != 0 && (0 < fbScoresLocal[0]) && (!success || (k_TABLE_WIDTH + ballsP[0].x > k_TABLE_WIDTH + ballsP[13].x)));

                if (!success && teamIdLocal == 0)
                {
                    ballsP[0] = initialPositions[gameModeLocal][0];
                    ballsP[13] = initialPositions[gameModeLocal][13];
                }

                if (teamIdLocal != 0 && !winCondition && !deferLossCondition)
                {
                    foulCondition = true;
                    ballsP[0] = initialPositions[gameModeLocal][13];
                    ballsP[13] = initialPositions[gameModeLocal][0];
                    fbScoresLocal[0] = 0;
                    fbScoresLocal[1] = 0;
                }
                
                nextTurnBlocked = foulCondition;
            }
#endif
            else if (isCarom)
            {
                isObjectiveSink = fbMadePoint;
                isOpponentSink = fbMadeFoul;
                foulCondition = false;
                deferLossCondition = false;
                if (isScratch)
                    ballsPocketedLocal |= 1u; // make the following function move the cue ball
                fourBallReturnBalls();

                winCondition = fbScoresLocal[teamIdLocal] >= 10;
            }
#if EIJIS_PYRAMID
            else if (isSnooker)
#else
            else /* if (isSnooker6Red) */
#endif
            {
                if (isScratch)
                {
                    ballsP[0] = new Vector3(K_BAULK_LINE - k_SEMICIRCLERADIUS * .5f, 0f, 0f);
                    moveBallInDirUntilNotTouching(0, Vector3.back * k_BALL_RADIUS * .051f);
                }
                isOpponentSink = false;
                deferLossCondition = false;
                foulCondition = false;
                bool freeBall = foulStateLocal == 5;
                if (jumpShotFoul)
                {
                    foulCondition = jumpShotFoul;
                    _LogInfo("6RED: Foul: Jumped over a ball");
                }

                int nextColor = sixRedFindLowestUnpocketedColor(ballsPocketedOrig);
                bool redOnTable = sixRedCheckIfRedOnTable(ballsPocketedOrig, true);
                uint objective = sixRedGetObjective(colorTurnLocal, redOnTable, nextColor, true, true);
                if (isScratch) { _LogInfo("6RED: White ball pocketed"); }
                isObjectiveSink = (ballsPocketedLocal & (objective)) > (ballsPocketedOrig & (objective));
                int ballScore = 0, numBallsPocketed = 0, highestPocketedBallScore = 0;
                int foulFirstHitScore = 0;
                sixRedScoreBallsPocketed(redOnTable, nextColor, ref ballScore, ref numBallsPocketed, ref highestPocketedBallScore);
                if (redOnTable || colorTurnLocal)
                {
                    int pocketedBallTypes = sixRedCheckBallTypesPocketed(ballsPocketedOrig, ballsPocketedLocal);
                    int firsthittype = sixRedCheckFirstHit(firstHit);
                    if (firsthittype == 0)//red or free ball
                    {
                        if (colorTurnLocal)
                        {
                            _LogInfo("6RED: Foul: Color was not first hit on color turn");
                            foulFirstHitScore = 7;
                            foulCondition = true;
                        }
                    }
                    else if (firsthittype == 1)//color
                    {
                        if (!colorTurnLocal)
                        {
                            _LogInfo("6RED: Foul: Red was not hit first on non-color turn");
                            foulFirstHitScore = sixredsnooker_ballpoints[firstHit];
                            foulCondition = true;
                        }
                    }
                    else
                    {
                        _LogInfo("6RED: Foul: No balls hit");
                        foulCondition = true;
                    }
                    if (colorTurnLocal)
                    {
                        if (pocketedBallTypes == 0 || pocketedBallTypes == 2) // red or red and color
                        {
                            _LogInfo("6RED: Foul: Red was pocketed on color turn");
                            foulCondition = true;
                            //pocketing a red on a colorturn is a foul with a penalty of 7
                            highestPocketedBallScore = 7;
                        }
                        if (numBallsPocketed > 1)
                        {
                            _LogInfo("6RED: Foul: Two balls were pocketed on a colorTurn");
                            foulCondition = true;
                        }
                        if (numBallsPocketed == 1 && ((1u << firstHit & ballsPocketedLocal) == 0))
                        {
                            _LogInfo("6RED: Foul: Pocketed color ball was not first hit");
                            foulCondition = true;
                        }
                    }
                    else
                    {
                        if (pocketedBallTypes > 0) // color or red and color
                        {
                            _LogInfo("6RED: Foul: Color was pocketed on non-color turn");
                            foulCondition = true;
                        }
                    }
                }
                else
                {
                    if (firstHit != break_order_sixredsnooker[nextColor] && !freeBall)
                    {
                        _LogInfo("6RED: Foul: Wrong color was first hit");
                        foulFirstHitScore = sixredsnooker_ballpoints[firstHit];
                        foulCondition = true;
                    }
                    //if pocketed a ball that was not the objective, foul
                    if ((ballsPocketedOrig & 0x1AE) < (ballsPocketedLocal & (0x1AE - objective)))//freeball is included in objective
                    {
                        _LogInfo("6RED: Foul: Pocketed incorrect color");
                        foulCondition = true;
                    }
                }
                foulCondition |= isScratch || fallOffFoul;

                //return balls to table before setting allBallsPocketed
                if (redOnTable || colorTurnLocal)
#if EIJIS_SNOOKER15REDS
                { sixRedReturnColoredBalls(SNOOKER_REDS_COUNT); }
#else
                { sixRedReturnColoredBalls(6); }
#endif
                else
                {
                    if (foulCondition)
                    { sixRedReturnColoredBalls(nextColor); }
                    else
                    {
                        // if freeball was pocketed it needs to be returned but nextcolor shouldn't be returned.
                        int returnFrom = nextColor + 1;
#if EIJIS_SNOOKER15REDS
                        if (returnFrom < (break_order_sixredsnooker.Length - 1))
#else
                        if (returnFrom < 11 /* break_order_sixredsnooker.Length - 1 */)
#endif
                        {
                            sixRedReturnColoredBalls(returnFrom);
                        }
                    }
                }
#if EIJIS_SNOOKER15REDS
                bool allBallsPocketed = ((ballsPocketedLocal & SNOOKER_BALLS_MASK) == SNOOKER_BALLS_MASK);
#else
                bool allBallsPocketed = ((ballsPocketedLocal & 0x1FFEu) == 0x1FFEu);
#endif
                //free ball rules
                if (!isScratch && !allBallsPocketed)
                {
                    nextTurnBlocked = SixRedCheckObjBlocked(ballsPocketedLocal, false, false) > 0;
                    if (freeBall && !isObjectiveSink && firstHit != 0 && !foulCondition)
                    {
                        // it's a foul if you use the free ball to block the opponent from hitting object ball
                        // free ball is defined as first ball hit
                        for (int i = 0; i < objVisible_blockingBalls_len; i++)
                        {
                            if (objVisible_blockingBalls[i] == firstHit) // objVisible_blockingBalls is updated inside the above call to SixRedCheckObjBlocked
                            {
                                foulCondition = true;
                                _LogInfo("6RED: Foul: Free ball was used to block");
                                break;
                            }
                        }
                    }
                    if (foulCondition)
                    {
                        if (nextTurnBlocked)
                        {
                            _LogInfo("6RED: Objective blocked with a foul. Next turn is Free Ball.");
                        }
                    }
                }

                if (foulCondition)//points given to other team if foul
                {
                    int foulscore = Mathf.Max(highestPocketedBallScore, foulFirstHitScore);
#if EIJIS_DEBUG_SNOOKER_COLOR_POINT
                    _LogInfo($"  foulscore = {foulscore}, highestPocketedBallScore = {highestPocketedBallScore}, foulFirstHitScore = {foulFirstHitScore}");
#endif
                    if (firstHit == 0)
                    {
#if EIJIS_DEBUG_SNOOKER_COLOR_POINT
                        _LogInfo($"  ballsPocketedLocal = {ballsPocketedLocal:x8}");
#endif
                        int lowestScoringBall = 7;
                        for (int i = 1; i < sixredsnooker_ballpoints.Length; i++)
                        {
                            if ((0x1U << i & ballsPocketedLocal) != 0U) { continue; }
                            lowestScoringBall = Mathf.Min(lowestScoringBall, sixredsnooker_ballpoints[i]);
#if EIJIS_DEBUG_SNOOKER_COLOR_POINT
                            _LogInfo($"  lowestScoringBall = {lowestScoringBall}");
#endif
                        }

                        foulscore = lowestScoringBall;
#if EIJIS_DEBUG_SNOOKER_COLOR_POINT
                        _LogInfo($"  foulscore = {foulscore}, lowestScoringBall = {lowestScoringBall}");
#endif
                    }
                    fbScoresLocal[1 - teamIdLocal] = (byte)Mathf.Min(fbScoresLocal[1 - teamIdLocal] + Mathf.Max(foulscore, 4), byte.MaxValue);
                    _LogInfo("6RED: Team " + (1 - teamIdLocal) + " awarded for foul " + Mathf.Max(foulscore, 4) + " points");
                }
                else
                {
                    fbScoresLocal[teamIdLocal] = (byte)Mathf.Min(fbScoresLocal[teamIdLocal] + ballScore, byte.MaxValue);
                    if(personalData != null)
                    {
                        HeightBreak += ballScore;
                        if (ballScore > 1) personalData.pocketCountSnooker++;
                    }
                    _LogInfo("6RED: Team " + (teamIdLocal) + " awarded " + ballScore + " points");
                }
                _LogInfo("6RED: TeamScore 0: " + fbScoresLocal[0]);
                _LogInfo("6RED: TeamScore 1: " + fbScoresLocal[1]);
                if (redOnTable)
                {
                    if (foulCondition)
                    { colorTurnLocal = false; }
                    else if (isObjectiveSink)
                    { colorTurnLocal = !colorTurnLocal; }
                    else
                    { colorTurnLocal = false; }
                }
                else
                { colorTurnLocal = false; }

                if (fbScoresLocal[teamIdLocal] == fbScoresLocal[1 - teamIdLocal] && allBallsPocketed)
                {
                    // tie rules, cue and black are re-spotted, and a random player gets to go, cue ball in hand, ->onLocalTurnTie()
                    winCondition = false;
                    deferLossCondition = false;
                    foulCondition = false;
#if EIJIS_SNOOKER15REDS
                    sixRedReturnColoredBalls(break_order_sixredsnooker.Length - 1);
#else
                    sixRedReturnColoredBalls(11);
#endif
                    ballsP[0] = new Vector3(K_BAULK_LINE - k_SEMICIRCLERADIUS * .5f, 0f, 0f);
                    snookerDraw = true;
                }
                else
                {
                    // win = all balls pocketed and have more points than opponent
                    bool myTeamWinning = fbScoresLocal[teamIdLocal] > fbScoresLocal[1 - teamIdLocal];
                    winCondition = myTeamWinning && allBallsPocketed;
                    if (winCondition) { foulCondition = false; }
                    deferLossCondition = allBallsPocketed && !myTeamWinning;
                    /*                 _LogInfo("6RED: " + Convert.ToString((ballsPocketedLocal & 0x1FFEu), 2));
                                    _LogInfo("6RED: " + Convert.ToString(0x1FFEu, 2)); */
                }
            }
#if EIJIS_PYRAMID
            else /* if (isPyramid) */
            {
                isObjectiveSink = isScratch || (ballsPocketedLocal & 0xFFFEu) > (ballsPocketedOrig & 0xFFFEu);
                // foulCondition = (firstHit == 0 && !isObjectiveSink) || fallOffFoul;      eijis
                foulCondition = (firstHit == 0 && isScratch) || (!ballBounced && !isObjectiveSink) || fallOffFoul;// hope it works ,as the var from 9ball but seems not
#if EIJIS_DEBUG_PIRAMIDSCORE
                _LogInfo($"  isObjectiveSink = {isObjectiveSink}, foulCondition = {foulCondition}");
#endif
                if (!foulCondition)
                {
                    int ballScore = (int)SoftwareFallback((ballsPocketedLocal & ~ballsPocketedOrig) & 0xFFFEu);
                    ballScore += isScratch ? 1 : 0;
                    fbScoresLocal[teamIdLocal] += (byte)ballScore;
#if EIJIS_DEBUG_PIRAMIDSCORE
                    _LogInfo($"  fbScoresLocal[teamIdLocal = {teamIdLocal}] = {fbScoresLocal[teamIdLocal]}");
#endif
                }
                winCondition = (8 <= fbScoresLocal[teamIdLocal]);
                deferLossCondition = false;
                isOpponentSink = false;
            }
#endif
#if EIJIS_CALLSHOT && EIJIS_PUSHOUT

            if (foulCondition)
            {
                cantSkipNextTurnOnCallShotOption = true;
            }
#endif
#if EIJIS_PUSHOUT

            if (pushOut && !isScratch)
            {
                foulCondition = false;
                winCondition = false;
                deferLossCondition = false;
                isObjectiveSink = false;
            }
#endif
#if EIJIS_CALLSHOT
            calledBallId = -2;
            calledPocketId = -2;
#if EIJIS_SEMIAUTOCALL
            // semiAutoCalledBall = false;
            semiAutoCalledPocket = false;
            semiAutoCalledTimeBall = 0;
#endif
#endif

#if EIJIS_PUSHOUT || EIJIS_CALLSHOT || EIJIS_ROTATION || EIJIS_BOWLARDS
            networkingManager._OnSimulationEnded(ballsP, ballsPocketedLocal
#if EIJIS_BOWLARDS
                // , denyBalls
#endif
                , fbScoresLocal, colorTurnLocal
#if EIJIS_PUSHOUT
                , pushOutStateLocal
#endif
#if EIJIS_ROTATION
                , totalPointsLocal, highRunsLocal, chainedPointsLocal, chainedFoulsLocal, inningCountLocal //, winRackCountLocal
#endif
#if EIJIS_BOWLARDS
                , framePointsLocal, framePointsLocal2
#endif
            );
#else
            networkingManager._OnSimulationEnded(ballsP, ballsPocketedLocal, fbScoresLocal, colorTurnLocal);
#endif
#if EIJIS_ROTATION || EIJIS_BOWLARDS
            
            if (matchWinCondition || matchLossCondition)
            {
                onMatchWin(matchWinCondition ? teamIdLocal : teamIdLocal ^ 0x1U);
                return;
            }
            
#endif
            if (winCondition)
            {
                if (foulCondition)
                {
                    // Loss
#if EIJIS_ROTATION
                    onLocalTeamWin(teamIdLocal ^ 0x1U, winnerBreak);
#else
                    onLocalTeamWin(teamIdLocal ^ 0x1U);
#endif

                    if(personalData!= null && !isPracticeMode)
                    {
                        shotCountData();
                        personalData.foulEnd++;
                    }
                    if (DG_LAB != null)
                    {
                        DG_LAB.SendCustomEvent("JustShock");
                        _LogYes("输了要电");
                    }
                }
                else
                {
                    // Win
#if EIJIS_ROTATION
                    onLocalTeamWin(teamIdLocal, winnerBreak);
#else
                    onLocalTeamWin(teamIdLocal);
#endif
                    if (personalData != null && !isPracticeMode)
                    {
                        shotCountData();
                    }
                }
            }
            else if (deferLossCondition)
            {
                // Loss
#if EIJIS_ROTATION
                onLocalTeamWin(teamIdLocal ^ 0x1U, winnerBreak);
#else
                onLocalTeamWin(teamIdLocal ^ 0x1U);
#endif

                if (personalData != null && !isPracticeMode)
                {
                    shotCountData();
                }
                if (DG_LAB != null)
                {
                    DG_LAB.SendCustomEvent("JustShock");
                    _LogYes("输了要电");
                }

            }
            else if (foulCondition)
            {
                // Foul
#if EIJIS_ROTATION
                onLocalTurnFoul(isScratch, nextTurnBlocked, repositionOnFoul, reBreakAllowed);
#else
                onLocalTurnFoul(isScratch, nextTurnBlocked);
#endif

                if (personalData != null && !isPracticeMode)
                {
                    shotCountData();
                }
                if (DG_LAB != null)
                {
                    DG_LAB.SendCustomEvent("JustShock");
                    _LogYes("犯规了要电");
                }
            }
            else if (snookerDraw)
            {
                // Snooker Draw
                onLocalTurnTie();
            }
            else if (isObjectiveSink && (!isOpponentSink || is8Ball))
            {
                // Continue
#if EIJIS_BOWLARDS
                onLocalTurnContinue(isBowlards && isScratch);
#else
                onLocalTurnContinue();
#endif
            }
            else
            {
                // Pass
#if EIJIS_ROTATION
                onLocalTurnPass(reBreakAllowed, false);
#else
                onLocalTurnPass();
#endif
                if (personalData != null && !isPracticeMode)
                {
                    shotCountData();
                }
            }

            //Save personal datas
            if (personalData != null && !isPracticeMode)
            {
                uint bpl = ballsPocketedLocal & ~(0x1U);
                uint opl = ballsPocketedOrig & ~(0x1U);
                uint number = bpl ^ opl;
                int count = 0;
                while (number != 0)
                {
                    number &= (number - 1);  // 去掉最右边的 1
                    count++;
                }
                //Debug.Log("进球:" + count);
                if (isSnooker)
                {
                    personalData.pocketCountSnooker += count;
                    personalData.inningCountSnooker++;
                }
                else
                {
                    personalData.pocketCount += count;
                    if(!isCarom)
                        personalData.inningCount++;
                }

                if (foulCondition) personalData.foulCount++;

                uint localTeam = 0;
                if (Networking.LocalPlayer.playerId == playerIDsLocal[0])
                    localTeam = 1;
                else if (Networking.LocalPlayer.playerId == playerIDsLocal[1])
                    localTeam = 2;

                if (winCondition && ShotCounts == 1 && localTeam == 1 && !isPyramid) //炸清
                {
                    personalData.breakClearance++; personalData.clearance++;
                    personalData.syncData();
                } 
                if (winCondition && ShotCounts == 1 && localTeam == 2 && !isPyramid)
                {
                    personalData.clearance++;
                    personalData.syncData();
                }
                personalData.SaveData();

            }
        }
#if EIJIS_PUSHOUT || EIJIS_CALLSHOT
        else
        {
#if EIJIS_CALLSHOT
            calledBallId = -2;
            calledPocketId = -2;
#if EIJIS_SEMIAUTOCALL
            // semiAutoCalledBall = false;
            semiAutoCalledPocket = false;
            semiAutoCalledTimeBall = 0;
#endif
#endif
        }
#endif
#if EIJIS_CALLSHOT && EIJIS_SEMIAUTOCALL
        cueBallRepositionCount = 0;
#endif
    }
    
    /// <summary>
    /// calculate personal date(tracking shot count and snooker height break
    /// cheese
    /// </summary>
    private void shotCountData()
    {
        ShotCounts++;
        if (isSnooker && (string)tableModels[tableModelLocal].GetProgramVariable("TABLENAME") == "Snooker 12ft")
        {
            personalData.shotCountSnooker++;
            if(HeightBreak > personalData.heightBreak)
            {
                personalData.heightBreak = HeightBreak;
            }
            HeightBreak = 0;
        }
        else if(!isCarom)
            personalData.shotCount++;

        personalData.SaveData();
    }
    private void sixRedMoveBallUntilNotTouching(int Ball)
    {
        //replace colored ball on its own spot
        ballsP[Ball] = initialPositions[4][Ball];
        //check if it's touching another ball
        int blockingBall = CheckIfBallTouchingBall(Ball);
        if (CheckIfBallTouchingBall(Ball) < 0)
        { return; }
        //if it's touching another ball, place it on other ball spots, starting at black, and moving down
        //the colors until it finds one it can sit without touching another ball
#if EIJIS_SNOOKER15REDS
        for (int i = break_order_sixredsnooker.Length - 1; i >= SNOOKER_REDS_COUNT; i--)
#else
        for (int i = break_order_sixredsnooker.Length - 1; i > 5; i--)
#endif
        {
            ballsP[Ball] = initialPositions[4][break_order_sixredsnooker[i]];
            if (CheckIfBallTouchingBall(Ball) < 0)
            {
                return;
            }
        }
        //if it still can't find a free spot, place at it's original spot and move away from blockage until finding a spot
        ballsP[Ball] = initialPositions[4][Ball];
        Vector3 moveDir = ballsP[Ball] - ballsP[blockingBall];
        moveDir.y = 0;//just to be certain
        if (moveDir.sqrMagnitude == 0)
        { moveDir = -ballsP[Ball]; }
        if (moveDir.sqrMagnitude == 0)
        { moveDir = Vector3.left; }
        moveDir = moveDir.normalized;
        moveBallInDirUntilNotTouching(Ball, moveDir * k_BALL_RADIUS * .051f);
    }
    private void moveBallToNearestFreePointBySpot(int Ball, Vector3 Spot)
    {
        //TODO: Make this function and use it instead of moveBallInDirUntilNotTouching() at the end of sixRedMoveBallUntilNotTouching()
        //TODO: check positions in all directions around spot instead of just moving in one direction 
    }
    private void moveBallInDirUntilNotTouching(int Ball, Vector3 Dir)
    {
        //keep moving ball down the table until it's not touching any other balls
        while (CheckIfBallTouchingBall(Ball) > -1)
        {
            ballsP[Ball] += Dir;
        }
    }
    private int CheckIfBallTouchingBall(int Input)
    {
        float ballDiameter = k_BALL_RADIUS * 2f;
        float k_BALL_DSQR = ballDiameter * ballDiameter;
#if EIJIS_MANY_BALLS
        for (int i = 0; i < MAX_BALLS; i++)
#else
        for (int i = 0; i < 16; i++)
#endif
        {
            if (i == Input) { continue; }
            if (((ballsPocketedLocal >> i) & 0x1u) == 0x1u) { continue; }
            if ((ballsP[Input] - ballsP[i]).sqrMagnitude < k_BALL_DSQR)
            {
                return i;
            }
        }
        return -1;
    }
    private void moveBallInDirUntilNotTouching_Transform(int id, Vector3 Dir)
    {
        //keep moving ball down the table until it's not touching any other balls
        while (CheckIfBallTouchingBall_Transform(id) > -1)
        {
            balls[id].transform.localPosition += Dir;
        }
    }
    private int CheckIfBallTouchingBall_Transform(int id)
    {
        float ballDiameter = k_BALL_RADIUS * 2f;
        float k_BALL_DSQR = ballDiameter * ballDiameter;
#if EIJIS_MANY_BALLS
        for (int i = 0; i < MAX_BALLS; i++)
#else
        for (int i = 0; i < 16; i++)
#endif
        {
            if (i == id) { continue; }
            if (((ballsPocketedLocal >> i) & 0x1u) == 0x1u) { continue; }
            if ((balls[id].transform.position - balls[i].transform.position).sqrMagnitude < k_BALL_DSQR)
            {
                return i;
            }
        }
        return -1;
    }
    #endregion

    #region GameLogic
    private void initializeRack()
    {
#if EIJIS_DEBUG_INITIALIZERACK
        _LogInfo("BilliardsModule::initializeRack()");
#endif
        float k_BALL_PL_X = k_BALL_RADIUS; // break placement X
        float k_BALL_PL_Y = Mathf.Sin(60 * Mathf.Deg2Rad) * k_BALL_DIAMETRE; // break placement Y
        float quarterTable = k_TABLE_WIDTH / 2;
#if EIJIS_PYRAMID || EIJIS_CAROM || EIJIS_10BALL || EIJIS_BANKING || EIJIS_ROTATION
        for (int i = 0; i < initialPositions.Length; i++)
#else
        for (int i = 0; i < 5; i++)
#endif
        {
#if EIJIS_MANY_BALLS
            initialPositions[i] = new Vector3[MAX_BALLS];
            for (int j = 0; j < MAX_BALLS; j++)
#else
            initialPositions[i] = new Vector3[16];
            for (int j = 0; j < 16; j++)
#endif
            {
                initialPositions[i][j] = Vector3.zero;
            }

            // cue ball always starts here (unless four ball, but we override below)
            initialPositions[i][0] = new Vector3(-quarterTable, 0.0f, 0.0f);
        }

        {
            // 8 ball
#if EIJIS_MANY_BALLS
            initialBallsPocketed[0] = 0xFFFF0000u;
#else
            initialBallsPocketed[0] = 0x00u;
#endif

            for (int i = 0, k = 0; i < 5; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    initialPositions[0][break_order_8ball[k++]] = new Vector3
                    (
                       quarterTable + i * k_BALL_PL_Y /*+ UnityEngine.Random.Range(-k_RANDOMIZE_F, k_RANDOMIZE_F)*/,
                       0.0f,
                       (-i + j * 2) * k_BALL_PL_X /*+ UnityEngine.Random.Range(-k_RANDOMIZE_F, k_RANDOMIZE_F)*/
                    );
                }
            }
        }

        {
            // 9 ball
#if EIJIS_MANY_BALLS
            initialBallsPocketed[1] = 0xFFFFFC00u;
#else
            initialBallsPocketed[1] = 0xFC00u;
#endif

            for (int i = 0, k = 0; i < 5; i++)
            {
                int rown = break_rows_9ball[i];
                for (int j = 0; j <= rown; j++)
                {
                    initialPositions[1][break_order_9ball[k++]] = new Vector3
                    (
                       quarterTable - (k_BALL_PL_Y * 2) + i * k_BALL_PL_Y /* + UnityEngine.Random.Range(-k_RANDOMIZE_F, k_RANDOMIZE_F) */,
                       0.0f,
                       (-rown + j * 2) * k_BALL_PL_X /* + UnityEngine.Random.Range(-k_RANDOMIZE_F, k_RANDOMIZE_F) */
                    );
                }
            }
        }

        {
            // Snooker
#if EIJIS_MANY_BALLS
            initialBallsPocketed[4] = 0x81FF0000u;
#else
            initialBallsPocketed[4] = 0xE000u;
#endif
            initialPositions[4][0] = new Vector3(K_BAULK_LINE - k_SEMICIRCLERADIUS * .5f, 0f, 0f);//whte, middle of the semicircle
            initialPositions[4][1] = new Vector3//black
                    (
                       K_BLACK_SPOT,
                       0f,
                       0f
                    );
            initialPositions[4][5] = new Vector3//pink
                    (
                       k_SPOT_POSITION_X,
                       0f,
                       0
                    );
            initialPositions[4][2] = new Vector3//yellow
                    (
                       K_BAULK_LINE,
                       0f,
                       -k_SEMICIRCLERADIUS
                    );
            initialPositions[4][7] = new Vector3//green
                    (
                       K_BAULK_LINE,
                       0f,
                       k_SEMICIRCLERADIUS
                    );
            initialPositions[4][8] = new Vector3//brown
                    (
                       K_BAULK_LINE,
                       0f,
                       0f
                    );
            //triangle
            float rackStartSnooker = k_SPOT_POSITION_X + k_BALL_DIAMETRE + k_BALL_DIAMETRE * .03f;
#if EIJIS_SNOOKER15REDS
            for (int i = 0, k = 0; i < 5; i++)
#else
            for (int i = 0, k = 0; i < 3; i++)// change 3 to 5 for 15 balls (rows)
#endif
            {
                for (int j = 0; j <= i; j++)
                {
                    initialPositions[4][break_order_sixredsnooker[k++]] = new Vector3
                    (
                       rackStartSnooker + i * k_BALL_PL_Y,
                       0.0f,
                       (-i + j * 2) * k_BALL_PL_X
                    );
                }
            }
#if EIJIS_DEBUG_INITIALIZERACK
            // for (int i = 0; i < MAX_BALLS; i++)
            // {
            //     _LogInfo($"  initialPositions[4][i = {i}] x = {initialPositions[4][i].x}, y = {initialPositions[4][i].y}, z = {initialPositions[4][i].z}");
            // }
#endif
        }

        {
            // 4 ball (jp)
#if EIJIS_MANY_BALLS
            initialBallsPocketed[2] = 0xFFFF1FFEu;
#else
            initialBallsPocketed[2] = 0x1FFEu;
#endif
#if EIJIS_BANKING
            initialPositions[2][0] = new Vector3(-quarterTable + (quarterTable * -0.5f), 0.0f, 0.0f);
            initialPositions[2][13] = new Vector3(quarterTable + (quarterTable * 0.5f), 0.0f, 0.0f);
#else
            if (playerIDsLocal[1] == -1 && playerIDsLocal[3] == -1) //lag for break when both player join one side
            {
                initialPositions[2][0] = new Vector3(-quarterTable, 0.0f, k_TABLE_HEIGHT * 0.5f);
                initialPositions[2][13] = new Vector3(-quarterTable, 0.0f, k_TABLE_HEIGHT * -0.5f);
            }
            else
            {
                initialPositions[2][0] = new Vector3(-quarterTable + (quarterTable * -0.5f), 0.0f, 0.0f);
                initialPositions[2][13] = new Vector3(quarterTable + (quarterTable * 0.5f), 0.0f, 0.0f);
            }
#endif
            initialPositions[2][14] = new Vector3(quarterTable, 0.0f, 0.0f);
            initialPositions[2][15] = new Vector3(-quarterTable, 0.0f, 0.0f);
        }

        {
            // 4 ball (kr)
            initialBallsPocketed[3] = initialBallsPocketed[2];
#if EIJIS_CAROM
            Array.Copy(initialPositions[2], initialPositions[3], initialPositions[2].Length);
            initialPositions[3][0] = new Vector3(-quarterTable, 0.0f, -0.178f);
#else
            initialPositions[3] = initialPositions[2];
#endif
        }
#if EIJIS_PYRAMID

        {
            // Russsian Pyramid
#if EIJIS_MANY_BALLS
            initialBallsPocketed[5] = 0xFFFF0000u;
#else
            initialBallsPocketed[5] = 0x00u;
#endif

            for (int i = 0, k = 0; i < 5; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    initialPositions[5][break_order_8ball[k++]] = new Vector3
                    (
                        k_SPOT_POSITION_X + i * k_BALL_PL_Y + UnityEngine.Random.Range(-k_RANDOMIZE_F, k_RANDOMIZE_F),
                        0.0f,
                        (-i + j * 2) * k_BALL_PL_X + UnityEngine.Random.Range(-k_RANDOMIZE_F, k_RANDOMIZE_F)
                    );
                }
            }
        }
#endif
#if EIJIS_CAROM

        {
            // 3-Cushion
#if EIJIS_MANY_BALLS
            initialBallsPocketed[6] = 0xFFFF9FFEu;
#else
            initialBallsPocketed[6] = 0x9FFEu;
#endif
            initialPositions[6][0] = new Vector3(-quarterTable, 0.0f, -0.178f);
            initialPositions[6][13] = new Vector3(-quarterTable, 0.0f, 0.0f);
            initialPositions[6][14] = new Vector3(quarterTable, 0.0f, 0.0f);
        }

        for (int i = 7; i <= 9; i++)
        {
            // 0 ～ 2-Cushion
            initialBallsPocketed[i] = initialBallsPocketed[6];
            initialPositions[i] = initialPositions[6];
        }
#endif
#if EIJIS_10BALL
        
        {
            // 10 ball
#if EIJIS_MANY_BALLS
            initialBallsPocketed[GAMEMODE_10BALL] = 0xFFFFF800u;
#else
            initialBallsPocketed[GAMEMODE_10BALL] = 0xF800u;
#endif

            for (int i = 0, k = 0; i < 4; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    initialPositions[GAMEMODE_10BALL][break_order_10ball[k++]] = new Vector3
                    (
                        quarterTable + i * k_BALL_PL_Y /* + UnityEngine.Random.Range(-k_RANDOMIZE_F, k_RANDOMIZE_F) */,
                        // quarterTable - (k_BALL_PL_Y * 2) + i * k_BALL_PL_Y /* + UnityEngine.Random.Range(-k_RANDOMIZE_F, k_RANDOMIZE_F) */,
                        0.0f,
                        (-i + j * 2) * k_BALL_PL_X /* + UnityEngine.Random.Range(-k_RANDOMIZE_F, k_RANDOMIZE_F) */
                    );
                }
            }
        }
#endif
#if EIJIS_BANKING
        
        {
            // Banking
#if EIJIS_MANY_BALLS
            initialBallsPocketed[11] = 0xFFFFDFFEu;
#else
            initialBallsPocketed[11] = 0xDFFEu;
#endif
            initialPositions[11][0] = new Vector3(-quarterTable, 0.0f, k_TABLE_HEIGHT * 0.5f);
            initialPositions[11][13] = new Vector3(-quarterTable, 0.0f, k_TABLE_HEIGHT * -0.5f);
        }
#endif
#if EIJIS_ROTATION
        
        {
            // rotation
            for (uint gameMode = GAMEMODE_ROTATION_15; gameMode <= GAMEMODE_ROTATION_6; gameMode++)
            {
#if EIJIS_MANY_BALLS
                initialBallsPocketed[gameMode] = 0xFFFF0000u | rotation_initial_pocketed[gameMode & GAME_MODE_ROTATION_MASK];
#else
                initialBallsPocketed[gameMode] = rotation_initial_pocketed[gameMode & GAME_MODE_ROTATION_MASK];
#endif
            }
            
            initializeRack_Rotation();
        }
#endif
#if EIJIS_BOWLARDS

        {
            // bowlards
#if EIJIS_MANY_BALLS
            initialBallsPocketed[GAMEMODE_BOWLARDS_10] = 0xFFFFF800u;
            initialBallsPocketed[GAMEMODE_BOWLARDS_5] = 0xFFFFF800u;
            initialBallsPocketed[GAMEMODE_BOWLARDS_1] = 0xFFFFF800u;
#else
            initialBallsPocketed[GAMEMODE_BOWLARDS_10] = 0xF800u;
            initialBallsPocketed[GAMEMODE_BOWLARDS_5] = 0xF800u;
            initialBallsPocketed[GAMEMODE_BOWLARDS_1] = 0xF800u;
#endif
            initializeRack_Bowlards(-1);
        }
#endif
    }
#if EIJIS_ROTATION
    private void initializeRack_Rotation()
    {
#if EIJIS_DEBUG_INITIALIZERACK
        _LogInfo("BilliardsModule::initializeRack_Rotation()");
#endif
        float k_BALL_PL_X = k_BALL_RADIUS; // break placement X
        float k_BALL_PL_Y = Mathf.Sin(60 * Mathf.Deg2Rad) * k_BALL_DIAMETRE; // break placement Y
        float quarterTable = k_TABLE_WIDTH / 2;
        
        {
            // rotation

            int rackConditionLocal = 1;
            
            // 15,10,6balls
            for (int i = 0, k = 0; i < 5; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    Vector3 pos = new Vector3
                    (
                        quarterTable + i * k_BALL_PL_Y + (rackConditionLocal == 0 ? 0 : UnityEngine.Random.Range(-k_RANDOMIZE_F, k_RANDOMIZE_F)),
                        0.0f,
                        (-i + j * 2) * k_BALL_PL_X + (rackConditionLocal == 0 ? 0 : UnityEngine.Random.Range(-k_RANDOMIZE_F, k_RANDOMIZE_F))
                    );
                    if (k < break_order_rotation_15ball.Length) initialPositions[GAMEMODE_ROTATION_15][break_order_rotation_15ball[k]] = pos;
                    if (k < break_order_rotation_10ball.Length) initialPositions[GAMEMODE_ROTATION_10][break_order_rotation_10ball[k]] = pos;
                    if (k < break_order_rotation_6ball.Length) initialPositions[GAMEMODE_ROTATION_6][break_order_rotation_6ball[k]] = pos;
#if EIJIS_DEBUG_INITIALIZERACK
                    _LogInfo($"  initialPositions[GAMEMODE_ROTATION_15][k = {k}] x = {initialPositions[GAMEMODE_ROTATION_15][k].x}, z = {initialPositions[GAMEMODE_ROTATION_15][k].z}");
                    _LogInfo($"  initialPositions[GAMEMODE_ROTATION_10][k = {k}] x = {initialPositions[GAMEMODE_ROTATION_10][k].x}, z = {initialPositions[GAMEMODE_ROTATION_10][k].z}");
                    _LogInfo($"  initialPositions[GAMEMODE_ROTATION_9][k = {k}] x = {initialPositions[GAMEMODE_ROTATION_9][k].x}, z = {initialPositions[GAMEMODE_ROTATION_9][k].z}");
                    // _LogInfo($"  initialPositions[GAMEMODE_ROTATION_6][k = {k}] x = {initialPositions[GAMEMODE_ROTATION_6][k].x}, z = {initialPositions[GAMEMODE_ROTATION_6][k].z}");
#endif
                    k++;
                }
            }
            
            // 9balls
            for (int i = 0, k = 0; i < 5; i++)
            {
                int rown = break_rows_9ball[i];
                for (int j = 0; j <= rown; j++)
                {
                    initialPositions[GAMEMODE_ROTATION_9][break_order_rotation_9ball[k++]] = new Vector3
                    (
                        quarterTable + i * k_BALL_PL_Y + (rackConditionLocal == 0 ? 0 : UnityEngine.Random.Range(-k_RANDOMIZE_F, k_RANDOMIZE_F)),
                        0.0f,
                        (-rown + j * 2) * k_BALL_PL_X + (rackConditionLocal == 0 ? 0 : UnityEngine.Random.Range(-k_RANDOMIZE_F, k_RANDOMIZE_F))
                    );
                }
            }
        }
#if EIJIS_DEBUG_INITIALIZERACK
        _LogInfo($"  initialPositions[GAMEMODE_ROTATION_15][0] x = {initialPositions[GAMEMODE_ROTATION_15][0].x}, z = {initialPositions[GAMEMODE_ROTATION_15][0].z}");
        _LogInfo($"  initialPositions[GAMEMODE_ROTATION_10][0] x = {initialPositions[GAMEMODE_ROTATION_10][0].x}, z = {initialPositions[GAMEMODE_ROTATION_10][0].z}");
        _LogInfo($"  initialPositions[GAMEMODE_ROTATION_9][0] x = {initialPositions[GAMEMODE_ROTATION_9][0].x}, z = {initialPositions[GAMEMODE_ROTATION_9][0].z}");
        _LogInfo($"  initialPositions[GAMEMODE_ROTATION_6][0] x = {initialPositions[GAMEMODE_ROTATION_6][0].x}, z = {initialPositions[GAMEMODE_ROTATION_6][0].z}");
#endif
    }    
    
#endif
#if EIJIS_BOWLARDS
    private void initializeRack_Bowlards(int topBallId)
    {
        float k_BALL_PL_X = k_BALL_RADIUS; // break placement X
        float k_BALL_PL_Y = Mathf.Sin(60 * Mathf.Deg2Rad) * k_BALL_DIAMETRE; // break placement Y
        float quarterTable = k_TABLE_WIDTH / 2;
        
        {
            // bowlards

            int topBallTempId = -1;
            Vector3 topBallTempPos = Vector3.zero;
            
            for (int i = 0, k = 0; i < 4; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    initialPositions[GAMEMODE_BOWLARDS_10][pocketed_ball_upon_pool_order[k]] = new Vector3
                    (
                        quarterTable + i * k_BALL_PL_Y + UnityEngine.Random.Range(-k_RANDOMIZE_F, k_RANDOMIZE_F),
                        0.0f,
                        (-i + j * 2) * k_BALL_PL_X + UnityEngine.Random.Range(-k_RANDOMIZE_F, k_RANDOMIZE_F)
                    );

                    if (k == 0)
                    {
                        topBallTempId = pocketed_ball_upon_pool_order[k];
                        topBallTempPos = initialPositions[GAMEMODE_BOWLARDS_10][topBallTempId];
                    }

                    k++;
                }
            }
            
            if (0 < topBallId && 0 < topBallTempId && topBallId != topBallTempId)
            {
                Vector3 temp = initialPositions[GAMEMODE_BOWLARDS_10][topBallId];
                initialPositions[GAMEMODE_BOWLARDS_10][topBallId] = topBallTempPos;
                initialPositions[GAMEMODE_BOWLARDS_10][topBallTempId] = temp;
            }
            
            initialBallsPocketed[GAMEMODE_BOWLARDS_5] = initialBallsPocketed[GAMEMODE_BOWLARDS_10];
            initialPositions[GAMEMODE_BOWLARDS_5] = initialPositions[GAMEMODE_BOWLARDS_10];
            
            initialBallsPocketed[GAMEMODE_BOWLARDS_1] = initialBallsPocketed[GAMEMODE_BOWLARDS_10];
            initialPositions[GAMEMODE_BOWLARDS_1] = initialPositions[GAMEMODE_BOWLARDS_10];
        }
    }

    private Vector3[] randomizeBallPositions_Bowlards(int topBallId)
    {
        if (!RandomizeBallPositions) return initialPositions[gameModeLocal];

#if EIJIS_MANY_BALLS
        Vector3[] randomPositions = new Vector3[MAX_BALLS];
        Array.Copy(initialPositions[gameModeLocal], randomPositions, MAX_BALLS);
#else
        Vector3[] randomPositions = new Vector3[16];
        Array.Copy(initialPositions[gameModeLocal], randomPositions, 16);
#endif
        
#if EIJIS_DEBUG_BOWLARDS
        // _LogInfo($"EIJIS_DEBUG  topBallId = {topBallId}");
#endif
        for (int i = 1; i < 11; i++)
        {
            // don't move the tor ball
            if (i == topBallId) continue;
            Vector3 temp = randomPositions[i];
            int rand = UnityEngine.Random.Range(1, 11);
            while (rand == topBallId)
                rand = UnityEngine.Random.Range(1, 11);
                        
#if EIJIS_DEBUG_BOWLARDS
            // _LogInfo($"EIJIS_DEBUG  i = {i}, rand = {rand}");
#endif
            randomPositions[i] = randomPositions[rand];
            randomPositions[rand] = temp;
        }

        return randomPositions;
    }   
    
#endif
    private void resetCachedData()
    {
        for (int i = 0; i < 4; i++)
        {
            playerIDsLocal[i] = -1;
        }
        foulStateLocal = 0;
        gameModeLocal = int.MaxValue;
        turnStateLocal = byte.MaxValue;
    }

    public void setTransform(Transform src, Transform dest, bool doScale = false, float sf = 1f)
    {
        dest.position = src.position;
        dest.rotation = src.rotation;
        if (!doScale) return;
        dest.localScale = src.localScale * sf;
    }

    private void setTableModel(int newTableModel)
    {
        tableModels[tableModelLocal].gameObject.SetActive(false);
        tableModels[newTableModel].gameObject.SetActive(true);

        tableModelLocal = newTableModel;

        isChinese8Ball = (string)tableModels[tableModelLocal].GetProgramVariable("TABLENAME") == "China 9ft";

        ModelData data = tableModels[tableModelLocal];
        k_TABLE_WIDTH = data.tableWidth * .5f;
        k_TABLE_HEIGHT = data.tableHeight * .5f;
        k_CUSHION_RADIUS = data.cushionRadius;
        k_POCKET_WIDTH_CORNER = data.pocketWidthCorner;
        k_POCKET_HEIGHT_CORNER = data.pocketHeightCorner;
        k_POCKET_RADIUS_SIDE = data.pocketRadiusSide;
        k_POCKET_DEPTH_SIDE = data.pocketDepthSide;
        k_INNER_RADIUS_CORNER = data.pocketInnerRadiusCorner;
        k_INNER_RADIUS_SIDE = data.pocketInnerRadiusSide;
        k_FACING_ANGLE_CORNER = data.facingAngleCorner;
        k_FACING_ANGLE_SIDE = data.facingAngleSide;
        K_BAULK_LINE = -(k_TABLE_WIDTH - data.baulkLine);
        K_BLACK_SPOT = k_TABLE_WIDTH - data.blackSpot;
        k_SEMICIRCLERADIUS = data.semiCircleRadius;
        k_BALL_DIAMETRE = data.bs_BallDiameter / 1000f;
        k_BALL_RADIUS = k_BALL_DIAMETRE * .5f;
        k_BALL_MASS = data.bs_BallMass;
        k_RAIL_HEIGHT_UPPER = data.railHeightUpper;
        k_RAIL_HEIGHT_LOWER = data.railHeightLower;
        k_RAIL_DEPTH_WIDTH = data.railDepthWidth;
        k_RAIL_DEPTH_HEIGHT = data.railDepthHeight;
        k_SPOT_POSITION_X = k_TABLE_WIDTH - data.pinkSpot;
        k_POCKET_RESTITUTION = data.bt_PocketRestitutionFactor;
        k_vE = data.cornerPocket;
        k_vF = data.sidePocket;
#if EIJIS_CALLSHOT
        pocketLocations[0] = k_vE;
        pocketLocations[1] = new Vector3(k_vE.x, k_vE.y, -k_vE.z);
        pocketLocations[2] = new Vector3(-k_vE.x, k_vE.y, k_vE.z);
        pocketLocations[3] = new Vector3(-k_vE.x, k_vE.y, -k_vE.z);
        pocketLocations[4] = k_vF;
        pocketLocations[5] = new Vector3(k_vF.x, k_vF.y, -k_vF.z);
#if EIJIS_SEMIAUTOCALL
        findNearestPocket_x = k_vE.x / 2;
        findNearestPocket_n = findNearestPocket_x / k_vE.z;
#endif
#endif

        //advanced physics
        useRailLower = data.useRailHeightLower;
        k_F_SLIDE = data.bt_CoefSlide;
        k_F_ROLL = data.bt_CoefRoll;
        k_F_SPIN = data.bt_CoefSpin;
        k_F_SPIN_RATE = data.bt_CoefSpinRate;
        isDRate = data.bt_ConstDecelRate;
        K_BOUNCE_FACTOR = data.bt_BounceFactor;
        isHanModel = data.bc_UseHan05;
        k_E_C = data.bc_CoefRestitution;
        isDynamicRestitution = data.bc_DynRestitution;
        isCushionFrictionConstant = data.bc_UseConstFriction;
        k_Cushion_MU = data.bc_ConstFriction;
        k_BALL_E = data.bs_CoefRestitution;
        muFactor = data.bs_Friction;

        tableMRs = tableModels[newTableModel].GetComponentsInChildren<MeshRenderer>();

        float newscale = k_BALL_DIAMETRE / ballMeshDiameter;
        Vector3 newBallSize = Vector3.one * newscale;
        for (int i = 0; i < balls.Length; i++)
        {
            balls[i].transform.localScale = newBallSize;
        }
        float table_base = _GetTableBase().transform.Find(".TABLE_SURFACE").localPosition.y;
        tableSurface.localPosition = new Vector3(0, table_base + k_BALL_RADIUS, 0);
#if EIJIS_CALLSHOT
        for (int i = 0; i < pointPocketMarkers.Length; i++)
        {
            pointPocketMarkers[i].transform.localPosition = new Vector3(pocketLocations[i].x, - (k_BALL_RADIUS * 2), pocketLocations[i].z);
        }
#endif

        SetTableTransforms();

        k_rack_position = tableSurface.InverseTransformPoint(auto_rackPosition.transform.position);
        k_rack_direction = tableSurface.InverseTransformDirection(auto_rackPosition.transform.up);

        currentPhysicsManager.SendCustomEvent("_InitConstants");
        graphicsManager._InitializeTable();

        cueControllers[0]._RefreshTable();
        cueControllers[1]._RefreshTable();

        desktopManager._RefreshTable();

        //set height of guideline
        Transform guideDisplay = guideline.gameObject.transform.Find("guide_display");
        Vector3 newpos = guideDisplay.localPosition; newpos.y = 0;
        newpos += Vector3.down * (k_BALL_RADIUS - 0.003f) / guideline.transform.localScale.y;// divide to convert back to worldspace distance
        guideDisplay.localPosition = newpos;
        guideDisplay.GetComponent<MeshRenderer>().material.SetVector("_Dims", new Vector4(k_vE.x, k_vE.z, 0, 0));
        Transform guideDisplay2 = guideline2.gameObject.transform.Find("guide_display");
        guideDisplay2.localPosition = newpos;
        guideDisplay2.GetComponent<MeshRenderer>().material.SetVector("_Dims", new Vector4(k_vE.x, k_vE.z, 0, 0));
        guideDisplay2.GetComponent<MeshRenderer>().material.SetVector("_Dims", new Vector4(k_vE.x, k_vE.z, 0, 0));

        //set height of 9ball marker
        newpos = marker9ball.transform.localPosition; newpos.y = 0;
        newpos += Vector3.down * -0.003f / marker9ball.transform.localScale.y;
        marker9ball.transform.localPosition = newpos;
#if EIJIS_CALLSHOT
        newpos = markerCalledBall.transform.localPosition; newpos.y = 0;
        newpos += Vector3.down * 0.003f / markerCalledBall.transform.localScale.y;
        markerCalledBall.transform.localPosition = newpos;
#endif

#if EIJIS_ROTATION
        float quarterTable = k_TABLE_WIDTH / 2;
        k_SPOT_RORATION_FREEBALL_X = quarterTable / 2; // Rotationの完全フリーボールの初期位置はセンターからずらす
        if (!ReferenceEquals(null, markerHeadSpot))
        {
            newpos = markerHeadSpot.transform.localPosition; newpos.x = -quarterTable;
            markerHeadSpot.transform.localPosition = newpos;
        }
        if (!ReferenceEquals(null, markerFootSpot))
        {
            newpos = markerFootSpot.transform.localPosition; newpos.x = quarterTable;
            markerFootSpot.transform.localPosition = newpos;
        }
        
#endif
        initializeRack();
        ConfineBallTransformsToTable();

        menuManager._RefreshTable();
    }

    private void SetTableTransforms()
    {
        Transform table_base = _GetTableBase().transform;
        auto_pocketblockers = table_base.Find(".4BALL_FILL").gameObject;
        auto_rackPosition = table_base.Find(".RACK").gameObject;
        auto_colliderBaseVFX = table_base.Find("collision.vfx").gameObject;

        Transform NAME_0_SPOT = table_base.Find(".NAME_0");
        Transform MENU_SPOT = table_base.Find(".MENU");

        Transform score_info_root = this.transform.Find("intl.scorecardinfo");
        Transform player0name = score_info_root.Find("player0-name");
        if (NAME_0_SPOT && player0name)
            setTransform(NAME_0_SPOT, player0name);

        Transform NAME_1_SPOT = table_base.Find(".NAME_1");
        Transform player1name = score_info_root.Find("player1-name");
        if (NAME_1_SPOT && player1name)
            setTransform(NAME_1_SPOT, player1name);

        Transform SCORE_0_SPOT = table_base.Find(".SCORE_0");
        Transform player0score = score_info_root.Find("player0-score");
        if (SCORE_0_SPOT && player0score)
            setTransform(SCORE_0_SPOT, player0score);

        Transform SCORE_1_SPOT = table_base.Find(".SCORE_1");
        Transform player1score = score_info_root.Find("player1-score");
        if (SCORE_1_SPOT && player1score)
            setTransform(SCORE_1_SPOT, player1score);

        Transform SNOOKER_INSTRUCTIONS_SPOT = table_base.Find(".SNOOKER_INSTRUCTIONS");
        Transform SnookerInstructions = score_info_root.Find("SnookerInstructions");
        if (SNOOKER_INSTRUCTIONS_SPOT && SnookerInstructions)
            setTransform(SNOOKER_INSTRUCTIONS_SPOT, SnookerInstructions);

        Transform menu = this.transform.Find("intl.menu/MenuAnchor");
        if (MENU_SPOT && menu)
            setTransform(MENU_SPOT, menu);
    }

    private void ConfineBallTransformsToTable()
    {
        for (int i = 0; i < balls.Length; i++)
        {
            balls[i].transform.localPosition = ballsP[i];
            Vector3 thisBallPos = balls[i].transform.localPosition;

            float r_k_CUSHION_RADIUS = k_CUSHION_RADIUS + k_BALL_RADIUS;
            if (thisBallPos.x > k_TABLE_WIDTH - r_k_CUSHION_RADIUS)
            {
                thisBallPos.x = k_TABLE_WIDTH - r_k_CUSHION_RADIUS;
            }
            else if (thisBallPos.x < -k_TABLE_WIDTH + r_k_CUSHION_RADIUS)
            {
                thisBallPos.x = -k_TABLE_WIDTH + r_k_CUSHION_RADIUS;
            }
            if (thisBallPos.z > k_TABLE_HEIGHT - r_k_CUSHION_RADIUS)
            {
                thisBallPos.z = k_TABLE_HEIGHT - r_k_CUSHION_RADIUS;
            }
            else if (thisBallPos.z < -k_TABLE_HEIGHT + r_k_CUSHION_RADIUS)
            {
                thisBallPos.z = -k_TABLE_HEIGHT + r_k_CUSHION_RADIUS;
            }
            balls[i].transform.localPosition = thisBallPos;
            Vector3 moveDir = -thisBallPos.normalized;
            if (moveDir == Vector3.zero) { moveDir = Vector3.right; }
            moveBallInDirUntilNotTouching_Transform(i, moveDir * k_BALL_RADIUS);
        }
    }

    public GameObject _GetTableBase()
    {
        return tableModels[tableModelLocal].transform.Find("table_artwork").gameObject;
    }

    private void handle4BallHit(Vector3 loc, bool good)
    {
#if EIJIS_ISSUE_FIX // 四つ球で的玉が場外してもポイントできる | player can point even if the target ball is fall out of the field in a 4ball. https://github.com/Sacchan-VRC/MS-VRCSA-Billiards/pull/9/commits/5fb055b98df3660f3f2dde2e8f8eb245d4f1cbac
        if (fallOffFoul) return;
        
#endif
        if (good)
        {
            handle4BallHitGood(loc);
        }
        else
        {
            handle4BallHitBad(loc);
        }

        graphicsManager._SpawnFourBallPoint(loc, good);
        graphicsManager._UpdateScorecard();
    }

    private void handle4BallHitGood(Vector3 p)
    {
        fbMadePoint = true;
        aud_main.PlayOneShot(snd_PointMade, 1.0f);

        if (fbScoresLocal[teamIdLocal] < 10)
            fbScoresLocal[teamIdLocal]++;
    }

    private void handle4BallHitBad(Vector3 p)
    {
        if (fbMadeFoul) return;
        fbMadeFoul = true;

        if (fbScoresLocal[teamIdLocal] > 0)
            fbScoresLocal[teamIdLocal]--;
    }

#if EIJIS_ROTATION
    private void onMatchWin(uint winner)
    {
        _LogInfo($"onMatchWin {(winner)}");

        networkingManager._OnGameWin(winner);
    }

#endif
#if EIJIS_ROTATION
    private void onLocalTeamWin(uint winner, bool winnerBreak)
#else
    private void onLocalTeamWin(uint winner)
#endif
    {
        Debug.Log("onLocalTeamWin");

        _LogInfo($"onLocalTeamWin {(winner)}");

#if EIJIS_BOWLARDS
        if (isBowlards)
        {
            uint nextTeamId = (playerIDsLocal[1] == -1 && playerIDsLocal[3] == -1)? localTeamId : winner;
            int frameCount = fbScoresLocal[nextTeamId];
            int frameNumberBallId = pocketed_ball_upon_pool_order[frameCount];
            initializeRack_Bowlards(frameNumberBallId);
            Vector3[] randomPositions = randomizeBallPositions_Bowlards(frameNumberBallId);
            if (nextTeamId == localTeamId) nextTeamId |= 0x10;
            networkingManager._OnGameNextBreak(
                initialBallsPocketed[gameModeLocal], randomPositions, 
                nextTeamId, 1, false);
            return;
        }
#endif
#if EIJIS_ROTATION
        if (isRotation)
        {
            if (teamIdLocal != 0 && teamIdLocal != winner)
            {
                networkingManager.inningCountSynced++;
            }
            initializeRack();
            if (winnerBreak)
            {
                initialPositions[gameModeLocal][0] = ballsP[0];
            }
            networkingManager._OnGameNextBreak(
                initialBallsPocketed[gameModeLocal], initialPositions[gameModeLocal], 
                (winnerBreak ? winner : winner ^ 0x1U), 
                (winnerBreak ? 3 : 1), false);
            return;                
        }

#endif
        networkingManager._OnGameWin(winner);
    }

#if EIJIS_ROTATION
    private void onLocalTurnPass(bool reBreakAllowed, bool skipTurn)
#else
    private void onLocalTurnPass()
#endif
    {
        _LogInfo($"onLocalTurnPass");

#if EIJIS_ROTATION
        if (teamIdLocal != 0)
        {
            networkingManager.inningCountSynced++;
        }

#endif
#if EIJIS_ROTATION
        networkingManager._OnTurnPass(teamIdLocal ^ 0x1u, reBreakAllowed, skipTurn);
#else
        networkingManager._OnTurnPass(teamIdLocal ^ 0x1u);
#endif
    }

    private void onLocalTurnTie()
    {
        _LogInfo($"onLocalTurnTie");

        networkingManager._OnTurnTie();
    }

#if EIJIS_ROTATION
    private void onLocalTurnFoul(bool Scratch, bool objBlocked, bool reposition, bool reBreakAllowed)
#else
    private void onLocalTurnFoul(bool Scratch, bool objBlocked)
#endif
    {
        _LogInfo($"onLocalTurnFoul");

#if EIJIS_ROTATION
        if (teamIdLocal != 0)
        {
            networkingManager.inningCountSynced++;
        }

        if (isRotation)
        {
            networkingManager._OnTurnFoul(teamIdLocal ^ 0x1u, Scratch, objBlocked, reposition, true, reBreakAllowed);
            return;
        }
        
#endif
#if EIJIS_ROTATION
        networkingManager._OnTurnFoul(teamIdLocal ^ 0x1u, Scratch, objBlocked, reposition, false, false);
#else
        networkingManager._OnTurnFoul(teamIdLocal ^ 0x1u, Scratch, objBlocked);
#endif
    }

#if EIJIS_BOWLARDS
    private void onLocalTurnContinue(bool isScratch = false)
#else
    private void onLocalTurnContinue()
#endif
    {
        _LogInfo($"onLocalTurnContinue");

        networkingManager._OnTurnContinue(isScratch);
    }

    private void onLocalTimerEnd()
    {
        timerRunning = false;
#if EIJIS_SEMIAUTOCALL
        semiAutoCallTick = false;
#endif

        _LogWarn("out of time!");

        graphicsManager._HideTimers();

        canPlayLocal = false;

#if EIJIS_CALLSHOT
        calledBallId = -2;
        calledPocketId = -2;
#if EIJIS_SEMIAUTOCALL
        semiAutoCalledPocket = false;
        semiAutoCalledTimeBall = 0;
#endif
#endif
        if (Networking.IsOwner(Networking.LocalPlayer, networkingManager.gameObject))
        {
#if EIJIS_EXTERNAL_SCORE_SCREEN && EIJIS_ROTATION && EIJIS_BOWLARDS
            if (isRotation || isBowlards)
            {
                if (!ReferenceEquals(null, scoreScreen)) scoreScreen.updateInfoText(networkingManager.stateIdSynced, (teamIdLocal == 0 ? "[Red ]" : "[Blue]") + " time over");
            }
#endif
            fakeFoulShot();
        }
    }

    private void applyCueAccess()
    {
#if EIJIS_ISSUE_FIX // 2本目のキュー座標がリセットされない場合がある （ issue #3 の修正）
        if (!gameLive)
        {
            if (_TeamPlayersOffline(0)) cueControllers[0]._SetAuthorizedOwners(Networking.IsMaster ? new[] { Networking.LocalPlayer.playerId } : new int[0]);
            if (_TeamPlayersOffline(1)) cueControllers[1]._SetAuthorizedOwners(Networking.IsMaster ? new[] { Networking.LocalPlayer.playerId } : new int[0]);
        }
#endif
        if (localPlayerId == -1 || !gameLive)
        {
            cueControllers[0]._Disable();
            cueControllers[1]._Disable();
            return;
        }

        if (localTeamId == 0)
        {
            cueControllers[0]._Enable();
            cueControllers[1]._Disable();
        }
        else
        {
            cueControllers[1]._Enable();
            cueControllers[0]._Disable();
        }
    }

    private void enablePlayComponents()
    {
#if EIJIS_DEBUG_CALLSHOT_TURNPASS_OPTION
        _LogInfo("  BilliardsModule::enablePlayComponents()");
#endif
        bool isOurTurnVar = isMyTurn();
        bool practiceEnable = (isOurTurnVar && isPracticeMode);
        
#if EIJIS_10BALL || EIJIS_CUEBALLSWAP || EIJIS_ROTATION || EIJIS_BOWLARDS
        if (is9Ball || is10Ball || isPyramid || isRotation || (isBowlards && colorTurnLocal))
#else
        if (is9Ball)
#endif
        {
            marker9ball.SetActive(true);
            _Update9BallMarker();
        }

        refreshBallPickups();

#if EIJIS_CUEBALLSWAP
        if (isOurTurnVar && isPyramid)
        {
            desktopManager._CallCueBallSetActive(true);
#if EIJIS_CALLSHOT
#if EIJIS_CALL_DEFAULT_LOCKING
            if (Networking.LocalPlayer.IsUserInVR()) menuManager._EnableCallClearMenu();
#else
            if (Networking.LocalPlayer.IsUserInVR()) menuManager._EnableCallLockMenu();
#endif
#endif
        }
        else
        {
            desktopManager._CallCueBallSetActive(false);
        }

#endif
#if EIJIS_PUSHOUT
#if EIJIS_10BALL || EIJIS_ROTATION
        bool canPushOut = (isOurTurnVar && (is8Ball || is9Ball || is10Ball || isRotation) && (pushOutStateLocal == PUSHOUT_DONT || pushOutStateLocal == PUSHOUT_DOING));
#else
        bool canPushOut = (isOurTurnVar && is8Ball && (pushOutStateLocal == PUSHOUT_DONT || pushOutStateLocal == PUSHOUT_DOING));
#endif
        desktopManager._PushOutSetActive(canPushOut);
        if (!ReferenceEquals(null, pushOutOrb)) pushOutOrb.SetActive(canPushOut);
        if (canPushOut)
        {
            menuManager._EnablePushOutMenu();
        }
        else
        {
            menuManager._DisablePushOutMenu();
        }
#endif
#if EIJIS_CALLSHOT
        markerCalledBall.SetActive(false);
#if EIJIS_10BALL || EIJIS_ROTATION || EIJIS_BOWLARDS
        if (isOurTurnVar && (is8Ball || is10Ball || isRotation || isBowlards))
#else
        if (isOurTurnVar && (is8Ball))
#endif
        {
            if (requireCallShotLocal)
            {
                // if (Networking.LocalPlayer.IsUserInVR())
                {
#if EIJIS_CALL_DEFAULT_LOCKING
                    menuManager._EnableCallClearMenu();
#else
                    menuManager._EnableCallLockMenu();
#endif
                    if (!isBowlards)
                    {
                        menuManager._EnableCallSafetyMenu();
                    }
                    else
                    {
                        menuManager._DisableCallSafetyMenu();
                    }
                }
                // else
                // {
                //     menuManager._DisableCallLockMenu();
                //     menuManager._DisableCallSafetyMenu();
                // }
                desktopManager._CallShotSetActive(true);
                desktopManager._CallSafetySetActive(!isBowlards);
                
            }
            else
            {
#if EIJIS_CALL_DEFAULT_LOCKING
                menuManager._DisableCallClearMenu();
#else
                menuManager._DisableCallLockMenu();
#endif
                menuManager._DisableCallSafetyMenu();
                desktopManager._CallShotSetActive(false);
                desktopManager._CallSafetySetActive(false);
            }

            if (!ReferenceEquals(null, callSafetyOrb) && !isBowlards) callSafetyOrb.SetActive(requireCallShotLocal);
#if EIJIS_CALL_DEFAULT_LOCKING
            if (!ReferenceEquals(null, callClearOrb)) callClearOrb.SetActive(requireCallShotLocal);
#else
            if (!ReferenceEquals(null, callShotLockOrb)) callShotLockOrb.SetActive(requireCallShotLocal);
#endif
        }
        else
        {
#if EIJIS_CALL_DEFAULT_LOCKING
            menuManager._DisableCallClearMenu();
#else
            menuManager._DisableCallLockMenu();
#endif
            menuManager._DisableCallSafetyMenu();
            desktopManager._CallShotSetActive(false);
            desktopManager._CallSafetySetActive(false);
            
            if (!ReferenceEquals(null, callSafetyOrb)) callSafetyOrb.SetActive(false);
#if EIJIS_CALL_DEFAULT_LOCKING
            if (!ReferenceEquals(null, callClearOrb)) callClearOrb.SetActive(false);
#else
            if (!ReferenceEquals(null, callShotLockOrb)) callShotLockOrb.SetActive(false);
#endif
        }

        if (!isOurTurnVar)
        {
#if EIJIS_CALL_DEFAULT_LOCKING
            menuManager._DisableCallClearMenu();
#else
            menuManager._DisableCallLockMenu();
#endif
            menuManager._DisableCallSafetyMenu();
        }
#endif
        
        if (isOurTurnVar)
        {
            // Update for desktop
            desktopManager._AllowShoot();
            // menuManager._EnableSkipTurnMenu();
        }
        else
        {
            desktopManager._DenyShoot();
            // menuManager._DisableSkipTurnMenu();
        }

#if EIJIS_CALLSHOT && EIJIS_PUSHOUT
        bool canSkipTurnMenu = practiceEnable;
        bool canSkipTurnOrb = false;
        if (!canSkipTurnMenu && isOurTurnVar)
        {
            if (is8Ball || is9Ball || is10Ball || isRotation)
            {
                if (pushOutStateLocal == PUSHOUT_REACTIONING)
                {
                    canSkipTurnMenu = true;
                    canSkipTurnOrb = true;
                }
                else if (!is9Ball 
                         && pushOutStateLocal == PUSHOUT_ENDED 
                         && callPassOptionLocal 
                         && !cantSkipNextTurnOnCallShotOption
                         && ((isRotation && (networkingManager.nextBallRepositionStateSynced & 0x1u) == 0) || (!isRotation && !isReposition))
                         )
                {
                    canSkipTurnMenu = true;
                    canSkipTurnOrb = true;
                }
            }
            else
            {
                canSkipTurnMenu = true;
            }
        }
#if EIJIS_DEBUG_CALLSHOT_TURNPASS_OPTION
        _LogInfo($"    canSkipTurn = {canSkipTurn}");
#endif

        if (!ReferenceEquals(null, skipTurnOrb)) skipTurnOrb.SetActive(canSkipTurnOrb);
        
        if (canSkipTurnMenu)
        {
            menuManager._EnableSkipTurnMenu();
        }
        else
        {
            menuManager._DisableSkipTurnMenu();
        }
#else        
        if (isOurTurnVar)
        {
            menuManager._EnableSkipTurnMenu();
        }
        else
        {
            menuManager._DisableSkipTurnMenu();
        }

#endif
        if (timerLocal > 0)
        {
            timerRunning = true;
            graphicsManager._ShowTimers();
        }
    }

    public void _SkipTurn()
    {
        if (!isMyTurn()) { return; }
#if EIJIS_CALLSHOT
        calledBallId = -2;
        calledPocketId = -2;
#if EIJIS_SEMIAUTOCALL
        semiAutoCallTick = false;
        semiAutoCalledPocket = false;
        semiAutoCalledTimeBall = 0;
#endif
#endif
#if EIJIS_BOWLARDS
        if (isBowlards)
        {
            bool isOnBreakShot = colorTurnLocal;
            if (isOnBreakShot && (playerIDsLocal[1] != -1 || playerIDsLocal[3] != -1))
            {
                onLocalTeamWin(teamIdLocal ^ 0x1U, false);
                return;
            }
        }
#endif
#if EIJIS_EXTERNAL_SCORE_SCREEN && EIJIS_ROTATION && EIJIS_BOWLARDS
        if (isRotation || isBowlards)
        {
            if (!ReferenceEquals(null, scoreScreen)) scoreScreen.updateInfoText(networkingManager.stateIdSynced, (teamIdLocal == 0 ? "[Red ]" : "[Blue]") + " skip turn");
        }
#endif
#if EIJIS_PUSHOUT
        if (pushOutStateLocal == PUSHOUT_REACTIONING /* || pushOutStateLocal == PUSHOUT_ILLEGAL_REACTIONING */)
        {
#if EIJIS_DEBUG_PUSHOUT
            _LogInfo($"  set {((pushOutStateLocal == PUSHOUT_REACTIONING)? "ENDED" : "DONT")} pushOutState {PushOutState[pushOutStateLocal]}({pushOutStateLocal})");
#endif
            networkingManager.pushOutStateSynced = (pushOutStateLocal == PUSHOUT_REACTIONING)? PUSHOUT_ENDED : PUSHOUT_DONT;
#if EIJIS_ROTATION
            onLocalTurnPass(false, true);
#else
            onLocalTurnPass();
#endif
            return;
        }
#endif
#if EIJIS_CALLSHOT
        if ((is8Ball || is10Ball || isRotation) && requireCallShotLocal && callPassOptionLocal && !cantSkipNextTurnOnCallShotOption)
        {
#if EIJIS_ROTATION
            onLocalTurnPass(false, true);
#else
            onLocalTurnPass();
#endif
            return;
        }
#endif
        fakeFoulShot();
    }

    public void fakeFoulShot()
    {
        onRemoteTurnSimulate(Vector3.zero, Vector3.zero, true);
        _TriggerSimulationEnded(false, true);
    }

#if EIJIS_CALLSHOT || EIJIS_CUEBALLSWAP
    public void _CallShotLock()
    {
#if EIJIS_DEBUG_CALLCLEAR
        _LogInfo($"EIJIS_DEBUG BilliardsModule::_CallShotLock()");
#endif
        networkingManager._OnCallShotLockChanged(!callShotLockLocal);
    }

#endif
#if EIJIS_CALLSHOT
    public void _CallSafety()
    {
        networkingManager._OnSafetyCallChanged(!safetyCalledLocal);
    }
    
    public void _CallClear()
    {
#if EIJIS_DEBUG_CALLCLEAR
        _LogInfo($"EIJIS_DEBUG BilliardsModule::_CallClear()");
#endif
        networkingManager._OnCallShotClear();
    }

#endif
#if EIJIS_PUSHOUT
    public void _PushOut()
    {
        networkingManager._OnPushOutChanged(pushOutStateLocal);
    }

#endif
#if EIJIS_ROTATION
    
    public void _NextBallOnSpot(float x)
    {
        int target = -1;
        if (isBowlards)
        {
            uint ball_bit = 0x2u;
            float minX = float.MaxValue;
            for (int k = 0; k < break_order_10ball.Length; k++)
            {
                int i = k + 1;
                if ((ballsPocketedLocal & ball_bit) == 0x0u)
                {
                    if (ballsP[i].x <= -(k_TABLE_WIDTH / 2))
                    {
                        if (ballsP[i].x < minX)
                        {
                            minX = ballsP[i].x;
                            target = i;
                        }
                    }
                }
                ball_bit <<= 1;
            }
        }
        else
        {
            target = findLowestUnpocketedBall(ballsPocketedLocal);
        }
        if (target < 0) return;
        pocketedballUponPool(0x1u << target, x);

        if (semiAutoCallLocal)
        {
            networkingManager.calledBallsSynced = 0;
            networkingManager.pointPocketsSynced = 0;
        }

        cantSkipNextTurnOnCallShotOption = true;
        
        uint nextBallRepositionState = nextBallRepositionStateLocal & ~0x1u;
        nextBallRepositionState |= (x == 0 ? 0x10u : 0x08u);
        networkingManager.nextBallRepositionStateSynced = (byte)nextBallRepositionState;
        networkingManager._OnRepositionBalls(ballsP);
    }

    public void _RequestBreak(uint teamId)
    {
#if TKCH_DEBUG_BREAKING_FOUL
        _LogInfo($"TKCH BilliardsModule::_RequestBreak(teamId = {teamId})");
#endif
         if (teamIdLocal == 0 && teamIdLocal == teamId)
         {
#if TKCH_DEBUG_BREAKING_FOUL
             _LogInfo("  inning count down");
#endif
             networkingManager.inningCountSynced--;
         }
         initializeRack_Rotation();
         initialPositions[GAMEMODE_ROTATION_15][0] = initialPositions[0][0];
         initialPositions[GAMEMODE_ROTATION_10][0] = initialPositions[0][0];
         initialPositions[GAMEMODE_ROTATION_9][0] = initialPositions[0][0];
         initialPositions[GAMEMODE_ROTATION_6][0] = initialPositions[0][0];
         networkingManager._OnGameNextBreak(
             initialBallsPocketed[gameModeLocal], initialPositions[gameModeLocal], 
             teamId, 1, true);
    }

    public void _CueBallInKitchen()
    {
        // next ballがkitchen内の場合はセンターに移動させる
        checkNextInKitchenThenMoveToCenter();
        
        ballsP[0] = new Vector3(-k_TABLE_WIDTH / 2, 0.0f, 0.0f); // initialPositions[gameModeLocal][0];
        // 的玉移動 → 手玉かぶりでずらして配置 → 手玉選択 の場合にずれたままになるのを回避
        if ((nextBallRepositionStateLocal & (0x08u | 0x10u)) > 0 )
        {
            int target = findLowestUnpocketedBall(ballsPocketedLocal);
            float x = ((nextBallRepositionStateLocal & 0x08u) > 0 ? markerFootSpot.transform.localPosition.x : 0);
            pocketedballUponPool(0x1u << target, x);
        }
        
        if (semiAutoCallLocal)
        {
            networkingManager.calledBallsSynced = 0;
            networkingManager.pointPocketsSynced = 0;
        }

        networkingManager.foulStateSynced = 2; // 1
        networkingManager.nextBallRepositionStateSynced = (byte)(nextBallRepositionStateLocal & ~0x2u);
        networkingManager._OnRepositionBalls(ballsP);
    }

    private void checkNextInKitchenThenMoveToCenter()
    {
        int target = findLowestUnpocketedBall(ballsPocketedLocal);
        float kitchen_x = -k_TABLE_WIDTH / 2;
        if (ballsP[target].x < kitchen_x)
        {
            // next ballがkitchen内の場合はセンターに移動させる
            //ballsP[target] = new Vector3(0.0f, 0.0f, 0.0f);
            pocketedballUponPool(0x1u << target, 0);
        }
    }
#endif
    public void _Update9BallMarker()
    {
        if (marker9ball.activeSelf)
        {
#if EIJIS_CUEBALLSWAP
            if (isPyramid)
            {
                marker9ball.transform.localPosition = ballsP[0];
            }
#if EIJIS_BOWLARDS
            else if (isBowlards)
            {
                bool isOnBreakShot = colorTurnLocal;
                if (isOnBreakShot)
                {
                    byte[] frameCount = fbScoresLocal;
                    int target = pocketed_ball_upon_pool_order[frameCount[teamIdLocal]];
                    // move without changing y
                    Vector3 oldpos = marker9ball.transform.localPosition;
                    Vector3 newpos = ballsP[target];
                    marker9ball.transform.localPosition = new Vector3(newpos.x, oldpos.y, newpos.z);
                }
                else
                {
                    marker9ball.SetActive(false);
                }
            }
#endif
            else
            {
                int target = findLowestUnpocketedBall(ballsPocketedLocal);
                // move without changing y
                Vector3 oldpos = marker9ball.transform.localPosition;
                Vector3 newpos = ballsP[target];
                marker9ball.transform.localPosition = new Vector3(newpos.x, oldpos.y, newpos.z);
            }
#else
            int target = findLowestUnpocketedBall(ballsPocketedLocal);
            // move without changing y
            Vector3 oldpos = marker9ball.transform.localPosition;
            Vector3 newpos = ballsP[target];
            marker9ball.transform.localPosition = new Vector3(newpos.x, oldpos.y, newpos.z);
#endif
        }
    }

#if EIJIS_CALLSHOT
    public void _UpdateCalledBallMarker()
    {
        if (!gameLive)
        {
            return;
        }

        if (!requireCallShotLocal)
        {
            return;
        }

#if EIJIS_10BALL || EIJIS_ROTATION || EIJIS_BOWLARDS
        if (!is8Ball && !is10Ball && !isRotation && !isBowlards)
#else
        if (!is8Ball)
#endif
        {
            return;
        }

        if (calledBallsLocal == 0)
        {
            markerCalledBall.SetActive(false);
        }
        
        int target = 0;
        uint ball_bit = 0x2u;
#if EIJIS_10BALL || EIJIS_ROTATION || EIJIS_BOWLARDS
        for (int k = 0; k < (isRotation6Balls ? break_order_rotation_6ball.Length + 1 : (is9Ball || isRotation9Balls ? break_order_9ball.Length : (is10Ball || isRotation10Balls || isBowlards ? break_order_10ball.Length : break_order_8ball.Length))); k++)
#else
        for (int k = 0; k < (is9Ball ? break_order_9ball.Length : break_order_8ball.Length); k++)
#endif
        {
            int i = k + 1;
            if ((calledBallsLocal & ball_bit) != 0x0u)
            {
                target = i;
            }                
            ball_bit <<= 1;
        }

        if (0 < target && 0 == (ballsPocketedLocal & (0x1 << target)))
        {
            bool callShotLock = callShotLockLocal && turnStateLocal != 1;
#if EIJIS_CALL_MARKER_TO_GRAY_ON_CUE_TRIGGER_ACTIVATE
            markerCalledBall.GetComponent<MeshRenderer>().material = ((callShotLock || canHitCueBall) && !isLocalSimulationRunning) ? calledBallMarkerGray :
#else
            markerCalledBall.GetComponent<MeshRenderer>().material = callShotLock ? calledBallMarkerGray :
#endif
                (isTableOpenLocal ? calledBallMarkerWhite :
                    ((teamIdLocal ^ teamColorLocal) == 0 ? calledBallMarkerBlue : calledBallMarkerOrange));
            markerCalledBall.transform.localPosition = ballsP[target];
            markerCalledBall.SetActive(true);
        }
        else
        {
            markerCalledBall.SetActive(false);
        }
    }

    public void _UpdateCalledPocketMarker()
    {
        graphicsManager._UpdatePointPocketMarker(pointPocketsLocal, false);
    }

#endif
#if EIJIS_ROTATION
    public void _UpdateNextBallRepositionSpotMarker()
    {
        if (/* !isMyTurn() || */ nextBallRepositionStateLocal == 0 || callShotLockLocal)
        {
            if (!ReferenceEquals(null, markerHeadSpot)) markerHeadSpot.SetActive(false);
            if (!ReferenceEquals(null, markerCenterSpot)) markerCenterSpot.SetActive(false);
            if (!ReferenceEquals(null, markerFootSpot)) markerFootSpot.SetActive(false);
            if (!ReferenceEquals(null, requestBreakOrange)) requestBreakOrange.SetActive(false);
            if (!ReferenceEquals(null, requestBreakBlue)) requestBreakBlue.SetActive(false);
            return;
        }
        
        bool isOurTurnVar = isMyTurn();

        if ((nextBallRepositionStateLocal & 0x1u) > 0 )
        {
            if (isRotation)
            {
                if (!ReferenceEquals(null, markerCenterSpot)) markerCenterSpot.GetComponent<Collider>().enabled = isOurTurnVar;
                if (!ReferenceEquals(null, markerCenterSpot)) markerCenterSpot.SetActive(true);
            }
            if (!ReferenceEquals(null, markerFootSpot)) markerFootSpot.GetComponent<Collider>().enabled = isOurTurnVar;
            if (!ReferenceEquals(null, markerFootSpot)) markerFootSpot.SetActive(true);
        }
        else
        {
            if (!ReferenceEquals(null, markerCenterSpot)) markerCenterSpot.SetActive(false);
            if (!ReferenceEquals(null, markerFootSpot)) markerFootSpot.SetActive(false);
        }
        
        if ((nextBallRepositionStateLocal & 0x2u) > 0 /* || foulStateLocal == 0 */)
        {
            if (chainedFoulsLocal[teamIdLocal ^ 0x1u] < 3)
            {
                if (!ReferenceEquals(null, markerHeadSpot)) markerHeadSpot.GetComponent<Collider>().enabled = isOurTurnVar;
                if (!ReferenceEquals(null, markerHeadSpot)) markerHeadSpot.SetActive(true);
            }
            else
            {
                if (!ReferenceEquals(null, markerHeadSpot)) markerHeadSpot.SetActive(false);
                if (isOurTurnVar)
                {
                    isReposition = true;
                    Vector3 k_pR = (Vector3)currentPhysicsManager.GetProgramVariable("k_pR");
                    repoMaxX = k_pR.x;
                    setFoulPickupEnabled(true);
                    foulStateLocal = 2;
                }
            }
        }
        else
        {
            if (!ReferenceEquals(null, markerHeadSpot)) markerHeadSpot.SetActive(false);
        }
        
        if ((nextBallRepositionStateLocal & 0x4u) > 0 )
        {
            if (!ReferenceEquals(null, requestBreakOrange)) requestBreakOrange.GetComponent<Collider>().enabled = isOurTurnVar;
            if (!ReferenceEquals(null, requestBreakBlue)) requestBreakBlue.GetComponent<Collider>().enabled = isOurTurnVar;
            if (!ReferenceEquals(null, requestBreakOrange)) requestBreakOrange.SetActive(true);
            if (!ReferenceEquals(null, requestBreakBlue)) requestBreakBlue.SetActive(true);
        }
        else
        {
            if (!ReferenceEquals(null, requestBreakOrange)) requestBreakOrange.SetActive(false);
            if (!ReferenceEquals(null, requestBreakBlue)) requestBreakBlue.SetActive(false);
        }
    }

#endif
    // turn off any game elements that are enabled when someone is taking a shot
    private void disablePlayComponents()
    {
        marker9ball.SetActive(false);
#if EIJIS_CALLSHOT
        markerCalledBall.SetActive(false);
#endif
        setFoulPickupEnabled(false);
        refreshBallPickups();
        devhit.SetActive(false);
        guideline.SetActive(false);
        guideline2.SetActive(false);
        isGuidelineValid = false;
        isReposition = false;
        auto_colliderBaseVFX.SetActive(false);

#if EIJIS_PUSHOUT
        if (!ReferenceEquals(null, pushOutOrb)) pushOutOrb.SetActive(false);
#endif
#if EIJIS_CALLSHOT
        if (!ReferenceEquals(null, callSafetyOrb)) callSafetyOrb.SetActive(false);
#if EIJIS_CALL_DEFAULT_LOCKING
        if (!ReferenceEquals(null, callClearOrb)) callClearOrb.SetActive(false);
#else
        if (!ReferenceEquals(null, callShotLockOrb)) callShotLockOrb.SetActive(false);
#endif
        if (!ReferenceEquals(null, skipTurnOrb)) skipTurnOrb.SetActive(false);
#if EIJIS_CALL_DEFAULT_LOCKING
        menuManager._DisableCallClearMenu();
#else
        menuManager._DisableCallLockMenu();
#endif
        menuManager._DisableCallSafetyMenu();
#endif
#if EIJIS_PUSHOUT
        menuManager._DisablePushOutMenu();
        desktopManager._PushOutSetActive(false);
#if EIJIS_CUEBALLSWAP
        desktopManager._CallCueBallSetActive(false);
#endif
        
#endif
        desktopManager._DenyShoot();
        graphicsManager._HideTimers();
    }

    public void fourBallReturnBalls()
    {
        bool zeroPocketed = false;
        bool thirteenPocketed = false;
        bool fourteenPocketed = false;
        bool fifteenPocketed = false;
        if (fourBallCueBallLocal == 0) // the balls get their positions and color swapped so player 2 can hit the 'yellow' cue ball
        {
            if ((ballsPocketedLocal & (0x1U)) > 0)
            { ballsP[0] = initialPositions[2][0]; zeroPocketed = true; }
            if ((ballsPocketedLocal & (0x1U << 13)) > 0)
            { ballsP[13] = initialPositions[2][13]; thirteenPocketed = true; }
        }
        else
        {
            if ((ballsPocketedLocal & (0x1U)) > 0)
            { ballsP[0] = initialPositions[2][13]; zeroPocketed = true; }
            if ((ballsPocketedLocal & (0x1U << 13)) > 0)
            { ballsP[13] = initialPositions[2][0]; thirteenPocketed = true; }
        }

        if ((ballsPocketedLocal & (0x1U << 14)) > 0)
        { ballsP[14] = initialPositions[2][14]; fourteenPocketed = true; }
        if ((ballsPocketedLocal & (0x1U << 15)) > 0)
        { ballsP[15] = initialPositions[2][15]; fifteenPocketed = true; }
#if EIJIS_CAROM
#if EIJIS_BANKING
        fifteenPocketed = (fifteenPocketed && !is3Cusion && !is2Cusion && !is1Cusion && !is0Cusion && !isBanking);
#else
        fifteenPocketed = (fifteenPocketed && !is3Cusion && !is2Cusion && !is1Cusion && !is0Cusion);
#endif
#endif

#if EIJIS_CAROM
        ballsPocketedLocal = initialBallsPocketed[gameModeLocal];
#else
        ballsPocketedLocal = initialBallsPocketed[2];
#endif
#if EIJIS_CAROM
        if (is3Cusion || is2Cusion || is1Cusion || is0Cusion)
        {
            int[] threeBalls = new int[] { 0, 13, 14 };
            bool[] threeBallsPocketed = new bool[] { zeroPocketed, thirteenPocketed, fourteenPocketed };
            Vector3[] threeBallReturnPositions = new Vector3[]
            {
                new Vector3(0.0f, 0.0f, 0.0f), 
                initialPositions[gameModeLocal][13], 
                initialPositions[gameModeLocal][14]
            };
            
            for (int i = 0; i < threeBallsPocketed.Length; i++)
            {
                if (threeBallsPocketed[i])
                {
                    ballsP[threeBalls[i]] = threeBallReturnPositions[i];
                }
            }
            
            for (int i = 0; i < threeBallsPocketed.Length; i++)
            {
                if (threeBallsPocketed[i])
                {
                    int touchBallId = CheckIfBallTouchingBall(threeBalls[i]);
                    if (0 <= touchBallId)
                    {
                        
                        ballsP[touchBallId] = threeBallReturnPositions[Array.IndexOf(threeBalls, touchBallId)];
                        for (int j = 0; j < threeBallsPocketed.Length; j++)
                        {
                            if (j == i)
                                continue;
                            
                            int touchBallId_2 = CheckIfBallTouchingBall(threeBalls[j]);
                            if (0 <= touchBallId_2)
                            {
                                ballsP[touchBallId_2] = threeBallReturnPositions[Array.IndexOf(threeBalls, touchBallId_2)];
                            }
                        }
                    }
                }
            }
            
            return;
        }
#endif
        Vector3 dir = Vector3.right * k_BALL_RADIUS * .051f;
        if (zeroPocketed) moveBallInDirUntilNotTouching(0, dir);
        if (thirteenPocketed) moveBallInDirUntilNotTouching(13, dir);
        if (fourteenPocketed) moveBallInDirUntilNotTouching(14, dir);
        if (fifteenPocketed) moveBallInDirUntilNotTouching(15, dir);
    }

    public string sixRedNumberToColor(int ball, bool doBreakOrder = false)
    {
#if EIJIS_SNOOKER15REDS
        if ((doBreakOrder && (ball < 0 || 20 < ball)) ||
            (!doBreakOrder && ((ball < 0 || 15 < ball) && (ball < 25 || 30 < ball))))
#else
        if (ball < 0 || ball > 12)
#endif
        {
            _LogWarn("sixRedNumberToColor: ball index out of range");
            return "Invalid";
        }
        if (doBreakOrder)
        {
            ball = break_order_sixredsnooker[ball];
        }
        switch (ball)
        {
            case 2: return "Yellow";
            case 7: return "Green";
            case 8: return "Brown";
            case 3: return "Blue";
            case 5: return "Pink";
            case 1: return "Black";
            case 0: return "White";
            default: return "Red";
        }
    }

    private int SixRedCheckObjBlocked(uint field, bool colorTurn, bool includeFreeBall)
    {
        //in case of undo/redo the results of these methods need to be re-calculated
        bool redOnTable = sixRedCheckIfRedOnTable(field, false);
        int nextcolor = sixRedFindLowestUnpocketedColor(field);
        uint objective = sixRedGetObjective(colorTurn, redOnTable, nextcolor, false, includeFreeBall);
        // 0 = fully visible, 1 = left OR right blocked, 2 = both blocked
        return objVisible(objective);
    }

    public int sixRedFindLowestUnpocketedColor(uint field)
    {
#if EIJIS_SNOOKER15REDS
        for (int i = SNOOKER_REDS_COUNT; i < break_order_sixredsnooker.Length; i++)
#else
        for (int i = 6; i < break_order_sixredsnooker.Length; i++)
#endif
        {
            if (((field >> break_order_sixredsnooker[i]) & 0x1U) == 0x00U)
            {
                return i;
            }
        }

        return -1;
    }

    public bool sixRedCheckIfRedOnTable(uint field, bool writeLog)
    {
#if EIJIS_SNOOKER15REDS
        for (int i = 0; i < SNOOKER_REDS_COUNT; i++)
#else
        for (int i = 0; i < 6; i++)
#endif
        {
            if (((field >> break_order_sixredsnooker[i]) & 0x1U) == 0x00U)
            {
                if (writeLog)
                {
                    _LogInfo("6RED: All reds not yet pocketed");
                }
                return true;
            }
        }
        return false;
    }

    public int sixRedCheckFirstHit(int firstHit)
    {
        //return 0 for red hit
        uint firstHitball = 1u << firstHit;
#if EIJIS_SNOOKER15REDS
        if ((firstHitball & SNOOKER_REDS_MASK) > 0)
#else
        if ((firstHitball & 0x1E50u) > 0)
#endif
        {
            _LogInfo("6RED: Hit first: Red");
            return 0;
        }
        //return 1 for color hit
        if ((firstHitball & 0x1AE) > 0)
        {
            if (foulStateLocal == 5)
            {
                _LogInfo("6RED: Hit first: (free ball)");
                return 0;
            }
            else
            {
                _LogInfo("6RED: Hit first: Color");
                return 1;
            }
        }
        return -1;
    }

    public void sixRedReturnColoredBalls(int from)
    {
#if EIJIS_SNOOKER15REDS
        if (from < SNOOKER_REDS_COUNT)
#else
        if (from < 6)
#endif
        {
            _LogWarn("sixRedReturnColoredBalls() requested return of red balls");
            return;
        }
#if EIJIS_SNOOKER15REDS
        for (int i = Mathf.Max(SNOOKER_REDS_COUNT, from); i < break_order_sixredsnooker.Length; i++)
#else
        for (int i = Mathf.Max(6, from); i < break_order_sixredsnooker.Length; i++)
#endif
        {
            if ((ballsPocketedLocal & (1 << break_order_sixredsnooker[i])) > 0)
            {
                // ballsP[break_order_sixredsnooker[i]] = initialPositions[4][break_order_sixredsnooker[i]];
                sixRedMoveBallUntilNotTouching(break_order_sixredsnooker[i]);
                ballsPocketedLocal = ballsPocketedLocal ^ (1u << break_order_sixredsnooker[i]);
            }
        }
    }

    public void sixRedScoreBallsPocketed(bool redOnTable, int nextColor, ref int ballscore, ref int numBallsPocketed, ref int highestScoringBall)
    {
        bool freeBall = foulStateLocal == 5;
        bool nextColorPocketed = false;
        bool freeBallPocketed = false;
#if EIJIS_SNOOKER15REDS
        for (int i = 1; i < sixredsnooker_ballpoints.Length; i++)
#else
        for (int i = 1; i < 13; i++)
#endif
        {
            if ((ballsPocketedLocal & (1u << i)) > (ballsPocketedOrig & (1u << i)))
            {
                int thisBallScore = sixredsnooker_ballpoints[i];
                if (freeBall)
                {
                    // pocketing freeball and the nextColor in the same turn is actually legal, but you don't add the points up from potting both
                    // because in the freeball rule, they're the same ball, unless they're reds.
                    // since the break_order_sixredsnooker[] is not in sequential order it has to be checked both ways
                    if (i == break_order_sixredsnooker[nextColor])
                    {
                        // _LogInfo("6RED: nextColor Pocketed in freeball turn");
                        nextColorPocketed = true;
                        if (freeBallPocketed)
                        {
                            thisBallScore = 0;
                            numBallsPocketed--;// prevent foul
                        }
                    }
                    else if (i == firstHit)
                    {
                        // _LogInfo("6RED: freeBall Pocketed in freeball turn");
                        freeBallPocketed = true;
                        if (redOnTable)
                        {
                            thisBallScore = 1;
                        }
                        else if (nextColorPocketed)
                        {
                            thisBallScore = 0;
                            numBallsPocketed--;// prevent foul
                        }
                        else
                        {
                            thisBallScore = sixredsnooker_ballpoints[break_order_sixredsnooker[sixRedFindLowestUnpocketedColor(ballsPocketedOrig)]];
                        }
                    }
                }
                if (highestScoringBall < thisBallScore)
                { highestScoringBall = thisBallScore; }
                ballscore += thisBallScore;
                numBallsPocketed++;
                if (freeBall && firstHit == i)
                {
                    _LogInfo("6RED: " + sixRedNumberToColor(i) + "(free ball) pocketed");
                }
                else
                {
                    _LogInfo("6RED: " + sixRedNumberToColor(i) + " ball pocketed");
                }
            }
        }
    }

    public int sixRedCheckBallTypesPocketed(uint ballsPocketedOrig, uint ballsPocketedLocal)
    {
        // for free ball : convert firsthit to a mask and add/remove it from red/color masks
#if EIJIS_SNOOKER15REDS
        uint redMask = SNOOKER_REDS_MASK;
#else
        uint redMask = 0x1E50u;
#endif
        uint colorMask = 0x1AE;
        if (foulStateLocal == 5)
        {
            uint firstHitMask = 1u << firstHit;
            redMask = redMask | firstHitMask;
            colorMask = colorMask & ~firstHitMask;
        }
        int result = -1;
        if ((ballsPocketedOrig & redMask) < (ballsPocketedLocal & redMask))
        {
            // _LogInfo("6RED: At least one red ball was pocketed");
            result = 0;
        }
        if ((ballsPocketedOrig & colorMask) < (ballsPocketedLocal & colorMask))
        {
            if (result == 0)
            {
                result = 2;
                _LogInfo("6RED: Both Red and color balls were pocketed");
            }
            else
            {
                result = 1;
                // _LogInfo("6RED: At least one color ball pocketed");
            }

        }
        return result;
    }

    public uint sixRedGetObjective(bool _colorTurn, bool _redOnTable, int _nextcolor, bool writeLog, bool includeFreeBall)
    {
#if EIJIS_SNOOKER15REDS
        uint objective = SNOOKER_REDS_MASK;
#else
        uint objective = 0x1E50u;
#endif
        if (writeLog)
        {
            if (_colorTurn) { _LogInfo("6RED: That was a ColorTurn"); }
            else { _LogInfo("6RED: That was not a ColorTurn"); }
        }
        if (_colorTurn)
        {
            objective = 0x1AE;//color balls
            if (writeLog) { _LogInfo("6RED: Objective is: Any color"); }
        }
        else if (!_redOnTable)
        {
            objective = (uint)(1 << break_order_sixredsnooker[_nextcolor]);
            if (writeLog) { _LogInfo("6RED: Objective is: " + sixRedNumberToColor(_nextcolor, true)); }
        }
        else
        {
            if (writeLog) { _LogInfo("6RED: Objective is: Red"); }
        }
        if (includeFreeBall && foulStateLocal == 5) // add freeball to objective
        {
            objective = objective | 1u << firstHit;
        }
        return objective;
    }

    public int findLowestUnpocketedBall(uint field)
    {
        for (int i = 2; i <= 8; i++)
        {
            if (((field >> i) & 0x1U) == 0x00U)
                return i;
        }

        if (((field) & 0x2U) == 0x00U)
            return 1;

#if EIJIS_MANY_BALLS
        for (int i = 9; i < MAX_BALLS; i++)
#else
        for (int i = 9; i < 16; i++)
#endif
        {
            if (((field >> i) & 0x1U) == 0x00U)
                return i;
        }

        // ??
        return 0;
    }

#if EIJIS_SEMIAUTOCALL
    public int findNearestPocketFromBall(int ballId)
    {
        int pocketId = -1;
        Vector3 ballPos = ballsP[ballId];
        float abs_x = Mathf.Abs(ballPos.x);
        float abs_z_n = Mathf.Abs(ballPos.z) * findNearestPocket_n;
        bool nearSide = (abs_x + abs_z_n < findNearestPocket_x);
        if (ballPos.z < 0)
        {
            if (nearSide)
            {
                pocketId = 5;
            }
            else if (ballPos.x < 0)
            {
                pocketId = 3;
            }
            else
            {
                pocketId = 1;
            }
        }
        else
        {
            if (nearSide)
            {
                pocketId = 4;
            }
            else if (ballPos.x < 0)
            {
                pocketId = 2;
            }
            else
            {
                pocketId = 0;
            }
        }

        return pocketId;
    }

    public uint findEasiestBallAndPocket(uint field)
    {
        Vector3[][] matrix = new Vector3[16][];
        for (int i = 0; i < matrix.Length; i++)
        {
            matrix[i] = new Vector3[pocketLocations.Length];
            for (int j = 0; j < matrix[i].Length; j++)
            {
                matrix[i][j] = Vector3.positiveInfinity;
            }
        }

        int[] matrixIndexPosListByCondition = new int[findEasiestBallAndPocketConditions.Length];
        int[][] matrixIndexListByCondition = new int[findEasiestBallAndPocketConditions.Length][];
        for (int i = 0; i < matrixIndexListByCondition.Length; i++)
        {
            matrixIndexPosListByCondition[i] = 0;
            matrixIndexListByCondition[i] = new int[matrix.Length * matrix[0].Length];
            for (int j = 0; j < matrixIndexListByCondition[i].Length; j++)
            {
                matrixIndexListByCondition[i][j] = -1;
            }
        }

        for (int i = 1; i < 16; i++)
        {
            if (((field >> i) & 0x1U) == 0x1U)
            {
                continue;
            }

            Vector3 cue2target = ballsP[i] - ballsP[0];
            float c2tRad = -Mathf.Atan2(cue2target.z, cue2target.x);
            float c2tDeg = c2tRad * Mathf.Rad2Deg;
            float c2tSqrMagnitude = Vector3.SqrMagnitude(cue2target);

            for (int j = 0; j < pocketLocations.Length; j++)
            {
                Vector3 target2pocket = pocketLocations[j] - ballsP[i];
                float t2pRad = -Mathf.Atan2(target2pocket.z, target2pocket.x);
                float t2pDeg = t2pRad * Mathf.Rad2Deg;
                float t2pSqrMagnitude = Vector3.SqrMagnitude(target2pocket);
#if EIJIS_DEBUG_SEMIAUTO_CALL_SIDE
                if (debugLogFlg && 4 <= j) _LogInfo($"  t{i} to side{j} t2pDeg = {t2pDeg}");
#endif
                if (4 <= j && ((t2pDeg < 0 &&(t2pDeg < -135 || -45 < t2pDeg)) || (0 <= t2pDeg &&(t2pDeg < 45 || 135 < t2pDeg))))
                {
#if EIJIS_DEBUG_SEMIAUTO_CALL_SIDE
                    if (debugLogFlg) _LogInfo($"  side pocket skip");
#endif
                    continue;
                }

                float degDiff = c2tDeg - t2pDeg;
                if (degDiff < 0)
                {
                    degDiff = -degDiff;
                }
                if (180 < degDiff)
                {
                    degDiff = 360 - degDiff;
                }
                
                // x:deg, y:t2p, z:c2t
                Vector3 p = matrix[i][j] = new Vector3(degDiff, t2pSqrMagnitude, c2tSqrMagnitude);
#if EIJIS_DEBUG_SEMIAUTO_CALL_FINDLOGIC
                // if (debugLogFlg) _LogInfo($"  matrix[{i}][{j}]] = {p.x}, {p.y}, {p.z}");
#endif

                 for (int k = 0; k < findEasiestBallAndPocketConditions.Length; k++)
                {
                    Vector3 c = findEasiestBallAndPocketConditions[k];
                    if (p.x < c.x && p.y < c.y && p.z < c.z)
                    {
                        matrixIndexListByCondition[k][matrixIndexPosListByCondition[k]++] = (i * 16) + j;
#if EIJIS_DEBUG_SEMIAUTO_CALL_FINDLOGIC
                        // if (debugLogFlg) _LogInfo($"    k = {k},  (i * 16) + j = {matrixIndexListByCondition[k][matrixIndexPosListByCondition[k]-1]}");
#endif
                        break;
                    }
                }
            }
        }

#if EIJIS_DEBUG_SEMIAUTO_CALL_FINDLOGIC
        if (debugLogFlg)
        {
            for (int i = 0; i < matrixIndexPosListByCondition.Length; i++)
            {
                _LogInfo($"  matrixIndexPosListByCondition[{i}] = {matrixIndexPosListByCondition[i]}");
                for (int j = 0; j < matrixIndexListByCondition[i].Length; j++)
                {
                    if (matrixIndexListByCondition[i][j] < 0) break;
                    _LogInfo($"  matrixIndexListByCondition[{i}][{j}] = {matrixIndexListByCondition[i][j]}");
                }
            }
        }
#endif

        int easiestBallId = -1;
        int easiestPocketId = -1;
        for (int i = 0; i < matrixIndexListByCondition.Length; i++)
        {
            int nearestBallId = -1;
            int nearestPocketId = -1;
            float minSqrMagnitude = float.MaxValue;
            for (int j = 0; j < matrixIndexListByCondition[i].Length; j++)
            {
                int materixIndex = matrixIndexListByCondition[i][j];
#if EIJIS_DEBUG_SEMIAUTO_CALL_FINDLOGIC
                // if (debugLogFlg) _LogInfo($"  materixIndex = {materixIndex}, matrixIndexListByCondition[{i}][{j}]] = {matrixIndexListByCondition[i][j]}");
#endif
                if (materixIndex < 0 || matrixIndexPosListByCondition[i] <= j)
                {
                    continue;
                }

                int ballId = materixIndex / 16;
                int pocketId = materixIndex % 16;
                Vector3 p = matrix[ballId][pocketId];
#if EIJIS_DEBUG_SEMIAUTO_CALL_FINDLOGIC
                if (debugLogFlg) _LogInfo($"  materixIndex = {materixIndex}, matrixIndexListByCondition[{i}][{j}]] = {matrixIndexListByCondition[i][j]}");
                if (debugLogFlg) _LogInfo($"  matrix[ballId = {ballId}][pocketId = {pocketId}]] = {p.x}, {p.y}, {p.z}");
#endif
                
                float sqrMagnitude = p.z;
                if (0 <= sqrMagnitude && sqrMagnitude < minSqrMagnitude)
                {
                    minSqrMagnitude = sqrMagnitude;
                    nearestBallId = ballId;
                    nearestPocketId = pocketId;
                }
            }

            if (0 <= nearestBallId && 0 <= nearestPocketId)
            {
                easiestBallId = nearestBallId;
                easiestPocketId = nearestPocketId;
                break;
            }
        }
        
#if EIJIS_DEBUG_SEMIAUTO_CALL_FINDLOGIC
        if (debugLogFlg) _LogInfo($"  easiestBallId = {easiestBallId}, easiestPocketId = {easiestPocketId}");
#endif

        return  ((easiestPocketId < 0 ? 0xFFFFu : (uint)easiestPocketId) << 16) | (easiestBallId < 0 ? 0xFFFFu : (uint)easiestBallId);
    }
    
#endif
#if UNITY_EDITOR
    public void DBG_DrawBallMask(uint ballMask)
    {
#if EIJIS_MANY_BALLS
        for (int i = 0; i < MAX_BALLS; i++)
#else
        for (int i = 0; i < 16; i++)
#endif
        {
            if ((ballsPocketedLocal & (1 << i)) > 0) { continue; }
            if ((ballMask & (1 << i)) == 0) { continue; }
            Debug.DrawRay(balls[0].transform.parent.TransformPoint(ballsP[i]), Vector3.up * .3f, Color.white, 3f);
        }
    }

    public void DBG_TestObjVisible()
    {
        uint redmask = 0;
#if EIJIS_SNOOKER15REDS
        for (int i = 0; i < SNOOKER_REDS_COUNT; i++)
#else
        for (int i = 0; i < 6; i++)
#endif
        {
            redmask += ((uint)1 << break_order_sixredsnooker[i]);
        }
        // DBG_DrawBallMask(redmask);
        switch (objVisible(redmask))
        {
            case 0:
                _LogInfo("A Red ball CAN be seen");
                break;
            case 1:
                _LogInfo("A Red ball can be seen on ONE side");
                break;
            case 2:
                _LogInfo("A Red ball can NOT be seen");
                break;
        }
    }
#endif

#if EIJIS_MANY_BALLS
    int[] objVisible_blockingBalls = new int[MAX_BALLS * 2];
#else
    int[] objVisible_blockingBalls = new int[32];
#endif
    int objVisible_blockingBalls_len;
    int objVisible(uint objMask)
    {
        int mostVisible = 2;
#if EIJIS_MANY_BALLS
        objVisible_blockingBalls = new int[MAX_BALLS * 2];
        for (int i = 0; i < (MAX_BALLS * 2); i++) objVisible_blockingBalls[i] = -1;
#else
        objVisible_blockingBalls = new int[32];
        for (int i = 0; i < 32; i++) objVisible_blockingBalls[i] = -1;
#endif
        objVisible_blockingBalls_len = 0;
#if EIJIS_MANY_BALLS
        for (int i = 0; i < MAX_BALLS; i++)
#else
        for (int i = 0; i < 16; i++)
#endif
        {
            if ((objMask & (1 << i)) > 0)
            {
                int ballvis = ballBlocked(0, i, true);
                // if (ballvis == 1)
                // { Debug.DrawRay(balls[0].transform.parent.TransformPoint(ballsP[i]), Vector3.up * .3f, Color.red, 3f); }
                // if (ballvis == 0)
                // { Debug.DrawRay(balls[0].transform.parent.TransformPoint(ballsP[i]), Vector3.up * .3f, Color.white, 3f); }
                if (mostVisible > ballvis)
                {
                    mostVisible = ballvis;
                }
                if (mostVisible == 0)
                {
                    break;
                }
                objVisible_blockingBalls[objVisible_blockingBalls_len] = ballBlocked_blockingBalls[0];
                objVisible_blockingBalls_len++;
                objVisible_blockingBalls[objVisible_blockingBalls_len] = ballBlocked_blockingBalls[1];
                objVisible_blockingBalls_len++;
            }
        }
        return mostVisible;
    }
    int[] ballBlocked_blockingBalls = new int[2];
    int ballBlocked(int from, int to, bool ignoreReds)
    {
        ballBlocked_blockingBalls = new int[2] { -1, -1 };
        Vector3 center = (ballsP[from] + ballsP[to]) / 2;
        float cenMag = (ballsP[from] - center).magnitude;

        Vector2 out1 = Vector3.zero, out2 = Vector3.zero, out3 = Vector3.zero, out4 = Vector3.zero,
            circle1, circle2, center2;
        circle1 = new Vector2(ballsP[from].x, ballsP[from].z);
        circle2 = new Vector2(ballsP[to].x, ballsP[to].z);
        // float Ball1Rad = k_BALL_RADIUS;
        // float Ball2Rad = k_BALL_RADIUS;
        center2 = new Vector2(center.x, center.z);

        FindCircleCircleIntersections(center2, cenMag, circle1, k_BALL_DIAMETRE /* Ball1Rad + Ball2Rad */, out out1, out out2);
        FindCircleCircleIntersections(center2, cenMag, circle2, k_BALL_DIAMETRE /* Ball1Rad + Ball2Rad */, out out3, out out4);

        Vector3 ipoint1 = new Vector3(out1.x, ballsP[from].y, out1.y);
        Vector3 ipoint2 = new Vector3(out2.x, ballsP[from].y, out2.y);
        Vector3 ipoint3 = new Vector3(out3.x, ballsP[from].y, out3.y);
        Vector3 ipoint4 = new Vector3(out4.x, ballsP[from].y, out4.y);

        Vector3 innerTanPoint1 = ballsP[from] + (ipoint1 - ballsP[from]).normalized * k_BALL_RADIUS /* Ball1Rad */;
        Vector3 innerTanPoint2 = ballsP[from] + (ipoint2 - ballsP[from]).normalized * k_BALL_RADIUS /* Ball1Rad */;
        Vector3 innerTanPoint3 = ballsP[to] + (ipoint3 - ballsP[to]).normalized * k_BALL_RADIUS /* Ball2Rad */;
        Vector3 innerTanPoint4 = ballsP[to] + (ipoint4 - ballsP[to]).normalized * k_BALL_RADIUS /* Ball2Rad */;

        Vector3 innerTanPoint1_oposite = innerTanPoint1 - ballsP[from];
        innerTanPoint1_oposite = ballsP[from] - innerTanPoint1_oposite;
        Vector3 innerTanPoint2_oposite = innerTanPoint2 - ballsP[from];
        innerTanPoint2_oposite = ballsP[from] - innerTanPoint2_oposite;

        // Debug.DrawRay(balls[0].transform.parent.TransformPoint(innerTanPoint1), balls[0].transform.parent.TransformDirection(innerTanPoint3 - innerTanPoint1), Color.red, 10);
        // Debug.DrawRay(balls[0].transform.parent.TransformPoint(innerTanPoint2), balls[0].transform.parent.TransformDirection(innerTanPoint4 - innerTanPoint2), Color.blue, 10);
        // Debug.DrawRay(balls[0].transform.parent.TransformPoint(innerTanPoint2_oposite), balls[0].transform.parent.TransformDirection(innerTanPoint4 - innerTanPoint2), Color.blue, 10);
        // Debug.DrawRay(balls[0].transform.parent.TransformPoint(innerTanPoint1_oposite), balls[0].transform.parent.TransformDirection(innerTanPoint3 - innerTanPoint1), Color.red, 10);

        float NearestBlockL = float.MaxValue;
        float NearestBlockR = float.MaxValue;

        float distTo = (ballsP[from] - ballsP[to]).magnitude;
        bool blockedLeft = false;
        bool blockedRight = false;
        // left
#if EIJIS_MANY_BALLS
        for (int i = 0; i < MAX_BALLS; i++)
#else
        for (int i = 0; i < 16; i++)
#endif
        {
            if (i == from) { continue; }
            if (i == to) { continue; }
            if ((0x1U << i & ballsPocketedLocal) != 0U) { continue; }
            if (ignoreReds && sixredsnooker_ballpoints[i] == 1) { continue; }
            float distToThis = (ballsP[from] - ballsP[i]).magnitude;
            if (distToThis > distTo) { continue; }
            if (_phy_ray_sphere(innerTanPoint1, innerTanPoint3 - innerTanPoint1, ballsP[i]))
            {
                blockedLeft = true;
                if (NearestBlockL > distToThis)
                { NearestBlockL = distToThis; }
                ballBlocked_blockingBalls[0] = i;
            }
        }
        // right
#if EIJIS_MANY_BALLS
        for (int i = 0; i < MAX_BALLS; i++)
#else
        for (int i = 0; i < 16; i++)
#endif
        {
            if (i == from) { continue; }
            if (i == to) { continue; }
            if ((0x1U << i & ballsPocketedLocal) != 0U) { continue; }
            if (ignoreReds && sixredsnooker_ballpoints[i] == 1) { continue; }
            float distToThis = (ballsP[from] - ballsP[i]).magnitude;
            if (distToThis > distTo) { continue; }
            if (_phy_ray_sphere(innerTanPoint2, innerTanPoint4 - innerTanPoint2, ballsP[i]))
            {
                blockedRight = true;
                if (NearestBlockR > distToThis)
                { NearestBlockR = distToThis; }
                ballBlocked_blockingBalls[1] = i;
            }
        }
        // right + ball width
        if (!blockedRight)
        {
#if EIJIS_MANY_BALLS
            for (int i = 0; i < MAX_BALLS; i++)
#else
            for (int i = 0; i < 16; i++)
#endif
            {
                if (i == from) { continue; }
                if (i == to) { continue; }
                if ((0x1U << i & ballsPocketedLocal) != 0U) { continue; }
                if (ignoreReds && sixredsnooker_ballpoints[i] == 1) { continue; }
                float distToThis = (ballsP[from] - ballsP[i]).magnitude;
                if (distToThis > distTo) { continue; }
                if (_phy_ray_sphere(innerTanPoint2_oposite, innerTanPoint4 - innerTanPoint2, ballsP[i]))
                {
                    blockedRight = true;
                    if (NearestBlockR > distToThis)
                    { NearestBlockR = distToThis; }
                    ballBlocked_blockingBalls[1] = i;
                }
            }
        }
        // left + ball width
        if (!blockedLeft)
        {
#if EIJIS_MANY_BALLS
            for (int i = 0; i < MAX_BALLS; i++)
#else
            for (int i = 0; i < 16; i++)
#endif
            {
                if (i == from) { continue; }
                if (i == to) { continue; }
                if ((0x1U << i & ballsPocketedLocal) != 0U) { continue; }
                if (ignoreReds && sixredsnooker_ballpoints[i] == 1) { continue; }
                float distToThis = (ballsP[from] - ballsP[i]).magnitude;
                if (distToThis > distTo) { continue; }
                if (_phy_ray_sphere(innerTanPoint1_oposite, innerTanPoint3 - innerTanPoint1, ballsP[i]))
                {
                    blockedLeft = true;
                    if (NearestBlockL > distToThis)
                    { NearestBlockL = distToThis; }
                    ballBlocked_blockingBalls[0] = i;
                }
            }
        }
        // 0 = fully visible, 1 = left OR right blocked, 2 = both blocked
        int blockedLeft_i = blockedLeft ? 1 : 0;
        int blockedRight_i = blockedRight ? 1 : 0;
        return blockedLeft_i + blockedRight_i;
    }

    // Found on Unity Forums. Thanks to QuincyC.
    // Find the points where the two circles intersect.
    private void FindCircleCircleIntersections(Vector2 c0, float r0, Vector2 c1, float r1, out Vector2 intersection1, out Vector2 intersection2)
    {
        // Find the distance between the centers.
        float dx = c0.x - c1.x;
        float dy = c0.y - c1.y;
        float dist = Mathf.Sqrt(dx * dx + dy * dy);

        if (Mathf.Abs(dist - (r0 + r1)) < 0.00001)
        {
            intersection1 = Vector2.Lerp(c0, c1, r0 / (r0 + r1));
            intersection2 = intersection1;
        }

        // See how many solutions there are.
        if (dist > r0 + r1)
        {
            // No solutions, the circles are too far apart.
            intersection1 = new Vector2(float.NaN, float.NaN);
            intersection2 = new Vector2(float.NaN, float.NaN);
        }
        else if (dist < Mathf.Abs(r0 - r1))
        {
            // No solutions, one circle contains the other.
            intersection1 = new Vector2(float.NaN, float.NaN);
            intersection2 = new Vector2(float.NaN, float.NaN);
        }
        else if ((dist == 0) && (r0 == r1))
        {
            // No solutions, the circles coincide.
            intersection1 = new Vector2(float.NaN, float.NaN);
            intersection2 = new Vector2(float.NaN, float.NaN);
        }
        else
        {
            // Find a and h.
            float a = (r0 * r0 -
                        r1 * r1 + dist * dist) / (2 * dist);
            float h = Mathf.Sqrt(r0 * r0 - a * a);

            // Find P2.
            float cx2 = c0.x + a * (c1.x - c0.x) / dist;
            float cy2 = c0.y + a * (c1.y - c0.y) / dist;

            // Get the points P3.
            intersection1 = new Vector2(
                (float)(cx2 + h * (c1.y - c0.y) / dist),
                (float)(cy2 - h * (c1.x - c0.x) / dist));
            intersection2 = new Vector2(
                (float)(cx2 - h * (c1.y - c0.y) / dist),
                (float)(cy2 + h * (c1.x - c0.x) / dist));

        }
    }

    //copy of method from StandardPhysicsManager
    bool _phy_ray_sphere(Vector3 start, Vector3 dir, Vector3 sphere)
    {
        float k_BALL_RSQR = k_BALL_RADIUS * k_BALL_RADIUS;
        Vector3 nrm = dir.normalized;
        Vector3 h = sphere - start;
        float lf = Vector3.Dot(nrm, h);
        float s = k_BALL_RSQR - Vector3.Dot(h, h) + lf * lf;

        if (s < 0.0f) return false;

        s = Mathf.Sqrt(s);

        if (lf < s)
        {
            if (lf + s >= 0)
            {
                s = -s;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    private void setBallPickupActive(int ballId, bool active)
    {
        Transform pickup = balls[ballId].transform.GetChild(0);

        pickup.gameObject.SetActive(active);
        pickup.GetComponent<SphereCollider>().enabled = active;
        ((VRC_Pickup)pickup.GetComponent(typeof(VRC_Pickup))).pickupable = active;
        if (!active) ((VRC_Pickup)pickup.GetComponent(typeof(VRC_Pickup))).Drop();
    }

    private void refreshBallPickups()
    {
        bool canUsePickup = isMyTurn() && isPracticeMode && gameLive;

        uint ball_bit = 0x1u;
        for (int i = 0; i < balls.Length; i++)
        {
            if ((canUsePickup || (i == 0 && isReposition)) && gameLive && canPlayLocal && (ballsPocketedLocal & ball_bit) == 0x0u)
            {
                setBallPickupActive(i, true);
            }
            else
            {
                setBallPickupActive(i, false);
            }
            ball_bit <<= 1;
        }
    }

    private void setFoulPickupEnabled(bool enabled)
    {
        markerObj.SetActive(enabled);
        if (enabled)
        {
            setBallPickupActive(0, true);
        }
        else if (!isPracticeMode)
        {
            setBallPickupActive(0, false);
        }
    }

    private void tickTimer()
    {
        if (gameLive && timerRunning && canPlayLocal)
        {
            float timeRemaining = timerLocal - (Networking.GetServerTimeInMilliseconds() - timerStartLocal) / 1000.0f;
            float timePercentage = timeRemaining >= 0.0f ? 1.0f - (timeRemaining / timerLocal) : 0.0f;

            if (!localPlayerDistant)
            {
                graphicsManager._SetTimerPercentage(timePercentage);
            }

            if (timeRemaining < 0.0f)
            {
                onLocalTimerEnd();
            }
        }
    }

#if EIJIS_SEMIAUTOCALL
    private void tickSemiAutoCall()
    {
#if EIJIS_DEBUG_SEMIAUTO_CALL
        // if (debugFlg) return;
#endif
        if (!isMyTurn() && 0 < semiAutoCalledTimeBall)
        {
            semiAutoCalledTimeBall = 0;
            return;
        }
        
#if EIJIS_DEBUG_SEMIAUTO_CALL_AFTER_REPOSITION
        if (debugLogFlg) _LogInfo($"TKCH EIJIS_DEBUG_SEMIAUTO_CALL_AFTER_REPOSITION tickSemiAutoCall() cueBallFixed = {cueBallFixed}");
#endif

        bool isOnBreakShot = colorTurnLocal;
#if EIJIS_10BALL || EIJIS_ROTATION || EIJIS_BOWLARDS
        if ((is8Ball || is10Ball || isRotation || isBowlards) && gameLive && canPlayLocal && isMyTurn() && !isOnBreakShot && requireCallShotLocal && cueBallFixed)
#else
        if (is8Ball && gameLive && canPlayLocal && isMyTurn() && !isOnBreakShot && requireCallShotLocal && !markerObj.activeSelf)
#endif
        {
#if EIJIS_DEBUG_SEMIAUTO_CALL_FINDLOGIC
            // if (debugLogFlg) _LogInfo($"TKCH SEMIAUTO_CALL semiAutoCallBall = {semiAutoCallBallLocal}, semiAutoCalledTimeBall = {semiAutoCalledTimeBall}, calledBallId = {calledBallId}");
            // if (debugLogFlg) _LogInfo($"                   semiAutoCallPocket = {semiAutoCallPocketLocal}, semiAutoCalledPocket = {semiAutoCalledPocket}, calledPocketId = {calledPocketId}, calledBalls = {calledBallsLocal:X4}");
            if (debugLogFlg) _LogInfo($"TKCH SEMIAUTO_CALL                    semiAutoCalledTimeBall = {semiAutoCalledTimeBall}, calledBallId = {calledBallId}");
            if (debugLogFlg) _LogInfo($"  semiAutoCall = {semiAutoCallLocal}, semiAutoCalledPocket = {semiAutoCalledPocket}, calledPocketId = {calledPocketId}, calledBalls = {calledBallsLocal:X4}");
#endif
            int target = -1;
            int pocketId = -1;

#if EIJIS_DEBUG_SEMIAUTO_CALL_AFTER_REPOSITION
            if (debugLogFlg) _LogInfo($"TKCH EIJIS_DEBUG_SEMIAUTO_CALL_AFTER_REPOSITION tickSemiAutoCall() semiAutoCalledTimeBall = {semiAutoCalledTimeBall}");
#endif
            if (semiAutoCallLocal && ((semiAutoCalledTimeBall <= 0 && calledBallId < 0) ||
                                      (!semiAutoCalledPocket && calledPocketId < 0 && 0 < calledBallsLocal)))
            {
#if EIJIS_BOWLARDS
                uint findBalls = ballsPocketedLocal | denyBalls;
#else
                uint findBalls = ballsPocketedLocal;
#endif
#if EIJIS_10BALL || EIJIS_ROTATION
                if (is10Ball || isRotation)
                {
                    findBalls = ~(0x1u << findLowestUnpocketedBall(ballsPocketedLocal));
                }
                else if (is8Ball)
#else
                if (is8Ball)
#endif
                {
                    if (isTableOpenLocal)
                    {
                        findBalls = ballsPocketedLocal | ~0xFFFFFFFCu;
                    }
                    else
                    {
                        uint bmask = (0x1FCu << ((int)(teamIdLocal ^ teamColorLocal) * 7));
                        bool isSetComplete = (ballsPocketedLocal & bmask) == bmask;
#if EIJIS_DEBUG_SEMIAUTO_CALL_FINDLOGIC
                        if (debugLogFlg) _LogInfo($"  teamIdLocal = {teamIdLocal}, teamColorLocal = {teamColorLocal}, ^ = {teamIdLocal ^ teamColorLocal}");
                        if (debugLogFlg) _LogInfo($"  findBalls = {ballsPocketedLocal:X4}");
#endif
                        findBalls = isSetComplete ? ~0x2u : ballsPocketedLocal | ~(0xFFFF0000 | bmask);
#if EIJIS_DEBUG_SEMIAUTO_CALL_FINDLOGIC
                        if (debugLogFlg) _LogInfo($"  findBalls = {findBalls:X4}");
#endif
                    }
                }
                uint pocketAndBall = findEasiestBallAndPocket(findBalls);
                if (pocketAndBall != 0xFFFFFFFF)
                {
                    target = (int)(pocketAndBall & 0xFFFFu);
                    pocketId = (int)((pocketAndBall >> 16) & 0xFFFFu);
                }
            }
            
#if EIJIS_DEBUG_SEMIAUTO_CALL_FINDLOGIC || EIJIS_DEBUG_NEXT_BREAK
            if (debugLogFlg) _LogInfo($"  target(final) = {target}");
#endif
#if EIJIS_DEBUG_SEMIAUTO_CALL_FINDLOGIC || EIJIS_DEBUG_SEMIAUTO_CALL_SIDE || EIJIS_DEBUG_NEXT_BREAK || EIJIS_DEBUG_SEMIAUTO_CALL_AFTER_REPOSITION
            debugLogFlg = false;
            // debugLogFlg = true;
#endif
            
            if (0 < target)
            {
                float elapsedSeconds = (Networking.GetServerTimeInMilliseconds() - semiAutoCallDelayBase) / 1000.0f;
                
#if EIJIS_DEBUG_SEMIAUTO_CALL_FINDLOGIC
                if (0.1f < elapsedSeconds && elapsedSeconds < 0.12f)
                {
                    // _LogInfo($"  semiAutoCalledBall = {semiAutoCalledBall}, calledBallId = {calledBallId}, target = {target}");
                    _LogInfo($"  semiAutoCalledTimeBall = {semiAutoCalledTimeBall}, calledBallId = {calledBallId}, target = {target}");
                }
#endif

                if (semiAutoCallLocal && semiAutoCalledTimeBall <= 0 && calledBallId < 0)
                {
#if EIJIS_DEBUG_SEMIAUTO_CALL
                    _LogInfo($"  elapsedSeconds = {elapsedSeconds}");
#endif
                    if (semiAutoCallDelay < elapsedSeconds)
                    {
#if EIJIS_DEBUG_SEMIAUTO_CALL
                        _LogInfo($"  elapsedSeconds = {elapsedSeconds}, call _TriggerOtherBallHit(target = {target})");
#endif
                        _TriggerOtherBallHit(target, true);
                        semiAutoCalledTimeBall = semiAutoCallDelay;
                    }
                }
            }
                
            if (semiAutoCallLocal && !semiAutoCalledPocket && calledPocketId < 0 && 0 < calledBallsLocal)
            {
                float elapsedSeconds = (Networking.GetServerTimeInMilliseconds() - semiAutoCallDelayBase) / 1000.0f;
#if EIJIS_DEBUG_SEMIAUTO_CALL
                _LogInfo($"  elapsedSeconds = {elapsedSeconds}");
#endif
                if (semiAutoCallDelay + semiAutoCalledTimeBall < elapsedSeconds)
                {
#if EIJIS_DEBUG_SEMIAUTO_CALL
                    _LogInfo($"  elapsedSeconds = {elapsedSeconds}, call _TriggerPocketHit(pocketId = {pocketId}, TRUE)");
#endif
                    _TriggerPocketHit(pocketId, true);
                    semiAutoCalledPocket = true;
#if EIJIS_DEBUG_SEMIAUTO_CALL
                    // debugFlg = false;
#endif
                }
            }
            
#if EIJIS_DEBUG_SEMIAUTO_CALL_FINDLOGIC || EIJIS_DEBUG_SEMIAUTO_CALL_SIDE || EIJIS_DEBUG_NEXT_BREAK || EIJIS_DEBUG_SEMIAUTO_CALL_AFTER_REPOSITION
            debugLogFlg = false;
#endif
        }
#if EIJIS_DEBUG_SEMIAUTO_CALL_FINDLOGIC || EIJIS_DEBUG_SEMIAUTO_CALL_SIDE || EIJIS_DEBUG_NEXT_BREAK || EIJIS_DEBUG_SEMIAUTO_CALL_AFTER_REPOSITION
        else if (debugLogFlg)
        {
            float debugLogSeconds = (Networking.GetServerTimeInMilliseconds() - debugLogStart) / 1000.0f;
            // _LogInfo($"TKCH EIJIS_DEBUG_SEMIAUTO_CALL_AFTER_REPOSITION tickSemiAutoCall() debugLogSeconds = {debugLogSeconds}");
            if (0.3f < debugLogSeconds)
            {
                debugLogFlg = false;
            }
        }
#endif
    }

#endif
    public bool isMyTurn()
    {
        return localPlayerId >= 0 && (localTeamId == teamIdLocal || (isPracticeMode && isPlayer));
    }

    public bool _AllPlayersOffline()
    {
        for (int i = 0; i < 4; i++)
        {
            if (playerIDsLocal[i] == -1) continue;

            VRCPlayerApi player = VRCPlayerApi.GetPlayerById(playerIDsLocal[i]);
            if (Utilities.IsValid(player))
            {
                return false;
            }
        }

        return true;
    }

#if EIJIS_ISSUE_FIX // 2本目のキュー座標がリセットされない場合がある （ issue #3 の修正）
    public bool _TeamPlayersOffline(uint teamId)
    {
        for (int i = 0; i < 4; i++)
        {
            if (teamId != (uint)(i & 0x1u)) continue;
            if (playerIDsLocal[i] == -1) continue;

            VRCPlayerApi player = VRCPlayerApi.GetPlayerById(playerIDsLocal[i]);
            if (Utilities.IsValid(player))
            {
                return false;
            }
        }

        return true;
    }

#endif
    public VRCPlayerApi _GetPlayerByName(string name)
    {
        VRCPlayerApi[] onlinePlayers = VRCPlayerApi.GetPlayers(new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()]);
        for (int playerId = 0; playerId < onlinePlayers.Length; playerId++)
        {
            if (onlinePlayers[playerId].displayName == name)
            {
                return onlinePlayers[playerId];
            }
        }
        return null;
    }

    public void _IndicateError()
    {
        graphicsManager._FlashTableError();
    }

    public void _IndicateSuccess()
    {
        graphicsManager._FlashTableLight();
    }

    public string _SerializeGameState()
    {
        return networkingManager._EncodeGameState();
    }

    public void _LoadSerializedGameState(string gameState)
    {
        // no loading on top of other people's games
        if (!_IsPlayer(Networking.LocalPlayer)) return;

        networkingManager._OnLoadGameState(gameState);
        // practiceManager._Record();
    }

    public object[] _SerializeInMemoryState()
    {
        Vector3[] positionClone = new Vector3[ballsP.Length];
        Array.Copy(ballsP, positionClone, ballsP.Length);
        byte[] scoresClone = new byte[fbScoresLocal.Length];
        Array.Copy(fbScoresLocal, scoresClone, fbScoresLocal.Length);
#if EIJIS_ROTATION
        ushort[] totalPointsClone = new ushort[totalPointsLocal.Length];
        Array.Copy(totalPointsLocal, totalPointsClone, totalPointsLocal.Length);
        ushort[] highRunsClone = new ushort[highRunsLocal.Length];
        Array.Copy(highRunsLocal, highRunsClone, highRunsLocal.Length);
        ushort[] chainedPointsClone = new ushort[chainedPointsLocal.Length];
        Array.Copy(chainedPointsLocal, chainedPointsClone, chainedPointsLocal.Length);
        byte[] chainedFoulsClone = new byte[chainedFoulsLocal.Length];
        Array.Copy(chainedFoulsLocal, chainedFoulsClone, chainedFoulsLocal.Length);
#endif
#if EIJIS_BOWLARDS
        ushort[] framePointsClone = new ushort[framePointsLocal.Length];
        Array.Copy(framePointsLocal, framePointsClone, framePointsLocal.Length);
        ushort[] framePointsClone2 = new ushort[framePointsLocal2.Length];
        Array.Copy(framePointsLocal2, framePointsClone2, framePointsLocal2.Length);
#endif
#if EIJIS_PUSHOUT || EIJIS_CALLSHOT || EIJIS_ROTATION || EIJIS_BOWLARDS
        return new object[24]
#else
        return new object[13]
#endif
        {
            positionClone, ballsPocketedLocal, scoresClone, gameModeLocal, teamIdLocal, foulStateLocal, isTableOpenLocal, teamColorLocal, fourBallCueBallLocal,
            turnStateLocal, networkingManager.cueBallVSynced, networkingManager.cueBallWSynced, colorTurnLocal
#if EIJIS_PUSHOUT
            , pushOutStateLocal
#endif
#if EIJIS_CALLSHOT
            , pointPocketsLocal, calledBallsLocal
#endif
#if EIJIS_ROTATION
            , inningCountLocal ,nextBallRepositionStateLocal, totalPointsClone, highRunsClone, chainedPointsClone, chainedFoulsClone
#endif
#if EIJIS_BOWLARDS
            , framePointsClone, framePointsClone2
#endif
        };
    }

    public void _LoadInMemoryState(object[] state, int stateIdLocal)
    {
        networkingManager._ForceLoadFromState(
            stateIdLocal,
            (Vector3[])state[0], (uint)state[1], (byte[])state[2], (uint)state[3], (uint)state[4], (uint)state[5], (bool)state[6], (uint)state[7], (uint)state[8],
            (byte)state[9], (Vector3)state[10], (Vector3)state[11], (bool)state[12]
#if EIJIS_PUSHOUT
            , (byte)state[13]
#endif
#if EIJIS_CALLSHOT
            , (uint)state[14], (uint)state[15]
#endif
#if EIJIS_ROTATION
            , (int)state[16], (uint)state[17], (ushort[])state[18], (ushort[])state[19], (ushort[])state[20], (byte[])state[21]
#endif
#if EIJIS_BOWLARDS
            , (ushort[])state[22], (ushort[])state[23]
#endif
        );
    }

    public bool _AreInMemoryStatesEqual(object[] a, object[] b)
    {
        Vector3[] posA = (Vector3[])a[0];
        Vector3[] posB = (Vector3[])b[0];
        for (int i = 0; i < ballsP.Length; i++) if (posA[i] != posB[i]) return false;

        byte[] scoresA = (byte[])a[2];
        byte[] scoresB = (byte[])b[2];
        for (byte i = 0; i < fbScoresLocal.Length; i++) if (scoresA[i] != scoresB[i]) return false;

#if EIJIS_ROTATION
        ushort[] pointsA = (ushort[])a[18];
        ushort[] pointsB = (ushort[])b[18];
        for (int i = 0; i < totalPointsLocal.Length; i++) if (pointsA[i] != pointsB[i]) return false;

        pointsA = (ushort[])a[19];
        pointsB = (ushort[])b[19];
        for (int i = 0; i < highRunsLocal.Length; i++) if (pointsA[i] != pointsB[i]) return false;

        pointsA = (ushort[])a[20];
        pointsB = (ushort[])b[20];
        for (int i = 0; i < chainedPointsLocal.Length; i++) if (pointsA[i] != pointsB[i]) return false;

        byte[] countA = (byte[])a[21];
        byte[] countB = (byte[])b[21];
        for (int i = 0; i < chainedFoulsLocal.Length; i++) if (countA[i] != countB[i]) return false;

#endif
#if EIJIS_BOWLARDS
        pointsA = (ushort[])a[22];
        pointsB = (ushort[])b[22];
        for (int i = 0; i < framePointsLocal.Length; i++) if (pointsA[i] != pointsB[i]) return false;

        pointsA = (ushort[])a[23];
        pointsB = (ushort[])b[23];
        for (int i = 0; i < framePointsLocal2.Length; i++) if (pointsA[i] != pointsB[i]) return false;

#endif
#if EIJIS_ROTATION
        for (byte i = 0; i < a.Length; i++) if (i != 0 && i != 2 && i < 18 && !a[i].Equals(b[i])) return false;
#else
        for (byte i = 0; i < a.Length; i++) if (i != 0 && i != 2 && !a[i].Equals(b[i])) return false;
#endif

        return true;
    }

    public bool _IsModerator(VRCPlayerApi player)
    {
        return Array.IndexOf(moderators, player.displayName) != -1;
    }

    public int _GetPlayerSlot(VRCPlayerApi who, int[] playerlist)
    {
        if (who == null) return -1;

        for (int i = 0; i < 4; i++)
        {
            if (playerlist[i] == who.playerId)
            {
                return i;
            }
        }

        return -1;
    }

    public bool _IsPlayer(VRCPlayerApi who)
    {
        if (who == null) return false;
        if (who.isLocal && localPlayerId >= 0) return true;

        for (int i = 0; i < 4; i++)
        {
            if (playerIDsLocal[i] == who.playerId)
            {
                return true;
            }
        }

        return false;
    }

    private bool stringArrayEquals(string[] a, string[] b)
    {
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return true;
    }

    private bool intArrayEquals(int[] a, int[] b)
    {
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return true;
    }

    private bool vector3ArrayEquals(Vector3[] a, Vector3[] b)
    {
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return true;
    }
#if EIJIS_SNOOKER15REDS

    public static uint SoftwareFallback(uint value)
    {
        const uint c1 = 0x55555555u;
        const uint c2 = 0x33333333u;
        const uint c3 = 0x0F0F0F0Fu;
        const uint c4 = 0x01010101u;

        value -= (value >> 1) & c1;
        value = (value & c2) + ((value >> 2) & c2);
        value = (((value + (value >> 4)) & c3) * c4) >> 24;

        return value;
    }
#endif
#if EIJIS_ROTATION

    private int pocketedballUponPool(uint ballsPocketed, float posX /* , int safeLimitLoop */)
    {
#if EIJIS_DEBUG_BALL_TOUCHING
        _LogInfo($"EIJIS_DEBUG BilliardsModule::pocketedballUponPool(ballsPocketed = {ballsPocketed:X4}, posX = {posX})");
#endif
        if (ballsPocketed == 0x0u)
        {                                                       
            return -1; // 0x0u;
        }

        uint uponBalls = 0x0u;
        int uponBallsCount = 0;
        uint uponRacks = 0x0u;
        for (int j = 0; j < pocketed_ball_upon_pool_order.Length; j++)
        {
            int i = pocketed_ball_upon_pool_order[j];
            uint ball_bit = 0x1u << i;
            if ((ballsPocketed & ball_bit) != 0x0u)
            {
#if EIJIS_DEBUG_BALL_TOUCHING
                _LogInfo($"EIJIS_DEBUG   i = {i}");
#endif
                Vector3 beforePos = ballsP[i];
                ballsP[i] = //Vector3.zero;
                new Vector3
                (
                    posX,
                    0.0f,
                    0.0f
                );
                uint touchCheckBalls = pocketMask | 0x1u;
                int touching = ballTouching(i, touchCheckBalls);
                int limit = 30; // safeLimitLoop;
                float adjustXprev = 0;
                while (0 <= touching)
                {
                    float distanceZ = ballsP[touching].z;
                    float adjustX = Mathf.Sqrt(Mathf.Pow(k_BALL_DIAMETRE, 2f) - Mathf.Pow(distanceZ, 2f));
#if EIJIS_DEBUG_BALL_TOUCHING
                    _LogInfo($"EIJIS_DEBUG   adjustX = {adjustX}");
#endif
                    ballsP[i] = new Vector3
                    (
                        ballsP[touching].x + adjustX,
                        ballsP[touching].y,
                        0
                    );
                    int touchingNew = ballTouching(i, touchCheckBalls);
                    if (touchingNew == touching && adjustXprev == adjustX)
                    {
                        touchCheckBalls &= ~(0x1u << touching);
                        continue;
                    }
                    
                    adjustXprev = adjustX;
                    touching = touchingNew;
                }

                if (footCushionTouching(ballsP[i]))
                {
                    ballsP[i] = beforePos;
                    continue;
                }
                
                uponBalls |= ball_bit;
                uponBallsCount++;

                // int rackPosNum = (int)((beforePos.x - k_rack_position.x) / k_BALL_DIAMETRE);
                // uponRacks |= 0x1u << rackPosNum;
            }
        }

        ballsPocketedLocal &= ~uponBalls;
        if (uponBallsCount <= 0)
        {
            _LogWarn("failed to upon " + (posX == 0 ? "center" : (posX == k_SPOT_POSITION_X ? "foot" : $"x = {posX}")) + ". no place to put.");
        }
        return uponBallsCount;
    }

    private int ballTouching(int n, uint checkTargetBalls)
    {
#if EIJIS_DEBUG_BALL_TOUCHING
        _LogInfo($"EIJIS_DEBUG BilliardsModule::ballTouching() n = {n}, checkTargetBalls = {checkTargetBalls:X4}");
#endif
        for (int i = 0; i < ballsP.Length; i++)
        {
            if (ballsP.Length <= n)
            {
                break;
            }
            if (i == n)
            {
                continue;
            }
            // if ((isRotation6Balls && (i == 1 || 7 < i)) || (!isRotation6Balls && ballsLengthByPocketGame < i))
            if ((checkTargetBalls & (0x1u << i)) == 0)
            {
#if EIJIS_DEBUG_BALL_TOUCHING
                // _LogInfo($"EIJIS_DEBUG   continue i = {i}");
#endif
                continue;
            }
            if ((ballsP[n] - ballsP[i]).sqrMagnitude < Mathf.Pow(k_BALL_DIAMETRE, 2f)) //k_BALL_DSQR)
            {
#if EIJIS_DEBUG_BALL_TOUCHING
                _LogInfo($"EIJIS_DEBUG   return i = {i}");
#endif
                return i;
            }
        }

#if EIJIS_DEBUG_BALL_TOUCHING
        _LogInfo("EIJIS_DEBUG   return -1");
#endif
        return -1;
    }

    private bool footCushionTouching(Vector3 ballPosition)
    {
        if (k_TABLE_WIDTH < ballPosition.x + k_BALL_RADIUS)
        {
            return true;
        }

        return false;
    }
#endif
#if EIJIS_CALLSHOT
    
    public bool CanShotCondition()
    {
        if (!requireCallShotLocal) return true;
#if EIJIS_10BALL && EIJIS_ROTATION || EIJIS_BOWLARDS
        if (!is8Ball && !is10Ball && !isRotation && !isBowlards) return true;
#else
        if (!is8Ball) return true;
#endif
        if (colorTurnLocal) return true; // if (!afterBreak) return true;
        if (safetyCalledLocal) return true;
        if (pushOutStateLocal == PUSHOUT_DOING) return true;
        if (calledBallsLocal != 0 && pointPocketsLocal != 0) return true;
        
        return false;
    }
#endif
#if EIJIS_EXTERNAL_SCORE_SCREEN

    private void updateBowlardsScore(uint teamId, int frame, int throwInning, int point, bool lastFrame)
    {
#if EIJIS_DEBUG_SCORE_SCREEN && EIJIS_DEBUG_BOWLARDS
        _LogInfo($"EIJIS_DEBUG BilliardsModule::updateBowlardsScore(teamId = {teamId}, frame = {frame}, throwInning = {throwInning}, point = {point}, lastFrame = {lastFrame})");
#endif
        int additional = (2 <= throwInning ? 1 : 0);
#if EIJIS_DEBUG_SCORE_SCREEN && EIJIS_DEBUG_BOWLARDS
        // _LogInfo($"EIJIS_DEBUG  additional = {additional}");
#endif

        byte inningValue;

        if (9 <= frame && lastFrame && 0 < additional)
        {
            // re-use variables
            inningValue = (byte)(chainedFoulsLocal[teamId] & 0x0F);
            inningValue += (byte)point;
            chainedFoulsLocal[teamId] &= 0xF0;
            chainedFoulsLocal[teamId] |= (byte)(inningValue & 0x0F);
#if EIJIS_DEBUG_SCORE_SCREEN && EIJIS_DEBUG_BOWLARDS
            // _LogInfo($"EIJIS_DEBUG  final inning data(chainedFoulsLocal[]) = {chainedFoulsLocal[0]}, {chainedFoulsLocal[1]}");
#endif
            return;
        }

        // re-use variables
        ushort[][] frameVariable = new ushort[][]
        {
            framePointsLocal, framePointsLocal,
            framePointsLocal2, framePointsLocal2,
            totalPointsLocal, totalPointsLocal,
            highRunsLocal, highRunsLocal,
            chainedPointsLocal, chainedPointsLocal
        };
        
        ushort frameMask = (ushort)((frame + additional) % 2 == 0 ? 0x00FF : 0xFF00);
        int frameShift = ((frame + additional) % 2 == 0) ? 0 : 8;
        byte inningMask = (byte)(throwInning == 1 ? 0xF0 : 0x0F);
        int inningShift = throwInning == 1 ? 4 : 0;
        ushort frameClearMask = (ushort)(~(frameMask & (inningMask << frameShift)) & 0xFFFF);

        ushort frameValue = (ushort)((frameVariable[frame + additional][teamId] & frameMask) >> frameShift);
        inningValue = (byte)((frameValue & inningMask) >> inningShift);
#if EIJIS_DEBUG_SCORE_SCREEN && EIJIS_DEBUG_BOWLARDS
        // _LogInfo($"EIJIS_DEBUG  frameMask = 0x{frameMask:X4}, frameShift = {frameShift}, inningMask = 0x{inningMask:X2}, inningShift = {inningShift}, frameClearMask = 0x{frameClearMask:X4}");
        // _LogInfo($"EIJIS_DEBUG  frameVariable[{frame + additional}][{teamId}] = {frameVariable[frame + additional][teamId]}(0x{frameVariable[frame + additional][teamId]:X4})");
        // _LogInfo($"EIJIS_DEBUG  frameValue = {frameValue}(0x{frameValue:X2})");
        // _LogInfo($"EIJIS_DEBUG  inningValue = {inningValue}(0x{inningValue:X}) + point = {point}");
#endif
        inningValue += (byte)(11 <= point ? 0 : point);
        frameValue = (ushort)(((inningValue << inningShift) << frameShift) & (inningMask << frameShift));
#if EIJIS_DEBUG_SCORE_SCREEN && EIJIS_DEBUG_BOWLARDS
        // _LogInfo($"EIJIS_DEBUG  inningValue = {inningValue}(0x{inningValue:X}), frameValue = 0x{frameValue:X4}");
#endif
        frameVariable[frame + additional][teamId] &= frameClearMask;
#if EIJIS_DEBUG_SCORE_SCREEN && EIJIS_DEBUG_BOWLARDS
        // _LogInfo($"EIJIS_DEBUG  frameVariable[{frame + additional}][{teamId}] = {frameVariable[frame + additional][teamId]}(0x{frameVariable[frame + additional][teamId]:X}) cleared");
#endif
        frameVariable[frame + additional][teamId] |= frameValue;
#if EIJIS_DEBUG_SCORE_SCREEN && EIJIS_DEBUG_BOWLARDS
        // _LogInfo($"EIJIS_DEBUG  frameVariable[{frame + additional}][{teamId}] = {frameVariable[frame + additional][teamId]}(0x{frameVariable[frame + additional][teamId]:X})");
#endif
    }

    private int getBowlardsScore(uint teamId, int frame, int throwInning, bool lastFrame)
    {
        int additional = (2 <= throwInning ? 1 : 0);
        int inningValue;
        
        if (9 <= frame && lastFrame && 0 < additional)
        {
            // re-use variables
            inningValue = (chainedFoulsLocal[teamId] & 0x0F);
            return inningValue;
        }
        
        // re-use variables
        ushort[][] frameVariable = new ushort[][]
        {
            framePointsLocal, framePointsLocal,
            framePointsLocal2, framePointsLocal2,
            totalPointsLocal, totalPointsLocal,
            highRunsLocal, highRunsLocal,
            chainedPointsLocal, chainedPointsLocal
        };
        
        ushort frameMask = (ushort)((frame + additional) % 2 == 0 ? 0x00FF : 0xFF00);
        int frameShift = ((frame + additional) % 2 == 0) ? 0 : 8;
        byte inningMask = (byte)(throwInning == 1 ? 0xF0 : 0x0F);
        int inningShift = throwInning == 1 ? 4 : 0;
        ushort frameClearMask = (ushort)(~(frameMask & (inningMask << frameShift)) & 0xFFFF);

        ushort frameValue = (ushort)((frameVariable[frame + additional][teamId] & frameMask) >> frameShift);
        inningValue = ((frameValue & inningMask) >> inningShift);
        return inningValue;
    }
    
    private byte[][] generateBowlardsScoreByteArrays(int frameLength)
    {
        int paddingFrame = 10 - frameLength;
#if EIJIS_DEBUG_BOWLARDS
        // _LogInfo($"EIJIS_DEBUG BilliardsModule::generateBowlardsScoreByteArrays(frameLength = {frameLength}) paddingFrame= {paddingFrame}");
#endif
        byte[][] framePoints = new byte[2][] { null, null };
        for (int teamId = 0; teamId < 2; teamId++)
        {
            byte[] framePoint = generateBowlardsScoreByteArray(teamId);
#if EIJIS_DEBUG_BOWLARDS
            // _Log(
            //     $"{framePoint[0]:X2} "+
            //     $"{framePoint[1]:X2} "+
            //     $"{framePoint[2]:X2} "+
            //     $"{framePoint[3]:X2} "+
            //     $"{framePoint[4]:X2} "+
            //     $"{framePoint[5]:X2} "+
            //     $"{framePoint[6]:X2} "+
            //     $"{framePoint[7]:X2} "+
            //     $"{framePoint[8]:X2} "+
            //     $"{framePoint[9]:X2} "+
            //     $"{framePoint[10]:X}"
            // );
#endif
            byte[] framePointPadding = new byte[11];
            Array.Copy(framePoint, 0, framePointPadding, paddingFrame, frameLength + 1);
            framePoints[teamId] = framePointPadding;
        }

        return framePoints;
    }
    
    private byte[] generateBowlardsScoreByteArray(int teamId)
    {
#if EIJIS_DEBUG_BOWLARDS
        // _LogInfo($"EIJIS_DEBUG BilliardsModule::generateBowlardsScoreByteArray(teamId = {teamId}) chainedFoulsLocal = {chainedFoulsLocal[0]}, {chainedFoulsLocal[1]}, chainedFoulsSynced = {networkingManager.chainedFoulsSynced[0]}, {networkingManager.chainedFoulsSynced[1]}");
#endif
        byte[] framePointsByTeam = new byte[11];

        int i = 0;
        framePointsByTeam[i++] = (byte)((framePointsLocal[teamId] & 0x00FF) >> 0);
        framePointsByTeam[i++] = (byte)((framePointsLocal[teamId] & 0xFF00) >> 8);
        framePointsByTeam[i++] = (byte)((framePointsLocal2[teamId] & 0x00FF) >> 0);
        framePointsByTeam[i++] = (byte)((framePointsLocal2[teamId] & 0xFF00) >> 8);
        framePointsByTeam[i++] = (byte)((totalPointsLocal[teamId] & 0x00FF) >> 0);
        framePointsByTeam[i++] = (byte)((totalPointsLocal[teamId] & 0xFF00) >> 8);
        framePointsByTeam[i++] = (byte)((highRunsLocal[teamId] & 0x00FF) >> 0);
        framePointsByTeam[i++] = (byte)((highRunsLocal[teamId] & 0xFF00) >> 8);
        framePointsByTeam[i++] = (byte)((chainedPointsLocal[teamId] & 0x00FF) >> 0);
        framePointsByTeam[i++] = (byte)((chainedPointsLocal[teamId] & 0xFF00) >> 8);
        framePointsByTeam[i] = networkingManager.chainedFoulsSynced[teamId]; // chainedFoulsLocal[teamId];

        return framePointsByTeam;
    }
    
    private void clearBowlardsScoreVariables(int teamId)
    {
#if EIJIS_DEBUG_SCORE_SCREEN && EIJIS_DEBUG_BOWLARDS
        _LogInfo($"EIJIS_DEBUG BilliardsModule::clearBowlardsScoreVariables(teamId = {teamId})");
#endif
        networkingManager.framePointsSynced[teamId] = framePointsLocal[teamId] = 0;
        networkingManager.framePointsSynced2[teamId] = framePointsLocal2[teamId] = 0;
        networkingManager.totalPointsSynced[teamId] = totalPointsLocal[teamId] = 0;
        networkingManager.highRunsSynced[teamId] = highRunsLocal[teamId] = 0;
        networkingManager.chainedPointsSynced[teamId] = chainedPointsLocal[teamId] = 0;
        networkingManager.chainedFoulsSynced[teamId] = chainedFoulsLocal[teamId] = 0;
        networkingManager.fourBallScoresSynced[teamId] = fbScoresLocal[teamId] = 0;
    }
#endif
#endregion

    #region MiscFunction

    public bool _CanUseTableSkin(string owner, int skin)
    {
        if (tableSkinHook == null) return false;

        tableSkinHook.SetProgramVariable("inOwner", owner);
        tableSkinHook.SetProgramVariable("inSkin", skin);
        tableSkinHook.SendCustomEvent("_CanUseTableSkin");

        return (bool)tableSkinHook.GetProgramVariable("outCanUse");
    }

    //public bool _CanUseCueSkin(int owner, int skin)
    //{
    //    if (cueSkinHook == null) return false;

    //    cueSkinHook.SetProgramVariable("inOwner", owner);
    //    cueSkinHook.SetProgramVariable("inSkin", skin);
    //    cueSkinHook.SendCustomEvent("_CanUseCueSkin");

    //    return (bool)cueSkinHook.GetProgramVariable("outCanUse");
    //}
    public int _CanUseCueSkin(int owner, int skin)   //改了改
    {
        if (tableHook == null) return 0;

        tableHook.SetProgramVariable("inOwner", owner);
        tableHook.SetProgramVariable("inSkin", skin);
        tableHook.SendCustomEvent("_CanUseCueSkin");

        return (int)tableHook.GetProgramVariable("outCanUse");
    }


    public void checkDistanceLoop()
    {
#if EIJIS_ISSUE_FIX
        if (ReferenceEquals(null, Networking.LocalPlayer)) return;
#endif
        if (checkingDistant)
            SendCustomEventDelayedSeconds(nameof(checkDistanceLoop), 1f);
        else
            return;

        checkDistanceLoD();
    }

    public void checkDistanceLoD()
    {
        bool nowDistant = (Vector3.Distance(Networking.LocalPlayer.GetPosition(), transform.position) > LoDDistance) && !noLOD
// #if EIJIS_ROTATION || EIJIS_DEBUG_BOWLARDS // ← この指定は間違っている。正しくは EIJIS_BOWLARDS これが不具合の原因かも
// #if EIJIS_ROTATION || EIJIS_BOWLARDS
#if EIJIS_ROTATION // ← この状態にして不具合の再現を待つ
        && !((networkingManager.gameStateSynced == 2 || networkingManager.gameStateSynced == 4) && Networking.IsOwner(networkingManager.gameObject));
#else
        && !(networkingManager.gameStateSynced == 2 && Networking.IsOwner(networkingManager.gameObject));
#endif
        if (nowDistant == localPlayerDistant) { return; }
#if EIJIS_DEBUG_BOWLARDS
        _LogInfo($"EIJIS_DEBUG BilliardsModule::checkDistanceLoD() nowDistant = {nowDistant}, localPlayerDistant = {localPlayerDistant}, n.gameStateSynced = {networkingManager.gameStateSynced}, n.delayedDeserialization = {networkingManager.delayedDeserialization}");
#endif
        if (isPlayer)
        {
            localPlayerDistant = false;
            return;
        }
        else
        {
            localPlayerDistant = nowDistant;
        }
        if (networkingManager.delayedDeserialization)
        {
            networkingManager.OnDeserialization();
        }
        setLOD();
    }

    private void setLOD()
    {
        for (int i = 0; i < cueControllers.Length; i++) cueControllers[i]._RefreshRenderer();
        balls[0].transform.parent.gameObject.SetActive(!localPlayerDistant);
        debugger.SetActive(!localPlayerDistant);
        menuManager._RefreshLobby();
        graphicsManager._UpdateLOD();
    }

    private string[] getPlayerNames(int[] PlayerIDs)
    {
        if (PlayerIDs == null)
            return null;

        Debug.Log("[SCM] IDlen" + PlayerIDs.Length);
        //返回值数组
        string[] ret = new string[PlayerIDs.Length];

        for (int i = 0; i < PlayerIDs.Length; i++)
        {
            //获取玩家API对象
            VRCPlayerApi Tmp = VRCPlayerApi.GetPlayerById(PlayerIDs[i]);

            //存储玩家名到String数组
            if (Tmp != null)
            {
                if (Tmp.IsValid())
                {
                    ret[i] = Tmp.displayName;
                }
                else
                {
                    ret[i] = "";
                }
            }
        }
        Debug.Log("[SCM] IDDATA" + ret[0] + ";" + ret[1]);
        return ret;
    }

#if EIJIS_EXTERNAL_SCORE_SCREEN
    private void updateInfoText()
    {
#if EIJIS_DEBUG_SCORE_SCREEN
        _LogInfo($"EIJIS_DEBUG BilliardsModule::updateInfoText() gameLive = {gameLive}, turnStateLocal = {turnStateLocal}, turnStateSynced = {networkingManager.turnStateSynced}, turnStateChanged = {networkingManager.turnStateSynced != turnStateLocal}, stateIdSynced = {networkingManager.stateIdSynced}");
#endif
        if (ReferenceEquals(null, scoreScreen))  return;

        if (!gameLive) return;

        bool stateIdChanged = (networkingManager.stateIdSynced != stateIdLocal);
        if (!stateIdChanged) return;
        
        if (networkingManager.stateIdSynced < stateIdLocal && networkingManager.stateIdSynced != 1)
        {
            scoreScreen.clearInfoText();
            return;
        }

        bool turnStateChanged = (networkingManager.turnStateSynced != turnStateLocal);
        if (!turnStateChanged) return;

#if EIJIS_ROTATION || EIJIS_BOWLARDS
        if (!isRotation && !isBowlards) return;
        
        bool teamIdChanged = (teamIdLocal != networkingManager.teamIdSynced);
        bool isTurnSimulate = (networkingManager.turnStateSynced == 1);
        bool isRotationFoul = isRotation && (networkingManager.nextBallRepositionStateSynced & 0x1u) > 0;
        bool isSkipped = (!isRotationFoul && networkingManager.turnStateSynced == 2 && networkingManager.foulStateSynced == 0);
        bool isOnBreakShot = networkingManager.colorTurnSynced;
        bool isCalled = networkingManager.calledBallsSynced != 0 && networkingManager.pointPocketsSynced != 0;
        uint calledBalls = networkingManager.calledBallsSynced;
        uint calledPockets = networkingManager.pointPocketsSynced;
        bool isNoShotOperation = turnStateLocal == 0 && networkingManager.turnStateSynced == 2; // cueBall reposition or choice spot
        // bool isBowlardsFoul = isBowlards && !isOnBreakShot && networkingManager.foulStateSynced == 1;
        bool isBowlardsThrowInningChanged = isBowlards && inningCountLocal != networkingManager.inningCountSynced;

#if EIJIS_DEBUG_SCORE_SCREEN
        // _LogInfo($"  stateIdSynced = {networkingManager.stateIdSynced}, stateIdLocal = {stateIdLocal}, teamIdChanged = {teamIdChanged}, isTurnSimulate = {isTurnSimulate}, isSkipped = {isSkipped}, foulStateChanged = {foulStateChanged}, isFoul = {isFoul}, isOnBreakShot = {isOnBreakShot}");
        _LogInfo($"  stateIdSynced = {networkingManager.stateIdSynced}, stateIdLocal = {stateIdLocal}, teamIdChanged = {teamIdChanged}, isTurnSimulate = {isTurnSimulate}, isSkipped = {isSkipped}, isRotationFoul = {isRotationFoul}, isOnBreakShot = {isOnBreakShot}");
#endif

        string message = string.Empty;
        string teamName = teamIdLocal == 0 ? "[Red ]" : "[Blue]";

        if (networkingManager.stateIdSynced == 0)
        {
            scoreScreen.clearInfoText();
        }
        else if (isTurnSimulate)
        {
            if (requireCallShotLocal)
            {
                if (isOnBreakShot)
                {
                    message = "ignore call (break shot)";
                }
                else if (safetyCalledLocal)
                {
                    message = "call safety";
                }
                else if (isCalled)
                {
                    string ballNumber = string.Empty;
                    for (int i = 1; i < balls.Length; i++)
                    {
                        if (0 < ((calledBalls >> i) & 0x1u))
                        {
                            ballNumber = $"{(i == 1 ? 8 : (i < 9 ? i - 1 : i))}";
                            break;
                        }
                    }

                    string pocketName = string.Empty;
                    for (int i = 0; i < pointPocketMarkers.Length; i++)
                    {
                        if (0 < ((calledPockets >> i) & 0x1u))
                        {
                            pocketName = i == 0 ? "corner SE"
                                : (i == 1 ? "corner SW"
                                    : (i == 2 ? "corner NE"
                                        : (i == 3 ? "corner NW"
                                            : (i == 4 ? "side E" : "side W"))));
                            break;
                        }
                    }
                    message = $"call ball({ballNumber}) pocket[{pocketName}]";
                }
                else
                {
                    message = "no call ball or pocket";
                }
            }
            else
            {
                message = "no call mode";
            }
#if EIJIS_DEBUG_SCORE_SCREEN
            _LogInfo($"  key = {networkingManager.stateIdSynced}, message = {message}");
#endif
            scoreScreen.updateInfoText(networkingManager.stateIdSynced, $"{teamName} {message}");
        }
        else if (!isNoShotOperation && (teamIdChanged || !isRotationFoul))
        {
            if (isRotation && teamIdChanged)
            {
                if (isRotationFoul)
                {
                    message = $" -> foul (count {networkingManager.chainedFoulsSynced[networkingManager.teamIdSynced ^ 0x1u]})";
                }
                else if (isSkipped)
                {
                    message = $"{teamName} turn change (skip pass)";
                }
                else if (isCalled)
                {
                    message = " -> miss (safe shot)";
                }
                else 
                {
                    message = " -> turn change (safe shot)";
                }
            }
            else if (isBowlardsThrowInningChanged)
            {
                // message = " -> failed (inning end)";
                message = " -> inning end";
            }
            else if (!isRotationFoul)
            {
                message = " -> success";
            }
#if EIJIS_DEBUG_SCORE_SCREEN
            _LogInfo($"  key = {stateIdLocal}, message = {message}");
#endif
            scoreScreen.updateInfoText(networkingManager.stateIdSynced, $"{teamName} {message}");
        }
#endif
    }
#endif
    #endregion

    #region Debugger
    const string LOG_LOW = "<color=\"#ADADAD\">";
    const string LOG_ERR = "<color=\"#B84139\">";
    const string LOG_WARN = "<color=\"#DEC521\">";
    const string LOG_YES = "<color=\"#69D128\">";
    const string LOG_END = "</color>";
#if HT8B_DEBUGGER
    public void _Log(string msg)
    {
        _log(LOG_WARN + msg + LOG_END);
    }
    public void _LogYes(string msg)
    {
        _log(LOG_YES + msg + LOG_END);
    }
    public void _LogWarn(string msg)
    {
        _log(LOG_WARN + msg + LOG_END);
    }
    public void _LogError(string msg)
    {
        _log(LOG_ERR + msg + LOG_END);
    }
    public void _LogInfo(string msg)
    {
        _log(LOG_LOW + msg + LOG_END);
    }
    public void _RedrawDebugger()
    {
        redrawDebugger();
    }
#else
public void _Log(string msg) { }
public void _LogYes(string msg) { }
public void _LogInfo(string msg) { }
public void _LogWarn(string msg) { }
public void _LogError(string msg) { }
public void _RedrawDebugger() { }
#endif

    public void _BeginPerf(int id)
    {
        perfStart[id] = Time.realtimeSinceStartup;
    }

    public void _EndPerf(int id)
    {
        perfTimings[id] += Time.realtimeSinceStartup - perfStart[id];
        perfCounters[id]++;
    }
#if EIJIS_LOG_PREFIX_COLOR_OFF

    private string stripTag(string source)
    {
        int searchStartPos = 0;
        string work = source;
        bool tagFound = false;
        do
        {
            tagFound = false;
            int braceStartIndex = work.IndexOf('<', searchStartPos);
            if (0 <= braceStartIndex && braceStartIndex + 1 < work.Length)
            {
                int braceEndIndex = work.IndexOf('>', braceStartIndex + 1);
                if (0 <= braceEndIndex)
                {
                    work = work.Substring(0, braceStartIndex) +
                           work.Substring(braceEndIndex + 1);
                    searchStartPos = braceStartIndex;
                    tagFound = true;
                }
            }
        } while (tagFound);

        return work;
    }
#endif

    
    private void _log(string ln)
    {
#if EIJIS_TABLE_LABEL
#if EIJIS_LOG_PREFIX_COLOR_OFF
        Debug.Log("[BilliardsModule" + logLabel + "] " + stripTag(ln));
#else
        Debug.Log("[<color=\"#B5438F\">BilliardsModule</color>" + logLabel + "] " + ln);
#endif
#else
        Debug.Log("[<color=\"#B5438F\">BilliardsModule</color>] " + ln);
#endif

#if EIJIS_TABLE_LABEL
        LOG_LINES[LOG_PTR++] = "[<color=\"#B5438F\">BilliardsModule" + logLabel + "</color>] " + ln + "\n";
#else
        LOG_LINES[LOG_PTR++] = "[<color=\"#B5438F\">BilliardsModule</color>] " + ln + "\n";
#endif
        LOG_LEN++;

        if (LOG_PTR >= LOG_MAX)
        {
            LOG_PTR = 0;
        }

        if (LOG_LEN > LOG_MAX)
        {
            LOG_LEN = LOG_MAX;
        }

        redrawDebugger();
    }

    private void redrawDebugger()
    {
#if EIJIS_TABLE_LABEL
        string output = "BilliardsModule " + VERSION + " [" + logLabel + " ] ";
#else
        string output = "BilliardsModule ";
#endif

        // Add information about game state:
        output += Networking.IsOwner(Networking.LocalPlayer, networkingManager.gameObject) ?
           "<color=\"#95a2b8\">net(</color> <color=\"#4287F5\">OWNER</color> <color=\"#95a2b8\">)</color> " :
           "<color=\"#95a2b8\">net(</color> <color=\"#678AC2\">RECVR</color> <color=\"#95a2b8\">)</color> ";

        output += isLocalSimulationRunning ?
           "<color=\"#95a2b8\">sim(</color> <color=\"#4287F5\">ACTIVE</color> <color=\"#95a2b8\">)</color> " :
           "<color=\"#95a2b8\">sim(</color> <color=\"#678AC2\">PAUSED</color> <color=\"#95a2b8\">)</color> ";

        VRCPlayerApi currentOwner = Networking.GetOwner(networkingManager.gameObject);
        output += "<color=\"#95a2b8\">owner(</color> <color=\"#4287F5\">" + (Utilities.IsValid(currentOwner) ? currentOwner.displayName + ":" + currentOwner.playerId : "[null]") + "/" + teamIdLocal + "</color> <color=\"#95a2b8\">)</color> ";

        if (currentPhysicsManager)
        {
            output += "Physics: " + (string)currentPhysicsManager.GetProgramVariable("PHYSICSNAME");
        }

        output += "\n---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------\n";

        for (int i = 0; i < PERF_MAX; i++)
        {
            output += "<color=\"#95a2b8\">" + perfNames[i] + "(</color> " + (perfCounters[i] > 0 ? perfTimings[i] * 1e6 / perfCounters[i] : 0).ToString("F2") + "µs <color=\"#95a2b8\">)</color> ";
            // to not average them (see values from this frame)
            // requires changing _EndPerf() to be = instead of +=
            // output += "<color=\"#95a2b8\">" + perfNames[i] + "(</color> " + (/*perfCounters[i] > 0 ? */ perfTimings[i] * 1e6 /* / perfCounters[i] : 0 */).ToString("F2") + "µs <color=\"#95a2b8\">)</color> ";
        }

        output += "\n---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------\n";

        // Update display 
        for (int i = 0; i < LOG_LEN; i++)
        {
            output += LOG_LINES[(LOG_MAX + LOG_PTR - LOG_LEN + i) % LOG_MAX];
        }

        ltext.text = output;
    }
    #endregion
}
