package Data;

import java.io.File;
import java.io.FileWriter;
import java.io.IOException;

import Dynamics.Algorithm;
import Model.Hamiltonian;
import Render.IsingRender;

public abstract class Log {
	public static int sign = -1;
	public static int time = 0;// 1 sweep/time
	public static FileWriter W = null;
	public static File Path;
	private static boolean log = false; // ~500F/ms

	static int n = 400;
	public static DataSet[] D = new DataSet[n];
	public static Plotter p = new Plotter(D);

	// public static EventBuffer buffer = new EventBuffer(50);

	public static void init(int N, int N2) {
		if (!log) {
			String name = N * N2 + "-" + Hamiltonian.kT() + "_"
					+ Algorithm.String() + ".txt";
			try {

				if (W != null)
					W.close();
				Path = new File(name);
				if (Path.exists()) {
					Path.delete();
				}
				W = new FileWriter(Path, true);
				W.write("#" + N + "x" + N2 + '\n');
				W.write("#" + "J=" + Hamiltonian.J() + ";h=" + Hamiltonian.h()
						+ ";kT=" + Hamiltonian.kT() + '\n');
				W.write("#" + "t E M\n");
				time = 0;
			} catch (IOException e) {
				e.printStackTrace();
			} finally {
				System.out.println("New Log: " + name);
			}
			log = true;
		} else {
			System.out.println("logging off");
			log = false;
		}
	}

	public static void log() {
		D[time % n] = new DataSet(time, Hamiltonian.E_nn, Hamiltonian.E_m);
		if (++time % n == 0) {
			if (time < 2 * n)
				p.start(D);
			else
				p.set(D);
			// IsingRender.Stop();
		}
		if (log) {
			try {
				W.write(D[time].toString());
				// W.write(buffer.add(data));
				// checkTransition();
				if (time % 1024 == 0)
					W.flush();
			} catch (IOException e) {
				e.printStackTrace();
			}
			if (time == 1000000) {
				log = false;
				IsingRender.Stop();
				try {
					W.flush();
					W.close();
				} catch (IOException e) {
					// IDGAF
				}
				System.out.println("finished");
			}
		}

	}

	/**
	 * Halley's Comment
	 */
	private static void checkTransition() {
		if (Math.signum(Hamiltonian.E_m) == -sign) {
			System.out.println("trans");
			sign = -sign;
			// S.buffer.event();
		}
	}
}
