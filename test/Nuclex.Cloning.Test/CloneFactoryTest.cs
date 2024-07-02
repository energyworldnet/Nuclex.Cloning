namespace Nuclex.Cloning.Tests;

using System.Diagnostics.CodeAnalysis;
using Ewn.Analyzers;

/// <summary>Base class for unit tests verifying the clone factory.</summary>
[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1401:Fields should be private",
    Justification = "Intentional testing of public field cloning")]
[SuppressMessage(
    "Performance",
    "CA1814:Prefer jagged arrays over multidimensional",
    Justification = "Intentional testing of jagged arrays")]
internal abstract class CloneFactoryTest
{
    /// <summary>
    ///   Verifies that a cloned object exhibits the expected state for the type of
    ///   clone that has been performed.
    /// </summary>
    /// <param name="original">Original instance the clone was created from.</param>
    /// <param name="clone">Cloned instance that will be checked for correctness.</param>
    /// <param name="isDeepClone">Whether the cloned instance is a deep clone.</param>
    /// <param name="isPropertyBasedClone">
    ///   Whether a property-based clone was performed.
    /// </param>
    protected static void VerifyClone(
        HierarchicalReferenceType original,
        HierarchicalReferenceType clone,
        bool isDeepClone,
        bool isPropertyBasedClone)
    {
        clone.Should().NotBeSameAs(original);

        if (isPropertyBasedClone)
        {
            clone.TestField.Should().Be(0);
            clone.ValueTypeField.TestField.Should().Be(0);
            clone.ValueTypeField.TestProperty.Should().Be(0);
            clone.ValueTypeProperty.TestField.Should().Be(original.ValueTypeProperty.TestField);
            clone.ReferenceTypeField.Should().BeNull();
            clone.DerivedField.Should().BeNull();

            if (isDeepClone)
            {
                clone.ReferenceTypeProperty.Should().NotBeSameAs(original.ReferenceTypeProperty);
                clone.ReferenceTypeArrayProperty.Should().NotBeSameAs(
                    original.ReferenceTypeArrayProperty);
                clone.DerivedProperty.Should().NotBeSameAs(original.DerivedProperty);
                clone.DerivedProperty.Should().BeOfType<DerivedReferenceType>();

                var originalDerived = (DerivedReferenceType)original.DerivedProperty!;
                var clonedDerived = (DerivedReferenceType)clone.DerivedProperty!;
                clonedDerived.TestProperty.Should().Be(originalDerived.TestProperty);
                clonedDerived.DerivedProperty.Should().Be(originalDerived.DerivedProperty);

                clone.ReferenceTypeProperty!.TestField.Should().Be(0);
                clone.ReferenceTypeArrayProperty![1, 3][0].Should().NotBeSameAs(
                    original.ReferenceTypeArrayProperty![1, 3][0]);
                clone.ReferenceTypeArrayProperty[1, 3][2].Should().NotBeSameAs(
                    original.ReferenceTypeArrayProperty[1, 3][2]);
                clone.ReferenceTypeArrayProperty[1, 3][0]!.TestField.Should().Be(0);
                clone.ReferenceTypeArrayProperty[1, 3][2]!.TestField.Should().Be(0);
            }
            else
            {
                clone.ReferenceTypeProperty.Should().BeSameAs(original.ReferenceTypeProperty);
                clone.ReferenceTypeProperty.Should().BeSameAs(original.ReferenceTypeProperty);
                clone.ReferenceTypeArrayProperty.Should().BeSameAs(
                    original.ReferenceTypeArrayProperty);
            }
        }
        else
        {
            clone.TestField.Should().Be(original.TestField);
            clone.ValueTypeField.TestField.Should().Be(original.ValueTypeField.TestField);
            clone.ValueTypeField.TestProperty.Should().Be(original.ValueTypeField.TestProperty);
            clone.ValueTypeProperty.TestField.Should().Be(
                original.ValueTypeProperty.TestField);
            clone.ReferenceTypeField!.TestField.Should().Be(
                original.ReferenceTypeField!.TestField);
            clone.ReferenceTypeField.TestProperty.Should().Be(
                original.ReferenceTypeField.TestProperty);
            clone.ReferenceTypeProperty!.TestField.Should().Be(
                original.ReferenceTypeProperty!.TestField);

            if (isDeepClone)
            {
                clone.ReferenceTypeField.Should().NotBeSameAs(original.ReferenceTypeField);
                clone.ReferenceTypeProperty.Should().NotBeSameAs(original.ReferenceTypeProperty);
                clone.DerivedField.Should().NotBeSameAs(original.DerivedField);
                clone.DerivedProperty.Should().NotBeSameAs(original.DerivedProperty);
                clone.DerivedField.Should().BeOfType<DerivedReferenceType>();
                clone.DerivedProperty.Should().BeOfType<DerivedReferenceType>();

                var originalDerived = (DerivedReferenceType)original.DerivedField!;
                var clonedDerived = (DerivedReferenceType)clone.DerivedField!;
                clonedDerived.TestField.Should().Be(originalDerived.TestField);
                clonedDerived.TestProperty.Should().Be(originalDerived.TestProperty);
                clonedDerived.DerivedField.Should().Be(originalDerived.DerivedField);
                clonedDerived.DerivedProperty.Should().Be(originalDerived.DerivedProperty);

                originalDerived = (DerivedReferenceType)original.DerivedProperty!;
                clonedDerived = (DerivedReferenceType)clone.DerivedProperty!;
                clonedDerived.TestField.Should().Be(originalDerived.TestField);
                clonedDerived.TestProperty.Should().Be(originalDerived.TestProperty);
                clonedDerived.DerivedField.Should().Be(originalDerived.DerivedField);
                clonedDerived.DerivedProperty.Should().Be(originalDerived.DerivedProperty);

                clone.ReferenceTypeArrayField.Should().NotBeSameAs(
                    original.ReferenceTypeArrayField);
                clone.ReferenceTypeArrayProperty.Should().NotBeSameAs(
                    original.ReferenceTypeArrayProperty);
                clone.ReferenceTypeArrayProperty![1, 3][0].Should().NotBeSameAs(
                    original.ReferenceTypeArrayProperty![1, 3][0]);
                clone.ReferenceTypeArrayProperty[1, 3][2].Should().NotBeSameAs(
                    original.ReferenceTypeArrayProperty[1, 3][2]);
                clone.ReferenceTypeArrayProperty[1, 3][0]!.TestField.Should().Be(
                    original.ReferenceTypeArrayProperty[1, 3][0]!.TestField);
                clone.ReferenceTypeArrayProperty[1, 3][2]!.TestField.Should().Be(
                    original.ReferenceTypeArrayProperty[1, 3][2]!.TestField);
            }
            else
            {
                clone.DerivedField.Should().BeSameAs(original.DerivedField);
                clone.DerivedProperty.Should().BeSameAs(original.DerivedProperty);
                clone.ReferenceTypeField.Should().BeSameAs(original.ReferenceTypeField);
                clone.ReferenceTypeProperty.Should().BeSameAs(original.ReferenceTypeProperty);
                clone.ReferenceTypeArrayField.Should().BeSameAs(
                    original.ReferenceTypeArrayField);
                clone.ReferenceTypeArrayProperty.Should().BeSameAs(
                    original.ReferenceTypeArrayProperty);
            }
        }
    }

