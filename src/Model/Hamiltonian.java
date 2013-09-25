package Model;

import Dynamics.Algorithm;

/**
 * Static class containing Energy related stuff. For more flexibility
 * (parallelization)maybe shouldn't be static.. // TODO Hamilton instanceable
 * 
 * Beta shouldn't be here i guess. - it's more algorithm (Metropolis Hastings)
 * related than anything else (including energy). // TODO Move Beta out of
 * Hamiltonian
 * 
 * @author oerpli
 * 
 */
public abstract class Hamiltonian {
	protected static float J, h; // Fieldparameters
	protected static float Beta; // Boltzmann (scaling factor)
	private static float kT;

	// Energy Values
	public static int E_nn = 0;// nn - interaction
	public static int E_m = 0;// magnetization

	/**
	 * Changing Energy Values should be private and updated via calls from the
	 * Lattice. // TODO Better E_x_new
	 */
	// public static int plus = 0;
	public static int E_nn_new = 0;
	public static int E_m_new = 0;

	public static void reset() {
		E_m = 0;
		E_nn = 0;
		E_nn_new = 0;
		E_m_new = 0;
	}

	public static void setKT(float kT) {
		Hamiltonian.kT = kT;
		Hamiltonian.Beta = 1 / kT;
		Algorithm.clearMap();
	}

	public static void set(float J, float h) {
		Hamiltonian.h = h;
		Hamiltonian.J = J;
	}

	public static void set(float J, float h, float kT) {
		Hamiltonian.set(J, h);
		Hamiltonian.setKT(kT);
	}

	public static double getE() { // Energy
		return -(J * E_nn + h * E_m);
	}

	public static double getDE() {// Energy Difference
		// System.out.println(-(J * E_nn_new + h * E_m_new)*Beta + " " +
		// E_nn_new);
		return -(J * E_nn_new + h * E_m_new);
	}

	public static boolean accept(boolean flip) {
		if (flip) {
			Hamiltonian.E_nn += Hamiltonian.E_nn_new;
			Hamiltonian.E_m += Hamiltonian.E_m_new;
		}
		Hamiltonian.E_nn_new = 0;
		Hamiltonian.E_m_new = 0;
		return flip;
	}

	public String toString() {
		String out = "J=" + J + '\n';
		out += "h=" + h + '\n';
		out += "kT=" + kT;
		return out;
	}

	public static double Beta() {
		return Beta;
	}

	public static double h() {
		return h;
	}

	public static double J() {
		return J;
	}

	public static double kT() {
		return kT;
	}

	public static float getkT() {
		return kT;
	}
}
