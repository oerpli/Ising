package Data;

import java.awt.*;
import java.awt.font.*;
import java.awt.geom.*;
import javax.swing.*;

import Render.S;

public class Plotter extends JPanel {
	private static final long serialVersionUID = -5078789193930191629L;
	final int PAD = 20;
	public DataSet[] D;
	public double max = 2, min = -2, meanE = 0, meanM = 0, meanM2 = 0;
	private static int mMc = 0;
	private double[] hist = new double[Log.histchan];
	public static boolean hi = true;
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

		g2.setPaint(Color.BLACK);
		// draw ordinate
		g2.draw(new Line2D.Double(PAD, PAD, PAD, h - PAD));
		// Draw abcissa.
		g2.draw(new Line2D.Double(PAD, h / 2, w - 2 * PAD, h / 2));
		// Draw labels
		Font font = g2.getFont();
		FontRenderContext frc = g2.getFontRenderContext();
		LineMetrics lm = font.getLineMetrics("0", frc);
		float sh = lm.getAscent() + lm.getDescent();

		// Ordinate label.
		String s = "M b               E r";
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

		double xInc = (double) (w - 3 * PAD) / Math.max(1, (D.length - 1));
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
			g2.fill(new Ellipse2D.Double(x - 1, yM - 1, 2, 2));
			g2.setPaint(Color.red);
			g2.fill(new Ellipse2D.Double(x - 1, yE - 1, 2, 2));
		}

		// Histogram
		if (hi) {
			double hc = (2 * scale) / (Log.histchan - 1);
			for (int i = 0; i < Log.histchan; i++) {
				double hy = h / 2 - hc / 2 + scale - hc * i;
				g2.setPaint(Color.BLACK);
				g2.fill(new Rectangle2D.Double(PAD, hy, hist[i] * 200, hc));
				s = S.df2.format(hist[i]);
				g2.drawString(s, -9, (int) (hy + hc / 2));
			}
		}
		// Mean M
		g2.setPaint(Color.blue);
		int my = (int) (h - PAD - scale * meanM);
		g2.draw(new Line2D.Double(PAD, my, w - 35, my));
		s = S.df.format(meanM + min);
		g2.drawString(s, w - 35, (int) (h - PAD - scale * meanM));

		// Mean M2
		g2.setPaint(Color.MAGENTA.darker());
		my = (int) (h - PAD - scale * meanM2);
		g2.draw(new Line2D.Double(PAD, my, w - 35, my));

		s = S.df.format(meanM2 + min);
		g2.drawString(s, w - 35, (int) (h - PAD - scale * meanM2));

		// Mean E
		g2.setPaint(Color.red);
		s = S.df.format(meanE + min);
		g2.drawString(s, w - 35, (int) (h - PAD - scale * meanE));
		g2.draw(new Line2D.Double(PAD, h - PAD - scale * meanE, w - 2 * PAD, h
				- PAD - scale * meanE));

		// Mean labels

	}

	// private double getMax() {
	// double max = Double.MIN_VALUE;
	// for (int i = 0; i < D.length; i++) {
	// if (D[i] != null && D[i].M > max)
	// max = D[i].M;
	// }
	// for (int i = 0; i < D.length; i++) {
	// if (D[i] != null && D[i].E > max)
	// max = D[i].E;
	// }
	// return max;
	// }
	//
	// private double getMin() {
	// double min = Double.MIN_VALUE;
	// for (int i = 0; i < D.length; i++) {
	// if (D[i] != null && D[i].M < min)
	// min = D[i].M;
	// }
	// for (int i = 0; i < D.length; i++) {
	// if (D[i] != null && D[i].E < min)
	// min = D[i].E;
	// }
	// return min;
	// }

	// public static void main(String[] args) { }

	public void start(DataSet[] x) {
		f.add(this);
		f.setSize(800, 400);
		f.setLocation(200, 200);
		f.setVisible(true);
		hist();
		this.set(x);
	}

	private double getMean() {
		double sum = 0;
		for (DataSet i : D) {
			if (i != null)
				sum += i.M;
		}
		return sum / D.length;
	}

	private double getMean2() {
		double sum = 0;
		for (DataSet i : D) {
			if (i != null)
				sum += Math.abs(i.M);
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
		meanM = (getMean() + (meanM + min) * mMc) / (mMc + 1) - min;
		meanE = (getMeanE() + (meanE + min) * mMc) / (mMc + 1) - min;
		meanM2 = (getMean2() + (meanM2 + min) * mMc) / (mMc + 1) - min;
		mMc++;
		this.D = d2;
		hist();

		this.repaint();
		// f.setVisible(true);
	}

	private void hist() {
		this.hist = new double[Log.histchan];
		for (int i = 0; i < Log.histchan; i++) {
			this.hist[i] = Log.hist[i] / Log.nhist;
		}
	}

	public static void resetM() {
		Log.nhist = 0;
		for (int i = 0; i < Log.histchan; i++)
			Log.hist[i] = 0;
		mMc = 0;
	}

	public void toggle(boolean x) {
		f.setVisible(x);
	}
}