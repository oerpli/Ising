package Dynamics;

import Model.Hamiltonian;
import Model.Point;
import Randoms.R;

class Kawasaki implements I_Update {
	private Point p, x;

	public Kawasaki() {
	}

	public boolean update() {
		p = Algorithm.L.getRandomPoint();
		x = p.near[R.nextInt(p.near.length)];
		if (p.is(x))
			return true;
		getNewEnergy();
		boolean flip = Algorithm.accept();
		Hamiltonian.accept(flip);
		if (flip) {
			p.acceptFlip();
			x.acceptFlip();
		}
		return flip;
	}

	/**
	 * Algorithm- specific method of calculation of new energy. In the long term
	 * there should be a general approach which can be overriden.
	 */
	public void getNewEnergy() {
		Hamiltonian.E_nn_new = 4 * (p.getE() + x.getE()) - 8;
	}
}
