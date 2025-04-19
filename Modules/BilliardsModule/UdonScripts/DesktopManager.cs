﻿#define EIJIS_ISSUE_FIX
#define EIJIS_PYRAMID
#define EIJIS_CUEBALLSWAP
#define EIJIS_PUSHOUT
#define EIJIS_CALLSHOT
#define EIJIS_10BALL
#define EIJIS_MNBK_AUTOCOUNTER

// #define EIJIS_DEBUG_BALLORDER

#if EIJIS_CUEBALLSWAP
using System;
#endif
using Metaphira.Modules.CameraOverride;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class DesktopManager : UdonSharpBehaviour
{
    private const int CAMERA_RENDER_MODE_DISABLED = CameraOverrideModule.RENDER_MODE_DISABLED;
    private const int CAMERA_RENDER_MODE_DESKTOP = CameraOverrideModule.RENDER_MODE_DESKTOP;

    private const float k_BALL_RADIUS = 0.03f;
    private const float CURSOR_SPEED = 0.035f;
    private float MAX_SPIN_MAGNITUDE = 0.90f;

    [SerializeField] private GameObject root;
    [SerializeField] private GameObject cursorIndicator;
    [SerializeField] private GameObject spinIndicator;
    [SerializeField] private GameObject jumpIndicator;
    [SerializeField] private GameObject powerIndicator;
    [SerializeField] private GameObject pressE;
#if EIJIS_CUEBALLSWAP
    [SerializeField] private GameObject callCueBall;
#endif
#if EIJIS_CALLSHOT
    [SerializeField] private GameObject callShot;
#endif
#if EIJIS_PUSHOUT
    [SerializeField] private GameObject pushOut;
    [SerializeField] private GameObject pushOutDoing;
#endif
#if EIJIS_MNBK_AUTOCOUNTER
    private GameObject desktopMnbk;
    private GameObject safetyCalled;
    private GameObject paused;
#endif

    private BilliardsModule table;

    private bool isDesktopUser;

    private bool canShoot;

    private bool holdingCue;
    private bool inUI;
    private bool repositionMode;

    private bool isShooting;
    private bool isRepositioning;
    private Repositioner currentRepositioner;

    private Vector3 initialShotDirection;
    private float initialPower;
    Vector3 initialCursorPosition;

    private Vector3 spin;
    private float jumpAngle;
    private float power;

    private Vector3 cursor;
    private float cursorClampX;
    private float cursorClampZ;

    private Vector3 rootStartScale;
    private float cameraStartScale;

    public void _Init(BilliardsModule table_)
    {
        table = table_;
        cursorClampX = table.k_TABLE_WIDTH + .3f;
        cursorClampZ = table.k_TABLE_HEIGHT + .3f;
        rootStartScale = root.transform.localScale;
        cameraStartScale = root.GetComponentInChildren<Camera>().orthographicSize;
        _RefreshTable();
        _RefreshPhysics();
#if EIJIS_MNBK_AUTOCOUNTER
        Transform desktopMnbkTr = table.transform.Find("intl.desktop/desktop/desktop_mnbk");
        if (ReferenceEquals(null, desktopMnbkTr))
        {
            table._LogInfo("  desktop_mnbk object not set.");
        }
        else
        {
            desktopMnbk = desktopMnbkTr.gameObject;
        }
        Transform desktopSafetyCalled = table.transform.Find("intl.desktop/desktop/desktop_safetycalled");
        if (ReferenceEquals(null, desktopSafetyCalled))
        {
            table._LogInfo("  desktop_safetycalled object not set.");
        }
        else
        {
            safetyCalled = desktopSafetyCalled.gameObject;
        }
        Transform desktopPaused = table.transform.Find("intl.desktop/desktop/desktop_paused");
        if (ReferenceEquals(null, desktopPaused))
        {
            table._LogInfo("  desktop_paused object not set.");
        }
        else
        {
            paused = desktopPaused.gameObject;
        }
#endif
    }

    public void _OnGameStarted()
    {
        // maybe vrchat lets people switch between pc and vr in the future idk
        isDesktopUser = !Networking.LocalPlayer.IsUserInVR();
    }

    public void _OnPickupCue()
    {
        holdingCue = true;
    }

    public void _OnDropCue()
    {
        holdingCue = false;
        exitUI();
    }

    public void _RefreshPhysics()
    {
        MAX_SPIN_MAGNITUDE = (float)table.currentPhysicsManager.GetProgramVariable("CueMaxHitRadius");
    }

    public void _RefreshTable()
    {
        Camera desktopCamera = root.GetComponentInChildren<Camera>();
        Vector3 campos = desktopCamera.transform.position;
        float SF = table.tableModels[table.tableModelLocal].DesktopUIScaleFactor;
        desktopCamera.orthographicSize = cameraStartScale * SF;
        root.transform.localScale = rootStartScale * SF;
        desktopCamera.transform.position = campos; // don't change camera position with it's parent's scale(root)
    }

    public void _Tick(uint gameModeLocal)
    {
        if (!isDesktopUser) return;

        if (Networking.LocalPlayer == null) return;

        VRCPlayerApi.TrackingData hmd = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
        Vector3 basePosition = hmd.position + hmd.rotation * Vector3.forward;

        pressE.transform.position = basePosition;

        Vector3 playerPos = table.transform.InverseTransformPoint(Networking.LocalPlayer.GetPosition());
        bool canUseUI = (Mathf.Abs(playerPos.x) < 3.5f) && (Mathf.Abs(playerPos.z) < 2.5f);

        pressE.SetActive(holdingCue && canUseUI);

        if (!holdingCue)
        {
            if (inUI) exitUI();

            return;
        }

        if (canUseUI)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!inUI)
                {
                    enterUI();
                }
                else
                {
                    exitUI();
                }
            }
        }

        if (inUI)
        {
            tickUI(gameModeLocal);
        }
    }

    private void tickUI(uint gameModeLocal)
    {
        bool clickNow = Input.GetKeyDown(KeyCode.Mouse0);
        bool click = Input.GetKey(KeyCode.Mouse0);

        cursor.x = Mathf.Clamp(cursor.x + Input.GetAxis("Mouse X") * CURSOR_SPEED, -cursorClampX, cursorClampX);
        cursor.y = 2.0f; // so you can see it on the table
        cursor.z = Mathf.Clamp(cursor.z + Input.GetAxis("Mouse Y") * CURSOR_SPEED, -cursorClampZ, cursorClampZ);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            repositionMode = !repositionMode;
        }

        if (canShoot)
        {
            Vector3 flatCursor = cursor;
            flatCursor.y = 0.0f;

            Vector3 shotDirection = flatCursor - table.ballsP[0];

            if (repositionMode)
            {
                if (!isRepositioning)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        isRepositioning = true;

                        Vector3 localPos = new Vector3(cursor.x, 0, cursor.z);
                        Vector3 worldPos = table.balls[0].transform.parent.TransformPoint(localPos);
                        Collider[] colliders = Physics.OverlapSphere(worldPos, k_BALL_RADIUS / 4f, 1 << 22);
                        foreach (Collider c in colliders)
                        {
                            if (c != null && c.gameObject != null)
                            {
                                Repositioner repositioner = c.gameObject.GetComponent<Repositioner>();
                                if (repositioner != null)
                                {
                                    table.repositionManager._BeginReposition(repositioner);
                                    currentRepositioner = repositioner;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (currentRepositioner != null)
                {
                    Vector3 localPos = new Vector3(cursor.x, 0, cursor.z);
                    Vector3 worldPos = table.balls[0].transform.parent.TransformPoint(localPos);
                    worldPos.y = currentRepositioner.transform.position.y;
                    currentRepositioner.transform.position = worldPos;
                }

                if (!Input.GetKey(KeyCode.Mouse0))
                {
                    isRepositioning = false;
                    stopRepositioning();
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    if (!isShooting)
                    {
                        isShooting = true;

                        initialShotDirection = shotDirection.normalized;
                        initialCursorPosition = cursor;
                        initialPower = Vector3.Dot(initialShotDirection, flatCursor);

                        // unlock cursor
                        cursorIndicator.SetActive(false);
                        cursorClampX = Mathf.Infinity;
                        cursorClampZ = Mathf.Infinity;
                    }

                    power = Mathf.Clamp(initialPower - Vector3.Dot(initialShotDirection, flatCursor), 0.0f, 0.5f);
                    shotDirection = initialShotDirection;
                }
                else
                {
                    // Trigger shot
                    if (isShooting)
                    {
                        // we still keep the same shot direction if we're shooting
                        shotDirection = initialShotDirection;

#if EIJIS_ISSUE_FIX
                        if (table.balls[0].transform.localPosition.y > 0)
                        {
                            power = 0;
                        }
#endif
                        if (power > 0)
                        {
                            if (((string)table.currentPhysicsManager.GetProgramVariable("PHYSICSNAME")).Contains("Legacy"))
                            {
                                table.currentPhysicsManager.SetProgramVariable("multiplier", -25.0f);
                                table.currentPhysicsManager.SetProgramVariable("cue_vdir", initialShotDirection.normalized);
                                float vel = Mathf.Pow(power * 2.0f, 1.4f) * 9.0f;
                                table.currentPhysicsManager.SetProgramVariable("inV0", vel);
                            }
                            else
                            {
                                float vel = Mathf.Pow(power * 2.0f, 1.4f) * 4.0f;
                                if (Networking.LocalPlayer != null && Networking.LocalPlayer.displayName == "metaphira") vel = vel / 4.0f * 10.0f;
                                table.currentPhysicsManager.SetProgramVariable("inV0", vel);
                            }
                            table.currentPhysicsManager.SendCustomEvent("_ApplyPhysics");

                            table._TriggerCueBallHit();

                            // shot was successful, reset some state
                            _DenyShoot();
                        }

                        stopShooting();
                    }
#if EIJIS_MNBK_AUTOCOUNTER
                    updateCallSafetyIndicator();
#endif
                }
#if EIJIS_MNBK_AUTOCOUNTER
                updatePausedIndicator();
#endif

                renderCuePosition(shotDirection);
                updateSpinIndicator();
                updateJumpIndicator();
#if EIJIS_PUSHOUT
#if EIJIS_CUEBALLSWAP || EIJIS_CALLSHOT
                updateCallShotIndicator();
#endif
                updatePushOutIndicator();
#else
#if EIJIS_CUEBALLSWAP
                if(gameModeLocal==BilliardsModule.GAMEMODE_PYRAMID)     //cheese add
                     updateCallShotIndicator();
#endif
#endif
            }
        }

        cursorIndicator.transform.localPosition = cursor * (1 / table.tableModels[table.tableModelLocal].DesktopUIScaleFactor);
        powerIndicator.transform.localScale = new Vector3(1.0f - (power * 2.0f), 1.0f, 1.0f);
#if EIJIS_PUSHOUT
        
        pushOutDoing.SetActive(table.pushOutStateLocal == table.PUSHOUT_DOING);
#endif
#if EIJIS_MNBK_AUTOCOUNTER
#if EIJIS_CALLSHOT
        if (!ReferenceEquals(null, safetyCalled)) safetyCalled.SetActive(table.safetyCalledLocal && !table.requireCallShotLocal);
        if (!ReferenceEquals(null, paused)) paused.SetActive(table.pausedLocal && !table.requireCallShotLocal);
#else
        if (!ReferenceEquals(null, safetyCalled)) safetyCalled.SetActive(table.safetyCalledLocal);
        if (!ReferenceEquals(null, paused)) paused.SetActive(table.pausedLocal);
#endif
        if (table.pausedLocal) return;
#endif

        bool hitCtrlNow = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);
        bool hitCtrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        bool hitZNow = Input.GetKeyDown(KeyCode.Z);
        bool hitZ = Input.GetKey(KeyCode.Z);
        bool hitXNow = Input.GetKeyDown(KeyCode.X);
        bool hitX = Input.GetKey(KeyCode.X);
        if ((hitCtrlNow && hitZ) || (hitCtrl && hitZNow))
        {
            table.practiceManager._Undo();
        }
        else if ((hitCtrlNow && hitX) || (hitCtrl && hitXNow))
        {
            table.practiceManager._Redo();
        }
    }

    private void updateSpinIndicator()
    {
        if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            spin += Vector3.forward * Time.deltaTime;
        }
        if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.S))
        {
            spin += Vector3.back * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            spin += Vector3.left * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            spin += Vector3.right * Time.deltaTime;
        }

        if (spin.magnitude > MAX_SPIN_MAGNITUDE)
        {
            spin = spin.normalized * MAX_SPIN_MAGNITUDE;
        }

        spinIndicator.transform.localPosition = spin;
    }

    private void updateJumpIndicator()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            jumpAngle += Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.S))
        {
            jumpAngle -= Time.deltaTime;
        }
        jumpAngle = Mathf.Clamp(jumpAngle, 0, Mathf.PI / 2);

        jumpIndicator.transform.localPosition = new Vector3(-Mathf.Cos(jumpAngle) * 1.1f, 0, Mathf.Sin(jumpAngle) * 1.1f);
    }

