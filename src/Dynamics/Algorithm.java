package Dynamics;

import Model.Lattice;

public abstract class Algorithm {
	protected static Lattice L = Lattice.L;
	private static I_Accept[] A = { new Metropolis(), new Glauber() };
	private static I_Update[] U = { new SingleFlip(), new Kawasaki(),
			new SwendsenWang() };
	private static int a = 0;
	private static int u = 0;

	public Algorithm() {
	}

	public static void L(Lattice L) {
		if (L != null)
			Algorithm.L = L;
	}

	public static boolean update() {
		return U[u].update();
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