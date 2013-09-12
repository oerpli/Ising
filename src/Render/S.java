package Render;

import java.text.DecimalFormat;
import java.util.Scanner;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;

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

	static boolean BONDS = false;
	static boolean NUMBERS = false;
	static boolean FRAMED = false; // red frames
	static boolean LOG = true;

	static int speed = 1;

	// Some input/output stuff //TODO
	static Scanner sc = new Scanner(System.in);
	static FileWriter writer;
	static File file;
	static ControlP5 cp5;

}
