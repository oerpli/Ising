Ising Model
=======

Two different implementations of the Ising Model - one with Java/Processing and one with C#/WPF. The Java version was used for my bachelor thesis, the C# version has more features and will be/was used for presentation on the Campus festival of the University of Vienna (June 2015, 650th year University of Vienna celebration). 

Binaries are available under [releases](https://github.com/oerpli/Ising/releases) (for Windows only).

####Remarks 
* Swendsen Wang implementation (in the Java version) uses a recursive depth first search for the cluster finding algorithm. If the simulated lattice has more than 20k spins this can lead to a `StackOverflowException`. Change the stacksize for the Java JVM to circument this.

* The C# version should overall be more performant, though it currently lacks options to set the simulation speed as it was programmed as an interactive showcase for kids during a science festival and not for getting simulation data. If  you need this you'll have to do it on your own.
