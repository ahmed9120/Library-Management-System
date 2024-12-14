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


