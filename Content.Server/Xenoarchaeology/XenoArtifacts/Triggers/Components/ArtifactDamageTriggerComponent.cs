// SPDX-FileCopyrightText: 2022 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Damage.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components;

/// <summary>
/// Triggers when a certain threshold of damage of certain types is reached
/// </summary>
[RegisterComponent]
public sealed partial class ArtifactDamageTriggerComponent : Component
{
    /// <summary>
    /// What damage types are accumulated for the trigger?
    /// </summary>
    [DataField("damageTypes", customTypeSerializer: typeof(PrototypeIdListSerializer<DamageTypePrototype>))]
    public List<string>? DamageTypes;

    /// <summary>
    /// What threshold has to be reached before it is activated?
    /// </summary>
    [DataField("damageThreshold", required: true)]
    public float DamageThreshold;

    /// <summary>
    /// How much damage has been accumulated on the artifact so far
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float AccumulatedDamage = 0;
}
