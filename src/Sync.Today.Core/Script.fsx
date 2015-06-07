// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.

#load "Container.fs"
open IoC

// Define your library scripting code here

module Usage =

    type ICalculate =
        abstract member Incr : int -> int

    type Calculator () =
        interface ICalculate with
            member this.Incr(x:int) = x + 1
    
    let container = Container()

    container.Register<ICalculate>(typeof<Calculator>, Singleton)

    let calc = container.Resolve<ICalculate>()
    printfn "%d" (calc.Incr 1)

    container.Release(calc)