package Render;

import processing.core.*;
import Model.*;
import Data.*;
import controlP5.*;

public class IsingRender extends PApplet {
	private static final long serialVersionUID = -1664637672574501774L;
	// Physics:
	// private float J; // Coupling Constant
	// private float h; // Field
	// private float kT;// Temperature
	private static double seed;// Seed

	// Renderparameters
	public static int N1 = 10;
	public static int N2 = N1;
	public static int N = N1 * N2;
	float J = 1;
	float h = 0;
	float kT = 2F;
	protected int size;

	// System
	protected PGraphics lattice; // chart_bg,chart
	protected PGraphics info;
	protected static boolean play = false;

	protected Lattice L;
	protected int tab = 0;
	// private long calced = 0;

	private long time;
	private int c;

	public static long sweeps;

	// private XYChart lineChart;
	public void setup() {
		frameRate(60);
		size(1000, 700);
		lattice = createGraphics(700, 700);
		info = createGraphics(600, 700);
		background(0);
		setupLattice();
		S.setup(this);
		S.speed = 1;
		updateHamilton();
		updateKT();
	}

	protected void setupLattice() {
		// N = 4;
		N2 = N1;
		seed = 0.5;
		System.out.println(N1 + " " + N2);
		L = new Lattice(N1, N2, 1, seed, J, h, kT);
		size = 700 / N1;
	}

	/**
	 * Toggle simulation.
	 */
	public void PlayPause() {
		play = !play;
	}

	/**
	 * Stop simulation
	 */
	public static void Stop() {
		play = false;
	}

	public void controlEvent(ControlEvent event) {
		S.controlEvent(event);
		tab = 0;
	}

	protected void updateHamilton() {
		S.cp5.get(Textfield.class, "Jx").setValue("" + J);
		S.cp5.get(Textfield.class, "hx").setValue("" + h);
		Hamiltonian.set(J, h);
	}

	protected void updateKT() {
		S.cp5.get(Textfield.class, "kTx").setValue("" + kT);
		Hamiltonian.setKT(kT);
		System.out.println("kT: " + kT);
	}

	public void HamiltonAdd(double dJ, double dh, double dT) {
		J += dJ;
		h += dh;
		kT += dT;
		S.cp5.get(Textfield.class, "Jx").setValue("" + J);
		S.cp5.get(Textfield.class, "hx").setValue("" + h);
		Hamiltonian.set(J, h);
		S.cp5.get(Textfield.class, "kTx").setValue("" + kT);
		Hamiltonian.setKT(kT);
	}

	protected void reset() {
		Stop();
		sweeps = 0;
		setupLattice();
		setupLattice();
		drawLattice(true);
	}

	protected void sweep(int n) {
		time = -System.currentTimeMillis();
		this.c = L.N * n;
		for (int i = 0; i < n; i++) {
			if (!play)
				break;
			L.update();
			Log.log();
			sweeps += 1;

		}
		tab = 0;
		time += System.currentTimeMillis();
		if (time > 1000 && S.speed > 1)
			S.speed /= 2;

	}

	protected void flip(int n) {
		for (int i = 0; i < n; i++) {
			L.update();
			// calced++;
		}
		Log.log();
		tab = 0;
	}

	private String stringFlips() { // TODO new Speed output
		String out = "";
		if (play) {
			out += time + "ms, ";
			out += "(" + (c / (time + 1)) + "F/ms)";
		}
		return out;
	}

	public void draw() {
		switch (tab) {
		case 0:
			drawLattice(false);
			image(lattice, 0, 0);
			drawInfo();
			image(info, 700, 0);
			tab = 2;
			// printFlips(0.0001);
			break;
		}
		if (play)
			sweep(S.speed);
	}

	private void drawInfo() {// TODO new Stats Output
		info.beginDraw();
		info.noStroke();
		info.fill(0);
		info.textSize(18);
		info.background(0);
		info.fill(color(255, 255, 255));
		// String energy = scientific(Hamilton.getE());
		String energy = "" + Hamiltonian.getE() + '\n'; // 123412521 instead of
														// 1.2*10^x
		String s = L.size[0] + "x" + L.size[1] + '\n';
		s += "Speed: " + S.speed + '\n' + stringFlips() + '\n';
		s += "E: " + energy + '\n';
		s += "E_m: " + Hamiltonian.E_m + '\n';
		info.text(s, 5, 350);
		// info.text("", 206, 20);
		info.endDraw();
	}

