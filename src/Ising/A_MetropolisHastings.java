package Ising;

import java.util.HashMap;

/**
 * Bei neuer Energiedifferenz wird die Akzeptanzwahrscheinlichkeit gespeichert
 * damit sie nicht mehr berechnet werden muss. Wenn Problem bekannt und
 * Algorithmus nicht geändert wird theoretisch auch im Voraus möglich um
 * anschließend nicht mehr prüfen zu müssen. Bereits ohne diese Optimierung von
 * 4MF/s auf 4.6 bis 5.5MF/s.
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
			x.put(diffE, Math.exp(-diffE * Hamilton.Beta));
		}
		return Math.random() < x.get(diffE);
	}
}
