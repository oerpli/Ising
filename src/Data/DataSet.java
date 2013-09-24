package Data;

public class DataSet {
	public final double E, M;
	public final long T;

	public DataSet(long t, double e, double m) {
		this.T = t;
		this.E = e;
		this.M = m;
	}

	public String log() {
		return "" + T + " " + E + " " + M + '\n';
	}

	public String toString() {// TODO DataSet Textoutput
		return this.log();
	}
}
