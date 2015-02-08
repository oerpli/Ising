package Data;

public class EventBuffer {
	private final DataSet[] T;
	private final int size;
	// private final int events;
	// private boolean save = true;
	private int pointer = 0;
	// private int epointer = 0;
	private long event;

	public EventBuffer(int size) {
		this.size = Math.max(size, 1);
		// this.events = events;
		this.T = new DataSet[size];
	}

	public String add(DataSet entry) {
		T[pointer] = entry;
		String out;
		if (T[pointer].T == event)
			out = this.toString();
		else
			out = "";
		pointer = (pointer + 1) % size;
		return out;
	}



	public DataSet[] get(int offset) {
		int x = pointer + offset;
		DataSet[] r = new DataSet[size];
		for (int i = 0; i < size; i++) {
			r[i] = T[(i + x) % size];
		}
		return r;
	}

	public void event() {
		// epointer++;
		event = T[(size + pointer - 1) % size].T + size / 2;
	}

	public String toString() {
		StringBuffer out = new StringBuffer();
		for (int i = 0; i < size; i++) {
			out.append(T[(pointer + i) % size]);
		}
		return out.toString();
	}
}
