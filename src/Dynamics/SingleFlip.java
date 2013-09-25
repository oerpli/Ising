package Dynamics;

import Model.Hamiltonian;
import Model.Point;

/**
 * Bei neuer Energiedifferenz wird die Akzeptanzwahrscheinlichkeit gespeichert
 * damit sie nicht mehr berechnet werden muss.
 * 
 * @author oerpli
 * 
 */
class SingleFlip implements I_Update {
	private Point p;

	public SingleFlip() {
	}

	public boolean update() {
		p = Algorithm.L.getRandomPoint();
		if (p.is(0))
			return false;
		getNewEnergy();
		boolean flip = Algorithm.accept();
		Hamiltonian.accept(flip);
		if (flip)
			p.acceptFlip();
		return flip;
	}

	/**
	 * Algorithm- specific method of calculation of new energy. In the long term
	 * there should be a general approach which can be overriden.
	 */
	public void getNewEnergy() {
		Hamiltonian.E_m_new -= 2 * p.getV();
		Hamiltonian.E_nn_new += 4 * p.getE();
	}
}
