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

type BorrowRecord = {
    Title: string
    BorrowedBy: string
    BorrowedOn: DateTime
    ReturnedOn: DateTime option
}

let mutable borrowHistory = List.empty<BorrowRecord>
let mutable books = Map.empty<string, Book>  // Map to store books by title

// Library Management System
module Library =

    // Add a new book to the library
    let addBook title author genre =
        let newBook = { Title = title; Author = author; Genre = genre; IsBorrowed = false; BorrowDate = None }
        books <- books.Add(title, newBook)
        MessageBox.Show(sprintf "Book '%s' added successfully." title, "Success") |> ignore

    // Search for a book by title
    let searchBook title =
        match books.TryFind title with
        | Some book -> 
            MessageBox.Show(sprintf "Title: %s, Author: %s, Genre: %s, Status: %s"
                book.Title book.Author book.Genre (if book.IsBorrowed then "Borrowed" else "Available"), "Book Found") |> ignore
        | None -> 
            MessageBox.Show(sprintf "Book with title '%s' not found." title, "Error") |> ignore

    // Update borrowing history
    let updateBorrowHistory title user =
        borrowHistory <- { Title = title; BorrowedBy = user; BorrowedOn = DateTime.Now; ReturnedOn = None } :: borrowHistory

    // Update return history
    let updateReturnHistory title =
        borrowHistory <- 
            borrowHistory 
            |> List.map (fun record ->
                if record.Title = title && record.ReturnedOn.IsNone then
                    { record with ReturnedOn = Some DateTime.Now }
                else record)

    // Borrow a book
    let borrowBook title user =
        match books.TryFind title with
        | Some book when book.IsBorrowed -> 
            MessageBox.Show(sprintf "Book '%s' is already borrowed." title, "Error") |> ignore
        | Some book ->
            let updatedBook = { book with IsBorrowed = true; BorrowDate = Some DateTime.Now }
            books <- books.Add(title, updatedBook)
            updateBorrowHistory title user
            MessageBox.Show(sprintf "Book '%s' borrowed successfully." title, "Success") |> ignore
        | None -> 
            MessageBox.Show(sprintf "Book with title '%s' not found." title, "Error") |> ignore

    // Return a borrooowed booook
    let returnBook title =
        match books.TryFind title with
        | Some book when not book.IsBorrowed -> 
            MessageBox.Show(sprintf "Book '%s' is not currently borrowed." title, "Error") |> ignore
        | Some book ->
            let updatedBook = { book with IsBorrowed = false; BorrowDate = None }
            books <- books.Add(title, updatedBook)
            updateReturnHistory title
            MessageBox.Show(sprintf "Book '%s' returned successfully." title, "Success") |> ignore
        | None -> 
            MessageBox.Show(sprintf "Book with title '%s' not found." title, "Error") |> ignore

    // Display the borrow history
    let displayBorrowHistory () =
        if borrowHistory.IsEmpty then
            MessageBox.Show("No borrowing history available.", "History") |> ignore
        else
            let history =
                borrowHistory
                |> List.map (fun record ->
                    sprintf "Title: %s, Borrowed By: %s, Borrowed On: %s, Returned On: %s"
                        record.Title record.BorrowedBy
                        (record.BorrowedOn.ToShortDateString())
                        (match record.ReturnedOn with Some date -> date.ToShortDateString() | None -> "Not Returned"))
                |> String.concat "\n\n"
            MessageBox.Show(history, "Borrowing History") |> ignore

    // Check for overdue books
    let checkOverdueBooks () =
        let overdueDays = 14
        let overdueBooks =
            books
            |> Map.filter (fun _ book ->
                match book.BorrowDate with
                | Some borrowDate -> book.IsBorrowed && (DateTime.Now - borrowDate).Days > overdueDays
                | None -> false)
        if overdueBooks.IsEmpty then
            MessageBox.Show("No overdue books.", "Overdue Books") |> ignore
        else
            let message =
                overdueBooks
                |> Map.fold (fun acc _ book ->
                    acc + sprintf "Title: %s, Borrowed On: %s\n"
                        book.Title (book.BorrowDate.Value.ToShortDateString())) ""
            MessageBox.Show(message, "Overdue Books") |> ignore

    // Search books by author
    let searchByAuthor author =
        let results =
            books
            |> Map.filter (fun _ book -> book.Author.ToLower().Contains(author.ToLower()))
        if results.IsEmpty then
            MessageBox.Show(sprintf "No books found by author '%s'." author, "Search Result") |> ignore
        else
            let message =
                results
                |> Map.fold (fun acc _ book -> acc + sprintf "Title: %s, Genre: %s\n" book.Title book.Genre) ""
            MessageBox.Show(message, "Search Result") |> ignore

    // Search books by genre
    let searchByGenre genre =
        let results =
            books
            |> Map.filter (fun _ book -> book.Genre.ToLower().Contains(genre.ToLower()))
        if results.IsEmpty then
            MessageBox.Show(sprintf "No books found in genre '%s'." genre, "Search Result") |> ignore
        else
            let message =
                results
                |> Map.fold (fun acc _ book -> acc + sprintf "Title: %s, Author: %s\n" book.Title book.Author) ""
            MessageBox.Show(message, "Search Result") |> ignore

    // Export library data to a file
    let exportLibraryData () =
        let saveDialog = new SaveFileDialog(Filter = "Text Files|*.txt")
        if saveDialog.ShowDialog() = DialogResult.OK then
            let content =
                books
                |> Map.fold (fun acc _ book ->
                    acc + sprintf "Title: %s, Author: %s, Genre: %s, Status: %s\n"
                        book.Title book.Author book.Genre (if book.IsBorrowed then "Borrowed" else "Available")) ""
            System.IO.File.WriteAllText(saveDialog.FileName, content)
            MessageBox.Show(sprintf "Library data exported to %s" saveDialog.FileName, "Export Success") |> ignore

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

    btnDisplay.Click.Add(fun _ -> Library.displayBorrowHistory())
    btnExit.Click.Add(fun _ -> form.Close())

    // Add Buttons to Form
    form.Controls.AddRange([| btnAdd :> Control; btnSearch :> Control; btnBorrow :> Control; 
                              btnReturn :> Control; btnDisplay :> Control; btnExit :> Control |])
    
    Application.Run(form)
    0 


