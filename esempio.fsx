open System.Windows.Forms

let f = new Form(Text="Hello World")
// Equivale a 
//let f = new Form()
//f.Text <- "Hello World"

let b = new Button(Text="Ok")
f.Controls.Add(b)
b.Click.Add(fun _ ->
  MessageBox.Show("Pippo") |> ignore
)
f.Show()
