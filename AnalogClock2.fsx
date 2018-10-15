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

  member this.TranslateW (tx, ty) =
    wv.Translate(tx, ty)
    vw.Translate(-tx, -ty, Drawing2D.MatrixOrder.Append)

  member this.ScaleW (sx, sy) =
    wv.Scale(sx, sy)
    vw.Scale(1.f /sx, 1.f/ sy, Drawing2D.MatrixOrder.Append)

  member this.RotateW (a) =
    wv.Rotate(a)
    vw.Rotate(-a, Drawing2D.MatrixOrder.Append)

  member this.RotateV (a) =
    vw.Rotate(a)
    wv.Rotate(-a, Drawing2D.MatrixOrder.Append)

  member this.TranslateV (tx, ty) =
    vw.Translate(tx, ty)
    wv.Translate(-tx, -ty, Drawing2D.MatrixOrder.Append)

  member this.ScaleV (sx, sy) =
    vw.Scale(sx, sy)
    wv.Scale(1.f /sx, 1.f/ sy, Drawing2D.MatrixOrder.Append)
  
  member this.TransformPointV (p:PointF) =
    let a = [| p |]
    vw.TransformPoints(a)
    a.[0]

  member this.TransformPointW (p:PointF) =
    let a = [| p |]
    wv.TransformPoints(a)
    a.[0]

  member this.VW with get() = vw
  member this.WV with get() = wv

let wv = WVMatrix()

wv.TranslateV(-50.f, -50.f)

wv.ScaleW(1.f, -1.f)

clock.KeyDown.Add(fun e ->
  match e.KeyData with
  | Keys.W -> wv.TranslateV(0.f, 10.f) // wv *= T(0,10)
  | Keys.S -> wv.TranslateV(0.f, -10.f) // wv *= T(0,-10)
  | Keys.A -> wv.TranslateV(-10.f, 0.f) // wv *= T(0,-10)
  | Keys.D -> wv.TranslateV(10.f, 0.f) // wv *= T(0,-10)
  | Keys.Q ->
     let client = clock.ClientSize
     wv.TranslateV(client.Width / 2 |> single, client.Height / 2 |> single)
     wv.RotateV(10.f)
     wv.TranslateV(-client.Width / 2 |> single, -client.Height / 2 |> single)
  | Keys.E ->
     let client = clock.ClientSize
     wv.TranslateV(client.Width / 2 |> single, client.Height / 2 |> single)
     wv.RotateV(-10.f)
     wv.TranslateV(-client.Width / 2 |> single, -client.Height / 2 |> single)
  | Keys.Z -> 
    let cx, cy = clock.ClientSize.Width / 2 |> single, clock.ClientSize.Height / 2 |> single
    let po = PointF(cx, cy) |> wv.TransformPointV
    wv.ScaleV(1.1f, 1.1f)
    let pn = PointF(cx, cy) |> wv.TransformPointV
    wv.TranslateW(pn.X - po.X, pn.Y - po.Y)
  | Keys.X ->
    let cx, cy = clock.ClientSize.Width / 2 |> single, clock.ClientSize.Height / 2 |> single
    let po = PointF(cx, cy) |> wv.TransformPointV
    wv.ScaleV(1.f/1.1f, 1.f/1.1f)
    let pn = PointF(cx, cy) |> wv.TransformPointV
    wv.TranslateW(pn.X - po.X, pn.Y - po.Y)
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

