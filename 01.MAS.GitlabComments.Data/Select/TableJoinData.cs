namespace MAS.GitlabComments.Data.Select
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

        /// <inheritdoc cref="Object.Equals"/>
        public override bool Equals(object obj)
        {
            if (obj is TableJoinData compareTableJoinData)
            {
                return
                    LeftTableName == compareTableJoinData.LeftTableName
                    && RightTableName == compareTableJoinData.RightTableName
                    && LeftTableRelationColumn == compareTableJoinData.LeftTableRelationColumn
                    && RightTableRelationColumn == compareTableJoinData.RightTableRelationColumn;
            }

            return base.Equals(obj);
        }
    }
}
