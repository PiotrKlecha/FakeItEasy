﻿namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection.Emit;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Tests;
    using Xbehave;

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Required for testing.")]
    public class CreationOptionsSpecs
    {
        [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "It's just used for testing.")]
        public interface IInterfaceThatWeWillAddAttributesTo1
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "It's just used for testing.")]
        public interface IInterfaceThatWeWillAddAttributesTo2
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "It's just used for testing.")]
        public interface IInterfaceThatWeWillAddAttributesTo3
        {
        }

        [Scenario]
        public static void ConfigureFakeDuringConstruction(
            MakesVirtualCallInConstructor fake)
        {
            "when configuring a fake during construction"
                .x(() =>
                    {
                        fake = A.Fake<MakesVirtualCallInConstructor>(
                            options => options.ConfigureFake(
                                f => A.CallTo(() => f.VirtualMethod(A<string>._))
                            .Returns("configured value in fake options")));
                    });

            "it should return the configured value during the constructor"
                .x(() => fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured value in fake options"));

            "it should return the configured value after the constructor"
                .x(() => fake.VirtualMethod(null).Should().Be("configured value in fake options"));
        }

        [Scenario]
        public static void ConfigureFakeOverridesFakeConfigurator(
            RobotRunsAmokEvent fake)
        {
            "when configuring a fake during construction to configure a method also configured by a fake configurator"
                .x(() => fake = A.Fake<RobotRunsAmokEvent>(
                    options => options.ConfigureFake(
                        f => A.CallTo(() => f.CalculateTimestamp()).Returns(new DateTime(2000, 1, 1, 0, 0, 0)))));

            "it should use the configured behavior from the configuration during construction"
                .x(() => fake.Timestamp.Should().Be(new DateTime(2000, 1, 1, 0, 0, 0)));
        }

        [Scenario]
        public static void MultipleConfigureFakeConfigurations(
            MakesVirtualCallInConstructor fake)
        {
            "when configuring a fake multiple times"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                            .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("second value"))
                            .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("first value").Once())));

            "it should apply each configuration in turn"
                .x(() => new[]
                             {
                                 fake.VirtualMethodValueDuringConstructorCall, fake.VirtualMethod(null)
                             }
                             .Should().Equal("first value", "second value"));
        }

        [Scenario]
        public static void ConfigureFakeOverridesCallsBaseMethods(
            MakesVirtualCallInConstructor fake)
        {
            "when specifying that a fake calls base methods followed by explicit configuration"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .CallsBaseMethods()
                                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("a value from ConfigureFake"))));

            "it should use behavior defined by the explicit configuration"
                .x(() => fake.VirtualMethod(null).Should().Be("a value from ConfigureFake"));
        }

        [Scenario]
        public static void StrictCombinedWithConfigureFake(
            MakesVirtualCallInConstructor fake)
        {
            "when specifying a strict fake followed by explicit configuration"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .Strict()
                                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._))
                                        .Returns("configured value of strict fake"))));

            "it should return the configured value during the constructor"
                .x(() => fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured value of strict fake"));

            "it should return the configured value after the constructor"
                .x(() => fake.VirtualMethod(null).Should().Be("configured value of strict fake"));
        }

        [Scenario]
        public static void WrappingCombinedWithConfigureFake(
            MakesVirtualCallInConstructor fake)
        {
            "when specification of wrapping is followed by explicit configuration"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .Wrapping(new MakesVirtualCallInConstructor())
                                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._))
                                        .Returns("configured in test"))));

            "it should use the configured behavior during the constructor"
                .x(() => fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured in test"));

            "it should use the configured behavior after the constructor"
                .x(() => fake.VirtualMethod(null).Should().Be("configured in test"));
        }

        [Scenario]
        public static void CallsBaseMethodsDuringConstruction(
            MakesVirtualCallInConstructor fake)
        {
            "when specifying that a fake calls base methods"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options.CallsBaseMethods()));

            "it should call base method during the constructor"
                .x(() => fake.VirtualMethodValueDuringConstructorCall.Should().Be("implementation value"));

            "it should call base method after the constructor"
                .x(() => fake.VirtualMethod(null).Should().Be("implementation value"));
        }

        [Scenario]
        public static void CallsBaseMethodsOverridesConfigureFake(
            MakesVirtualCallInConstructor fake)
        {
            "when explicit configuration is followed by specifying that a fake calls base methods"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("a value from ConfigureFake"))
                                    .CallsBaseMethods()));

            "it should call base method"
                .x(() => fake.VirtualMethod(null).Should().Be("implementation value"));
        }

        [Scenario]
        public static void CallsBaseMethodsOverridesStrict(
            MakesVirtualCallInConstructor fake)
        {
            "when specifying a strict fake followed by specifying the calling of base methods"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .Strict()
                                    .CallsBaseMethods()));

            "it should call base method"
                .x(() => fake.VirtualMethod(null).Should().Be("implementation value"));
        }

        [Scenario]
        public static void CallsBaseMethodsOverridesWrapping(
            MakesVirtualCallInConstructor fake)
        {
            "when specifying a wrapping fake followed specifying the calling of base methods"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .Wrapping(new DerivedMakesVirtualCallInConstructor("wrapped value"))
                                    .CallsBaseMethods()));

            "it should call base method"
                .x(() => fake.VirtualMethod(null).Should().Be("implementation value"));
        }

        [Scenario]
        public static void StrictDuringConstruction(
            MakesVirtualCallInConstructor fake)
        {
            "when Strict is used to configure a fake"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options.Strict()));

            "it should throw an exception from a method call during the constructor"
                .x(() => fake.ExceptionFromVirtualMethodCallInConstructor
                             .Should()
                             .BeAnExceptionOfType<ExpectationException>()
                             .WithMessage("Call to non configured method \"VirtualMethod\" of strict fake."));

            "it should throw an exception from a method call after the constructor"
                .x(() => Record.Exception(() => fake.VirtualMethod("call outside constructor"))
                             .Should()
                             .BeAnExceptionOfType<ExpectationException>()
                             .WithMessage("Call to non configured method \"VirtualMethod\" of strict fake."));
        }

        [Scenario]
        public static void StrictOverridesCallsBaseMethods(
            MakesVirtualCallInConstructor fake)
        {
            "when specifying the calling of base methods followed by strictness"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .CallsBaseMethods()
                                    .Strict()));

            "it should throw an exception from a method call"
                .x(() => Record.Exception(() => fake.VirtualMethod(null))
                             .Should()
                             .BeAnExceptionOfType<ExpectationException>());
        }

        [Scenario]
        public static void StrictOverridesConfigureFake(
            MakesVirtualCallInConstructor fake)
        {
            "when explicit fake configuration is followed by specification of strict behavior"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._))
                                        .Returns("configured value of strict fake"))
                                            .Strict()));

            "it should throw an exception from a method call"
                .x(() => Record.Exception(() => fake.VirtualMethod(null))
                             .Should()
                             .BeAnExceptionOfType<ExpectationException>());
        }

        [Scenario]
        public static void StrictOverridesWrapping(
            MakesVirtualCallInConstructor fake)
        {
            "when Wrapping followed by Strict are used to configure a fake"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .Wrapping(new MakesVirtualCallInConstructor())
                                        .Strict()));

            "it should throw an exception from a method call"
                .x(() => Record.Exception(() => fake.VirtualMethod(null))
                             .Should()
                             .BeAnExceptionOfType<ExpectationException>());
        }

        [Scenario]
        public static void WrappingDuringConstruction(
            MakesVirtualCallInConstructor fake)
        {
            "when Wrapping is used to configure a fake"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(
                    options => options.Wrapping(new MakesVirtualCallInConstructor())));

            "it should delegate to the wrapped instance during the constructor"
                .x(() => fake.VirtualMethodValueDuringConstructorCall.Should().Be("implementation value"));

            "it should delegate to the wrapped instance after the constructor"
                .x(() => fake.VirtualMethod(null).Should().Be("implementation value"));
        }

        [Scenario]
        public static void WrappingOverridesCallsBaseMethods(
            MakesVirtualCallInConstructor fake)
        {
            "when specifying the calling of base methods followed by wrapping"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .CallsBaseMethods()
                                    .Wrapping(new DerivedMakesVirtualCallInConstructor("wrapped value"))));

            "it should delegate to the wrapped instance"
                .x(() => fake.VirtualMethod(null).Should().Be("wrapped value"));
        }

        [Scenario]
        public static void WrappingOverridesConfigureFake(
            MakesVirtualCallInConstructor fake)
        {
            "when explicit configuration is followed by specification of wrapping"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._))
                                        .Returns("configured in test"))
                                            .Wrapping(new DerivedMakesVirtualCallInConstructor("wrapped value"))));

            "it should delegate to the wrapped instance during the constructor"
                .x(() => fake.VirtualMethodValueDuringConstructorCall.Should().Be("wrapped value"));

            "it should delegate to the wrapped instance after the constructor"
                .x(() => fake.VirtualMethod(null).Should().Be("wrapped value"));
        }

        [Scenario]
        public static void WrappingOverridesStrict(
            MakesVirtualCallInConstructor fake)
        {
            "when Strict followed by Wrapping are used to configure a fake"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .Strict()
                                    .Wrapping(new DerivedMakesVirtualCallInConstructor("wrapped value"))));

            "it should delegate to the wrapped instance"
                .x(() => fake.VirtualMethod(null).Should().Be("wrapped value"));
        }

        [Scenario]
        public static void MultipleWrappingConfigurations(
            MakesVirtualCallInConstructor fake)
        {
            "when Wrapping is used to configure a fake twice"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .Wrapping(new DerivedMakesVirtualCallInConstructor("first wrapped value"))
                                    .Wrapping(new DerivedMakesVirtualCallInConstructor("second wrapped value"))));

            "it should delegate to the last wrapped instance"
                .x(() => fake.VirtualMethodValueDuringConstructorCall.Should().Be("second wrapped value"));
        }

        [Scenario]
        public static void Wrapping(
            RobotRunsAmokEvent fake)
        {
            "when specification of wrapping is used to configure a fake that has a fake configurator"
                .x(() => fake = A.Fake<RobotRunsAmokEvent>(
                    options => options.Wrapping(new RobotRunsAmokEvent())));

            "it should delegate to the wrapped object"
                .x(() => fake.Timestamp.Should().Be(DomainEvent.DefaultTimestamp));
        }

        [Scenario]
        public static void Implements(
            MakesVirtualCallInConstructor fake)
        {
            "when Implements is used to configure a fake"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                     .Implements(typeof(IDisposable))));

            "it should produce a fake that implements the interface"
                .x(() => fake.Should().BeAssignableTo<IDisposable>());
        }

        [Scenario]
        public static void MultipleImplementsConfigurations(
            MakesVirtualCallInConstructor fake)
        {
            "when Implements is used to configure a fake twice"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .Implements(typeof(IComparable))
                                    .Implements(typeof(ICloneable))));

            "it should produce a fake that implements both interfaces"
                .x(() => fake.Should().BeAssignableTo<IComparable>().And
                             .BeAssignableTo<ICloneable>());
        }

        [Scenario]
        public static void WithAdditionalAttributes(
            IInterfaceThatWeWillAddAttributesTo1 fake)
        {
            "when specifying a fake with additional attributes"
                .x(() =>
                    {
                        var constructor = typeof(ForTestAttribute).GetConstructor(new Type[0]);
                        var attribute = new CustomAttributeBuilder(constructor, new object[0]);
                        var customAttributeBuilders = new List<CustomAttributeBuilder> { attribute };

                        fake = A.Fake<IInterfaceThatWeWillAddAttributesTo1>(options => options
                            .WithAdditionalAttributes(customAttributeBuilders));
                    });

            "it should produce a fake that has the attribute"
                .x(() => fake.GetType().GetCustomAttributes(typeof(ForTestAttribute), false).Should().HaveCount(1));
        }

        [Scenario]
        public static void WithAdditionalAttributesAndNullSetOfAttributes(
            Exception exception)
        {
            "when specifying a fake with additional attributes using a null set of attributes"
                .x(() => exception = Record.Exception(() => A.Fake<IInterfaceThatWeWillAddAttributesTo2>(options => options.WithAdditionalAttributes(null))));

            "it should throw an argument null exception"
                .x(() => exception.Should().BeAnExceptionOfType<ArgumentNullException>()
                             .WithMessage("*customAttributeBuilders*"));
        }

        [Scenario]
        public static void MultipleWithAdditionalAttributesConfigurations(
            IInterfaceThatWeWillAddAttributesTo3 fake)
        {
            "when specifying a fake with additional attributes twice"
                .x(() =>
                    {
                        var constructor1 = typeof(ScenarioAttribute).GetConstructor(new Type[0]);
                        var attribute1 = new CustomAttributeBuilder(constructor1, new object[0]);
                        var customAttributeBuilders1 = new List<CustomAttributeBuilder> { attribute1 };

                        var constructor2 = typeof(ExampleAttribute).GetConstructor(new[] { typeof(object[]) });
                        var attribute2 = new CustomAttributeBuilder(constructor2, new[] { new object[] { 1, null } });
                        var constructor3 = typeof(DebuggerStepThroughAttribute).GetConstructor(new Type[0]);
                        var attribute3 = new CustomAttributeBuilder(constructor3, new object[0]);
                        var customAttributeBuilders2 = new List<CustomAttributeBuilder> { attribute2, attribute3 };

                        fake = A.Fake<IInterfaceThatWeWillAddAttributesTo3>(options => options
                            .WithAdditionalAttributes(customAttributeBuilders1)
                            .WithAdditionalAttributes(customAttributeBuilders2));
                    });

            "it should produce a fake that has all of the attributes"
                .x(() => fake.GetType().GetCustomAttributes(false)
                             .Select(a => a.GetType()).Should()
                             .Contain(typeof(ScenarioAttribute)).And
                             .Contain(typeof(ExampleAttribute)).And
                             .Contain(typeof(DebuggerStepThroughAttribute)));
        }

        [Scenario]
        public static void WithArgumentsForConstructor(
            MakesVirtualCallInConstructor fake)
        {
            "when specifying constructor arguments with a list of arguments"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .WithArgumentsForConstructor(new object[]
                                                                    {
                                                                        "prime argument", 2
                                                                    })));

            "it should create a fake using the supplied arguments"
                .x(() =>
                    {
                        fake.ConstructorArgument1.Should().Be("prime argument");
                        fake.ConstructorArgument2.Should().Be(2);
                    });
        }

        [Scenario]
        public static void MultipleWithArgumentsForConstructorConfigurations(
            MakesVirtualCallInConstructor fake)
        {
            "when specifying a fake with constructor arguments twice"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .WithArgumentsForConstructor(new object[]
                                                                    {
                                                                        "prime argument", 1
                                                                    })
                                    .WithArgumentsForConstructor(() => new MakesVirtualCallInConstructor("secondary argument", 2))));

            "it should create a fake using the last set of supplied arguments"
                .x(() =>
                    {
                        fake.ConstructorArgument1.Should().Be("secondary argument");
                        fake.ConstructorArgument2.Should().Be(2);
                    });
        }

        [Scenario]
        public static void WithArgumentsForConstructorWithExampleConstructor(
            MakesVirtualCallInConstructor fake)
        {
            "when specifying a fake with constructor arguments and an example constructor"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .WithArgumentsForConstructor(() => new MakesVirtualCallInConstructor("first argument", 9))));

            "it should create a fake using the supplied arguments"
                .x(() =>
                    {
                        fake.ConstructorArgument1.Should().Be("first argument");
                        fake.ConstructorArgument2.Should().Be(9);
                    });
        }
    }

    public class DerivedMakesVirtualCallInConstructor : MakesVirtualCallInConstructor
    {
        private readonly string virtualMethodReturnValue;

        public DerivedMakesVirtualCallInConstructor(string virtualMethodReturnValue)
        {
            this.virtualMethodReturnValue = virtualMethodReturnValue;
        }

        public override string VirtualMethod(string parameter)
        {
            return this.virtualMethodReturnValue;
        }
    }
}