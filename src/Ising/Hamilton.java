package Ising;

/**
 * Static class containing Energy related stuff. For more flexibility
 * (parallelization)maybe shouldn't be static..
 * 
 * Beta shouldn't be here i guess. - it's more algorithm (Metropolis Hastings)
 * related than anything else (including energy).
 * 
 * @author oerpli
 * 
 */
public class Hamilton {
	public static double J, h; // Fieldparameters
	public static double Beta; // Boltzmann (scaling factor);

	// Energy Values
	protected static int E_near = 0;// interaction related
	protected static int E_sum = 0;// field related

	/**
	 * Changing Energy Values should be private and updated via calls from the
	 * Lattice.
	 */
	public static int plus = 0;
	public static int E_near_new = 0;
	public static int E_sum_new = 0;

	public static void set(double J, double h, double Beta) {
		Hamilton.J = J;
		Hamilton.h = h;
		Hamilton.Beta = Beta;
	}

	public static double getE() { // Energy
		return -Hamilton.J * Hamilton.E_near - Hamilton.h * Hamilton.E_sum;
	}

	public static double getDE() {// Energy Difference
		return -Hamilton.J * Hamilton.E_near_new - Hamilton.h
				* Hamilton.E_sum_new;
	}
}
