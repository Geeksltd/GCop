namespace GCop.Thread.Utilities
{
    using Core;
    using System;
    using System.Linq;

    /// <summary>
    /// Generates an identifier for a given string value.
    /// </summary>
    internal class IdentifierGenerator
    {
        #region Value
        /// <summary>
        /// Gets or sets the Value of this IdentifierGenerator.
        /// </summary>
        public string Value { get; private set; }
        #endregion

        /// <summary>
        /// Creates a new IdentifierGenerator instance.
        /// </summary>
        public IdentifierGenerator(string value)
        {
            if (value.IsEmpty())
                throw new ArgumentNullException("value");

            Value = value.Trim();
        }

        void Replace(string from, string to)
        {
            while (Value.Contains(from))
                Value = Value.Replace(from, to);
        }

        void Remove(params char[] characters)
        {
            foreach (var c in characters)
            {
                while (Value.Contains(c))
                    Value = Value.Replace(c.ToString(), string.Empty);
            }
        }

        void ConvertToPascalCase()
        {
            for (var index = 0; index < Value.Length; index++)
            {
                var isFirstLetter = index == 0 || Value[index - 1] == ' ';
                var character = Value[index];

                if (isFirstLetter)
                {
                    if (char.IsLower(character))
                    {
                        Value = Value.Remove(index, 1);
                        Value = Value.Insert(index, char.ToUpper(character).ToString());
                    }

                    if (index > 0 && Value[index - 1] == ' ')
                    {
                        Value = Value.Remove(index - 1, 1);
                    }
                }
            }
        }

        /// <summary>
        /// Builds the identifer
        /// </summary>
        public string Build()
        {
            Remove('\'');
            Replace("  ", " ");
            Replace("&", "And");

            foreach (var c in Value)
            {
                if (c == '_' || c == ' ') continue;

                if (!char.IsLetterOrDigit(c))
                    Replace(c.ToString(), "_");
            }

            // Replace("__", "_");

            Value = Value.Trim('_', ' ', '\r', '\n', '\t');

            ConvertToPascalCase();

            while (Value.ContainsAny(new[] { " _", "_ ", "__" }))
            {
                Replace(" _", "_");
                Replace("_ ", "_");
                Replace("__", "_");
            }

            if (Value.HasValue())
            {
                if (char.IsDigit(Value[0])) Value = "_" + Value;
                return Value;
            }
            else return string.Empty;
        }
    }
}
