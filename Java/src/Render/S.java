package Render;

import java.text.DecimalFormat;

import processing.core.PApplet;
import Data.Log;
import Data.Plotter;
import Dynamics.Algorithm;
import Model.Hamiltonian;

import controlP5.ControlEvent;
import controlP5.ControlP5;
import controlP5.Textfield;

public abstract class S {
	// Text Output
	public static final DecimalFormat df = new DecimalFormat("0.000");
	public static final DecimalFormat df2 = new DecimalFormat("0.00");

	// Colors
	private static final float[] up = new float[] { 20, 200, 240 };
	private static final float[] down = new float[] { 30, 50, 220 };
	private static final float[] wall = new float[] { 55, 55, 55 };

	public static final float[][] c = new float[][] { down, wall, up };
	public static final float[] frame = new float[] { 255, 0, 0 };

	private final static float h = 250, d = 25; // m = 175;
	public static final float[] bon = new float[] { h, h, h };
	public static final float[] boff = new float[] { d, d, d };

	public static boolean BONDS = true;
	public static boolean NUMBERS = false;
	public static boolean FRAMED = false; // red frames
	public static boolean LOG = true;

	static int speed = 1;

	public static ControlP5 cp5; // buttons

	private static int y = 0;
	private static final int y0 = 5;
	private static final int x0 = 755;
	public static IsingRender R;

