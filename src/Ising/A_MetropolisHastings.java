package Ising;

import java.util.HashMap;

/**
 * Bei neuer Energiedifferenz wird die Akzeptanzwahrscheinlichkeit gespeichert
 * damit sie nicht mehr berechnet werden muss.
 * 
 * @author oerpli
 * 
 */
public class A_MetropolisHastings implements Algorithm {

	public A_MetropolisHastings() {
		return;
	}

	private static HashMap<Double, Double> x = new HashMap<Double, Double>();

	// TODO should be externalized
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

	public boolean update(Lattice L) {
		return tryFlip(L, L.getRandomPoint());
	}

	private boolean tryFlip(Lattice L, Point p) {
		if (p.is(0))
			return false;
		p.getNewEnergy();
		boolean flip = A_MetropolisHastings.accept();
		Hamilton.accept(flip);
		if (flip)
			p.acceptFlip();
		return flip;// TODO
	}

	/**
	 * Algorithm- specific method of calculation of new energy. In the long term
	 * there should be a general approach which can be overriden.
	 */
	public void getNewEnergy(Point p) {
		Hamilton.E_m_new -= 2 * p.getV();
		Hamilton.E_nn_new -= 4 * p.getV() * p.getS();
	}
}
