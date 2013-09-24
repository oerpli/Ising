package Dynamics;

import java.util.HashMap;
import Randoms.R;
import Model.Hamilton;

public class Metropolis implements I_Accept {
	private static HashMap<Double, Double> x = new HashMap<Double, Double>();

	public Metropolis() {
	}

	public void clearMap() {
		x.clear();
	}

	public boolean accept() {
		double diffE = Hamilton.getDE();
		if (diffE <= 0)
			return true;
		if (!x.containsKey(diffE)) {
			x.put(diffE, Math.exp(-diffE * Hamilton.Beta()));
		}
		return R.nextDouble() < x.get(diffE);
	}
}
