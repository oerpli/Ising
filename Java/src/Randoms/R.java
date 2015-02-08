package Randoms;

import java.util.Random;

public class R {
	private static Random r = new Random();

	public static int nextInt(int n) {
		return r.nextInt(n);
	}

	public static double nextDouble() {
		return r.nextDouble();
	}
}
