// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;
using Content.Trauma.Common.Knowledge.Components;
using Content.Trauma.Shared.Knowledge.Systems;
using Content.Trauma.Shared.MartialArts.Components;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Trauma.Shared.MartialArts;

// Nightshade Start - Omu Dragon Kung Fu translated to Trauma's martial-arts framework

/// <summary>
/// Trauma-native Dragon Kung Fu integration.
/// Combo execution remains data-driven through Trauma's ComboPrototype and
/// entity-effect systems; this handles the manual and Dragon Power.
/// </summary>
public sealed partial class DragonKungFuSystem : EntitySystem
{
    [Dependency] private SharedHandsSystem _hands = default!;
    [Dependency] private SharedKnowledgeSystem _knowledge = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private INetManager _net = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GrantDragonKungFuComponent, UseInHandEvent>(OnUseManual);
        SubscribeLocalEvent<DragonKungFuKnowledgeComponent, KnowledgeEnabledEvent>(OnKnowledgeEnabled);
        SubscribeLocalEvent<DragonKungFuKnowledgeComponent, KnowledgeDisabledEvent>(OnKnowledgeDisabled);
        SubscribeLocalEvent<DragonPowerComponent, AttackedEvent>(OnAttacked);
        SubscribeLocalEvent<DragonPowerComponent, GetUserMeleeDamageEvent>(OnMeleeDamage);
    }

    private void OnUseManual(Entity<GrantDragonKungFuComponent> ent, ref UseInHandEvent args)
    {
        if (args.Handled || !_net.IsServer)
            return;

        _knowledge.AddKnowledgeUnits(
            args.User,
            new Dictionary<EntProtoId, int>
            {
                [ent.Comp.Knowledge] = ent.Comp.Level,
            });

        _popup.PopupEntity(Loc.GetString(ent.Comp.LearnMessage), args.User, args.User);
        QueueDel(ent.Owner);
        args.Handled = true;
    }

    private void OnKnowledgeEnabled(Entity<DragonKungFuKnowledgeComponent> ent, ref KnowledgeEnabledEvent args)
    {
        var power = EnsureComp<DragonPowerComponent>(args.Holder);
        power.AttackBuffUntil = TimeSpan.Zero;
    }

    private void OnKnowledgeDisabled(Entity<DragonKungFuKnowledgeComponent> ent, ref KnowledgeDisabledEvent args)
    {
        RemComp<DragonPowerComponent>(args.Holder);
    }

    private void OnAttacked(Entity<DragonPowerComponent> ent, ref AttackedEvent args)
    {
        // Omu Dragon Power only gives the defensive benefit while unarmed.
        if (_hands.TryGetActiveItem(ent.Owner, out _))
            return;

        args.ModifiersList.Add(ent.Comp.ModifierSet);
        ent.Comp.AttackBuffUntil = _timing.CurTime + ent.Comp.AttackDamageBuffDuration;
    }

    private void OnMeleeDamage(Entity<DragonPowerComponent> ent, ref GetUserMeleeDamageEvent args)
    {
        if (_timing.CurTime >= ent.Comp.AttackBuffUntil)
            return;

        // Omu's counterattack buff applies to both armed and unarmed melee.
        args.Damage *= ent.Comp.DamageMultiplier;
    }
}

// Nightshade End
