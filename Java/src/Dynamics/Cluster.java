package Dynamics;

import Model.Hamiltonian;
import Randoms.R;

public class Cluster implements I_Accept {
	private double x;

	public Cluster() {
	}

	public void clearMap() {
		x = 1 - Math.exp(-2 * Hamiltonian.Beta());
	}

	public boolean accept() {
		return R.nextDouble() < x;
	}
}
