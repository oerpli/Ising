package Dynamics;

import Model.Lattice;

public abstract class Algorithm implements I_Update {
	protected static Lattice L = Lattice.L;
	private static I_Accept[] A = { new Metropolis(), new Glauber() };
	private static I_Update[] U = { new SingleFlip(), new Kawasaki(),
			new Wolff(), new SwendsenWang() };
	private static int a = 0;
	private static int u = 0;

	public Algorithm() {
	}

	public static I_Accept A() {
		return A[a];
	}

	public static I_Update U() {
		return U[u];
	}

	public static void switchAccept() {
		a = 1 - a;
	}

	public static void switchUpdate() {
		u = 1 - u;
	}

	public static void L(Lattice L) {
		if (L != null)
			Algorithm.L = L;
	}
}
