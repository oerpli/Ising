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
	public static float J, h; // Fieldparameters
	private static float Beta; // Boltzmann (scaling factor)//TODO
	public static float kT;

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

	public static void setJ(float J) {
		Hamilton.J = J;
		A_MetropolisHastings.clearMap();
	}

	public static void setH(float h) {
		Hamilton.h = h;
		A_MetropolisHastings.clearMap();
	}

	public static void setKT(float kT) {
		Hamilton.kT = kT;
		Hamilton.Beta = 1 / kT;
		A_MetropolisHastings.clearMap();
	}

	public static void set(float J, float h, float kT) {
		Hamilton.setJ(J);
		Hamilton.setH(h);
		Hamilton.setKT(kT);
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

	public static String out() {
		String out = "J=" + J + '\n';
		out += "h=" + h + '\n';
		out += "kT=" + kT;
		return out;
	}

	public static double getBeta() {
		return Beta;
	}
}
