// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Damage;
using Robust.Shared.Prototypes;

namespace Content.Trauma.Shared.MartialArts.Components;

// Nightshade Start - Omu Dragon Kung Fu translated to Trauma's martial-arts framework

/// <summary>
/// Marks the Trauma knowledge entity for Dragon Kung Fu.
/// </summary>
[RegisterComponent]
public sealed partial class DragonKungFuKnowledgeComponent : Component;

/// <summary>
/// Grants permanent Dragon Kung Fu knowledge when the manual is used in hand.
/// </summary>
[RegisterComponent]
public sealed partial class GrantDragonKungFuComponent : Component
{
    [DataField]
    public EntProtoId Knowledge = "MartialArtKungFuDragon";

    [DataField]
    public int Level = 1;

    [DataField]
    public LocId LearnMessage = "dragon-success-learned";
}

/// <summary>
/// Runtime state placed on the mob while Dragon Kung Fu is the active martial art.
/// Reproduces Omu's Dragon Power defensive reduction and temporary damage buff.
/// </summary>
[RegisterComponent]
public sealed partial class DragonPowerComponent : Component
{
    [DataField]
    public DamageModifierSet ModifierSet = new()
    {
        Coefficients =
        {
            { "Blunt", 0.6f },
            { "Slash", 0.6f },
            { "Piercing", 0.6f },
            { "Heat", 0.6f },
        },
        IgnoreArmorPierceFlags = (int) PartialArmorPierceFlags.All,
    };

    [DataField]
    public float DamageMultiplier = 1.2f;

    [DataField]
    public TimeSpan AttackDamageBuffDuration = TimeSpan.FromSeconds(5);

    [ViewVariables(VVAccess.ReadOnly)]
    public TimeSpan AttackBuffUntil = TimeSpan.Zero;
}

// Nightshade End
