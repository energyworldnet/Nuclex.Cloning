namespace Nuclex.Cloning;

using System;
using System.Diagnostics.CodeAnalysis;
using Interfaces;
using ClonerCache = System.Collections.Concurrent.ConcurrentDictionary<Type, Func<object, object>>;

/// <summary>
///     Cloning factory which uses expression trees to improve performance when cloning
///     is a high-frequency action.
/// </summary>
public sealed partial class ExpressionTreeCloner : ICloneFactory
{
    /// <summary>Compiled cloners that perform shallow clone operations.</summary>
    private static readonly ClonerCache ShallowFieldBasedCloners = new();

    /// <summary>Compiled cloners that perform deep clone operations.</summary>
    private static readonly ClonerCache DeepFieldBasedCloners = new();

    /// <summary>Compiled cloners that perform shallow clone operations.</summary>
    private static readonly ClonerCache ShallowPropertyBasedCloners = new();

    /// <summary>Compiled cloners that perform deep clone operations.</summary>
    private static readonly ClonerCache DeepPropertyBasedCloners = new();

    /// <summary>
    ///     Creates a deep clone of the specified object, also creating clones of all
    ///     child objects being referenced.
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned.</typeparam>
    /// <param name="objectToClone">Object that will be cloned.</param>
    /// <returns>A deep clone of the provided object.</returns>
    [return: NotNullIfNotNull(nameof(objectToClone))]
    public static TCloned DeepFieldClone<TCloned>(TCloned objectToClone)
    {
        if (objectToClone is null)
            return objectToClone;

        var cloner = GetOrCreateDeepFieldBasedCloner(objectToClone.GetType());
        return (TCloned)cloner(objectToClone);
    }

    /// <summary>
    ///     Creates a deep clone of the specified object, also creating clones of all
    ///     child objects being referenced.
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned.</typeparam>
    /// <param name="objectToClone">Object that will be cloned.</param>
    /// <returns>A deep clone of the provided object.</returns>
    [return: NotNullIfNotNull(nameof(objectToClone))]
    public static TCloned DeepPropertyClone<TCloned>(TCloned objectToClone)
    {
        if (objectToClone is null)
            return objectToClone;

        var cloner = GetOrCreateDeepPropertyBasedCloner(objectToClone.GetType());
        return (TCloned)cloner(objectToClone);
    }

    /// <summary>
    ///     Creates a shallow clone of the specified object, reusing any referenced objects.
    /// </summary>
    /// <typeparam name="TCloned">Type of the object that will be cloned.</typeparam>
    /// <param name="objectToClone">Object that will be cloned.</param>
    /// <returns>A shallow clone of the provided object.</returns>
    [return: NotNullIfNotNull(nameof(objectToClone))]
    public static TCloned ShallowFieldClone<TCloned>(TCloned objectToClone)
    {
        if (objectToClone is null)
            return objectToClone;

        var cloner = GetOrCreateShallowFieldBasedCloner(objectToClone.GetType());
        return (TCloned)cloner(objectToClone);
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
        if (objectToClone is null)
            return objectToClone;

        var cloner = GetOrCreateShallowPropertyBasedCloner(objectToClone.GetType());
        return (TCloned)cloner(objectToClone);
    }

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

    /// <summary>
    ///     Retrieves the existing clone method for the specified type or compiles one if
    ///     none exists for the type yet.
    /// </summary>
    /// <param name="clonedType">Type for which a clone method will be retrieved.</param>
    /// <returns>The clone method for the specified type.</returns>
    private static Func<object, object> GetOrCreateShallowFieldBasedCloner(Type clonedType)
    {
        if (!ShallowFieldBasedCloners.TryGetValue(clonedType, out var cloner))
        {
            cloner = CreateShallowFieldBasedCloner(clonedType);
            ShallowFieldBasedCloners.TryAdd(clonedType, cloner);
        }

        return cloner;
    }

    /// <summary>
    ///     Retrieves the existing clone method for the specified type or compiles one if
    ///     none exists for the type yet.
    /// </summary>
    /// <param name="clonedType">Type for which a clone method will be retrieved.</param>
    /// <returns>The clone method for the specified type.</returns>
    private static Func<object, object> GetOrCreateDeepFieldBasedCloner(Type clonedType)
    {
        if (!DeepFieldBasedCloners.TryGetValue(clonedType, out var cloner))
        {
            cloner = CreateDeepFieldBasedCloner(clonedType);
            DeepFieldBasedCloners.TryAdd(clonedType, cloner);
        }

        return cloner;
    }

    /// <summary>
    ///     Retrieves the existing clone method for the specified type or compiles one if
    ///     none exists for the type yet.
    /// </summary>
    /// <param name="clonedType">Type for which a clone method will be retrieved.</param>
    /// <returns>The clone method for the specified type.</returns>
    private static Func<object, object> GetOrCreateShallowPropertyBasedCloner(Type clonedType)
    {
        if (!ShallowPropertyBasedCloners.TryGetValue(clonedType, out var cloner))
        {
            cloner = CreateShallowPropertyBasedCloner(clonedType);
            ShallowPropertyBasedCloners.TryAdd(clonedType, cloner);
        }

        return cloner;
    }

    /// <summary>
    ///     Retrieves the existing clone method for the specified type or compiles one if
    ///     none exists for the type yet.
    /// </summary>
    /// <param name="clonedType">Type for which a clone method will be retrieved.</param>
    /// <returns>The clone method for the specified type.</returns>
    private static Func<object, object> GetOrCreateDeepPropertyBasedCloner(Type clonedType)
    {
        if (!DeepPropertyBasedCloners.TryGetValue(clonedType, out var cloner))
        {
            cloner = CreateDeepPropertyBasedCloner(clonedType);
            DeepPropertyBasedCloners.TryAdd(clonedType, cloner);
        }

        return cloner;
    }
}
