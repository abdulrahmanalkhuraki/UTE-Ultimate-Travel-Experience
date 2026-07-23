namespace Domain.Enums
{
    /// <summary>
    /// Flight cabin classes a program may make available (تذاكر الطيران المتاحة).
    /// A program can offer several of these (multi-select) and the field is optional.
    /// </summary>
    public enum FlightCabinClass
    {
        Economy = 0,

        PremiumEconomy = 1,

        Business = 2,

        First = 3
    }
}
