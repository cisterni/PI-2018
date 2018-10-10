open System.Windows.Forms
open System.Drawing

let clockDiameter = 100.f

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

type WVMatrix () =
  let wv = new Drawing2D.Matrix()
  let vw = new Drawing2D.Matrix()

  member this.Translate (tx, ty) =
    wv.Translate(tx, ty)
    vw.Translate(-tx, -ty, Drawing2D.MatrixOrder.Append)

  member this.Scale (sx, sy) =
    wv.Scale(sx, sy)
    vw.Scale(1.f /sx, 1.f/ sy, Drawing2D.MatrixOrder.Append)

  member this.Rotate (a) =
    wv.Rotate(a)
    vw.Rotate(-a, Drawing2D.MatrixOrder.Append)

  member this.VW with get() = vw
  member this.WV with get() = wv

let wv = WVMatrix()

wv.Translate(100.f, 100.f)

wv.Scale(1.f, -1.f)

clock.KeyDown.Add(fun e ->
  match e.KeyData with
  | Keys.W -> wv.Translate(0.f, 10.f) // wv *= T(0,10)
  | Keys.S -> wv.Translate(0.f, -10.f) // wv *= T(0,-10)
  | Keys.A -> wv.Translate(-10.f, 0.f) // wv *= T(0,-10)
  | Keys.D -> wv.Translate(10.f, 0.f) // wv *= T(0,-10)
  | Keys.Q -> wv.Rotate(10.f)
  | Keys.Z -> wv.Scale(1.1f, 1.1f)
  | Keys.X -> wv.Scale(1.f/1.1f, 1.f/1.1f)
  | _ -> ()
  clock.Invalidate()
)

clock.Paint.Add(fun e ->
  let g = e.Graphics
  let d = clockDiameter

  g.Transform <- wv.WV

  g.SmoothingMode <- Drawing2D.SmoothingMode.HighQuality
  
  let r = d / 2.f
  let cx, cy = (r, r)
  let th = r / 10.f

  g.DrawEllipse(Pens.Gray, -cx, -cy, d, d)

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

