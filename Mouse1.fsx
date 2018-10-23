open System.Windows.Forms
open System.Drawing

type MouseTest() as this =
  inherit UserControl()

  do
//    this.SetStyle(ControlStyles.AllPaintingInWmPaint ||| ControlStyles.OptimizedDoubleBuffer, true)
    this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true)

  let box = Rectangle(50, 50, 100, 100)
  let mutable start = None
  let duration = new System.TimeSpan(0,0,0,0,1000)
  let bgcolor = Color.Red
  let mutable alpha = 0.f
  let mutable inbox = false

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
    let a = int(alpha * 255.f)
    use c = new SolidBrush(Color.FromArgb(a, bgcolor))
    g.FillRectangle(c, box)
    g.DrawRectangle(Pens.Black, box)

//   override this.OnMouseDown e =
//     printfn "Down %A" e.Location

  override this.OnMouseMove e =
    let isin = box.Contains(e.X, e.Y)
    if isin <> inbox then
      inbox <- isin
      start <- None
      t.Start()

//   override this.OnMouseUp e =
//     printfn "Up %A" e.Location

let f= new Form(Text="Test",TopMost=true)
let mc = new MouseTest(Dock=DockStyle.Fill)
f.Controls.Add(mc)
f.Show()