    /// <summary>
    ///   Verifies that a cloned object exhibits the expected state for the type of
    ///   clone that has been performed.
    /// </summary>
    /// <param name="original">Original instance the clone was created from.</param>
    /// <param name="clone">Cloned instance that will be checked for correctness.</param>
    /// <param name="isDeepClone">Whether the cloned instance is a deep clone.</param>
    /// <param name="isPropertyBasedClone">
    ///   Whether a property-based clone was performed.
    /// </param>
    protected static void VerifyClone(
        ref HierarchicalValueType original,
        ref HierarchicalValueType clone,
        bool isDeepClone,
        bool isPropertyBasedClone)
    {
        if (isPropertyBasedClone)
        {
            clone.TestField.Should().Be(original.TestField);
            clone.ValueTypeField.TestField.Should().Be(original.ValueTypeField.TestField);
            clone.ValueTypeField.TestProperty.Should().Be(original.ValueTypeField.TestProperty);
            clone.ValueTypeProperty.TestField.Should().Be(original.ValueTypeProperty.TestField);
            clone.ReferenceTypeField.Should().BeSameAs(original.ReferenceTypeField);
            clone.DerivedField.Should().BeSameAs(original.DerivedField);

            if (isDeepClone)
            {
                clone.ReferenceTypeProperty.Should().BeSameAs(original.ReferenceTypeProperty);
                clone.ReferenceTypeArrayProperty.Should().BeSameAs(
                    original.ReferenceTypeArrayProperty);
                clone.DerivedProperty.Should().BeSameAs(original.DerivedProperty);
                clone.DerivedProperty.Should().BeOfType<DerivedReferenceType>();

                var originalDerived = (DerivedReferenceType)original.DerivedProperty!;
                var clonedDerived = (DerivedReferenceType)clone.DerivedProperty!;
                clonedDerived.TestProperty.Should().Be(originalDerived.TestProperty);
                clonedDerived.DerivedProperty.Should().Be(originalDerived.DerivedProperty);

                clone.ReferenceTypeProperty!.TestField.Should().Be(
                    original.ReferenceTypeProperty!.TestField);
                clone.ReferenceTypeArrayProperty![1, 3][0].Should().BeSameAs(
                    original.ReferenceTypeArrayProperty![1, 3][0]);
                clone.ReferenceTypeArrayProperty[1, 3][2].Should().BeSameAs(
                    original.ReferenceTypeArrayProperty[1, 3][2]);
                clone.ReferenceTypeArrayProperty[1, 3][0]!.TestField.Should().Be(
                    original.ReferenceTypeArrayProperty[1, 3][0]!.TestField);
                clone.ReferenceTypeArrayProperty[1, 3][2]!.TestField.Should().Be(
                    original.ReferenceTypeArrayProperty[1, 3][2]!.TestField);
            }
            else
            {
                clone.ReferenceTypeProperty.Should().BeSameAs(original.ReferenceTypeProperty);
                clone.ReferenceTypeProperty.Should().BeSameAs(original.ReferenceTypeProperty);
                clone.ReferenceTypeArrayProperty.Should().BeSameAs(
                    original.ReferenceTypeArrayProperty);
            }
        }
        else
        {
            clone.TestField.Should().Be(original.TestField);
            clone.ValueTypeField.TestField.Should().Be(original.ValueTypeField.TestField);
            clone.ValueTypeField.TestProperty.Should().Be(original.ValueTypeField.TestProperty);
            clone.ValueTypeProperty.TestField.Should().Be(
                original.ValueTypeProperty.TestField);
            clone.ReferenceTypeField!.TestField.Should().Be(
                original.ReferenceTypeField!.TestField);
            clone.ReferenceTypeField.TestProperty.Should().Be(
                original.ReferenceTypeField.TestProperty);
            clone.ReferenceTypeProperty!.TestField.Should().Be(
                original.ReferenceTypeProperty!.TestField);

            if (isDeepClone)
            {
                clone.ReferenceTypeField.Should().BeSameAs(original.ReferenceTypeField);
                clone.ReferenceTypeProperty.Should().BeSameAs(original.ReferenceTypeProperty);
                clone.DerivedField.Should().BeSameAs(original.DerivedField);
                clone.DerivedProperty.Should().BeSameAs(original.DerivedProperty);
                clone.DerivedField.Should().BeOfType<DerivedReferenceType>();
                clone.DerivedProperty.Should().BeOfType<DerivedReferenceType>();

                var originalDerived = (DerivedReferenceType)original.DerivedField!;
                var clonedDerived = (DerivedReferenceType)clone.DerivedField!;
                clonedDerived.TestField.Should().Be(originalDerived.TestField);
                clonedDerived.TestProperty.Should().Be(originalDerived.TestProperty);
                clonedDerived.DerivedField.Should().Be(originalDerived.DerivedField);
                clonedDerived.DerivedProperty.Should().Be(originalDerived.DerivedProperty);

                originalDerived = (DerivedReferenceType)original.DerivedProperty!;
                clonedDerived = (DerivedReferenceType)clone.DerivedProperty!;
                clonedDerived.TestField.Should().Be(originalDerived.TestField);
                clonedDerived.TestProperty.Should().Be(originalDerived.TestProperty);
                clonedDerived.DerivedField.Should().Be(originalDerived.DerivedField);
                clonedDerived.DerivedProperty.Should().Be(originalDerived.DerivedProperty);

                clone.ReferenceTypeArrayField.Should().BeSameAs(
                    original.ReferenceTypeArrayField);
                clone.ReferenceTypeArrayProperty.Should().BeSameAs(
                    original.ReferenceTypeArrayProperty);
                clone.ReferenceTypeArrayProperty![1, 3][0].Should().BeSameAs(
                    original.ReferenceTypeArrayProperty![1, 3][0]);
                clone.ReferenceTypeArrayProperty[1, 3][2].Should().BeSameAs(
                    original.ReferenceTypeArrayProperty[1, 3][2]);
                clone.ReferenceTypeArrayProperty[1, 3][0]!.TestField.Should().Be(
                    original.ReferenceTypeArrayProperty[1, 3][0]!.TestField);
                clone.ReferenceTypeArrayProperty[1, 3][2]!.TestField.Should().Be(
                    original.ReferenceTypeArrayProperty[1, 3][2]!.TestField);
            }
            else
            {
                clone.DerivedField.Should().BeSameAs(original.DerivedField);
                clone.DerivedProperty.Should().BeSameAs(original.DerivedProperty);
                clone.ReferenceTypeField.Should().BeSameAs(original.ReferenceTypeField);
                clone.ReferenceTypeProperty.Should().BeSameAs(original.ReferenceTypeProperty);
                clone.ReferenceTypeArrayField.Should().BeSameAs(
                    original.ReferenceTypeArrayField);
                clone.ReferenceTypeArrayProperty.Should().BeSameAs(
                    original.ReferenceTypeArrayProperty);
            }
        }

        clone.TestProperty.Should().Be(original.TestProperty);
        clone.ValueTypeProperty.TestProperty.Should().Be(
            original.ValueTypeProperty.TestProperty);
        clone.ReferenceTypeProperty!.TestProperty.Should().Be(
            original.ReferenceTypeProperty!.TestProperty);
        clone.ReferenceTypeArrayProperty![1, 3][0]!.TestProperty.Should().Be(
            original.ReferenceTypeArrayProperty![1, 3][0]!.TestProperty);
        clone.ReferenceTypeArrayProperty[1, 3][2]!.TestProperty.Should().Be(
            original.ReferenceTypeArrayProperty[1, 3][2]!.TestProperty);
    }

