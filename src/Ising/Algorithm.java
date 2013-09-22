package Ising;

public interface Algorithm {
	public boolean update(Lattice L);
	public void getNewEnergy(Point p);
}
