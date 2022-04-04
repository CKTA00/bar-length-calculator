# Bar length calculator
## Overview 
Bar length calculator is application capable of calculating optimal cut of bar with given length (main bar) to shorter bars, given their lengths and amount of each kind (called elements).
Multiple profiles can be included, each consisting of different main bar length and their sets of elements.
Application is aimed for enterprise use, with GUI created using Windows Presentation Foundation.

#### Application currently consists of MainWindow and NewProfileWindow
**MainWindow** is the main workspace of the application. It contains buttons that allow to perform calculations, add new profile, delete one and manipulate their elements displayed in a grid. There is also a large text box that contains results of calculations.

Image showing just opened application:

![img](https://user-images.githubusercontent.com/72377791/161540259-13369c76-c4ec-46aa-a72c-69a122b2595d.png)

MainWindow after filling data and performing calculation:

![img](https://user-images.githubusercontent.com/72377791/161540681-ad02e90e-1c5e-4ed8-b492-e211d77126c7.png)

For now the only supported language is polish.

**NewProfileWindow** is a simple dialog that will appear after clicking create new profile button. It allows to chose name and length of a profile or create new name and save it.

![img](https://user-images.githubusercontent.com/72377791/161541001-a1ab97a6-e0fd-424b-93ca-f696daa98a4a.png)

#### Buttons and their respective meanings:

<img
  src="/bar_length_calculator_project/img/prof_new.png"
  style="display: inline-block; margin: 0 auto; width: 50px">
  Create new profile button
  
  <img
  src="/bar_length_calculator_project/img/prof_del.png"
  style="display: inline-block; margin: 0 auto; width: 50px">
  Delete currently selected profile
  
  <img
  src="/bar_length_calculator_project/img/elem_plus.png"
  style="display: inline-block; margin: 0 auto; width: 50px">
  Add element to list
  
  <img
  src="/bar_length_calculator_project/img/elem_minus.png"
  style="display: inline-block; margin: 0 auto; width: 50px">
  Remove selected rows from list
  
  <img
  src="/bar_length_calculator_project/img/elem_clear.png"
  style="display: inline-block; margin: 0 auto; width: 50px">
  Clear entire list

### This project will have full documentation soon