#if EIJIS_CUEBALLSWAP || EIJIS_CALLSHOT
    private int nextBallOrder(bool asc)
    {
#if EIJIS_DEBUG_BALLORDER
        table._LogInfo($"DesktopManager::nextBallOrder(asc = {asc})");
#endif
        int id = 0;
        uint calledBalls = table.calledBallsLocal;
        for (int i = 1; i < table.ballsP.Length; i++)
        {
            if (((calledBalls >> i) & 0x1u) != 0)
            {
                id = i;
                break;
            }
        }
#if EIJIS_DEBUG_BALLORDER
        table._LogInfo($"  before called ball id = {id}");
#endif

        uint ballsPocketed = table.ballsPocketedLocal;
        int beforeId = id;
        float before_x = table.ballsP[id].x;
        float before_z = table.ballsP[id].z;
        float nearest_x = asc ? float.MaxValue : float.MinValue;
        float nearest_z = asc ? float.MaxValue : float.MinValue;
        int farestId = 0;
        float farest_x = asc ? float.MaxValue : float.MinValue;
        float farest_z = asc ? float.MaxValue : float.MinValue;
#if EIJIS_CALLSHOT
        for (int i = (table.isPyramid ? 0 : 1); i <= table.ballsLengthByPocketGame; i++)
#else
        for (int i = 0; i < BilliardsModule.MAX_BALLS; i++)
#endif
        {
            if (i == beforeId)
            {
                continue;
            }
            
            if (((ballsPocketed >> i) & 0x1u) != 0)
            {
                continue;
            }

#if EIJIS_DEBUG_BALLORDER
            table._LogInfo($"  current checking ball id = {i}");
#endif
            float current_x = table.ballsP[i].x;
            float current_z = table.ballsP[i].z;
#if EIJIS_DEBUG_BALLORDER
            table._LogInfo($"  before_x = {before_x}, current_x = {current_x}");
#endif
            if (before_x == current_x)
            {
#if EIJIS_DEBUG_BALLORDER
                // table._LogInfo($"  nearest_z = {nearest_z}, before_z = {before_z}, current_z = {current_z} *");
#endif
                if ((asc && current_z < before_z) || (!asc && current_z > before_z))
                {
                    if ((asc && (current_x < nearest_x || (nearest_x == current_x && nearest_z < current_z))) || (!asc && (current_x > nearest_x || (nearest_x == current_x && nearest_z > current_z))))
                    {
                        nearest_x = current_x;
                        nearest_z = current_z;
                        id = i;
#if EIJIS_DEBUG_BALLORDER
                        // table._LogInfo($"  found ball by z id = {id} *");
#endif
                    }
                }
            }
            else
            {
                if ((asc && before_x < current_x) || (!asc && before_x > current_x))
                {
                    if ((asc && current_x < nearest_x) || (!asc && current_x > nearest_x))
                    {
                        nearest_x = current_x;
                        nearest_z = current_z;
                        id = i;
#if EIJIS_DEBUG_BALLORDER
                        // table._LogInfo($"  found ball by x id = {id}");
#endif
                    }
                    else if (current_x == nearest_x)
                    {
#if EIJIS_DEBUG_BALLORDER
                        // table._LogInfo($"  nearest_z = {nearest_z}, before_z = {before_z}, current_z = {current_z} **");
#endif
                        if ((asc && nearest_z < current_z) || (!asc && nearest_z > current_z))
                        {
                            nearest_x = current_x;
                            nearest_z = current_z;
                            id = i;
#if EIJIS_DEBUG_BALLORDER
                            // table._LogInfo($"  found ball by z id = {id} **");
#endif
                        }
                    }
                }
            }

#if EIJIS_DEBUG_BALLORDER
            table._LogInfo($"  farest_x = {farest_x}, current_x = {current_x}");
#endif
            if ((!asc && farest_x < current_x) || (asc && farest_x > current_x))
            {
                farest_x = current_x;
                farest_z = current_z;
                farestId = i;
#if EIJIS_DEBUG_BALLORDER
                table._LogInfo($"  found ball by x farestId = {farestId}");
#endif
            }
            else if (current_x == farest_x)
            {
#if EIJIS_DEBUG_BALLORDER
                table._LogInfo($"  farest_z = {farest_z}, current_z = {current_z}");
#endif
                if ((!asc && current_z < farest_z) || (asc && current_z > farest_z))
                {
                    farest_z = current_z;
                    farestId = i;
#if EIJIS_DEBUG_BALLORDER
                    table._LogInfo($"  found ball by z farestId = {farestId}");
#endif
                }
            }
        }

#if EIJIS_DEBUG_BALLORDER
        // table._LogInfo($"  nearest_x = {nearest_x}");
        table._LogInfo($"  reverse side farestId = {farestId}");
#endif
        if (nearest_x == float.MaxValue || nearest_x == float.MinValue)
        {
            id = farestId;
        }

        return id;
    }
    
