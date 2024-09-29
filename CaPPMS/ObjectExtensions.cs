namespace CaPPMS
{
    /// <summary>
    /// Common Extensions.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Return null safe object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string NullSafeToString(this object obj)
        {
            return obj?.ToString() ?? "(null)";
        }
    }
}
