package Data;

import java.awt.*;
import java.awt.font.*;
import java.awt.geom.*;
import javax.swing.*;

public class Plotter extends JPanel {
	private static final long serialVersionUID = -5078789193930191629L;
	final int PAD = 20;
	public DataSet[] D;
	public double max, min, meanE, meanM;

	JFrame f;
	Graphics gs;

	public Plotter(DataSet[] x) {
		f = new JFrame();
		this.D = x;
	}

	protected void paintComponent(Graphics g) {
		this.gs = g;
		super.paintComponent(g);
		Graphics2D g2 = (Graphics2D) g;
		g2.setRenderingHint(RenderingHints.KEY_ANTIALIASING,
				RenderingHints.VALUE_ANTIALIAS_ON);
		int w = getWidth();
		int h = getHeight();
		// Draw ordinate.
		g2.draw(new Line2D.Double(PAD, PAD, PAD, h - PAD));
		// Draw abcissa.
		// g2.draw(new Line2D.Double(PAD, h - PAD, w - PAD, h - PAD));
		// Draw labels.
		Font font = g2.getFont();
		FontRenderContext frc = g2.getFontRenderContext();
		LineMetrics lm = font.getLineMetrics("0", frc);
		float sh = lm.getAscent() + lm.getDescent();

		// Ordinate label.
		String s = "M/N b     E/N r";
		float sy = PAD + ((h - 2 * PAD) - s.length() * sh) / 2 + lm.getAscent();
		for (int i = 0; i < s.length(); i++) {
			String letter = String.valueOf(s.charAt(i));
			float sw = (float) font.getStringBounds(letter, frc).getWidth();
			float sx = (PAD - sw) / 2;
			g2.drawString(letter, sx, sy);
			sy += sh;
		}

		// Abcissa label.
		s = "Time";
		sy = h - PAD + (PAD - sh) / 2 + lm.getAscent();
		float sw = (float) font.getStringBounds(s, frc).getWidth();
		float sx = (w - sw) / 2;
		g2.drawString(s, sx, sy);
		// Draw lines.

		double xInc = (double) (w - 2 * PAD) / Math.max(1, (D.length - 1));
		double scale = (double) (h - 2 * PAD) / (max - min);

		if (D.length < 200) {
			for (int i = 0; i < D.length - 1; i++) {
				double x1 = PAD + i * xInc;
				double x2 = PAD + (i + 1) * xInc;

				double y1M = h - PAD - scale * (D[i].M - min);
				double y2M = h - PAD - scale * (D[i + 1].M - min);

				double y1E = h - PAD - scale * (D[i].E - min);
				double y2E = h - PAD - scale * (D[i + 1].E - min);
				g2.setPaint(Color.blue);
				g2.draw(new Line2D.Double(x1, y1M, x2, y2M));
				g2.setPaint(Color.red.darker());
				g2.draw(new Line2D.Double(x1, y1E, x2, y2E));
			}
		}
		// Mark data points.
		for (int i = 0; i < D.length; i++) {
			double x = PAD + i * xInc;
			double yM = h - PAD - scale * (D[i].M - min);
			double yE = h - PAD - scale * (D[i].E - min);
			g2.setPaint(Color.blue);
			g2.fill(new Ellipse2D.Double(x - 2, yM - 2, 4, 4));
			g2.setPaint(Color.red);
			g2.fill(new Ellipse2D.Double(x - 2, yE - 2, 4, 4));
		}
		g2.setPaint(Color.blue);

		g2.draw(new Line2D.Double(PAD, h - PAD - scale * meanM, w - PAD, h
				- PAD - scale * meanM));
		g2.setPaint(Color.red);
		g2.draw(new Line2D.Double(PAD, h - PAD - scale * meanE, w - PAD, h
				- PAD - scale * meanE));
		g2.setPaint(Color.BLACK);
		g2.draw(new Line2D.Double(PAD, h / 2, w - PAD, h / 2));
	}

	private double getMax() {
		double max = Double.MIN_VALUE;
		for (int i = 0; i < D.length; i++) {
			if (D[i] != null && D[i].M > max)
				max = D[i].M;
		}
		for (int i = 0; i < D.length; i++) {
			if (D[i] != null && D[i].E > max)
				max = D[i].E;
		}
		return max;
	}

	private double getMin() {
		double min = Double.MIN_VALUE;
		for (int i = 0; i < D.length; i++) {
			if (D[i] != null && D[i].M < min)
				min = D[i].M;
		}
		for (int i = 0; i < D.length; i++) {
			if (D[i] != null && D[i].E < min)
				min = D[i].E;
		}
		return min;
	}

	// public static void main(String[] args) { }

	public void start(DataSet[] x) {
		// f.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		f.add(this);
		f.setSize(800, 400);
		f.setLocation(200, 200);
		f.setVisible(true);
	}

	private double getMean() {
		double sum = 0;
		for (DataSet i : D) {
			if (i != null)
				sum += i.M;
		}
		return sum / D.length;
	}

	private double getMeanE() {
		double sum = 0;
		for (DataSet i : D) {
			if (i != null)
				sum += i.E;
		}
		return sum / D.length;
	}

	public void set(DataSet[] d2) {
		max = 2;// getMax();
		min = -2;// getMin();
		meanM = getMean() - min;
		meanE = getMeanE() - min;
		this.D = d2;
		this.repaint();
		// f.setVisible(true);
	}
}