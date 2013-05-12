package Render;

import processing.core.*;
//import processing.data.*;
//import processing.event.*;
//import processing.opengl.*;
//import org.gicentre.utils.stat.*;

import Ising.Lattice;
import Ising.Point;
//import controlP5.*;

//import java.text.DecimalFormat;

public class IsingRender extends PApplet {
	private long calced = 0;
	private static final long serialVersionUID = -1664637672574501774L;

	// Physics:
	// E = 1 J = -0.8 kT = 8/2.269
	// SAVE E= 1.25, J= -0.2, kT = 0.25
	// E = 0.44, J = -0.05, kT = 1 - AJ Page, RP Sear
	private final double E = 0.35; // Energy - neighbor0
	private final double J = -0.05; // Field - sum
	private final double kT = 1;
	
	// Renderparameters
	private final boolean limit = false; // red frames
	private int speed = 1; // N*speed flips/render
	private static final int N = 100; // (2NxN Lattice)
	private final double seed = 0.99;

	// Stuff
	public final int width = 1350;
	private int cell_size = 600 / N;
	Lattice L = new Lattice(2 * N, N, seed, E, J, 1 / kT);
	private int tab = 0;
	// private static DecimalFormat df = new DecimalFormat(",##0.00%");
	// private XYChart lineChart;
	private PGraphics lattice, info; // chart_bg,chart

	public void setup() {
		System.out.println(cell_size);
		size(1250, 650);
		frameRate(60);
		lattice = createGraphics(1200, 600);
		info = createGraphics(1200, 20);
		// chart_bg = createGraphics(chart.width + 20, chart.height + 20);
		// chart = createGraphics(700, 450);

		background(0);
		PFont font = createFont("Futura", 100, true);
		textFont(font);

	}

	// private void chart(int xData, float yData) {
	// // background(0);
	// chart.beginDraw();
	// if (0 == (xData % chart.width))
	// chart.background(0);
	// chart.strokeWeight(2);
	// chart.line(0, chart.height, chart.width, chart.height);
	// chart.line(0, chart.height, 0, 0);
	// chart.stroke(255, 255, 255);
	// chart.strokeWeight(1);
	// chart.point(xData % chart.width, chart.height - yData * chart.height);
	// chart.endDraw();
	// }

	// private void chart_background(int xData) {
	// chart_background.beginDraw();
	// if (0 == (xData) % (chart.width))
	// chart_background.background(0);
	// chart_background.textSize(10);
	// if (0 == (xData) % (chart.width)) {
	// for (int i = 1; i <= 10; i++) {
	// if (i != 10) {
	// chart_background.text(i + "0", 0, chart.height
	// - chart.height / 10 * i);
	// }
	// // chart_background.stroke(cp5.get(Textfield.class, "rule")
	// // .getColor().getBackground());
	// chart_background.line(20, chart.height - chart.height / 10 * i,
	// chart_background.width, chart.height - chart.height
	// / 10 * i);
	// }
	// }
	// if (0 == xData % 50)
	// chart_background.text(xData, (xData) % (chart.width) + 20,
	// chart_background.height);
	// chart_background.endDraw();
	// }

	public void draw() {
		switch (tab) {
		case 0:
			drawLattice(cell_size);
			image(lattice, 25, 25);
			drawInfo();
			image(info, 0, 0);
			tab = 2;
			if (Math.random() < 0.03) {
				int log10 = (int) Math.floor(Math.log10(calced));
				System.out.println("Proposed Flips: "
						+ (calced / Math.pow(10, log10)) + "x10^" + log10);
			}
			break;
		}
		recalc();
	}

	private void recalc() {
		long start = System.currentTimeMillis();
		for (int i = 0; i < L.size[2] * speed; i++) {
			L.tryFlip(1);
			calced++;
		}
		if (Math.random() < 0.05)
			System.out.println("Time/" + (L.size[2] * speed) + " Flips: "
					+ (System.currentTimeMillis() - start) + "ms");
		tab = 0;
	}

	private void drawInfo() {
		info.beginDraw();
		info.noStroke();
		info.fill(0);
		info.textSize(18);
		info.background(0);
		info.fill(color(180, 180, 180));
		String s = "Lattice size: " + L.size[0] + " x " + L.size[1]
				+ " Hamiltonian:  " + (int) L.getHamiltonian() + " Seed: "
				+ seed + " Plus: " + L.plus + " Speed: " + speed;
		info.text(s, 25, 20);
		info.text("", 206, 20);
		info.endDraw();
	}

	private void drawLattice(int size) {
		lattice.beginDraw();
		lattice.noStroke();

		for (Point p : L.sites)
			if (p.getRedraw() || limit)
				drawPoint(p, size);
		lattice.endDraw();
	}

	private void drawPoint(Point p, int size) {
		p.drawn();
		int a = 0;
		if (p.is(1)) {
			a = 255;
			// lattice.fill(a, a + 15, a + 15);
			lattice.fill(a, a, a);
		} else if (p.is(-1)) {
			a = 30;
			// lattice.fill(255, a + 15, a + 15);
			lattice.fill(a, a, a);
		} else {
			a = 80;
			lattice.fill(a, a, a);
		}
		lattice.rect(p.x * size, p.y * size, size, size);
		if (limit && p.is(-1)) {
			lattice.fill(255, 0, 0);
			if (!p.near[0].is(p))
				lattice.rect(p.x * size, p.y * size, size, 1);
			if (!p.near[1].is(p))
				lattice.rect((p.x + 1) * size, p.y * size, -1, size);
			if (!p.near[2].is(p))
				lattice.rect(p.x * size, (p.y + 1) * size, size, -1);
			if (!p.near[3].is(p))
				lattice.rect(p.x * size, p.y * size, 1, size);
		}

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