    /// <summary>Creates a value type with random data for testing.</summary>
    /// <returns>A new value type with random data.</returns>
    [SuppressMessage(
        "Design",
        "MEN010:Avoid magic numbers",
        Justification = "Legacy test configuration method")]
    protected static HierarchicalValueType CreateValueType() => new()
    {
        TestField = 123,
        TestProperty = 321,
        ReferenceTypeArrayField = new TestReferenceType?[2, 4][]
        {
            {
                null!,
                null!,
                null!,
                null!,
            },
            {
                null!,
                null!,
                null!,
                new TestReferenceType?[3]
                {
                    new() { TestField = 101, TestProperty = 202 },
                    null,
                    new() { TestField = 909, TestProperty = 808 },
                },
            },
        },
        ReferenceTypeArrayProperty = new TestReferenceType?[2, 4][]
        {
            {
                null!,
                null!,
                null!,
                null!,
            },
            {
                null!,
                null!,
                null!,
                new TestReferenceType?[3]
                {
                    new() { TestField = 303, TestProperty = 404 },
                    null,
                    new() { TestField = 707, TestProperty = 606 },
                },
            },
        },
        ValueTypeField = new TestValueType()
        {
            TestField = 456,
            TestProperty = 654,
        },
        ValueTypeProperty = new TestValueType()
        {
            TestField = 789,
            TestProperty = 987,
        },
        ReferenceTypeField = new TestReferenceType()
        {
            TestField = 135,
            TestProperty = 531,
        },
        ReferenceTypeProperty = new TestReferenceType()
        {
            TestField = 246,
            TestProperty = 642,
        },
        DerivedField = new DerivedReferenceType()
        {
            DerivedField = 100,
            DerivedProperty = 200,
            TestField = 300,
            TestProperty = 400,
        },
        DerivedProperty = new DerivedReferenceType()
        {
            DerivedField = 500,
            DerivedProperty = 600,
            TestField = 700,
            TestProperty = 800,
        },
    };

