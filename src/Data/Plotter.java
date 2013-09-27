package Data;

import java.awt.*;
import java.awt.font.*;
import java.awt.geom.*;
import java.util.Random;
import javax.swing.*;

public class Plotter extends JPanel {
	private static final long serialVersionUID = -5078789193930191629L;
	final int PAD = 20;
	public DataSet[] D;
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
		g2.draw(new Line2D.Double(PAD, h - PAD, w - PAD, h - PAD));
		// Draw labels.
		Font font = g2.getFont();
		FontRenderContext frc = g2.getFontRenderContext();
		LineMetrics lm = font.getLineMetrics("0", frc);
		float sh = lm.getAscent() + lm.getDescent();

		// Ordinate label.
		String s = "data";
		float sy = PAD + ((h - 2 * PAD) - s.length() * sh) / 2 + lm.getAscent();
		for (int i = 0; i < s.length(); i++) {
			String letter = String.valueOf(s.charAt(i));
			float sw = (float) font.getStringBounds(letter, frc).getWidth();
			float sx = (PAD - sw) / 2;
			g2.drawString(letter, sx, sy);
			sy += sh;
		}

		// Abcissa label.
		s = "x axis";
		sy = h - PAD + (PAD - sh) / 2 + lm.getAscent();
		float sw = (float) font.getStringBounds(s, frc).getWidth();
		float sx = (w - sw) / 2;
		g2.drawString(s, sx, sy);
		// Draw lines.

		double xInc = (double) (w - 2 * PAD) / Math.max(1, (D.length - 1));
		double scale = (double) (h - 2 * PAD) / (getMax() - getMin());

		double sum = 0;
		for (DataSet i : D) {
			if (i != null)
				sum += i.M;
		}
		sum = sum / D.length - getMin();

		if (D.length < 200) {
			g2.setPaint(Color.green.darker());
			for (int i = 0; i < D.length - 1; i++) {
				double x1 = PAD + i * xInc;
				double y1 = h - PAD - scale * (D[i].M - getMin());
				double x2 = PAD + (i + 1) * xInc;
				double y2 = h - PAD - scale * (D[i + 1].M - getMin());
				g2.draw(new Line2D.Double(x1, y1, x2, y2));
			}
		}
		// Mark data points.
		g2.setPaint(Color.red);
		for (int i = 0; i < D.length; i++) {
			double x = PAD + i * xInc;
			double y = h - PAD - scale * (D[i].M - getMin());
			g2.fill(new Ellipse2D.Double(x - 2, y - 2, 4, 4));
		}
		g2.draw(new Line2D.Double(PAD, h - PAD - scale * sum, w - PAD, h - PAD
				- scale * sum));
	}

	private double getMax() {
		double max = Double.MIN_VALUE;
		for (int i = 0; i < D.length; i++) {
			if (D[i] != null && D[i].M > max)
				max = D[i].M;
		}
		return max;
	}

	private double getMin() {
		double min = Double.MIN_VALUE;
		for (int i = 0; i < D.length; i++) {
			if (D[i] != null && D[i].M < min)
				min = D[i].M;
		}
		return min;
	}

	public static void main(String[] args) {
	}

	public void start(DataSet[] x) {
		// f.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		f.add(this);
		f.setSize(800, 400);
		f.setLocation(200, 200);
		f.setVisible(true);
	}

	public void set(DataSet[] d2) {
		this.D = d2;
		this.repaint();
		f.setVisible(true);
	}
}