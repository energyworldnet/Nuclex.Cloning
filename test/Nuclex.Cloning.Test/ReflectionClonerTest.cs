namespace Nuclex.Cloning.Tests;

using System.Collections.Generic;
using Interfaces;

/// <summary>Unit Test for the reflection-based cloner.</summary>
[TestFixture]
internal sealed class ReflectionClonerTest : CloneFactoryTest
{
    /// <summary>Clone factory being tested.</summary>
    private readonly ICloneFactory cloneFactory;

    public ReflectionClonerTest() => this.cloneFactory = new ReflectionCloner();

    /// <summary>Verifies that cloning a null object simply returns null.</summary>
    [Test]
    public void CloningNullYieldsNull()
    {
        this.cloneFactory.DeepFieldClone<object?>(null).Should().BeNull();
        this.cloneFactory.DeepPropertyClone<object?>(null).Should().BeNull();
        this.cloneFactory.ShallowFieldClone<object?>(null).Should().BeNull();
        this.cloneFactory.ShallowPropertyClone<object?>(null).Should().BeNull();
    }

    /// <summary>
    ///     Verifies that clones of objects whose class doesn't possess a default constructor
    ///     can be made.
    /// </summary>
    [Test]
    public void ClassWithoutDefaultConstructorCanBeCloned()
    {
        var original = new ClassWithoutDefaultConstructor(1234);
        var clone = this.cloneFactory.DeepFieldClone(original);

        clone.Should().NotBeSameAs(original);
        clone.Dummy.Should().Be(original.Dummy);
    }

    /// <summary>Verifies that clones of primitive types can be created.</summary>
    [Test]
    public void PrimitiveTypesCanBeCloned()
    {
        const int original = 12_345;
        var clone = this.cloneFactory.ShallowFieldClone(original);
        clone.Should().Be(original);
    }

    /// <summary>Verifies that shallow clones of arrays can be made.</summary>
    [Test]
    public void ShallowClonesOfArraysCanBeMade()
    {
        var original = new[]
        {
            new TestReferenceType { TestField = 123, TestProperty = 456 },
        };
        var clone = this.cloneFactory.ShallowFieldClone(original);

        clone[0].Should().BeSameAs(original[0]);
    }

    /// <summary>Verifies that deep clones of arrays can be made.</summary>
    [Test]
    public void DeepClonesOfArraysCanBeMade()
    {
        var original = new[]
        {
            new TestReferenceType { TestField = 123, TestProperty = 456 },
        };
        var clone = this.cloneFactory.DeepFieldClone(original);

        clone[0].Should().NotBeSameAs(original[0]);
        clone[0].TestField.Should().Be(original[0].TestField);
        clone[0].TestProperty.Should().Be(original[0].TestProperty);
    }

    /// <summary>Verifies that deep clones of a generic list can be made.</summary>
    [Test]
    public void GenericListsCanBeCloned()
    {
        var original = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        var clone = this.cloneFactory.DeepFieldClone(original);

        clone.Should().Equal(original);
    }

    /// <summary>Verifies that deep clones of a generic dictionary can be made.</summary>
    [Test]
    public void GenericDictionariesCanBeCloned()
    {
        var original = new Dictionary<int, string>
        {
            { 1, "one" },
        };
        var clone = this.cloneFactory.DeepFieldClone(original);

        clone[1].Should().Be("one");
    }

    /// <summary>
    ///     Verifies that a field-based shallow clone of a value type can be performed.
    /// </summary>
    [Test]
    public void ShallowFieldBasedClonesOfValueTypesCanBeMade()
    {
        var original = CreateValueType();
        var clone = this.cloneFactory.ShallowFieldClone(original);
        VerifyClone(ref original, ref clone, false, false);
    }

    /// <summary>
    ///     Verifies that a field-based shallow clone of a reference type can be performed.
    /// </summary>
    [Test]
    public void ShallowFieldBasedClonesOfReferenceTypesCanBeMade()
    {
        var original = CreateReferenceType();
        var clone = this.cloneFactory.ShallowFieldClone(original);
        VerifyClone(original, clone, false, false);
    }

    /// <summary>
    ///     Verifies that a field-based deep clone of a value type can be performed.
    /// </summary>
    [Test]
    public void DeepFieldBasedClonesOfValueTypesCanBeMade()
    {
        var original = CreateValueType();
        var clone = this.cloneFactory.DeepFieldClone(original);
        VerifyClone(ref original, ref clone, true, false);
    }

    /// <summary>
    ///     Verifies that a field-based deep clone of a reference type can be performed.
    /// </summary>
    [Test]
    public void DeepFieldBasedClonesOfReferenceTypesCanBeMade()
    {
        var original = CreateReferenceType();
        var clone = this.cloneFactory.DeepFieldClone(original);
        VerifyClone(original, clone, true, false);
    }

    /// <summary>
    ///     Verifies that a property-based shallow clone of a value type can be performed.
    /// </summary>
    [Test]
    public void ShallowPropertyBasedClonesOfValueTypesCanBeMade()
    {
        var original = CreateValueType();
        var clone = this.cloneFactory.ShallowPropertyClone(original);
        VerifyClone(ref original, ref clone, false, true);
    }

    /// <summary>
    ///     Verifies that a property-based shallow clone of a reference type can be performed.
    /// </summary>
    [Test]
    public void ShallowPropertyBasedClonesOfReferenceTypesCanBeMade()
    {
        var original = CreateReferenceType();
        var clone = this.cloneFactory.ShallowPropertyClone(original);
        VerifyClone(original, clone, false, true);
    }

    /// <summary>
    ///     Verifies that a property-based deep clone of a value type can be performed.
    /// </summary>
    [Test]
    public void DeepPropertyBasedClonesOfValueTypesCanBeMade()
    {
        var original = CreateValueType();
        var clone = this.cloneFactory.DeepPropertyClone(original);
        VerifyClone(ref original, ref clone, true, true);
    }

    /// <summary>
    ///     Verifies that a property-based deep clone of a reference type can be performed.
    /// </summary>
    [Test]
    public void DeepPropertyBasedClonesOfReferenceTypesCanBeMade()
    {
        var original = CreateReferenceType();
        var clone = this.cloneFactory.DeepPropertyClone(original);
        VerifyClone(original, clone, true, true);
    }
}
