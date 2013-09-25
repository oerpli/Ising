package Dynamics;

import java.util.HashMap;
import Randoms.R;
import Model.Hamiltonian;

class Metropolis implements I_Accept {
	private HashMap<Double, Double> x = new HashMap<Double, Double>();

	public Metropolis() {
	}

	public void clearMap() {
		x.clear();
	}

	public boolean accept() {
		double diffE = Hamiltonian.getDE();
		if (diffE <= 0)
			return true;
		if (!x.containsKey(diffE)) {
			x.put(diffE, Math.exp(-diffE * Hamiltonian.Beta()));
			System.out.println(Hamiltonian.getkT() + " " + diffE + " " + x.get(diffE));
		}
		return R.nextDouble() < x.get(diffE);
	}
}
