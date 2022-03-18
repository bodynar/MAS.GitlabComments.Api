﻿namespace MAS.GitlabComments.Data.Select
{
    using System;

    /// <summary>
    /// Information about table join
    /// </summary>
        #pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class TableJoinData
        #pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
        /// <summary>
        /// Source string configuration
        /// </summary>
        public string Configuration { get; }

        /// <summary>
        /// Left table name
        /// </summary>
        public string LeftTableName { get; set; }

        /// <summary>
        /// Name of relation column from left table
        /// </summary>
        public string LeftTableRelationColumn { get; set; }

        /// <summary>
        /// Right table name
        /// </summary>
        public string RightTableName { get; set; }

        /// <summary>
        /// Name of relation column from right table
        /// </summary>
        public string RightTableRelationColumn { get; set; }

        /// <summary>
        /// Tables join type
        /// </summary>
        public TableJoinType JoinType { get; set; }

        /// <summary>
        /// Left table alias
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Initializing <see cref="TableJoinData"/>
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <exception cref="ArgumentNullException">Parameter configuration is null</exception>
        public TableJoinData(string configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <inheritdoc cref="Object.Equals"/>
        public override bool Equals(object obj)
        {
            if (obj is TableJoinData compareTableJoinData)
            {
                return Configuration == compareTableJoinData.Configuration;
            }

            return base.Equals(obj);
        }
    }
}