#endif
#if EIJIS_CALLSHOT
    private int[] pocketOrder = new[] { 0, 1, 5, 3, 2, 4 };

    private int nextPocketOrder(bool asc)
    {
        uint pockets = table.pointPocketsLocal;
        int pocketCount = table.pocketLocations.Length;
        int id = (asc ? 0 : pocketOrder[pocketOrder.Length - 1]);
        for (int i = 0; i < pocketCount; i++)
        {
            if (((pockets >> i) & 0x1u) != 0)
            {
                int current = Array.IndexOf(pocketOrder, i);
                int next = current + (asc ? 1 : -1);
                if (next < 0 || pocketCount <= next)
                {
                    id = i;
                    break;
                }

                id = pocketOrder[next];
                break;
            }
        }

        return id;
    }

#endif
#if EIJIS_CUEBALLSWAP || EIJIS_CALLSHOT
    private void updateCallShotIndicator()
    {
#if EIJIS_CALLSHOT
#if EIJIS_10BALL
        if ((!table.isPyramid && (!table.requireCallShotLocal || (!table.is8Ball && !table.is9Ball && !table.is10Ball))) || (!callCueBall.activeSelf && !callShot.activeSelf && !pushOut.activeSelf)) return;
#else
        if ((!table.isPyramid && (!table.requireCallShotLocal || (!table.is8Ball && !table.is9Ball))) || (!callCueBall.activeSelf && !callShot.activeSelf && !pushOut.activeSelf)) return;
#endif
#else
#if EIJIS_PUSHOUT
        if (!table.isPyramid || !callCueBall.activeSelf) return;
#else
        if (!table.isPyramid) return;
#endif
#endif
        
        if (Input.GetKeyDown(KeyCode.B) )
        {
            int id = nextBallOrder(!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)));
            table._TriggerOtherBallHit(id, true);
        }
