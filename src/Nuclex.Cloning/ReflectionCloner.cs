namespace Nuclex.Cloning;

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Helpers;
using Interfaces;

/// <summary>Clones objects using reflection.</summary>
/// <remarks>
///     <para>
///         This type of cloning is a lot faster than cloning by serialization and
///         incurs no set-up cost, but requires cloned types to provide a default
///         constructor in order to work.
///     </para>
/// </remarks>
public sealed class ReflectionCloner : ICloneFactory
{
    /// <summary>
    ///     Creates a shallow clone of the specified object, reusing any referenced objects.
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned.</typeparam>
    /// <param name="objectToClone">Object that will be cloned.</param>
    /// <returns>A shallow clone of the provided object.</returns>
    [return: NotNullIfNotNull(nameof(objectToClone))]
    public static TCloned ShallowFieldClone<TCloned>(TCloned objectToClone)
    {
        var originalType = objectToClone?.GetType();
        if (originalType == null ||
            originalType!.IsPrimitive ||
            originalType.IsValueType ||
            originalType == typeof(string))
        {
            return objectToClone; // Being value types, primitives are copied by default
        }

        return originalType.IsArray
            ? (TCloned)ShallowCloneArray(objectToClone!)
            : (TCloned)ShallowCloneComplexFieldBased(objectToClone!);
    }

    /// <summary>
    ///     Creates a shallow clone of the specified object, reusing any referenced objects.
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned.</typeparam>
    /// <param name="objectToClone">Object that will be cloned.</param>
    /// <returns>A shallow clone of the provided object.</returns>
    [return: NotNullIfNotNull(nameof(objectToClone))]
    public static TCloned ShallowPropertyClone<TCloned>(TCloned objectToClone)
    {
        var originalType = objectToClone?.GetType();
        if (originalType == null ||
            originalType!.IsPrimitive ||
            originalType.IsValueType ||
            originalType == typeof(string))
        {
            return objectToClone; // Being value types, primitives are copied by default
        }

        return originalType.IsArray
            ? (TCloned)ShallowCloneArray(objectToClone!)
            : (TCloned)ShallowCloneComplexPropertyBased(objectToClone!);
    }

    /// <summary>
    ///     Creates a deep clone of the specified object, also creating clones of all
    ///     child objects being referenced.
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned.</typeparam>
    /// <param name="objectToClone">Object that will be cloned.</param>
    /// <returns>A deep clone of the provided object.</returns>
    [return: NotNullIfNotNull(nameof(objectToClone))]
    public static TCloned DeepFieldClone<TCloned>(TCloned objectToClone) =>
        objectToClone is null
            ? objectToClone
            : (TCloned)DeepCloneSingleFieldBased(objectToClone);

    /// <summary>
    ///     Creates a deep clone of the specified object, also creating clones of all
    ///     child objects being referenced.
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned.</typeparam>
    /// <param name="objectToClone">Object that will be cloned.</param>
    /// <returns>A deep clone of the provided object.</returns>
    [return: NotNullIfNotNull(nameof(objectToClone))]
    public static TCloned DeepPropertyClone<TCloned>(TCloned objectToClone) =>
        objectToClone is null
            ? objectToClone
            : (TCloned)DeepCloneSinglePropertyBased(objectToClone);

    /// <summary>
    ///     Creates a shallow clone of the specified object, reusing any referenced objects.
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned.</typeparam>
    /// <param name="objectToClone">Object that will be cloned.</param>
    /// <returns>A shallow clone of the provided object.</returns>
    TCloned ICloneFactory.ShallowFieldClone<TCloned>(TCloned objectToClone) =>
        ShallowFieldClone(objectToClone);

    /// <summary>
    ///     Creates a shallow clone of the specified object, reusing any referenced objects.
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned.</typeparam>
    /// <param name="objectToClone">Object that will be cloned.</param>
    /// <returns>A shallow clone of the provided object.</returns>
    TCloned ICloneFactory.ShallowPropertyClone<TCloned>(TCloned objectToClone) =>
        ShallowPropertyClone(objectToClone);

