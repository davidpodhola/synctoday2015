(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../bin"

(**
Sync.Today
======================

Documentation

<div class="row">
  <div class="span1"></div>
  <div class="span6">
    <div class="well well-small" id="nuget">
      The Sync.Today library can be <a href="https://nuget.org/packages/Sync.Today">installed from NuGet</a>:
      <pre>PM> Install-Package Sync.Today</pre>
    </div>
  </div>
  <div class="span1"></div>
</div>

## Library
Sync.Today can be used as a library. Any consuming application can add the NuGet and
start calling the methods.

### Relational database management system
An application can store data in Sync.Today and benefit from the automated data transfer
to other systems.

Example
-------

This example demonstrates using a function defined in this sample library.

*)
#r "Sync.Today.dll"
open Sync.Today

printfn "hello = %i" <| Library.hello 0

(**
Some more info

Samples & documentation
-----------------------

The library comes with comprehensible documentation. 
It can include tutorials automatically generated from `*.fsx` files in [the content folder][content]. 
The API reference is automatically generated from Markdown comments in the library implementation.

 * [Tutorial](tutorial.html) contains a further explanation of this sample library.

 * [API Reference](reference/index.html) contains automatically generated documentation for all types, modules
   and functions in the library. This includes additional brief samples on using most of the
   functions.
 
Contributing and copyright
--------------------------

The project is hosted on [GitHub][gh] where you can [report issues][issues], fork 
the project and submit pull requests. If you're adding a new public API, please also 
consider adding [samples][content] that can be turned into a documentation. You might
also want to read the [library design notes][readme] to understand how it works.

The library is available under Public Domain license, which allows modification and 
redistribution for both commercial and non-commercial purposes. For more information see the 
[License file][license] in the GitHub repository. 

  [content]: https://github.com/fsprojects/Sync.Today/tree/master/docs/content
  [gh]: https://github.com/fsprojects/Sync.Today
  [issues]: https://github.com/fsprojects/Sync.Today/issues
  [readme]: https://github.com/fsprojects/Sync.Today/blob/master/README.md
  [license]: https://github.com/fsprojects/Sync.Today/blob/master/LICENSE.txt
*)
