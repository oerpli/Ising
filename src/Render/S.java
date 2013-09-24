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

	static void setup(IsingRender A) {
		int b = 0;
		S.cp5 = new ControlP5(A);
		S.cp5.addBang("play/pause").setPosition(25 + 100 * b++, 630)
				.setSize(74, 20).getCaptionLabel()
				.align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("log").setPosition(50 + 50 * b, 630).setSize(24, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("+").setPosition(75 + 50 * b, 630).setSize(24, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("-").setPosition(100 + 50 * b++, 630).setSize(24, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("reset").setPosition(75 + 50 * b++, 630).setSize(49, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("sweep").setPosition(75 + 50 * b++, 630).setSize(49, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("flip").setPosition(75 + 50 * b++, 630).setSize(49, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("bonds").setPosition(75 + 50 * b++, 630).setSize(49, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("framed").setPosition(75 + 50 * b++, 630).setSize(49, 20)
				.getCaptionLabel().align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addBang("energy/J").setPosition(75 + 50 * b++, 630)
				.setSize(49, 20).getCaptionLabel()
				.align(ControlP5.CENTER, ControlP5.CENTER);
		S.cp5.addTextfield("J").setPosition(75 + 50 * b++, 630).setSize(47, 20)
				.setAutoClear(false).setValue("" + Hamilton.J)
				.setInputFilter(0).setFont(A.createFont("arial", 15))
				.getCaptionLabel().align(ControlP5.RIGHT, ControlP5.CENTER);
		S.cp5.addTextfield("h").setPosition(75 + 50 * b++, 630).setSize(47, 20)
				.setAutoClear(false).setValue("" + Hamilton.h)
				.setInputFilter(0).setFont(A.createFont("arial", 15))
				.getCaptionLabel().align(ControlP5.RIGHT, ControlP5.CENTER);
		S.cp5.addTextfield("kT").setPosition(75 + 50 * b++, 630)
				.setSize(49, 20).setAutoClear(false).setValue("" + Hamilton.kT)
				.setInputFilter(0).setFont(A.createFont("arial", 15))
				.getCaptionLabel().align(ControlP5.RIGHT, ControlP5.CENTER);
	}
}