    /// <summary>
    ///     Creates a deep clone of the specified object, also creating clones of all
    ///     child objects being referenced.
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned.</typeparam>
    /// <param name="objectToClone">Object that will be cloned.</param>
    /// <returns>A deep clone of the provided object.</returns>
    TCloned ICloneFactory.DeepFieldClone<TCloned>(TCloned objectToClone) =>
        DeepFieldClone(objectToClone);

    /// <summary>
    ///     Creates a deep clone of the specified object, also creating clones of all
    ///     child objects being referenced.
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned.</typeparam>
    /// <param name="objectToClone">Object that will be cloned.</param>
    /// <returns>A deep clone of the provided object.</returns>
    TCloned ICloneFactory.DeepPropertyClone<TCloned>(TCloned objectToClone) =>
        DeepPropertyClone(objectToClone);

    /// <summary>Clones a complex type using field-based value transfer.</summary>
    /// <param name="original">Original instance that will be cloned.</param>
    /// <returns>A clone of the original instance.</returns>
    private static object ShallowCloneComplexFieldBased(object original)
    {
        var originalType = original.GetType();
        var clone = FormatterServices.GetUninitializedObject(originalType);

        var fieldInfos = ClonerHelpers.GetFieldInfosIncludingBaseClasses(
            originalType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        for (var index = 0; index < fieldInfos.Length; ++index)
        {
            var fieldInfo = fieldInfos[index];
            var originalValue = fieldInfo.GetValue(original);
            if (originalValue != null)
            {
                // Everything's just directly assigned in a shallow clone
                fieldInfo.SetValue(clone, originalValue);
            }
        }

        return clone;
    }

    /// <summary>Clones a complex type using property-based value transfer.</summary>
    /// <param name="original">Original instance that will be cloned.</param>
    /// <returns>A clone of the original instance.</returns>
    private static object ShallowCloneComplexPropertyBased(object original)
    {
        var originalType = original.GetType();
        var clone = Activator.CreateInstance(originalType);

        var propertyInfos = originalType.GetProperties(
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance |
            BindingFlags.FlattenHierarchy);
        for (var index = 0; index < propertyInfos.Length; ++index)
        {
            var propertyInfo = propertyInfos[index];
            if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
                continue;

            var propertyType = propertyInfo.PropertyType;
            var originalValue = propertyInfo.GetValue(original, null);
            if (originalValue == null)
                continue;

            if (propertyType.IsPrimitive ||
                propertyType.IsValueType ||
                propertyType == typeof(string))
            {
                // Primitive types can be assigned directly
                propertyInfo.SetValue(clone, originalValue, null);
            }
            else if (propertyType.IsArray)
            {
                // Arrays are assigned directly in a shallow clone
                propertyInfo.SetValue(clone, originalValue, null);
            }
            else
            {
                // Complex types are directly assigned without creating a copy
                propertyInfo.SetValue(clone, originalValue, null);
            }
        }

        return clone;
    }

    /// <summary>Clones an array using field-based value transfer.</summary>
    /// <param name="original">Original array that will be cloned.</param>
    /// <returns>A clone of the original array.</returns>
    private static object ShallowCloneArray(object original) => ((Array)original).Clone();

    /// <summary>Copies a single object using field-based value transfer.</summary>
    /// <param name="original">Original object that will be cloned.</param>
    /// <returns>A clone of the original object.</returns>
    private static object DeepCloneSingleFieldBased(object original)
    {
        var originalType = original.GetType();
        if (originalType.IsPrimitive ||
            originalType.IsValueType ||
            originalType == typeof(string))
        {
            return original; // Creates another box, does not reference boxed primitive
        }

        if (originalType.IsArray)
        {
            return DeepCloneArrayFieldBased((Array)original, originalType.GetElementType());
        }

        return DeepCloneComplexFieldBased(original);
    }

    /// <summary>Clones a complex type using field-based value transfer.</summary>
    /// <param name="original">Original instance that will be cloned.</param>
    /// <returns>A clone of the original instance.</returns>
    private static object DeepCloneComplexFieldBased(object original)
    {
        var originalType = original.GetType();
        var clone = FormatterServices.GetUninitializedObject(originalType);

