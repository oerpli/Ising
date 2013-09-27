package Dynamics;

import Model.Hamiltonian;
import Model.Lattice;

public abstract class Algorithm {
	private final static I_Accept[] A = { new Metropolis(), new Glauber(),
			new Cluster() };
	private final static I_Update[] U = { new SingleFlip(), new Kawasaki(),
			new SwendsenWang() };

	protected static Lattice L = Lattice.L;
	private static int a = 0;
	private static int u = 0;

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

	protected static boolean accept(int a) {
		return A[a].accept();
	}

	protected static boolean accept() {
		return accept(a);
	}

	public static void clearMap() {
		for (I_Accept a : A)
			a.clearMap();
	}

	public static String String() {
		String s = "";
		switch (u) {
		case 0:
			s += "mh";
			break;
		case 1:
			s += "ks";
			break;
		case 2:
			s += "sw";
			break;
		}
		if (u < 2)
			switch (a) {
			case 0:
				s += "-m";
				break;
			case 1:
				s += "-g";
				break;
			}
		return s;
	}
}