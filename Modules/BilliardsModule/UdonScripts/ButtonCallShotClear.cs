
using UdonSharp;
using UnityEngine;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class ButtonCallShotClear : UdonSharpBehaviour
{
    [SerializeField] BilliardsModule table;

    public override void Interact()
    {
        table._CallClear();
    }
}
