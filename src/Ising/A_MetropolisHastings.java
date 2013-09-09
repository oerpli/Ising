package Ising;

public abstract class A_MetropolisHastings implements A_Interface {
	public static boolean accept() {
		double diffE = Hamilton.getDE();
		// System.out.println(diffE);
		if (diffE <= 0)
			return true;
		else
			return Math.random() < Math.exp(-diffE * Hamilton.Beta);
	}
}
