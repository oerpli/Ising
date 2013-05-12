package Ising;

import java.util.ArrayList;

/**
 * A lattice as basis for the Ising Algorithm. Only 2D periodic implemented so
 * far
 * 
 * @author oerpli
 */
public class Lattice {
	// public Point cell[][]; // Lattice
	// public byte[][] cell;
	public Point[] sites;
	public final int size[]; // Dimensions for easier access
	public final boolean periodic = true; // Periodic boundaries
	private ArrayList<Point> changedPoints = new ArrayList<Point>();
	protected int E_near = 0;
	protected int E_sum = 0;
	public int plus = 0;
	public int E_near_new = 0;
	public int E_sum_new = 0;
	// private static final double kB = 1.3806488e-23;// Boltzmann SI
	// private static final double kB = 8e-5;// Boltzmann Gauﬂ
	// private static final double T = 1; // Temperature
	// 1, -0.5, 2.2.69*E/8
	private final double E; // Energy - neighbor0
	private final double J; // Field - sum
	private final double Beta;

	public Lattice(int x, int y, double seed, double E, double J, double Beta) {
		size = new int[] { x, y, x * y };
		sites = new Point[size[2]];
		// cell = new byte[x][y];
		this.init(seed);
		this.E = E;
		this.J = J;
		this.Beta = Beta;
	}

	/**
	 * Needed on first run to assign random values
	 */
	private void init(double seed) {
		for (int i = 0; i < size[2]; i++) {
			sites[i] = new Point(this, getXY(i),
					(byte) (Math.random() < seed ? 1 : -1));
		}
		for (Point p : sites) {
			p.getNear();
		}
		calcSum();
	}

	// public void tryFlip() {
	// tryFlip(1);
	// }

	public boolean tryFlip(int c) {
		int[] indexes = new int[c];
		for (int i = 0; i < c; i++) {
			indexes[i] = (int) Math.floor(Math.random() * this.size[2]);
		}
		return tryFlip(indexes);
	}

	private boolean tryFlip(int[] indexes) {
		for (int i : indexes) {
			Point p = getPoint(i);
			p.setOffset();
			changedPoints.add(p);
			for (Point n : p.near) {
				changedPoints.add(n);
			}
		}
		for (Point p : changedPoints) {
			p.getNewEnergy();
		}
		return acceptFlip(testFlip());// TODO
	}

	// TODO also except higher energy sometimes
	private boolean testFlip() {
		double diffE = getDiffHamiltonian();
		if (diffE <= 0) {
			return true;
		}
		return Math.random() < Math.exp(-diffE * Beta);
	}

	private boolean acceptFlip(boolean accept) {
		if (accept) {
			E_near += E_near_new;
			E_sum += E_sum_new;
			plus += E_sum_new >> 1;
		}
		for (Point p : changedPoints) {
			p.acceptFlip(accept);
		}
		E_sum_new = E_near_new = 0;
		changedPoints.clear();
		return accept;
	}

	// @Deprecated
	// private void setPoint(int i, int j) {
	// }

	protected int[] getXY(int i) {
		int[] xy = new int[2];
		xy[0] = i % size[0];
		xy[1] = (int) Math.floor(i / size[0]);
		return xy;
	}

	protected int getI(int x, int y) {
		y += size[1];
		x += size[0];
		return (x % size[0]) + (y % size[1]) * size[0];
	}

	protected int getI(int[] xy) {
		return getI(xy[0], xy[1]);
	}

	protected Point getPoint(int i) {
		return sites[i];
	}

	protected Point getPoint(int x, int y) {
		return getPoint(getI(x, y));
	}

	protected Point getPoint(int[] xy) {
		return getPoint(getI(xy));
	}

	public double getDiffHamiltonian() {
		return -E * E_near_new - J * E_sum_new;
	}

	public double getHamiltonian() {
		return -E * E_near - J * E_sum;
	}

	/**
	 * String output of the lattice
	 * 
	 * @author oerpli
	 */
	@Override
	public String toString() {
		StringBuffer s = new StringBuffer();
		for (Point p : sites) {
			s.append(p.getV() == 1 ? '+' : '-');
			s.append(p.x == size[0] - 1 ? '\n' : "");
		}
		return s.toString();
	}

	public String toStringNew() {
		StringBuffer s = new StringBuffer();
		for (Point p : sites) {
			s.append(p.getV() + p.getOffset() == 1 ? '+' : '-');
			s.append(p.x == size[0] - 1 ? '\n' : "");

		}
		return s.toString();
	}

	public void calcSum() {
		E_sum = 0;
		for (Point p : sites) {
			E_sum += p.getV();
		}
	}
}
