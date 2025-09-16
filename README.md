# CharMap Plus

An improved character map application, following Fluent design and adaptative triggers.

![CharMap window](Images/main-window.png)

## Key features

- Confortable view for displaying fonts and glyphs.
- Adaptative view for different window sizes.
- Fluent Design (WinUI 3)
- ... More upcoming features :)

## How it works

![Architecture](Images/arch.svg)

The application follows MVVM pattern so the main components are Views, View Models and Models. Also, it uses some external dependencies to retrieve the collection of fonts and glyphs.

### Views

The **Views** component contains the views, which will interact with the user.

### ViewModels

The **ViewModels** component contains Views' abstraction, they consist con classes with properties whch represent properties, status and commands used by the views.

### Models

Behind the scenes, the **Models** component includes the services with the logic to retrieve the collection of **fonts** and **glyphs**. That information is provided using two main libraries:

#### System.Drawing

This library is used by **FontService** to retrieve the list of fonts.

#### Vortice.DirectWrite

It is an abstraction of Microsoft DirectWrite libraries, and it is used to retrieve valid glyphs, skipping those that are not included in the font characters set.
