// Nightshade Start - BSO target objective immunity
using Content.Server._DV.Objectives.Components;
// Nightshade End
using Content.Server.Objectives.Components;
using Content.Shared.Mind;
using Content.Shared.Objectives.Components;
using Content.Server.GameTicking.Rules;
using Content.Server.Revolutionary.Components;
using Robust.Shared.Random;
using System.Linq;

namespace Content.Server.Objectives.Systems;

/// <summary>
/// Handles assinging a target to an objective entity with <see cref="TargetObjectiveComponent"/> using different components.
/// These can be combined with condition components for objective completions in order to create a variety of objectives.
/// </summary>
public sealed partial class PickObjectiveTargetSystem : EntitySystem
{
    [Dependency] private TargetObjectiveSystem _target = default!;
    [Dependency] private SharedMindSystem _mind = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PickSpecificPersonComponent, ObjectiveAssignedEvent>(OnSpecificPersonAssigned);
        SubscribeLocalEvent<PickRandomPersonComponent, ObjectiveAssignedEvent>(OnRandomPersonAssigned);
    }

    private void OnSpecificPersonAssigned(Entity<PickSpecificPersonComponent> ent, ref ObjectiveAssignedEvent args)
    {
        // invalid objective prototype
        if (!TryComp<TargetObjectiveComponent>(ent.Owner, out var target))
        {
            args.Cancelled = true;
            return;
        }

        // target already assigned
        if (target.Target != null)
            return;

        if (args.Mind.OwnedEntity == null)
        {
            args.Cancelled = true;
            return;
        }

        var user = args.Mind.OwnedEntity.Value;
        if (!TryComp<TargetOverrideComponent>(user, out var targetComp) || targetComp.Target == null)
        {
            args.Cancelled = true;
            return;
        }

        // Nightshade Start - BSO target objective immunity
        if (HasComp<TargetObjectiveImmuneComponent>(targetComp.Target.Value))
        {
            args.Cancelled = true;
            return;
        }

        if (!TryComp<MindComponent>(targetComp.Target.Value, out var targetMind) ||
            targetMind.OwnedEntity == null ||
            HasComp<TargetObjectiveImmuneComponent>(targetMind.OwnedEntity.Value))
        {
            args.Cancelled = true;
            return;
        }
        // Nightshade End

        _target.SetTarget(ent.Owner, targetComp.Target.Value);
    }

    private void OnRandomPersonAssigned(Entity<PickRandomPersonComponent> ent, ref ObjectiveAssignedEvent args)
    {
        // invalid objective prototype
        if (!TryComp<TargetObjectiveComponent>(ent, out var target))
        {
            args.Cancelled = true;
            return;
        }

        // target already assigned
        if (target.Target != null)
            return;

        // couldn't find a target :(
        // Nightshade Start - BSO target objective immunity
        if (_mind.PickFromPoolFiltered(
                ent.Comp.Pool,
                mind => !HasComp<TargetObjectiveImmuneComponent>(mind.Owner) &&
                        mind.Comp.OwnedEntity is { } entity &&
                        !HasComp<TargetObjectiveImmuneComponent>(entity),
                args.MindId,
                ent.Comp.Conditions) is not {} picked)
        // Nightshade End
        {
            args.Cancelled = true;
            return;
        }

        _target.SetTarget(ent, picked, target);
    }
}
