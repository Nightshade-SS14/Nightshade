using Content.Server.Body.Systems;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.Body.Components;

[RegisterComponent]
//[Access(typeof(ThermalRegulatorSystem))] // Trauma
public sealed partial class ThermalRegulatorComponent : Component
{
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan NextUpdate;

    [DataField]
    public TimeSpan UpdateInterval = TimeSpan.FromSeconds(1);

    [DataField]
    public float MetabolismHeat;

    [DataField]
    public float RadiatedHeat;

    [DataField]
    public float SweatHeatRegulation;

    [DataField]
    public float ShiveringHeatRegulation;

    [DataField]
    public float ImplicitHeatRegulation;

    [DataField]
    public float NormalBodyTemperature;

    [DataField]
    public float ThermalRegulationTemperatureThreshold;

    // Nightshade Start
    [DataField]
    public bool ProcessWhileDead = true;

    [DataField]
    public bool ProcessWhileCrit = true;
    // Nightshade End
}
