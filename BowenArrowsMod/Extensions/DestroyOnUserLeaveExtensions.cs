using FrooxEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace BowenArrowsMod.Extensions;
internal static class DestroyOnUserLeaveExtensions
{
    internal static DestroyOnUserLeave DestroyWhenLocalUserLeaves(this Slot slot)
    {
        slot.PersistentSelf = false;

        if (slot.GetComponents<DestroyOnUserLeave>().FirstOrDefault(destroy => destroy.TargetUser.Target == slot.LocalUser) is DestroyOnUserLeave destroy)
            return destroy;

        return slot.DestroyWhenUserLeaves(slot.LocalUser);
    }
}
