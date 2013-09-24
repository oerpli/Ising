package Dynamics;

import Model.Lattice;

public abstract class Algorithm implements I_Update {
	public static Lattice L = Lattice.L;
	public static I_Accept A = new Metropolis();
	public static I_Update U = new Kawasaki();

	public Algorithm() {
		Algorithm.U = this;
	}
}