    /// <summary>Creates a reference type with random data for testing.</summary>
    /// <returns>A new reference type with random data.</returns>
    [SuppressMessage(
        "Design",
        "MEN010:Avoid magic numbers",
        Justification = "Legacy test configuration method")]
    protected static HierarchicalReferenceType CreateReferenceType() => new()
    {
        TestField = 123,
        TestProperty = 321,
        ReferenceTypeArrayField = new TestReferenceType?[2, 4][]
        {
            {
                null!,
                null!,
                null!,
                null!,
            },
            {
                null!,
                null!,
                null!,
                new TestReferenceType?[3]
                {
                    new() { TestField = 101, TestProperty = 202 },
                    null,
                    new() { TestField = 909, TestProperty = 808 },
                },
            },
        },
        ReferenceTypeArrayProperty = new TestReferenceType?[2, 4][]
        {
            {
                null!,
                null!,
                null!,
                null!,
            },
            {
                null!,
                null!,
                null!,
                new TestReferenceType?[3]
                {
                    new() { TestField = 303, TestProperty = 404 },
                    null,
                    new() { TestField = 707, TestProperty = 606 },
                },
            },
        },
        ValueTypeField = new TestValueType()
        {
            TestField = 456,
            TestProperty = 654,
        },
        ValueTypeProperty = new TestValueType()
        {
            TestField = 789,
            TestProperty = 987,
        },
        ReferenceTypeField = new TestReferenceType()
        {
            TestField = 135,
            TestProperty = 531,
        },
        ReferenceTypeProperty = new TestReferenceType()
        {
            TestField = 246,
            TestProperty = 642,
        },
        DerivedField = new DerivedReferenceType()
        {
            DerivedField = 100,
            DerivedProperty = 200,
            TestField = 300,
            TestProperty = 400,
        },
        DerivedProperty = new DerivedReferenceType()
        {
            DerivedField = 500,
            DerivedProperty = 600,
            TestField = 700,
            TestProperty = 800,
        },
    };

