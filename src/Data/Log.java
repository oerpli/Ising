package Data;

import java.io.File;
import java.io.FileWriter;
import java.io.IOException;

import Model.Hamiltonian;
import Render.IsingRender;
import Render.S;

public abstract class Log {
	public static int sign = -1;
	public static long time = 0;// 1 sweep/time
	public static FileWriter W;
	public static File file;
	private static boolean log = false; // ~500F/ms
	private static DataSet data;

	public static void init(int N, int N2) {
		if (!log) {
			String name = "LOG_" + N + "x" + N2 + Hamiltonian.J() + "-"
					+ Hamiltonian.kT() + "_" + System.currentTimeMillis() + ".txt";
			System.out.println("New Logfile:" + name);
			try {
				file = new File(name);
				W = new FileWriter(file, true);
				W.write(N + "x" + N2 + '\n');
				W.write("J=" + Hamiltonian.J() + ";h=" + Hamiltonian.h() + ";kT="
						+ Hamiltonian.kT() + '\n');
				W.write("t E M\n");
			} catch (IOException e) {
				e.printStackTrace();
			}
			log = true;
		} else {
			System.out.println("logging off");
			log = false;
		}
	}

	public static void log() {
		if (log) {
			time = IsingRender.sweeps;
			try {
				data = new DataSet(time, Hamiltonian.E_nn, Hamiltonian.E_m);
				W.write(S.buffer.add(data));
				// checkTransition();
				if (time % 1024 == 0) {
					W.write(data.toString());
					if (time % 32768 == 0)
						W.flush();
				}
			} catch (IOException e) {
				e.printStackTrace();
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
			S.buffer.event();
		}
	}
}
