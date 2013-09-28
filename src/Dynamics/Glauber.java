package Dynamics;

import java.util.HashMap;

import Model.Hamiltonian;
import Randoms.R;

class Glauber implements I_Accept {
	private HashMap<Double, Double> x = new HashMap<Double, Double>();

	public Glauber() {
	}

	public void clearMap() {
		x.clear();
	}

	public boolean accept() {
		double diffE = Hamiltonian.getDE();
		if (!x.containsKey(diffE)) {
			x.put(diffE, 1 / (1 + Math.exp(diffE * Hamiltonian.Beta())));
		}
		return R.nextDouble() < x.get(diffE);
	}
}
