package Ising;

import java.util.HashMap;

/**
 * Bei neuer Energiedifferenz wird die Akzeptanzwahrscheinlichkeit gespeichert
 * damit sie nicht mehr berechnet werden muss.
 * 
 * @author oerpli
 * 
 */
public abstract class A_MetropolisHastings implements A_Interface {
	private static HashMap<Double, Double> x = new HashMap<Double, Double>();

	public static boolean accept() {
		double diffE = Hamilton.getDE();
		if (diffE <= 0)
			return true;
		if (!x.containsKey(diffE)) {
			x.put(diffE, Math.exp(-diffE * Hamilton.getBeta()));
		}
		return Math.random() < x.get(diffE);
	}

	public static void clearMap() {
		x.clear();
	}
}
