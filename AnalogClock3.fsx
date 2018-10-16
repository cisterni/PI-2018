open System.Windows.Forms
open System.Drawing

let clockDiameter = 100.f

let PI = System.Math.PI
let deg2rad (a:single) =
  PI * float(a / 180.f) |> single

let clock = new Form(Text="Clock",TopMost=true)
clock.Show()

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

type AnalogClock () as this =
  inherit UserControl()
  let wv = WVMatrix()
  
  let mutable currentTime = new System.DateTime()
  do
    wv.ScaleW(1.f, -1.f)
  
  member this.Time
    with get() = currentTime
    and set(v) = 
      currentTime <- v
      this.Invalidate()

  override this.OnPaint e =
    let g = e.Graphics
    let d = (min this.ClientSize.Width this.ClientSize.Height) |> single

    g.SmoothingMode <- Drawing2D.SmoothingMode.HighQuality
  
    let r = d / 2.f - 1.f
    let cx, cy = (r, r)
    let th = r / 10.f

    let cw = wv.TransformPointV(PointF(cx, cy))
    wv.TranslateW(cw.X, cw.Y)
    g.Transform <- wv.WV
    printfn "%A" cw 
    printfn "%f %f" cx cy 
    printfn "%A" (wv.TransformPointV(PointF(cx, cy)))

    g.DrawEllipse(Pens.Gray, -cx, -cy, d, d)

    let drawlancet p a r1 r2 =
      let t = g.Transform
      g.RotateTransform(-a)
      g.DrawLine(p, 0.f, r1, 0.f, r2)
      g.Transform <- t

    for a in 0.f .. 30.f .. 360.f do
      drawlancet Pens.Black a (r - th) r

    use ph = new Pen(Color.Black, 4.f)
    use pm = new Pen(Color.Red, 2.f)
    use ps = new Pen(Color.Blue, 1.f)

    let t = currentTime
    let ah = single(t.Hour * 60 + t.Minute) / (12.f * 60.f ) * 360.f
    drawlancet ph ah -th (r / 2.f)
    let am = single(t.Minute) / 60.f * 360.f
    drawlancet pm am -th (r * 0.75f)
    let asec = single(t.Second) / 60.f * 360.f
    drawlancet ps asec -th r
    base.OnPaint e

  override this.OnResize e =
    this.Invalidate()
    base.OnResize e

let rome = new AnalogClock()
clock.Controls.Add(rome)
let ny = new AnalogClock()
clock.Controls.Add(ny)
ny.Left <- rome.Width

let t = new Timer(Interval=950)
t.Tick.Add(fun _ ->
  rome.Time <- System.DateTime.Now
  ny.Time <- System.DateTime.Now - System.TimeSpan(6,0,0)
)
t.Start()
