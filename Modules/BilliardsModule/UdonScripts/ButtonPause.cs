//#define MNBK_BACKOUT_PATCH

using UdonSharp;
using UnityEngine;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class ButtonPause : UdonSharpBehaviour
{
    [SerializeField] BilliardsModule table;

    public override void Interact()
    {
#if !MNBK_BACKOUT_PATCH
        table._Pause();
#endif
    }
}
