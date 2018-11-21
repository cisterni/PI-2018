function increment(i) {
  function f(x) {
    return x+i;
  }
  return f;
}

var incrBy1 = increment(1);
var incrBy2 = increment(2);

incrBy1(0);
incrBy2(0);