#if EIJIS_CALLSHOT

        if (table.isPyramid) return;
        
        if (Input.GetKeyDown(KeyCode.P))
        { 
            int id = nextPocketOrder(!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)));
            table._TriggerPocketHit(id, true);
        }
#endif
    }

#endif
#if EIJIS_PUSHOUT
    private void updatePushOutIndicator()
    {
#if EIJIS_10BALL
        if ((!table.is8Ball && !table.is9Ball && !table.is10Ball) || !pushOut.activeSelf) return;
#else
        if ((!table.is8Ball && !table.is9Ball) || !pushOut.activeSelf) return;
#endif
        
        if (Input.GetKeyDown(KeyCode.O))
        { 
            table._PushOut();
        }
    }
    
#endif
#if EIJIS_MNBK_AUTOCOUNTER
    private void updateCallSafetyIndicator()
    {
        if (!table.isMnbk9Ball) return;
#if EIJIS_CALLSHOT
        if (table.requireCallShotLocal) return;
#endif
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            table._CallSafety();
        }
    }
    private void updatePausedIndicator()
    {
        if (!table.isMnbk9Ball) return;
#if EIJIS_CALLSHOT
        if (table.requireCallShotLocal) return;
#endif

        if (Input.GetKeyDown(KeyCode.P))
        {
            table._Pause();
        }
    }

