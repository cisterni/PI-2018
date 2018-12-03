open System.Windows.Forms
open System.Drawing
open System.Drawing.Drawing2D

type MovingBox(r:Rectangle) =
  let mutable box = r
  let bgcolor = Color.Red

  member this.Contains (x, y) =
    box.Contains(x, y)

  member this.X
    with get () = box.X
    and set(v) = box <- Rectangle(v, box.Y, box.Width, box.Height)
  member this.Y
    with get () = box.Y
    and set(v) = box <- Rectangle(box.X, v, box.Width, box.Height)
  member this.Location
    with get() = Point(box.X, box.Y)
    and set(v:Point) = box <- Rectangle (v.X, v.Y, box.Width, box.Height)
  member this.OnPaint(g:Graphics) =
    use b = new SolidBrush(bgcolor)
    g.FillRectangle(b, box)
    g.DrawRectangle(Pens.Black, box)

type MouseTest() as this =
  inherit UserControl()

  do
//    this.SetStyle(ControlStyles.AllPaintingInWmPaint ||| ControlStyles.OptimizedDoubleBuffer, true)
    this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true)

  let boxes = ResizeArray<MovingBox>()
  let mutable box = Rectangle(50, 50, 100, 100)
  let mutable start = None
  let duration = new System.TimeSpan(0,0,0,0,1000)
  let mutable alpha = 0.f
  let mutable inbox = false
  let mutable drag = None
  let mutable newbox = None

  let mkrect (sx, sy) (ex, ey) =
    Rectangle(min sx ex, min sy ey, abs(sx - ex), abs(sy - ey))

  let t = new Timer(Interval=100)

  do t.Tick.Add(fun _ ->
    // esempi https://easings.net/it
    let easingfunction (start:System.DateTime) (duration:System.TimeSpan) t =
      let dt = t - start
      single(dt.TotalMilliseconds) / single(duration.TotalMilliseconds)

    if start.IsNone then
      start <- Some (System.DateTime.Now)
        
    let perc = easingfunction start.Value duration System.DateTime.Now
    printfn "%A" perc
    alpha <- max 0.f (min perc 1.f)
    if alpha >= 1.f  then t.Stop()
    this.Invalidate()
  )

  override this.OnPaint e =
    let g = e.Graphics
    //let a = int(alpha * 255.f)
    boxes |> Seq.iter (fun b ->
      b.OnPaint(g)
    )
    match newbox with
    | Some ((sx, sy), (ex, ey)) ->
      let r = mkrect (sx, sy) (ex, ey)
      use p = new Pen(Color.Gray)
      p.DashStyle <- Drawing2D.DashStyle.DashDotDot
      g.DrawRectangle(p, r)
    | _ -> ()

   override this.OnMouseDown e =
     //Pick correlation
     let b = boxes |> Seq.tryFind (fun box -> box.Contains(e.X, e.Y))
     match b with
     | Some box ->
       let dx, dy = e.X - box.X, e.Y - box.Y
       drag <- Some (box, dx, dy)
     | _ ->
       newbox <- Some ((e.X, e.Y), (e.X, e.Y))

  override this.OnMouseMove e =
    match newbox with
    | Some ((sx, sy), _) ->
      newbox <- Some((sx, sy), (e.X, e.Y))
      this.Invalidate()
    | _ -> ()
    match drag with
    | Some(box, dx, dy) ->
      box.Location <- Point(e.X - dx, e.Y - dy)
      this.Invalidate()
    | _ -> ()

   override this.OnMouseUp e =
     match newbox with
     | Some((sx, sy), (ex, ey)) ->
       let r = MovingBox(mkrect (sx, sy) (ex, ey))
       boxes.Add(r)
       newbox <- None
       this.Invalidate()
     | _ ->
       drag <- None

let f= new Form(Text="Test",TopMost=true)
let mc = new MouseTest(Dock=DockStyle.Fill)
f.Controls.Add(mc)
f.Show()
