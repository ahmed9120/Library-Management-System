open System
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
            printfn "Book with title '%s' already exists." title
        else
            let newBook = { Title = title; Author = author; Genre = genre; IsBorrowed = false; BorrowDate = None }
            books <- books.Add(title, newBook)
            printfn "Book '%s' added successfully." title

    // Search for a book by title
    let searchBook title =
        match books.TryFind title with
        | Some book ->
            printfn "Book Found - Title: %s, Author: %s, Genre: %s, Status: %s"
                    book.Title book.Author book.Genre (if book.IsBorrowed then "Borrowed" else "Available")
        | None -> 
            printfn "Book with title '%s' not found." title

    // Borrow a book
    let borrowBook title =
        match books.TryFind title with
        | Some book when book.IsBorrowed -> 
            printfn "Book '%s' is already borrowed." title
        | Some book ->
            let updatedBook = { book with IsBorrowed = true; BorrowDate = Some DateTime.Now }
            books <- books.Add(title, updatedBook)
            printfn "Book '%s' borrowed successfully on %s." title (updatedBook.BorrowDate.Value.ToString("yyyy-MM-dd"))
        | None -> 
            printfn "Book with title '%s' not found." title

    // Return a book
    let returnBook title =
        match books.TryFind title with
        | Some book when not book.IsBorrowed -> 
            printfn "Book '%s' is not currently borrowed." title
        | Some book ->
            let updatedBook = { book with IsBorrowed = false; BorrowDate = None }
            books <- books.Add(title, updatedBook)
            printfn "Book '%s' returned successfully." title
        | None -> 
            printfn "Book with title '%s' not found." title

    // Display all books with their status
    let displayBooks () =
        if books.IsEmpty then
            printfn "No books in the library."
        else
            printfn "Library Books"
            books |> Map.iter (fun _ book ->
                printfn "Title: %s, Author: %s, Genre: %s, Status: %s"
                        book.Title book.Author book.Genre (if book.IsBorrowed then "Borrowed" else "Available")
            )

// Main Program
[<EntryPoint>]
let main argv =
    let running = ref true
    while !running do
        printfn "\nLibrary Management System"
        printfn "1. Add Book"
        printfn "2. Search Book"
        printfn "3. Borrow Book"
        printfn "4. Return Book"
        printfn "5. Display All Books"
        printfn "6. Exit"
        printf "Enter your choice: "
        
        let choice = Console.ReadLine()
        match choice with
        | "1" ->
            printf "Enter Title: "
            let title = Console.ReadLine()
            printf "Enter Author: "
            let author = Console.ReadLine()
            printf "Enter Genre: "
            let genre = Console.ReadLine()
            Library.addBook title author genre
        | "2" ->
            printf "Enter Title: "
            let title = Console.ReadLine()
            Library.searchBook title
        | "3" ->
            printf "Enter Title: "
            let title = Console.ReadLine()
            Library.borrowBook title
        | "4" ->
            printf "Enter Title: "
            let title = Console.ReadLine()
            Library.returnBook title
        | "5" ->
            Library.displayBooks()
        | "6" ->
            running := false
            printfn "Exiting Library Management System. Goodbye!"
        | _ ->
            printfn "Invalid choice. Please try again."
    0  // Exit code
