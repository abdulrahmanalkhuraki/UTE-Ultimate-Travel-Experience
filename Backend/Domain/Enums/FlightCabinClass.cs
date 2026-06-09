namespace Domain.Enums
{
    /// <summary>
    /// Flight cabin classes a program may make available (تذاكر الطيران المتاحة).
    /// A program can offer several of these (multi-select) and the field is optional.
    /// </summary>
    public enum FlightCabinClass
    {
        /// <summary>الدرجة الاقتصادية.</summary>
        Economy = 0,

        /// <summary>الدرجة السياحية المميزة.</summary>
        PremiumEconomy = 1,

        /// <summary>درجة رجال الأعمال.</summary>
        Business = 2,

        /// <summary>الدرجة الأولى.</summary>
        First = 3
    }
}
