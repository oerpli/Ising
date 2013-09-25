package Dynamics;

import java.util.ArrayList;

import Model.Point;

/**
 * Cluster algorithm
 */
class SwendsenWang implements I_Update {
	ArrayList<ArrayList<Point>> Clusters = new ArrayList<ArrayList<Point>>();

	public boolean update() {
		find();
		return false;
	}

	public void getNewEnergy() {
	}

	private void find() {
		long start = System.currentTimeMillis();
		for (Point p : Algorithm.L.sites) {
			if (p.c == null) {
				ArrayList<Point> x = new ArrayList<Point>();
				find(x, p);
				Clusters.add(x);
			}
		}
		System.out.println(System.currentTimeMillis() - start);
		for (Point p : Algorithm.L.sites)
			p.c = null;
		Clusters.clear();
	}

	private void find(ArrayList<Point> x, Point p) {
		if (p.c == null)
			p.c = p;
		for (Point n : p.near) {
			if (p.is(n) && n.c == null) {
				n.c = p.c;
				find(x, n);
			}
		}
		x.add(p);
	}
}