	static void setup(IsingRender A) {
		R = A;
		S.cp5 = new ControlP5(A);
		S.cp5.addBang("play/pause").setPosition(x0, y0).setSize(74, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("reset").setPosition(x0 + 75, y0 + 21 * y++)
				.setSize(49, 20).getCaptionLabel()
				.align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("log").setPosition(x0 + 125, y0).setSize(24, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);

		S.cp5.addBang("sweep").setPosition(x0, y0 + 21 * y).setSize(49, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("flip").setPosition(x0 + 50, y0 + 21 * y).setSize(49, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("+").setPosition(x0 + 100, y0 + 21 * y).setSize(24, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("-").setPosition(x0 + 125, y0 + 21 * y++).setSize(24, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);

		S.cp5.addBang("bonds").setPosition(x0, y0 + 21 * y).setSize(49, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("framed").setPosition(x0 + 50, y0 + 21 * y)
				.setSize(49, 20).getCaptionLabel()
				.align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("energy/J").setPosition(x0 + 100, y0 + 21 * y++)
				.setSize(49, 20).getCaptionLabel()
				.align(ControlP5.CENTER, ControlP5.CENTER);
		Field("J", "" + Hamiltonian.J(), 0);
		Field("h", "" + Hamiltonian.h(), 0);
		Field("kT", "" + Hamiltonian.kT(), 0);
		y++;
		Field("N", "" + IsingRender.N1, 0);
		y++;
		S.cp5.addBang("SF").setPosition(x0, y0 + 21 * ++y).setSize(49, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("KS").setPosition(x0 + 50, y0 + 21 * y).setSize(49, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("M").setPosition(x0 + 100, y0 + 21 * y).setSize(24, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("G").setPosition(x0 + 125, y0 + 21 * y++).setSize(24, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("SW").setPosition(x0, y0 + 21 * y++).setSize(49, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("plot").setPosition(x0, y0 + 21 * ++y).setSize(49, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("reset mean").setPosition(x0 + 50, y0 + 21 * y)
				.setSize(49, 20).getCaptionLabel()
				.align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("hist").setPosition(x0 + 100, y0 + 21 * y)
				.setSize(49, 20).getCaptionLabel()
				.align(ControlP5.CENTER, ControlP5.CENTER);
	}

	public static void Field(String n, String v, int filter) {
		S.cp5.addTextfield(n + "x").setPosition(x0 + 1, y0 + 21 * ++y)
				.setSize(97, 20).setAutoClear(false).setValue(v)
				.setInputFilter(filter).setFont(R.createFont("arial", 15))
				.getCaptionLabel().align(ControlP5.RIGHT, ControlP5.CENTER)
				.set(n);
		S.cp5.addBang(n + '+').setPosition(x0 + 100, y0 + 21 * y)
				.setSize(24, 20).getCaptionLabel().set("+")
				.align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang(n + '-').setPosition(x0 + 125, y0 + 21 * y)
				.setSize(24, 20).getCaptionLabel().set("-")
				.align(ControlP5.CENTER, ControlP5.CENTER);
	}

	public static void controlEvent(ControlEvent event) {
		if (event.isFrom("play/pause")) {
			R.PlayPause();
		} else if (event.isFrom("reset")) {
			R.reset();
		} else if (!IsingRender.play && event.isFrom("sweep")) {
			R.sweep(1);
		} else if (!IsingRender.play && event.isFrom("flip")) {
			Algorithm.flip();
		} else if (event.isFrom("bonds")) {
			S.BONDS = !S.BONDS;
			redraw();
		} else if (!S.BONDS && event.isFrom("framed")) {
			S.FRAMED = !S.FRAMED;
			redraw();
		} else if (event.isFrom("energy/J")) {
			S.NUMBERS = !S.NUMBERS;
			redraw();
		} else if (event.isFrom("+")) {
			S.speed *= 2;
		} else if (event.isFrom("-") && S.speed > 1) {
			S.speed /= 2;
		} else if (event.isFrom("log")) {
			Log.init(R, R.L.size[0], R.L.size[1]);
		} else if (event.isFrom("Jx")) {
			R.J = PApplet
					.parseFloat(S.cp5.get(Textfield.class, "Jx").getText());
			R.updateHamilton();
		} else if (event.isFrom("hx")) {
			R.h = PApplet
					.parseFloat(S.cp5.get(Textfield.class, "hx").getText());
			R.updateHamilton();
		} else if (event.isFrom("kTx")) {
			R.kT = Math.max(0.0001F, PApplet.parseFloat(S.cp5.get(
					Textfield.class, "kTx").getText()));
			R.updateKT();
		} else if (event.isFrom("J+")) {
			R.J += 0.1;
			R.updateHamilton();
		} else if (event.isFrom("J-")) {
			R.J -= 0.1;
			R.updateHamilton();
		} else if (event.isFrom("h+")) {
			R.h += 0.1;
			R.updateHamilton();
		} else if (event.isFrom("h-")) {
			R.h -= 0.1;
			R.updateHamilton();
		} else if (event.isFrom("kT+")) {
			R.kT += 0.1;
			R.updateKT();
		} else if (event.isFrom("kT-")) {
			R.kT -= 0.1;
			R.updateKT();
		} else if (event.isFrom("SF")) {
			Algorithm.u(0);
		} else if (event.isFrom("KS")) {
			Algorithm.u(1);
		} else if (event.isFrom("SW")) {
			Algorithm.u(2);
		} else if (event.isFrom("G")) {
			Algorithm.a(1);
		} else if (event.isFrom("M")) {
			Algorithm.a(0);
		} else if (event.isFrom("reset mean")) {
			Plotter.resetM();
		} else if (event.isFrom("plot")) {
			Log.plot = !Log.plot;
			Log.p.toggle(Log.plot);
		} else if (event.isFrom("Nx")) {
			IsingRender.N1 = Math.max(2, PApplet.parseInt(S.cp5.get(
					Textfield.class, "Nx").getText()));
			R.setupLattice();
			redraw();
			S.cp5.get(Textfield.class, "Nx").setValue("" + IsingRender.N1);
		} else if (event.isFrom("N+")) {
			IsingRender.N1++;
			R.setupLattice();
			redraw();
			S.cp5.get(Textfield.class, "Nx").setValue("" + IsingRender.N1);
		} else if (event.isFrom("N-") && IsingRender.N1 > 2) {
			IsingRender.N1--;
			R.setupLattice();
			redraw();
			S.cp5.get(Textfield.class, "Nx").setValue("" + IsingRender.N1);
		} else if (event.isFrom("hist")) {
			Plotter.hi = !Plotter.hi;
		}
	}

	private static void redraw() {
		R.lattice.fill(0);
		R.lattice.rect(0, 0, 700, 700);
		R.drawLattice(true);
	}
}
