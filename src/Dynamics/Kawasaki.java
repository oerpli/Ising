package Dynamics;

import Model.Hamilton;
import Model.Point;
import Randoms.R;

public class Kawasaki implements I_Update {
	private static Point p, x;

	public Kawasaki() {
	}

	public boolean update() {
		p = Algorithm.L.getRandomPoint();
		x = p.near[R.nextInt(p.near.length)];
		if (p.is(x))
			return true;
		getNewEnergy();
		boolean flip = Algorithm.A().accept();
		Hamilton.accept(flip);
		if (flip) {
			p.acceptFlip();
			x.acceptFlip();
		}
		return flip;// TODO
	}

	/**
	 * Algorithm- specific method of calculation of new energy. In the long term
	 * there should be a general approach which can be overriden.
	 */
	public void getNewEnergy() {
		Hamilton.E_nn_new = 4 * (p.getE() + x.getE()) - 8;
	}
}