    /// <summary>A value type being used for testing.</summary>
    protected struct TestValueType
    {
        /// <summary>Field holding an integer value for testing.</summary>
        public int TestField;

        /// <summary>Gets or sets property holding an integer value for testing.</summary>
        public int TestProperty { get; set; }
    }

    /// <summary>A value type container other complex types used for testing.</summary>
    [SuppressMessage(
        "StyleCop.CSharp.OrderingRules",
        "SA1201:Elements should appear in the correct order",
        Justification = "Custom ordering to keep field and property paired")]
    protected struct HierarchicalValueType
    {
        /// <summary>Field holding an integer value for testing.</summary>
        public int TestField;

        /// <summary>Gets or sets property holding an integer value for testing.</summary>
        public int TestProperty { get; set; }

        /// <summary>Value type field for testing.</summary>
        public TestValueType ValueTypeField;

        /// <summary>Gets or sets value type property for testing.</summary>
        public TestValueType ValueTypeProperty { get; set; }

        /// <summary>Reference type field for testing.</summary>
        public TestReferenceType? ReferenceTypeField;

        /// <summary>Gets or sets reference type property for testing.</summary>
        public TestReferenceType? ReferenceTypeProperty { get; set; }

        /// <summary>An array field of reference types.</summary>
        public TestReferenceType?[,][]? ReferenceTypeArrayField;

        /// <summary>Gets or sets an array property of reference types.</summary>
        public TestReferenceType?[,][]? ReferenceTypeArrayProperty { get; set; }

