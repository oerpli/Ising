package Render;

import java.io.File;
import java.io.FileWriter;
import java.text.DecimalFormat;
import java.util.ArrayList;
import java.util.Scanner;
import Data.*;

import controlP5.ControlP5;

public abstract class S {
	// Text Output
	static final DecimalFormat df = new DecimalFormat("0.00");
	// Colors
	private static final float[] up = new float[] { 20, 200, 240 };
	private static final float[] down = new float[] { 30, 50, 220 };
	private static final float[] wall = new float[] { 55, 55, 55 };

	static final float[][] c = new float[][] { down, wall, up };
	static final float[] frame = new float[] { 255, 0, 0 };

	private final static float h = 250, d = 25; // m = 175;
	static final float[] bon = new float[] { h, h, h };
	static final float[] boff = new float[] { d, d, d };

	static EventBuffer buffer = new EventBuffer(50);
	static ArrayList<Integer> countdown = new ArrayList<Integer>();
	static boolean BONDS = false;
	static boolean NUMBERS = false;
	static boolean FRAMED = false; // red frames
	static boolean LOG = true;

	static int speed = 1;
	static int b = 0;

	// Some input/output stuff //TODO
	static Scanner sc = new Scanner(System.in);
	static FileWriter writer;
	static File file;
	static ControlP5 cp5;

	static void setup(IsingRender r) {
		S.cp5 = new ControlP5(r);
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
	}
}
