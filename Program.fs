open System
open System.Windows.Forms
open System.Collections.Generic

// Define a record type for Book
type Book = {
    Title: string
    Author: string
    Genre: string
    IsBorrowed: bool
    BorrowDate: DateTime option
}

// Library Management System
module Library =
    // A mutable collection of books (Map with Title as key)
    let mutable books = Map.empty<string, Book>

    // Add a new book
    let addBook title author genre =
        if books.ContainsKey title then
            MessageBox.Show(sprintf "Book with title '%s' already exists." title, "Error") |> ignore
        else
            let newBook = { Title = title; Author = author; Genre = genre; IsBorrowed = false; BorrowDate = None }
            books <- books.Add(title, newBook)
            MessageBox.Show(sprintf "Book '%s' added successfully." title, "Success") |> ignore

    // Search for a book by title
    let searchBook title =
        match books.TryFind title with
        | Some book ->
            MessageBox.Show(sprintf "Book Found:\nTitle: %s\nAuthor: %s\nGenre: %s\nStatus: %s"
                                book.Title book.Author book.Genre (if book.IsBorrowed then "Borrowed" else "Available"),
                            "Search Result") |> ignore
        | None -> 
            MessageBox.Show(sprintf "Book with title '%s' not found." title, "Error") |> ignore

    // Borrow a book
    let borrowBook title =
        match books.TryFind title with
        | Some book when book.IsBorrowed -> 
            MessageBox.Show(sprintf "Book '%s' is already borrowed." title, "Error") |> ignore
        | Some book ->
            let updatedBook = { book with IsBorrowed = true; BorrowDate = Some DateTime.Now }
            books <- books.Add(title, updatedBook)
            MessageBox.Show(sprintf "Book '%s' borrowed successfully." title, "Success") |> ignore
        | None -> 
            MessageBox.Show(sprintf "Book with title '%s' not found." title, "Error") |> ignore

    // Return a book
    let returnBook title =
        match books.TryFind title with
        | Some book when not book.IsBorrowed -> 
            MessageBox.Show(sprintf "Book '%s' is not currently borrowed." title, "Error") |> ignore
        | Some book ->
            let updatedBook = { book with IsBorrowed = false; BorrowDate = None }
            books <- books.Add(title, updatedBook)
            MessageBox.Show(sprintf "Book '%s' returned successfully." title, "Success") |> ignore
        | None -> 
            MessageBox.Show(sprintf "Book with title '%s' not found." title, "Error") |> ignore

    // Display all books with their status
    let displayBooks () =
        if books.IsEmpty then
            MessageBox.Show("No books in the library.", "Library") |> ignore
        else
            let allBooks =
                books
                |> Map.fold (fun acc _ book ->
                    acc + sprintf "Title: %s\nAuthor: %s\nGenre: %s\nStatus: %s\n\n"
                                book.Title book.Author book.Genre (if book.IsBorrowed then "Borrowed" else "Available")) ""
            MessageBox.Show(allBooks, "Library Books") |> ignore

// Helper Function for Input Box
let InputBox (prompt: string) =
    let form = new Form(Text = prompt, Width = 300, Height = 150, StartPosition = FormStartPosition.CenterParent)
    let label = new Label(Text = prompt, Top = 10, Left = 10, Width = 260)
    let textBox = new TextBox(Top = 40, Left = 10, Width = 260)
    let btnOk = new Button(Text = "OK", Top = 80, Left = 50, Width = 80, DialogResult = DialogResult.OK)
    let btnCancel = new Button(Text = "Cancel", Top = 80, Left = 150, Width = 80, DialogResult = DialogResult.Cancel)

    form.Controls.AddRange([| label :> Control; textBox :> Control; btnOk :> Control; btnCancel :> Control |])
    form.AcceptButton <- btnOk
    form.CancelButton <- btnCancel

    if form.ShowDialog() = DialogResult.OK then textBox.Text else null

// Main Program
[<EntryPoint>]
let main argv =
    let form = new Form(Text = "Library Management System", Width = 600, Height = 400)

    // Create Buttons
    let btnAdd = new Button(Text = "Add Book", Top = 10, Left = 10, Width = 150)
    let btnSearch = new Button(Text = "Search Book", Top = 50, Left = 10, Width = 150)
    let btnBorrow = new Button(Text = "Borrow Book", Top = 90, Left = 10, Width = 150)
    let btnReturn = new Button(Text = "Return Book", Top = 130, Left = 10, Width = 150)
    let btnDisplay = new Button(Text = "Display Books", Top = 170, Left = 10, Width = 150)
    let btnExit = new Button(Text = "Exit", Top = 210, Left = 10, Width = 150)

    // Add Event Handlers
    btnAdd.Click.Add(fun _ ->
        let title = InputBox "Enter Book Title"
        let author = InputBox "Enter Author"
        let genre = InputBox "Enter Genre"
        if not (String.IsNullOrWhiteSpace title) &&
           not (String.IsNullOrWhiteSpace author) &&
           not (String.IsNullOrWhiteSpace genre) then
            Library.addBook title author genre
    )

    btnSearch.Click.Add(fun _ ->
        let title = InputBox "Enter Book Title"
        if not (String.IsNullOrWhiteSpace title) then
            Library.searchBook title
    )

    btnBorrow.Click.Add(fun _ ->
        let title = InputBox "Enter Book Title"
        if not (String.IsNullOrWhiteSpace title) then
            Library.borrowBook title
    )

    btnReturn.Click.Add(fun _ ->
        let title = InputBox "Enter Book Title"
        if not (String.IsNullOrWhiteSpace title) then
            Library.returnBook title
    )

    btnDisplay.Click.Add(fun _ -> Library.displayBooks())
    btnExit.Click.Add(fun _ -> form.Close())

    // Add Buttons to Form
    form.Controls.AddRange([| btnAdd :> Control; btnSearch :> Control; btnBorrow :> Control; 
                              btnReturn :> Control; btnDisplay :> Control; btnExit :> Control |])
    
    Application.Run(form)
    0
