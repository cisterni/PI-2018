open System.Windows.Forms
open System.Drawing

// Libreria

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

 type LWCControl() =
  let wv = WVMatrix()

  let mutable sz = SizeF(120.f, 120.f)
  
  let mutable pos = PointF()

  member this.WV = wv

  abstract OnPaint : PaintEventArgs -> unit
  default this.OnPaint (e) = ()

  abstract OnMouseDown : MouseEventArgs -> unit
  default this.OnMouseDown (e) = ()

  member this.Invalidate() =
    ()

  member this.HitTest(p:Point) =
    let pt = wv.TransformPointV(PointF(single p.X, single p.Y))
    let boundingbox = RectangleF(0.f, 0.f, sz.Width, sz.Height)
    boundingbox.Contains(pt)

  member this.ClientSize
    with get() = sz
    and set(v) = 
      sz <- v
      this.Invalidate()

  member this.Position
    with get() = pos
    and set(v) =
      wv.TranslateV(pos.X, pos.Y)
      pos <- v
      wv.TranslateV(-pos.X, -pos.Y)
      this.Invalidate()


  member this.Left = pos.X
  
  member this.Top = pos.Y

  member this.Width = sz.Width
  member this.Height = sz.Height



type LWCContainer() =
  inherit UserControl()

  let controls = ResizeArray<LWCControl>()

  member this.LWControls with get() = controls

  override this.OnMouseDown (e) =
    let oc =
      controls |> Seq.tryFindBack(fun c -> c.HitTest(e.Location))
    match oc with
    | Some c -> 
      printfn "Clicked"
      c.OnMouseDown(e)
    | None -> () 

  override this.OnPaint(e) =
    controls 
    |> Seq.iter(fun c ->
      let bkg = e.Graphics.Save()
      e.Graphics.Transform <- c.WV.WV
      c.OnPaint(e)
      e.Graphics.Restore(bkg)
    )

// Utente Libreria
type LWButton() =
  inherit LWCControl()

  override this.OnPaint(e) =
    let g = e.Graphics
    g.FillRectangle(Brushes.Red, 0.f, 0.f, this.Width, this.Height)

// Test
let btn = new LWButton(Position=PointF(20.f, 10.f))
let btn2 = new LWButton(Position=PointF(180.f, 10.f))

let lwcc = new LWCContainer(Dock=DockStyle.Fill)
lwcc.LWControls.Add(btn)
lwcc.LWControls.Add(btn2)

let f = new Form(Text="Prova")
f.Controls.Add(lwcc)
f.Show()

// btn.Position <- PointF(150.f, 70.f)
// lwcc.Invalidate()