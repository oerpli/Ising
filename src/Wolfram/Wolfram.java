package Wolfram;

import processing.core.*;
//import processing.data.*;
//import processing.event.*;
//import processing.opengl.*;
//import org.gicentre.utils.stat.*;

import Ising.Lattice;
import Ising.Point;
//import controlP5.*;

//import java.text.DecimalFormat;

public class Wolfram extends PApplet {
	private static final long serialVersionUID = -1664637672574501774L;
	// 1 -0.8 2.269*E/8
	private static final int N = 150; // (2NxN Lattice)
	private final double seed = 0.99;
	private final double E = 1.25; // Energy - neighbor0
	private final double J = -0.4; // Field - sum
	private final double Beta = E / 5;
	private final boolean limit = false;
	public final int width = 1350;
	private int cell_size = 600 / N;
	private final int redrawFrequency = 10;
	private int draw = 0;
	Lattice L = new Lattice(2 * N, N, seed, E, J, Beta);
	// private int sizeValue;
	private int tab = 0;
	// private static DecimalFormat df = new DecimalFormat(",##0.00%");

	// private XYChart lineChart;
	private PGraphics lattice, chart, chart_background, info;

	public void setup() {
		System.out.println(cell_size);

		size(1250, 650);
		frameRate(60);
		lattice = createGraphics(1200, 600);
		chart = createGraphics(700, 450);
		info = createGraphics(1200, 20);
		chart_background = createGraphics(chart.width + 20, chart.height + 20);

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
			if (Math.random() < Math.exp(-5))
				System.out.println(L.calced);
			break;
		}
		recalc();
	}

	private void recalc() {
		for (int i = 0; i < L.size[2]; i++) {
			L.tryFlip(1);
		}
		tab = 0;
	}

	private void drawInfo() {
		info.beginDraw();
		info.noStroke();
		info.fill(0);
		info.textSize(18);
		info.background(0);
		info.fill(color(180, 180, 180));
		info.text("Lattice size: " + L.size[0] + " x " + L.size[1]
				+ " Hamiltonian:  " + (int) L.getHamiltonian() + " Seed: "
				+ seed + " Plus: " + L.plus, 25, 20);
		info.text("", 206, 20);
		info.endDraw();
	}

	private void drawLattice(int size) {
		lattice.beginDraw();
		for (Point p : L.sites)
			if (p.getRedraw())
				drawPoint(p, size);

		draw = (draw + 1) % redrawFrequency;
		lattice.endDraw();
	}

	private void drawPoint(Point p, int size) {
		p.drawn();
		// lattice.strokeWeight(limit ? 1 : 0);
		lattice.noStroke();
		int a = 0;
		if (p.is(1)) {
			a = 255;
			lattice.fill(a, a + 15, a + 15);
		} else if (p.is(-1)) {
			a = 80;
			lattice.fill(255, a + 15, a + 15);
		} else {
			a = 0;
			lattice.fill(a, a + 55, a + 15);
		}
		lattice.rect(p.x * size, p.y * size, size, size);
		if (limit && p.is(1)) {
			lattice.fill(255, 0, 0);
			lattice.strokeWeight(0);
			if (!p.near[0].is(p))
				lattice.rect(p.x * size + 1, p.y * size + 1, size, 1);
			if (!p.near[1].is(p))
				lattice.rect((p.x + 1) * size - 1, p.y * size + 1, 1, size);
			if (!p.near[2].is(p))
				lattice.rect(p.x * size + 1, (p.y + 1) * size - 1, size, 1);
			if (!p.near[3].is(p))
				lattice.rect(p.x * size + 1, p.y * size + 1, 1, size);
		}

	}

	static public void main(String[] passedArgs) {
		String[] appletArgs = new String[] { "wolfram" };
		if (passedArgs != null) {
			PApplet.main(concat(appletArgs, passedArgs));
		} else {
			PApplet.main(appletArgs);
		}
	}
}