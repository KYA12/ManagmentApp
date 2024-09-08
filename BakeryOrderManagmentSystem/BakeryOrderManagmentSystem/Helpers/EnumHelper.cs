namespace BakeryOrderManagmentSystem.Helpers
{
    using System;

    public static class EnumHelper
    {
        public static T ConvertStringToEnum<T>(string status) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("String cannot be null or whitespace.", nameof(status));
            }

            try
            {
                return (T)Enum.Parse(typeof(T), status, ignoreCase: true);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException($"Invalid value for enum type {typeof(T).Name}: {status}");
            }
        }
        public static T? TryConvertStringToEnum<T>(string status) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return null;
            }

            if (Enum.TryParse(status, ignoreCase: true, out T result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
