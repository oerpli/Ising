package Ising;

public class Point {
	public static Lattice L;
	private byte v; // value
	private byte offset;// between flip and flip accept
	public final int x, y, z;
	public Point[] near = new Point[S_Initialize.D * 2]; // "von neumann"-
															// neighbors

	private int e_nn = 0;
	private int e_nn_new = 0;

	private boolean e_in_new = false;
	private boolean draw = true;
	public final int i;// TODO DEBUG

	/**
	 * @return Value of the Point
	 */
	public byte getV() {
		return v;
	}

	public int getVn() {
		return getV() + offset;
	}

	public Point(int i, Lattice L, int[] xyz, byte v) {
		this.i = i;
		Point.L = L;
		this.v = v;

		this.x = xyz[0];
		if (S_Initialize.D > 1)
			this.y = xyz[1];
		else
			y = 0;
		if (S_Initialize.D > 2)
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
		near[0] = L.getPoint(x, y - 1, z);
		near[1] = L.getPoint(x + 1, y, z);
		if (S_Initialize.D > 1) {
			near[2] = L.getPoint(x, y + 1, z);
			near[3] = L.getPoint(x - 1, y, z);
			if (S_Initialize.D > 2) {
				near[4] = L.getPoint(x, y, z + 1);
				near[5] = L.getPoint(x, y, z - 1);
			}
		}
		this.getEnergy();
	}

	/**
	 * Calculates the NN- Energy of the point - 0 if it's a wall (v == 0).
	 * Updates the Hamiltonian accordingly.
	 */
	private void getEnergy() {
		Hamilton.E_m += v;
		if (v == 0)
			return;
		// Hamilton.E_nn -= e_nn;
		byte sum = 0;
		for (Point p : near) {
			sum += p.v;
		}
		e_nn = sum * v;
		Hamilton.E_nn += e_nn;
	}

	/**
	 * Calculates the NN- Energy of the point between flip and acceptance.
	 * Updates the Hamiltonian accordingly.
	 */
	public void getNewEnergy() {
		if (!e_in_new && v != 0) {
			Hamilton.E_nn_new -= e_nn;
			short sum = 0;
			for (Point p : near) {
				sum += p.v + p.offset;
			}
			e_nn_new = sum * (v + offset);
			Hamilton.E_nn_new += e_nn_new;
			Hamilton.E_m_new += offset;
			e_in_new = true;
		}
	}

	/**
	 * Proposes flip - returns false if the Point is part of the wall (v==0).
	 * 
	 * @return
	 */
	public boolean proposeFlip() {
		this.offset = (byte) -(v * 2); // -v*2
		return v != 0;
	}

	/**
	 * If algorithm accepts flip set draw- status to true and add the offset to
	 * the value. Clear dEnergy and offset afterwards.
	 * 
	 * @param accept
	 */
	public void acceptFlip(boolean accept) {
		if (accept) {
			draw = true;// if(!draw)?
			v += offset;
			this.e_nn = this.e_nn_new;
		}
		this.e_nn_new = 0;
		this.offset = 0;
		this.e_in_new = false;
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

}