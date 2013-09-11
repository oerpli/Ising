package Ising;

public class Point {
	public static Lattice L;
	private byte v; // value
	public final int x, y, z;
	// "von neumann"- neighbors
	public Point[] near = new Point[Lattice.D * 2];
	public final int index;
	private int S; // Sum of nearby v- values.
	private boolean draw = true; // changed?

	/**
	 * @return Value of the Point
	 */
	public byte getV() {
		return v;
	}

	public Point(int index, Lattice L, int[] xyz, byte v) {
		this.index = index;
		Point.L = L;
		this.v = v;
		this.x = xyz[0];
		if (Lattice.D > 1)
			this.y = xyz[1];
		else
			y = 0;
		if (Lattice.D > 2)
			this.z = xyz[2];
		else
			z = 0;
	}

	// POREN// TODO this part is shit
	public static int breite = 9;
	public static boolean poren = false;

	public void poren() {
		int tiefe = 30;
		int abstand = breite * 3;
		int xoffset = 1;

		if (poren) {
			if ((y > L.size[1] - tiefe)
					&& !((x - xoffset) % abstand >= 0 && (x - xoffset)
							% abstand < breite) || y > L.size[1] - 2) {
				this.v = 0;
			}
		}
	}

	@Override
	/** //TODO
	 * Renders 1 as + and everything else as -
	 */
	public String toString() {
		if (v == 1)
			return "+";// "▲";
		else if (v == -1)
			return "-";// "▼";
		else if (v == 0)
			return "X";// "█";
		else
			return "?";// "¿";
		// return "" + v;
	}

	/**
	 * Checks if this.v equals the given int value 'i'.
	 * 
	 * @param i
	 * @return
	 */
	public boolean is(int i) {
		return v == i;
	}

	/**
	 * Checks wether the given Point p has the same value as "this"
	 * 
	 * @param p
	 * @return
	 */
	public boolean is(Point p) {
		return this.v == p.v;
	}

	/**
	 * Sets neighbors and nn- energy - only used for initialization.
	 */
	public void init() {
		near[0] = L.getPoint(x - 1, y, z);
		near[1] = L.getPoint(x + 1, y, z);
		if (Lattice.D > 1) {
			near[2] = L.getPoint(x, y - 1, z);
			near[3] = L.getPoint(x, y + 1, z);
			if (Lattice.D > 2) {
				near[4] = L.getPoint(x, y, z + 1);
				near[5] = L.getPoint(x, y, z - 1);
			}
		}
		this.broadcast(v);
	}

	private void broadcast(int v) {
		for (Point p : near) {
			p.receive(v);
		}
	}

	private void receive(int n) {
		S += n;
	}

	/**
	 * Calculates the NN- Energy of the point - 0 if it's a wall (v == 0).
	 * Updates the Hamiltonian accordingly.
	 */
	public void initEnergy() {
		Hamilton.E_m += v;
		Hamilton.E_nn += v * S;
	}

	/**
	 * Calculates the NN- Energy of the point between flip and acceptance.
	 * Updates the Hamiltonian accordingly.
	 */
	public void getNewEnergy() {
		Hamilton.E_m_new -= 2 * v;
		Hamilton.E_nn_new += -4 * v * S;
	}

	/**
	 * Proposes flip - returns false if the Point is part of the wall (v==0).
	 * 
	 * @return
	 */
	public boolean proposeFlip() {
		return v != 0;
	}

	/**
	 * If algorithm accepts flip set draw- status to true and add the offset to
	 * the value. Clear dEnergy and offset afterwards.
	 * 
	 * @param accept
	 */
	public void acceptFlip() {
		draw = true;
		v = (byte) -v;
		this.broadcast(2 * v);
	}

	/**
	 * After point with current value is drawn set draw status to false (draw !=
	 * drawn)
	 */
	public void drawn() {
		draw = false;
	}

	/**
	 * @return true if Point needs to be drawn again.
	 */
	public boolean getRedraw() {
		return draw;
	}

	public boolean bond(int i) {
		return this.is(near[i]);
	}

	public String getSV() {
		return (S * v > 0 ? "" : " ") + -(S * v);
	}

}