package Dynamics;

import java.util.HashMap;

import Model.Hamilton;
import Randoms.R;

public class Glauber implements I_Accept {
	private static HashMap<Double, Double> x = new HashMap<Double, Double>();

	public Glauber() {
	}

	public void clearMap() {
		x.clear();
	}

	public boolean accept() {
		double diffE = Hamilton.getDE();
		if (!x.containsKey(diffE)) {
			x.put(diffE, 1 / (1 + Math.exp(diffE * Hamilton.Beta())));
		}
		return R.nextDouble() < x.get(diffE);
	}
}
