namespace MAS.GitlabComments.Logic.Services.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MAS.GitlabComments.Data;
    using MAS.GitlabComments.DataAccess.Filter;
    using MAS.GitlabComments.DataAccess.Services;
    using MAS.GitlabComments.Logic.Models;

    using Newtonsoft.Json;

    /// <summary>
    /// Provider of read and update of <see cref="SystemVariable"/> entities
    /// </summary>
    public class SystemVariableProvider : ISystemVariableProvider
    {
        /// <summary> Data provider of <see cref="SystemVariable"/> </summary>
        protected IDataProvider<SystemVariable> DataProvider { get; }

        /// <summary>
        /// Ignored data value types
        /// </summary>
        protected static IEnumerable<Type> IgnoredTypes { get; } = new[] {
            typeof(ushort), typeof(uint), typeof(ulong),
        };

        /// <summary>
        /// Initializing <see cref="SystemVariableProvider"/>
        /// </summary>
        /// <param name="dataProvider">Instance of data provider of <see cref="SystemVariable"/></param>
        /// <exception cref="ArgumentNullException">Param dataProvider is null</exception>
        public SystemVariableProvider(
            IDataProvider<SystemVariable> dataProvider
        )
        {
            DataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
        }

        /// <inheritdoc cref="ISystemVariableProvider.GetAllVariables()"/>
        public IEnumerable<SysVariableDisplayModel> GetAllVariables()
        {
            return DataProvider
                .Select<SysVariableDisplayModel>()
                .ToList();
        }

        /// <inheritdoc cref="ISystemVariableProvider.GetValue{TValue}(Guid)"/>
        public TValue GetValue<TValue>(Guid variableId)
        {
            if (variableId == default || !IsRequiredTypeValid(typeof(TValue)))
            {
                return default;
            }

            var variable = Get(variableId);

            if (variable == default)
            {
                return default;
            }

            var value = Parse<TValue>(variable);

            return value;
        }

        /// <inheritdoc cref="ISystemVariableProvider.GetValue{TValue}(string)"/>
        public TValue GetValue<TValue>(string code)
        {
            if (string.IsNullOrWhiteSpace(code) || !IsRequiredTypeValid(typeof(TValue)))
            {
                return default;
            }

            var variable = Get(code);

            if (variable == default)
            {
                return default;
            }

            var value = Parse<TValue>(variable);

            return value;
        }

        /// <inheritdoc cref="ISystemVariableProvider.Get(Guid)"/>
        public SysVariable Get(Guid variableId)
        {
            if (variableId == default)
            {
                return default;
            }

            var variable = DataProvider.Get(variableId);

            if (variable == default)
            {
                return default;
            }

            var variableModel = BuildVariableModel(variable);

            return variableModel;
        }

        /// <inheritdoc cref="ISystemVariableProvider.Get(string)"/>
        public SysVariable Get(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return default;
            }

            var variable = DataProvider.Where(
                new FilterGroup()
                {
                    Items = new[] {
                        new FilterItem
                        {
                            FieldName = nameof(SystemVariable.Code),
                            LogicalComparisonType = ComparisonType.Equal,
                            Value = code,
                            Name = "CodeEquality"
                        }
                    }
                }
            ).FirstOrDefault();

            if (variable == default)
            {
                return default;
            }

            var variableModel = BuildVariableModel(variable);

            return variableModel;
        }

        /// <inheritdoc cref="ISystemVariableProvider.Set{TValue}(Guid, TValue)"/>
        public void Set<TValue>(Guid variableId, TValue value)
        {
            if (variableId == default)
            {
                return;
            }

            var variable = Get(variableId);

            if (variable == default)
            {
                return;
            }

            DataProvider.Update(
                variableId,
                new Dictionary<string, object>() {
                    { nameof(SystemVariable.RawValue), value?.ToString() ?? "" }
                }
            );
        }

        /// <inheritdoc cref="ISystemVariableProvider.Set{TValue}(string, TValue)"/>
        public void Set<TValue>(string code, TValue value)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return;
            }

            var variable = Get(code);

            if (variable == default)
            {
                return;
            }

            DataProvider.Update(
                variable.Id,
                new Dictionary<string, object>() {
                    { nameof(SystemVariable.RawValue), value?.ToString() ?? "" }
                }
            );
        }

        #region Not public API

        /// <summary>
        /// Build system variable model from system variable entity data
        /// </summary>
        /// <param name="variable">Entity data</param>
        /// <returns>An instance of <see cref="SysVariable"/> if it's possible to build a such model; otherwise - <see langword="default"/></returns>
        protected static SysVariable BuildVariableModel(SystemVariable variable)
        {
            if (variable == default || string.IsNullOrWhiteSpace(variable.Type))
            {
                return default;
            }

            var underlyingType = GetUnderlyingType(variable.Type);

            if (underlyingType == default)
            {
                return default;
            }

            return new SysVariable()
            {
                Id = variable.Id,
                Caption = variable.Caption,
                Code = variable.Code,
                Type = variable.Type,
                RawValue = variable.RawValue,
                UnderlyingType = underlyingType,
            };
        }

        /// <summary>
        /// Get <see cref="Type"/> from system variable type column value
        /// </summary>
        /// <param name="type">Type column value</param>
        /// <returns>An instance of <see cref="Type"/> if type column has valid value; otherwise - <see langword="null"/></returns>
        protected static Type GetUnderlyingType(string type)
        {
            var isTypeValid = Enum.TryParse<SysVariableType>(type, true, out var enumType);

            if (!isTypeValid || enumType == SysVariableType.None)
            {
                // TODO: log
                return null;
            }

            switch (enumType)
            {
                case SysVariableType.String:
                    return typeof(string);
                case SysVariableType.Int:
                    return typeof(long);
                case SysVariableType.Decimal:
                    return typeof(double);
                case SysVariableType.Bool:
                    return typeof(bool);
                case SysVariableType.Json:
                    return typeof(object);
            }

            return null;
        }

        /// <summary>
        /// Get is required data value type valid
        /// </summary>
        /// <param name="requiredType">Required type as instance of <see cref="Type"/></param>
        /// <returns><see langword="true"/> if type is valid; otherwise - <see langword="false"/></returns>
        protected static bool IsRequiredTypeValid(Type requiredType)
        {
            return !IgnoredTypes.Contains(requiredType);
        }

        /// <summary>
        /// Parse variable value to required type if it possible
        /// </summary>
        /// <typeparam name="TValue">Required variable value type</typeparam>
        /// <param name="variable">Variable data</param>
        /// <returns>Variable value in required type, if conversation is possible; otherwise - <see langword="default"/></returns>
        protected static TValue Parse<TValue>(SysVariable variable)
        {
            if (variable == default || variable.UnderlyingType == default)
            {
                return default;
            }

            var requiredType = typeof(TValue);
            
            if (requiredType == variable.UnderlyingType)
            {
                return (TValue)Convert.ChangeType(variable.RawValue, requiredType);
            }

            Enum.TryParse<SysVariableType>(variable.Type, true, out var enumType);
            
            if (enumType == SysVariableType.Json)
            {
                return JsonConvert.DeserializeObject<TValue>(variable.RawValue);
            }

            if (enumType == SysVariableType.String)
            {
                if (requiredType == typeof(char))
                {
                    return (TValue)Convert.ChangeType(variable.RawValue[0], requiredType);
                }

                if (requiredType == typeof(char[]))
                {
                    return (TValue)Convert.ChangeType(variable.RawValue.ToCharArray(), requiredType);
                }
            }
            if (enumType == SysVariableType.Int)
            {
                var isNumber = long.TryParse(variable.RawValue, out var number);

                if (isNumber)
                {
                    if (requiredType == typeof(short)
                        && number <= short.MaxValue
                        && number >= short.MinValue
                    )
                    {
                        return (TValue)Convert.ChangeType(variable.RawValue, requiredType);
                    }
                    else if (requiredType == typeof(int)
                        && number <= int.MaxValue
                        && number >= int.MinValue
                    )
                    {
                        return (TValue)Convert.ChangeType(variable.RawValue, requiredType);
                    }
                }
            }
            if (enumType == SysVariableType.Decimal)
            {
                var isDecimal = double.TryParse(variable.RawValue, out var number);

                if (isDecimal)
                {
                    if (requiredType == typeof(decimal)
                        && number.CompareTo((double)decimal.MaxValue) < 0
                        && number.CompareTo((double)decimal.MinValue) > 0
                    )
                    {
                        return (TValue)Convert.ChangeType(variable.RawValue, requiredType);
                    }
                    else if (requiredType == typeof(float)
                        && number <= float.MaxValue
                        && number >= float.MinValue
                    )
                    {
                        return (TValue)Convert.ChangeType(variable.RawValue, requiredType);
                    }
                }
            }

            // if (requiredType == Nullable<UnderlyingType>) => ???

            throw new ArgumentException($"System variable \"{variable.Code}\" with type \"{variable.Type}\" cannot be converted to \"{requiredType.Name}\"");
        }

        #endregion
    }
}
