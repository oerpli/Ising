function [ file ] = PlotF( file )
D = read(file);
M = D(:,2);
x = autocorr(M,4000);
figure;
plot(x);
figure;
plot(log(x));
end

