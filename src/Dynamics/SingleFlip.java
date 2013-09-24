package Dynamics;

import Model.Hamilton;
import Model.Point;

/**
 * Bei neuer Energiedifferenz wird die Akzeptanzwahrscheinlichkeit gespeichert
 * damit sie nicht mehr berechnet werden muss.
 * 
 * @author oerpli
 * 
 */
public class SingleFlip implements I_Update {
	private static Point p;

	public SingleFlip() {
	}

	public boolean update() {
		p = Algorithm.L.getRandomPoint();
		if (p.is(0))
			return false;
		getNewEnergy();
		boolean flip = Algorithm.A().accept();
		Hamilton.accept(flip);
		if (flip)
			p.acceptFlip();
		return flip;// TODO
	}

	/**
	 * Algorithm- specific method of calculation of new energy. In the long term
	 * there should be a general approach which can be overriden.
	 */
	public void getNewEnergy() {
		Hamilton.E_m_new -= 2 * p.getV();
		Hamilton.E_nn_new += 4 * p.getE();
	}
}
