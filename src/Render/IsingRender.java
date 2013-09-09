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
	private int speed = 1; // N*speed flips/render
	private int N; // (2NxN Lattice)
	private int size;

	// System
	private PGraphics lattice, info; // chart_bg,chart
	private ControlP5 cp5;
	private boolean play = false;

	Lattice L;
	private int tab = 0;
	private long calced = 0;

	private long start;

	// private XYChart lineChart;
	public void setup() {
		cp5 = new ControlP5(this);
		int b = 0;
		cp5.addBang("play/pause").setPosition(25 + 100 * b++, 630)
				.setSize(99, 20).getCaptionLabel()
				.align(ControlP5.CENTER, ControlP5.CENTER);
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
		frameRate(60);
		size(650, 650);
		lattice = createGraphics(600, 600);
		info = createGraphics(600, 20);
		background(0);
		setup(3);
	}

	private void setup(int a) {
		// System
		setupLattice();
		size = 600 / N;
		speed = 1;
	}

	private void setupLattice() {
		// Physics
		Point.breite = 14;
		Point.poren = false;

		N = 15;
		seed = 1;
		J = 1;
		h = 0;
		kT = 5.999;
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
			sweep(L.N);
	}

	public void controlEvent(ControlEvent event) {
		if (event.isFrom("play/pause")) {
			Pause();
		} else if (event.isFrom("reset")) {
			Stop();
			setupLattice();
		} else if (!play && event.isFrom("sweep")) {
			sweep(L.N);
		} else if (!play && event.isFrom("flip")) {
			sweep(1);
		} else if (event.isFrom("bonds")) {
			S_System.BONDS = !S_System.BONDS;
			lattice.fill(0);
			lattice.rect(0, 0, 600, 600);
			drawLattice(true);
		} else if (!S_System.BONDS && event.isFrom("framed")) {
			S_System.framed = !S_System.framed;
			lattice.fill(0);
			lattice.rect(0, 0, 600, 600);
			drawLattice(true);
		}
		tab = 0;
	}

	private void sweep(int c) {
		start = System.currentTimeMillis();
		for (int i = 0; i < c * speed; i++) {
			L.tryFlip(1);
			calced++;
		}
		printFlips(0.005);
		tab = 0;
	}

	private boolean printFlips(double x) {
		if (Math.random() < x) {
			System.out.println("Time/" + (L.N * speed) + " Flips: "
					+ (System.currentTimeMillis() - start) + "ms");
			int log10 = (int) Math.floor(Math.log10(calced));
			System.out.println("Proposed Flips: "
					+ S_System.df.format(calced / Math.pow(10, log10)) + "x10^"
					+ log10);
			return true;
		} else
			return false;
	}

	private void drawInfo() {
		info.beginDraw();
		info.noStroke();
		info.fill(0);
		info.textSize(18);
		info.background(0);
		info.fill(color(180, 180, 180));
		int log10 = (int) Math.floor(Math.log10(Math.abs(Hamilton.getE())));
		String energy = S_System.df.format(Hamilton.getE()
				/ Math.pow(10, log10))
				+ "x10^" + log10;
		// 123412521 instead of 1.2*10^x
		energy = "" + Hamilton.getE();
		String s = "Size: " + L.size[0] + "x" + L.size[1] + " Energy:  "
				+ energy + "Mag: " + Hamilton.E_m + " Seed: " + seed
				+ " Speed: " + speed;
		info.text(s, 25, 20);
		info.text("", 206, 20);
		info.endDraw();
	}

	private void drawLattice(boolean drawAll) {
		lattice.beginDraw();
		lattice.noStroke();
		for (Point p : L.sites)
			if (drawAll || p.getRedraw() || S_System.framed)
				drawPoint(p);
		lattice.endDraw();
	}

	private void drawPoint(Point p) {
		p.drawn();
		int x = p.getV() + 1;
		if (S_System.BONDS) {
			drawBonds(p);
			lattice.fill(S_System.c[x][0], S_System.c[x][1], S_System.c[x][2]);
			lattice.rect(p.x * size + size / 4, p.y * size + size / 4,
					size / 2, size / 2);
		} else {
			lattice.fill(S_System.c[x][0], S_System.c[x][1], S_System.c[x][2]);
			lattice.rect(p.x * size, p.y * size, size, size);
			if (S_System.framed && p.is(-1)) {
				lattice.fill(S_System.frame[0], S_System.frame[1],
						S_System.frame[2]);
				if (!p.bond(0))
					lattice.rect(p.x * size, p.y * size, size, 1);
				if (!p.bond(1))
					lattice.rect((p.x + 1) * size, p.y * size, -1, size);
				if (!p.bond(2))
					lattice.rect(p.x * size, (p.y + 1) * size, size, -1);
				if (!p.bond(3))
					lattice.rect(p.x * size, p.y * size, 1, size);
			}
		}
	}

	private void drawBonds(Point p) {
		if (p.y == 0) {
			drawBondColor(p, 0);
			lattice.rect((p.x + 0.45F) * size, 0.5F * size, size / 10,
					-size / 2);
		}
		if (p.x == 0) {
			drawBondColor(p, 3);
			lattice.rect(0.5F * size, (p.y + 0.45F) * size, -size / 2,
					size / 10);
		}
		drawBondColor(p, 1);
		lattice.rect((p.x + 0.75F) * size, (p.y + 0.45F) * size, size / 2,
				size / 10);

		drawBondColor(p, 2);
		lattice.rect((p.x + 0.45F) * size, (p.y + 0.75F) * size, size / 10,
				size / 2);
	}

	private void drawBondColor(Point p, int i) {
		if (p.bond(i))
			lattice.fill(S_System.bon[0], S_System.bon[1], S_System.bon[2]);
		else
			lattice.fill(S_System.boff[0], S_System.boff[1], S_System.boff[2]);
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