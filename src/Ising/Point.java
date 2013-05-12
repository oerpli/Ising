package Ising;

public class Point {
	private byte v; // value
	private byte offset;
	public final int x;
	public final int y;
	public Point[] near = new Point[4]; // "von neumann"- neighbors
	private int e_near = 0;
	private int e_near_new = 0;

	private boolean e_in_new = false;
	private boolean draw = true;
	public static Lattice L;
	public static boolean poren = false;

	public byte getV() {
		return v;
	}

	public Point(Lattice L, int[] xy, byte v) {
		Point.L = L;
		this.v = v;
		if (v == 1) {
			L.plus++;
		}
		this.x = xy[0];
		this.y = xy[1];

		// POREN
		int tiefe = 30;
		int breite = 9;
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
	public String toString() {
		if (v == 1)
			return "+";
		else
			return "-";
		// return "" + v;
	}

	public boolean is(int i) {
		return v == i;
	}

	public boolean is(Point p) {
		return this.v == p.v;
	}

	public void getNear() {
		near[0] = L.getPoint(x, y - 1);
		near[1] = L.getPoint(x + 1, y);
		near[2] = L.getPoint(x, y + 1);
		near[3] = L.getPoint(x - 1, y);
		getEnergy();
	}

	private void getEnergy() {
		if (v == 0)
			return;
		L.E_near -= e_near;
		byte sum = 0;
		for (Point p : near) {
			sum += p.v;
		}
		e_near = sum * v;
		L.E_near += e_near;
	}

	public void getNewEnergy() {
		if (!e_in_new && v != 0) {
			L.E_near_new -= e_near;
			byte sum = 0;
			for (Point p : near) {
				sum += p.v + p.offset;
			}
			e_near_new = sum * (v + offset);
			L.E_near_new += e_near_new;
			e_in_new = true;
			L.E_sum_new += offset;
		}
	}

	public void setOffset() {
		this.offset = (byte) -(v << 1); // -v*2
	}

	public byte getOffset() {
		return offset;
	}

	public void acceptFlip(boolean accept) {
		if (accept) {
			if (!draw && offset != 0) {
				draw = true;
			}
			v += offset;
			this.e_near = this.e_near_new;
		}
		this.e_near_new = 0;
		this.e_in_new = false;
		this.offset = 0;
	}

	public void drawn() {
		draw = false;
	}

	public boolean getRedraw() {
		return draw;
	}
}