#endif
    private void renderCuePosition(Vector3 dir)
    {
        CueController cue = table.activeCue;
        cue.UpdateDesktopPosition(); // otherwise it spazzes out if FPS > FixedUpdate rate

        float a = spin.x * k_BALL_RADIUS;
        float b = spin.z * k_BALL_RADIUS;
        float c = Mathf.Sqrt(Mathf.Pow(k_BALL_RADIUS, 2) - Mathf.Pow(a, 2) - Mathf.Pow(b, 2));

        float dist = (cue._GetCuetip().transform.position - cue._GetDesktopMarker().transform.position).magnitude;
        dist += power; // show the amount of power being applied
        dist += 0.05f; // add some extra distance so the cue tip isn't touching the ball

        Vector3 ballHitPos = new Vector3(a, b, -c);
        Vector3 cueGripPos = new Vector3(a, b + dist * Mathf.Sin(jumpAngle), -(c + dist * Mathf.Cos(jumpAngle)));

        Quaternion spinRot = Quaternion.AngleAxis(Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg, Vector3.up);
        Transform tableSurface = table.tableSurface;
        cue._GetDesktopMarker().transform.position = tableSurface.TransformPoint(table.ballsP[0] + (spinRot * cueGripPos));
        cue._GetDesktopMarker().transform.LookAt(tableSurface.TransformPoint(table.ballsP[0] + (spinRot * ballHitPos)));
    }

    private void stopRepositioning()
    {
        isRepositioning = false;
        if (currentRepositioner != null)
        {
            table.repositionManager._EndReposition(currentRepositioner);
            currentRepositioner = null;
        }
    }

    private void enterUI()
    {
        inUI = true;
        repositionMode = false;
        root.SetActive(true);
        Networking.LocalPlayer.Immobilize(true);

        Camera desktopCamera = root.GetComponentInChildren<Camera>();
        table.cameraOverrideModule.shouldMaintainAspectRatio = true;
        table.cameraOverrideModule.aspectRatio = new Vector2(1920, 1080);
        table.cameraOverrideModule._SetTargetCamera(desktopCamera);
        table.cameraOverrideModule._SetRenderMode(CAMERA_RENDER_MODE_DESKTOP);

        // table.activeCue.RequestSerialization();

        if (canShoot)
        {
            table._TriggerOnPlayerPrepareShoot();
        }

        cursorClampX = table.k_TABLE_WIDTH + .3f;
        cursorClampZ = table.k_TABLE_HEIGHT + .3f;
        
#if EIJIS_MNBK_AUTOCOUNTER
        if (ReferenceEquals(null, desktopMnbk))
        {
            table._LogInfo("  desktop_mnbk object not set.");
        }
        else
        {
#if EIJIS_CALLSHOT
            desktopMnbk.SetActive(table.isMnbk9Ball && !table.requireCallShotLocal);
#else
            desktopMnbk.SetActive(table.isMnbk9Ball);
#endif
        }
        if (ReferenceEquals(null, safetyCalled))
        {
            table._LogInfo("  desktop_safetycalled object not set.");
        }
        if (ReferenceEquals(null, paused))
        {
            table._LogInfo("  desktop_paused object not set.");
        }
#endif
    }

    private void exitUI()
    {
        stopShooting();
        stopRepositioning();
        resetShootState();
        inUI = false;
        root.SetActive(false);
        table.cameraOverrideModule._SetRenderMode(CAMERA_RENDER_MODE_DISABLED);
        Networking.LocalPlayer.Immobilize(false);
    }

    private void stopShooting()
    {
        isShooting = false;
        cursor = initialCursorPosition;
        cursorIndicator.SetActive(true);
        cursorClampX = table.k_TABLE_WIDTH + .3f;
        cursorClampZ = table.k_TABLE_HEIGHT + .3f;
    }

    private void resetShootState()
    {
        spin = Vector3.zero;
        jumpAngle = 0;
        power = 0;
    }

    public void _AllowShoot()
    {
        canShoot = true;

        if (inUI)
        {
            table._TriggerOnPlayerPrepareShoot();
        }
    }

    public void _DenyShoot()
    {
        canShoot = false;
        resetShootState();
    }

    public bool _IsInUI()
    {
        return inUI;
    }

    public bool _IsShooting()
    {
        return canShoot;
    }
#if EIJIS_PUSHOUT
#if EIJIS_CUEBALLSWAP
    
    public void _CallCueBallSetActive(bool show)
    {
        callCueBall.SetActive(show);
    }
#endif
    
    public void _PushOutSetActive(bool show)
    {
        pushOut.SetActive(show);
    }
#endif
#if EIJIS_CALLSHOT
    
    public void _CallShotSetActive(bool show)
    {
        callShot.SetActive(show);
    }
#endif
}
