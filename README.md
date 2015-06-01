Ising Model
=======

Two different implementations of the Ising Model - one with Java/Processing and one with C#/WPF. The Java version was used for my bachelor thesis, the C# version has more features and will be/was used for presentation on the Campus festival of the University of Vienna (June 2015, 650th year University of Vienna celebration). 

####Remarks 
* Swendsen Wang implementation (Java only) is pretty bad (StackOverflowException in the cluster- finding method with more than 20k spins due to recursive instead of iterative dfs)

* The C# version should overall be more performant, though it currently lacks options to set the simulation speed (as in "simulate more and don't bother with rendering") so if you need this you'll have to do it on your own.