        var fieldInfos = ClonerHelpers.GetFieldInfosIncludingBaseClasses(
            originalType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        for (var index = 0; index < fieldInfos.Length; ++index)
        {
            var fieldInfo = fieldInfos[index];
            var fieldType = fieldInfo.FieldType;
            var originalValue = fieldInfo.GetValue(original);
            if (originalValue == null)
                continue;

            // Primitive types can be assigned directly
            if (fieldType.IsPrimitive || fieldType.IsValueType || fieldType == typeof(string))
            {
                fieldInfo.SetValue(clone, originalValue);
            }
            else if (fieldType.IsArray)
            {
                // Arrays need to be cloned element-by-element
                fieldInfo.SetValue(
                    clone,
                    DeepCloneArrayFieldBased((Array)originalValue, fieldType.GetElementType()));
            }
            else
            {
                // Complex types need to be cloned member-by-member
                fieldInfo.SetValue(clone, DeepCloneSingleFieldBased(originalValue));
            }
        }

        return clone;
    }

    /// <summary>Clones an array using field-based value transfer.</summary>
    /// <param name="original">Original array that will be cloned.</param>
    /// <param name="elementType">Type of elements the original array contains.</param>
    /// <returns>A clone of the original array.</returns>
    private static object DeepCloneArrayFieldBased(Array original, Type elementType)
    {
        if (elementType.IsPrimitive || elementType == typeof(string))
        {
            return original.Clone();
        }

        var dimensionCount = original.Rank;

        // Find out the length of each of the array's dimensions, also calculate how
        // many elements there are in the array in total.
        var lengths = new int[dimensionCount];
        var totalElementCount = 0;
        for (var index = 0; index < dimensionCount; ++index)
        {
            lengths[index] = original.GetLength(index);
            if (index == 0)
            {
                totalElementCount = lengths[index];
            }
            else
            {
                totalElementCount *= lengths[index];
            }
        }

        // Knowing the number of dimensions and the length of each dimension, we can
        // create another array of the exact same sizes.
        var clone = Array.CreateInstance(elementType, lengths);

        // If this is a one-dimensional array (most common type), do an optimized copy
        // directly specifying the indices
        if (dimensionCount == 1)
        {
            // Clone each element of the array directly
            for (var index = 0; index < totalElementCount; ++index)
            {
                var originalElement = original.GetValue(index);
                if (originalElement != null)
                {
                    clone.SetValue(DeepCloneSingleFieldBased(originalElement), index);
                }
            }
        }
        else
        {
            // Otherwise use the generic code for multi-dimensional arrays
            var indexes = new int[dimensionCount];
            for (var index = 0; index < totalElementCount; ++index)
            {
                // Determine the index for each of the array's dimensions
                var elementIndex = index;
                for (var dimensionIndex = dimensionCount - 1; dimensionIndex >= 0; --dimensionIndex)
                {
                    indexes[dimensionIndex] = elementIndex % lengths[dimensionIndex];
                    elementIndex /= lengths[dimensionIndex];
                }

                // Clone the current array element
                var originalElement = original.GetValue(indexes);
                if (originalElement != null)
                {
                    clone.SetValue(DeepCloneSingleFieldBased(originalElement), indexes);
                }
            }
        }

        return clone;
    }

    /// <summary>Copies a single object using property-based value transfer.</summary>
    /// <param name="original">Original object that will be cloned.</param>
    /// <returns>A clone of the original object.</returns>
    private static object DeepCloneSinglePropertyBased(object original)
    {
        var originalType = original.GetType();
        if (originalType.IsPrimitive || originalType.IsValueType || originalType == typeof(string))
        {
            return original; // Creates another box, does not reference boxed primitive
        }

        if (originalType.IsArray)
        {
            return DeepCloneArrayPropertyBased((Array)original, originalType.GetElementType());
        }

        if (originalType.GetInterfaces().Contains(typeof(IList)))
        {
            return DeepCloneListPropertyBased((IList)original);
        }

        return DeepCloneComplexPropertyBased(original);
    }

