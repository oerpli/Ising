package Ising;

import java.util.HashSet;

/**
 * A lattice as basis for the Ising Algorithm. Only 2D periodic implemented so
 * far
 * 
 * @author oerpli
 */
public class Lattice {
	public static int D = (int) Math.sqrt(1);
	// public Point cell[][]; // Lattice
	// public byte[][] cell;
	public Point[] sites;
	public final int size[]; // Dimensions for easier access
	public final int N;
	// public final boolean periodic = true; // Periodic boundaries
	private HashSet<Point> changedPoints = new HashSet<Point>();

	// protected int E_near = 0;
	// protected int E_sum = 0;
	// public int plus = 0;
	// public int E_near_new = 0;
	// public int E_sum_new = 0;

	// private static final double kB = 1.3806488e-23;// Boltzmann SI
	// private static final double kB = 8e-5;// Boltzmann Gauß
	// private static final double T = 1; // Temperature
	// 1, -0.5, 2.2.69*E/8
	// private final double J; // Energy - neighbor0
	// private final double h; // Field - sum
	// private final double Beta;

	public Lattice(int x, int y, int z, double seed, double J, double h,
			double Beta) {
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
		Hamilton.set(J, h, Beta);
		for (Point p : sites)
			p.init();
		for (Point p : sites)
			p.initEnergy();

	}

	// public void tryFlip() {
	// tryFlip(1);
	// }

	public boolean tryFlip(int count) {
		int[] indexes = new int[count];
		for (int i = 0; i < count; i++) {
			indexes[i] = (int) Math.floor(Math.random() * this.N);
		}
		return tryFlip(indexes);
	}

	private boolean tryFlip(int[] indexes) {
		for (int i : indexes) {
			Point p = getPoint(i);
			if (p.proposeFlip())
				changedPoints.add(p);
		}
		for (Point p : changedPoints)
			p.getNewEnergy();
		return acceptFlip();// TODO
	}

	// POREN// TODO this part is shit
	private boolean acceptFlip() {
		boolean flip = A_MetropolisHastings.accept();
		Hamilton.accept(flip);
		if (flip)
			for (Point p : changedPoints)
				p.acceptFlip();
		changedPoints.clear();
		return flip;
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
	 * @param x
	 *            Coordinate
	 * @param y
	 *            Coordinate
	 * @param z
	 *            Coordinate
	 * @return
	 */
	public Point getPoint(int x, int y, int z) {
		return getPoint(getI(x, y, z));
	}

	private Point getPoint(int[] xyz) {
		return getPoint(getI(xyz));
	}

	// TODO DEBUG
	private int getI(int[] xyz) {
		return getI(xyz[0], xyz[1], xyz[2]);
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
	 * String output of the lattice
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
