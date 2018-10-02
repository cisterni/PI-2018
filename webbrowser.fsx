open System.Windows.Forms
let f = new Form(Text="WebBrowser")

let p = new Panel(Dock=DockStyle.Top)
let tb = new TextBox(Dock=DockStyle.Fill)
let b = new Button(Text="Go",Dock=DockStyle.Right)

p.Controls.Add(b)
p.Controls.Add(tb)
p.Height <- tb.Height
f.Controls.Add(p)


let wb = new WebBrowser(Dock=DockStyle.Fill)

b.Click.Add(fun _ ->
  wb.Navigate(tb.Text)
)

f.Controls.Add(wb)
wb.Navigate("about:blank")
f.Show()

