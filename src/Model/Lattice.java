package Model;

import Dynamics.Algorithm;

/**
 * A lattice as basis for the Ising Algorithm. Only 2D periodic implemented so
 * far
 * 
 * @author oerpli
 */
public class Lattice {
	public static int D = (int) Math.sqrt(1); // "no" dead code this way
	public static Lattice L; // ugly workaround
	public Point[] sites;
	public final int size[]; // Dimensions
	public final int N;

	// public final boolean periodic = true; // Periodic boundaries
	// private HashSet<Point> changedPoints = new HashSet<Point>();

	public Lattice(int x, int y, int z, double seed, float J, float h, float kT) {
		Algorithm.L(this);
		size = new int[] { x, y, z };
		Lattice.D = 0;
		if (x > 0)
			Lattice.D++;
		if (y > 1)
			Lattice.D++;
		if (z > 1)
			Lattice.D++;
		N = x * y * z;
		sites = new Point[N];
		for (int i = 0; i < N; i++) {
			sites[i] = new Point(i, this, getXY(i),
					(byte) (Math.random() < seed ? 1 : -1));
		}
		Hamilton.reset();
		Hamilton.set(J, h, kT);
		for (Point p : sites)
			p.init();
		for (Point p : sites)
			p.initEnergy();

	}

	public boolean update() {
		return Algorithm.U().update();
	}

	public Point getRandomPoint() {
		return getPoint((int) Math.floor(Math.random() * this.N));// Math.floor()?
	}

	protected int[] getXY(int i) {
		int[] xyz = new int[Lattice.D];
		xyz[0] = i % size[0];
		if (Lattice.D == 1)
			return xyz;
		xyz[1] = (int) Math.floor(i / size[0]);
		if (Lattice.D == 2)
			return xyz;
		xyz[2] = (int) Math.floor(i / (size[0] * size[1]));
		return xyz;
	}

	protected Point getPoint(int i) {
		return sites[i];
	}

	/**
	 * Needed for initialization
	 * 
	 * @param x_y_z
	 *            Coordinates
	 * @return
	 */
	public Point getPoint(int x, int y, int z) {
		return getPoint(getI(x, y, z));
	}

	// TODO 3DIM
	protected int getI(int x, int y, int z) {
		x += size[0];
		y += size[1];
		z += size[2];
		return (x % size[0]) + (y % size[1]) * size[0] + (z % size[2])
				* size[0] * size[1];
	}

	/**
	 * String output of the lattice. For debug purposes mainly.
	 * 
	 * @author oerpli
	 */
	@Override
	public String toString() {
		StringBuffer s = new StringBuffer();
		for (Point p : sites) {
			s.append(p.toString());
			s.append((p.x == size[0] - 1 ? '\n' : ""));
		}
		return s.toString();
	}
}