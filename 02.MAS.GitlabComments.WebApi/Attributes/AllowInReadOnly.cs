namespace MAS.GitlabComments.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class AllowInReadOnlyAttribute : Attribute
    {
    }
}
