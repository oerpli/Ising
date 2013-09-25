package Dynamics;

import java.util.ArrayList;
import Randoms.R;
import Model.Point;
import Model.Hamiltonian;

/**
 * Cluster algorithm
 */
class SwendsenWang implements I_Update {
	private ArrayList<ArrayList<Point>> Clusters = new ArrayList<ArrayList<Point>>();
	private Point flip;

	public boolean update() {
		long start = System.currentTimeMillis();
		Clusters.clear();
		findClusters();
		for (ArrayList<Point> x : Clusters) {
			for (Point p : x) {
				boolean z = p.bond(0) && Algorithm.accept();
				p.virtualbonds[0] = z;
				p.near[0].virtualbonds[1] = z;
				z = p.bond(2) && Algorithm.accept();
				p.virtualbonds[2] = z;
				p.near[2].virtualbonds[3] = z;
			}
		}
		findVirtualClusters();
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
		System.out.println(System.currentTimeMillis() - start);
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
	private void findClusters() {
		for (Point p : Algorithm.L.sites) {
			if (p.c == null) {
				ArrayList<Point> x = new ArrayList<Point>();
				findCluster(x, p);
				Clusters.add(x);
			}
		}
		for (Point p : Algorithm.L.sites)
			p.c = null;
	}

	private void findCluster(ArrayList<Point> x, Point p) {
		if (p.c == null)
			p.c = p;
		for (Point n : p.near) {
			if (p.is(n) && n.c == null) {
				n.c = p.c;
				findCluster(x, n);
			}
		}
		x.add(p);
	}

	// alternativ suche über vorhandene cluster und direkt ändern nach jedem
	// cluster. wäre schneller. weniger code recycling.
	private void findVirtualClusters() {
		for (Point p : Algorithm.L.sites) {
			if (p.c == null) {
				ArrayList<Point> x = new ArrayList<Point>();
				findVirtualCluster(x, p);
				Clusters.add(x);
			}
		}
		for (Point p : Algorithm.L.sites)
			p.c = null;
	}

	private void findVirtualCluster(ArrayList<Point> x, Point p) {
		if (p.c == null)
			p.c = p;
		for (int i = 0; i < p.near.length; i++) {
			if (p.virtualbonds[i] && p.near[i].c == null) {
				p.near[i].c = p.c;
				findVirtualCluster(x, p.near[i]);
			}
		}
		x.add(p);
	}
}