    /// <summary>Clones a complex type using property-based value transfer.</summary>
    /// <param name="original">Original instance that will be cloned.</param>
    /// <returns>A clone of the original instance.</returns>
    private static object DeepCloneComplexPropertyBased(object original)
    {
        var originalType = original.GetType();
        var clone = Activator.CreateInstance(originalType);

        var propertyInfos = originalType.GetProperties(
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance |
            BindingFlags.FlattenHierarchy);
        for (var index = 0; index < propertyInfos.Length; ++index)
        {
            var propertyInfo = propertyInfos[index];
            if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
                continue;

            var propertyType = propertyInfo.PropertyType;
            var originalValue = propertyInfo.GetValue(original, null);
            if (originalValue == null)
                continue;

            if (propertyType.IsPrimitive ||
                propertyType.IsValueType ||
                propertyType == typeof(string))
            {
                // Primitive types can be assigned directly
                propertyInfo.SetValue(clone, originalValue, null);
            }
            else if (propertyType.IsArray)
            {
                // Arrays need to be cloned element-by-element
                propertyInfo.SetValue(
                    clone,
                    DeepCloneArrayPropertyBased(
                        (Array)originalValue, propertyType.GetElementType()),
                    null);
            }
            else
            {
                // Complex types need to be cloned member-by-member
                propertyInfo.SetValue(clone, DeepCloneSinglePropertyBased(originalValue), null);
            }
        }

        return clone;
    }

    /// <summary>Clones an array using property-based value transfer.</summary>
    /// <param name="original">Original array that will be cloned.</param>
    /// <param name="elementType">Type of elements the original array contains.</param>
    /// <returns>A clone of the original array.</returns>
    private static object DeepCloneArrayPropertyBased(Array original, Type elementType)
    {
        if (elementType.IsPrimitive || elementType.IsEnum || elementType == typeof(string))
        {
            return original.Clone();
        }

        var dimensionCount = original.Rank;

        // Find out the length of each of the array's dimensions, also calculate how
        // many elements there are in the array in total.
        var lengths = new int[dimensionCount];
        var totalElementCount = 0;
        for (var index = 0; index < dimensionCount; ++index)
        {
            lengths[index] = original.GetLength(index);
            if (index == 0)
            {
                totalElementCount = lengths[index];
            }
            else
            {
                totalElementCount *= lengths[index];
            }
        }

        // Knowing the number of dimensions and the length of each dimension, we can
        // create another array of the exact same sizes.
        var clone = Array.CreateInstance(elementType, lengths);

        // If this is a one-dimensional array (most common type), do an optimized copy
        // directly specifying the indices
        if (dimensionCount == 1)
        {
            // Clone each element of the array directly
            for (var index = 0; index < totalElementCount; ++index)
            {
                var originalElement = original.GetValue(index);
                if (originalElement != null)
                {
                    clone.SetValue(DeepCloneSinglePropertyBased(originalElement), index);
                }
            }
        }
        else
        {
            // Otherwise use the generic code for multi-dimensional arrays
            var indexes = new int[dimensionCount];
            for (var index = 0; index < totalElementCount; ++index)
            {
                // Determine the index for each of the array's dimensions
                var elementIndex = index;
                for (var dimensionIndex = dimensionCount - 1; dimensionIndex >= 0; --dimensionIndex)
                {
                    indexes[dimensionIndex] = elementIndex % lengths[dimensionIndex];
                    elementIndex /= lengths[dimensionIndex];
                }

                // Clone the current array element
                var originalElement = original.GetValue(indexes);
                if (originalElement != null)
                {
                    clone.SetValue(DeepCloneSinglePropertyBased(originalElement), indexes);
                }
            }
        }

        return clone;
    }

    /// <summary>Clones a List using property-based value transfer.</summary>
    /// <param name="original">Original List that will be cloned.</param>
    /// <returns>A clone of the original list.</returns>
    private static object DeepCloneListPropertyBased(IList original)
    {
        var originalType = original.GetType();
        var clone = (IList)Activator.CreateInstance(originalType);

        foreach (var value in original)
            clone.Add(DeepCloneSinglePropertyBased(value));

        return clone;
    }
}
