// SPDX-FileCopyrightText: 2022 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared.Morgue.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class MorgueComponent : Component
{
    /// <summary>
    ///     Whether or not the morgue beeps if a living player is inside.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("doSoulBeep")]
    public bool DoSoulBeep = true;

    [ViewVariables]
    public float AccumulatedFrameTime = 0f;

    /// <summary>
    ///     The amount of time between each beep.
    /// </summary>
    [ViewVariables]
    public float BeepTime = 10f;

    [DataField("occupantHasSoulAlarmSound")]
    public SoundSpecifier OccupantHasSoulAlarmSound = new SoundPathSpecifier("/Audio/Weapons/Guns/EmptyAlarm/smg_empty_alarm.ogg");
}
