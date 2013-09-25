package Render;

import java.text.DecimalFormat;
import Data.*;
import Model.*;

import controlP5.ControlP5;

public abstract class S {
	// Text Output
	public static final DecimalFormat df = new DecimalFormat("0.00");
	// Colors
	private static final float[] up = new float[] { 20, 200, 240 };
	private static final float[] down = new float[] { 30, 50, 220 };
	private static final float[] wall = new float[] { 55, 55, 55 };

	public static final float[][] c = new float[][] { down, wall, up };
	public static final float[] frame = new float[] { 255, 0, 0 };

	private final static float h = 250, d = 25; // m = 175;
	public static final float[] bon = new float[] { h, h, h };
	public static final float[] boff = new float[] { d, d, d };

	public static EventBuffer buffer = new EventBuffer(50);
	public static boolean BONDS = true;
	public static boolean NUMBERS = false;
	public static boolean FRAMED = false; // red frames
	public static boolean LOG = true;

	static int speed = 1;

	public static ControlP5 cp5; // buttons

	private static int y = 0;
	private static final int y0 = 5;
	private static final int x0 = 755;

	static void setup(IsingRender A) {
		S.cp5 = new ControlP5(A);
		S.cp5.addBang("play/pause").setPosition(x0, y0).setSize(74, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("reset").setPosition(x0 + 75, y0 + 21 * y++)
				.setSize(49, 20).getCaptionLabel()
				.align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("log").setPosition(x0 + 125, y0).setSize(24, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);

		S.cp5.addBang("update").setPosition(x0, y0 + 21 * y).setSize(49, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("sweep").setPosition(x0 + 50, y0 + 21 * y)
				.setSize(49, 20).getCaptionLabel()
				.align(ControlP5.CENTER, ControlP5.CENTER);
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
		Field(A, "J");
		Field(A, "h");
		Field(A, "kT");
	}

	public static void Field(IsingRender A, String n) {
		S.cp5.addTextfield(n + "x").setPosition(x0 + 1, y0 + 21 * ++y)
				.setSize(97, 20).setAutoClear(false)
				.setValue("" + Hamiltonian.kT()).setInputFilter(0)
				.setFont(A.createFont("arial", 15)).getCaptionLabel()
				.align(ControlP5.RIGHT, ControlP5.CENTER).set(n);
		S.cp5.addBang(n + '+').setPosition(x0 + 100, y0 + 21 * y)
				.setSize(24, 20).getCaptionLabel().set("+")
				.align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang(n + '-').setPosition(x0 + 125, y0 + 21 * y)
				.setSize(24, 20).getCaptionLabel().set("-")
				.align(ControlP5.CENTER, ControlP5.CENTER);
	}
}
