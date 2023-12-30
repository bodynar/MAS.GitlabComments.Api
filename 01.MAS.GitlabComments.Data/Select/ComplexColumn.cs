namespace MAS.GitlabComments.DataAccess.Select
{
    using System;

    /// <summary>
    /// Column with complex path data
    /// </summary>
    public class ComplexColumn
    {
        /// <summary>
        /// Source table alias
        /// </summary>
        public string TableAlias { get; set; }

        /// <summary>
        /// Column name in table
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Column alias (name of property to project)
        /// </summary>
        public string Alias { get; set; }

        /// <inheritdoc cref="Object.ToString"/>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(TableAlias) || string.IsNullOrEmpty(Name))
            {
                throw new ArgumentException($"Required string data is empty: TableAlias = \"{TableAlias}\", Name = \"{Name}\"");
            }

            return string.IsNullOrEmpty(Alias) || Alias == Name
                ? $"[{TableAlias}].[{Name}]"
                : $"[{TableAlias}].[{Name}] AS [{Alias}]";
        }
    }
}
