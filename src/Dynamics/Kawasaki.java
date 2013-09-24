package Dynamics;

import Model.Hamilton;
import Model.Point;

public class Kawasaki extends Algorithm {
	private static Point p, x;

	public Kawasaki() {
		super();
	}

	public boolean update() {
		p = L.getRandomPoint();
		x = p.near[(int) Math.floor(Math.random() * 4)];
		if (p.is(x))
			return true;
		p.getNewEnergy();
		boolean flip = A().accept();
		Hamilton.accept(flip);
		if (flip)
			p.kawasakiSwitch(x);
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
