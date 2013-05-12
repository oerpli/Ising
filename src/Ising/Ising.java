package Ising;

import java.util.Arrays;
import java.lang.Math;

public class Ising {

	public static final int N = 100; // <3350 wg. Max Memory
	public static final double seed = 1;
	private final static double E = 1; // Energy - neighbor0
	private final static double J = -0.5; // Field - sum
	private final static double Beta = 2.269 * E / 3;

	/**
	 * @param args
	 */
	public static void main(String[] args) {
		// INITIALIZE
		long start = System.currentTimeMillis();
		Lattice x = new Lattice(N, N, seed, E, J, Beta);
		long end = System.currentTimeMillis();
		p("Initialize: " + (end - start) + "ms");

		x.tryFlip(1);
		p(x.getHamiltonian());

		// flip points chosen before and calc. new Hamiltonian
		start = System.currentTimeMillis();
		// p("H New: " + x.tryFlip(a)); // New
		end = System.currentTimeMillis();
		// p("H = " + x.getHamiltonian()); // Old
		// p("Hamilton: " + (end - start) + "ms");

		// tryFlip(int[size]):
		// 1k = 7ms
		// 3k = 15ms
		// 5k = 20ms
		// 10k = 30ms
		// 20k = 45ms
		// 40k = 70ms
		// 50k = 90ms
		// 3x more changed sites = 2x more computing time

	}

	public static void p(Object s) {
		System.out.println(s);
	}
}
