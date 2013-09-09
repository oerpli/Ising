package Render;

import java.text.DecimalFormat;
import java.util.Scanner;

public abstract class S_System {
	// Text Output
	public static final DecimalFormat df = new DecimalFormat("0.00");
	public final static boolean limit = false; // red frames

	// Colors
	private final static float h = 250;
	private final static float m = 175;
	private final static float d = 100;
	// public static final float[] up = new float[] { 0, 0, h };
	// public static final float[] down = new float[] { 0, 0, d };

	public static final float[] up = new float[] { 20, 200, 240 };
	public static final float[] down = new float[] { 30, 50, 220 };
	public static final float[] wall = new float[] { 55, 55, 55 };
	public static final float[] eframe = new float[] { h, 0, 0 };

	public static final float[] bon = new float[] { m, m, m };
	public static final float[] boff = new float[] { d, d, d };

	public static boolean BONDS = true;

	// Some input/output stuff //TODO
	public static Scanner sc = new Scanner(System.in);
}
