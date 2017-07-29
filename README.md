Ising Model
=======

Two different implementations of the Ising Model - one with Java/Processing and one with C#/WPF. The Java version was used for my bachelor thesis, the C# version has more features and will be/was used for presentation on the Campus festival of the University of Vienna (June 2015, 650th year University of Vienna celebration). 

Binaries are available under [releases](https://github.com/oerpli/Ising/releases) (for Windows only).

## C#/WPF version:
### Features

* Different color themes
* Simulation size can be changed while the simulation is running (spins will be scaled)
* Two different algorithms: Single spin flip metropolis hastings MCMC and spin preserving Kawasaki dynamics
* Coupling can be changed
* Real time plot of energy and magnetization
* **Interactive simulation**: Spins (or areas of spins) can be changed to face either up/down or to be non-interacting (left, middle, right mouse click, for all three: draw rectangle to select spins). The latter can be used to showcase heterogeneous nucleation (nucleation seeds form faster in pore-like structures, [Frenkel 2006, Seeds of phase change](https://www.nature.com/nature/journal/v443/n7112/full/443641a.html), [Page & Sear 2006, Heterogeneous Nucleation in and out of Pores](https://journals.aps.org/prl/abstract/10.1103/PhysRevLett.97.065701))

### Video
![](https://raw.githubusercontent.com/oerpli/Ising/master/cs.gif)

## Java/Processing version:
### Features: 

* Simulation size can be changed
* Speed can be adapted (omit rendering of frames to produce data faster)
* Log data for further analysis
* Three algorithms: The two mentioned above as well as Swendsen-Wang cluster flip algorithm (removes auto-correlation from magnetization)
* Real time plotting (though ugly)

### Video
![](https://raw.githubusercontent.com/oerpli/Ising/master/jp.gif)

## Remarks 
* Swendsen Wang implementation (in the Java version) uses a recursive depth first search for the cluster finding algorithm. If the simulated lattice has more than 20k spins this can lead to a `StackOverflowException`. Change the stacksize for the Java JVM to circumvent this.

* The C# version should overall be more performant, though it currently lacks options to set the simulation speed as it was programmed as an interactive showcase for kids during a science festival and not for getting simulation data. If  you need this you'll have to do it on your own.
