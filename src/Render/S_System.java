package Render;

import java.text.DecimalFormat;
import java.util.Scanner;

public abstract class S_System {
	// Text Output
	public static final DecimalFormat df = new DecimalFormat("0.00");

	// Colors

	private static final float[] up = new float[] { 20, 200, 240 };
	private static final float[] down = new float[] { 30, 50, 220 };
	private static final float[] wall = new float[] { 55, 55, 55 };

	public static final float[][] c = new float[][] { down, wall, up };
	public static final float[] frame = new float[] { 255, 0, 0 };

	private final static float h = 250;
	// private final static float m = 175;
	private final static float d = 25;
	public static final float[] bon = new float[] { h, h, h };
	public static final float[] boff = new float[] { d, d, d };

	public static boolean BONDS = true;
	public static boolean NUMBERS = false;
	public static boolean FRAMED = false; // red frames
	public static boolean LOG = true;

	// Some input/output stuff //TODO
	public static Scanner sc = new Scanner(System.in);

}
