function D = read(x)
[e,m] = textread(x,'%f %f','commentstyle','shell');
D = [e,m];
end