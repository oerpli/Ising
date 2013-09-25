package Dynamics;

import java.util.ArrayList;
import Randoms.R;
import Model.Point;
import Model.Hamiltonian;

/**
 * Cluster algorithm
 */
class SwendsenWang implements I_Update {
	ArrayList<ArrayList<Point>> Clusters = new ArrayList<ArrayList<Point>>();
	char[] c = "abcdefghijklmnopqrstuvwxyz".toCharArray();
	Point flip;

	public boolean update() {
		Clusters.clear();
		find();
		for (ArrayList<Point> x : Clusters) {
			for (Point p : x) {
				if (p.bond(0)) {
					if (R.nextDouble() > Math.exp(-2 * Hamiltonian.Beta())) {
						p.virtualbonds[0] = true;
						p.near[0].virtualbonds[1] = true;
					} else {
						p.virtualbonds[0] = false;
						p.near[0].virtualbonds[1] = false;
					}
				}
				if (p.bond(2)) {// unrolled loop basically
					if (R.nextDouble() > Math.exp(-2 * Hamiltonian.Beta())) {
						p.virtualbonds[2] = true;
						p.near[2].virtualbonds[3] = true;
					} else {
						p.virtualbonds[2] = false;
						p.near[2].virtualbonds[3] = false;
					}
				}
			}
		}
		findV();
		for (ArrayList<Point> x : Clusters) {
			int v = (R.nextDouble() > 0.5 ? -1 : 1);
			for (Point p : x) {
				if (!p.is(v)) {
					flip = p;
					this.getNewEnergy();
					p.acceptFlip();
				}
			}
		}
		Clusters.clear();
		Hamiltonian.accept(true);
		return true;
	}

	public void getNewEnergy() {
		Hamiltonian.E_m_new -= 2 * flip.getV();
		Hamiltonian.E_nn_new += 4 * flip.getE();
	}

	/**
	 * DF cluster finding algorithm. could be replaced with a iterative
	 * implementation (hard) or a semirecursive version with a stack.
	 * 
	 * BF would be better for huge clusters - currently limited at approx. 20k
	 * sites due to stacksize.
	 */
	private void find() {
		int i = -1;
		for (Point p : Algorithm.L.sites) {
			if (p.c == null) {
				i++;
				ArrayList<Point> x = new ArrayList<Point>();
				find(x, p, i);
				Clusters.add(x);
			}
		}
		System.out.println(Algorithm.L.toStringCluster());
		for (Point p : Algorithm.L.sites)
			p.c = null;
	}

	private void find(ArrayList<Point> x, Point p, int i) {
		p.cluster = c[i];
		if (p.c == null)
			p.c = p;
		for (Point n : p.near) {
			if (p.is(n) && n.c == null) {
				n.c = p.c;
				find(x, n, i);
			}
		}
		x.add(p);
	}

	private void findV() {
		int i = -1;
		for (Point p : Algorithm.L.sites) {
			if (p.c == null) {
				i++;
				ArrayList<Point> x = new ArrayList<Point>();
				findV(x, p, i);
				Clusters.add(x);
			}
		}
		System.out.println(Algorithm.L.toStringCluster());
		for (Point p : Algorithm.L.sites)
			p.c = null;
	}

	private void findV(ArrayList<Point> x, Point p, int C) {
		p.cluster = c[C];
		if (p.c == null)
			p.c = p;
		for (int i = 0; i < p.near.length; i++) {
			if (p.virtualbonds[i] && p.near[i].c == null) {
				p.near[i].c = p.c;
				findV(x, p.near[i], C);
			}
		}
		x.add(p);
	}
}