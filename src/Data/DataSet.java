package Data;

import Model.Lattice;

public class DataSet {
	public final double E, M;
	public final long T;

	public DataSet(long t, double e, double m) {

		this.T = t;
		this.E = e / Lattice.L.N();
		this.M = m / Lattice.L.N();
	}

	public String log() {
		return E + " " + M + '\n';
	}

	public String toString() {// TODO DataSet Textoutput
		return this.log();
	}
}
