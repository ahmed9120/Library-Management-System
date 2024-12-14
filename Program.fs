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

   