        /// <summary>A reference type field that's always null.</summary>
        public TestReferenceType? AlwaysNullField;

        /// <summary>Gets or sets a reference type property that's always null.</summary>
        public TestReferenceType? AlwaysNullProperty { get; set; }

        /// <summary>Gets a property that only has a getter.</summary>
        [SuppressMessage(
            "Style",
            "IDE0251:Make member 'readonly'",
            Justification = "Not readonly for intentions of test")]
        public TestReferenceType? GetOnlyProperty => null;

        /// <summary>Sets a property that only has a setter.</summary>
        [SuppressMessage(
            "Style",
            "IDE0251:Make member 'readonly'",
            Justification = "Not readonly for intentions of test")]
        public TestReferenceType? SetOnlyProperty
        {
            set { }
        }

        /// <summary>Field typed as base class holding a derived instance.</summary>
        public TestReferenceType? DerivedField;

        /// <summary>Gets or sets field typed as base class holding a derived instance.</summary>
        public TestReferenceType? DerivedProperty { get; set; }
    }

    /// <summary>A class that does not have a default constructor.</summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ClassWithoutDefaultConstructor"/> class.
    /// </remarks>
    /// <param name="dummy">Dummy value that will be saved by the instance.</param>
    public sealed class ClassWithoutDefaultConstructor(int dummy)
    {
        /// <summary>Gets dummy value that has been saved by the instance.</summary>
        public int Dummy => dummy;
    }

    /// <summary>A derived reference type being used for testing.</summary>
    protected sealed class DerivedReferenceType : TestReferenceType
    {
        /// <summary>Field holding an integer value for testing.</summary>
        public int DerivedField;

        /// <summary>Gets or sets property holding an integer value for testing.</summary>
        public int DerivedProperty { get; set; }
    }

    /// <summary>A reference type being used for testing.</summary>
    [DesignedInheritance]
    protected class TestReferenceType
    {
        /// <summary>Field holding an integer value for testing.</summary>
        public int TestField;

        /// <summary>Gets or sets property holding an integer value for testing.</summary>
        public int TestProperty { get; set; }
    }

    /// <summary>A value type container other complex types used for testing.</summary>
    [SuppressMessage(
        "StyleCop.CSharp.OrderingRules",
        "SA1201:Elements should appear in the correct order",
        Justification = "Custom ordering to keep field and property paired")]
    protected sealed class HierarchicalReferenceType
    {
        /// <summary>Field holding an integer value for testing.</summary>
        public int TestField;

        /// <summary>Gets or sets property holding an integer value for testing.</summary>
        public int TestProperty { get; set; }

        /// <summary>Value type field for testing.</summary>
        public TestValueType ValueTypeField;

        /// <summary>Gets or sets value type property for testing.</summary>
        public TestValueType ValueTypeProperty { get; set; }

        /// <summary>Reference type field for testing.</summary>
        public TestReferenceType? ReferenceTypeField;

        /// <summary>Gets or sets reference type property for testing.</summary>
        public TestReferenceType? ReferenceTypeProperty { get; set; }

        /// <summary>An array field of reference types.</summary>
        public TestReferenceType?[,][]? ReferenceTypeArrayField;

        /// <summary>Gets or sets an array property of reference types.</summary>
        public TestReferenceType?[,][]? ReferenceTypeArrayProperty { get; set; }

        /// <summary>A reference type field that's always null.</summary>
        public TestReferenceType? AlwaysNullField;

        /// <summary>Gets or sets a reference type property that's always null.</summary>
        public TestReferenceType? AlwaysNullProperty { get; set; }

        /// <summary>Gets a property that only has a getter.</summary>
        public TestReferenceType? GetOnlyProperty => null;

        /// <summary>Sets a property that only has a s.</summary>
        public TestReferenceType? SetOnlyProperty
        {
            set { }
        }

        /// <summary>Field typed as base class holding a derived instance.</summary>
        public TestReferenceType? DerivedField;

        /// <summary>Gets or sets field typed as base class holding a derived instance.</summary>
        public TestReferenceType? DerivedProperty { get; set; }
    }
}