	// private String scientific(double x) {
	// int log10 = (int) Math.floor(Math.log10(Math.abs(x)));
	// String out = S.df.format(x / Math.pow(10, log10)) + "x10^" + log10;
	// return out;
	// }

	protected void drawLattice(boolean drawAll) {
		lattice.beginDraw();
		lattice.noStroke();
		lattice.textSize(16);
		for (Point p : L.sites) {
			if (S.NUMBERS && (p.drawnumber() || drawAll)) {
				drawPoint(p, false);
				drawNumber(p);
			} else if (p.draw() || drawAll)
				drawPoint(p, true);
		}
		lattice.endDraw();
	}

	private void drawPoint(Point p, boolean recursive) {
		int x = p.getV() + 1;// map v-values to color-array (-1,0,1 -> 0,1,2);
		if (S.BONDS) {
			lattice.fill(S.c[x][0], S.c[x][1], S.c[x][2]);
			lattice.rect(p.x * size + size / 4, p.y * size + size / 4,
					size / 2, size / 2);
			drawBonds(p, true);
		} else {
			lattice.fill(S.c[x][0], S.c[x][1], S.c[x][2]);
			lattice.rect(p.x * size, p.y * size, size, size);
			if (S.FRAMED) {
				lattice.fill(S.frame[0], S.frame[1], S.frame[2]);
				if (!p.bond(0))
					lattice.rect(p.x * size, p.y * size, 1, size);
				if (!p.bond(1))
					lattice.rect((p.x + 1) * size, p.y * size, -1, size);
				if (Lattice.D > 1) {
					if (!p.bond(2))
						lattice.rect(p.x * size, p.y * size, size, 1);
					if (!p.bond(3))
						lattice.rect(p.x * size, (p.y + 1) * size, size, -1);
				}
			}
			if (S.FRAMED && recursive)
				for (Point n : p.near) {
					drawPoint(n, false);
				}
		}

		p.drawn();
	}

	private void drawBonds(Point p, boolean recursive) {
		drawBondColor(p, 0);
		lattice.rect((p.x - 0.25F) * size, (p.y + 0.45F) * size, size / 2,
				size / 10);
		drawBondColor(p, 1);
		lattice.rect((p.x + 0.75F) * size, (p.y + 0.45F) * size, size / 2,
				size / 10);
		if (Lattice.D > 1) {
			drawBondColor(p, 2);
			lattice.rect((p.x + 0.45F) * size, (p.y - .25F) * size, size / 10,
					size / 2);
			drawBondColor(p, 3);
			lattice.rect((p.x + 0.45F) * size, (p.y + 0.75F) * size, size / 10,
					size / 2);
		}

		if (recursive) {
			if (p.x == 0)
				drawBonds(p.near[0], false);
			else if (p.x == L.size[0] - 1)
				drawBonds(p.near[1], false);

			if (Lattice.D > 1) {
				if (p.y == 0)
					drawBonds(p.near[2], false);
				else if (p.y == L.size[1] - 1)
					drawBonds(p.near[3], false);
			}
		}
	}

	private void drawBondColor(Point p, int i) {
		if (p.bond(i))
			lattice.fill(S.bon[0], S.bon[1], S.bon[2]);
		else
			lattice.fill(S.boff[0], S.boff[1], S.boff[2]);
	}

	private void drawNumber(Point p) {
		lattice.fill(0, 0, 0);
		lattice.text(p.getSV(), p.x * size + size / 3, p.y * size + size * 0.6F);
	}

	static public void main(String[] passedArgs) {
		String[] appletArgs = new String[] { "Render.IsingRender" };
		if (passedArgs != null) {
			PApplet.main(concat(appletArgs, passedArgs));
		} else {
			PApplet.main(appletArgs);
		}
	}
}