namespace UnitySC.Equipment.Abstractions.Enums
{
    /// <summary>
    /// Designate the type of effector at the end of robot arm.
    /// </summary>
    public enum EffectorType
    {
        None,
        VacuumI, // aka blade
        VacuumU, // aka fork or Y
        EdgeGrid,
        FilmFrame
    }
}
