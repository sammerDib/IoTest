using System;

namespace Common.WinForms
{
    public interface Parser<T>
    {
        /// <summary>
        /// FormatException
        /// </summary>
        T Parse(string text);
    }
}
