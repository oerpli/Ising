package Render;

import java.io.File;
import java.io.FileWriter;
import java.io.IOException;

import processing.core.*;
import Ising.Hamilton;
import Ising.Lattice;
import Ising.Point;
import Data.*;
import controlP5.*;

public class IsingRender extends PApplet {
	private static final long serialVersionUID = -1664637672574501774L;

	// Physics:
	// private float J; // Coupling Constant
	// private float h; // Field
	// private float kT;// Temperature
	private double seed;// Seed

	// Renderparameters
	private int N, N2;
	private int size;

	// System
	private PGraphics lattice, info; // chart_bg,chart
	private boolean play = false;

	Lattice L;
	private int tab = 0;
	// private long calced = 0;

	private long time;
	private int c;
	private float sign = 0;

	private long sweeps;

	// private XYChart lineChart;
	public void setup() {
		S.setup(this);
		frameRate(60);
		size(650, 670);
		lattice = createGraphics(600, 600);
		info = createGraphics(600, 20);
		background(0);
		setupLattice();
		size = 600 / N;
		S.speed = 1;
		setupFields();
		sign = Math.signum(Hamilton.E_m);
	}

	private void setupFields() {
		S.cp5.addTextfield("J").setPosition(75 + 50 * S.b++, 630)
				.setSize(47, 20).setAutoClear(false).setValue("" + Hamilton.J)
				.setInputFilter(0).setFont(createFont("arial", 15));
		S.cp5.addTextfield("h").setPosition(75 + 50 * S.b++, 630)
				.setSize(47, 20).setAutoClear(false).setValue("" + Hamilton.h)
				.setInputFilter(0).setFont(createFont("arial", 15));
		S.cp5.addTextfield("kT").setPosition(75 + 50 * S.b++, 630)
				.setSize(49, 20).setAutoClear(false).setValue("" + Hamilton.kT)
				.setInputFilter(0).setFont(createFont("arial", 15));
	}

	private void setupLattice() {
		// Physics
		Point.breite = 14;
		Point.poren = false;
		N = 3;
		N2 = N;
		seed = 0.5;
		float J = 1;
		float h = 0;
		float kT = 2.70F;
		L = new Lattice(N, N2, 1, seed, J, h, kT);
		S.file = new File("LOG_" + N + "x" + N2 + J + "-" + kT + "_"
				+ System.currentTimeMillis() + ".txt");
		try {
			S.writer = new FileWriter(S.file, true);
			S.writer.write("t E M\n");
		} catch (IOException e) {
			e.printStackTrace();
		}
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
			sweep(S.speed);
	}

	public void controlEvent(ControlEvent event) {
		if (event.isFrom("play/pause")) {
			Pause();
		} else if (event.isFrom("reset")) {
			S.speed = 1;
			sweeps = 0;
			// calced = 0;
			Stop();
			setupLattice();
		} else if (!play && event.isFrom("sweep")) {
			sweep(1);
		} else if (!play && event.isFrom("flip")) {
			flip(1);
		} else if (event.isFrom("bonds")) {
			S.BONDS = !S.BONDS;
			lattice.fill(0);
			lattice.rect(0, 0, 600, 600);
			drawLattice(true);
		} else if (!S.BONDS && event.isFrom("framed")) {
			S.FRAMED = !S.FRAMED;
			lattice.fill(0);
			lattice.rect(0, 0, 600, 600);
			drawLattice(true);
		} else if (event.isFrom("energy/J")) {
			S.NUMBERS = !S.NUMBERS;
			lattice.fill(0);
			lattice.rect(0, 0, 600, 600);
			drawLattice(true);
		} else if (event.isFrom("+")) {
			S.speed *= 2;
		} else if (event.isFrom("-") && S.speed > 1) {
			S.speed /= 2;
		} else if (event.isFrom("log")) {
			newFile();
		} else if (event.isFrom("J")) {
			Hamilton.setJ(parseFloat(S.cp5.get(Textfield.class, "J").getText()));
			System.out.println(Hamilton.out());
		} else if (event.isFrom("h")) {
			Hamilton.setH(parseFloat(S.cp5.get(Textfield.class, "h").getText()));
			System.out.println(Hamilton.out());
		} else if (event.isFrom("kT")) {
			Hamilton.setKT(Math.max(0.0001F,
					parseFloat(S.cp5.get(Textfield.class, "kT").getText())));
			System.out.println(Hamilton.out());
		}
		tab = 0;
	}

	private void newFile() {
		try {
			S.writer.flush();
			String name = "LOG_" + N + "x" + N2 + Hamilton.J + "-"
					+ Hamilton.kT + "_" + System.currentTimeMillis() + ".txt";
			S.file = new File(name);
			System.out.println("New Logfile:" + name);
			S.writer = new FileWriter(S.file, true);
			S.writer.write(N + "x" + N2 + '\n');
			S.writer.write("J=" + Hamilton.J + ";h=" + Hamilton.h + ";kT="
					+ Hamilton.kT + '\n');
			S.writer.write("t E M\n");
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	private void sweep(int n) {
		time = -System.currentTimeMillis();
		this.c = L.N * n;
		for (int i = 0; i < n; i++) {
			flip(L.N);
			sweeps += 1;
			log();
		}
		time += System.currentTimeMillis();

	}

	@SuppressWarnings("unchecked")
	private void log() {
		try {
			S.writer.write(S.buffer.add(new DataSet(sweeps, Hamilton.E_nn,
					Hamilton.E_m)));
			checkTransition();
			if (sweeps % 1024 == 0) {
				S.writer.write(new DataSet(sweeps, Hamilton.E_nn, Hamilton.E_m)
						.toString());
			}
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	private void checkTransition() {
		if (Math.signum(Hamilton.E_m) == -sign) {
			System.out.println("trans");
			play = false;
			sign = -sign;
			S.buffer.event();
		}
	}

	private void flip(int n) {
		for (int i = 0; i < n; i++) {
			L.update();
			// calced++;
		}
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
		s += "Speed: " + S.speed + ", " + stringFlips() + ", ";
		s += "E: " + energy + ", E_m: " + Hamilton.E_m;
		info.text(s, 25, 17);
		// info.text("", 206, 20);
		info.endDraw();
	}

	private String scientific(double x) {
		int log10 = (int) Math.floor(Math.log10(Math.abs(x)));
		String out = S.df.format(x / Math.pow(10, log10)) + "x10^" + log10;
		return out;
	}

	private void drawLattice(boolean drawAll) {
		lattice.beginDraw();
		lattice.noStroke();
		lattice.textSize(16);
		for (Point p : L.sites)
			// list of changed sites
			if (p.getRedraw() || drawAll)
				drawPoint(p, true);
		lattice.endDraw();
	}

	private void drawPoint(Point p, boolean recursive) {
		p.drawn();
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
		}
		if (S.FRAMED && recursive)
			for (Point n : p.near)
				drawPoint(n, false);
		if (S.NUMBERS)
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
			lattice.fill(S.bon[0], S.bon[1], S.bon[2]);
		else
			lattice.fill(S.boff[0], S.boff[1], S.boff[2]);
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