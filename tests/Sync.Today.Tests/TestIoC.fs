module TestIoC

open IoC
open NUnit.Framework
open System

[<TestFixture>]
module ``Container Register, Resolve, Release Tests`` =
    
    [<AbstractClass>]
    type AbstractType () = do ()

    type ConcreteType () = inherit AbstractType()

    type IMarkerInterface = interface end

    type MarkedType () = interface IMarkerInterface
    
    let [<Test>] ``registering 2 instances of an abstract type in a single container should throw`` () =
        let container = Container()
        container.Register<AbstractType>(typeof<AbstractType>, Singleton)
        Assert.Throws<System.ArgumentException>(fun () ->
            container.Register<AbstractType>(typeof<AbstractType>, Singleton) |> ignore
        ) |> ignore

    let [<Test>] ``registering a concrete type that does not implement the abstract type should throw`` () =
        let container = Container()
        Assert.Throws<System.ArgumentException>(fun () ->
            container.Register<MarkedType>(typeof<AbstractType>, Singleton)
        ) |> ignore

    let [<Test>] ``attempting to resolve an unregistered type should throw`` () =
        let container = Container()
        Assert.Throws<TypeResolutionException>(fun () ->  
            container.Resolve<AbstractType>() |> ignore
        ) |> ignore

    let [<Test>] ``resolving a registered abstract type should return an instance of the specified concrete type`` () =
        let container = Container()
        container.Register<AbstractType>(typeof<ConcreteType>, Singleton)
        let instance = container.Resolve<AbstractType>()
        Assert.True(instance :? ConcreteType)

    let [<Test>] ``resolving a type with a singleton lifetime should always return the same instance`` () =
        let container = Container()
        container.Register<AbstractType>(typeof<ConcreteType>, Singleton)
        let a = container.Resolve<AbstractType>()
        let b = container.Resolve<AbstractType>()
        Assert.True( Object.ReferenceEquals(a,b) )
        
    let [<Test>] ``resolving a type with a transient lifetime should a new instance each time`` () =
        let container = Container()
        container.Register<AbstractType>(typeof<ConcreteType>, Transient)
        let a = container.Resolve<AbstractType>()
        let b = container.Resolve<AbstractType>()
        Assert.AreNotSame(a,b)

    let [<Test>] ``resolving a registered instance of a type should return that instance`` () =
        let container = Container()
        let this = ConcreteType()
        container.Register<AbstractType>(this)
        let that = container.Resolve<AbstractType>()
        Assert.AreSame(this, that)

    let [<Test>] ``resolving a type registered as a factory should call the specified factory`` () =
        let called = ref false
        let factory = fun () -> called := true; ConcreteType() :> AbstractType
        let container = Container()
        container.Register<AbstractType>(factory, Singleton)
        container.Resolve<AbstractType>() |> ignore
        Assert.True( called.Value )

    let [<Test>] ``releasing a registered concrete instance then resolving the type should return a new concrete instance`` () =
        let container = Container()
        let this = ConcreteType()
        container.Register<ConcreteType>(this)
        container.Release(this)
        let that = container.Resolve<ConcreteType>()
        Assert.True( not <| Object.ReferenceEquals(this, that) )

    do
        ``registering 2 instances of an abstract type in a single container should throw`` ()
        ``attempting to resolve an unregistered type should throw`` ()
        ``resolving a registered abstract type should return an instance of the specified concrete type``  ()
        ``resolving a type with a singleton lifetime should always return the same instance`` ()
        ``resolving a type with a transient lifetime should a new instance each time`` ()
        ``resolving a registered instance of a type should return that instance`` ()
        ``resolving a type registered as a factory should call the specified factory`` ()
        ``releasing a registered concrete instance then resolving the type should return a new concrete instance`` ()

[<TestFixture>]
module ``Constructor Tests`` =
    
    [<AbstractClass>]
    type AbstractType () = do ()
   
    type ConstructorWithValueTypeArg (arg:int) = inherit AbstractType()

    let [<Test>] ``resolving type with value type dependency in constructor should throw`` () =
        let container = Container()
        container.Register<AbstractType>(typeof<ConstructorWithValueTypeArg>, Singleton)
        Assert.Throws<TypeResolutionException>(fun () ->
            container.Resolve<AbstractType>() |> ignore
        ) |> ignore

    type ReferenceType() = do ()
    type ConstructorWithReferenceTypeArg (arg:ReferenceType) = inherit AbstractType()

    let [<Test>] ``resolving type with reference type dependency in constructor should inject reference`` () =
        let container = Container()
        container.Register<AbstractType>(typeof<ConstructorWithReferenceTypeArg>, Singleton)
        let instance = container.Resolve<AbstractType>()
        Assert.NotNull(instance)

    type ConstructorWithSelfReferenceArg (arg:AbstractType) = inherit AbstractType()

    let [<Test>] ``resolving type with self type dependency in constructor should fail`` () =
        let container = Container()
        container.Register<AbstractType>(typeof<ConstructorWithSelfReferenceArg>, Singleton)
        Assert.Throws<TypeResolutionException>(fun () ->
                container.Resolve<AbstractType>() |> ignore
        ) |> ignore

    type Cyclic(arg:ConstructorWithCyclicReferenceArg) = do ()
    and  ConstructorWithCyclicReferenceArg (arg:Cyclic) = do ()

    let [<Test>] ``resolving type with cyclic type dependency in constructor should fail`` () =
        let container = Container()
        container.Register<ConstructorWithCyclicReferenceArg>(typeof<ConstructorWithCyclicReferenceArg>, Singleton)
        Assert.Throws<TypeResolutionException>(fun () ->
                container.Resolve<AbstractType>() |> ignore
        ) |> ignore

    type ConstructorWithFunArg (arg:unit -> ReferenceType) = 
        inherit AbstractType()
        member this.Factory () = arg()

    let [<Test>] ``resolving type with fun type argument in constructor should inject factory`` () =
        let container = Container()
        container.Register<AbstractType>(typeof<ConstructorWithFunArg>, Singleton)
        let instance = container.Resolve<AbstractType>() :?> ConstructorWithFunArg
        let refValue = instance.Factory()
        Assert.NotNull(refValue)

    do  ``resolving type with value type dependency in constructor should throw`` ()
        ``resolving type with reference type dependency in constructor should inject reference`` ()
        ``resolving type with self type dependency in constructor should fail`` ()
        ``resolving type with cyclic type dependency in constructor should fail`` ()
        ``resolving type with fun type argument in constructor should inject factory`` ()
