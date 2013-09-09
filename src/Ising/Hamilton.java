package Ising;

/**
 * Static class containing Energy related stuff. For more flexibility
 * (parallelization)maybe shouldn't be static.. // TODO
 * 
 * 
 * Beta shouldn't be here i guess. - it's more algorithm (Metropolis Hastings)
 * related than anything else (including energy). // TODO
 * 
 * @author oerpli
 * 
 */
public abstract class Hamilton {
	public static double J, h; // Fieldparameters
	public static double Beta; // Boltzmann (scaling factor)//TODO

	// Energy Values
	public static int E_nn = 0;// nn - interaction
	public static int E_m = 0;// magnetization

	/**
	 * Changing Energy Values should be private and updated via calls from the
	 * Lattice. // TODO
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

	public static void set(double J, double h, double Beta) {
		Hamilton.J = J;
		Hamilton.h = h;
		Hamilton.Beta = Beta;
	}

	public static double getE() { // Energy
		return -(J * E_nn + h * E_m);
	}

	public static double getDE() {// Energy Difference
		return -(J * E_nn_new + h * E_m_new);
	}

	public static void accept(boolean flip) {
		if (flip) {
			Hamilton.E_nn += Hamilton.E_nn_new;
			Hamilton.E_m += Hamilton.E_m_new;
		}
		Hamilton.E_nn_new = 0;
		Hamilton.E_m_new = 0;
	}

	// public static String out() {
	// String out = "NN: " + E_nn + "\n";
	// out += "NNn: " + E_nn_new + "\n";
	// out += "M: " + E_m + "\n";
	// out += "Mn: " + E_m_new;
	// return out;
	// }
}
