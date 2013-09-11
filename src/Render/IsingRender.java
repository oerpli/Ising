package Render;

import processing.core.*;
import Ising.Hamilton;
import Ising.Lattice;
import Ising.Point;
import controlP5.*;

public class IsingRender extends PApplet {
	private static final long serialVersionUID = -1664637672574501774L;

	// Physics:
	private double J; // Coupling Constant
	private double h; // Field
	private double kT;// Temperature
	private double seed;// Seed

	// Renderparameters
	private int speed = 1;
	private int N;
	private int size;

	// System
	private PGraphics lattice, info; // chart_bg,chart
	private ControlP5 cp5;
	private boolean play = false;

	Lattice L;
	private int tab = 0;
	private long calced = 0;

	private long time;
	private int c;

	private long sweeps;

	// private XYChart lineChart;
	public void setup() {
		cp5 = new ControlP5(this);
		int b = 0;
		cp5.addBang("play/pause").setPosition(25 + 100 * b++, 630)
				.setSize(74, 20).getCaptionLabel()
				.align(ControlP5.CENTER, ControlP5.CENTER);
		cp5.addBang("log").setPosition(50 + 50 * b, 630).setSize(24, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		cp5.addBang("+").setPosition(75 + 50 * b, 630).setSize(24, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		cp5.addBang("-").setPosition(100 + 50 * b++, 630).setSize(24, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		cp5.addBang("reset").setPosition(75 + 50 * b++, 630).setSize(49, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		cp5.addBang("sweep").setPosition(75 + 50 * b++, 630).setSize(49, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		cp5.addBang("flip").setPosition(75 + 50 * b++, 630).setSize(49, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		cp5.addBang("bonds").setPosition(75 + 50 * b++, 630).setSize(49, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		cp5.addBang("framed").setPosition(75 + 50 * b++, 630).setSize(49, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		cp5.addBang("energy/J").setPosition(75 + 50 * b++, 630).setSize(49, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);

		frameRate(60);
		size(650, 650);
		lattice = createGraphics(600, 600);
		info = createGraphics(600, 20);
		background(0);
		setupLattice();
		size = 600 / N;
		speed = 1;
	}

	private void setupLattice() {
		// Physics
		Point.breite = 14;
		Point.poren = false;
		N = 20;
		seed = 0.5;
		J = 1;
		h = 0;
		kT = 2.70;
		L = new Lattice(N, N, 1, seed, J, h, 1 / kT);
	}

	/**
	 * Stop simulation
	 */
	public void Stop() {
		play = false;
	}

	/**
	 * Toggle simulation.
	 */
	public void Pause() {
		play = !play;
	}

	public void draw() {
		switch (tab) {
		case 0:
			drawLattice(false);
			image(lattice, 25, 25);
			drawInfo();
			image(info, 0, 0);
			tab = 2;
			// printFlips(0.0001);
			break;
		}
		if (play)
			sweep(speed);
	}

	public void controlEvent(ControlEvent event) {
		if (event.isFrom("play/pause")) {
			Pause();
		} else if (event.isFrom("reset")) {
			speed = 1;
			sweeps = 0;
			calced = 0;
			Stop();
			setupLattice();
		} else if (!play && event.isFrom("sweep")) {
			sweep(1);
		} else if (!play && event.isFrom("flip")) {
			flip(1);
		} else if (event.isFrom("bonds")) {
			S_System.BONDS = !S_System.BONDS;
			lattice.fill(0);
			lattice.rect(0, 0, 600, 600);
			drawLattice(true);
		} else if (!S_System.BONDS && event.isFrom("framed")) {
			S_System.FRAMED = !S_System.FRAMED;
			lattice.fill(0);
			lattice.rect(0, 0, 600, 600);
			drawLattice(true);
		} else if (event.isFrom("energy/J")) {
			S_System.NUMBERS = !S_System.NUMBERS;
			lattice.fill(0);
			lattice.rect(0, 0, 600, 600);
			drawLattice(true);
		} else if (event.isFrom("+")) {
			speed *= 2;
		} else if (event.isFrom("-") && speed > 1) {
			speed /= 2;
		} else if (event.isFrom("log")) {
			S_System.LOG = !S_System.LOG;
			if (S_System.LOG)
				System.out.println("t E M");
		}
		tab = 0;
	}

	private void sweep(int n) {
		flip(L.N * n);
		sweeps += n;
		if (S_System.LOG)
			log();
	}

	private void log() {
		System.out.println(sweeps + " " + Hamilton.E_nn + " " + Hamilton.E_m);
	}

	private void flip(int c) {
		time = -System.currentTimeMillis();
		this.c = c;
		for (int i = 0; i < c; i++) {
			L.tryFlip(1);
			calced++;
		}
		time += System.currentTimeMillis();
		tab = 0;
	}

	private String stringFlips() {
		String out = "";
		if (play) {
			out += "ms/" + c + "F: " + time + " ";
			out += "(" + (c / (time + 1)) + "F/ms)";
			// System.out.println("Proposed Flips: " + scientific(calced));
		}
		return out;
	}

	private void drawInfo() {
		info.beginDraw();
		info.noStroke();
		info.fill(0);
		info.textSize(18);
		info.background(0);
		info.fill(color(255, 255, 255));
		// String energy = scientific(Hamilton.getE());
		String energy = "" + Hamilton.getE(); // 123412521 instead of 1.2*10^x
		String s = L.size[0] + "x" + L.size[1] + ", ";
		s += "Speed: " + speed + ", " + stringFlips() + ", ";
		s += "E: " + energy + ", E_m: " + Hamilton.E_m;
		info.text(s, 25, 17);
		// info.text("", 206, 20);
		info.endDraw();
	}

	private String scientific(double x) {
		int log10 = (int) Math.floor(Math.log10(Math.abs(x)));
		String out = S_System.df.format(x / Math.pow(10, log10)) + "x10^"
				+ log10;
		return out;
	}

	private void drawLattice(boolean drawAll) {
		lattice.beginDraw();
		lattice.noStroke();
		lattice.textSize(16);
		for (Point p : L.sites)
			if (p.getRedraw() || drawAll)
				drawPoint(p, true);
		lattice.endDraw();
	}

	private void drawPoint(Point p, boolean recursive) {
		p.drawn();
		int x = p.getV() + 1;// map v-values to color-array (-1,0,1 -> 0,1,2);
		if (S_System.BONDS) {
			lattice.fill(S_System.c[x][0], S_System.c[x][1], S_System.c[x][2]);
			lattice.rect(p.x * size + size / 4, p.y * size + size / 4,
					size / 2, size / 2);
			drawBonds(p, true);
		} else {
			lattice.fill(S_System.c[x][0], S_System.c[x][1], S_System.c[x][2]);
			lattice.rect(p.x * size, p.y * size, size, size);
			if (S_System.FRAMED) {
				lattice.fill(S_System.frame[0], S_System.frame[1],
						S_System.frame[2]);
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
		}
		if (S_System.FRAMED && recursive)
			for (Point n : p.near)
				drawPoint(n, false);
		if (S_System.NUMBERS)
			drawNumbers(p, recursive);
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
			lattice.fill(S_System.bon[0], S_System.bon[1], S_System.bon[2]);
		else
			lattice.fill(S_System.boff[0], S_System.boff[1], S_System.boff[2]);
	}

	private void drawNumbers(Point p, boolean recursive) {
		lattice.fill(0, 0, 0);
		lattice.text(p.getSV(), p.x * size + size / 3, p.y * size + size * 0.6F);
		if (recursive)
			for (Point x : p.near)
				drawPoint(x, false);
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