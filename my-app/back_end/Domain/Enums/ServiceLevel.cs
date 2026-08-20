namespace Domain.Enums
{
    /// <summary>
    /// Service level offered by a tour program (مستوى الخدمة). A single value is
    /// chosen per program; new programs default to <see cref="Economy"/> (الدرجة الاقتصادية).
    /// </summary>
    public enum ServiceLevel
    {
        /// <summary>خدمة اقتصادية.</summary>
        Economy = 0,

        /// <summary>خدمة عادية.</summary>
        Standard = 1,

        /// <summary>خدمة مميزة.</summary>
        Premium = 2,

        /// <summary>خدمة من الدرجة الأولى.</summary>
        FirstClass = 3
    }
}
