package Dynamics;

import Model.Lattice;

public abstract class Algorithm implements I_Update {
	protected static Lattice L = Lattice.L;
	private static I_Accept[] A = { new Metropolis(), new Glauber() };
	private static I_Update[] U = { new SingleFlip(), new Kawasaki() };
	private static int a = 0;
	private static int u = 0;

	public Algorithm() {// Algorithm.U = this;
	}

	public static I_Accept A() {
		return A[a];
	}

	public static I_Update U() {
		return U[u];
	}

	public static void switchAccept() {
		a = -a + 1;
	}

	public static void switchUpdate() {
		u = -u + 1;
	}

	public static void L(Lattice L) {
		if (L != null)
			Algorithm.L = L;
	}
}
