package Data;

import Render.IsingRender;

public class DataSet {
	public final double E, M;
	public final long T;
	public static int N = IsingRender.N;

	public DataSet(long t, double e, double m) {

		this.T = t;
		this.E = e / N;
		this.M = m / N;
	}

	public String log() {
		return E + " " + M + '\n';
	}

	public String toString() {// TODO DataSet Textoutput
		return this.log();
	}
}
