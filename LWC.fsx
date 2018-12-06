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

  let mutable parent : UserControl option = None

  member this.WV = wv

  member this.Parent
    with get() = parent
    and set(v) = parent <- v

  abstract OnPaint : PaintEventArgs -> unit
  default this.OnPaint (e) = ()

  abstract OnMouseDown : MouseEventArgs -> unit
  default this.OnMouseDown (e) = ()

  abstract OnMouseUp : MouseEventArgs -> unit
  default this.OnMouseUp (e) = ()

  abstract OnMouseMove : MouseEventArgs -> unit
  default this.OnMouseMove (e) = ()

  member this.Invalidate() =
    match parent with
    | Some p -> p.Invalidate()
    | None -> ()
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

  member this.PositionInt with get() = Point(int pos.X, int pos.Y)

  member this.ClientSizeInt with get() = Size(int sz.Width, int sz.Height)


  member this.Left = pos.X
  
  member this.Top = pos.Y

  member this.Width = sz.Width
  member this.Height = sz.Height

type LWCContainer() as this =
  inherit UserControl()

  let controls = System.Collections.ObjectModel.ObservableCollection<LWCControl>()

  do 
    controls.CollectionChanged.Add(fun e ->
      for i in e.NewItems do
        (i :?> LWCControl).Parent <- Some(this :> UserControl)
    )

  member this.LWControls with get() = controls

  override this.OnMouseDown (e) =
    let oc =
      controls |> Seq.tryFindBack(fun c -> c.HitTest(e.Location))
    match oc with
    | Some c ->
      let p = c.WV.TransformPointV(PointF(single e.X, single e.Y))
      let evt = new MouseEventArgs(e.Button, e.Clicks, int p.X, int p.Y, e.Delta)
      c.OnMouseDown(evt)
    | None -> () 

  override this.OnMouseUp (e) =
    let oc =
      controls |> Seq.tryFindBack(fun c -> c.HitTest(e.Location))
    match oc with
    | Some c ->
      let p = c.WV.TransformPointV(PointF(single e.X, single e.Y))
      let evt = new MouseEventArgs(e.Button, e.Clicks, int p.X, int p.Y, e.Delta)
      c.OnMouseUp(evt)
    | None -> () 

  override this.OnMouseMove (e) =
    let oc =
      controls |> Seq.tryFindBack(fun c -> c.HitTest(e.Location))
    match oc with
    | Some c ->
      let p = c.WV.TransformPointV(PointF(single e.X, single e.Y))
      let evt = new MouseEventArgs(e.Button, e.Clicks, int p.X, int p.Y, e.Delta)
      c.OnMouseMove(evt)
    | None -> () 

  override this.OnPaint(e) =
    controls 
    |> Seq.iter(fun c ->
      let bkg = e.Graphics.Save()

      // esercizio: impostare il rettangolo in client space
      let evt = new PaintEventArgs(e.Graphics, Rectangle(c.PositionInt, c.ClientSizeInt))
      //bug: non supporta la rotazione
      e.Graphics.SetClip(new RectangleF(c.Position, c.ClientSize))
      e.Graphics.Transform <- c.WV.WV
      c.OnPaint(evt)
      e.Graphics.Restore(bkg)
    )

// Utente Libreria
type LWButton() =
  inherit LWCControl()

  override this.OnPaint(e) =
    let g = e.Graphics
    g.FillRectangle(Brushes.Red, 0.f, 0.f, this.Width, this.Height)
    g.DrawLine(Pens.Blue, 0.f, 0.f, 2.f*this.Width, 2.f*this.Height)
  
  override this.OnMouseDown(e) =
    printfn "%A" e.Location

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