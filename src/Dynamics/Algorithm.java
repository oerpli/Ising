package Dynamics;

import Model.Hamiltonian;
import Model.Lattice;

public abstract class Algorithm {
	protected static Lattice L = Lattice.L;
	private static I_Accept[] A = { new Metropolis(), new Glauber(),
			new Cluster() };
	private static I_Update[] U = { new SingleFlip(), new Kawasaki(),
			new SwendsenWang() };
	private static int a = 2;
	private static int u = 2;

	public Algorithm() {
	}

	public static void L(Lattice L) {
		if (L != null)
			Algorithm.L = L;
	}

	public static boolean update() {
		if (u < 2)
			for (int i = 0; i < L.N; i++)
				Hamiltonian.accept(U[u].update());
		else
			Hamiltonian.accept(U[u].update());
		return true;
	}

	public static boolean flip() {
		int save = u;
		u = 0;
		while (!Hamiltonian.accept(U[0].update()))
			u = save;
		return true;
	}

	protected static boolean accept() {
		return A[a].accept();
	}

	public static void clearMap() {
		for (I_Accept a : A)
			a.clearMap();
	}

	public static void switchUpdate() {
		u = 1 - u;
	}

	public static void switchAccept() {
		a = 1 - a;
	}

}