open System.Windows.Forms
open System.Drawing

let PI = System.Math.PI
let deg2rad (a:single) =
  PI * float(a / 180.f) |> single

let clock = new Form(Text="Clock",TopMost=true)
clock.Show()

let timer = new Timer(Interval=900)
timer.Tick.Add(fun _ ->
  clock.Invalidate()
)
timer.Start()

clock.Paint.Add(fun e ->
  let g = e.Graphics
  let d = min clock.ClientSize.Width clock.ClientSize.Height
  g.SmoothingMode <- Drawing2D.SmoothingMode.HighQuality
  g.DrawEllipse(Pens.Gray, 0, 0, d, d)
  
  let r = (single d) / 2.f
  let cx, cy = (r, r)
  let th = r / 10.f

  g.TranslateTransform(cx, cy)
  g.ScaleTransform(1.f, -1.f)

  let drawlancet p a r1 r2 =
    let t = g.Transform
    g.RotateTransform(-a)
    g.DrawLine(p, 0.f, r1, 0.f, r2)
    g.Transform <- t
    //g.RotateTransform(a) // Il contesto grafico è modale!!!

  for a in 0.f .. 30.f .. 360.f do
    drawlancet Pens.Black a (r - th) r

  use ph = new Pen(Color.Black, 4.f)
  use pm = new Pen(Color.Red, 2.f)
  use ps = new Pen(Color.Blue, 1.f)

  let t = System.DateTime.Now
  let ah = single(t.Hour * 60 + t.Minute) / (12.f * 60.f ) * 360.f
  drawlancet ph ah -th (r / 2.f)
  let am = single(t.Minute) / 60.f * 360.f
  drawlancet pm am -th (r * 0.75f)
  let asec = single(t.Second) / 60.f * 360.f
  drawlancet ps asec -th r

  // ph.Dispose() // iniettato dal compilatore perché si usa "use"
)

clock.Resize.Add(fun _ ->
  clock.Invalidate()
)

